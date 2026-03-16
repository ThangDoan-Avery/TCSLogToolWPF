using System.IO;
using System.Text.RegularExpressions;
using TCSLogTool.Core.Interfaces;

namespace TCSLogTool.Infrastructure.Readers;

public class FileLogReader : ILogReader
{
    private static readonly Regex suffixRegex =
        new(@"_(\d+)\.(log|txt)$", RegexOptions.IgnoreCase);

    public IEnumerable<string> Read(IEnumerable<string> paths)
    {
        var sortedFiles = paths
            .OrderBy(GetSuffixNumber)
            .ToList();

        foreach (var file in sortedFiles)
        {
            foreach (var line in File.ReadLines(file))
            {
                yield return line;
            }
        }
    }

    private int GetSuffixNumber(string path)
    {
        var name = Path.GetFileName(path);

        var match = suffixRegex.Match(name);

        if (!match.Success)
            return 0;

        return int.Parse(match.Groups[1].Value);
    }
}