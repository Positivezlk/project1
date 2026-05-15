namespace CertDesk.Services;
public static class StatusService
{
    public static string CalculateCertificateStatus(DateTime validTo,string currentStatus)=>Calc(validTo,currentStatus);
    public static string CalculateMchdStatus(DateTime validTo,string currentStatus)=>Calc(validTo,currentStatus);
    private static string Calc(DateTime validTo,string currentStatus){ if(currentStatus is "revoked" or "archived") return currentStatus; var left=DaysLeft(validTo); if(validTo.Date<DateTime.Today) return "expired"; return left<=30?"warning":"active"; }
    public static int DaysLeft(DateTime validTo)=>(validTo.Date-DateTime.Today).Days;
    public static void UpdateAllStatuses(){ DbUtil.Execute("UPDATE certificates SET status=CASE WHEN status IN ('revoked','archived') THEN status WHEN date(valid_to)<date('now') THEN 'expired' WHEN julianday(valid_to)-julianday(date('now'))<=30 THEN 'warning' ELSE 'active' END"); DbUtil.Execute("UPDATE mchd SET status=CASE WHEN status IN ('revoked','archived') THEN status WHEN date(valid_to)<date('now') THEN 'expired' WHEN julianday(valid_to)-julianday(date('now'))<=30 THEN 'warning' ELSE 'active' END"); }
}
