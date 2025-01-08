using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Windows.UI.Input.Preview.Injection;


namespace fasttool_modern.Services
{
    internal class TaskManager
    {
        private const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
        private const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
        private const int APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
        private const int WM_APPCOMMAND = 0x0319;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        private static AudioDeviceMonitor _audioDeviceMonitor = AudioDeviceMonitor.Instance;

        public static void RunExternalApplication(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd uruchamiania aplikacji: {ex.Message}");
            }
        }

        public static void SendMediaKey(int key)
        {
            IntPtr hWnd = GetForegroundWindow();
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)(key << 16));
        }

        public static void MultimediaCommand(string command)
        {
            var defaultPlaybackDevice = _audioDeviceMonitor.GetDefaultPlaybackDevice();
            //obsługa przycisków multimedialnych
            switch (command)
            {
                case "play/pause":
                    SendMediaKey(APPCOMMAND_MEDIA_PLAY_PAUSE);
                    break;
                case "mute/unmute":
                    defaultPlaybackDevice.Mute(!defaultPlaybackDevice.IsMuted);
                    break;
                case "next":
                    SendMediaKey(APPCOMMAND_MEDIA_NEXTTRACK);
                    break;
                case "previous":
                    SendMediaKey(APPCOMMAND_MEDIA_PREVIOUSTRACK);
                    break;
                case "volume up":
                    ++defaultPlaybackDevice.Volume;
                    break;
                case "volume down":
                    --defaultPlaybackDevice.Volume;
                    break;
                default:
                    Console.WriteLine($"Nieobsługiwana akcja multimedialna: {command}", "Błąd");
                    break;
            }
        }
    }
}
