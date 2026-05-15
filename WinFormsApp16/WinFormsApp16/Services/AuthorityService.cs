using System.Data; using CertDesk.Models;
namespace CertDesk.Services;
public static class AuthorityService { public static List<Authority> GetActive(){ var t=DbUtil.Query("SELECT * FROM authorities WHERE is_active=1 ORDER BY name"); return t.Rows.Cast<DataRow>().Select(r=>new Authority{Id=Convert.ToInt32(r["id"]),Name=Convert.ToString(r["name"])!,Inn=Convert.ToString(r["inn"]),AccreditationNumber=Convert.ToString(r["accreditation_number"]),Website=Convert.ToString(r["website"]),IsActive=true}).ToList(); } }
