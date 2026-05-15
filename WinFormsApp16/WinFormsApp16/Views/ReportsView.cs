using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
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

        ConfigureButton(_csvButton, "Экспорт CSV", 20, 115, 120);
        ConfigureButton(_xlsxButton, "Экспорт XLSX", 150, 115, 120);
        ConfigureButton(_openFolderButton, "Открыть папку отчетов", 285, 115, 120);
        Controls.AddRange(new Control[] { _csvButton, _xlsxButton, _openFolderButton });

        _csvButton.Click += (_, _) => Export(false);
        _xlsxButton.Click += (_, _) => Export(true);
        _openFolderButton.Click += (_, _) => Process.Start(new ProcessStartInfo { FileName = ReportService.OutputDirectory, UseShellExecute = true });
    }

    private static void ConfigureButton(Button button, string text, int x, int y, int width)
    {
        button.Text = text;
        button.Left = x;
        button.Top = y;
        button.Width = width;
        UiTheme.ApplyButtonStyle(button);
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
