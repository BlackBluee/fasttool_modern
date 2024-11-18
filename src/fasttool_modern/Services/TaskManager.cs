using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
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
        public void RunExternalApplication(string path)
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
        public static void SendMediaKey(int key)
        {
            IntPtr hWnd = GetForegroundWindow();
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)(key << 16));
        }

        public void SimulateKeyPress(VirtualKey key)
        {
            var inputInjector = InputInjector.TryCreate();

            if (inputInjector != null)
            {
                // Utwórz listę klawiszy do naciśnięcia
                var downEvent = new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)key,
                    KeyOptions = InjectedInputKeyOptions.None
                };

                var upEvent = new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)key,
                    KeyOptions = InjectedInputKeyOptions.KeyUp
                };

                // Wstrzykuj klawisz jako naciśnięcie i zwolnienie
                inputInjector.InjectKeyboardInput(new[] { downEvent, upEvent });
            }
        }
    }
}
