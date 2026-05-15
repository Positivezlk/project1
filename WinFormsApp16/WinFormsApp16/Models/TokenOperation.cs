namespace CertDesk.Models;
public sealed class TokenOperation { public int Id { get; set; } public int TokenId { get; set; } public int? EmployeeId { get; set; } public string Operation { get; set; }=""; public string? ActNumber { get; set; } public string? Comment { get; set; } public int? OperatorUserId { get; set; } public DateTime CreatedAt { get; set; } }
