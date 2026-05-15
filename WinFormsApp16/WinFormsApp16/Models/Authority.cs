namespace CertDesk.Models;
public sealed class Authority { public int Id { get; set; } public string Name { get; set; }=""; public string? Inn { get; set; } public string? AccreditationNumber { get; set; } public string? Website { get; set; } public bool IsActive { get; set; }=true; public override string ToString()=>Name; }
