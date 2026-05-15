using CertDesk.Common;
using CertDesk.Forms;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class TokensView : UserControl
{
    private readonly CurrentUser u;
    private readonly DataGridView grid = new();
    private readonly TextBox q = new();
    private readonly ComboBox st = new();

    public TokensView(CurrentUser user)
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
        st.SetBounds(185, 15, 120, 25);
        st.Items.AddRange(new object[] { "Все", "storage", "issued", "damaged", "written_off" });
        st.SelectedIndex = 0;

        Button add = B("Добавить");
        Button edit = B("Изменить");
        Button iss = B("Выдать");
        Button ret = B("Вернуть");
        Button dam = B("Поврежден");
        Button wo = B("Списать");
        Button refb = B("Обновить");

        Flow(315, add, edit, iss, ret, dam, wo, refb);

        grid.SetBounds(15, 60, 1000, 560);
        grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        UiTheme.ApplyGridStyle(grid);

        Controls.AddRange(new Control[] { q, st, add, edit, iss, ret, dam, wo, refb, grid });

        q.TextChanged += (_, __) => LoadData();
        st.SelectedIndexChanged += (_, __) => LoadData();
        refb.Click += (_, __) => LoadData();
        add.Click += (_, __) => Save(new TokenEditForm(), false);
        edit.Click += (_, __) =>
        {
            TokenDevice? t = TokenService.GetById(Id);
            if (t != null)
                Save(new TokenEditForm(t), true);
        };
        iss.Click += (_, __) =>
        {
            using TokenIssueForm f = new();
            if (Id > 0 && f.ShowDialog() == DialogResult.OK)
            {
                TokenService.Issue(Id, f.EmployeeId, f.ActNumber, f.Comment, u);
                LoadData();
            }
        };
        ret.Click += (_, __) =>
        {
            if (Id > 0 && MessageHelper.Confirm("Вернуть токен?"))
            {
                TokenService.Return(Id, u);
                LoadData();
            }
        };
        dam.Click += (_, __) =>
        {
            if (Id > 0 && MessageHelper.Confirm("Отметить токен поврежденным?"))
            {
                TokenService.MarkDamaged(Id, u);
                LoadData();
            }
        };
        wo.Click += (_, __) =>
        {
            if (Id > 0 && MessageHelper.Confirm("Списать токен?"))
            {
                TokenService.WriteOff(Id, u);
                LoadData();
            }
        };

        if (!RoleGuard.CanManageTokens(u))
        {
            add.Enabled = false;
            edit.Enabled = false;
            iss.Enabled = false;
            ret.Enabled = false;
            dam.Enabled = false;
            wo.Enabled = false;
        }
    }

    private static Button B(string t)
    {
        Button b = new() { Text = t, Top = 13, Width = 95 };
        UiTheme.ApplyButtonStyle(b);
        return b;
    }

    private static void Flow(int x, params Button[] bs)
    {
        foreach (Button b in bs)
        {
            b.Left = x;
            x += 101;
        }
    }

    private void Save(TokenEditForm f, bool edit)
    {
        try
        {
            if (f.ShowDialog() == DialogResult.OK)
            {
                if (edit)
                    TokenService.Update(f.Token, u);
                else
                    TokenService.Create(f.Token, u);
                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.Error("Не удалось сохранить. Проверьте уникальность номера. " + ex.Message);
        }
    }

    private void LoadData()
    {
        grid.DataSource = TokenService.Search(q.Text, st.Text);
    }
}
