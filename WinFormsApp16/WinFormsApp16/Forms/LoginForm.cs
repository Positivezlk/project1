using CertDesk.Common;
using CertDesk.Services;

namespace CertDesk.Forms;

public partial class LoginForm : Form
{
    private readonly TextBox txtLogin = new() { Width = 300 };
    private readonly TextBox txtPassword = new() { Width = 300, UseSystemPasswordChar = true };

    public LoginForm()
    {
        InitializeComponent();
        Build();
    }

    private void Build()
    {
        UiTheme.ApplyFormStyle(this);
        ClientSize = new Size(460, 420);
        MinimumSize = new Size(460, 420);
        MaximumSize = new Size(460, 420);
        BackColor = UiTheme.Light;

        Panel card = new() { Left = 40, Top = 34, Width = 380, Height = 340 };
        UiTheme.ApplyCardStyle(card);

        Label title = new() { Text = "CertDesk", Left = 0, Top = 22, Width = card.Width, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 24F, FontStyle.Bold), ForeColor = UiTheme.Primary };
        Label subtitle = new() { Text = "Контроль сертификатов ЭП и МЧД", Left = 0, Top = 66, Width = card.Width, TextAlign = ContentAlignment.MiddleCenter, ForeColor = UiTheme.Secondary, Font = new Font("Segoe UI", 10F) };

        Label loginLabel = new() { Text = "Логин", Left = 38, Top = 112, AutoSize = true };
        Label passwordLabel = new() { Text = "Пароль", Left = 38, Top = 168, AutoSize = true };
        txtLogin.Left = 38;
        txtLogin.Top = 132;
        txtPassword.Left = 38;
        txtPassword.Top = 188;
        UiTheme.ApplyTextBoxStyle(txtLogin);
        UiTheme.ApplyTextBoxStyle(txtPassword);

        Button signInButton = new() { Text = "Войти", Left = 38, Top = 238, Width = 142 };
        Button exitButton = new() { Text = "Выход", Left = 196, Top = 238, Width = 142 };
        UiTheme.ApplyButtonStyle(signInButton);
        UiTheme.ApplySecondaryButtonStyle(exitButton);

        Label hint = new()
        {
            Text = "Тестовые пользователи:\nadmin/admin123   spec/spec123   view/view123",
            Left = 38,
            Top = 292,
            Width = 310,
            Height = 38,
            ForeColor = UiTheme.Secondary,
            Font = new Font("Segoe UI", 8.5F),
            TextAlign = ContentAlignment.MiddleCenter
        };

        card.Controls.AddRange(new Control[] { title, subtitle, loginLabel, txtLogin, passwordLabel, txtPassword, signInButton, exitButton, hint });
        Controls.Add(card);

        signInButton.Click += (_, _) => DoLogin();
        exitButton.Click += (_, _) => Close();
        AcceptButton = signInButton;
    }

    private void DoLogin()
    {
        try
        {
            var user = AuthService.Login(txtLogin.Text, txtPassword.Text);
            if (user == null)
            {
                MessageHelper.Error("Неверный логин или пароль");
                return;
            }

            AuditService.Write(user, "login", "users", user.Id, "Вход в систему");
            Hide();
            using MainForm mainForm = new(user);
            mainForm.ShowDialog();
            Show();
            txtPassword.Clear();
        }
        catch (Exception ex)
        {
            MessageHelper.Error("Ошибка входа: " + ex.Message);
        }
    }
}
