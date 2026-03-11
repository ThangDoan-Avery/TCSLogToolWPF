using TCSLogTool.Domain.Entities;

namespace TCSLogTool.Core.Interfaces;

public interface ILogParser
{
    LogEntry? Parse(string line);
}