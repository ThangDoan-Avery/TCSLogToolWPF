namespace TCSLogTool.Domain.Entities;

public class AttributeSeries
{
    public string Device { get; set; } = "";

    public DeviceType DeviceType { get; set; } = DeviceType.Other;

    public string Attribute { get; set; } = "";

    public bool IsDiscrete { get; set; }

    public List<AttributePoint> Points { get; set; } = new();

    public string? Unit { get; set; }
}