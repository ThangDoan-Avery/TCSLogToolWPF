using TCSLogTool.Core.Interfaces;
using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Services;

public class LogAnalyzerService
{
    private readonly ILogReader reader;
    private readonly ILogParser parser;
    private readonly StateTimelineBuilder stateBuilder = new();

    private readonly CommandLifecycleBuilder commandBuilder = new();

    public LogAnalyzerService(
        ILogReader reader,
        ILogParser parser)
    {
        this.reader = reader;
        this.parser = parser;
    }

    public List<StateSegment> BuildStates(List<LogEntry> logs)
    {
        return stateBuilder.Build(logs);
    }

    public List<LogEntry> Load(string path)
    {
        var result = new List<LogEntry>();

        foreach (var line in reader.Read(path))
        {
            var entry = parser.Parse(line);

            if (entry != null)
                result.Add(entry);
        }

        return result;
    }

    public List<CommandExecution> BuildCommands(List<LogEntry> logs)
    {
        return commandBuilder.Build(logs);
    }
}