using QuickCopier.Core.Logging;
using QuickCopier.Core.Models;
using QuickCopier.Windows.Printing;
using QuickCopier.Windows.Scanning;
using QuickCopier.Core.Interfaces;

namespace QuickCopier.App;

public partial class Form1 : Form
{
private readonly IScannerService _scannerService;
private readonly IPrinterService _printerService;

    private readonly InMemoryLoggerProvider _logProvider;

    private ComboBox _scannerComboBox = null!;
    private ComboBox _printerComboBox = null!;

    private Button _refreshScannerButton = null!;
    private Button _refreshPrinterButton = null!;
    private Button _scanButton = null!;
    private Button _printButton = null!;
    private Button _copyButton = null!;
    private Button _logsButton = null!;


    private Button _inspectScannerButton = null!;
    // temp inspect for srttings
    private Label _statusLabel = null!;

    private IReadOnlyList<ScannerDevice> _scanners =
        Array.Empty<ScannerDevice>();

    private IReadOnlyList<PrinterDevice> _printers =
        Array.Empty<PrinterDevice>();

    private ScannedDocument? _scannedDocument;

public Form1(
    InMemoryLoggerProvider logProvider,
    IScannerService scannerService,
    IPrinterService printerService)
{
    InitializeComponent();

    _logProvider = logProvider;

    _scannerService = scannerService;
    _printerService = printerService;

    CreateControls();

    _ = LoadScannersAsync();
    LoadPrinters();
}



    private void CreateControls()
    {
        Text = "QuickCopier";
        ClientSize = new Size(600, 350);
        StartPosition = FormStartPosition.CenterScreen;

        // -------------------------
        // Scanner
        // -------------------------

        var scannerLabel = new Label
        {
            Text = "Scanner:",
            Location = new Point(30, 25),
            AutoSize = true
        };

        _scannerComboBox = new ComboBox
        {
            Location = new Point(30, 50),
            Width = 400,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        _refreshScannerButton = new Button
        {
            Text = "Refresh",
            Location = new Point(450, 49),
            Width = 90
        };

        _refreshScannerButton.Click += async (_, _) =>
        {
            await LoadScannersAsync();
        };

        // -------------------------
        // Printer
        // -------------------------

        var printerLabel = new Label
        {
            Text = "Printer:",
            Location = new Point(30, 95),
            AutoSize = true
        };

        _printerComboBox = new ComboBox
        {
            Location = new Point(30, 120),
            Width = 400,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        _refreshPrinterButton = new Button
        {
            Text = "Refresh",
            Location = new Point(450, 119),
            Width = 90
        };

        _refreshPrinterButton.Click += (_, _) =>
        {
            LoadPrinters();
        };

        // -------------------------
        // Status
        // -------------------------

        _statusLabel = new Label
        {
            Text = "Starting QuickCopier...",
            Location = new Point(30, 170),
            AutoSize = true
        };

        // -------------------------
        // Scan button
        // -------------------------

        _scanButton = new Button
        {
            Text = "SCAN",
            Location = new Point(30, 210),
            Width = 150,
            Height = 40
        };

        _scanButton.Click += async (_, _) =>
        {
            await ScanAsync();
        };

        // -------------------------
        // Print button
        // -------------------------

        _printButton = new Button
        {
            Text = "PRINT",
            Location = new Point(200, 210),
            Width = 150,
            Height = 40,
            Enabled = false
        };

        _printButton.Click += async (_, _) =>
        {
            await PrintAsync();
        };

        // -------------------------
        // Copy button
        // -------------------------

        _copyButton = new Button
        {
            Text = "COPY",
            Location = new Point(370, 210),
            Width = 150,
            Height = 40
        };

        _copyButton.Click += async (_, _) =>
        {
            await CopyAsync();
        };

        _logsButton = new Button
        {
            Text = "LOGS",
            Location = new Point(30, 270),
            Width = 150,
            Height = 40
        };

        _logsButton.Click += (_, _) =>
        {
            using var logsForm = new LogsForm(_logProvider);
            logsForm.ShowDialog(this);
        };



        //temp setting button
        _inspectScannerButton = new Button
        {
            Text = "INSPECT",
            Location = new Point(370, 210),
            Width = 150,
            Height = 40
        };

        _inspectScannerButton.Click += (_, _) =>
        {
            InspectSelectedScanner();
        };
        Controls.Add(_inspectScannerButton);




        // -------------------------
        // Add controls
        // -------------------------

        Controls.Add(scannerLabel);
        Controls.Add(_scannerComboBox);
        Controls.Add(_refreshScannerButton);

        Controls.Add(printerLabel);
        Controls.Add(_printerComboBox);
        Controls.Add(_refreshPrinterButton);

        Controls.Add(_statusLabel);

        Controls.Add(_scanButton);
        Controls.Add(_printButton);
        Controls.Add(_copyButton);
        Controls.Add(_logsButton);
        Controls.Add(_logsButton);

    }

    private async Task ScanAsync()
    {
        if (_scannerComboBox.SelectedIndex < 0)
        {
            MessageBox.Show(
                this,
                "Please select a scanner first.",
                "QuickCopier",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        var scanner =
            _scanners[_scannerComboBox.SelectedIndex];

        try
        {
            _statusLabel.Text = "Scanning...";

            _scanButton.Enabled = false;
            _printButton.Enabled = false;
            _copyButton.Enabled = false;

            // Release previous scan from RAM.
            _scannedDocument?.Dispose();
            _scannedDocument = null;

            _scannedDocument =
                await _scannerService.ScanAsync(
                    scanner,
                    new ScanSettings());

            _statusLabel.Text =
                $"Scan successful: {_scannedDocument.Data.Length / 1024} KB (RAM)";

            // IMPORTANT:
            // The scanned document is now still in RAM.
            _printButton.Enabled = true;
        }
        catch (Exception ex)
        {
            _scannedDocument?.Dispose();
            _scannedDocument = null;

            _statusLabel.Text = "Scan failed.";

            MessageBox.Show(
                this,
                ex.Message,
                "Scanner Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _scanButton.Enabled = true;
            _copyButton.Enabled = true;
        }
    }

    private async Task PrintAsync()
    {
        if (_scannedDocument == null)
        {
            MessageBox.Show(
                this,
                "There is no scanned document in RAM.",
                "QuickCopier",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (_printerComboBox.SelectedIndex < 0)
        {
            MessageBox.Show(
                this,
                "Please select a printer first.",
                "QuickCopier",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        var printer =
            _printers[_printerComboBox.SelectedIndex];

        try
        {
            _statusLabel.Text =
                $"Printing to {printer.Name}...";

            _scanButton.Enabled = false;
            _printButton.Enabled = false;
            _copyButton.Enabled = false;

            await _printerService.PrintAsync(
                printer,
                _scannedDocument,
                new PrintSettings());

            _statusLabel.Text =
                "Print job sent successfully.";

            // Release the scanned document from RAM.
            _scannedDocument.Dispose();
            _scannedDocument = null;
        }
        catch (Exception ex)
        {
            _statusLabel.Text =
                "Printing failed.";

            MessageBox.Show(
                this,
                ex.Message,
                "Printer Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _scanButton.Enabled = true;
            _copyButton.Enabled = true;

            if (_scannedDocument != null)
            {
                _printButton.Enabled = true;
            }
        }
    }

    private async Task CopyAsync()
    {
        if (_scannerComboBox.SelectedIndex < 0)
        {
            MessageBox.Show(
                this,
                "Please select a scanner first.",
                "QuickCopier",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (_printerComboBox.SelectedIndex < 0)
        {
            MessageBox.Show(
                this,
                "Please select a printer first.",
                "QuickCopier",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        var scanner =
            _scanners[_scannerComboBox.SelectedIndex];

        var printer =
            _printers[_printerComboBox.SelectedIndex];

        try
        {
            _statusLabel.Text =
                "Scanning...";

            _scanButton.Enabled = false;
            _printButton.Enabled = false;
            _copyButton.Enabled = false;

            // Make sure no previous scanned document
            // remains in RAM.
            _scannedDocument?.Dispose();
            _scannedDocument = null;

            // -------------------------
            // Scan
            // -------------------------

            _scannedDocument =
                await _scannerService.ScanAsync(
                    scanner,
                    new ScanSettings());

            _statusLabel.Text =
                $"Scan successful: {_scannedDocument.Data.Length / 1024} KB (RAM)";

            // -------------------------
            // Print
            // -------------------------

            _statusLabel.Text =
                $"Printing to {printer.Name}...";

            await _printerService.PrintAsync(
                printer,
                _scannedDocument,
                new PrintSettings());

            // -------------------------
            // Release RAM
            // -------------------------

            _scannedDocument.Dispose();
            _scannedDocument = null;

            _statusLabel.Text =
                "Copy completed successfully.";
        }
        catch (Exception ex)
        {
            _statusLabel.Text =
                "Copy failed.";

            MessageBox.Show(
                this,
                ex.Message,
                "QuickCopier Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _scanButton.Enabled = true;
            _copyButton.Enabled = true;

            if (_scannedDocument != null)
            {
                _printButton.Enabled = true;
            }
            else
            {
                _printButton.Enabled = false;
            }
        }
    }

    private async Task LoadScannersAsync()
    {
        try
        {
            _statusLabel.Text =
                "Searching for scanners...";

            _refreshScannerButton.Enabled = false;

            _scanners =
                await _scannerService.GetScannersAsync();

            _scannerComboBox.Items.Clear();

            foreach (var scanner in _scanners)
            {
                _scannerComboBox.Items.Add(scanner.Name);
            }

            if (_scannerComboBox.Items.Count > 0)
            {
                _scannerComboBox.SelectedIndex = 0;

                _statusLabel.Text =
                    $"Found {_scannerComboBox.Items.Count} scanner(s).";
            }
            else
            {
                _statusLabel.Text =
                    "No scanners found.";
            }
        }
        catch (Exception ex)
        {
            _statusLabel.Text =
                "Scanner discovery failed.";

            MessageBox.Show(
                this,
                ex.Message,
                "Scanner Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _refreshScannerButton.Enabled = true;
        }
    }

    private void LoadPrinters()
    {
        try
        {
            _refreshPrinterButton.Enabled = false;

            _printers =
                _printerService.GetPrinters();

            _printerComboBox.Items.Clear();

            foreach (var printer in _printers)
            {
                _printerComboBox.Items.Add(printer.Name);
            }

            if (_printerComboBox.Items.Count > 0)
            {
                _printerComboBox.SelectedIndex = 0;

                _statusLabel.Text =
                    $"Found {_printerComboBox.Items.Count} printer(s).";
            }
            else
            {
                _statusLabel.Text =
                    "No printers found.";
            }
        }
        catch (Exception ex)
        {
            _statusLabel.Text =
                "Printer discovery failed.";

            MessageBox.Show(
                this,
                ex.Message,
                "Printer Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _refreshPrinterButton.Enabled = true;
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _scannedDocument?.Dispose();
        _scannedDocument = null;

        base.OnFormClosed(e);
    }



    private void InspectSelectedScanner()
    {
        if (_scannerComboBox.SelectedIndex < 0)
        {
            MessageBox.Show(
                this,
                "Please select a scanner first.",
                "QuickCopier",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        try
        {
            var scanner =
                _scanners[_scannerComboBox.SelectedIndex];

            var inspector =
                new WindowsScannerCapabilities();

            var result =
                inspector.Inspect(scanner.Id);

            using var dialog = new Form
            {
                Text = "Scanner Capabilities",
                Width = 900,
                Height = 700,
                StartPosition = FormStartPosition.CenterParent
            };

            var textBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                Text = result
            };

            dialog.Controls.Add(textBox);

            dialog.ShowDialog(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Scanner Inspection Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

}