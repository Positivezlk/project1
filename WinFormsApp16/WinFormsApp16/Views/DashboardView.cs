using System.Data;
using CertDesk.Common;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class DashboardView : UserControl
{
    private readonly DataGridView _grid = new();
    private readonly Label[] _cardValues = new Label[7];

    public DashboardView(CurrentUser user)
    {
        InitializeComponent();
        Build();
        LoadData();
    }

    private void Build()
    {
        BackColor = UiTheme.Light;

        TableLayoutPanel layout = new() { Dock = DockStyle.Fill, BackColor = UiTheme.Light, RowCount = 4, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 210));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(layout);

        Label title = new() { Text = "Главная панель", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        UiTheme.ApplyLabelTitleStyle(title);
        layout.Controls.Add(title, 0, 0);

        FlowLayoutPanel cards = new() { Dock = DockStyle.Fill, WrapContents = true, AutoScroll = true, BackColor = UiTheme.Light };
        layout.Controls.Add(cards, 0, 1);

        string[] captions =
        {
            "Действующие сертификаты",
            "Истекающие сертификаты",
            "Действующие МЧД",
            "Истекающие МЧД",
            "Всего токенов",
            "Выданные токены",
            "Активные сотрудники"
        };

        for (int i = 0; i < captions.Length; i++)
        {
            Panel card = new() { Width = 205, Height = 86, Margin = new Padding(0, 0, 14, 14) };
            UiTheme.ApplyCardStyle(card);
            Label caption = new() { Text = captions[i], Left = 14, Top = 12, Width = 170, Height = 20 };
            UiTheme.ApplyLabelMutedStyle(caption);
            Label value = new() { Text = "0", Left = 14, Top = 36, Width = 170, Height = 34, Font = new Font("Segoe UI", 20F, FontStyle.Bold), ForeColor = UiTheme.Primary };
            card.Controls.AddRange(new Control[] { caption, value });
            cards.Controls.Add(card);
            _cardValues[i] = value;
        }

        Panel captionPanel = new() { Dock = DockStyle.Fill, BackColor = UiTheme.Light };
        Label attention = new() { Text = "Требуют внимания", Left = 0, Top = 8 };
        UiTheme.ApplyLabelTitleStyle(attention);
        Button refresh = new() { Text = "Обновить", Left = 250, Top = 6, Width = 120 };
        UiTheme.ApplySecondaryButtonStyle(refresh);
        refresh.Click += (_, _) => LoadData();
        captionPanel.Controls.AddRange(new Control[] { attention, refresh });
        layout.Controls.Add(captionPanel, 0, 2);

        Panel gridCard = new() { Dock = DockStyle.Fill };
        UiTheme.ApplyCardStyle(gridCard);
        _grid.Dock = DockStyle.Fill;
        UiTheme.ApplyGridStyle(_grid);
        gridCard.Controls.Add(_grid);
        layout.Controls.Add(gridCard, 0, 3);
    }

    private void LoadData()
    {
        _cardValues[0].Text = CertificateService.CountByStatus("active").ToString();
        _cardValues[1].Text = CertificateService.CountByStatus("warning").ToString();
        _cardValues[2].Text = MchdService.CountByStatus("active").ToString();
        _cardValues[3].Text = MchdService.CountByStatus("warning").ToString();
        _cardValues[4].Text = TokenService.CountAll().ToString();
        _cardValues[5].Text = TokenService.CountIssued().ToString();
        _cardValues[6].Text = EmployeeService.GetActive().Count.ToString();

        DataTable table = CertificateService.GetAttentionItems();
        table.Merge(MchdService.GetAttentionItems());
        _grid.DataSource = table;
    }
}
