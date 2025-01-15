using AudioSwitcher.AudioApi.CoreAudio;
using fasttool_modern.Helpers;
using Persistance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fasttool_modern.Services
{
    public class SerialPortResponder
    {
        private static SerialPortResponder instance;
        private SerialPortManager _serialPortManager = SerialPortManager.Instance;
        ActiveWindowTracker _activeWindowTracker = ActiveWindowTracker.Instance;
        KeyShortcutRecorder _keyShortcutRecorder = KeyShortcutRecorder.Instance;
        private CoreAudioDevice defaultPlaybackDevice;
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
            var actionHandlers = new Dictionary<string, Action<string>>()
            {
                { "open app", TaskManager.RunExternalApplication },
                { "hotkey", _keyShortcutRecorder.PlayRecordedShortcut },
                { "multimedia", TaskManager.MultimediaCommand }
            };

            // Rozdzielenie i przetworzenie otrzymanych informacji
            string[] parts = response.Split(',');
            Dictionary<string, string> deviceData = new Dictionary<string, string>();
            foreach (string part in parts)
            {
                string[] keyValue = part.Split(':');
                if (keyValue.Length == 2)
                {
                    deviceData[keyValue[0]] = keyValue[1];
                }
            }
            if (deviceData.ContainsKey("Type"))
            {
                string type = deviceData["Type"];
                if (type == "DeviceInfo")
                {
                    var did = "1";
                    var model = deviceData.ContainsKey("Model") ? deviceData["Model"] : "Unknown";
                    var version = deviceData.ContainsKey("Version") ? int.Parse(deviceData["Version"]) : 0;
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
                        Logger.Instance.LogError("Błąd", e);
                    }
                }
                else if (type == "ButtonPress")
                {
                    string pressedButton = string.Empty;
                    if (deviceData.ContainsKey("AButton"))
                    {
                        pressedButton = deviceData["AButton"].Trim();
                    }
                    else if (deviceData.ContainsKey("Button"))
                    {
                        pressedButton = deviceData["Button"].Trim();
                    }

                    string processName = _activeWindowTracker.GetCurrentProcessName();
                    // Wyszukanie danych przycisku na podstawie numeru przycisku
                    using (var context = new AppDbContext())
                    {
                        string profileID = string.Empty;
                        var profile = context.Profiles.FirstOrDefault(p => p.ProfileName == processName);

                        if (deviceData.ContainsKey("Button"))
                        {
                            profileID = "HOME";
                        }
                        else if (deviceData.ContainsKey("AButton") && profile != null)
                        {
                            profileID = profile.ProfileID;
                        }
                        var buttonData = context.ButtonDatas
                            .Where(b => b.ButtonID == pressedButton && b.ProfileID == profileID)
                            .SingleOrDefault();
                        var action = buttonData == null ? null : context.Actions.SingleOrDefault(a => a.ActionID == buttonData.ActionID);

                        if (!object.ReferenceEquals(buttonData, null))
                        {
                            if (actionHandlers.TryGetValue(action.Type, out var handler))
                            {
                                handler(action.DoAction);
                            }
                            else
                            {
                                Console.WriteLine($"Unknown Type: {action.Type}, unable to process.");
                            }
                        }
                        else if (type == "winSound")
                        {
                            int valueSound = deviceData.ContainsKey("Value") ? int.Parse(deviceData["Value"]) : 0;
                            defaultPlaybackDevice.Volume = valueSound;
                        }
                        else if (type == "winBrightness")
                        {
                            int valueBrightness = deviceData.ContainsKey("Value") ? int.Parse(deviceData["Value"]) : 0;
                        }
                        else
                        {
                            Console.WriteLine($"Nieobsługiwany typ wiadomości: {type}", "Błąd");
                        }
                    }
                }
            }
        }
    }
}
