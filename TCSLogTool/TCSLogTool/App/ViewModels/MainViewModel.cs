using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using TCSLogTool.Core.Services;
using TCSLogTool.Domain.Entities;

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

    public MainViewModel(LogAnalyzerService analyzer)
    {
        this.analyzer = analyzer;

        OpenFileCommand = new RelayCommand(OpenFiles);
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
}