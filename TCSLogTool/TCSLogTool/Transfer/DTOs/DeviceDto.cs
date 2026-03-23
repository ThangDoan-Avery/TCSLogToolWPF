public class DeviceDto
{
    public string id { get; set; } = "";
    public string type { get; set; } = "";
    public string batch { get; set; } = "";
    public string lastState { get; set; } = "";
    public int offset { get; set; }

    public List<SegmentDto> segments { get; set; } = new();
    public List<CommandDto> commands { get; set; } = new();
    public List<AttrDto> attrs { get; set; } = new();
}