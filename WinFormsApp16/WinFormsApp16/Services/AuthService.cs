using CertDesk.Models;
using Microsoft.Data.Sqlite;
namespace CertDesk.Services;
public static class AuthService
{
    public static CurrentUser? Login(string login,string password)
    { var t=DbUtil.Query("SELECT id,login,password_hash,role,employee_id FROM users WHERE login=@l AND is_active=1",DbUtil.P("@l",login.Trim())); if(t.Rows.Count!=1) return null; var r=t.Rows[0]; if(!PasswordHasher.VerifyPassword(password,Convert.ToString(r["password_hash"])!)) return null; return new CurrentUser{Id=Convert.ToInt32(r["id"]),Login=Convert.ToString(r["login"])!,Role=Convert.ToString(r["role"])!,EmployeeId=r["employee_id"]==DBNull.Value?null:Convert.ToInt32(r["employee_id"])}; }
}
