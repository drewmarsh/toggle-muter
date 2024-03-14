namespace Toggle_Muter {
    public partial class ConfigureHotkeyForm : System.Windows.Forms.Form {
        private HotkeyTextbox hotkeyTextbox;
        private SettingsManager settingsManager;
        
        public ConfigureHotkeyForm(SettingsManager settingsManager) {
            InitializeComponent();
            this.settingsManager = settingsManager;
            ShowHotkeyConfig();

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

            // Create and configure the HotkeyTextbox control
            hotkeyTextbox = new HotkeyTextbox(settingsManager.GetKeyCodes(), settingsManager.GetKeyText()) {
                Location = new Point(112, 19),
                Width = 76,
                TabStop = false // Prevent the TextBox from highlighting on ConfigureHotkeyForm initialization
            };
            Controls.Add(hotkeyTextbox);

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
            settingsManager.SetKeyCodes(hotkeyTextbox.GetSelectedKeyCodes());
            settingsManager.SetKeyText(hotkeyTextbox.GetTextboxText());
        }

        private void CloseButton_Click(object? sender, EventArgs e) {
            Close();
        }
    }
}