using TCSLogTool.Domain.Entities;

public static class DeviceMapper
{
    public static List<DeviceDto> Map(
        List<StateSegment> stateSegments,
        List<CommandExecution> commands,
        List<AttributeSeries> attrs)
    {
        var devices = stateSegments
            .GroupBy(x => x.Device);

        var result = new List<DeviceDto>();

        foreach (var deviceGroup in devices)
        {
            var device = deviceGroup.Key;

            var deviceStates = deviceGroup.ToList();
            var deviceCommands = commands.Where(x => x.Device == device).ToList();
            var deviceAttrs = attrs.Where(x => x.Device == device).ToList();

            var dto = new DeviceDto
            {
                id = device,
                type = deviceStates.First().DeviceType.ToString(),
                batch = deviceStates.First().DeviceType.ToString(),
                lastState = deviceStates.Last().State
            };

            // SEGMENTS
            dto.segments = deviceStates.Select(s => new SegmentDto
            {
                s = TimeHelper.ToUnixMs(s.Start),
                e = TimeHelper.ToUnixMs(s.End),
                st = s.State
            }).ToList();

            // COMMANDS
            dto.commands = deviceCommands.Select(c => new CommandDto
            {
                name = c.Command,
                s = TimeHelper.ToUnixMs(c.Start),
                e = TimeHelper.ToUnixMs(c.End),
                lane = 0,
                note = ""
            }).ToList();

            // ATTRS
            dto.attrs = deviceAttrs.Select(a =>
            {
                var series = a.Points.Select(p => new AttrPointDto
                {
                    t = TimeHelper.ToUnixMs(p.Time),
                    v = p.Value,
                    label = a.IsDiscrete ? p.Label : null
                }).ToList();

                return new AttrDto
                {
                    key = $"{a.Device}_{a.Attribute}",
                    label = a.Attribute,
                    unit = a.Unit ?? "",
                    color = ColorHelper.RandomColor(),
                    series = series,
                    isDiscrete = a.IsDiscrete,
                    min = series.Count > 0 ? series.Min(x => x.v) : 0,
                    max = series.Count > 0 ? series.Max(x => x.v) : 0,
                };
            }).ToList();

            result.Add(dto);
        }

        return result;
    }
}