using CertDesk.Common;
using CertDesk.Forms;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class CertificatesView : UserControl
{
    private readonly CurrentUser _user;
    private readonly DataGridView _grid = new();
    private readonly TextBox _searchBox = new();
    private readonly ComboBox _statusFilter = new();
    private readonly ComboBox _typeFilter = new();

    public CertificatesView(CurrentUser user)
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
        _typeFilter.SetBounds(305, 15, 80, 25);
        _statusFilter.Items.AddRange(new object[] { "Все", "active", "warning", "expired", "revoked", "archived" });
        _typeFilter.Items.AddRange(new object[] { "Все", "SES", "NES", "QES" });
        _statusFilter.SelectedIndex = 0;
        _typeFilter.SelectedIndex = 0;

        Button addButton = CreateButton("Добавить");
        Button editButton = CreateButton("Изменить");
        Button revokeButton = CreateButton("Отозвать");
        Button archiveButton = CreateButton("Архивировать");
        Button refreshButton = CreateButton("Обновить");
        Button exportButton = CreateButton("Экспорт");
        FlowButtons(395, addButton, editButton, revokeButton, archiveButton, refreshButton, exportButton);

        _grid.SetBounds(15, 60, 1000, 560);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        UiTheme.ApplyGridStyle(_grid);

        Controls.AddRange(new Control[] { _searchBox, _statusFilter, _typeFilter, addButton, editButton, revokeButton, archiveButton, refreshButton, exportButton, _grid });

        _searchBox.TextChanged += (_, _) => LoadData();
        _statusFilter.SelectedIndexChanged += (_, _) => LoadData();
        _typeFilter.SelectedIndexChanged += (_, _) => LoadData();
        refreshButton.Click += (_, _) => LoadData();
        addButton.Click += (_, _) => Save(new CertificateEditForm(), false);
        editButton.Click += (_, _) =>
        {
            Certificate? certificate = CertificateService.GetById(SelectedId);
            if (certificate != null)
            {
                Save(new CertificateEditForm(certificate), true);
            }
        };
        revokeButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Отозвать сертификат?"))
            {
                CertificateService.Revoke(SelectedId, _user);
                LoadData();
            }
        };
        archiveButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Архивировать сертификат?"))
            {
                CertificateService.Archive(SelectedId, _user);
                LoadData();
            }
        };
        exportButton.Click += (_, _) => MessageHelper.Info("CSV создан:\n" + ReportService.ExportCsv("Реестр сертификатов", DateTime.Today.AddYears(-1), DateTime.Today, _user));

        if (!RoleGuard.CanEdit(_user))
        {
            addButton.Enabled = false;
            editButton.Enabled = false;
            revokeButton.Enabled = false;
            archiveButton.Enabled = false;
            exportButton.Enabled = false;
        }
    }

    private static Button CreateButton(string text)
    {
        Button button = new() { Text = text, Top = 13, Width = 100 };
        UiTheme.ApplyButtonStyle(button);
        return button;
    }

    private static void FlowButtons(int x, params Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            button.Left = x;
            x += 108;
        }
    }

    private void Save(CertificateEditForm form, bool edit)
    {
        try
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (edit)
                {
                    CertificateService.Update(form.Certificate, _user);
                }
                else
                {
                    CertificateService.Create(form.Certificate, _user);
                }

                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.Error("Не удалось сохранить. Проверьте уникальность серийного номера. " + ex.Message);
        }
    }

    private void LoadData() => _grid.DataSource = CertificateService.Search(_searchBox.Text, _statusFilter.Text, _typeFilter.Text);
}
