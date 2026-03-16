namespace TCSLogTool.Domain.Entities;

public class LogStatistics
{
    public int FileCount { get; set; }

    public int CommandCount { get; set; }

    public int DeviceCount { get; set; }

    public DateTimeOffset? StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }
}