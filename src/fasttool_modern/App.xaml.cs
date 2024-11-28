using Microsoft.UI.Xaml;
using fasttool_modern.Services;


namespace fasttool_modern
{
    public partial class App : Application
    {
        private Window m_window;

        ActiveWindowTracker _activeWindowTracker = ActiveWindowTracker.Instance;
        DeviceSearcher _deviceSearcher = DeviceSearcher.Instance;
        AudioDeviceMonitor _audioDeviceMonitor = AudioDeviceMonitor.Instance;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
            _activeWindowTracker.Start();
            _deviceSearcher.Start();
            _audioDeviceMonitor.Start();
        }
    }
}
