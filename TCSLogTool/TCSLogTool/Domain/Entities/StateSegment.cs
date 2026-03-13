using System.Security.AccessControl;

namespace TCSLogTool.Domain.Entities;

public class StateSegment
{
    public string Device { get; set; } = "";

    public DeviceType DeviceType { get; set; } = DeviceType.Other;

    public string State { get; set; } = "";

    public DateTimeOffset Start { get; set; }

    public DateTimeOffset End { get; set; }

    public TimeSpan Duration => End - Start;
}