using QuickCopier.App.UI.Actions;
using QuickCopier.App.UI.Devices;
using QuickCopier.App.UI.Status;
using QuickCopier.App.Workflows;
using QuickCopier.Core.Logging;

namespace QuickCopier.App;

public partial class Form1 : Form
{
    private readonly DocumentWorkflow _workflow;
    private readonly InMemoryLoggerProvider _logProvider;

    private readonly ScannerPanel _scannerPanel;
    private readonly PrinterPanel _printerPanel;
    private readonly CopyActionsPanel _actionsPanel;
    private readonly StatusPanel _statusPanel;

    private Button _logsButton = null!;

    public Form1(
        InMemoryLoggerProvider logProvider,
        DocumentWorkflow workflow,
        ScannerPanel scannerPanel,
        PrinterPanel printerPanel,
        CopyActionsPanel actionsPanel,
        StatusPanel statusPanel)
    {
        InitializeComponent();

        _logProvider = logProvider;
        _workflow = workflow;

        _scannerPanel = scannerPanel;
        _printerPanel = printerPanel;
        _actionsPanel = actionsPanel;
        _statusPanel = statusPanel;

        ConfigureForm();
        CreateControls();
        WireEvents();

        _ = LoadInitialDataAsync();
    }

    private void ConfigureForm()
    {
        Text = "QuickCopier";
        ClientSize = new Size(650, 400);
        StartPosition = FormStartPosition.CenterScreen;
    }

    private void CreateControls()
    {
        _scannerPanel.Location = new Point(30, 20);
        _scannerPanel.Width = 610;

        _printerPanel.Location = new Point(30, 105);
        _printerPanel.Width = 610;

        _statusPanel.Location = new Point(30, 190);
        _statusPanel.Width = 610;

        _actionsPanel.Location = new Point(30, 230);
        _actionsPanel.Width = 490;

        _logsButton = new Button
        {
            Text = "LOGS",
            Location = new Point(30, 290),
            Width = 150,
            Height = 40
        };

        Controls.Add(_scannerPanel);
        Controls.Add(_printerPanel);
        Controls.Add(_statusPanel);
        Controls.Add(_actionsPanel);
        Controls.Add(_logsButton);
    }

    private void WireEvents()
    {
        _scannerPanel.StatusChanged += (_, message) =>
            _statusPanel.SetStatus(message);

        _printerPanel.StatusChanged += (_, message) =>
            _statusPanel.SetStatus(message);

        _actionsPanel.ScanClicked += async (_, _) =>
            await HandleScanAsync();

        _actionsPanel.PrintClicked += async (_, _) =>
            await HandlePrintAsync();

        _actionsPanel.CopyClicked += async (_, _) =>
            await HandleCopyAsync();

        _logsButton.Click += (_, _) =>
            ShowLogs();
    }

    private async Task LoadInitialDataAsync()
    {
        try
        {
            await _scannerPanel.LoadScannersAsync();
            _printerPanel.LoadPrinters();

            _statusPanel.SetStatus(
                "QuickCopier ready.");

            UpdateActionState();
        }
        catch (Exception ex)
        {
            ShowError(
                "Startup Error",
                ex.Message);
        }
    }

    private async Task HandleScanAsync()
    {
        var scanner =
            _scannerPanel.SelectedScanner;

        if (scanner is null)
        {
            ShowInformation(
                "Please select a scanner first.");

            return;
        }

        try
        {
            SetBusy(true);

            _statusPanel.SetStatus(
                "Scanning...");

            var document =
                await _workflow.ScanAsync(scanner);

            _statusPanel.SetStatus(
                $"Scan successful: " +
                $"{document.Data.Length / 1024} KB (RAM)");
        }
        catch (Exception ex)
        {
            _workflow.ReleaseDocument();

            _statusPanel.SetStatus(
                "Scan failed.");

            ShowError(
                "Scanner Error",
                ex.Message);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task HandlePrintAsync()
    {
        var printer =
            _printerPanel.SelectedPrinter;

        if (printer is null)
        {
            ShowInformation(
                "Please select a printer first.");

            return;
        }

        if (!_workflow.HasDocument)
        {
            ShowInformation(
                "There is no scanned document in RAM.");

            return;
        }

        try
        {
            SetBusy(true);

            _statusPanel.SetStatus(
                $"Printing to {printer.Name}...");

            await _workflow.PrintAsync(printer);

            _statusPanel.SetStatus(
                "Print job sent successfully.");
        }
        catch (Exception ex)
        {
            _statusPanel.SetStatus(
                "Printing failed.");

            ShowError(
                "Printer Error",
                ex.Message);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task HandleCopyAsync()
    {
        var scanner =
            _scannerPanel.SelectedScanner;

        var printer =
            _printerPanel.SelectedPrinter;

        if (scanner is null || printer is null)
        {
            ShowInformation(
                "Please select both a scanner and a printer.");

            return;
        }

        try
        {
            SetBusy(true);

            _statusPanel.SetStatus(
                "Copying...");

            await _workflow.CopyAsync(
                scanner,
                printer);

            _statusPanel.SetStatus(
                "Copy completed successfully.");
        }
        catch (Exception ex)
        {
            _statusPanel.SetStatus(
                "Copy failed.");

            ShowError(
                "QuickCopier Error",
                ex.Message);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy)
    {
        _actionsPanel.SetEnabled(!busy);

        _actionsPanel.SetPrintEnabled(
            !busy &&
            _workflow.HasDocument);
    }

    private void UpdateActionState()
    {
        _actionsPanel.SetPrintEnabled(
            _workflow.HasDocument);
    }

    private void ShowLogs()
    {
        using var logsForm =
            new LogsForm(_logProvider);

        logsForm.ShowDialog(this);
    }

    private void ShowInformation(string message)
    {
        MessageBox.Show(
            this,
            message,
            "QuickCopier",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ShowError(
        string title,
        string message)
    {
        MessageBox.Show(
            this,
            message,
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    protected override void OnFormClosed(
        FormClosedEventArgs e)
    {
        _workflow.ReleaseDocument();

        base.OnFormClosed(e);
    }


}