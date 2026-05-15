using CertDesk.Common;
using CertDesk.Forms;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class EmployeesView : UserControl
{
    private readonly CurrentUser _user;
    private readonly DataGridView _grid = new();
    private readonly TextBox _searchBox = new();
    private readonly ComboBox _statusFilter = new();
    private readonly Button _addButton = new();
    private readonly Button _editButton = new();
    private readonly Button _archiveButton = new();
    private readonly Button _refreshButton = new();

    public EmployeesView(CurrentUser user)
    {
        _user = user;
        InitializeComponent();
        Build();
        LoadData();
    }

    private int SelectedId => _grid.CurrentRow == null ? 0 : Convert.ToInt32(_grid.CurrentRow.Cells["ID"].Value);

    private void Build()
    {
        BackColor = UiTheme.Light;

        TableLayoutPanel layout = CreateLayout("Сотрудники");
        Panel filterPanel = CreateFilterPanel();
        Panel gridCard = CreateGridCard();
        layout.Controls.Add(filterPanel, 0, 1);
        layout.Controls.Add(gridCard, 0, 2);
        Controls.Add(layout);

        AddField(filterPanel, "Поиск", _searchBox, 0, 18, 230);
        AddField(filterPanel, "Статус", _statusFilter, 250, 18, 140);
        _statusFilter.Items.AddRange(new object[] { "Все", "Активные", "Архивные" });
        _statusFilter.SelectedIndex = 0;

        ConfigureButton(_addButton, "Добавить", UiTheme.ApplyButtonStyle);
        ConfigureButton(_editButton, "Изменить", UiTheme.ApplySecondaryButtonStyle);
        ConfigureButton(_archiveButton, "Архивировать", UiTheme.ApplyDangerButtonStyle);
        ConfigureButton(_refreshButton, "Обновить", UiTheme.ApplySecondaryButtonStyle);
        AddButtons(filterPanel, 420, _addButton, _editButton, _archiveButton, _refreshButton);

        _grid.Dock = DockStyle.Fill;
        UiTheme.ApplyGridStyle(_grid);
        gridCard.Controls.Add(_grid);

        _searchBox.TextChanged += (_, _) => LoadData();
        _statusFilter.SelectedIndexChanged += (_, _) => LoadData();
        _refreshButton.Click += (_, _) => LoadData();
        _addButton.Click += (_, _) => Save(new EmployeeEditForm(), false);
        _editButton.Click += (_, _) =>
        {
            Employee? employee = EmployeeService.GetById(SelectedId);
            if (employee != null) Save(new EmployeeEditForm(employee), true);
        };
        _archiveButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Архивировать сотрудника?"))
            {
                EmployeeService.Archive(SelectedId, _user);
                LoadData();
            }
        };

        if (!RoleGuard.CanEdit(_user))
        {
            _addButton.Enabled = false;
            _editButton.Enabled = false;
            _archiveButton.Enabled = false;
        }
    }

    private static TableLayoutPanel CreateLayout(string titleText)
    {
        TableLayoutPanel layout = new() { Dock = DockStyle.Fill, BackColor = UiTheme.Light, RowCount = 3, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Label title = new() { Text = titleText, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        UiTheme.ApplyLabelTitleStyle(title);
        layout.Controls.Add(title, 0, 0);
        return layout;
    }

    private static Panel CreateFilterPanel()
    {
        Panel panel = new() { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 14) };
        UiTheme.ApplyCardStyle(panel);
        return panel;
    }

    private static Panel CreateGridCard()
    {
        Panel panel = new() { Dock = DockStyle.Fill };
        UiTheme.ApplyCardStyle(panel);
        return panel;
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
        panel.Controls.Add(label);
        panel.Controls.Add(control);
    }

    private static void ConfigureButton(Button button, string text, Action<Button> style)
    {
        button.Text = text;
        button.Width = 112;
        button.Height = 36;
        style(button);
    }

    private static void AddButtons(Panel panel, int left, params Button[] buttons)
    {
        FlowLayoutPanel flow = new() { Left = left, Top = 26, Height = 42, Width = 520, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, WrapContents = false };
        foreach (Button button in buttons) flow.Controls.Add(button);
        panel.Controls.Add(flow);
    }

    private void Save(EmployeeEditForm form, bool edit)
    {
        try
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (edit) EmployeeService.Update(form.Employee, _user);
                else EmployeeService.Create(form.Employee, _user);
                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.Error("Не удалось сохранить: " + ex.Message);
        }
    }

    private void LoadData() => _grid.DataSource = EmployeeService.Search(_searchBox.Text, _statusFilter.Text);
}
