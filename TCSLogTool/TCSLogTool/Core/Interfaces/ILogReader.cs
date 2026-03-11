namespace TCSLogTool.Core.Interfaces;

public interface ILogReader
{
    IEnumerable<string> Read(string path);
}