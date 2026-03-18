public class AttrDto
{
    public string key { get; set; } = "";
    public string label { get; set; } = "";
    public string unit { get; set; } = "";
    public string color { get; set; } = "";
    public bool isDiscrete { get; set; }

    public List<AttrPointDto> series { get; set; } = new();

    public double min { get; set; }
    public double max { get; set; }
}