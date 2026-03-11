using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Analyzers;

public class StateAnalyzer
{
    public List<StateSegment> Analyze(List<LogEntry> logs)
    {
        List<StateSegment> result = new();

        if (logs.Count == 0)
            return result;

        DateTimeOffset timelineStart = logs[0].Timestamp;

        Dictionary<string, StateSegment> active =
            new();

        foreach (var log in logs)
        {
            if (!log.IsState)
                continue;

            if (log.Device == null || log.Value == null)
                continue;

            string device = log.Device;

            string state = MapState(log.Value);

            if (!active.ContainsKey(device))
            {
                // tạo IDLE từ đầu timeline
                StateSegment idle = new()
                {
                    Device = device,
                    State = "IDLE",
                    Start = timelineStart,
                    End = log.Timestamp
                };

                result.Add(idle);
            }
            else
            {
                // kết thúc state trước
                var prev = active[device];

                prev.End = log.Timestamp;

                result.Add(prev);
            }

            // tạo state mới
            StateSegment current = new()
            {
                Device = device,
                State = state,
                Start = log.Timestamp
            };

            active[device] = current;
        }

        return result;
    }

    private string MapState(string value)
    {
        return value switch
        {
            "-1" => "ERROR",
            "0" => "CLOSE",
            "1" => "OPEN",
            _ => value
        };
    }
}