using CertDesk.Common;
using CertDesk.Services;

namespace CertDesk.Forms;

public partial class TokenIssueForm : Form
{
    private readonly ComboBox emp = new() { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox act = new() { Width = 300 };
    private readonly TextBox comment = new() { Width = 300 };

    public int EmployeeId => ((ComboBoxItem)emp.SelectedItem).Id!.Value;
    public string ActNumber => act.Text;
    public string Comment => comment.Text;

    public TokenIssueForm()
    {
        InitializeComponent();
        Build();
    }

    private void Build()
    {
        UiTheme.ApplyFormStyle(this);
        Text = "Выдача токена";
        ClientSize = new Size(520, 290);
        BackColor = UiTheme.Light;

        Label title = new() { Text = "Выдача токена сотруднику", Left = 24, Top = 18 };
        UiTheme.ApplyLabelTitleStyle(title);
        Controls.Add(title);

        Panel card = new() { Left = 24, Top = 64, Width = 470, Height = 140 };
        UiTheme.ApplyCardStyle(card);
        Controls.Add(card);

        int y = 8;
        AddRow(card, "Сотрудник", emp, ref y);
        AddRow(card, "Номер акта", act, ref y);
        AddRow(card, "Комментарий", comment, ref y);

        foreach (var employee in EmployeeService.GetActive()) emp.Items.Add(new ComboBoxItem { Id = employee.Id, Text = employee.FullName });
        if (emp.Items.Count > 0) emp.SelectedIndex = 0;

        Button ok = new() { Text = "Сохранить", Left = 274, Top = 224, Width = 110 };
        Button cancel = new() { Text = "Отмена", Left = 394, Top = 224, Width = 100, DialogResult = DialogResult.Cancel };
        UiTheme.ApplyButtonStyle(ok);
        UiTheme.ApplySecondaryButtonStyle(cancel);
        Controls.AddRange(new Control[] { ok, cancel });
        ok.Click += (_, _) =>
        {
            if (emp.SelectedItem == null) MessageHelper.Error("Выберите сотрудника");
            else DialogResult = DialogResult.OK;
        };
    }

    private static void AddRow(Control parent, string labelText, Control control, ref int y)
    {
        Label label = new() { Text = labelText, Left = 0, Top = y + 5, Width = 120 };
        UiTheme.ApplyLabelMutedStyle(label);
        control.Left = 140;
        control.Top = y;
        if (control is TextBox textBox) UiTheme.ApplyTextBoxStyle(textBox);
        if (control is ComboBox comboBox) UiTheme.ApplyComboBoxStyle(comboBox);
        parent.Controls.Add(label);
        parent.Controls.Add(control);
        y += 40;
    }
}
