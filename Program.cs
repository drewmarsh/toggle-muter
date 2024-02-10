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
            using (Stream iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Toggle_Muter.application_icon.ico")) {
                if (iconStream != null) {
                    Icon appIcon = new Icon(iconStream);
                    Application.Run(new Form() { Icon = appIcon });
                }
                else {
                    MessageBox.Show("Failed to load embedded icon resource.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
    }    
}