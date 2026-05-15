using CertDesk.Common;
using CertDesk.Forms;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class MchdView : UserControl
{
    private readonly CurrentUser _user;
    private readonly DataGridView _grid = new();
    private readonly TextBox _searchBox = new();
    private readonly ComboBox _statusFilter = new();
    private readonly ComboBox _registrationFilter = new();
    private readonly Button _addButton = new();
    private readonly Button _editButton = new();
    private readonly Button _revokeButton = new();
    private readonly Button _archiveButton = new();
    private readonly Button _refreshButton = new();
    private readonly Button _exportButton = new();

    public MchdView(CurrentUser user)
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

        TableLayoutPanel layout = CreateLayout("Реестр машиночитаемых доверенностей");
        Panel filterPanel = CreateFilterPanel();
        Panel gridCard = CreateGridCard();
        layout.Controls.Add(filterPanel, 0, 1);
        layout.Controls.Add(gridCard, 0, 2);
        Controls.Add(layout);

        AddField(filterPanel, "Поиск", _searchBox, 0, 18, 190);
        AddField(filterPanel, "Статус", _statusFilter, 210, 18, 130);
        AddField(filterPanel, "Регистрация", _registrationFilter, 360, 18, 150);
        _statusFilter.Items.AddRange(new object[] { "Все", "active", "warning", "expired", "revoked", "archived" });
        _registrationFilter.Items.AddRange(new object[] { "Все", "Зарегистрирована", "Не зарегистрирована" });
        _statusFilter.SelectedIndex = 0;
        _registrationFilter.SelectedIndex = 0;

        ConfigureButton(_addButton, "Добавить", UiTheme.ApplyButtonStyle);
        ConfigureButton(_editButton, "Изменить", UiTheme.ApplySecondaryButtonStyle);
        ConfigureButton(_revokeButton, "Отозвать", UiTheme.ApplyDangerButtonStyle);
        ConfigureButton(_archiveButton, "Архивировать", UiTheme.ApplyDangerButtonStyle);
        ConfigureButton(_refreshButton, "Обновить", UiTheme.ApplySecondaryButtonStyle);
        ConfigureButton(_exportButton, "Экспорт", UiTheme.ApplySecondaryButtonStyle);
        AddButtons(filterPanel, 535, _addButton, _editButton, _revokeButton, _archiveButton, _refreshButton, _exportButton);

        _grid.Dock = DockStyle.Fill;
        UiTheme.ApplyGridStyle(_grid);
        gridCard.Controls.Add(_grid);

        _searchBox.TextChanged += (_, _) => LoadData();
        _statusFilter.SelectedIndexChanged += (_, _) => LoadData();
        _registrationFilter.SelectedIndexChanged += (_, _) => LoadData();
        _refreshButton.Click += (_, _) => LoadData();
        _addButton.Click += (_, _) => Save(new MchdEditForm(), false);
        _editButton.Click += (_, _) =>
        {
            Mchd? mchd = MchdService.GetById(SelectedId);
            if (mchd != null) Save(new MchdEditForm(mchd), true);
        };
        _revokeButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Отозвать МЧД?"))
            {
                MchdService.Revoke(SelectedId, _user);
                LoadData();
            }
        };
        _archiveButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Архивировать МЧД?"))
            {
                MchdService.Archive(SelectedId, _user);
                LoadData();
            }
        };
        _exportButton.Click += (_, _) => MessageHelper.Info("CSV создан:\n" + ReportService.ExportCsv("Реестр МЧД", DateTime.Today.AddYears(-1), DateTime.Today, _user));

        if (!RoleGuard.CanEdit(_user))
        {
            _addButton.Enabled = false;
            _editButton.Enabled = false;
            _revokeButton.Enabled = false;
            _archiveButton.Enabled = false;
            _exportButton.Enabled = false;
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
        button.Width = 104;
        button.Height = 36;
        style(button);
    }

    private static void AddButtons(Panel panel, int left, params Button[] buttons)
    {
        FlowLayoutPanel flow = new() { Left = left, Top = 26, Height = 42, Width = 640, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, WrapContents = false };
        foreach (Button button in buttons) flow.Controls.Add(button);
        panel.Controls.Add(flow);
    }

    private void Save(MchdEditForm form, bool edit)
    {
        try
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (edit) MchdService.Update(form.Mchd, _user);
                else MchdService.Create(form.Mchd, _user);
                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.Error("Не удалось сохранить. Проверьте уникальность номера МЧД. " + ex.Message);
        }
    }

    private void LoadData() => _grid.DataSource = MchdService.Search(_searchBox.Text, _statusFilter.Text, _registrationFilter.Text);
}
