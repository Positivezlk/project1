using CertDesk.Models;
namespace CertDesk.Common;
public static class RoleGuard { public static bool CanEdit(CurrentUser u)=>u.Role is "administrator" or "specialist"; public static bool CanExport(CurrentUser u)=>u.Role is "administrator" or "specialist"; public static bool CanViewAudit(CurrentUser u)=>u.Role=="administrator"; public static bool CanBackup(CurrentUser u)=>u.Role=="administrator"; public static bool CanManageTokens(CurrentUser u)=>u.Role is "administrator" or "specialist"; }
