using System;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using fasttool_modern.Services.Interfaces;

namespace fasttool_modern.Services
{
    
    public class SerialPortManager : IConnectionManager
    {

        private SerialPort serialPort;
        public event Action<string> DataReceived;
        string devicePortName = string.Empty;
        readonly int baudRate = 115200;

        private static SerialPortManager instance;

        private SerialPortManager() {}

        public static SerialPortManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SerialPortManager();
                    var responder = SerialPortResponder.Instance;
                }
                return instance;
            }
        }

        public async Task<string> FindDeviceAsync()
        {
            string[] ports = SerialPort.GetPortNames();
            
                foreach (var port in ports)
                {
                    string result = await Task.Run(() => IdentifyDevice(port));
                    if (result != null)
                    {
                        return result;
                    }
                }
                
            
            return null;
        }

        private string IdentifyDevice(string portName)
        {
            try
            {
                using (var tserialPort = new SerialPort(portName, baudRate))
                {
                    tserialPort.ReadTimeout = 100;
                    tserialPort.Open();
                    tserialPort.WriteLine("HELLO");
                    string response = tserialPort.ReadLine();
                    if (response.Contains("FASTPANEL"))
                    {
                        devicePortName = portName;
                        serialPort = tserialPort;
                        tserialPort.Close();
                        serialPort = new SerialPort(devicePortName, baudRate);
                        serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                        serialPort.Open();
                        return portName;
                    }
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine($"Timeout na porcie {portName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd na porcie {portName}: {ex.Message}");
            }
            devicePortName = "";
            return null;
        }

        public void ConnectDevice(string port)
        {
            devicePortName = port;
            serialPort = new SerialPort(devicePortName, baudRate);

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
            try
            {
                if (serialPort.IsOpen)
                {
                    string response = serialPort.ReadLine();
                    DataReceived?.Invoke(response);
                }
            }
            catch (IOException ex)
            {
                // Handle the exception (e.g., log the error, attempt to reconnect, etc.)
                Console.WriteLine($"IOException: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                // Handle the exception (e.g., log the error, ensure the port is open, etc.)
                Console.WriteLine($"InvalidOperationException: {ex.Message}");
            }
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
            return devicePortName;
        }
    }
}
