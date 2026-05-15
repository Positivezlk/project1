using CertDesk.Common;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Forms;

public partial class TokenEditForm : Form
{
    public TokenDevice Token { get; }

    private readonly TextBox inv = new() { Width = 300 };
    private readonly TextBox model = new() { Width = 300 };
    private readonly TextBox sn = new() { Width = 300 };
    private readonly TextBox notes = new() { Width = 300 };
    private readonly ComboBox type = new() { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox status = new() { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox holder = new() { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker rec = new();

    public TokenEditForm(TokenDevice? t = null)
    {
        Token = t ?? new TokenDevice();
        InitializeComponent();
        Build();
        Lists();
        if (t != null) LoadT();
    }

    private void Build()
    {
        UiTheme.ApplyFormStyle(this);
        Text = "Токен";
        ClientSize = new Size(570, 540);
        BackColor = UiTheme.Light;

        Label title = new() { Text = "Носитель ключевой информации", Left = 24, Top = 18 };
        UiTheme.ApplyLabelTitleStyle(title);
        Controls.Add(title);

        Panel card = new() { Left = 24, Top = 64, Width = 520, Height = 380 };
        UiTheme.ApplyCardStyle(card);
        Controls.Add(card);

        int y = 8;
        AddRow(card, "Инвентарный номер", inv, ref y);
        AddRow(card, "Тип токена", type, ref y);
        AddRow(card, "Модель", model, ref y);
        AddRow(card, "Серийный номер", sn, ref y);
        AddRow(card, "Дата поступления", rec, ref y);
        AddRow(card, "Статус", status, ref y);
        AddRow(card, "Держатель", holder, ref y);
        AddRow(card, "Примечание", notes, ref y);

        Button ok = new() { Text = "Сохранить", Left = 314, Top = 464, Width = 110 };
        Button cancel = new() { Text = "Отмена", Left = 434, Top = 464, Width = 110, DialogResult = DialogResult.Cancel };
        UiTheme.ApplyButtonStyle(ok);
        UiTheme.ApplySecondaryButtonStyle(cancel);
        Controls.AddRange(new Control[] { ok, cancel });
        ok.Click += Save;
    }

    private static void AddRow(Control parent, string labelText, Control control, ref int y)
    {
        Label label = new() { Text = labelText, Left = 0, Top = y + 5, Width = 150 };
        UiTheme.ApplyLabelMutedStyle(label);
        control.Left = 170;
        control.Top = y;
        if (control is TextBox textBox) UiTheme.ApplyTextBoxStyle(textBox);
        if (control is ComboBox comboBox) UiTheme.ApplyComboBoxStyle(comboBox);
        if (control is DateTimePicker picker) UiTheme.ApplyDatePickerStyle(picker);
        parent.Controls.Add(label);
        parent.Controls.Add(control);
        y += 40;
    }

    private void Lists()
    {
        type.Items.AddRange(new object[] { "Rutoken", "JaCarta", "eToken", "Other" });
        status.Items.AddRange(new object[] { "storage", "issued", "damaged", "written_off" });
        holder.Items.Add(new ComboBoxItem { Text = "Не указан" });
        foreach (Employee employee in EmployeeService.GetActive()) holder.Items.Add(new ComboBoxItem { Id = employee.Id, Text = employee.FullName });
        type.SelectedIndex = 0;
        status.SelectedIndex = 0;
        holder.SelectedIndex = 0;
    }

    private void LoadT()
    {
        inv.Text = Token.InventoryNumber;
        type.Text = Token.TokenType;
        model.Text = Token.Model;
        sn.Text = Token.SerialNumber;
        if (Token.ReceivedAt.HasValue) rec.Value = Token.ReceivedAt.Value;
        status.Text = Token.Status;
        foreach (ComboBoxItem item in holder.Items) if (item.Id == Token.HolderId) holder.SelectedItem = item;
        notes.Text = Token.Notes;
    }

    private void Save(object? sender, EventArgs args)
    {
        if (string.IsNullOrWhiteSpace(inv.Text) || type.SelectedItem == null)
        {
            MessageHelper.Error("Инвентарный номер и тип обязательны");
            return;
        }

        Token.InventoryNumber = inv.Text.Trim();
        Token.TokenType = type.Text;
        Token.Model = model.Text;
        Token.SerialNumber = sn.Text;
        Token.ReceivedAt = rec.Value;
        Token.Status = status.Text;
        Token.HolderId = ((ComboBoxItem)holder.SelectedItem).Id;
        Token.Notes = notes.Text;
        DialogResult = DialogResult.OK;
    }
}
