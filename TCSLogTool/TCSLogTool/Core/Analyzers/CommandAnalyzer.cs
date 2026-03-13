using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Analyzers;

public class CommandAnalyzer
{
    public List<CommandExecution> Analyze(List<LogEntry> logs)
    {
        var result = new List<CommandExecution>();

        var pending = new Dictionary<int, LogEntry>();

        logs = logs
            .OrderBy(x => x.Timestamp)
            .ToList();

        foreach (var log in logs)
        {
            HandleCommandStart(log, pending);

            HandleRes(log, pending, result);
        }

        return result;
    }

    private void HandleCommandStart(
        LogEntry log,
        Dictionary<int, LogEntry> pending)
    {
        if (!log.IsCommand)
            return;

        if (log.TrId == null)
            return;

        int trid = log.TrId.Value;

        // TrID reuse thì drop old
        if (pending.ContainsKey(trid))
            pending.Remove(trid);

        pending[trid] = log;
    }

    private void HandleRes(
        LogEntry log,
        Dictionary<int, LogEntry> pending,
        List<CommandExecution> result)
    {
        if (!log.IsRes)
            return;

        if (log.TrId == null)
            return;

        int trid = log.TrId.Value;

        if (!pending.TryGetValue(trid, out var start))
            return;

        if (start.Device == null || start.Command == null)
            return;

        if (log.Timestamp < start.Timestamp)
            return;

        var cmd = new CommandExecution
        {
            TrId = trid,
            Device = start.Device,
            Command = start.Command,
            Start = start.Timestamp,
            End = log.Timestamp,
        };

        result.Add(cmd);

        pending.Remove(trid);
    }
}