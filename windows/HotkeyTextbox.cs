namespace Toggle_Muter
{
    public class HotkeyTextbox : TextBox
    {
        private const int MaxWidth = 200;
        private const int MinWidth = 100;
        private readonly List<Keys> _hotkeys = new List<Keys>();
        private readonly SettingsManager _settingsManager;
        private int[] _selectedKeyCodes;
        private string _keyText;
        private bool _isNewKeyPressed = true;
        private Keys _lastKeyPressed = Keys.None;

        public event EventHandler? WidthChanged;

        public HotkeyTextbox(int[] initialKeyCodes, string initialKeyText, ConfigureHotkeyForm configureHotkeyForm, SettingsManager settingsManager) : base()
        {
            _selectedKeyCodes = initialKeyCodes;
            _keyText = initialKeyText;
            _settingsManager = settingsManager;
            SetInitialText();
            TextAlign = HorizontalAlignment.Center;
            KeyDown += HotkeyTextbox_KeyDown;
            KeyUp += HotkeyTextbox_KeyUp;
            GotFocus += HotkeyTextbox_GotFocus;
            LostFocus += HotkeyTextbox_LostFocus;
        }

        // Handles the KeyDown event of the HotkeyTextbox control.
        private void HotkeyTextbox_KeyDown(object? sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;

            Keys key = (Keys)char.ToUpper((char)e.KeyCode);

            // Ignore the ALT key
            if (key == Keys.Menu) { return; }

            // Ignore repeated events for the same key while it's held down
            if (!_isNewKeyPressed && key == _lastKeyPressed)
            {
                return;
            }

            if (_isNewKeyPressed)
            {
                // Clear the hotkeys list if a new key is pressed
                _hotkeys.Clear();
                _isNewKeyPressed = false;
            }

            // Add the newly pressed key to the hotkeys list
            _hotkeys.Add(key);
            _lastKeyPressed = key;
            UpdateHotkeyText();
        }

        // Handles the KeyUp event of the HotkeyTextbox control.
        private void HotkeyTextbox_KeyUp(object? sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;

            // Set the flag to indicate that a new key can be pressed
            _isNewKeyPressed = true;
            _lastKeyPressed = Keys.None;
        }

        // Updates the text of the HotkeyTextbox control based on the selected hotkeys.
        public void UpdateHotkeyText()
        {
            Text = _hotkeys.Count == 0 ? "None" : string.Join(" + ", _hotkeys.Select(k => k.ToString()));
            AdjustWidth();
        }

        // Adjusts the width of the HotkeyTextbox control based on its text content.
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

        // Raises the WidthChanged event.
        protected virtual void OnWidthChanged(EventArgs e)
        {
            WidthChanged?.Invoke(this, e);
        }

        // Sets the initial text of the HotkeyTextbox control based on the selected key codes and key text.
        public void SetInitialText()
        {
            _hotkeys.Clear();
            if (_selectedKeyCodes == null || _selectedKeyCodes.Length == 0)
            {
                if (string.IsNullOrEmpty(_keyText))
                {
                    UpdateHotkeyText();
                    return;
                }
                else
                {
                    Text = _keyText;
                    AdjustWidth();
                    return;
                }
            }

            foreach (int keyCode in _selectedKeyCodes)
            {
                _hotkeys.Add((Keys)keyCode);
            }
            UpdateHotkeyText();
            AdjustWidth();
        }

        public int[] GetSelectedKeyCodes()
        {
            return _hotkeys.Select(k => (int)k).ToArray();
        }

        public string GetTextboxText()
        {
            return Text;
        }

        // Handles the GotFocus event of the HotkeyTextbox control.
        private void HotkeyTextbox_GotFocus(object? sender, EventArgs e)
        {
            ForeColor = Color.Black;
            Text = string.Empty;
            _hotkeys.Clear();
            _lastKeyPressed = Keys.None;
            _isNewKeyPressed = true;
        }

        // Handles the LostFocus event of the HotkeyTextbox control.
        private void HotkeyTextbox_LostFocus(object? sender, EventArgs e)
        {
            if (Text != _settingsManager.GetKeyText())
            {
                ForeColor = BackColor;
            }
        }
    }
}