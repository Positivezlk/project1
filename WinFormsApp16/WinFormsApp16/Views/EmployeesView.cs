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

        Button addButton = CreateButton("Добавить");
        Button editButton = CreateButton("Изменить");
        Button archiveButton = CreateButton("Архивировать");
        Button refreshButton = CreateButton("Обновить");

        _searchBox.SetBounds(15, 15, 180, 25);
        _statusFilter.SetBounds(205, 15, 120, 25);
        _statusFilter.Items.AddRange(new object[] { "Все", "Активные", "Архивные" });
        _statusFilter.SelectedIndex = 0;

        FlowButtons(addButton, editButton, archiveButton, refreshButton);

        _grid.SetBounds(15, 60, 900, 560);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        UiTheme.ApplyGridStyle(_grid);

        Controls.AddRange(new Control[] { _searchBox, _statusFilter, addButton, editButton, archiveButton, refreshButton, _grid });

        _searchBox.TextChanged += (_, _) => LoadData();
        _statusFilter.SelectedIndexChanged += (_, _) => LoadData();
        refreshButton.Click += (_, _) => LoadData();
        addButton.Click += (_, _) => Save(new EmployeeEditForm(), false);
        editButton.Click += (_, _) =>
        {
            Employee? employee = EmployeeService.GetById(SelectedId);
            if (employee != null)
            {
                Save(new EmployeeEditForm(employee), true);
            }
        };
        archiveButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Архивировать сотрудника?"))
            {
                EmployeeService.Archive(SelectedId, _user);
                LoadData();
            }
        };

        if (!RoleGuard.CanEdit(_user))
        {
            addButton.Enabled = false;
            editButton.Enabled = false;
            archiveButton.Enabled = false;
        }
    }

    private static Button CreateButton(string text)
    {
        Button button = new() { Text = text, Top = 13, Width = 110 };
        UiTheme.ApplyButtonStyle(button);
        return button;
    }

    private static void FlowButtons(params Button[] buttons)
    {
        int x = 335;
        foreach (Button button in buttons)
        {
            button.Left = x;
            x += 118;
        }
    }

    private void Save(EmployeeEditForm form, bool edit)
    {
        try
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (edit)
                {
                    EmployeeService.Update(form.Employee, _user);
                }
                else
                {
                    EmployeeService.Create(form.Employee, _user);
                }

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
