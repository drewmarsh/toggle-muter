using System.Reflection;

namespace Toggle_Muter;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Set the icon for the application
        Icon appIcon = LoadApplicationIcon();
        Application.Run(new Form() { Icon = appIcon });
    }

    public static Icon LoadApplicationIcon()
    {
        using (Stream iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Toggle_Muter.application_icon.ico")!)
        {
            if (iconStream != null)
            {
                return new Icon(iconStream);
            }
            else
            {
                MessageBox.Show("Failed to load embedded icon resource.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return SystemIcons.Application; // Return the default application icon
            }
        }
    }
}