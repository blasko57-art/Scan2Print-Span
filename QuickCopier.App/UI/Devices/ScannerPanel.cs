using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Models;

namespace QuickCopier.App.UI.Devices;

public sealed class ScannerPanel : UserControl
{
    private readonly IScannerService _scannerService;
    private readonly IScannerCapabilities _scannerCapabilities;

    private ComboBox _scannerComboBox = null!;
    private Button _refreshButton = null!;
    private Button _inspectButton = null!;

    private IReadOnlyList<ScannerDevice> _scanners =
        Array.Empty<ScannerDevice>();

    public ScannerPanel(
        IScannerService scannerService,
        IScannerCapabilities scannerCapabilities)
    {
        _scannerService = scannerService;
        _scannerCapabilities = scannerCapabilities;

        CreateControls();
    }

    public ScannerDevice? SelectedScanner
    {
        get
        {
            if (_scannerComboBox.SelectedIndex < 0)
            {
                return null;
            }

            return _scanners[_scannerComboBox.SelectedIndex];
        }
    }

    public event EventHandler<string>? StatusChanged;

    private void CreateControls()
    {
        Dock = DockStyle.Top;
        Height = 80;

        var scannerLabel = new Label
        {
            Text = "Scanner:",
            Location = new Point(0, 0),
            AutoSize = true
        };

        _scannerComboBox = new ComboBox
        {
            Location = new Point(0, 25),
            Width = 400,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        _refreshButton = new Button
        {
            Text = "Refresh",
            Location = new Point(410, 24),
            Width = 90
        };

        _inspectButton = new Button
        {
            Text = "Inspect",
            Location = new Point(510, 24),
            Width = 90
        };

        _refreshButton.Click += async (_, _) =>
        {
            await LoadScannersAsync();
        };

        _inspectButton.Click += (_, _) =>
        {
            InspectSelectedScanner();
        };

        Controls.Add(scannerLabel);
        Controls.Add(_scannerComboBox);
        Controls.Add(_refreshButton);
        Controls.Add(_inspectButton);
    }

    public async Task LoadScannersAsync()
    {
        try
        {
            SetStatus("Searching for scanners...");

            _refreshButton.Enabled = false;
            _scannerComboBox.Enabled = false;
            _inspectButton.Enabled = false;

            _scanners =
                await _scannerService.GetScannersAsync();

            _scannerComboBox.Items.Clear();

            foreach (var scanner in _scanners)
            {
                _scannerComboBox.Items.Add(scanner.Name);
            }

            if (_scanners.Count > 0)
            {
                _scannerComboBox.SelectedIndex = 0;
                SetStatus($"Found {_scanners.Count} scanner(s).");
            }
            else
            {
                SetStatus("No scanners found.");
            }
        }
        catch (Exception ex)
        {
            SetStatus("Scanner discovery failed.");

            MessageBox.Show(
                FindForm(),
                ex.Message,
                "Scanner Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _refreshButton.Enabled = true;
            _scannerComboBox.Enabled = true;
            _inspectButton.Enabled = _scanners.Count > 0;
        }
    }

    private void InspectSelectedScanner()
    {
        var scanner = SelectedScanner;

        if (scanner is null)
        {
            MessageBox.Show(
                FindForm(),
                "Please select a scanner first.",
                "QuickCopier",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        try
        {
            var result =
                _scannerCapabilities.Inspect(scanner.Id);

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

            dialog.ShowDialog(FindForm());
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                FindForm(),
                ex.Message,
                "Scanner Inspection Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void SetStatus(string message)
    {
        StatusChanged?.Invoke(this, message);
    }


}