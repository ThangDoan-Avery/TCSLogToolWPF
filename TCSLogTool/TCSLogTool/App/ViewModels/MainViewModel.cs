using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;
using System.Windows.Input;
using TCSLogTool.Core.Services;
using TCSLogTool.Domain.Entities;
using TCSLogTool.Infrastructure.Mappers;

namespace TCSLogTool.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly LogAnalyzerService analyzer;

    public ObservableCollection<LogEntry> Logs { get; } = new();
    public ObservableCollection<CommandExecution> Commands { get; } = new();
    public ObservableCollection<StateSegment> States { get; } = new();
    public ObservableCollection<AttributeSeries> AttributeSeries { get; } = new();
    public ObservableCollection<AttributePoint> AttributePoints { get; } = new();

    private List<LogEntry> _allLogs = new();
    private int _fileCount = 0;

    [ObservableProperty]
    private DateTime? filterStart;

    [ObservableProperty]
    private DateTime? filterEnd;

    [ObservableProperty]
    private LogStatistics statistics = new();

    public ICommand OpenFileCommand { get; }
    public ICommand ExportJsonCommand { get; }
    public ICommand OpenViewerCommand { get; }
    public ICommand OpenCommandDurationLogViewer { get; }
    public ICommand ExportCommandDurationJsonFile { get; }

    public ICommand ApplyFilterCommand { get; }

    public MainViewModel(LogAnalyzerService analyzer)
    {
        this.analyzer = analyzer;

        OpenFileCommand = new RelayCommand(OpenFiles);
        ExportJsonCommand = new RelayCommand(ExportJson);
        OpenViewerCommand = new RelayCommand(OpenDeviceCommandLogViewer);
        OpenCommandDurationLogViewer = new RelayCommand(OpenCommandDurationViewer);
        ExportCommandDurationJsonFile = new RelayCommand(ExportCommandDurationJson);

        ApplyFilterCommand = new RelayCommand(
            ReFilter,
            () => IsFilterValid()
        );
    }

    private bool IsFilterValid()
    {
        if (!_allLogs.Any()) return false;

        if (FilterStart.HasValue && FilterEnd.HasValue && FilterStart > FilterEnd)
            return false;

        return true;
    }

    private void OpenFiles()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Log files (*.log;*.txt)|*.log;*.txt",
            Multiselect = true
        };

        if (dialog.ShowDialog() != true)
            return;

        _fileCount = dialog.FileNames.Length;

        _allLogs = analyzer.Load(dialog.FileNames)
            .OrderBy(x => x.Timestamp)
            .ToList();

        if (_allLogs.Any())
        {
            FilterStart = _allLogs.First().Timestamp.DateTime;
            FilterEnd = _allLogs.Last().Timestamp.DateTime;
        }
        else
        {
            FilterStart = null;
            FilterEnd = null;
        }

        LoadFullLogs();
    }

    private void LoadFullLogs()
    {
        if (_allLogs == null || !_allLogs.Any())
        {
            ClearAll();
            Statistics = new LogStatistics();
            return;
        }

        var commands = analyzer.AnalyzeCommands(_allLogs);
        var states = analyzer.AnalyzeStates(_allLogs);
        var attributes = analyzer.AnalyzeAttributes(_allLogs);

        Statistics = LogStatisticsBuilder.Build(_allLogs, _fileCount);

        UpdateCollections(_allLogs, commands, states, attributes);
    }

    private void ReFilter()
    {
        if (_allLogs == null || !_allLogs.Any())
            return;

        if (FilterStart.HasValue && FilterEnd.HasValue && FilterStart > FilterEnd)
            return;

        var start = FilterStart.HasValue
            ? new DateTimeOffset(FilterStart.Value, _allLogs.First().Timestamp.Offset)
            : (DateTimeOffset?)null;

        var end = FilterEnd.HasValue
            ? new DateTimeOffset(FilterEnd.Value, _allLogs.First().Timestamp.Offset)
            : (DateTimeOffset?)null;

        var logs = _allLogs
            .Where(x =>
                (!start.HasValue || x.Timestamp >= start.Value) &&
                (!end.HasValue || x.Timestamp <= end.Value))
            .OrderBy(x => x.Timestamp)
            .ToList();

        if (!logs.Any())
        {
            ClearAll();
            Statistics = new LogStatistics();
            return;
        }

        var commands = analyzer.AnalyzeCommands(logs);
        var states = analyzer.AnalyzeStates(logs);
        var attributes = analyzer.AnalyzeAttributes(logs);

        Statistics = LogStatisticsBuilder.Build(logs, _fileCount);

        UpdateCollections(logs, commands, states, attributes);
    }

    private void ClearAll()
    {
        Logs.Clear();
        Commands.Clear();
        States.Clear();
        AttributeSeries.Clear();
        AttributePoints.Clear();
    }

    private void UpdateCollections(
        List<LogEntry> logs,
        List<CommandExecution> commands,
        List<StateSegment> states,
        List<AttributeSeries> attributes)
    {
        ClearAll();

        foreach (var log in logs)
            Logs.Add(log);

        foreach (var command in commands)
            Commands.Add(command);

        foreach (var state in states)
            States.Add(state);

        foreach (var series in attributes)
        {
            AttributeSeries.Add(series);

            foreach (var point in series.Points)
                AttributePoints.Add(point);
        }
    }

    public void ExportJson()
    {
        var devices = DeviceMapper.Map(
            States.ToList(),
            Commands.ToList(),
            AttributeSeries.ToList());

        var json = JsonExportService.Export(devices);

        var dialog = new SaveFileDialog
        {
            Filter = "JSON file (*.json)|*.json",
            FileName = "DeviceLog.json"
        };

        if (dialog.ShowDialog() != true)
            return;

        File.WriteAllText(dialog.FileName, json);
    }

    public void OpenDeviceCommandLogViewer()
    {
        var devices = DeviceMapper.Map(
            States.ToList(),
            Commands.ToList(),
            AttributeSeries.ToList());

        OpenHtml("TCSLogTool.html", devices, "tcs_viewer.html");
    }

    public void OpenCommandDurationViewer()
    {
        var deviceCommands = CommandDurationMapper.Map(
            Commands.ToList());

        OpenHtml("command-duration-analysis.html", deviceCommands, "command-duration-analysis.html");
    }

    private void OpenHtml(string assetFile, object data, string tempFile)
    {
        var json = JsonExportService.Export(data);

        var htmlPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Assets",
            assetFile
        );

        var tempHtml = Path.Combine(Path.GetTempPath(), tempFile);

        var html = File.ReadAllText(htmlPath);

        var inject = $@"
        <script>
        window.addEventListener('load', function() {{
            const data = {json};
            window.setDevicesFromWpf(data);
        }});
        </script>
        </body>";

        html = html.Replace("</body>", inject);

        File.WriteAllText(tempHtml, html);

        Process.Start(new ProcessStartInfo
        {
            FileName = tempHtml,
            UseShellExecute = true
        });
    }

    public void ExportCommandDurationJson()
    {
        var deviceCommands = CommandDurationMapper.Map(
            Commands.ToList());

        var json = JsonExportService.Export(deviceCommands);

        var dialog = new SaveFileDialog
        {
            Filter = "JSON file (*.json)|*.json",
            FileName = "commandDuration.json"
        };

        if (dialog.ShowDialog() != true)
            return;

        File.WriteAllText(dialog.FileName, json);
    }

    partial void OnFilterStartChanged(DateTime? value)
    {
        (ApplyFilterCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }

    partial void OnFilterEndChanged(DateTime? value)
    {
        (ApplyFilterCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }
}