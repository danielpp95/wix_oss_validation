namespace OsWarningDemo.App;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    ///  Pass --simulate-unsupported-os to show the warning banner on a supported OS.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        var simulateUnsupportedOs = args.Contains("--simulate-unsupported-os", StringComparer.OrdinalIgnoreCase);
        Application.Run(new MainForm(simulateUnsupportedOs));
    }
}
