using System.Data; using CertDesk.Reports; using CertDesk.Models;
namespace CertDesk.Services;
public static class ReportService
{
 public static string OutputDirectory { get { var d=Path.Combine(AppContext.BaseDirectory,"ReportsOutput"); Directory.CreateDirectory(d); return d; } }
 public static string[] ReportTypes => new[]{"Реестр сотрудников","Реестр сертификатов","Сертификаты, истекающие в течение 30 дней","Реестр МЧД","МЧД, истекающие в течение 30 дней","Реестр токенов","Журнал операций токенов","Сводка по подразделениям"};
 public static DataTable Build(string type,DateTime from,DateTime to)=>type switch{
  "Реестр сотрудников"=>EmployeeService.Search("","Все"),
  "Реестр сертификатов"=>CertificateService.Search("","Все","Все"),
  "Сертификаты, истекающие в течение 30 дней"=>DbUtil.Query("SELECT * FROM ("+"SELECT c.serial_number AS 'Серийный номер',e.full_name AS 'Владелец',c.valid_to AS 'Действует до',c.status AS 'Статус' FROM certificates c JOIN employees e ON e.id=c.employee_id"+") WHERE status='warning'"),
  "Реестр МЧД"=>MchdService.Search("","Все","Все"),
  "МЧД, истекающие в течение 30 дней"=>DbUtil.Query("SELECT m.number AS 'Номер',e.full_name AS 'Представитель',m.valid_to AS 'Действует до',m.status AS 'Статус' FROM mchd m JOIN employees e ON e.id=m.representative_employee_id WHERE m.status='warning'"),
  "Реестр токенов"=>TokenService.Search("","Все"),
  "Журнал операций токенов"=>TokenService.GetOperations(),
  _=>DbUtil.Query("SELECT department AS 'Подразделение', COUNT(*) AS 'Сотрудников' FROM employees GROUP BY department ORDER BY department")};
 public static string ExportCsv(string type,DateTime from,DateTime to,CurrentUser u){ var path=Path.Combine(OutputDirectory,Safe(type)+"_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".csv"); CsvExporter.Export(Build(type,from,to),path); AuditService.Write(u,"экспорт отчета","reports",null,type+" CSV"); return path; }
 public static string ExportXlsx(string type,DateTime from,DateTime to,CurrentUser u){ var path=Path.Combine(OutputDirectory,Safe(type)+"_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".xlsx"); XlsxExporter.Export(Build(type,from,to),path,type); AuditService.Write(u,"экспорт отчета","reports",null,type+" XLSX"); return path; }
 static string Safe(string s)=>string.Concat(s.Select(ch=>Path.GetInvalidFileNameChars().Contains(ch)?'_':ch)).Replace(' ','_');
}
