using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Infrastructure.Mappers;

public static class CommandDurationMapper
{
    public static CommandDurationRoot Map(List<CommandExecution> commands)
    {
        var result = new CommandDurationRoot
        {
            devices = commands
                .GroupBy(c => c.Device)
                .Select(deviceGroup =>
                {
                    var first = deviceGroup.First();

                    return new DeviceDurationDto
                    {
                        id = first.Device,
                        type = first.DeviceType.ToString(),

                        commands = deviceGroup
                            .OrderBy(c => c.Start)
                            .Select((c, index) => new CommandDurationDto
                            {
                                name = c.Command,
                                s = TimeHelper.ToUnixMs(c.Start),
                                e = TimeHelper.ToUnixMs(c.End),
                                duration = (long)c.Duration.TotalMilliseconds
                            })
                            .ToList()
                    };
                })
                .ToList()
        };

        return result;
    }
}