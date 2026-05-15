namespace CertDesk.Common;

public static class UiTheme
{
    public static readonly Color Primary = ColorTranslator.FromHtml("#2F5F9F");
    public static readonly Color PrimaryDark = ColorTranslator.FromHtml("#244A7C");
    public static readonly Color Light = ColorTranslator.FromHtml("#F2F4F7");
    public static readonly Color Card = Color.White;
    public static readonly Color Border = ColorTranslator.FromHtml("#D7DCE2");
    public static readonly Color Text = ColorTranslator.FromHtml("#1F2933");
    public static readonly Color Secondary = ColorTranslator.FromHtml("#6B7280");
    public static readonly Color Success = ColorTranslator.FromHtml("#2E7D32");
    public static readonly Color Warning = ColorTranslator.FromHtml("#F39C12");
    public static readonly Color Danger = ColorTranslator.FromHtml("#C0392B");
    public static readonly Color Muted = ColorTranslator.FromHtml("#7F8C8D");

    private static readonly Font DefaultFont = new("Segoe UI", 9F, FontStyle.Regular);
    private static readonly Font TitleFont = new("Segoe UI", 17F, FontStyle.Bold);
    private static readonly Font HeaderFont = new("Segoe UI", 9F, FontStyle.Bold);

    public static void ApplyFormStyle(Form form)
    {
        form.BackColor = Light;
        form.Font = DefaultFont;
        form.StartPosition = form.StartPosition == FormStartPosition.Manual ? FormStartPosition.CenterScreen : form.StartPosition;
    }

    public static void ApplyMainHeaderStyle(Panel panel)
    {
        panel.BackColor = Primary;
        panel.Height = 62;
        panel.Padding = new Padding(18, 8, 18, 8);
    }

    public static void ApplySidebarStyle(Panel panel)
    {
        panel.BackColor = Card;
        panel.Width = 232;
        panel.Padding = new Padding(10, 14, 10, 10);
    }

    public static void ApplyContentStyle(Panel panel)
    {
        panel.BackColor = Light;
        panel.Padding = new Padding(18);
    }

    public static void ApplyCardStyle(Panel panel)
    {
        panel.BackColor = Card;
        panel.BorderStyle = BorderStyle.FixedSingle;
        panel.Padding = new Padding(14);
    }

    public static void ApplyPanelStyle(Panel panel) => ApplyCardStyle(panel);

    public static void ApplyButtonStyle(Button button)
    {
        ConfigureButton(button, Primary, PrimaryDark, Color.White, Primary);
    }

    public static void ApplySecondaryButtonStyle(Button button)
    {
        ConfigureButton(button, ColorTranslator.FromHtml("#E8EEF5"), Border, Text, Border);
        button.FlatAppearance.BorderSize = 1;
    }

    public static void ApplyDangerButtonStyle(Button button)
    {
        ConfigureButton(button, Danger, ColorTranslator.FromHtml("#A93226"), Color.White, Danger);
    }

    public static void ApplyMenuButtonStyle(Button button, bool active = false)
    {
        button.Height = 42;
        button.Cursor = Cursors.Hand;
        button.Font = new Font("Segoe UI", 10F, active ? FontStyle.Bold : FontStyle.Regular);
        button.TextAlign = ContentAlignment.MiddleLeft;
        button.Padding = new Padding(16, 0, 0, 0);
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = active ? Primary : Card;
        button.ForeColor = active ? Color.White : Text;
        button.FlatAppearance.MouseOverBackColor = active ? PrimaryDark : ColorTranslator.FromHtml("#E8EEF5");
    }

    public static void ApplyTextBoxStyle(TextBox textBox)
    {
        textBox.Font = DefaultFont;
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.BackColor = Card;
        textBox.ForeColor = Text;
        if (textBox.Height < 28 && !textBox.Multiline) textBox.Height = 28;
    }

    public static void ApplyComboBoxStyle(ComboBox comboBox)
    {
        comboBox.Font = DefaultFont;
        comboBox.BackColor = Card;
        comboBox.ForeColor = Text;
        if (comboBox.Height < 28) comboBox.Height = 28;
    }

    public static void ApplyDatePickerStyle(DateTimePicker picker)
    {
        picker.Font = DefaultFont;
        picker.CalendarForeColor = Text;
        picker.CalendarTitleBackColor = Primary;
        picker.CalendarTitleForeColor = Color.White;
        if (picker.Height < 28) picker.Height = 28;
    }

    public static void ApplyGridStyle(DataGridView grid)
    {
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowHeadersVisible = false;
        grid.BackgroundColor = Card;
        grid.BorderStyle = BorderStyle.None;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = HeaderFont;
        grid.ColumnHeadersHeight = 34;
        grid.DefaultCellStyle.Font = DefaultFont;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.BackColor = Card;
        grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#DCEBFA");
        grid.DefaultCellStyle.SelectionForeColor = Text;
        grid.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8FAFC");
        grid.GridColor = Border;
        grid.RowTemplate.Height = 30;
        ApplyStatusCellFormatting(grid);
    }

    public static void ApplyLabelTitleStyle(Label label)
    {
        label.Font = TitleFont;
        label.ForeColor = Text;
        label.AutoSize = true;
    }

    public static void ApplyLabelMutedStyle(Label label)
    {
        label.Font = DefaultFont;
        label.ForeColor = Secondary;
        label.AutoSize = true;
    }

    public static string GetStatusText(string status) => status switch
    {
        "active" => "действует",
        "warning" => "истекает",
        "expired" => "истек",
        "revoked" => "отозван",
        "archived" => "архивный",
        "storage" => "на хранении",
        "issued" => "выдан",
        "damaged" => "поврежден",
        "written_off" => "списан",
        _ => status
    };

    public static Color GetStatusColor(string status) => status.Trim().ToLowerInvariant() switch
    {
        "active" or "действует" or "storage" or "на хранении" or "активен" => Success,
        "warning" or "истекает" or "issued" or "выдан" => Warning,
        "expired" or "истек" or "revoked" or "отозван" or "отозвана" or "damaged" or "поврежден" => Danger,
        "archived" or "архивный" or "архивная" or "written_off" or "списан" or "архивные" => Muted,
        _ => Text
    };

    public static void ApplyStatusCellFormatting(DataGridView grid)
    {
        grid.CellFormatting -= GridOnCellFormatting;
        grid.CellFormatting += GridOnCellFormatting;
    }

    private static void GridOnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (sender is not DataGridView grid || e.RowIndex < 0 || e.ColumnIndex < 0 || e.Value is null) return;
        string header = grid.Columns[e.ColumnIndex].HeaderText.ToLowerInvariant();
        if (!header.Contains("статус") && !header.Contains("status")) return;

        string raw = Convert.ToString(e.Value) ?? string.Empty;
        Color color = GetStatusColor(raw);
        e.CellStyle.ForeColor = color;
        e.CellStyle.SelectionForeColor = color;
        e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        string text = GetStatusText(raw);
        if (!string.Equals(text, raw, StringComparison.Ordinal))
        {
            e.Value = text;
            e.FormattingApplied = true;
        }
    }

    private static void ConfigureButton(Button button, Color back, Color hover, Color fore, Color border)
    {
        button.BackColor = back;
        button.ForeColor = fore;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = border;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = hover;
        button.Height = Math.Max(button.Height, 36);
        button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
    }
}
