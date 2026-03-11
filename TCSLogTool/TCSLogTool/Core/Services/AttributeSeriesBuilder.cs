using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Services;

public class AttributeSeriesBuilder
{
    public List<AttributeSeries> Build(IEnumerable<LogEntry> logs)
    {
        var result = new List<AttributeSeries>();

        var attributeLogs = logs
            .Where(x =>
                x.Device != null &&
                x.Attribute != null &&
                x.NumericValue != null &&
                !x.IsState);

        var groups = attributeLogs
            .GroupBy(x => new { x.Device, x.Attribute });

        foreach (var group in groups)
        {
            var ordered = group
                .OrderBy(x => x.Timestamp)
                .ToList();

            var series = new AttributeSeries
            {
                Device = group.Key.Device!,
                Attribute = group.Key.Attribute!
            };

            foreach (var log in ordered)
            {
                series.Points.Add(new AttributePoint
                {
                    Time = log.Timestamp,
                    Value = log.NumericValue!.Value
                });
            }

            series.IsDiscrete = DetectDiscrete(series.Points);

            result.Add(series);
        }

        return result;
    }

    private bool DetectDiscrete(List<AttributePoint> points)
    {
        var unique = points
            .Select(x => x.Value)
            .Distinct()
            .Take(20)
            .ToList();

        return unique.All(x => Math.Abs(x - Math.Round(x)) < 0.0001);
    }
}