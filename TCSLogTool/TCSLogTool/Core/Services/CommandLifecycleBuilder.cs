using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Services;

public class CommandLifecycleBuilder
{
    public List<CommandExecution> Build(IEnumerable<LogEntry> logs)
    {
        var result = new List<CommandExecution>();

        // TrID → pending command
        var pending = new Dictionary<int, LogEntry>();

        foreach (var log in logs)
        {
            if (log.TrId == null)
                continue;

            int trid = log.TrId.Value;

            // COMMAND START
            if (log.IsCommand)
            {
                // nếu TrID reuse → drop pending cũ
                pending[trid] = log;
                continue;
            }

            // RES → close lifecycle
            if (log.IsRes)
            {
                if (!pending.TryGetValue(trid, out var start))
                    continue;

                var exec = new CommandExecution
                {
                    Device = start.Device ?? "",
                    Command = start.Command ?? "",
                    TrId = trid,
                    Start = start.Timestamp,
                    End = log.Timestamp
                };

                result.Add(exec);

                pending.Remove(trid);
            }
        }

        return result;
    }
}