using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using System.Timers;
using System.Runtime.InteropServices;
using Windows.Foundation.Collections;
using System.Threading;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Diagnostics;
using System.Threading.Tasks;
using fasttool_modern.Shared;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace fasttool_modern
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private string previousProcessName = "";
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
        private System.Timers.Timer windowCheckTimer;
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
            //windowCheckTimer.Tick += new EventHandler(CheckActiveWindow);
            //windowCheckTimer.Start();

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

        private void CheckActiveWindow(object sender, EventArgs e)
        {
            // Pobranie uchwytu aktywnego okna
            IntPtr handle = GetForegroundWindow();

            // Pobranie ID procesu związanego z aktywnym oknem
            GetWindowThreadProcessId(handle, out uint processId);

            try
            {
                // Pobranie nazwy procesu
                Process process = Process.GetProcessById((int)processId);
                string currentProcessName = process.ProcessName;

                // Aktualizacja etykiety, jeśli nazwa procesu się zmieniła
                if (currentProcessName != previousProcessName)
                {
                    

                    previousProcessName = currentProcessName;
                    

                    //Type:AcitveApp,Name: chrome
                    connection.comSend("Type:ActiveApp,Name:" + currentProcessName);

                }
            }
            catch (ArgumentException)
            {
                // Proces mógł zostać zakończony, zresetuj nazwę procesu
                previousProcessName = "";
                
            }
        }

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
