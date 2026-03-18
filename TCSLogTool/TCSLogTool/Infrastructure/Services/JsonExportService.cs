using Newtonsoft.Json;
using System.Xml;

public static class JsonExportService
{
    public static string Export(object data)
    {
        var json = JsonConvert.SerializeObject(data, (Newtonsoft.Json.Formatting)System.Xml.Formatting.Indented);

        // xử lý fmt: v => v (remove quotes)
        json = json.Replace("\"v => v\"", "v => v");

        return json;
    }
}