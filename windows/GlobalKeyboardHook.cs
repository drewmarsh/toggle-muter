using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Toggle_Muter
{
    public class GlobalKeyboardHook
    {
        private static LowLevelKeyboardProc? _hookCallback;
        private static IntPtr _hookId = IntPtr.Zero;
        private static Dictionary<Keys, bool> _keyStates = new Dictionary<Keys, bool>();
        private static Form? _form;
        private static SettingsManager? _settingsManager;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public GlobalKeyboardHook(Form form, SettingsManager settingsManager)
        {
            _form = form;
            _settingsManager = settingsManager;
        }

        // Register the global keyboard hook
        public static void RegisterHook()
        {
            int[] keyCodes = _settingsManager?.GetKeyCodes() ?? Array.Empty<int>();
            if (keyCodes.Length == 0)
            {
                return;
            }

            _hookCallback = HookCallback;
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule!)
            {
                _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _hookCallback!, GetModuleHandle(currentModule.ModuleName), 0);
            }
            Debug.WriteLine("Hook registered");
            Debug.WriteLine($"Keycode(s): '{string.Join(", ", _settingsManager?.GetKeyCodes() ?? Array.Empty<int>())}'");
            Debug.WriteLine($"Hotkey: '{_settingsManager?.GetKeyText() ?? ""}'");
        }

        // Unregister the global keyboard hook
        public static void UnregisterHook()
        {
            UnhookWindowsHookEx(_hookId);
            Debug.WriteLine("Hook unregistered");
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                Keys key = (Keys)Marshal.ReadInt32(lParam);
                bool isKeyDown = (wParam == (IntPtr)WM_KEYDOWN);

                HandleKeyPress(key, isKeyDown);
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static void HandleKeyPress(Keys key, bool isKeyDown)
        {
            // Convert left/right modifiers to their base modifier keys
            key = NormalizeModifierKey(key);

            // Update the key state in the dictionary
            _keyStates[key] = isKeyDown;

            CheckForDesiredKeyCombination();
        }

        private static void CheckForDesiredKeyCombination()
        {
            if (_form == null || _settingsManager == null)
            {
                return;
            }

            Keys[] desiredKeys = _settingsManager.GetKeyCodes().Select(k => (Keys)k).ToArray();

            // Check if all desired keys are pressed simultaneously
            bool allDesiredKeysPressed = desiredKeys.All(key => _keyStates.ContainsKey(key) && _keyStates[key]);

            if (allDesiredKeysPressed)
            {
                Debug.WriteLine($"Desired key combination ({string.Join(", ", desiredKeys)}) has been pressed!");
                _form.AdjustMuteStatus();

                // Reset the key states after handling the desired combination
                foreach (Keys key in desiredKeys)
                {
                    _keyStates[key] = false;
                }
            }
        }

        private static Keys NormalizeModifierKey(Keys key)
        {
            switch (key)
            {
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return Keys.ControlKey;
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return Keys.ShiftKey;
                case Keys.LMenu:
                case Keys.RMenu:
                    return Keys.Menu;
                case Keys.LWin:
                case Keys.RWin:
                    return Keys.LWin;
                default:
                    return key;
            }
        }

        // Import the necessary Win32 API functions
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // Constants for Win32 API function parameters
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
    }
}