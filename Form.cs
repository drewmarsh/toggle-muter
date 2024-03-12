using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Toggle_Muter {
    public partial class Form : System.Windows.Forms.Form {
        private readonly NotifyIcon notifyIcon;
        private GlobalKeyboardHook _keyboardHook;
        private ConfigureHotkeyForm? configureHotkeyForm;
        private SettingsManager settingsManager;

        public Form() {
            InitializeComponent();
            settingsManager = new SettingsManager(this, _keyboardHook);
            _keyboardHook = new GlobalKeyboardHook(this, settingsManager);
            GlobalKeyboardHook.RegisterHook();
            
            configureHotkeyForm = null;

            notifyIcon = new NotifyIcon {
                Text = "Toggle Muter",
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
            LoadSystemTrayIcon();

            // Add Context menu items
            // "Autostart" menu item (checkbox)
            var autostartMenuItem = new ToolStripMenuItem("Autostart") {
                CheckOnClick = true,
                Checked = IsAppSetToRunAtStartup() // Set it based on current startup behavior
            };
            autostartMenuItem.Click += AutostartMenuItem_Click;
            notifyIcon.ContextMenuStrip.Items.Add(autostartMenuItem);
            
            // Separator
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            
            // "Configure hotkey..." menu item (button)
            notifyIcon.ContextMenuStrip.Items.Add("Configure Hotkey...", null, MenuConfigureHotkey_Click);

            // "Monochromatic Icon" menu item (checkbox)
            var monochromaticIconMenuItem = new ToolStripMenuItem("Monochromatic Icon") {
                CheckOnClick = true,
                Checked = settingsManager.GetMonochromaticSysTrayIcon()  // Set it based on saved setting
            };
            monochromaticIconMenuItem.Click += MonochromaticIconMenuItem_Click;
            notifyIcon.ContextMenuStrip.Items.Add(monochromaticIconMenuItem);

            // Separator
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator()); 
            
            // "Exit" menu item (button)
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, MenuExit_Click);

            // Wire up the Form_Closing event handler
            Closing += Form_Closing;
        }

        private void Form_Closing(object? sender, CancelEventArgs e)
        {
            GlobalKeyboardHook.UnregisterHook();
        }

        // Loads the system tray icon
        private void LoadSystemTrayIcon() {
        string iconResourceName = settingsManager.GetMonochromaticSysTrayIcon() ? "Toggle_Muter.mono_icon.ico" : "Toggle_Muter.black_icon.ico";
        
        using (var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(iconResourceName)) {
                if (iconStream != null) {
                    notifyIcon.Icon = new Icon(iconStream);
                }
                else {
                    MessageBox.Show("Failed to load embedded icon resource.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }   
        }

        // Check if app is set to run at Startup
        private bool IsAppSetToRunAtStartup() {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            return key?.GetValue("Toggle Muter") != null;
        }

        // Set the app to run at Startup
        private void SetAppToRunAtStartup(bool runAtStartup) {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (runAtStartup) {
                key?.SetValue("Toggle Muter", "\"" + Application.ExecutablePath + "\"");
            }
            else {
                key?.DeleteValue("Toggle Muter", false);
            }
        }

        #region Context Menu Event Handlers

        // Event handler for the "Configure Hotkey..." menu item
        private void MenuConfigureHotkey_Click(object? sender, EventArgs e) {
                if (configureHotkeyForm == null || configureHotkeyForm.IsDisposed) {
                    configureHotkeyForm = new ConfigureHotkeyForm(settingsManager);
                    configureHotkeyForm.Show();
                }
                else {
                    configureHotkeyForm.Focus();
                }
        }

       // Event handler for the "Monochromatic Icon" menu item
        private void MonochromaticIconMenuItem_Click(object? sender, EventArgs e) {
            if (sender is ToolStripMenuItem menuItem) {
                bool isChecked = menuItem.Checked;

                settingsManager.SetMonochromaticSysTrayIcon(isChecked);

                LoadSystemTrayIcon();
            }
        }

        // Event handler for the "Autostart" menu item
        private void AutostartMenuItem_Click(object? sender, EventArgs e) {
            if (sender is ToolStripMenuItem menuItem) {
                bool isChecked = menuItem.Checked;
                SetAppToRunAtStartup(isChecked);
            }
        }

        // Event handler for the "Exit" menu item
        private void MenuExit_Click(object? sender, EventArgs e) {
            Close();
        }

        #endregion

        #region Audio Adjustment

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // Adjusts the mute status based on the application in focus
        public void AdjustMuteStatus() {
            GetWindowThreadProcessId(GetForegroundWindow(), out var processID);

            var muteStatus = VolumeMixer.GetApplicationMute((int)processID);
            if (muteStatus == null) {
                Console.WriteLine("The audio session of the application in focus could not be detected");
                return;
            }

            VolumeMixer.SetApplicationMute((int)processID, !muteStatus.Value);
            Console.WriteLine($"Current application '{processID}' has been {(muteStatus.Value ? "unmuted" : "muted")}");
        }

        #endregion
    } 
}