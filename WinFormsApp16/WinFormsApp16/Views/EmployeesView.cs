using System;
using System.Drawing;
using System.Windows.Forms;
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

        ConfigureButton(_addButton, "Добавить", 110);
        ConfigureButton(_editButton, "Изменить", 110);
        ConfigureButton(_archiveButton, "Архивировать", 110);
        ConfigureButton(_refreshButton, "Обновить", 110);

        _searchBox.SetBounds(15, 15, 180, 25);
        _statusFilter.SetBounds(205, 15, 120, 25);
        _statusFilter.Items.AddRange(new object[] { "Все", "Активные", "Архивные" });
        _statusFilter.SelectedIndex = 0;

        FlowButtons(_addButton, _editButton, _archiveButton, _refreshButton);

        _grid.SetBounds(15, 60, 900, 560);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        UiTheme.ApplyGridStyle(_grid);

        Controls.AddRange(new Control[] { _searchBox, _statusFilter, _addButton, _editButton, _archiveButton, _refreshButton, _grid });

        _searchBox.TextChanged += (_, _) => LoadData();
        _statusFilter.SelectedIndexChanged += (_, _) => LoadData();
        _refreshButton.Click += (_, _) => LoadData();
        _addButton.Click += (_, _) => Save(new EmployeeEditForm(), false);
        _editButton.Click += (_, _) =>
        {
            Employee? employee = EmployeeService.GetById(SelectedId);
            if (employee != null)
            {
                Save(new EmployeeEditForm(employee), true);
            }
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

    private static void ConfigureButton(Button button, string text, int width)
    {
        button.Text = text;
        button.Top = 13;
        button.Width = width;
        UiTheme.ApplyButtonStyle(button);
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
