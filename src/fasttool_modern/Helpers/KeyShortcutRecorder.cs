using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

            string[] keys = shortcut.Split(" + ");
            SimulateKeyPress(keys);
        }

        private void SimulateKeyPress(string[] keys)
        {
            foreach (var key in keys)
            {
                if (Enum.TryParse(key, out VirtualKey virtualKey))
                {
                    KeyDown((ushort)virtualKey);
                }
            }

            foreach (var key in keys.Reverse())
            {
                if (Enum.TryParse(key, out VirtualKey virtualKey))
                {
                    KeyUp((ushort)virtualKey);
                }
            }
        }


        private void KeyDown(ushort keyCode)
        {
            INPUT[] inputs = new INPUT[]
            {
                new INPUT
                {
                    Type = INPUT_KEYBOARD,
                    Data = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = keyCode,
                            dwFlags = 0 
                        }
                    }
                }
            };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        private void KeyUp(ushort keyCode)
        {
            INPUT[] inputs = new INPUT[]
            {
                new INPUT
                {
                    Type = INPUT_KEYBOARD,
                    Data = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = keyCode,
                            dwFlags = KEYEVENTF_KEYUP 
                        }
                    }
                }
            };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }


        private const int INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int Type;
            public InputUnion Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        private enum VirtualKey
        {
            Control = 0x11,
            Alt = 0x12,
            Shift = 0x10,
            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A,
            Zero = 0x30,
            One = 0x31,
            Two = 0x32,
            Three = 0x33,
            Four = 0x34,
            Five = 0x35,
            Six = 0x36,
            Seven = 0x37,
            Eight = 0x38,
            Nine = 0x39,

            Enter = 0x0D,
            Escape = 0x1B,
            Space = 0x20,
            Tab = 0x09,
            Backspace = 0x08,
            Delete = 0x2E,
            ArrowUp = 0x26,
            ArrowDown = 0x28,
            ArrowLeft = 0x25,
            ArrowRight = 0x27,
            Home = 0x24,
            End = 0x23,
            PageUp = 0x21,
            PageDown = 0x22,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,

            NumLock = 0x90,
            CapsLock = 0x14,
            ScrollLock = 0x91,
            Insert = 0x2D,
            PrintScreen = 0x2C,

            LeftWindows = 0x5B,
            RightWindows = 0x5C,
            Applications = 0x5D,
            Sleep = 0x5F,

            Numpad0 = 0x60,
            Numpad1 = 0x61,
            Numpad2 = 0x62,
            Numpad3 = 0x63,
            Numpad4 = 0x64,
            Numpad5 = 0x65,
            Numpad6 = 0x66,
            Numpad7 = 0x67,
            Numpad8 = 0x68,
            Numpad9 = 0x69,
            NumpadDivide = 0x6F,
            NumpadMultiply = 0x6A,
            NumpadSubtract = 0x6D,
            NumpadAdd = 0x6B,
            NumpadEnter = 0xA00,
            NumpadDecimal = 0x6E
        }
    }
}
