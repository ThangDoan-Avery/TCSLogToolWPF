using System.Text.RegularExpressions;
using TCSLogTool.Core.Interfaces;
using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Infrastructure.Parsers;

public class TcsLogParser : ILogParser
{
    private readonly Regex timestampRegex =
        new(@"^\d{4}-\d{2}-\d{2} .*?\+\d{2}:\d{2}");

    private readonly Regex commandRegex =
        new(@"<(\d+):([A-Za-z0-9_]+),([A-Za-z0-9_]+)");

    private readonly Regex resRegex =
        new(@"<(\d+):Res");

    private readonly Regex stateRegex =
        new(@"<-?\d+:Sts,([A-Za-z0-9_]+),([A-Za-z0-9_]+),(-?\d+)");

    public LogEntry? Parse(string line)
    {
        var tsMatch = timestampRegex.Match(line);

        if (!tsMatch.Success)
            return null;

        var timestamp = DateTime.Parse(tsMatch.Value);

        var entry = new LogEntry
        {
            Timestamp = timestamp,
            Raw = line
        };

        ParseCommand(line, entry);
        ParseRes(line, entry);
        ParseState(line, entry);

        return entry;
    }

    private void ParseCommand(string line, LogEntry entry)
    {
        var match = commandRegex.Match(line);

        if (!match.Success)
            return;

        entry.TrId = int.Parse(match.Groups[1].Value);
        entry.Command = match.Groups[2].Value;
        entry.Device = match.Groups[3].Value;
        entry.IsCommand = true;
    }

    private void ParseRes(string line, LogEntry entry)
    {
        var match = resRegex.Match(line);

        if (!match.Success)
            return;

        entry.TrId = int.Parse(match.Groups[1].Value);
        entry.IsRes = true;
    }

    private void ParseState(string line, LogEntry entry)
    {
        var match = stateRegex.Match(line);

        if (!match.Success)
            return;

        entry.Device = match.Groups[1].Value;
        entry.Attribute = match.Groups[2].Value;
        entry.Value = match.Groups[3].Value;

        if (double.TryParse(entry.Value, out var num))
            entry.NumericValue = num;

        entry.IsState = true;
    }
}