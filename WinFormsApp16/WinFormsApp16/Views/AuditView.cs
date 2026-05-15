using CertDesk.Common;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class AuditView : UserControl
{
    private readonly DataGridView _grid = new();
    private readonly DateTimePicker _from = new();
    private readonly DateTimePicker _to = new();
    private readonly ComboBox _action = new() { Width = 190 };
    private readonly TextBox _login = new() { Width = 150 };
    private readonly Button _refreshButton = new();

    public AuditView(CurrentUser user)
    {
        InitializeComponent();
        Build();
        LoadData();
    }

    private void Build()
    {
        BackColor = UiTheme.Light;

        TableLayoutPanel layout = new() { Dock = DockStyle.Fill, BackColor = UiTheme.Light, RowCount = 3, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(layout);

        Label title = new() { Text = "Журнал действий пользователей", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        UiTheme.ApplyLabelTitleStyle(title);
        layout.Controls.Add(title, 0, 0);

        Panel filterPanel = new() { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 14) };
        UiTheme.ApplyCardStyle(filterPanel);
        layout.Controls.Add(filterPanel, 0, 1);

        AddField(filterPanel, "Дата с", _from, 0, 18, 180);
        AddField(filterPanel, "Дата по", _to, 200, 18, 180);
        AddField(filterPanel, "Действие", _action, 400, 18, 190);
        AddField(filterPanel, "Пользователь", _login, 610, 18, 150);
        ConfigureButton(_refreshButton, "Обновить", 790, 36, 120);
        filterPanel.Controls.Add(_refreshButton);

        _from.Value = DateTime.Today.AddMonths(-1);
        _action.Items.Add("");
        foreach (string action in AuditService.Actions()) _action.Items.Add(action);
        _action.SelectedIndex = 0;

        Panel gridCard = new() { Dock = DockStyle.Fill };
        UiTheme.ApplyCardStyle(gridCard);
        _grid.Dock = DockStyle.Fill;
        UiTheme.ApplyGridStyle(_grid);
        gridCard.Controls.Add(_grid);
        layout.Controls.Add(gridCard, 0, 2);

        _refreshButton.Click += (_, _) => LoadData();
    }

    private static void AddField(Panel panel, string labelText, Control control, int left, int top, int width)
    {
        Label label = new() { Text = labelText, Left = left, Top = 8 };
        UiTheme.ApplyLabelMutedStyle(label);
        control.Left = left;
        control.Top = top + 18;
        control.Width = width;
        if (control is TextBox textBox) UiTheme.ApplyTextBoxStyle(textBox);
        if (control is ComboBox comboBox) UiTheme.ApplyComboBoxStyle(comboBox);
        if (control is DateTimePicker picker) UiTheme.ApplyDatePickerStyle(picker);
        panel.Controls.Add(label);
        panel.Controls.Add(control);
    }

    private static void ConfigureButton(Button button, string text, int left, int top, int width)
    {
        button.Text = text;
        button.Left = left;
        button.Top = top;
        button.Width = width;
        UiTheme.ApplySecondaryButtonStyle(button);
    }

    private void LoadData() => _grid.DataSource = AuditService.GetList(_from.Value, _to.Value, _action.Text, _login.Text);
}
