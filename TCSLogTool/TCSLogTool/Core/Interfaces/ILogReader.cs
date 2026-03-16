namespace TCSLogTool.Core.Interfaces;

public interface ILogReader
{
    IEnumerable<string> Read(IEnumerable<string> paths);
}