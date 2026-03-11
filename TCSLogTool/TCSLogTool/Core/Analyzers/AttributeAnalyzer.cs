using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Analyzers;

public class AttributeAnalyzer
{
    public List<AttributeSeries> Analyze(List<LogEntry> logs)
    {
        Dictionary<string, AttributeSeries> seriesMap = new();

        foreach (var log in logs)
        {
            if (log.Device == null)
                continue;

            if (log.Attribute == null)
                continue;

            if (log.NumericValue == null)
                continue;

            if (log.IsState)
                continue;

            string key = $"{log.Device}:{log.Attribute}";

            if (!seriesMap.TryGetValue(key, out var series))
            {
                series = new AttributeSeries
                {
                    Device = log.Device,
                    Attribute = log.Attribute,
                    IsDiscrete = DetectDiscrete(log.Attribute)
                };

                seriesMap[key] = series;
            }

            series.Points.Add(new AttributePoint
            {
                Time = log.Timestamp,
                Value = log.NumericValue.Value
            });
        }

        foreach (var s in seriesMap.Values)
        {
            s.Points = s.Points
                .OrderBy(p => p.Time)
                .ToList();
        }

        return seriesMap.Values.ToList();
    }

    private bool DetectDiscrete(string attribute)
    {
        attribute = attribute.ToLower();

        if (attribute.Contains("state"))
            return true;

        if (attribute.Contains("mode"))
            return true;

        if (attribute.Contains("status"))
            return true;

        if (attribute.Contains("enable"))
            return true;

        return false;
    }
}