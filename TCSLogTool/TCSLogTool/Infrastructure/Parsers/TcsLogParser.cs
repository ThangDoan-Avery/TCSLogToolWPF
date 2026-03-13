using System.Text.RegularExpressions;
using TCSLogTool.Core.Interfaces;
using TCSLogTool.Domain.Catalogs;
using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Infrastructure.Parsers;

public class TcsLogParser : ILogParser
{
    private static readonly Regex timestampRegex =
    new(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [+-]\d{2}:\d{2})");

    private static readonly Regex commandRegex =
        new(@"<(\d+):([A-Za-z0-9_]+),([A-Za-z0-9_]+)");
    private static readonly Regex resRegex =
        new(@"<(\d+):Res");

    private static readonly Regex stateRegex =
        new(@"Sts,([A-Za-z0-9_]+),State,(-?\d+)");

    public LogEntry? Parse(string line)
    {
        var timeMatch = timestampRegex.Match(line);

        if (!timeMatch.Success)
            return null;

        if (!DateTimeOffset.TryParseExact(
        timeMatch.Groups[1].Value,
        "yyyy-MM-dd HH:mm:ss.fff zzz",
        System.Globalization.CultureInfo.InvariantCulture,
        System.Globalization.DateTimeStyles.None,
        out DateTimeOffset time))
        {
            return null;
        }

        var entry = new LogEntry
        {
            Timestamp = time,
            Raw = line
        };

        ParseCommand(line, entry);

        ParseRes(line, entry);

        ParseState(line, entry);

        ParseAttribute(line, entry);

        return entry;
    }

    private void ParseCommand(string line, LogEntry entry)
    {
        if (!line.Contains("Received from TC"))
            return;

        var m = commandRegex.Match(line);

        if (!m.Success)
            return;

        entry.TrId = int.Parse(m.Groups[1].Value);

        entry.Command = m.Groups[2].Value;

        entry.Device = m.Groups[3].Value;

        entry.DeviceType = DeviceCatalog.GetType(entry.Device);

        entry.IsCommand = true;
    }

    private void ParseRes(string line, LogEntry entry)
    {
        var m = resRegex.Match(line);

        if (!m.Success)
            return;

        entry.TrId = int.Parse(m.Groups[1].Value);

        entry.IsRes = true;
    }

    private void ParseState(string line, LogEntry entry)
    {
        var m = stateRegex.Match(line);

        if (!m.Success)
            return;

        entry.Device = m.Groups[1].Value;

        entry.Attribute = "State";

        entry.Value = m.Groups[2].Value;

        entry.DeviceType = DeviceCatalog.GetType(entry.Device);

        entry.AttributeType = AttributeCatalog.GetType(entry.Attribute);

        if (double.TryParse(entry.Value, out double v))
        {
            entry.NumericValue = v;
        }

        entry.IsState = true;
    }

    private void ParseAttribute(string line, LogEntry entry)
    {
        if (entry.IsState || entry.IsCommand || entry.IsRes)
            return;

        if (entry.IsState)
            return;

        if (!line.Contains("Sts,"))
            return;

        var parts = line.Split(',');

        if (parts.Length < 4)
            return;

        string device = parts[1];
        string attr = parts[2];
        string valuePart = parts[3];

        int idx = valuePart.IndexOf('>');
        if (idx >= 0)
            valuePart = valuePart.Substring(0, idx);

        entry.Device = device;
        entry.Attribute = attr;
        entry.Value = valuePart;
        entry.AttributeType = AttributeCatalog.GetType(entry.Attribute);
        entry.DeviceType = DeviceCatalog.GetType(entry.Device);

        if (double.TryParse(valuePart, out double v))
            entry.NumericValue = v;
    }
}