using System;
using System.Drawing;
using System.Windows.Forms;
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

        _searchBox.SetBounds(15, 15, 160, 25);
        _statusFilter.SetBounds(185, 15, 110, 25);
        _registrationFilter.SetBounds(305, 15, 150, 25);
        _statusFilter.Items.AddRange(new object[] { "Все", "active", "warning", "expired", "revoked", "archived" });
        _registrationFilter.Items.AddRange(new object[] { "Все", "Зарегистрирована", "Не зарегистрирована" });
        _statusFilter.SelectedIndex = 0;
        _registrationFilter.SelectedIndex = 0;

        ConfigureButton(_addButton, "Добавить", 100);
        ConfigureButton(_editButton, "Изменить", 100);
        ConfigureButton(_revokeButton, "Отозвать", 100);
        ConfigureButton(_archiveButton, "Архивировать", 100);
        ConfigureButton(_refreshButton, "Обновить", 100);
        ConfigureButton(_exportButton, "Экспорт", 100);
        FlowButtons(465, _addButton, _editButton, _revokeButton, _archiveButton, _refreshButton, _exportButton);

        _grid.SetBounds(15, 60, 1000, 560);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        UiTheme.ApplyGridStyle(_grid);

        Controls.AddRange(new Control[] { _searchBox, _statusFilter, _registrationFilter, _addButton, _editButton, _revokeButton, _archiveButton, _refreshButton, _exportButton, _grid });

        _searchBox.TextChanged += (_, _) => LoadData();
        _statusFilter.SelectedIndexChanged += (_, _) => LoadData();
        _registrationFilter.SelectedIndexChanged += (_, _) => LoadData();
        _refreshButton.Click += (_, _) => LoadData();
        _addButton.Click += (_, _) => Save(new MchdEditForm(), false);
        _editButton.Click += (_, _) =>
        {
            Mchd? mchd = MchdService.GetById(SelectedId);
            if (mchd != null)
            {
                Save(new MchdEditForm(mchd), true);
            }
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

    private static void ConfigureButton(Button button, string text, int width)
    {
        button.Text = text;
        button.Top = 13;
        button.Width = width;
        UiTheme.ApplyButtonStyle(button);
    }

    private static void FlowButtons(int x, params Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            button.Left = x;
            x += 108;
        }
    }

    private void Save(MchdEditForm form, bool edit)
    {
        try
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (edit)
                {
                    MchdService.Update(form.Mchd, _user);
                }
                else
                {
                    MchdService.Create(form.Mchd, _user);
                }

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
