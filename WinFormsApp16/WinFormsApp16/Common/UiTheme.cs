namespace CertDesk.Common;
public static class UiTheme
{
    public static readonly Color Primary = ColorTranslator.FromHtml("#2F5F9F"), PrimaryDark=ColorTranslator.FromHtml("#244A7C"), Light=ColorTranslator.FromHtml("#F2F4F7"), Border=ColorTranslator.FromHtml("#D7DCE2"), Text=ColorTranslator.FromHtml("#1F2933"), Secondary=ColorTranslator.FromHtml("#6B7280");
    public static void ApplyButtonStyle(Button b){ b.BackColor=Primary; b.ForeColor=Color.White; b.FlatStyle=FlatStyle.Flat; b.FlatAppearance.BorderSize=0; b.Height=34; b.Cursor=Cursors.Hand; }
    public static void ApplyDangerButtonStyle(Button b){ ApplyButtonStyle(b); b.BackColor=ColorTranslator.FromHtml("#C0392B"); }
    public static void ApplyGridStyle(DataGridView g){ g.ReadOnly=true; g.AllowUserToAddRows=false; g.AllowUserToDeleteRows=false; g.SelectionMode=DataGridViewSelectionMode.FullRowSelect; g.MultiSelect=false; g.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill; g.RowHeadersVisible=false; g.BackgroundColor=Color.White; g.BorderStyle=BorderStyle.FixedSingle; g.EnableHeadersVisualStyles=false; g.ColumnHeadersDefaultCellStyle.BackColor=Primary; g.ColumnHeadersDefaultCellStyle.ForeColor=Color.White; g.ColumnHeadersDefaultCellStyle.Font=new Font("Segoe UI",9,FontStyle.Bold); }
    public static void ApplyPanelStyle(Panel p){ p.BackColor=Color.White; p.BorderStyle=BorderStyle.FixedSingle; }
    public static string GetStatusText(string s)=>s switch {"active"=>"действует","warning"=>"истекает","expired"=>"истек","revoked"=>"отозван","archived"=>"архивный","storage"=>"на хранении","issued"=>"выдан","damaged"=>"поврежден","written_off"=>"списан", _=>s};
    public static Color GetStatusColor(string s)=>s switch {"active" or "storage"=>ColorTranslator.FromHtml("#2E7D32"),"warning" or "issued"=>ColorTranslator.FromHtml("#F39C12"),"expired" or "revoked" or "damaged"=>ColorTranslator.FromHtml("#C0392B"),"archived" or "written_off"=>ColorTranslator.FromHtml("#7F8C8D"), _=>Text};
}
