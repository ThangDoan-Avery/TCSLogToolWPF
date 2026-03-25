public class DeviceDurationDto
{
    public string id { get; set; } = "";
    public string type { get; set; } = "";
    public int offset { get; set; }

    public List<CommandDurationDto> commands { get; set; } = new();
}
