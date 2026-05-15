using CertDesk.Common;
using CertDesk.Models;

namespace CertDesk.Forms;

public partial class EmployeeEditForm : Form
{
    public Employee Employee { get; }

    private readonly TextBox n = new() { Width = 300 };
    private readonly TextBox p = new() { Width = 300 };
    private readonly TextBox d = new() { Width = 300 };
    private readonly TextBox e = new() { Width = 300 };
    private readonly TextBox ph = new() { Width = 300 };
    private readonly TextBox s = new() { Width = 300 };
    private readonly TextBox inn = new() { Width = 300 };
    private readonly CheckBox a = new() { Text = "Активен", Checked = true };

    public EmployeeEditForm(Employee? emp = null)
    {
        Employee = emp ?? new Employee();
        InitializeComponent();
        Build();
        if (emp != null) LoadE();
    }

    private void Build()
    {
        UiTheme.ApplyFormStyle(this);
        Text = "Сотрудник";
        ClientSize = new Size(560, 500);
        BackColor = UiTheme.Light;

        Label title = new() { Text = "Карточка сотрудника", Left = 24, Top = 18 };
        UiTheme.ApplyLabelTitleStyle(title);
        Controls.Add(title);

        Panel card = new() { Left = 24, Top = 64, Width = 510, Height = 350 };
        UiTheme.ApplyCardStyle(card);
        Controls.Add(card);

        int y = 8;
        AddRow(card, "ФИО", n, ref y);
        AddRow(card, "Должность", p, ref y);
        AddRow(card, "Подразделение", d, ref y);
        AddRow(card, "Email", e, ref y);
        AddRow(card, "Телефон", ph, ref y);
        AddRow(card, "СНИЛС", s, ref y);
        AddRow(card, "ИНН", inn, ref y);
        a.Left = 150;
        a.Top = y + 4;
        a.ForeColor = UiTheme.Text;
        card.Controls.Add(a);

        Button ok = new() { Text = "Сохранить", Left = 304, Top = 432, Width = 110 };
        Button cancel = new() { Text = "Отмена", Left = 424, Top = 432, Width = 110, DialogResult = DialogResult.Cancel };
        UiTheme.ApplyButtonStyle(ok);
        UiTheme.ApplySecondaryButtonStyle(cancel);
        Controls.AddRange(new Control[] { ok, cancel });
        ok.Click += Save;
    }

    private static void AddRow(Control parent, string text, TextBox box, ref int y)
    {
        Label label = new() { Text = text, Left = 0, Top = y + 5, Width = 130 };
        UiTheme.ApplyLabelMutedStyle(label);
        box.Left = 150;
        box.Top = y;
        UiTheme.ApplyTextBoxStyle(box);
        parent.Controls.Add(label);
        parent.Controls.Add(box);
        y += 40;
    }

    private void LoadE()
    {
        n.Text = Employee.FullName;
        p.Text = Employee.Position;
        d.Text = Employee.Department;
        e.Text = Employee.Email;
        ph.Text = Employee.Phone;
        s.Text = Employee.Snils;
        inn.Text = Employee.Inn;
        a.Checked = Employee.IsActive;
    }

    private void Save(object? sender, EventArgs args)
    {
        if (string.IsNullOrWhiteSpace(n.Text) || string.IsNullOrWhiteSpace(p.Text))
        {
            MessageHelper.Error("ФИО и должность обязательны");
            return;
        }
        if (e.TextLength > 0 && !e.Text.Contains('@'))
        {
            MessageHelper.Error("Некорректный email");
            return;
        }

        Employee.FullName = n.Text.Trim();
        Employee.Position = p.Text.Trim();
        Employee.Department = d.Text.Trim();
        Employee.Email = e.Text.Trim();
        Employee.Phone = ph.Text.Trim();
        Employee.Snils = s.Text.Trim();
        Employee.Inn = inn.Text.Trim();
        Employee.IsActive = a.Checked;
        DialogResult = DialogResult.OK;
    }
}
