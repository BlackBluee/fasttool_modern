using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Text;
using Windows.System;
using WindowsInput;
using WindowsInput.Native;

namespace fasttool_modern.Helpers
{
    public class KeyShortcutRecorder
    {
        private static readonly Lazy<KeyShortcutRecorder> _instance =
            new(() => new KeyShortcutRecorder());

        private StringBuilder _shortcutBuilder = new();
        private TextBox _outputTextBox;

        private KeyShortcutRecorder() { }

        public static KeyShortcutRecorder Instance => _instance.Value;

        public void SetTextBox(TextBox textBox)
        {
            _outputTextBox = textBox;
        }

        public void StartRecording()
        {
            _shortcutBuilder.Clear();
            if (_outputTextBox != null)
                _outputTextBox.Text = "Press keys...";
        }

        public void StopRecording()
        {
            if (_outputTextBox != null)
                _outputTextBox.Text = _shortcutBuilder.ToString();
        }

        public void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            e.Handled = true;
            if (_shortcutBuilder.ToString().Contains(e.Key.ToString()))
                return;
            if (_shortcutBuilder.Length > 0)
                _shortcutBuilder.Append(" + ");
            _shortcutBuilder.Append(e.Key.ToString());
            if (_outputTextBox != null)
                _outputTextBox.Text = _shortcutBuilder.ToString();
        }

        public void PlayRecordedShortcut(string shortcut)
        {
            if (string.IsNullOrEmpty(shortcut))
            {
                if (_outputTextBox != null)
                    _outputTextBox.Text = "No shortcut provided!";
                return;
            }

            SimulateShortcut(shortcut);
        }

        private void SimulateShortcut(string shortcut)
        {
            var simulator = new InputSimulator();
            string[] keys = shortcut.Split(new string[] { " + " }, StringSplitOptions.None);

            bool hasCtrl = false;
            bool hasShift = false;
            bool hasAlt = false;
            VirtualKeyCode mainKey = VirtualKeyCode.SPACE;  

            foreach (string key in keys)
            {
                if (key.Equals("Control", StringComparison.OrdinalIgnoreCase))
                {
                    hasCtrl = true;
                }
                else if (key.Equals("Shift", StringComparison.OrdinalIgnoreCase))
                {
                    hasShift = true;
                }
                else if (key.Equals("Alt", StringComparison.OrdinalIgnoreCase))
                {
                    hasAlt = true;
                }
                else
                {
                    mainKey = ConvertToVirtualKeyCode(key);  
                }
            }

            if (hasCtrl)
            {
                simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, mainKey);  
            }
            else if (hasShift)
            {
                simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, mainKey);  
            }
            else if (hasAlt)
            {
                simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LMENU, mainKey); 
            }
            else
            {
                simulator.Keyboard.KeyPress(mainKey);  
            }
        }

        private VirtualKeyCode ConvertToVirtualKeyCode(string key)
        {
            switch (key.ToUpper())
            {
                case "A": return VirtualKeyCode.VK_A;
                case "B": return VirtualKeyCode.VK_B;
                case "C": return VirtualKeyCode.VK_C;
                case "V": return VirtualKeyCode.VK_V;
                case "T": return VirtualKeyCode.VK_T;
                case "TAB": return VirtualKeyCode.TAB;
                case "ESCAPE": return VirtualKeyCode.ESCAPE;
                default: return VirtualKeyCode.SPACE; 
            }
        }
    }
}
