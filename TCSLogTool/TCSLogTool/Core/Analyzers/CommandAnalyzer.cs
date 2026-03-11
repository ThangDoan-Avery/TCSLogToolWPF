using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Analyzers;

public class CommandAnalyzer
{
    private readonly Dictionary<int, LogEntry> pending =
        new();

    public List<CommandExecution> Analyze(List<LogEntry> logs)
    {
        List<CommandExecution> result = new();

        foreach (var log in logs)
        {
            HandleCommandStart(log);

            HandleRes(log, result);
        }

        return result;
    }

    private void HandleCommandStart(LogEntry log)
    {
        if (!log.IsCommand)
            return;

        if (log.TrId == null)
            return;

        int trid = log.TrId.Value;

        // nếu TrID bị reuse thì drop command cũ
        if (pending.ContainsKey(trid))
            pending.Remove(trid);

        pending[trid] = log;
    }

    private void HandleRes(LogEntry log, List<CommandExecution> result)
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

        CommandExecution cmd = new()
        {
            TrId = trid,
            Device = start.Device,
            Command = start.Command,
            Start = start.Timestamp,
            End = log.Timestamp
        };

        result.Add(cmd);

        pending.Remove(trid);
    }
}