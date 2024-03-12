using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Toggle_Muter {

    public class GlobalKeyboardHook
    {
        private static LowLevelKeyboardProc _proc;
        private static IntPtr _hookID = IntPtr.Zero;
        private static Keys[] _pressedKeys = new Keys[0];
        private static Form _form;
        private static SettingsManager _settingsManager;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public GlobalKeyboardHook(Form form, SettingsManager settingsManager)
        {
            _form = form;
            _settingsManager = settingsManager;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)0x0100) // WM_KEYDOWN
            {
                Keys key = (Keys)Marshal.ReadInt32(lParam);
                HandleKeyPress(key, true);
            }
            else if (nCode >= 0 && wParam == (IntPtr)0x0101) // WM_KEYUP
            {
                Keys key = (Keys)Marshal.ReadInt32(lParam);
                HandleKeyPress(key, false);
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void HandleKeyPress(Keys key, bool isPressed)
        {
            if (isPressed)
            {
                _pressedKeys = new[] { key };
                CheckForDesiredKeyCombination();
            }
            else
            {
                _pressedKeys = new Keys[0];
            }
        }

        private static void CheckForDesiredKeyCombination()
        {
            Keys desiredKey = GetDesiredKeyCombination();

            if (_pressedKeys.Length == 1 && _pressedKeys[0] == desiredKey)
            {
                _form.AdjustMuteStatus();
                _pressedKeys = new Keys[0]; // Reset _pressedKeys after handling the desired combination
            }
        }

        private static Keys GetDesiredKeyCombination()
        {
            return (Keys)_settingsManager.GetKeyCode();
        }

        public static void RegisterHook()
        {
            _proc = HookCallback;
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                _hookID = SetWindowsHookEx(13, _proc, GetModuleHandle(currentModule.ModuleName), 0);
            }
            Console.WriteLine("Hook registered");
        }

        public static void UnregisterHook()
        {
            UnhookWindowsHookEx(_hookID);
            Console.WriteLine("Hook unregistered");
        }
    }
}