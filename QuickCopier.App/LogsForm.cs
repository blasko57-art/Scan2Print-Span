using QuickCopier.Core.Logging;

namespace QuickCopier.App;

public sealed class LogsForm : Form
{
    private readonly InMemoryLoggerProvider _logProvider;

    private readonly ListView _logListView;
    private readonly Button _clearButton;
    private readonly Button _refreshButton;

    public LogsForm(InMemoryLoggerProvider logProvider)
    {
        _logProvider = logProvider;

        Text = "QuickCopier - Logs";
        ClientSize = new Size(900, 500);
        StartPosition = FormStartPosition.CenterParent;

        // ----------------------------------------
        // Log list
        // ----------------------------------------

        _logListView = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            HideSelection = false
        };

        _logListView.Columns.Add("Time", 150);
        _logListView.Columns.Add("Level", 100);
        _logListView.Columns.Add("Category", 220);
        _logListView.Columns.Add("Message", 400);

        // ----------------------------------------
        // Buttons
        // ----------------------------------------

        _clearButton = new Button
        {
            Text = "Clear",
            Width = 100,
            Height = 30
        };

        _refreshButton = new Button
        {
            Text = "Refresh",
            Width = 100,
            Height = 30
        };

        _clearButton.Click += (_, _) =>
        {
            ClearLogs();
        };

        _refreshButton.Click += (_, _) =>
        {
            RefreshLogs();
        };

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 45,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(5)
        };

        buttonPanel.Controls.Add(_refreshButton);
        buttonPanel.Controls.Add(_clearButton);

        Controls.Add(_logListView);
        Controls.Add(buttonPanel);

        // Show existing logs immediately.
        RefreshLogs();
    }

    private void RefreshLogs()
    {
        _logListView.Items.Clear();

        foreach (var entry in _logProvider.Entries)

        {
            var item = new ListViewItem(
                entry.Timestamp.ToString("HH:mm:ss"));

            item.SubItems.Add(entry.Level.ToString());
            item.SubItems.Add(entry.Category);
            var message = entry.Message;

            if (entry.Exception is not null)
            {
                message += $" | {entry.Exception.Message}"; // show error condition if error
            }

            item.SubItems.Add(message);


            if (entry.Level == Microsoft.Extensions.Logging.LogLevel.Error ||
                entry.Level == Microsoft.Extensions.Logging.LogLevel.Critical)
            {
                item.BackColor = Color.MistyRose;
            }
            else if (entry.Level == Microsoft.Extensions.Logging.LogLevel.Warning)
            {
                item.BackColor = Color.LemonChiffon;
            }

            _logListView.Items.Add(item);
        }

        if (_logListView.Items.Count > 0)
        {
            _logListView.Items[
                _logListView.Items.Count - 1].EnsureVisible();
        }
    }

    private void ClearLogs()
    {
        _logProvider.Clear();

        RefreshLogs();
    }
}