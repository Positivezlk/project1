using CertDesk.Data; using CertDesk.Models;
namespace CertDesk.Services;
public static class BackupService { public static string CreateBackup(CurrentUser u){ var d=Path.Combine(AppContext.BaseDirectory,"Backups"); Directory.CreateDirectory(d); var path=Path.Combine(d,"certdesk_backup_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".db"); File.Copy(Db.DatabasePath,path,true); AuditService.Write(u,"создание резервной копии","backup",null,path); return path; } }
