using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;

namespace fasttool_modern.Services
{
    public class ActiveWindowTracker
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        private Timer windowCheckTimer;
        public string previousProcessName { get; set; } = "";
        public List<string> availableApps = new List<string>();
        SerialPortManager serialPortManager = SerialPortManager.Instance;
        private static ActiveWindowTracker instance;


        public ActiveWindowTracker()
        {
            // Inicjalizacja timera
            windowCheckTimer = new Timer(1000); // Interval ustawiony na 1000 ms (1 sekunda)
            windowCheckTimer.Elapsed += CheckActiveWindow; // Subskrypcja metody do zdarzenia Elapsed
        }

        public static ActiveWindowTracker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ActiveWindowTracker();
                }
                return instance;
            }
        }
        public void StartTracking()
        {
            windowCheckTimer.Start(); // Rozpoczęcie działania timera
            Console.WriteLine("Tracking started.");
        }

        public void StopTracking()
        {
            windowCheckTimer.Stop(); // Zatrzymanie działania timera
            Console.WriteLine("Tracking stopped.");
        }

        public void CheckActiveWindow(object sender, EventArgs e)
        {
            IntPtr handle = GetForegroundWindow();
            GetWindowThreadProcessId(handle, out uint processId);
            try
            {
                Process process = Process.GetProcessById((int)processId);
                string currentProcessName = process.ProcessName;
                if (currentProcessName != previousProcessName)
                {
                    previousProcessName = currentProcessName;
                    if (!availableApps.Contains(currentProcessName))
                    {
                        availableApps.Add(currentProcessName);
                    }
                    //Type:AcitveApp,Name: chrome
                    serialPortManager.Send("Type:ActiveApp,Name:" + currentProcessName);
                }
            }
            catch (ArgumentException)
            {
                // Proces mógł zostać zakończony, zresetuj nazwę procesu
                previousProcessName = "";
            }
        }
    }
}
