using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using Microsoft.Win32;
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

    [ObservableProperty]
    private LogStatistics statistics = new();

    public ICommand OpenFileCommand { get; }

    public ICommand ExportJsonCommand { get; }

    public ICommand OpenViewerCommand { get; }

    public ICommand OpenCommandDurationLogViewer { get; }

    public ICommand ExportCommandDurationJsonFile { get; }

    public MainViewModel(LogAnalyzerService analyzer)
    {
        this.analyzer = analyzer;

        OpenFileCommand = new RelayCommand(OpenFiles);

        ExportJsonCommand = new RelayCommand(ExportJson);

        OpenViewerCommand = new RelayCommand(OpenDeviceCommandLogViewer);

        OpenCommandDurationLogViewer = new RelayCommand(OpenCommandDurationViewer);

        ExportCommandDurationJsonFile = new RelayCommand(ExportCommandDurationJson);
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

        var logs = analyzer.Load(dialog.FileNames);

        logs = logs
            .OrderBy(x => x.Timestamp)
            .ToList();

        var commands = analyzer.AnalyzeCommands(logs);
        var states = analyzer.AnalyzeStates(logs);
        var attributes = analyzer.AnalyzeAttributes(logs);

        Statistics = LogStatisticsBuilder.Build(
            logs,
            dialog.FileNames.Length);

        Debug.WriteLine($"Statistics: {Statistics.FileCount}");

        Logs.Clear();
        Commands.Clear();
        States.Clear();
        AttributeSeries.Clear();
        AttributePoints.Clear();

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

        OpenDeviceCommandLogHtmlWithData(devices);
    }

    private void OpenDeviceCommandLogHtmlWithData(object devices)
    {
        var json = JsonExportService.Export(devices);

        var htmlPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Assets",
            "TCSLogTool.html"
        );

        var tempHtml = Path.Combine(Path.GetTempPath(), "tcs_viewer.html");

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

    // Command Duration Analysis Viewer
    public void OpenCommandDurationViewer()
    {
        var deviceCommands = CommandDurationMapper.Map(
            Commands.ToList());

        OpenCommandDurationHtmlWithData(deviceCommands);
    }

    private void OpenCommandDurationHtmlWithData(object deviceCommands)
    {
        var json = JsonExportService.Export(deviceCommands);

        var htmlPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Assets",
            "command-duration-analysis.html"
        );

        var tempHtml = Path.Combine(Path.GetTempPath(), "command-duration-analysis.html");

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
}