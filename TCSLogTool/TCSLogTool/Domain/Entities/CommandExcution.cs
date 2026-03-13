namespace TCSLogTool.Domain.Entities;

public class CommandExecution
{
    public int TrId { get; set; }

    public string Device { get; set; } = "";

    public DeviceType DeviceType { get; set; } = DeviceType.Other;

    public string Command { get; set; } = "";

    public DateTimeOffset Start { get; set; }

    public DateTimeOffset End { get; set; }

    public TimeSpan Duration => End - Start;
}