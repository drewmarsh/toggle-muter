namespace Toggle_Muter {
    public partial class ConfigureHotkeyForm : System.Windows.Forms.Form {
        private HotkeyTextbox hotkeyTextbox;
        private SettingsManager _settingsManager;
        private const int ButtonSpacing = 10;
        private Button confirmButton;
        private Button closeButton;
        
        public ConfigureHotkeyForm(SettingsManager settingsManager) {
            InitializeComponent();
            _settingsManager = settingsManager;
            Icon = Program.LoadApplicationIcon(); // Set the application icon

            ShowHotkeyConfig();

            // Add the "Confirm" button on the left
            confirmButton = new Button {
                Text = "Confirm",
                FlatStyle = FlatStyle.System,
                TabStop = false
            };
            confirmButton.Click += ConfirmButton_Click;
            confirmButton.Location = new Point(ClientSize.Width / 2 - confirmButton.Width - ButtonSpacing, 60);
            Controls.Add(confirmButton);

            // Add the "Close" button on the right
            closeButton = new Button {
                Text = "Close",
                FlatStyle = FlatStyle.System,
                TabStop = false
            };
            closeButton.Click += CloseButton_Click;
            closeButton.Location = new Point(ClientSize.Width / 2 + ButtonSpacing, 60);
            Controls.Add(closeButton);

            // Create and configure the HotkeyTextbox control
            hotkeyTextbox = new HotkeyTextbox(settingsManager.GetKeyCodes(), settingsManager.GetKeyText(), this, settingsManager)
            {
                TabStop = false
            };
            hotkeyTextbox.WidthChanged += HotkeyTextbox_WidthChanged;
            CenterHotkeyTextbox();
            Controls.Add(hotkeyTextbox);

            // Handle the form's Activated and Deactivate events
            Activated += ConfigureHotkeyForm_Activated;
            Deactivate += ConfigureHotkeyForm_Deactivated;

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
            _settingsManager.SetKeyCodesAndText(hotkeyTextbox.GetSelectedKeyCodes(), hotkeyTextbox.GetTextboxText());
            hotkeyTextbox.UpdateHotkeyText();
            hotkeyTextbox.ForeColor = Color.Black;
        }

        private void CloseButton_Click(object? sender, EventArgs e) {
            Close();
        }

        private void HotkeyTextbox_WidthChanged(object? sender, EventArgs e) {
            CenterHotkeyTextbox();
        }

        private void CenterHotkeyTextbox() {
            hotkeyTextbox.Location = new Point((ClientSize.Width - hotkeyTextbox.Width) / 2, 19);
        }

        private void ConfigureHotkeyForm_Activated(object? sender, EventArgs e) {
            // Enable the Confirm and Close buttons when the form is activated
            confirmButton.Enabled = true;
            closeButton.Enabled = true;
        }

        private void ConfigureHotkeyForm_Deactivated(object? sender, EventArgs e) {
            // Disable the Confirm and Close buttons when the form loses focus
            confirmButton.Enabled = false;
            closeButton.Enabled = false;
        }
    }
}