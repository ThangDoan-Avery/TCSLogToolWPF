using Newtonsoft.Json;
using System.Xml;

public static class JsonExportService
{
    public static string Export(object data)
    {
        var json = JsonConvert.SerializeObject(data, (Newtonsoft.Json.Formatting)System.Xml.Formatting.Indented);

        return json;
    }
}