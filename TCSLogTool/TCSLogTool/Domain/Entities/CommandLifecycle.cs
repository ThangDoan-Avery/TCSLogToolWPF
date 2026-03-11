namespace TCSLogTool.Domain.Entities;

public class CommandLifecycle
{
    public int TrId { get; set; }

    public string Device { get; set; } = "";

    public string Command { get; set; } = "";

    public DateTimeOffset Start { get; set; }

    public DateTimeOffset End { get; set; }
}