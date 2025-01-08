using Microsoft.UI.Xaml;
using fasttool_modern.Services;


namespace fasttool_modern
{
    public partial class App : Application
    {
        public static Window MainWindow { get; private set; }

        ActiveWindowTracker _activeWindowTracker = ActiveWindowTracker.Instance;
        DeviceSearcher _deviceSearcher = DeviceSearcher.Instance;
        AudioDeviceMonitor _audioDeviceMonitor = AudioDeviceMonitor.Instance;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            MainWindow.Activate();
            InitializeServices();
        }
        private void InitializeServices()
        {
            _activeWindowTracker.Start();
            _deviceSearcher.Start();
            _audioDeviceMonitor.Start();
        }
    }
}
