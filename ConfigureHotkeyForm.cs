namespace Toggle_Muter {
    public partial class ConfigureHotkeyForm : System.Windows.Forms.Form {
        private HotkeyTextbox hotkeyTextbox;
        private SettingsManager settingsManager;
        private const int ButtonSpacing = 10;
        
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
            confirmButton.Location = new Point(ClientSize.Width / 2 - confirmButton.Width - ButtonSpacing, 60);
            Controls.Add(confirmButton);

            // Add the "Close" button on the right
            Button closeButton = new Button {
                Text = "Close",
                FlatStyle = FlatStyle.System
            };
            closeButton.Click += CloseButton_Click;
            closeButton.Location = new Point(ClientSize.Width / 2 + ButtonSpacing, 60);
            Controls.Add(closeButton);

            // Create and configure the HotkeyTextbox control
            hotkeyTextbox = new HotkeyTextbox(settingsManager.GetKeyCodes(), settingsManager.GetKeyText());
            hotkeyTextbox.WidthChanged += HotkeyTextbox_WidthChanged;
            CenterHotkeyTextbox();
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
            Text = "Configure Hotkey";
            ResumeLayout(false);
        }

        // Shows the hotkey configuration window
        private void ShowHotkeyConfig() {
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            ControlBox = false;
            TopMost = true;
            Opacity = 100;
        }

        private void ConfirmButton_Click(object? sender, EventArgs e) {
            settingsManager.SetKeyCodesAndText(hotkeyTextbox.GetSelectedKeyCodes(), hotkeyTextbox.GetTextboxText());
        }

        private void CloseButton_Click(object? sender, EventArgs e) {
            Close();
        }

        private void HotkeyTextbox_WidthChanged(object sender, EventArgs e) {
            CenterHotkeyTextbox();
        }

        private void CenterHotkeyTextbox() {
            hotkeyTextbox.Location = new Point((ClientSize.Width - hotkeyTextbox.Width) / 2, 19);
        }
    }
}