using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Services;

public class StateTimelineBuilder
{
    public List<StateSegment> Build(List<LogEntry> logs)
    {
        var result = new List<StateSegment>();

        if (logs.Count == 0)
            return result;

        DateTimeOffset timelineStart = logs.First().Timestamp;

        // group theo device
        var deviceStates = logs
            .Where(x => x.IsState && x.Device != null)
            .GroupBy(x => x.Device);

        foreach (var deviceGroup in deviceStates)
        {
            var device = deviceGroup.Key!;

            var states = deviceGroup
                .OrderBy(x => x.Timestamp)
                .ToList();

            DateTimeOffset currentStart = timelineStart;
            string currentState = "IDLE";

            foreach (var s in states)
            {
                // tạo segment trước khi state đổi
                var segment = new StateSegment
                {
                    Device = device,
                    State = currentState,
                    Start = currentStart,
                    End = s.Timestamp
                };

                result.Add(segment);

                // update state
                currentStart = s.Timestamp;
                currentState = s.Value ?? "UNKNOWN";
            }

            // state cuối giữ tới cuối log
            var lastSegment = new StateSegment
            {
                Device = device,
                State = currentState,
                Start = currentStart,
                End = states.Last().Timestamp
            };

            result.Add(lastSegment);
        }

        return result;
    }
}