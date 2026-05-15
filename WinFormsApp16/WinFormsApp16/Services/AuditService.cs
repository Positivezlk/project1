using System.Data;
using CertDesk.Models;
using Microsoft.Data.Sqlite;
namespace CertDesk.Services;
public static class AuditService
{
    public static void Write(CurrentUser? user,string action,string? entityType,int? entityId,string? description)
    { DbUtil.Execute("INSERT INTO audit_log(user_id,login,action,entity_type,entity_id,description) VALUES(@u,@l,@a,@t,@i,@d)",DbUtil.P("@u",user?.Id),DbUtil.P("@l",user?.Login),DbUtil.P("@a",action),DbUtil.P("@t",entityType),DbUtil.P("@i",entityId),DbUtil.P("@d",description)); }
    public static DataTable GetList(DateTime from,DateTime to,string action,string login)
    { return DbUtil.Query(@"SELECT id AS ID, strftime('%d.%m.%Y %H:%M',created_at) AS 'Дата и время', COALESCE(login,'система') AS 'Пользователь', action AS 'Действие', entity_type AS 'Объект', entity_id AS 'ID объекта', description AS 'Описание' FROM audit_log WHERE date(created_at) BETWEEN date(@f) AND date(@t) AND (@a='' OR action=@a) AND (@l='' OR login LIKE @lp) ORDER BY id DESC",DbUtil.P("@f",DbUtil.D(from)),DbUtil.P("@t",DbUtil.D(to)),DbUtil.P("@a",action),DbUtil.P("@l",login),DbUtil.P("@lp","%"+login+"%")); }
    public static List<string> Actions(){ var t=DbUtil.Query("SELECT DISTINCT action FROM audit_log ORDER BY action"); return t.Rows.Cast<DataRow>().Select(r=>r[0].ToString()!).ToList(); }
}
