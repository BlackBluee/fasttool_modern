using System.Timers;
using fasttool_modern.Services.Interfaces;

namespace fasttool_modern.Services
{
    public class DeviceSearcher : IBackgroundTask
    {
        SerialPortManager _serialPortManager = SerialPortManager.Instance;
        private static DeviceSearcher instance;
        private System.Timers.Timer checkTimer;
        public DeviceSearcher()
        {
            checkTimer = new Timer(2000);
            checkTimer.Elapsed += OnConnectionCheckTimerElapsed;
        }
        public static DeviceSearcher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DeviceSearcher();
                }
                return instance;
            }
        }

        private void OnConnectionCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {

            if (_serialPortManager.AskConnection())
            {
                checkTimer.Stop();
                checkTimer.Dispose();
            }
            else
            {
                if (_serialPortManager.FindDeviceAsync() == null)
                {
                    checkTimer.Stop();
                    checkTimer.Dispose();
                }
            }
        }

        public void Start()
        {
            checkTimer.Start();
        }
        public void Stop()
        {
            checkTimer.Stop();
        }
    }
    
}
