namespace TCSLogTool.Domain.Entities;

public class LogEntry
{
    public DateTimeOffset Timestamp { get; set; }

    public string Raw { get; set; } = "";

    public int? TrId { get; set; }

    public string? Device { get; set; }

    public string? Command { get; set; }

    public string? Attribute { get; set; }

    public string? Value { get; set; }

    public double? NumericValue { get; set; }

    public bool IsCommand { get; set; }

    public bool IsRes { get; set; }

    public bool IsState { get; set; }
}