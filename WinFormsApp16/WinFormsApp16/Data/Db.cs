using Microsoft.Data.Sqlite;
namespace CertDesk.Data;
public static class Db
{
    public static string DataDirectory { get { var d=Path.Combine(AppContext.BaseDirectory,"DataFiles"); Directory.CreateDirectory(d); return d; } }
    public static string DatabasePath => Path.Combine(DataDirectory,"certdesk.db");
    public static SqliteConnection OpenConnection(){ var c=new SqliteConnection($"Data Source={DatabasePath}"); c.Open(); using var cmd=c.CreateCommand(); cmd.CommandText="PRAGMA foreign_keys = ON;"; cmd.ExecuteNonQuery(); return c; }
}
