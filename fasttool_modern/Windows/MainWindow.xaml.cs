using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Threading;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Diagnostics;
using System.Threading.Tasks;
using fasttool_modern.Shared;




namespace fasttool_modern
{
    public sealed partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        
        //private ContextMenuStrip trayMenu; //menu kontekstowe

        // Inicjalizacja kontrolera audio
        private const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
        private const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
        private const int APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
        private const int WM_APPCOMMAND = 0x0319;
        private CancellationTokenSource cancellationTokenSource;
        private CoreAudioController audioController;
        private CoreAudioDevice defaultPlaybackDevice;
        private System.Timers.Timer deviceCheckTimer;
        
        public string selectedImage = "";

        Connection connection = Connection.Instance;
        public MainWindow()
        {
            this.InitializeComponent();
            NavView.SelectedItem = NavView.MenuItems[0];
            audioController = new CoreAudioController();
            defaultPlaybackDevice = audioController.DefaultPlaybackDevice;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            connection.connectDevice();
            StartConnectionChecker();
            // Inicjalizacja timera - jeśli potrzebujesz timera w środowisku niewizualnym, wielowątkowym lub w aplikacji konsolowej.
            deviceCheckTimer = new System.Timers.Timer();
            deviceCheckTimer.Interval = 1000; // Sprawdzanie co sekundę
            deviceCheckTimer.Elapsed += OnDeviceCheckTimerElapsed;
            deviceCheckTimer.Start();
            // Inicjalizacja timera - jeśli pracujesz w aplikacji Windows Forms i potrzebujesz okresowo aktualizować UI.
            

        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = args.SelectedItem as NavigationViewItem;
            var pageTag = selectedItem.Tag.ToString();

            switch (pageTag)
            {
                case "DashboardPage":
                    ContentFrame.Navigate(typeof(DashboardPage));
                    NavView.Header = "Dashboard";
                    break;

                case "ProfilesPage":
                    ContentFrame.Navigate(typeof(ProfilesPage));
                    NavView.Header = "Profiles";
                    break;

                case "ConnectionPage":
                    ContentFrame.Navigate(typeof(ConnectionPage));
                    NavView.Header = "Connection";
                    break;

                case "SettingsPage":
                    ContentFrame.Navigate(typeof(SettingsPage));
                    NavView.Header = "Settings";
                    break;
            }
        }
        /*private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }*/

        

        //Sprawdzanie połączenia z urządzeniem
        private void StartConnectionChecker()
        {
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (connection.askConnection())
                    {
                        //statusLabel.Text = "Device: connected";
                        

                        //btnIconButton.Image = Properties.Resources.nazwa_ikony;
                    }
                    else
                    {
                        //statusLabel.Text = "Device: disconnected";
                        
                    }
                    Console.WriteLine("Sprawdzanie w tle...");
                    Thread.Sleep(1000); // Opóźnienie 1 sekunda
                }
            }, cancellationTokenSource.Token);
        }

        private void StopConnectionChecker()
        {
            cancellationTokenSource?.Cancel();
        }

        private static void SendMediaKey(int key)
        {
            IntPtr hWnd = GetForegroundWindow();
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)(key << 16));
        }

        //do sprawdzenia zmiany urządzenia audio
        private void OnDeviceCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var currentDefaultDevice = audioController.DefaultPlaybackDevice;

            if (currentDefaultDevice.Id != defaultPlaybackDevice.Id)
            {
                defaultPlaybackDevice = currentDefaultDevice;
                Console.WriteLine("Domyślne urządzenie odtwarzania zmienione.");
            }
        }
        private void RunExternalApplication(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Błąd uruchamiania aplikacji: {ex.Message}");
            }
        }
    }
}
