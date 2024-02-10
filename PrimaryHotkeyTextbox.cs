namespace Toggle_Muter
{
    public class PrimaryHotkeyTextbox : TextBox
    {
        private Keys hotkey;
        private int selectedPrimaryKeyCode;
        private string primaryKeyText;

        public Keys Hotkey {
            get { return hotkey; }
        }

        public PrimaryHotkeyTextbox(int initialPrimaryKeyCode, string initialPrimaryKeyText) : base() {
            selectedPrimaryKeyCode = initialPrimaryKeyCode;
            primaryKeyText = initialPrimaryKeyText;
            SetInitialText();
            TextAlign = HorizontalAlignment.Center; // Center align the textbox
            KeyDown += HotkeyTextbox_KeyDown;
        }

        private void HotkeyTextbox_KeyDown(object sender, KeyEventArgs e) {
            if (sender == null || e == null) { return; } // Handle the case when sender or e is null

            e.SuppressKeyPress = true; // Suppress the key press to prevent text input

            // Check if the pressed key is a modifier key
            if (e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.Menu) {
                hotkey = (Keys)char.ToUpper((char)e.KeyCode); // Translate the character to Keys value
                selectedPrimaryKeyCode = (int)hotkey; // Store the selected primary value
                UpdateHotkeyText();
            }
        }

        
        // Set the initial textbox text to the saved value 
        public void SetInitialText() {
            Text = primaryKeyText;
        }

        private void UpdateHotkeyText() {
            Text = hotkey.ToString();
        }

        public int GetSelectedPrimaryKeyCode() {
            return selectedPrimaryKeyCode;
        }

        public string GetPrimaryTextboxText() {
            return Text;
        }
    }
}