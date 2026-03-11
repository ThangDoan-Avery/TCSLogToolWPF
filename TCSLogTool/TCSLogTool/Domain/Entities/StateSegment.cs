namespace TCSLogTool.Domain.Entities;

public class StateSegment
{
    public string Device { get; set; } = "";

    public string State { get; set; } = "";

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public TimeSpan Duration => End - Start;
}