namespace Toggle_Muter {
    public partial class ConfigureHotkeyForm : System.Windows.Forms.Form {
        private PrimaryHotkeyTextbox primaryHotkeyTextbox;
        private ModifierHotkeyDropdown modifierHotkeyDropdown;
        private SettingsManager settingsManager;
        
        public ConfigureHotkeyForm(SettingsManager settingsManager) {
            InitializeComponent();
            this.settingsManager = settingsManager;
            ShowHotkeyConfig();

            // Create and configure the ModifierDropdown control
            modifierHotkeyDropdown = new ModifierHotkeyDropdown(settingsManager.GetModifierKeyCode());
            Controls.Add(modifierHotkeyDropdown.GetDropdown());

            // Create and configure the HotkeyTextbox control
            primaryHotkeyTextbox = new PrimaryHotkeyTextbox(settingsManager.GetPrimaryKeyCode(), settingsManager.GetPrimaryText()) {
                Location = new Point(164, 20),
                Width = 76
            };
            Controls.Add(primaryHotkeyTextbox);

            // Create and configure the "+" Label between the dropdown and textbox
            Label label = new Label();
            label.Text = "+";
            label.Font = new Font(label.Font.FontFamily, 12, FontStyle.Bold);
            label.Size = new Size(20, 20);
            label.Location = new Point(141, 20);
            Controls.Add(label);

            // Add the "Confirm" button on the left
            Button confirmButton = new Button {
                Text = "Confirm",
                FlatStyle = FlatStyle.System
            };
            confirmButton.Click += ConfirmButton_Click;
            confirmButton.Location = new Point(62, 60);
            Controls.Add(confirmButton);

            // Add the "Close" button on the right
            Button closeButton = new Button {
                Text = "Close",
                Top = 30,
                FlatStyle = FlatStyle.System
            };
            closeButton.Click += CloseButton_Click;
            closeButton.Location = new Point(163, 60);
            Controls.Add(closeButton);

            FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(300, 102);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Configure Hotkey Form";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configure Hotkey Form";
            ResumeLayout(false);
        }

        // Shows the hotkey configuration window
        private void ShowHotkeyConfig() {
            Text = "Configure Hotkey";
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            ControlBox = false;
            TopMost = true;
            Opacity = 100;
        }

        private void ConfirmButton_Click(object? sender, EventArgs e) {
            settingsManager.SetModifierKeyCode(modifierHotkeyDropdown.GetSelectedModifier());
            settingsManager.SetPrimaryKeyCode(primaryHotkeyTextbox.GetSelectedPrimaryKeyCode());
            settingsManager.SetPrimaryKeyText(primaryHotkeyTextbox.GetPrimaryTextboxText());
        }

        private void CloseButton_Click(object? sender, EventArgs e) {
            Close();
        }
    }
}