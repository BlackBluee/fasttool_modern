using Microsoft.UI.Xaml;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;





namespace fasttool_modern
{
    public partial class App : Application
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public string previousProcessName { get; set; } = "";
        public List<string> availableApps = new List<string>();

        private System.Timers.Timer windowCheckTimer;
        public App()
        {
            
            this.InitializeComponent();
            windowCheckTimer = new System.Timers.Timer();
            windowCheckTimer.Interval = 1000; // Sprawdzanie co sekundę  
            windowCheckTimer.Elapsed += new System.Timers.ElapsedEventHandler(CheckActiveWindow);
            windowCheckTimer.Start();
        }



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
                    if (!availableApps.Contains(currentProcessName))
                    {
                        availableApps.Add(currentProcessName);
                    }


                    //Type:AcitveApp,Name: chrome
                    //connection.comSend("Type:ActiveApp,Name:" + currentProcessName);

                }
            }
            catch (ArgumentException)
            {
                // Proces mógł zostać zakończony, zresetuj nazwę procesu
                previousProcessName = "";

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
