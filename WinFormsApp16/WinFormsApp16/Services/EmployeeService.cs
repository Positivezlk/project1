using System.Data;
using CertDesk.Models;
namespace CertDesk.Services;
public static class EmployeeService
{
 public static DataTable Search(string q="",string status="Все")=>DbUtil.Query(@"SELECT id AS ID, full_name AS 'ФИО', position AS 'Должность', department AS 'Подразделение', email AS Email, phone AS 'Телефон', snils AS 'СНИЛС', inn AS 'ИНН', CASE is_active WHEN 1 THEN 'Активен' ELSE 'Архивный' END AS 'Статус' FROM employees WHERE (@q='' OR full_name LIKE @p OR position LIKE @p OR department LIKE @p) AND (@s='Все' OR (@s='Активные' AND is_active=1) OR (@s='Архивные' AND is_active=0)) ORDER BY full_name",DbUtil.P("@q",q),DbUtil.P("@p","%"+q+"%"),DbUtil.P("@s",status));
 public static List<Employee> GetActive(){ var t=DbUtil.Query("SELECT * FROM employees WHERE is_active=1 ORDER BY full_name"); return t.Rows.Cast<DataRow>().Select(Map).ToList(); }
 public static Employee? GetById(int id){ var t=DbUtil.Query("SELECT * FROM employees WHERE id=@id",DbUtil.P("@id",id)); return t.Rows.Count==0?null:Map(t.Rows[0]); }
 static Employee Map(DataRow r)=>new(){Id=Convert.ToInt32(r["id"]),FullName=Convert.ToString(r["full_name"])!,Position=Convert.ToString(r["position"])!,Department=Convert.ToString(r["department"]),Email=Convert.ToString(r["email"]),Phone=Convert.ToString(r["phone"]),Snils=Convert.ToString(r["snils"]),Inn=Convert.ToString(r["inn"]),IsActive=Convert.ToInt32(r["is_active"])==1};
 public static int Create(Employee e,CurrentUser u){ var id=Convert.ToInt32(DbUtil.Scalar("INSERT INTO employees(full_name,position,department,email,phone,snils,inn,is_active) VALUES(@n,@p,@d,@e,@ph,@s,@i,@a) RETURNING id",P(e))); AuditService.Write(u,"создание сотрудника","employees",id,e.FullName); return id; }
 public static void Update(Employee e,CurrentUser u){ DbUtil.Execute("UPDATE employees SET full_name=@n,position=@p,department=@d,email=@e,phone=@ph,snils=@s,inn=@i,is_active=@a WHERE id=@id",P(e).Append(DbUtil.P("@id",e.Id)).ToArray()); AuditService.Write(u,"изменение сотрудника","employees",e.Id,e.FullName); }
 public static void Archive(int id,CurrentUser u){ DbUtil.Execute("UPDATE employees SET is_active=0 WHERE id=@id",DbUtil.P("@id",id)); AuditService.Write(u,"архивирование сотрудника","employees",id,"Сотрудник архивирован"); }
 static Microsoft.Data.Sqlite.SqliteParameter[] P(Employee e)=>new[]{DbUtil.P("@n",e.FullName),DbUtil.P("@p",e.Position),DbUtil.P("@d",e.Department),DbUtil.P("@e",e.Email),DbUtil.P("@ph",e.Phone),DbUtil.P("@s",e.Snils),DbUtil.P("@i",e.Inn),DbUtil.P("@a",e.IsActive?1:0)};
}
