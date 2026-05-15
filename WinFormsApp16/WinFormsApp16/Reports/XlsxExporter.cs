using System.Data; using ClosedXML.Excel;
namespace CertDesk.Reports;
public static class XlsxExporter { public static void Export(DataTable t,string path,string title){ using var wb=new XLWorkbook(); var ws=wb.Worksheets.Add("Отчет"); ws.Cell(1,1).Value=title; ws.Cell(1,1).Style.Font.Bold=true; ws.Cell(2,1).Value="Дата формирования: "+DateTime.Now.ToString("dd.MM.yyyy HH:mm"); ws.Cell(4,1).InsertTable(t,true); ws.Row(4).Style.Font.Bold=true; ws.Columns().AdjustToContents(); wb.SaveAs(path); } }
