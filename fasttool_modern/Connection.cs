using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fasttool_modern
{

    public class Connection
    {
        private Connection connection;
        private SerialPort serialPort;
        public event Action<string> DataReceived;
        string portName = "COM7"; // Ustaw odpowiedni port COM
        int baudRate = 115200;

        private static Connection instance;

        private Connection() { }

        public static Connection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Connection();
                }
                return instance;
            }
        }


        public void connectDevice()
        {
            serialPort = new SerialPort(portName, baudRate);

            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open(); // Otwórz port
                    getInfoDevice();
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

        public bool askConnection()
        {
            if (serialPort.IsOpen && serialPort != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void comSend(string message)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.WriteLine(message);
            }
        }

        public void getInfoDevice()
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

        public void connect()
        {
            serialPort.Open();
        }
        public void disconnect()
        {
            serialPort.Close();
        }

        public string getPortName()
        {
            return serialPort.PortName;
        }
    }
}
