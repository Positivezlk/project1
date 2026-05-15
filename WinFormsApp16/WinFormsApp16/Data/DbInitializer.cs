using CertDesk.Services;
using Microsoft.Data.Sqlite;

namespace CertDesk.Data;

public static class DbInitializer
{
    public static void EnsureCreated()
    {
        Directory.CreateDirectory(Db.DataDirectory);
        using var c=Db.OpenConnection();
        foreach(var sql in Schema.Sql.Split(';',StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries)){ using var cmd=c.CreateCommand(); cmd.CommandText=sql; cmd.ExecuteNonQuery(); }
        if(Convert.ToInt32(Scalar(c,"SELECT COUNT(*) FROM users"))>0) return;
        Seed(c);
    }
    static object? Scalar(SqliteConnection c,string sql){ using var cmd=c.CreateCommand(); cmd.CommandText=sql; return cmd.ExecuteScalar(); }
    static void Exec(SqliteConnection c,string sql,params SqliteParameter[] ps){ using var cmd=c.CreateCommand(); cmd.CommandText=sql; cmd.Parameters.AddRange(ps); cmd.ExecuteNonQuery(); }
    static SqliteParameter P(string n,object? v)=>new(n,v??DBNull.Value);
    static string D(DateTime d)=>d.ToString("yyyy-MM-dd");
    static void Seed(SqliteConnection c)
    {
        Exec(c,"INSERT INTO users(login,password_hash,role) VALUES(@l,@p,@r)",P("@l","admin"),P("@p",PasswordHasher.HashPassword("admin123")),P("@r","administrator"));
        Exec(c,"INSERT INTO users(login,password_hash,role) VALUES(@l,@p,@r)",P("@l","spec"),P("@p",PasswordHasher.HashPassword("spec123")),P("@r","specialist"));
        Exec(c,"INSERT INTO users(login,password_hash,role) VALUES(@l,@p,@r)",P("@l","view"),P("@p",PasswordHasher.HashPassword("view123")),P("@r","viewer"));
        string[] names={"Иванов Сергей Петрович","Петрова Анна Викторовна","Сидоров Павел Андреевич","Смирнова Елена Игоревна","Кузнецов Дмитрий Олегович","Попова Мария Сергеевна","Васильев Андрей Николаевич","Соколова Ольга Петровна","Михайлов Кирилл Денисович","Новикова Татьяна Алексеевна","Федоров Илья Романович","Морозова Наталья Юрьевна","Волков Артем Максимович","Алексеева Ирина Павловна","Лебедев Виктор Семенович","Семенова Дарья Олеговна","Егоров Роман Ильич","Павлова Светлана Геннадьевна","Козлов Денис Евгеньевич","Степанова Ксения Валерьевна","Николаев Максим Антонович","Орлова Юлия Дмитриевна","Андреев Константин Алексеевич","Макарова Валентина Игоревна","Зайцев Леонид Павлович"};
        string[] pos={"директор","заместитель директора","главный бухгалтер","бухгалтер","специалист по кадрам","специалист по закупкам","системный администратор","преподаватель","методист"}; string[] dep={"руководство","бухгалтерия","кадры","учебный отдел","ИТ-отдел","закупки"};
        for(int i=0;i<names.Length;i++) Exec(c,"INSERT INTO employees(full_name,position,department,email,phone,snils,inn,is_active) VALUES(@n,@p,@d,@e,@ph,@s,@i,1)",P("@n",names[i]),P("@p",pos[i%pos.Length]),P("@d",dep[i%dep.Length]),P("@e",$"user{i+1}@example.local"),P("@ph",$"+7 (900) {100+i:000}-{10+i:00}-{20+i:00}"),P("@s",$"{100+i:000}-{200+i:000}-{300+i:000} {10+i:00}"),P("@i",$"7700{100000+i}"));
        string[,] auth={{"ФНС России","7707329152","UC-FNS-001","https://www.nalog.gov.ru"},{"Контур","6663003127","UC-KONTUR-002","https://kontur.ru"},{"СберКорус","7727003506","UC-SBER-003","https://sberkorus.ru"}};
        for(int i=0;i<3;i++) Exec(c,"INSERT INTO authorities(name,inn,accreditation_number,website) VALUES(@n,@i,@a,@w)",P("@n",auth[i,0]),P("@i",auth[i,1]),P("@a",auth[i,2]),P("@w",auth[i,3]));
        string[] ttypes={"Rutoken","JaCarta","eToken"}; string[] sts={"storage","issued","damaged","written_off"};
        for(int i=1;i<=18;i++){ var st=sts[i%4]; Exec(c,"INSERT INTO tokens(inventory_number,token_type,model,serial_number,status,holder_id,received_at,notes) VALUES(@inv,@ty,@m,@sn,@st,@h,@r,@n)",P("@inv",$"TK-{i:000}"),P("@ty",ttypes[i%3]),P("@m",$"Model {i%5+1}"),P("@sn",$"SN-TOKEN-{i:000}"),P("@st",st),P("@h",st=="issued"?(i%25)+1:null),P("@r",D(DateTime.Today.AddDays(-200-i))),P("@n","Демо-токен")); }
        string[] sig={"SES","NES","QES"}; string[] cstat={"active","warning","expired","revoked"};
        for(int i=1;i<=34;i++){ int m=i%4; DateTime to=m==0?DateTime.Today.AddDays(150+i):m==1?DateTime.Today.AddDays(5+i):m==2?DateTime.Today.AddDays(-10-i):DateTime.Today.AddDays(200+i); Exec(c,"INSERT INTO certificates(employee_id,authority_id,token_id,serial_number,signature_type,issued_at,valid_from,valid_to,status,purpose) VALUES(@e,@a,@t,@s,@sg,@i,@vf,@vt,@st,@p)",P("@e",(i%25)+1),P("@a",(i%3)+1),P("@t",i<=18?i:null),P("@s",$"CERT-{DateTime.Today.Year}-{i:0000}"),P("@sg",sig[i%3]),P("@i",D(to.AddYears(-1))),P("@vf",D(to.AddYears(-1).AddDays(1))),P("@vt",D(to)),P("@st",cstat[m]),P("@p","Подписание электронных документов")); }
        for(int i=1;i<=12;i++){ int m=i%4; DateTime to=m==0?DateTime.Today.AddDays(120):m==1?DateTime.Today.AddDays(20):m==2?DateTime.Today.AddDays(-5):DateTime.Today.AddDays(80); Exec(c,"INSERT INTO mchd(number,principal_employee_id,representative_employee_id,certificate_id,powers,powers_codes,valid_from,valid_to,is_registered,status,notes) VALUES(@n,@p,@r,@c,@pw,@pc,@vf,@vt,@reg,@st,@no)",P("@n",$"МЧД-{DateTime.Today.Year}-{i:000}"),P("@p",1),P("@r",(i%25)+1),P("@c",i),P("@pw","Представление интересов организации и подписание документов"),P("@pc",$"P{i:000};D{i:000}"),P("@vf",D(to.AddMonths(-6))),P("@vt",D(to)),P("@reg",i%2),P("@st",cstat[m]),P("@no","Демо МЧД")); }
        for(int i=1;i<=60;i++){ string op=i%4==0?"return":i%7==0?"damage":i%11==0?"write_off":"issue"; Exec(c,"INSERT INTO token_operations(token_id,employee_id,operation,act_number,comment,operator_user_id,created_at) VALUES(@t,@e,@o,@a,@c,1,@dt)",P("@t",(i%18)+1),P("@e",(i%25)+1),P("@o",op),P("@a",$"АКТ-{i:000}"),P("@c","Стартовая операция"),P("@dt",DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd HH:mm:ss"))); }
        for(int i=1;i<=15;i++) Exec(c,"INSERT INTO audit_log(login,action,entity_type,entity_id,description,created_at) VALUES('system',@a,'seed',@i,@d,@dt)",P("@a","начальная запись аудита"),P("@i",i),P("@d","Демо-событие"),P("@dt",DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd HH:mm:ss")));
    }
}
