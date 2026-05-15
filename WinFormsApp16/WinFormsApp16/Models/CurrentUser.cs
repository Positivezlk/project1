namespace CertDesk.Models;

public sealed class CurrentUser
{
    public int Id { get; init; }
    public string Login { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public int? EmployeeId { get; init; }
    public string RoleTitle => Role switch { "administrator" => "Администратор", "specialist" => "Специалист", "viewer" => "Наблюдатель", _ => Role };
}
