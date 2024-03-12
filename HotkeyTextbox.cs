namespace Toggle_Muter
{
    public class HotkeyTextbox : TextBox
    {
        private Keys hotkey;
        private int selectedKeyCode;
        private string keyText;

        public Keys Hotkey {
            get { return hotkey; }
        }

        public HotkeyTextbox(int initialKeyCode, string initialKeyText) : base() {
            selectedKeyCode = initialKeyCode;
            keyText = initialKeyText;
            SetInitialText();
            TextAlign = HorizontalAlignment.Center; // Center align the textbox
            KeyDown += HotkeyTextbox_KeyDown;
        }

        private void HotkeyTextbox_KeyDown(object sender, KeyEventArgs e) {
            if (sender == null || e == null) { return; } // Handle the case when sender or e is null

            e.SuppressKeyPress = true; // Suppress the key press to prevent text input

            hotkey = (Keys)char.ToUpper((char)e.KeyCode); // Translate the character to Keys value
            selectedKeyCode = (int)hotkey; // Store the selected value
            UpdateHotkeyText();
        }

        
        // Set the initial textbox text to the saved value 
        public void SetInitialText() {
            Text = keyText;
        }

        private void UpdateHotkeyText() {
            Text = hotkey.ToString();
        }

        public int GetSelectedKeyCode() {
            return selectedKeyCode;
        }

        public string GetTextboxText() {
            return Text;
        }
    }
}