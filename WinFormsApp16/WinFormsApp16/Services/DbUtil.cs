using System.Data;
using CertDesk.Data;
using Microsoft.Data.Sqlite;
namespace CertDesk.Services;
internal static class DbUtil
{
    public static string D(DateTime d)=>d.ToString("yyyy-MM-dd");
    public static DateTime ParseDate(object? v)=>DateTime.TryParse(Convert.ToString(v),out var d)?d:DateTime.Today;
    public static DataTable Query(string sql, params SqliteParameter[] ps){ using var c=Db.OpenConnection(); using var cmd=c.CreateCommand(); cmd.CommandText=sql; cmd.Parameters.AddRange(ps); using var r=cmd.ExecuteReader(); var t=new DataTable(); t.Load(r); return t; }
    public static int Execute(string sql, params SqliteParameter[] ps){ using var c=Db.OpenConnection(); using var cmd=c.CreateCommand(); cmd.CommandText=sql; cmd.Parameters.AddRange(ps); return cmd.ExecuteNonQuery(); }
    public static object? Scalar(string sql, params SqliteParameter[] ps){ using var c=Db.OpenConnection(); using var cmd=c.CreateCommand(); cmd.CommandText=sql; cmd.Parameters.AddRange(ps); return cmd.ExecuteScalar(); }
    public static SqliteParameter P(string n, object? v)=>new(n, v ?? DBNull.Value);
}
