namespace OsWarningDemo.App;

public class MainForm : Form
{
    public MainForm(bool simulateUnsupportedOs)
    {
        Text = "OS Warning Demo";
        ClientSize = new Size(640, 360);
        StartPosition = FormStartPosition.CenterScreen;

        var os = OsSupport.GetCurrent();
        var osName = OsSupport.GetFriendlyName(os);
        var supported = OsSupport.IsSupported(os) && !simulateUnsupportedOs;

        var content = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(Font.FontFamily, 12f),
            Text = $"Hello from OsWarningDemo!\n\nDetected OS: {osName}\nSupported: {(supported ? "yes" : "no")}",
        };
        Controls.Add(content);

        // Non-blocking warning: a dismissible banner instead of a modal dialog,
        // so the user can keep working on an unsupported OS.
        if (!supported)
            Controls.Add(CreateUnsupportedOsBanner(osName));
    }

    private static Panel CreateUnsupportedOsBanner(string osName)
    {
        var banner = new Panel
        {
            Dock = DockStyle.Top,
            Height = 56,
            BackColor = Color.FromArgb(255, 244, 206),
            Padding = new Padding(12, 8, 8, 8),
        };

        var icon = new PictureBox
        {
            Image = SystemIcons.Warning.ToBitmap(),
            SizeMode = PictureBoxSizeMode.CenterImage,
            Dock = DockStyle.Left,
            Width = 40,
        };

        var message = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(80, 60, 0),
            Text = $"{osName} is not a supported operating system. The app may not work correctly. " +
                   "Supported systems: Windows 10 and Windows 11.",
        };

        var dismiss = new Button
        {
            Text = "Dismiss",
            Dock = DockStyle.Right,
            Width = 80,
            FlatStyle = FlatStyle.System,
        };
        dismiss.Click += (_, _) => banner.Visible = false;

        // Dock order: last added docks first, so add Fill last.
        banner.Controls.Add(message);
        banner.Controls.Add(icon);
        banner.Controls.Add(dismiss);
        message.BringToFront();
        return banner;
    }
}
