using CertDesk.Common;
using CertDesk.Models;
using CertDesk.Services;
using CertDesk.Views;

namespace CertDesk.Forms;

public partial class MainForm : Form
{
    private readonly CurrentUser _user;
    private readonly Panel _menu = new() { Dock = DockStyle.Left, Width = 232 };
    private readonly Panel _content = new() { Dock = DockStyle.Fill };
    private readonly StatusStrip _status = new();
    private readonly List<Button> _menuButtons = new();

    public MainForm(CurrentUser user)
    {
        _user = user;
        InitializeComponent();
        Build();
        ShowView(new DashboardView(_user), null);
    }

    private void Build()
    {
        UiTheme.ApplyFormStyle(this);
        BackColor = UiTheme.Light;

        Panel header = new() { Dock = DockStyle.Top };
        UiTheme.ApplyMainHeaderStyle(header);
        Label title = new() { Text = "CertDesk", Left = 18, Top = 8, AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 18F, FontStyle.Bold) };
        Label subtitle = new() { Text = "Контроль сертификатов ЭП и МЧД", Left = 20, Top = 38, AutoSize = true, ForeColor = Color.FromArgb(225, 235, 245), Font = new Font("Segoe UI", 9F) };
        Label userLabel = new()
        {
            Text = $"{_user.Login} • {_user.RoleTitle}",
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Left = ClientSize.Width - 300,
            Top = 22,
            Width = 260,
            TextAlign = ContentAlignment.TopRight,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };
        header.Controls.AddRange(new Control[] { title, subtitle, userLabel });

        UiTheme.ApplySidebarStyle(_menu);
        UiTheme.ApplyContentStyle(_content);
        _status.BackColor = Color.White;
        _status.ForeColor = UiTheme.Secondary;
        _status.Items.Add("Готово");

        Controls.AddRange(new Control[] { _content, _menu, header, _status });

        AddMenuButton("Главная", button => ShowView(new DashboardView(_user), button));
        AddMenuButton("Сертификаты", button => ShowView(new CertificatesView(_user), button));
        AddMenuButton("МЧД", button => ShowView(new MchdView(_user), button));
        AddMenuButton("Токены", button => ShowView(new TokensView(_user), button));
        AddMenuButton("Сотрудники", button => ShowView(new EmployeesView(_user), button));
        if (RoleGuard.CanExport(_user)) AddMenuButton("Отчеты", button => ShowView(new ReportsView(_user), button));
        if (RoleGuard.CanViewAudit(_user)) AddMenuButton("Аудит", button => ShowView(new AuditView(_user), button));
        if (RoleGuard.CanBackup(_user)) AddMenuButton("Резервная копия", _ => Backup());
        AddMenuButton("Выход", _ => Logout());
    }

    private void AddMenuButton(string text, Action<Button> click)
    {
        Button button = new() { Text = text, Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 6) };
        UiTheme.ApplyMenuButtonStyle(button);
        button.Click += (_, _) => click(button);
        _menuButtons.Add(button);
        _menu.Controls.Add(button);
        _menu.Controls.SetChildIndex(button, _menu.Controls.Count - 1);
    }

    private void ShowView(UserControl view, Button? activeButton)
    {
        foreach (Button button in _menuButtons)
        {
            UiTheme.ApplyMenuButtonStyle(button, button == activeButton);
        }

        _content.Controls.Clear();
        view.Dock = DockStyle.Fill;
        _content.Controls.Add(view);
        _status.Items[0].Text = "Открыт раздел: " + view.Name;
    }

    private void Backup()
    {
        try
        {
            string path = BackupService.CreateBackup(_user);
            MessageHelper.Info("Резервная копия создана:\n" + path);
        }
        catch (Exception ex)
        {
            MessageHelper.Error(ex.Message);
        }
    }

    private void Logout()
    {
        AuditService.Write(_user, "logout", "users", _user.Id, "Выход из системы");
        Close();
    }
}
