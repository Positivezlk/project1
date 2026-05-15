using CertDesk.Common;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Forms;

public partial class CertificateEditForm : Form
{
    public Certificate Certificate { get; }

    private readonly ComboBox emp = new() { Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox auth = new() { Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox tok = new() { Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox type = new() { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox sn = new() { Width = 320 };
    private readonly TextBox purpose = new() { Width = 320 };
    private readonly DateTimePicker issued = new();
    private readonly DateTimePicker vf = new();
    private readonly DateTimePicker vt = new();

    public CertificateEditForm(Certificate? c = null)
    {
        Certificate = c ?? new Certificate();
        InitializeComponent();
        Build();
        LoadLists();
        if (c != null) LoadC();
    }

    private void Build()
    {
        UiTheme.ApplyFormStyle(this);
        Text = "Сертификат";
        ClientSize = new Size(620, 560);
        BackColor = UiTheme.Light;

        Label title = new() { Text = "Сертификат электронной подписи", Left = 24, Top = 18 };
        UiTheme.ApplyLabelTitleStyle(title);
        Controls.Add(title);

        Panel card = new() { Left = 24, Top = 64, Width = 570, Height = 410 };
        UiTheme.ApplyCardStyle(card);
        Controls.Add(card);

        int y = 8;
        AddRow(card, "Владелец", emp, ref y);
        AddRow(card, "Удостоверяющий центр", auth, ref y);
        AddRow(card, "Токен", tok, ref y);
        AddRow(card, "Серийный номер", sn, ref y);
        AddRow(card, "Тип подписи", type, ref y);
        AddRow(card, "Дата выдачи", issued, ref y);
        AddRow(card, "Действует с", vf, ref y);
        AddRow(card, "Действует до", vt, ref y);
        AddRow(card, "Назначение", purpose, ref y);

        Button ok = new() { Text = "Сохранить", Left = 364, Top = 494, Width = 110 };
        Button cancel = new() { Text = "Отмена", Left = 484, Top = 494, Width = 110, DialogResult = DialogResult.Cancel };
        UiTheme.ApplyButtonStyle(ok);
        UiTheme.ApplySecondaryButtonStyle(cancel);
        Controls.AddRange(new Control[] { ok, cancel });
        ok.Click += Save;
    }

    private static void AddRow(Control parent, string labelText, Control control, ref int y)
    {
        Label label = new() { Text = labelText, Left = 0, Top = y + 5, Width = 170 };
        UiTheme.ApplyLabelMutedStyle(label);
        control.Left = 190;
        control.Top = y;
        if (control is TextBox textBox) UiTheme.ApplyTextBoxStyle(textBox);
        if (control is ComboBox comboBox) UiTheme.ApplyComboBoxStyle(comboBox);
        if (control is DateTimePicker picker) UiTheme.ApplyDatePickerStyle(picker);
        parent.Controls.Add(label);
        parent.Controls.Add(control);
        y += 40;
    }

    private void LoadLists()
    {
        foreach (Employee employee in EmployeeService.GetActive()) emp.Items.Add(new ComboBoxItem { Id = employee.Id, Text = employee.FullName });
        foreach (Authority authority in AuthorityService.GetActive()) auth.Items.Add(new ComboBoxItem { Id = authority.Id, Text = authority.Name });
        tok.Items.Add(new ComboBoxItem { Id = null, Text = "Без токена" });
        foreach (TokenDevice token in TokenService.GetAll()) tok.Items.Add(new ComboBoxItem { Id = token.Id, Text = token.InventoryNumber });
        type.Items.AddRange(new object[] { "SES", "NES", "QES" });
        if (emp.Items.Count > 0) emp.SelectedIndex = 0;
        if (auth.Items.Count > 0) auth.SelectedIndex = 0;
        tok.SelectedIndex = 0;
        type.SelectedIndex = 2;
    }

    private void LoadC()
    {
        Select(emp, Certificate.EmployeeId);
        Select(auth, Certificate.AuthorityId);
        Select(tok, Certificate.TokenId);
        sn.Text = Certificate.SerialNumber;
        type.SelectedItem = Certificate.SignatureType;
        issued.Value = Certificate.IssuedAt;
        vf.Value = Certificate.ValidFrom;
        vt.Value = Certificate.ValidTo;
        purpose.Text = Certificate.Purpose;
    }

    private static void Select(ComboBox comboBox, int? id)
    {
        foreach (ComboBoxItem item in comboBox.Items)
        {
            if (item.Id == id)
            {
                comboBox.SelectedItem = item;
                break;
            }
        }
    }

    private void Save(object? sender, EventArgs args)
    {
        if (emp.SelectedItem == null || auth.SelectedItem == null || string.IsNullOrWhiteSpace(sn.Text))
        {
            MessageHelper.Error("Заполните владельца, УЦ и серийный номер");
            return;
        }
        if (vt.Value.Date < vf.Value.Date || issued.Value.Date > vt.Value.Date)
        {
            MessageHelper.Error("Проверьте даты сертификата");
            return;
        }

        Certificate.EmployeeId = ((ComboBoxItem)emp.SelectedItem).Id!.Value;
        Certificate.AuthorityId = ((ComboBoxItem)auth.SelectedItem).Id!.Value;
        Certificate.TokenId = ((ComboBoxItem)tok.SelectedItem).Id;
        Certificate.SerialNumber = sn.Text.Trim();
        Certificate.SignatureType = type.Text;
        Certificate.IssuedAt = issued.Value;
        Certificate.ValidFrom = vf.Value;
        Certificate.ValidTo = vt.Value;
        Certificate.Purpose = purpose.Text;
        DialogResult = DialogResult.OK;
    }
}
