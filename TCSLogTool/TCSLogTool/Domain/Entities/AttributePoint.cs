namespace TCSLogTool.Domain.Entities;

public class AttributePoint
{
    public string Device { get; set; } = "";

    public string Attribute { get; set; } = "";

    public DateTime Time { get; set; }

    public double Value { get; set; }
}