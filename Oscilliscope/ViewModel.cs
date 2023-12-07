using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.ExceptionServices;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Maui;

namespace Oscilliscope;
public partial class ViewModel : ObservableObject
{

    private static ObservableCollection<ObservablePoint> observableValues = new ObservableCollection<ObservablePoint>();
    private bool bPortOpen = false;
    private string sendPacket = "";
    const int samples = 32;
    static double[] voltages = new double[samples];
    Random random = new Random();

    public ObservableCollection<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
    {
        new LineSeries<ObservablePoint>
        {
            Values = observableValues,
            Fill = null,
            GeometrySize = 0,
            LineSmoothness = 0
        }
    };

    [ObservableProperty]
    string comPortButtonText = "Open";

    [ObservableProperty]
    string simulatedButtonText = "Real";

    [ObservableProperty]
    List<string> comPorts = new List<string>();

    [ObservableProperty]
    string selectedPort = SerialPort.GetPortNames()[0];

    [ObservableProperty]
    string newPacket = "###0000";

    [ObservableProperty]
    string currentStatus = "Initializing";

    [ObservableProperty]
    string frequencyText = "0 Hz";

    [ObservableProperty]
    string peakText = "0 Vpp";

    [ObservableProperty]
    string minText = "0 V";

    [ObservableProperty]
    string maxText = "0 V";

    SerialPort serialPort = new SerialPort();

    public ViewModel()
    {
        for(int i = 0; i <samples; i++)
        {
            observableValues.Add(new ObservablePoint(i, 0));
        }
        serialPort.BaudRate = 115200;
        serialPort.ReceivedBytesThreshold = 1;
        serialPort.DataReceived += SerialPort_DataReceived;

        foreach (string port in SerialPort.GetPortNames())
        {
            comPorts.Add(port);
        }
        //comPorts = ports;
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        NewPacket = serialPort.ReadLine();
        CurrentStatus = "Packet Recieved";
        try
        {

            if (NewPacket.Substring(0, 3) == "###")
            {
                CurrentStatus = "Parsing Packet";
                int currPlace = 0;
                float currMax = -100;
                float currMin = 100;
                for (int i = 6; i < ((samples + 1) * 4); i += 4)
                {
                    try
                    {
                        observableValues[currPlace].Y = map((Int32.Parse(NewPacket.Substring(i, 4))), 778, 1052, -9.4f, 23.2f) ;
                        observableValues[currPlace].X = currPlace * 0.025f;
                        if (observableValues[currPlace].Y > currMax)
                        {
                            currMax = (float)observableValues[currPlace].Y;
                        }
                        if (observableValues[currPlace].Y < currMin)
                        {
                            currMin = (float)observableValues[currPlace].Y;
                        }
                    }
                    catch (Exception err)
                    {
                        CurrentStatus = "Error: " + err;
                    }
                    currPlace++;
                };
                MaxText = currMax.ToString("0.00") + " V";
                MinText = currMin.ToString("0.00") + " V";
                PeakText = (currMax - currMin).ToString("0.00" + " Vpp");

                ObservablePoint temp1 = null;
                ObservablePoint temp2 = null;
                float period = 0;
                for(int i = 0; i < currPlace; i++)
                {
                    try
                    {
                        if ((observableValues[i - 1].Y < observableValues[i].Y) & (observableValues[i - 1].Y < 0) & (observableValues[i].Y > 0))
                        {
                            if(temp1 == null)
                            {
                                temp1 = observableValues[i];
                            }
                            else
                            {
                                temp2 = observableValues[i];
                                period = (float)temp2.X - (float)temp1.X;
                                break;
                            }
                        }
                    }
                    catch(Exception err) { }
                }
                FrequencyText = (1f / period).ToString("0.0") + " Hz";
            }
        }
        catch(Exception err)
        {
            CurrentStatus = "Error: " + err;
        }
        byte[] messageBytes = Encoding.UTF8.GetBytes(sendPacket);
        try
        {
            serialPort.Write(messageBytes, 0, messageBytes.Length);

        }
        catch (Exception err)
        {
            CurrentStatus = "Error: " + err;
        }
    }

    [RelayCommand]
    void OpenClose()
    {
        if (!bPortOpen)
        {
            //Opens selected port, changes button action to "Close"
            serialPort.PortName = SelectedPort;
            try
            {
                serialPort.Open();
            }
            catch(Exception ex)
            {
                CurrentStatus = (("Error: "+ ex.ToString));
            }
            ComPortButtonText = "Close";
            bPortOpen = true;
        }
        else
        {
            //Closes selected port, changes button action to "Open"
            serialPort.Close();
            ComPortButtonText = "Open";
            bPortOpen = false;
        }
    }

    [RelayCommand]
    void Simulate()
    {
        CurrentStatus = "SendingCommand";
        if(SimulatedButtonText == "Real")
        {
            sendPacket = "###0000192/r/n";
            SimulatedButtonText = "Simulated";

        }
        else
        {
            sendPacket = "###1111196/r/n";
            SimulatedButtonText = "Real";
        }
    }
    private static float map(int value, int fromLow, int fromHigh, float toLow, float toHigh)
    {
        return ((float)value - (float)fromLow) * (toHigh - toLow) / ((float)fromHigh - (float)fromLow) + toLow;
    }
}

