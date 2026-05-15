using CertDesk.Common;
using CertDesk.Forms;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class EmployeesView : UserControl
{
    private readonly CurrentUser u;
    private readonly DataGridView grid = new();
    private readonly TextBox q = new();
    private readonly ComboBox st = new();

    public EmployeesView(CurrentUser user)
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

        Button add = B("Добавить");
        Button edit = B("Изменить");
        Button arc = B("Архивировать");
        Button refb = B("Обновить");

        q.SetBounds(15, 15, 180, 25);
        st.SetBounds(205, 15, 120, 25);
        st.Items.AddRange(new object[] { "Все", "Активные", "Архивные" });
        st.SelectedIndex = 0;

        Flow(add, edit, arc, refb);

        grid.SetBounds(15, 60, 900, 560);
        grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        UiTheme.ApplyGridStyle(grid);

        Controls.AddRange(new Control[] { q, st, add, edit, arc, refb, grid });

        q.TextChanged += (_, __) => LoadData();
        st.SelectedIndexChanged += (_, __) => LoadData();
        refb.Click += (_, __) => LoadData();
        add.Click += (_, __) => Save(new EmployeeEditForm(), false);
        edit.Click += (_, __) =>
        {
            Employee? e = EmployeeService.GetById(Id);
            if (e != null)
                Save(new EmployeeEditForm(e), true);
        };
        arc.Click += (_, __) =>
        {
            if (Id > 0 && MessageHelper.Confirm("Архивировать сотрудника?"))
            {
                EmployeeService.Archive(Id, u);
                LoadData();
            }
        };

        if (!RoleGuard.CanEdit(u))
        {
            add.Enabled = false;
            edit.Enabled = false;
            arc.Enabled = false;
        }
    }

    private static Button B(string t)
    {
        Button b = new() { Text = t, Top = 13, Width = 110 };
        UiTheme.ApplyButtonStyle(b);
        return b;
    }

    private static void Flow(params Button[] bs)
    {
        int x = 335;
        foreach (Button b in bs)
        {
            b.Left = x;
            x += 118;
        }
    }

    private void Save(EmployeeEditForm f, bool edit)
    {
        try
        {
            if (f.ShowDialog() == DialogResult.OK)
            {
                if (edit)
                    EmployeeService.Update(f.Employee, u);
                else
                    EmployeeService.Create(f.Employee, u);
                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.Error("Не удалось сохранить: " + ex.Message);
        }
    }

    private void LoadData()
    {
        grid.DataSource = EmployeeService.Search(q.Text, st.Text);
    }
}
