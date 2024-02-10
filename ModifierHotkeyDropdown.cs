public class ModifierHotkeyDropdown
{
    private ComboBox dropdown;
    private const int MOD_ALT = 1; // Virtual keycode for the alt key
    private const int MOD_CONTROL = 2; // Virtual keycode for the ctrl key
    private const int MOD_SHIFT = 4; // Virtual keycode for the shift key
    private int selectedModifierKeyCode;

    public ModifierHotkeyDropdown(int initialModifierKeyCode) {
        selectedModifierKeyCode = initialModifierKeyCode; // Set the selected modifier hotkey to the key specified in the settings.ini
        InitializeDropdown();
        InitializeOptions();
        SetInitialSelectedIndex();
        AttachEventHandlers();
    }

    private void InitializeDropdown() {
        dropdown = new ComboBox {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 76,
            Location = new Point(60, 20)
        };
    }

    private void InitializeOptions() {
        List<string> options = new()
        {
            "Alt",
            "Control",
            "Shift",
        };

        dropdown.Items.AddRange(options.ToArray());
    }

    private void AttachEventHandlers() {
        dropdown.SelectedIndexChanged += Dropdown_SelectedIndexChanged;
    }

    // Set the initial dropdown box selection to the saved value 
    public void SetInitialSelectedIndex() {
        switch (selectedModifierKeyCode) {
            case MOD_ALT:
                dropdown.SelectedItem = "Alt";
                break;
            case MOD_CONTROL:
                dropdown.SelectedItem = "Control";
                break;
            case MOD_SHIFT:
                dropdown.SelectedItem = "Shift";
                break;
            default:
                dropdown.SelectedItem = null;
                break;
        }
    }

    // Update the modifier selection when the user changes the value
    private void Dropdown_SelectedIndexChanged(object sender, EventArgs e) {
        ComboBox comboBox = (ComboBox)sender;
        string selectedItem = (string)comboBox.SelectedItem;

        switch (selectedItem) {
            case "Alt":
                selectedModifierKeyCode = MOD_ALT;
                break;
            case "Control":
                selectedModifierKeyCode = MOD_CONTROL;
                break;
            case "Shift":
                selectedModifierKeyCode = MOD_SHIFT;
                break;
            default:
                selectedModifierKeyCode = 0;
                break;
        }

        Console.WriteLine(selectedModifierKeyCode);
    }

    public ComboBox GetDropdown() {
        return dropdown;
    }

    public int GetSelectedModifier() {
        return selectedModifierKeyCode;
    }
}