namespace Toggle_Muter {
    public class HotkeyTextbox : TextBox
    {
        private const int MaxWidth = 200;
        private const int MinWidth = 100;
        private readonly List<Keys> hotkeys = new List<Keys>();
        private int[] selectedKeyCodes;
        private string keyText;
        private bool isNewKeyPressed = true; // Flag to track if a new key is pressed
        private Keys lastKeyPressed = Keys.None; // Track the last key pressed
        public event EventHandler WidthChanged;
        private ConfigureHotkeyForm _configureHotkeyForm;
        private SettingsManager _settingsManager;

        public HotkeyTextbox(int[] initialKeyCodes, string initialKeyText, ConfigureHotkeyForm configureHotkeyForm, SettingsManager settingsManager) : base()
        {
            selectedKeyCodes = initialKeyCodes;
            keyText = initialKeyText;
            _configureHotkeyForm = configureHotkeyForm;
            _settingsManager = settingsManager;
            SetInitialText();
            TextAlign = HorizontalAlignment.Center;
            KeyDown += HotkeyTextbox_KeyDown;
            KeyUp += HotkeyTextbox_KeyUp;
            GotFocus += HotkeyTextbox_GotFocus;
            LostFocus += HotkeyTextbox_LostFocus;
        }

        private void HotkeyTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender == null || e == null) { return; }

            e.SuppressKeyPress = true;

            Keys key = (Keys)char.ToUpper((char)e.KeyCode);

            // Ignore repeated events for the same key while it's held down
            if (!isNewKeyPressed && key == lastKeyPressed)
            {
                return;
            }

            if (isNewKeyPressed)
            {
                // Clear the hotkeys list if a new key is pressed
                hotkeys.Clear();
                isNewKeyPressed = false;
            }

            // Add the newly pressed key
            hotkeys.Add(key);
            lastKeyPressed = key;
            UpdateHotkeyText();
        }

        private void HotkeyTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender == null || e == null) { return; }

            e.SuppressKeyPress = true;

            // Set the flag to indicate that a new key can be pressed
            isNewKeyPressed = true;
            lastKeyPressed = Keys.None;
        }

        
        public void UpdateHotkeyText()
        {
            if (hotkeys.Count == 0)
            {
                Text = "None";
            }
            else
            {
                Text = string.Join(" + ", hotkeys.Select(k => k.ToString()));
            }
            AdjustWidth();
        }

        private void AdjustWidth()
        {
            using (Graphics g = CreateGraphics())
            {
                int textWidth = (int)g.MeasureString(Text, Font).Width + Padding.Horizontal;
                int newWidth = Math.Min(Math.Max(textWidth, MinWidth), MaxWidth);
                if (Width != newWidth)
                {
                    Width = newWidth;
                    OnWidthChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnWidthChanged(EventArgs e)
        {
            WidthChanged?.Invoke(this, e);
        }

        public void SetInitialText()
        {
            hotkeys.Clear();
            if (selectedKeyCodes == null || selectedKeyCodes.Length == 0)
            {
                if (string.IsNullOrEmpty(keyText))
                {
                    UpdateHotkeyText(); // This will set the Text to "None"
                    return;
                }
                else
                {
                    // Handle the case where keyText is not null or empty, but selectedKeyCodes is null or empty
                    Text = keyText;
                    AdjustWidth();
                    return;
                }
            }

            foreach (int keyCode in selectedKeyCodes)
            {
                hotkeys.Add((Keys)keyCode);
            }
            UpdateHotkeyText();
            AdjustWidth(); // Adjust width based on initial text
        }

        public int[] GetSelectedKeyCodes()
        {
            return hotkeys.Select(k => (int)k).ToArray();
        }

        public string GetTextboxText()
        {
            return Text;
        }

        private void HotkeyTextbox_GotFocus(object sender, EventArgs e)
        {
            ForeColor = Color.Black;
            Text = string.Empty;
            hotkeys.Clear();
            lastKeyPressed = Keys.None;
            isNewKeyPressed = true;
        }

        private void HotkeyTextbox_LostFocus(object sender, EventArgs e)
        {
            if (Text != _settingsManager.GetKeyText()) {
                ForeColor = BackColor;
            }
        }
    }
}