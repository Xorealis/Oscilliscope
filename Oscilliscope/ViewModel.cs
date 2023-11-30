using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace Oscilliscope;

[ObservableObject]
public partial class ViewModel
{
    private static ObservableCollection<ObservableValue> observableValues;
    private bool bPortOpen = false;
    private string newPacket = "";
    private string sendPacket = "";
    const int samples = 32;
    private float[] voltages = new float[samples];

    [ObservableProperty]
    public string comPortButtonText = "Open";

    [ObservableProperty]
    public string[] comPorts;

    [ObservableProperty]
    public string selectedPort = "";

    SerialPort serialPort = new SerialPort();

    public ViewModel()
    {
        serialPort.BaudRate = 115200;
        serialPort.ReceivedBytesThreshold = 1;
        serialPort.DataReceived += SerialPort_DataReceived;

        observableValues = new ObservableCollection<ObservableValue> { };
        string[] ports = SerialPort.GetPortNames();
        comPorts = ports;
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        newPacket = serialPort.ReadLine();

        if (newPacket.Substring(0, 3) == "###")
        {
            int currPlace = 0;
            for (int i = 6; i < (samples * 4); i += 4)
            {
                voltages[currPlace] = Int32.Parse(newPacket.Substring(i, 4)) / 4096.0f;
                currPlace++;
            }
        }
    }

    [RelayCommand]
    void OpenClose()
    {
        if (!bPortOpen)
        {
            //Opens selected port, changes button action to "Close"
            serialPort.PortName = SelectedPort;
            serialPort.Open();
            comPortButtonText = "Close";
            bPortOpen = true;
        }
        else
        {
            //Closes selected port, changes button action to "Open"
            serialPort.Close();
            comPortButtonText = "Open";
            bPortOpen = false;
        }
    }


    public ISeries[] Series { get; set; } =
    {
        new LineSeries<ObservableValue>
        {
            Values = observableValues,
            Fill = null,
            GeometrySize = 0,
            LineSmoothness = 0
        }
    };
}

