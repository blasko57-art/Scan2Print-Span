using System.Drawing;

namespace QuickCopier.App.UI.Status;

public sealed class StatusPanel : Panel
{
    private readonly Label _statusLabel;

    public StatusPanel()
    {
        Height = 30;

        _statusLabel = new Label
        {
            Text = "Starting QuickCopier...",
            AutoSize = true,
            Location = new Point(0, 5)
        };

        Controls.Add(_statusLabel);
    }

    public void SetStatus(string message)
    {
        _statusLabel.Text = message;
    }


}