namespace TCSLogTool.Domain.Entities;

public class StateSegment
{
    public string Device { get; set; } = "";

    public string State { get; set; } = "";

    public DateTimeOffset Start { get; set; }

    public DateTimeOffset End { get; set; }

    public TimeSpan Duration => End - Start;
}