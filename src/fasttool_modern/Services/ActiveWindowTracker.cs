using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using fasttool_modern.Services.Interfaces;

namespace fasttool_modern.Services
{
    public class ActiveWindowTracker : IBackgroundTask
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        private Timer checkTimer;
        public string previousProcessName { get; set; } = string.Empty;
        public List<string> availableApps = new List<string>();
        SerialPortManager serialPortManager = SerialPortManager.Instance;
        private static ActiveWindowTracker instance;


        public ActiveWindowTracker()
        {
            checkTimer = new Timer(1000);
            checkTimer.Elapsed += CheckActiveWindow; 
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
        public void Start()
        {
            checkTimer.Start(); 
            Console.WriteLine("Tracking started.");
        }

        public void Stop()
        {
            checkTimer.Stop(); 
            Console.WriteLine("Tracking stopped.");
        }

        public string GetPreviousProcessName()
        {
            return previousProcessName;
        }

        private void CheckActiveWindow(object sender, EventArgs e)
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
                previousProcessName = "";
            }
        }
    }
}
