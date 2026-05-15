namespace CertDesk.Common;
public sealed class ComboBoxItem { public int? Id { get; init; } public string Text { get; init; }=""; public string? Value { get; init; } public override string ToString()=>Text; }
