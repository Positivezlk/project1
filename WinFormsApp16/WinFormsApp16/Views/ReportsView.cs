using System.Diagnostics;
using CertDesk.Common;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class ReportsView : UserControl
{
    private readonly CurrentUser _user;
    private readonly ComboBox _reportType = new() { Width = 430, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _periodFrom = new();
    private readonly DateTimePicker _periodTo = new();
    private readonly Label _lastSavedFileLabel = new() { AutoSize = false, Width = 720, Height = 45 };
    private readonly Button _csvButton = new();
    private readonly Button _xlsxButton = new();
    private readonly Button _openFolderButton = new();

    public ReportsView(CurrentUser user)
    {
        _user = user;
        InitializeComponent();
        Build();
    }

    private void Build()
    {
        BackColor = UiTheme.Light;

        TableLayoutPanel layout = new() { Dock = DockStyle.Fill, BackColor = UiTheme.Light, RowCount = 2, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Label title = new() { Text = "Отчеты", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        UiTheme.ApplyLabelTitleStyle(title);
        layout.Controls.Add(title, 0, 0);

        Panel card = new() { Dock = DockStyle.Top, Height = 245, Margin = new Padding(0, 0, 0, 14) };
        UiTheme.ApplyCardStyle(card);
        layout.Controls.Add(card, 0, 1);
        Controls.Add(layout);

        Label description = new() { Text = "Выберите тип отчета и формат выгрузки", Left = 0, Top = 0 };
        UiTheme.ApplyLabelMutedStyle(description);
        card.Controls.Add(description);

        AddLabel(card, "Тип отчета", 0, 42);
        _reportType.Left = 0;
        _reportType.Top = 62;
        UiTheme.ApplyComboBoxStyle(_reportType);
        card.Controls.Add(_reportType);

        AddLabel(card, "Период с", 0, 104);
        _periodFrom.Left = 0;
        _periodFrom.Top = 124;
        UiTheme.ApplyDatePickerStyle(_periodFrom);
        card.Controls.Add(_periodFrom);

        AddLabel(card, "по", 230, 104);
        _periodTo.Left = 230;
        _periodTo.Top = 124;
        UiTheme.ApplyDatePickerStyle(_periodTo);
        card.Controls.Add(_periodTo);
        _periodFrom.Value = DateTime.Today.AddMonths(-1);

        foreach (string report in ReportService.ReportTypes)
        {
            _reportType.Items.Add(report);
        }
        _reportType.SelectedIndex = 0;

        ConfigureButton(_csvButton, "Экспорт CSV", 0, 170, 130, UiTheme.ApplyButtonStyle);
        ConfigureButton(_xlsxButton, "Экспорт XLSX", 145, 170, 135, UiTheme.ApplyButtonStyle);
        ConfigureButton(_openFolderButton, "Открыть папку отчетов", 295, 170, 190, UiTheme.ApplySecondaryButtonStyle);
        card.Controls.AddRange(new Control[] { _csvButton, _xlsxButton, _openFolderButton });

        _lastSavedFileLabel.Left = 0;
        _lastSavedFileLabel.Top = 212;
        _lastSavedFileLabel.ForeColor = UiTheme.Secondary;
        _lastSavedFileLabel.Text = "Последний файл: не сформирован";
        card.Controls.Add(_lastSavedFileLabel);

        _csvButton.Click += (_, _) => Export(false);
        _xlsxButton.Click += (_, _) => Export(true);
        _openFolderButton.Click += (_, _) => Process.Start(new ProcessStartInfo { FileName = ReportService.OutputDirectory, UseShellExecute = true });
    }

    private static void AddLabel(Control parent, string text, int left, int top)
    {
        Label label = new() { Text = text, Left = left, Top = top };
        UiTheme.ApplyLabelMutedStyle(label);
        parent.Controls.Add(label);
    }

    private static void ConfigureButton(Button button, string text, int left, int top, int width, Action<Button> style)
    {
        button.Text = text;
        button.Left = left;
        button.Top = top;
        button.Width = width;
        style(button);
    }

    private void Export(bool xlsx)
    {
        try
        {
            string path = xlsx
                ? ReportService.ExportXlsx(_reportType.Text, _periodFrom.Value, _periodTo.Value, _user)
                : ReportService.ExportCsv(_reportType.Text, _periodFrom.Value, _periodTo.Value, _user);
            _lastSavedFileLabel.Text = "Последний файл: " + path;
            MessageHelper.Info("Отчет сохранен:\n" + path);
        }
        catch (Exception ex)
        {
            MessageHelper.Error(ex.Message);
        }
    }
}
