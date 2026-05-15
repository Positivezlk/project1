using System.Data; using System.Text;
namespace CertDesk.Reports;
public static class CsvExporter { public static void Export(DataTable t,string path){ using var w=new StreamWriter(path,false,new UTF8Encoding(true)); w.WriteLine(string.Join(';',t.Columns.Cast<DataColumn>().Select(c=>Esc(c.ColumnName)))); foreach(DataRow r in t.Rows) w.WriteLine(string.Join(';',t.Columns.Cast<DataColumn>().Select(c=>Esc(Convert.ToString(r[c])??"")))); } static string Esc(string s)=>'"'+s.Replace("\"","\"\"")+'"'; }
