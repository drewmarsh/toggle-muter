using System.Windows.Forms.Design;

namespace Toggle_Muter {
    public partial class SettingsManager : System.Windows.Forms.Form {
        private Form mainForm;
        private GlobalKeyboardHook keyboardHook;
        private const int DEFAULT_KEY_CODE = 42; // Default hotkey is set to the virtual keycode for 'B'
        private const string DEFAULT_KEY_TEXT = "B"; // Default text representation of the hotkey is set to 'B'
        private const bool DEFAULT_MONOCHROMATIC_SYS_TRAY_ICON = true; // Default system tray icon style is monochromatic
        private int keyCode;
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
                foreach (string line in lines) {
                    // Split the line into key and value based on the '=' separator
                    string[] parts = line.Split('=');

                    // Ensure that the line is in the correct format with key and value
                    if (parts.Length == 2) {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        // Parse and assign the values based on the key
                        switch (key) {
                            case "KeyCode":
                                keyCode = Convert.ToInt32(value, 16);
                                break;
                            case "KeyText":
                                keyText = value;
                                Console.WriteLine("Hotkey is set to '" + keyText + "'");
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
                keyCode = DEFAULT_KEY_CODE;
                keyText = DEFAULT_KEY_TEXT;
                monochromaticSysTrayIcon = DEFAULT_MONOCHROMATIC_SYS_TRAY_ICON;

                // Check if the settings.ini file already exists
                if (File.Exists("settings.ini")) {
                    // Delete the file and create a new one
                    File.Delete("settings.ini");
                }
                
                using (StreamWriter sw = new StreamWriter("settings.ini")) {
                        sw.WriteLine($"KeyCode={keyCode}");
                        sw.WriteLine($"KeyText={keyText}");
                        sw.WriteLine($"MonochromaticSysTrayIcon={monochromaticSysTrayIcon}");
                }

                SetKeyCode(keyCode);
                SetKeyText(keyText);
                SetMonochromaticSysTrayIcon(monochromaticSysTrayIcon);

                Console.WriteLine($"Error reading settings.ini, reading default settings instead: {ex.Message}");
            }
        }

        private void WriteValuesToSettings() {
            try {
                using StreamWriter writer = new StreamWriter(settingsFilePath);
                writer.WriteLine($"KeyCode={keyCode:X}");
                writer.WriteLine($"KeyText={keyText:X}");
                writer.WriteLine($"MonochromaticSysTrayIcon={monochromaticSysTrayIcon}");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error writing to settings.ini: {ex.Message}");
            }
        }

        #endregion

        #region Setters

        public void SetKeyCode(int KEY_CODE) {
            GlobalKeyboardHook.UnregisterHook();;
            keyCode = KEY_CODE;
            GlobalKeyboardHook.RegisterHook();
            WriteValuesToSettings();
        }

        public void SetKeyText(string KEY_TEXT) {
            keyText = KEY_TEXT;
            WriteValuesToSettings();
        }

        public void SetMonochromaticSysTrayIcon(bool SYS_TRAY_ICON_STYLE) {
            monochromaticSysTrayIcon = SYS_TRAY_ICON_STYLE;
            WriteValuesToSettings();
        }

        #endregion

        #region Getters

        public int GetKeyCode() {
            return keyCode;
        }

        public string GetKeyText() {
            return keyText;
        }

        public bool GetMonochromaticSysTrayIcon() {
            return monochromaticSysTrayIcon;
        }

        #endregion
    }
}