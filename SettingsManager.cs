using System.Windows.Forms.Design;

namespace Toggle_Muter {
    public partial class SettingsManager : System.Windows.Forms.Form {
        private Form mainForm;
        private GlobalKeyboardHook keyboardHook;
        private const bool DEFAULT_MONOCHROMATIC_SYS_TRAY_ICON = true; // Default system tray icon style is monochromatic
        private static readonly int[] DEFAULT_KEY_CODES = { 42 }; // Default hotkey is set to the virtual keycode for 'B'
        private const string DEFAULT_KEY_TEXT = "B"; // Default text representation of the hotkey is set to 'B'
         private int[] keyCodes;
        private string keyText;
        private bool monochromaticSysTrayIcon;
        private string settingsFilePath = "settings.ini";
        
        public SettingsManager(Form mainForm, GlobalKeyboardHook keyboardHook) {
            ReadValuesFromSettings();
            this.mainForm = mainForm; // Initialize mainForm
            this.keyboardHook = keyboardHook;
        }

        #region Read/Write settings.ini 

        // Method to read values from settings.ini
        private void ReadValuesFromSettings() {

            try {
                // Read all lines from the settings file
                string[] lines = File.ReadAllLines(settingsFilePath);

                // Loop through each line and parse key-value pairs
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
                                keyCodes = value.Split(',').Select(int.Parse).ToArray();
                                Console.WriteLine("Keycode(s): '"+ string.Join(",", keyCodes) + "'");
                                break;
                            case "KeyText":
                                keyText = value;
                                Console.WriteLine("Hotkey is set to: '" + keyText + "'");
                                break;
                            case "MonochromaticSysTrayIcon":
                                monochromaticSysTrayIcon = Convert.ToBoolean(value);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) {
                // Handle exception for when the settings.ini file is not found
                keyCodes = DEFAULT_KEY_CODES;
                keyText = DEFAULT_KEY_TEXT;
                monochromaticSysTrayIcon = DEFAULT_MONOCHROMATIC_SYS_TRAY_ICON;

                // Check if the settings.ini file already exists
                if (File.Exists("settings.ini")) {
                    // Delete the file and create a new one
                    File.Delete("settings.ini");
                }
                
                using (StreamWriter sw = new StreamWriter("settings.ini")) {
                        sw.WriteLine($"KeyCode={keyCodes}");
                        sw.WriteLine($"KeyText={keyText}");
                        sw.WriteLine($"MonochromaticSysTrayIcon={monochromaticSysTrayIcon}");
                }

                SetKeyCodes(keyCodes);
                SetKeyText(keyText);
                SetMonochromaticSysTrayIcon(monochromaticSysTrayIcon);

                Console.WriteLine($"Error reading settings.ini, reading default settings instead: {ex.Message}");
            }
        }

        private void WriteValuesToSettings()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(settingsFilePath))
                {
                    writer.WriteLine($"KeyCodes={string.Join(",", keyCodes)}");
                    writer.WriteLine($"KeyText={keyText}");
                    writer.WriteLine($"MonochromaticSysTrayIcon={monochromaticSysTrayIcon}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to settings.ini: {ex.Message}");
            }
        }

        #endregion

        #region Setters

        public void SetKeyCodes(int[] KEY_CODES)
        {
            GlobalKeyboardHook.UnregisterHook();
            keyCodes = KEY_CODES;
            GlobalKeyboardHook.RegisterHook();
            WriteValuesToSettings();
        }

        public void SetKeyText(string KEY_TEXT)
        {
            keyText = KEY_TEXT;
            WriteValuesToSettings();
        }

        public void SetMonochromaticSysTrayIcon(bool SYS_TRAY_ICON_STYLE) {
            monochromaticSysTrayIcon = SYS_TRAY_ICON_STYLE;
            WriteValuesToSettings();
        }

        #endregion

        #region Getters

        public int[] GetKeyCodes()
        {
            return keyCodes ?? new int[0];
        }

        public string GetKeyText()
        {
            return keyText;
        }

        public bool GetMonochromaticSysTrayIcon() {
            return monochromaticSysTrayIcon;
        }

        #endregion
    }
}