﻿using System;
using System.IO.Ports;
using System.Text;
namespace Oscilliscope
{
    public partial class MainPage : ContentPage
    {
        private bool bPortOpen = false;
        private string newPacket = "";
        private string sendPacket = "";
        const int samples = 32;

        SerialPort serialPort = new SerialPort();

        public MainPage()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            portPicker.ItemsSource = ports;
            portPicker.SelectedIndex = ports.Length;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, EventArgs e)
        {
            serialPort.BaudRate = 115200;
            serialPort.ReceivedBytesThreshold = 1;
            serialPort.DataReceived += SerialPort_DataRecieved;
        }

        private void SerialPort_DataRecieved(object sender, EventArgs e)
        {
            newPacket = serialPort.ReadLine();
            MainThread.BeginInvokeOnMainThread(MyMainCodeThread);
        }

        private void MyMainCodeThread()
        {

        }
        private void Button1_Clicked(object sender, EventArgs e)
        {

        }

        private void Button2_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnOpenClose_Clicked(object sender, EventArgs e)
        {
            if (!bPortOpen)
            {
                //Opens selected port, changes button action to "Close"
                serialPort.PortName = portPicker.SelectedItem.ToString();
                serialPort.Open();
                BtnOpenClose.Text = "Close";
                bPortOpen = true;
            }
            else
            {
                //Closes selected port, changes button action to "Open"
                serialPort.Close();
                BtnOpenClose.Text = "Open";
                bPortOpen = false;
            }
        }
    }
}