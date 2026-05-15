using System.Diagnostics;
using CertDesk.Common;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class ReportsView : UserControl
{
    private readonly CurrentUser _user;
    private readonly ComboBox _reportType = new() { Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _periodFrom = new();
    private readonly DateTimePicker _periodTo = new();
    private readonly Label _lastSavedFileLabel = new() { AutoSize = true, Top = 170, Left = 20, Width = 800 };

    public ReportsView(CurrentUser user)
    {
        _user = user;
        InitializeComponent();
        Build();
    }

    private void Build()
    {
        BackColor = UiTheme.Light;

        Controls.AddRange(new Control[]
        {
            new Label { Text = "Тип отчета", Left = 20, Top = 25 },
            _reportType,
            new Label { Text = "Период с", Left = 20, Top = 70 },
            _periodFrom,
            new Label { Text = "по", Left = 310, Top = 70 },
            _periodTo,
            _lastSavedFileLabel
        });

        _reportType.Left = 130;
        _reportType.Top = 20;
        _periodFrom.Left = 130;
        _periodFrom.Top = 65;
        _periodTo.Left = 340;
        _periodTo.Top = 65;
        _periodFrom.Value = DateTime.Today.AddMonths(-1);

        foreach (string report in ReportService.ReportTypes)
        {
            _reportType.Items.Add(report);
        }
        _reportType.SelectedIndex = 0;

        Button csvButton = CreateButton("Экспорт CSV", 20, 115);
        Button xlsxButton = CreateButton("Экспорт XLSX", 150, 115);
        Button openFolderButton = CreateButton("Открыть папку отчетов", 285, 115);
        Controls.AddRange(new Control[] { csvButton, xlsxButton, openFolderButton });

        csvButton.Click += (_, _) => Export(false);
        xlsxButton.Click += (_, _) => Export(true);
        openFolderButton.Click += (_, _) => Process.Start(new ProcessStartInfo { FileName = ReportService.OutputDirectory, UseShellExecute = true });
    }

    private static Button CreateButton(string text, int x, int y)
    {
        Button button = new() { Text = text, Left = x, Top = y, Width = 120 };
        UiTheme.ApplyButtonStyle(button);
        return button;
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
