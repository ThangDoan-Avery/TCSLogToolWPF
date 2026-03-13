public class StatusDefinition
{
    public DeviceType DeviceType { get; set; }

    public Dictionary<int, string> Labels { get; set; } = new();
}