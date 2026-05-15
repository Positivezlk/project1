using CertDesk.Common;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Forms;

public partial class MchdEditForm : Form
{
    public Mchd Mchd { get; }

    private readonly ComboBox pr = new() { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox rep = new() { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox cert = new() { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox num = new() { Width = 340 };
    private readonly TextBox powers = new() { Width = 340, Height = 76, Multiline = true };
    private readonly TextBox codes = new() { Width = 340 };
    private readonly TextBox notes = new() { Width = 340 };
    private readonly DateTimePicker vf = new();
    private readonly DateTimePicker vt = new();
    private readonly CheckBox reg = new() { Text = "Зарегистрирована" };

    public MchdEditForm(Mchd? m = null)
    {
        Mchd = m ?? new Mchd();
        InitializeComponent();
        Build();
        Lists();
        if (m != null) LoadM();
    }

    private void Build()
    {
        UiTheme.ApplyFormStyle(this);
        Text = "МЧД";
        ClientSize = new Size(660, 660);
        BackColor = UiTheme.Light;

        Label title = new() { Text = "Машиночитаемая доверенность", Left = 24, Top = 18 };
        UiTheme.ApplyLabelTitleStyle(title);
        Controls.Add(title);

        Panel card = new() { Left = 24, Top = 64, Width = 610, Height = 500 };
        UiTheme.ApplyCardStyle(card);
        Controls.Add(card);

        int y = 8;
        AddRow(card, "Номер МЧД", num, ref y);
        AddRow(card, "Доверитель", pr, ref y);
        AddRow(card, "Представитель", rep, ref y);
        AddRow(card, "Сертификат", cert, ref y);
        AddRow(card, "Полномочия", powers, ref y);
        AddRow(card, "Коды полномочий", codes, ref y);
        AddRow(card, "Действует с", vf, ref y);
        AddRow(card, "Действует до", vt, ref y);
        AddRow(card, "Примечание", notes, ref y);
        reg.Left = 210;
        reg.Top = y + 4;
        reg.ForeColor = UiTheme.Text;
        card.Controls.Add(reg);

        Button ok = new() { Text = "Сохранить", Left = 404, Top = 584, Width = 110 };
        Button cancel = new() { Text = "Отмена", Left = 524, Top = 584, Width = 110, DialogResult = DialogResult.Cancel };
        UiTheme.ApplyButtonStyle(ok);
        UiTheme.ApplySecondaryButtonStyle(cancel);
        Controls.AddRange(new Control[] { ok, cancel });
        ok.Click += Save;
    }

    private static void AddRow(Control parent, string labelText, Control control, ref int y)
    {
        Label label = new() { Text = labelText, Left = 0, Top = y + 5, Width = 190 };
        UiTheme.ApplyLabelMutedStyle(label);
        control.Left = 210;
        control.Top = y;
        if (control is TextBox textBox) UiTheme.ApplyTextBoxStyle(textBox);
        if (control is ComboBox comboBox) UiTheme.ApplyComboBoxStyle(comboBox);
        if (control is DateTimePicker picker) UiTheme.ApplyDatePickerStyle(picker);
        parent.Controls.Add(label);
        parent.Controls.Add(control);
        y += control.Height + 12;
    }

    private void Lists()
    {
        pr.Items.Add(new ComboBoxItem { Text = "Не указан" });
        cert.Items.Add(new ComboBoxItem { Text = "Не указан" });
        foreach (Employee employee in EmployeeService.GetActive())
        {
            pr.Items.Add(new ComboBoxItem { Id = employee.Id, Text = employee.FullName });
            rep.Items.Add(new ComboBoxItem { Id = employee.Id, Text = employee.FullName });
        }
        foreach (Certificate certificate in CertificateService.GetAll()) cert.Items.Add(new ComboBoxItem { Id = certificate.Id, Text = certificate.SerialNumber });
        pr.SelectedIndex = 0;
        if (rep.Items.Count > 0) rep.SelectedIndex = 0;
        cert.SelectedIndex = 0;
    }

    private void LoadM()
    {
        num.Text = Mchd.Number;
        Sel(pr, Mchd.PrincipalEmployeeId);
        Sel(rep, Mchd.RepresentativeEmployeeId);
        Sel(cert, Mchd.CertificateId);
        powers.Text = Mchd.Powers;
        codes.Text = Mchd.PowersCodes;
        vf.Value = Mchd.ValidFrom;
        vt.Value = Mchd.ValidTo;
        reg.Checked = Mchd.IsRegistered;
        notes.Text = Mchd.Notes;
    }

    private static void Sel(ComboBox comboBox, int? id)
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
        if (string.IsNullOrWhiteSpace(num.Text) || rep.SelectedItem == null || string.IsNullOrWhiteSpace(powers.Text))
        {
            MessageHelper.Error("Заполните номер, представителя и полномочия");
            return;
        }
        if (vt.Value.Date < vf.Value.Date)
        {
            MessageHelper.Error("Дата окончания не может быть раньше начала");
            return;
        }

        Mchd.Number = num.Text.Trim();
        Mchd.PrincipalEmployeeId = ((ComboBoxItem)pr.SelectedItem).Id;
        Mchd.RepresentativeEmployeeId = ((ComboBoxItem)rep.SelectedItem).Id!.Value;
        Mchd.CertificateId = ((ComboBoxItem)cert.SelectedItem).Id;
        Mchd.Powers = powers.Text;
        Mchd.PowersCodes = codes.Text;
        Mchd.ValidFrom = vf.Value;
        Mchd.ValidTo = vt.Value;
        Mchd.IsRegistered = reg.Checked;
        Mchd.Notes = notes.Text;
        DialogResult = DialogResult.OK;
    }
}
