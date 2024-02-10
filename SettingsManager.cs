namespace Toggle_Muter {
    public partial class SettingsManager : System.Windows.Forms.Form {
        private Form mainForm;
        private const int DEFAULT_MOD_KEY_CODE = 2; // Default modifier hotkey is set to the virtual keycode for 'CTRL'
        private const int DEFAULT_PRIMARY_KEY_CODE = 42; // Default primary hotkey is set to the virtual keycode for 'B'
        private const string DEFAULT_PRIMARY_KEY_TEXT = "B"; // Default text representation of the primary hotkey is set to 'B'
        private const bool DEFAULT_MONOCHROMATIC_SYS_TRAY_ICON = true; // Default system tray icon style is monochromatic
        private int modifierKeyCode;
        private int primaryKeyCode;
        private string primaryKeyText;
        private bool monochromaticSysTrayIcon;
        private string settingsFilePath = "settings.ini";
        
        public SettingsManager(Form mainForm) {
            ReadValuesFromSettings();
            this.mainForm = mainForm; // Initialize mainForm
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
                            case "ModifierKeyCode":
                                modifierKeyCode = Convert.ToInt32(value, 16);
                                break;
                            case "PrimaryKeyCode":
                                primaryKeyCode = Convert.ToInt32(value, 16);
                                break;
                            case "PrimaryKeyText":
                                primaryKeyText = value;
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
                modifierKeyCode = DEFAULT_MOD_KEY_CODE;
                primaryKeyCode = DEFAULT_PRIMARY_KEY_CODE;
                primaryKeyText = DEFAULT_PRIMARY_KEY_TEXT;
                monochromaticSysTrayIcon = DEFAULT_MONOCHROMATIC_SYS_TRAY_ICON;

                // Check if the settings.ini file already exists
                if (File.Exists("settings.ini")) {
                    // Delete the file and create a new one
                    File.Delete("settings.ini");
                }
                
                using (StreamWriter sw = new StreamWriter("settings.ini")) {
                        sw.WriteLine($"ModifierKeyCode={modifierKeyCode}");
                        sw.WriteLine($"PrimaryKeyCode={primaryKeyCode}");
                        sw.WriteLine($"PrimaryKeyText={primaryKeyText}");
                        sw.WriteLine($"MonochromaticSysTrayIcon={monochromaticSysTrayIcon}");
                }

                SetModifierKeyCode(modifierKeyCode);
                SetPrimaryKeyCode(primaryKeyCode);
                SetPrimaryKeyText(primaryKeyText);
                SetMonochromaticSysTrayIcon(monochromaticSysTrayIcon);

                Console.WriteLine($"Error reading settings.ini, reading default settings instead: {ex.Message}");
            }
        }

        private void WriteValuesToSettings() {
            try {
                using StreamWriter writer = new StreamWriter(settingsFilePath);
                writer.WriteLine($"ModifierKeyCode={modifierKeyCode:X}");
                writer.WriteLine($"PrimaryKeyCode={primaryKeyCode:X}");
                writer.WriteLine($"PrimaryKeyText={primaryKeyText:X}");
                writer.WriteLine($"MonochromaticSysTrayIcon={monochromaticSysTrayIcon}");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error writing to settings.ini: {ex.Message}");
            }
        }

        #endregion

        #region Setters

        public void SetModifierKeyCode(int MOD_KEY_CODE) {
            mainForm.UnregHotkey();
            modifierKeyCode = MOD_KEY_CODE;
            mainForm.RegHotkey();
            WriteValuesToSettings();
        }

        public void SetPrimaryKeyCode(int PRIMARY_KEY_CODE) {
            mainForm.UnregHotkey();
            primaryKeyCode = PRIMARY_KEY_CODE;
            mainForm.RegHotkey();
            WriteValuesToSettings();
        }

        public void SetPrimaryKeyText(string PRIMARY_KEY_TEXT) {
            primaryKeyText = PRIMARY_KEY_TEXT;
            WriteValuesToSettings();
        }

        public void SetMonochromaticSysTrayIcon(bool SYS_TRAY_ICON_STYLE) {
            monochromaticSysTrayIcon = SYS_TRAY_ICON_STYLE;
            WriteValuesToSettings();
        }

        #endregion

        #region Getters

        public int GetModifierKeyCode() {
            return modifierKeyCode;
        }

        public int GetPrimaryKeyCode() {
            return primaryKeyCode;
        }

        public string GetPrimaryText() {
            return primaryKeyText;
        }

        public bool GetMonochromaticSysTrayIcon() {
            return monochromaticSysTrayIcon;
        }

        #endregion
    }
}