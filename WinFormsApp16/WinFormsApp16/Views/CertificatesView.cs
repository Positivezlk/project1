using CertDesk.Common;
using CertDesk.Forms;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class CertificatesView : UserControl
{
    private readonly CurrentUser u;
    private readonly DataGridView grid = new();
    private readonly TextBox q = new();
    private readonly ComboBox st = new();
    private readonly ComboBox ty = new();

    public CertificatesView(CurrentUser user)
    {
        u = user;
        InitializeComponent();
        Build();
        LoadData();
    }

    private int Id => grid.CurrentRow == null ? 0 : Convert.ToInt32(grid.CurrentRow.Cells["ID"].Value);

    private void Build()
    {
        BackColor = UiTheme.Light;

        q.SetBounds(15, 15, 160, 25);
        st.SetBounds(185, 15, 110, 25);
        ty.SetBounds(305, 15, 80, 25);

        st.Items.AddRange(new object[] { "Все", "active", "warning", "expired", "revoked", "archived" });
        ty.Items.AddRange(new object[] { "Все", "SES", "NES", "QES" });
        st.SelectedIndex = 0;
        ty.SelectedIndex = 0;

        Button add = B("Добавить");
        Button edit = B("Изменить");
        Button rev = B("Отозвать");
        Button arc = B("Архивировать");
        Button refb = B("Обновить");
        Button exp = B("Экспорт");

        Flow(395, add, edit, rev, arc, refb, exp);

        grid.SetBounds(15, 60, 1000, 560);
        grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        UiTheme.ApplyGridStyle(grid);

        Controls.AddRange(new Control[] { q, st, ty, add, edit, rev, arc, refb, exp, grid });

        q.TextChanged += (_, __) => LoadData();
        st.SelectedIndexChanged += (_, __) => LoadData();
        ty.SelectedIndexChanged += (_, __) => LoadData();
        refb.Click += (_, __) => LoadData();
        add.Click += (_, __) => Save(new CertificateEditForm(), false);
        edit.Click += (_, __) =>
        {
            Certificate? c = CertificateService.GetById(Id);
            if (c != null)
                Save(new CertificateEditForm(c), true);
        };
        rev.Click += (_, __) =>
        {
            if (Id > 0 && MessageHelper.Confirm("Отозвать сертификат?"))
            {
                CertificateService.Revoke(Id, u);
                LoadData();
            }
        };
        arc.Click += (_, __) =>
        {
            if (Id > 0 && MessageHelper.Confirm("Архивировать сертификат?"))
            {
                CertificateService.Archive(Id, u);
                LoadData();
            }
        };
        exp.Click += (_, __) => MessageHelper.Info("CSV создан:\n" + ReportService.ExportCsv("Реестр сертификатов", DateTime.Today.AddYears(-1), DateTime.Today, u));

        if (!RoleGuard.CanEdit(u))
        {
            add.Enabled = false;
            edit.Enabled = false;
            rev.Enabled = false;
            arc.Enabled = false;
            exp.Enabled = false;
        }
    }

    private static Button B(string t)
    {
        Button b = new() { Text = t, Top = 13, Width = 100 };
        UiTheme.ApplyButtonStyle(b);
        return b;
    }

    private static void Flow(int x, params Button[] bs)
    {
        foreach (Button b in bs)
        {
            b.Left = x;
            x += 108;
        }
    }

    private void Save(CertificateEditForm f, bool edit)
    {
        try
        {
            if (f.ShowDialog() == DialogResult.OK)
            {
                if (edit)
                    CertificateService.Update(f.Certificate, u);
                else
                    CertificateService.Create(f.Certificate, u);
                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.Error("Не удалось сохранить. Проверьте уникальность серийного номера. " + ex.Message);
        }
    }

    private void LoadData()
    {
        grid.DataSource = CertificateService.Search(q.Text, st.Text, ty.Text);
    }
}
