using AudioSwitcher.AudioApi.CoreAudio;
using Persistance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fasttool_modern.Services
{
    public class SerialPortResponder
    {
        private static SerialPortResponder instance;
        private SerialPortManager _serialPortManager = SerialPortManager.Instance;
        AudioDeviceMonitor _audioDeviceMonitor = AudioDeviceMonitor.Instance;
        ActiveWindowTracker _activeWindowTracker = ActiveWindowTracker.Instance;
        
        private const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
        private const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
        private const int APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
        private CoreAudioDevice defaultPlaybackDevice;
        public string previousProcessName { get; set; } = string.Empty;
        private SerialPortResponder() 
        {
            _serialPortManager.DataReceived += OnDataReceived;

        }
        public static SerialPortResponder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SerialPortResponder();
                }
                return instance;
            }
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
                    var port = _serialPortManager.GetPortName();
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
                    string pressedButton = string.Empty;
                    if (deviceInfo.ContainsKey("AButton"))
                    {
                        pressedButton = deviceInfo["AButton"].Trim();
                    }
                    else if (deviceInfo.ContainsKey("Button"))
                    {
                        pressedButton = deviceInfo["Button"].Trim();
                    } 
                    

                    string processName = _activeWindowTracker.GetCurrentProcessName();
                    // Wyszukanie danych przycisku na podstawie numeru przycisku
                    using (var context = new AppDbContext())
                    {
                        string profileID = string.Empty;
                        var profile = context.Profiles.FirstOrDefault(p => p.ProfileName == processName);
                        
                        if (deviceInfo.ContainsKey("Button"))
                        {
                            profileID = "HOME";
                        }
                        else if (deviceInfo.ContainsKey("AButton") && profile != null)
                        {
                            profileID = profile.ProfileID;
                        }
                        var buttonData = context.ButtonDatas
                            .Where(b => b.ButtonID == pressedButton && b.ProfileID == profileID)
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
                                defaultPlaybackDevice = _audioDeviceMonitor.GetDefaultPlaybackDevice();
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
    }
}
