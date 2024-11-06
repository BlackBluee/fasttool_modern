using fasttool_modern.Services.Interfaces;

namespace fasttool_modern.Services
{
    public class BluetoothManager : IConnectionManager
    {
        public BluetoothManager() { }

        public void ConnectDevice()
        {

        }
        public void DisconnectDevice() {

        }
        public void Send(string message)
        {

        }
        public void GetInfoDevice()
        {

        }
        public bool AskConnection()
        {
            throw new System.NotImplementedException();
        }
        public void Connect()
        {

        }
        public void Disconnect()
        {

        }

        public event System.Action<string> DataReceived;
    }
}
