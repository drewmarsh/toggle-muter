public class HotkeyTextbox : TextBox
{
    private List<Keys> hotkeys = new List<Keys>();
    private int[] selectedKeyCodes;
    private string keyText;
    private bool isNewKeyPressed = true; // Flag to track if a new key is pressed
    private Keys lastKeyPressed = Keys.None; // Track the last key pressed

    public HotkeyTextbox(int[] initialKeyCodes, string initialKeyText) : base()
    {
        selectedKeyCodes = initialKeyCodes;
        keyText = initialKeyText;
        SetInitialText();
        TextAlign = HorizontalAlignment.Center;
        KeyDown += HotkeyTextbox_KeyDown;
        KeyUp += HotkeyTextbox_KeyUp;
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


    private void UpdateHotkeyText()
    {
        Text = string.Join(" + ", hotkeys.Select(k => k.ToString()));
    }

    public void SetInitialText()
    {
        hotkeys.Clear();
        if (selectedKeyCodes != null)
        {
            foreach (int keyCode in selectedKeyCodes)
            {
                hotkeys.Add((Keys)keyCode);
            }
        }
        UpdateHotkeyText();
    }

    public int[] GetSelectedKeyCodes()
    {
        return hotkeys.Select(k => (int)k).ToArray();
    }

    public string GetTextboxText()
    {
        return Text;
    }
}