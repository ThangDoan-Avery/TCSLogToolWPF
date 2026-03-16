using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Services;

public static class LogStatisticsBuilder
{
    public static LogStatistics Build(
        List<LogEntry> logs,
        int fileCount)
    {
        LogStatistics stat = new();

        stat.FileCount = fileCount;

        stat.CommandCount =
            logs.Count(x => x.IsCommand);

        stat.DeviceCount =
            logs.Where(x => !string.IsNullOrEmpty(x.Device))
                .Select(x => x.Device)
                .Distinct()
                .Count();

        stat.StartTime =
            logs.Min(x => x.Timestamp);

        stat.EndTime =
            logs.Max(x => x.Timestamp);

        return stat;
    }
}