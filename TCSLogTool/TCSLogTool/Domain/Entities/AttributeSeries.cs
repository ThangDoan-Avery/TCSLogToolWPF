namespace TCSLogTool.Domain.Entities;

public class AttributeSeries
{
    public string Device { get; set; } = "";

    public string Attribute { get; set; } = "";

    public bool IsDiscrete { get; set; }

    public DeviceType DeviceType { get; set; } = DeviceType.Other;

    public List<AttributePoint> Points { get; set; } = new();
}