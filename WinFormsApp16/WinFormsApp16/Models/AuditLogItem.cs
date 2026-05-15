namespace CertDesk.Models;
public sealed class AuditLogItem { public int Id { get; set; } public string? Login { get; set; } public string Action { get; set; }=""; public string? EntityType { get; set; } public int? EntityId { get; set; } public string? Description { get; set; } public DateTime CreatedAt { get; set; } }
