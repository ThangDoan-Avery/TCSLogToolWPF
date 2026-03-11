using System.IO;
using TCSLogTool.Core.Interfaces;

namespace TCSLogTool.Infrastructure.Readers;

public class FileLogReader : ILogReader
{
    public IEnumerable<string> Read(string path)
    {
        return File.ReadLines(path);
    }
}