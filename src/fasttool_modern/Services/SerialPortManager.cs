using System;
using System.IO;
using System.IO.Ports;
using fasttool_modern.Services.Interfaces;

namespace fasttool_modern.Services
{
    
    public class SerialPortManager : IConnectionManager
    {
        private SerialPortManager _connection;
        private SerialPort serialPort;
        public event Action<string> DataReceived;
        string portName = "COM19"; // Ustaw odpowiedni port COM
        int baudRate = 115200;

        private static SerialPortManager instance;

        private SerialPortManager() { }

        public static SerialPortManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SerialPortManager();
                }
                return instance;
            }
        }


        public void ConnectDevice()
        {
            serialPort = new SerialPort(portName, baudRate);

            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open(); // Otwórz port
                    GetInfoDevice();
                }
                else
                {
                    //MessageBox.Show("Port jest już otwarty.");
                }
            }
            catch (UnauthorizedAccessException)
            {
                //MessageBox.Show("Błąd: Port jest już używany przez inną aplikację.");
            }
            catch (IOException ioEx)
            {
                //MessageBox.Show("Błąd: Problem z wejściem/wyjściem - " + ioEx.Message);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Błąd: " + ex.Message);
            }


        }

        public bool AskConnection() => serialPort != null && serialPort.IsOpen ? true : false;
       

        public void Send(string message)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.WriteLine(message);
            }
        }

        public void GetInfoDevice()
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("InfoDevice");
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string response = serialPort.ReadLine();
            // Wywołanie zdarzenia, gdy dane zostaną odebrane
            DataReceived?.Invoke(response);
        }

        public void Connect()
        {
            serialPort.Open();
        }
        public void Disconnect()
        {
            serialPort.Close();
        }

        public string GetPortName()
        {
            return serialPort.PortName;
        }
    }
}
