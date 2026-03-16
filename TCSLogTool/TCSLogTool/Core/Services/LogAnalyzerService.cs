using TCSLogTool.Core.Analyzers;
using TCSLogTool.Core.Interfaces;
using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Services;

public class LogAnalyzerService
{
    private readonly ILogReader reader;
    private readonly ILogParser parser;

    private readonly CommandAnalyzer commandAnalyzer = new();

    private readonly StateAnalyzer stateAnalyzer = new();

    private readonly AttributeAnalyzer attributeAnalyzer = new();

    public LogAnalyzerService(ILogReader reader, ILogParser parser)
    {
        this.reader = reader;
        this.parser = parser;
    }

    public List<LogEntry> Load(IEnumerable<string> paths)
    {
        List<LogEntry> logs = new();

        foreach (var line in reader.Read(paths))
        {
            var entry = parser.Parse(line);

            if (entry != null)
                logs.Add(entry);
        }

        return logs;
    }

    public List<CommandExecution> AnalyzeCommands(List<LogEntry> logs)
    {
        return commandAnalyzer.Analyze(logs);
    }

    public List<StateSegment> AnalyzeStates(List<LogEntry> logs)
    {
        return stateAnalyzer.Analyze(logs);
    }

    public List<AttributeSeries> AnalyzeAttributes(List<LogEntry> logs)
    {
        return attributeAnalyzer.Analyze(logs);
    }
}