using System.Diagnostics;
using CertDesk.Common;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class ReportsView : UserControl
{
    private readonly CurrentUser u;
    private readonly ComboBox type = new() { Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker from = new();
    private readonly DateTimePicker to = new();
    private readonly Label last = new() { AutoSize = true, Top = 170, Left = 20, Width = 800 };

    public ReportsView(CurrentUser user)
    {
        u = user;
        InitializeComponent();
        Build();
    }

    private void Build()
    {
        BackColor = UiTheme.Light;

        Controls.AddRange(new Control[]
        {
            new Label { Text = "Тип отчета", Left = 20, Top = 25 },
            type,
            new Label { Text = "Период с", Left = 20, Top = 70 },
            from,
            new Label { Text = "по", Left = 310, Top = 70 },
            to,
            last
        });

        type.Left = 130;
        type.Top = 20;
        from.Left = 130;
        from.Top = 65;
        to.Left = 340;
        to.Top = 65;
        from.Value = DateTime.Today.AddMonths(-1);

        foreach (string r in ReportService.ReportTypes)
            type.Items.Add(r);
        type.SelectedIndex = 0;

        Button csv = B("Экспорт CSV", 20, 115);
        Button xlsx = B("Экспорт XLSX", 150, 115);
        Button open = B("Открыть папку отчетов", 285, 115);

        Controls.AddRange(new Control[] { csv, xlsx, open });

        csv.Click += (_, __) => Export(false);
        xlsx.Click += (_, __) => Export(true);
        open.Click += (_, __) => Process.Start(new ProcessStartInfo { FileName = ReportService.OutputDirectory, UseShellExecute = true });
    }

    private static Button B(string t, int x, int y)
    {
        Button b = new() { Text = t, Left = x, Top = y, Width = 120 };
        UiTheme.ApplyButtonStyle(b);
        return b;
    }

    private void Export(bool x)
    {
        try
        {
            string p = x
                ? ReportService.ExportXlsx(type.Text, from.Value, to.Value, u)
                : ReportService.ExportCsv(type.Text, from.Value, to.Value, u);
            last.Text = "Последний файл: " + p;
            MessageHelper.Info("Отчет сохранен:\n" + p);
        }
        catch (Exception ex)
        {
            MessageHelper.Error(ex.Message);
        }
    }
}
