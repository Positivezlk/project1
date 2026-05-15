using CertDesk.Common; using CertDesk.Services;
namespace CertDesk.Forms;
public partial class LoginForm:Form
{
 TextBox txtLogin=new(){Width=240}; TextBox txtPassword=new(){Width=240,UseSystemPasswordChar=true};
 public LoginForm(){ InitializeComponent(); Build(); }
 void Build(){ BackColor=UiTheme.Light; var title=new Label{Text="CertDesk",Font=new Font("Segoe UI",22,FontStyle.Bold),ForeColor=UiTheme.Primary,AutoSize=true,Left=145,Top=25}; var sub=new Label{Text="Контроль сертификатов ЭП и МЧД",AutoSize=true,Left=95,Top=72,ForeColor=UiTheme.Secondary}; var p=new Panel{Left=55,Top=110,Width=320,Height=170,BackColor=Color.White}; UiTheme.ApplyPanelStyle(p); p.Controls.AddRange(new Control[]{new Label{Text="Логин",Left=25,Top=20,AutoSize=true},txtLogin,new Label{Text="Пароль",Left=25,Top=75,AutoSize=true},txtPassword}); txtLogin.Left=25;txtLogin.Top=40; txtPassword.Left=25;txtPassword.Top=95; var b=new Button{Text="Войти",Left=25,Top=130,Width=115}; var ex=new Button{Text="Выход",Left=150,Top=130,Width=115}; UiTheme.ApplyButtonStyle(b); UiTheme.ApplyDangerButtonStyle(ex); p.Controls.AddRange(new Control[]{b,ex}); var hint=new Label{Text="Тестовые пользователи: admin/admin123, spec/spec123, view/view123",Left=35,Top=290,Width=360,Height=35,ForeColor=UiTheme.Secondary}; Controls.AddRange(new Control[]{title,sub,p,hint}); b.Click+=(_,__)=>DoLogin(); ex.Click+=(_,__)=>Close(); AcceptButton=b; }
 void DoLogin(){ try{ var u=AuthService.Login(txtLogin.Text,txtPassword.Text); if(u==null){ MessageHelper.Error("Неверный логин или пароль"); return;} AuditService.Write(u,"login","users",u.Id,"Вход в систему"); Hide(); using var m=new MainForm(u); m.ShowDialog(); Show(); txtPassword.Clear(); } catch(Exception ex){ MessageHelper.Error("Ошибка входа: "+ex.Message); } }
}
