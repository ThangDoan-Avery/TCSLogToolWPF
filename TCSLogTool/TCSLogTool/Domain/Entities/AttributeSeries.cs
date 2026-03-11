namespace TCSLogTool.Domain.Entities;

public class AttributeSeries
{
    public string Device { get; set; } = "";

    public string Attribute { get; set; } = "";

    public bool IsDiscrete { get; set; }

    public List<AttributePoint> Points { get; set; } = new();
}