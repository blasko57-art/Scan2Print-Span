using QuickCopier.Core.Interfaces;
using QuickCopier.Core.Models;

namespace QuickCopier.App.UI.Devices;

public sealed class PrinterPanel : UserControl
{
    private readonly IPrinterService _printerService;

    private ComboBox _printerComboBox = null!;
    private Button _refreshButton = null!;

    private IReadOnlyList<PrinterDevice> _printers =
        Array.Empty<PrinterDevice>();

    public PrinterPanel(IPrinterService printerService)
    {
        _printerService = printerService;

        CreateControls();
    }

    public PrinterDevice? SelectedPrinter
    {
        get
        {
            if (_printerComboBox.SelectedIndex < 0)
            {
                return null;
            }

            return _printers[_printerComboBox.SelectedIndex];
        }
    }

    public event EventHandler<string>? StatusChanged;

    private void CreateControls()
    {
        Dock = DockStyle.Top;
        Height = 80;

        var printerLabel = new Label
        {
            Text = "Printer:",
            Location = new Point(0, 0),
            AutoSize = true
        };

        _printerComboBox = new ComboBox
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

        _refreshButton.Click += (_, _) =>
        {
            LoadPrinters();
        };

        Controls.Add(printerLabel);
        Controls.Add(_printerComboBox);
        Controls.Add(_refreshButton);
    }

    public void LoadPrinters()
    {
        try
        {
            SetStatus("Searching for printers...");

            _refreshButton.Enabled = false;
            _printerComboBox.Enabled = false;

            _printers =
                _printerService.GetPrinters();

            _printerComboBox.Items.Clear();

            foreach (var printer in _printers)
            {
                _printerComboBox.Items.Add(printer.Name);
            }

            if (_printers.Count > 0)
            {
                _printerComboBox.SelectedIndex = 0;

                SetStatus(
                    $"Found {_printers.Count} printer(s).");
            }
            else
            {
                SetStatus("No printers found.");
            }
        }
        catch (Exception ex)
        {
            SetStatus("Printer discovery failed.");

            MessageBox.Show(
                FindForm(),
                ex.Message,
                "Printer Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _refreshButton.Enabled = true;
            _printerComboBox.Enabled = true;
        }
    }

    private void SetStatus(string message)
    {
        StatusChanged?.Invoke(this, message);
    }


}