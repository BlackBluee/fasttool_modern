using Microsoft.UI.Xaml;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using fasttool_modern.Services;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using AudioSwitcher.AudioApi.CoreAudio;
using Windows.System;
using Persistance;
using System.Linq;
using Windows.UI.Input.Preview.Injection;
using Microsoft.UI.Xaml.Controls;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq.Expressions;





namespace fasttool_modern
{
    public partial class App : Application
    {
        public string previousProcessName { get; set; } = "";
        public List<string> availableApps = new List<string>();

        private System.Timers.Timer windowCheckTimer;

        // Inicjalizacja kontrolera audio
        private const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
        private const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
        private const int APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
        private const int WM_APPCOMMAND = 0x0319;
        private CancellationTokenSource cancellationTokenSource;
        private CoreAudioController audioController;
        private CoreAudioDevice defaultPlaybackDevice;
        private System.Timers.Timer deviceCheckTimer;

        SerialPortManager serialPortManager = SerialPortManager.Instance;
        public App()
        {
            this.InitializeComponent();

            ActiveWindowTracker tracker = new ActiveWindowTracker();
            tracker.StartTracking();

            StartConnectionChecker();
            // Inicjalizacja timera - jeśli potrzebujesz timera w środowisku niewizualnym, wielowątkowym lub w aplikacji konsolowej.
            deviceCheckTimer = new System.Timers.Timer();
            deviceCheckTimer.Interval = 1000; // Sprawdzanie co sekundę
            deviceCheckTimer.Elapsed += OnDeviceCheckTimerElapsed;
            deviceCheckTimer.Start();
            audioController = new CoreAudioController();
            defaultPlaybackDevice = audioController.DefaultPlaybackDevice;
            serialPortManager.ConnectDevice();
            serialPortManager.DataReceived += OnDataReceived;

        }
        

        private void OnDataReceived(string data)
        {
            ProcessResponse(data);           
        }

        private void ProcessResponse(string response)
        {
            // Rozdzielenie i przetworzenie otrzymanych informacji
            string[] parts = response.Split(',');
            Dictionary<string, string> deviceInfo = new Dictionary<string, string>();
            foreach (string part in parts)
            {
                string[] keyValue = part.Split(':');
                if (keyValue.Length == 2)
                {
                    deviceInfo[keyValue[0]] = keyValue[1];
                }
            }
            if (deviceInfo.ContainsKey("Type"))
            {
                string type = deviceInfo["Type"];
                if (type == "DeviceInfo")
                {
                    var did = "1";
                    var model = deviceInfo.ContainsKey("Model") ? deviceInfo["Model"] : "Unknown";
                    var version = deviceInfo.ContainsKey("Version") ? int.Parse(deviceInfo["Version"]) : 0;
                    //var version = "1.0";
                    var port = serialPortManager.GetPortName();
                    try
                    {
                        using (var context = new AppDbContext())
                        {
                            context.Database.EnsureCreated();
                            context.Devices.Add(new Device { DeviceID = did, Model = model, Version = version, Port = port });
                            context.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
                else if (type == "ButtonPress")
                {

                    string pressedButton = deviceInfo.ContainsKey("Button") ? deviceInfo["Button"].Trim() : string.Empty;


                    // Wyszukanie danych przycisku na podstawie numeru przycisku
                    using (var context = new AppDbContext())
                    {
                        var profile = context.Profiles.FirstOrDefault(p => p.ProfileName == previousProcessName);
                        var buttonData = context.ButtonDatas
                            .Where(b => b.ButtonID == pressedButton && b.ProfileID == "HOME")
                            .SingleOrDefault();
                        var action = buttonData == null ? null : context.Actions.SingleOrDefault(a => a.ActionID == buttonData.ActionID);



                        if (!object.ReferenceEquals(buttonData, null))
                        {
                            if (action.Type == "open app")
                            {

                                try
                                {
                                    Process.Start(action.DoAction);
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show($"Błąd uruchamiania aplikacji: {ex.Message}");
                                }
                                
                            }
                            else if (action.Type == "hotkey")
                            {
                                //TaskManager.SimulateKeyPress(action.DoAction);
                            }
                            else if (action.Type == "multimedia")
                            {
                                //obsługa przycisków multimedialnych
                                switch (action.DoAction)
                                {
                                    case "play/pause":
                                        TaskManager.SendMediaKey(APPCOMMAND_MEDIA_PLAY_PAUSE);
                                        break;
                                    case "mute/unmute":
                                        defaultPlaybackDevice.Mute(!defaultPlaybackDevice.IsMuted);
                                        break;
                                    case "next":
                                        TaskManager.SendMediaKey(APPCOMMAND_MEDIA_NEXTTRACK);
                                        break;
                                    case "previous":
                                        TaskManager.SendMediaKey(APPCOMMAND_MEDIA_PREVIOUSTRACK);
                                        break;
                                    case "volume up":
                                        ++defaultPlaybackDevice.Volume;
                                        break;
                                    case "volume down":
                                        --defaultPlaybackDevice.Volume;
                                        break;
                                    default:
                                        //MessageBox.Show($"Nieobsługiwana akcja multimedialna: {buttonData.Action}", "Błąd");
                                        break;
                                }
                            }
                            
                        }
                        
                        


                    else if (type == "winSound")
                        {
                            int valueSound = deviceInfo.ContainsKey("Value") ? int.Parse(deviceInfo["Value"]) : 0;
                            defaultPlaybackDevice.Volume = valueSound;
                        }
                        else if (type == "winBrightness")
                        {
                            int valueBrightness = deviceInfo.ContainsKey("Value") ? int.Parse(deviceInfo["Value"]) : 0;

                        }
                        else
                        {
                            //MessageBox.Show($"Nieobsługiwany typ wiadomości: {type}", "Błąd");
                        }
                    }
                }

            }
        }

        private void StartConnectionChecker()
        {
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (serialPortManager.AskConnection())
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

        //do sprawdzenia zmiany urządzenia audio
        private void OnDeviceCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (audioController != null)
            {
                var currentDefaultDevice = audioController.DefaultPlaybackDevice;

                if (currentDefaultDevice.Id != defaultPlaybackDevice.Id)
                {
                    defaultPlaybackDevice = currentDefaultDevice;
                    Console.WriteLine("Domyślne urządzenie odtwarzania zmienione.");
                }
            }
        }
        

        
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
           
            m_window = new MainWindow();
            m_window.Activate();
        }

     

        private Window m_window;
    }
}
