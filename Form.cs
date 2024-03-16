using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Toggle_Muter
{
    public partial class Form : System.Windows.Forms.Form
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly GlobalKeyboardHook _keyboardHook;
        private readonly SettingsManager _settingsManager;
        private ConfigureHotkeyForm? _configureHotkeyForm;

        public Form()
        {
            InitializeComponent();
            _settingsManager = new SettingsManager(this, _keyboardHook);
            _keyboardHook = new GlobalKeyboardHook(this, _settingsManager);

            // Register the global keyboard hook
            GlobalKeyboardHook.RegisterHook();

            // Initialize the system tray icon
            _notifyIcon = new NotifyIcon
            {
                Text = "Toggle Muter",
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
            LoadSystemTrayIcon();
            SetupContextMenu();

            // Wire up the Form_Closing event handler
            FormClosing += Form_FormClosing;
        }

        // Set up the context menu for the system tray icon
        private void SetupContextMenu()
        {
            // "Autostart" menu item (checkbox)
            var autostartMenuItem = new ToolStripMenuItem("Autostart")
            {
                CheckOnClick = true,
                Checked = IsAppSetToRunAtStartup()
            };
            autostartMenuItem.Click += AutostartMenuItem_Click;
            _notifyIcon.ContextMenuStrip.Items.Add(autostartMenuItem);

            // Separator
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            // "Configure hotkey..." menu item (button)
            _notifyIcon.ContextMenuStrip.Items.Add("Configure Hotkey...", null, MenuConfigureHotkey_Click);

            // "Monochromatic Icon" menu item (checkbox)
            var monochromaticIconMenuItem = new ToolStripMenuItem("Monochromatic Icon")
            {
                CheckOnClick = true,
                Checked = _settingsManager.GetMonochromaticSysTrayIcon()
            };
            monochromaticIconMenuItem.Click += MonochromaticIconMenuItem_Click;
            _notifyIcon.ContextMenuStrip.Items.Add(monochromaticIconMenuItem);

            // Separator
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            // "Exit" menu item (button)
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, MenuExit_Click);
        }

        // Load the system tray icon based on the settings
        private void LoadSystemTrayIcon()
        {
            string iconResourceName = _settingsManager.GetMonochromaticSysTrayIcon()
                ? "Toggle_Muter.mono_icon.ico"
                : "Toggle_Muter.black_icon.ico";

            using (var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(iconResourceName))
            {
                if (iconStream != null)
                {
                    _notifyIcon.Icon = new Icon(iconStream);
                }
                else
                {
                    MessageBox.Show("Failed to load the system tray icon.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Check if the application is set to run at startup
        private bool IsAppSetToRunAtStartup()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                return key?.GetValue("Toggle Muter") != null;
            }
        }

        // Set the application to run at startup based on the provided value
        private void SetAppToRunAtStartup(bool runAtStartup)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (runAtStartup)
                {
                    key?.SetValue("Toggle Muter", Application.ExecutablePath);
                }
                else
                {
                    key?.DeleteValue("Toggle Muter", false);
                }
            }
        }

        // Event handler for the "Configure Hotkey..." menu item
        private void MenuConfigureHotkey_Click(object? sender, EventArgs e)
        {
            if (_configureHotkeyForm == null || _configureHotkeyForm.IsDisposed)
            {
                _configureHotkeyForm?.Dispose();
                _configureHotkeyForm = new ConfigureHotkeyForm(_settingsManager);
                _configureHotkeyForm.Show();
            }
            else
            {
                _configureHotkeyForm.BringToFront();
            }
        }

        // Event handler for the "Monochromatic Icon" menu item
        private void MonochromaticIconMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem != null)
            {
                _settingsManager.SetMonochromaticSysTrayIcon(menuItem.Checked);
                LoadSystemTrayIcon();
            }
        }

        // Event handler for the "Autostart" menu item
        private void AutostartMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                SetAppToRunAtStartup(menuItem?.Checked ?? false);
            }
        }

        // Event handler for the "Exit" menu item
        private void MenuExit_Click(object? sender, EventArgs e)
        {
            Close();
        }

        // Event handler for the form closing event
        private void Form_FormClosing(object? sender, FormClosingEventArgs e)
        {
            GlobalKeyboardHook.UnregisterHook();
            _configureHotkeyForm?.Dispose();
        }

        // Adjust the mute status of the current application in focus
        public void AdjustMuteStatus()
        {
            GetWindowThreadProcessId(GetForegroundWindow(), out var processId);

            var muteStatus = VolumeMixer.GetApplicationMute((int)processId);
            if (muteStatus == null)
            {
                Debug.WriteLine("The audio session of the application in focus could not be detected.");
                return;
            }

            VolumeMixer.SetApplicationMute((int)processId, !muteStatus.Value);
            Debug.WriteLine($"Current application '{processId}' has been {(muteStatus.Value ? "unmuted" : "muted")}.");
        }

        // Import the necessary Win32 API functions
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    }
}