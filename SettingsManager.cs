namespace Toggle_Muter
{
    public class SettingsManager
    {
        private readonly Form _mainForm;
        private readonly GlobalKeyboardHook _keyboardHook;
        private readonly string _settingsFilePath = "settings.ini";

        private const bool DefaultMonochromaticSysTrayIcon = true;
        private static readonly int[] UnboundKeyCodes = Array.Empty<int>();
        private const string UnboundKeyText = "";

        private int[]? _keyCodes;
        private string? _keyText;
        private bool _monochromaticSysTrayIcon;

        public SettingsManager(Form mainForm, GlobalKeyboardHook keyboardHook)
        {
            _mainForm = mainForm;
            _keyboardHook = keyboardHook;
            ReadValuesFromSettings();
        }

        // Read values from settings.ini
        private void ReadValuesFromSettings()
        {
            try
            {
                string[] lines = File.ReadAllLines(_settingsFilePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split('=');

                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        switch (key)
                        {
                            case "KeyCodes":
                                _keyCodes = value.Split(',').Select(int.Parse).ToArray();
                                break;
                            case "KeyText":
                                _keyText = value;
                                break;
                            case "MonochromaticSysTrayIcon":
                                _monochromaticSysTrayIcon = Convert.ToBoolean(value);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception when settings.ini is not found or has invalid format
                _keyCodes = UnboundKeyCodes;
                _keyText = UnboundKeyText;
                _monochromaticSysTrayIcon = DefaultMonochromaticSysTrayIcon;

                if (File.Exists(_settingsFilePath))
                {
                    File.Delete(_settingsFilePath);
                }

                WriteValuesToSettings();
                Console.WriteLine($"Error reading settings.ini, using default settings instead: {ex.Message}");
            }
        }

        // Write values to settings.ini
        private void WriteValuesToSettings()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_settingsFilePath))
                {
                    writer.WriteLine($"KeyCodes={string.Join(",", _keyCodes ?? UnboundKeyCodes)}");
                    writer.WriteLine($"KeyText={_keyText}");
                    writer.WriteLine($"MonochromaticSysTrayIcon={_monochromaticSysTrayIcon}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to settings.ini: {ex.Message}");
            }
        }

        // Set key codes and text
        public void SetKeyCodesAndText(int[] keyCodes, string keyText)
        {
            GlobalKeyboardHook.UnregisterHook();
            _keyCodes = keyCodes;
            _keyText = keyText;
            GlobalKeyboardHook.RegisterHook();
            WriteValuesToSettings();
        }

        // Set monochromatic system tray icon style
        public void SetMonochromaticSysTrayIcon(bool monochromaticSysTrayIcon)
        {
            _monochromaticSysTrayIcon = monochromaticSysTrayIcon;
            WriteValuesToSettings();
        }

        // Get key codes
        public int[] GetKeyCodes()
        {
            return _keyCodes ?? UnboundKeyCodes;
        }

        // Get key text
        public string GetKeyText()
        {
            return _keyText ?? UnboundKeyText;
        }

        // Get monochromatic system tray icon style
        public bool GetMonochromaticSysTrayIcon()
        {
            return _monochromaticSysTrayIcon;
        }
    }
}