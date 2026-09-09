using System.Drawing;
using System.Windows.Forms;

namespace QuickCopier.App.UI.Actions;

public sealed class CopyActionsPanel : Panel
{
    private readonly Button _scanButton;
    private readonly Button _printButton;
    private readonly Button _copyButton;

    public event EventHandler? ScanClicked;
    public event EventHandler? PrintClicked;
    public event EventHandler? CopyClicked;

    public CopyActionsPanel()
    {
        Height = 40;
        Width = 490;

        _scanButton = new Button
        {
            Text = "SCAN",
            Location = new Point(0, 0),
            Width = 150,
            Height = 40
        };

        _printButton = new Button
        {
            Text = "PRINT",
            Location = new Point(170, 0),
            Width = 150,
            Height = 40,
            Enabled = false
        };

        _copyButton = new Button
        {
            Text = "COPY",
            Location = new Point(340, 0),
            Width = 150,
            Height = 40
        };

        _scanButton.Click += (_, _) =>
        {
            ScanClicked?.Invoke(this, EventArgs.Empty);
        };

        _printButton.Click += (_, _) =>
        {
            PrintClicked?.Invoke(this, EventArgs.Empty);
        };

        _copyButton.Click += (_, _) =>
        {
            CopyClicked?.Invoke(this, EventArgs.Empty);
        };

        Controls.Add(_scanButton);
        Controls.Add(_printButton);
        Controls.Add(_copyButton);
    }

    public void SetEnabled(bool enabled)
    {
        _scanButton.Enabled = enabled;
        _copyButton.Enabled = enabled;
    }

    public void SetPrintEnabled(bool enabled)
    {
        _printButton.Enabled = enabled;
    }


}