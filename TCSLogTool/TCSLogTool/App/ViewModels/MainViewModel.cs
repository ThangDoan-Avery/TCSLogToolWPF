using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TCSLogTool.Core.Services;
using TCSLogTool.Domain.Entities;

namespace TCSLogTool.App.ViewModels;

public class MainViewModel
{
    private readonly LogAnalyzerService analyzer;

    public ObservableCollection<LogEntry> Logs { get; } = new();

    public ObservableCollection<CommandExecution> Commands { get; } = new();

    public ObservableCollection<StateSegment> States { get; } = new();

    public ObservableCollection<AttributeSeries> AttributeSeries { get; } = new();

    public ObservableCollection<AttributePoint> AttributePoints { get; } = new();

    public ICommand OpenFileCommand { get; }

    public MainViewModel(LogAnalyzerService analyzer)
    {
        this.analyzer = analyzer;

        OpenFileCommand = new RelayCommand(OpenFile);
    }

    private void OpenFile()
    {
        var dialog = new OpenFileDialog();

        if (dialog.ShowDialog() != true)
            return;

        var logs = analyzer.Load(dialog.FileName);
        var commands = analyzer.AnalyzeCommands(logs);
        var states = analyzer.AnalyzeStates(logs);
        var attributes = analyzer.AnalyzeAttributes(logs);

        Logs.Clear();

        foreach (var log in logs)
            Logs.Add(log);

        Commands.Clear();
        States.Clear();
        AttributeSeries.Clear();

        foreach (var command in commands)
            Commands.Add(command);

        foreach (var state in states)
            States.Add(state);

        foreach (var attribute in attributes)
            AttributeSeries.Add(attribute);

        foreach (var series in attributes)
        {
            foreach (var point in series.Points)
            {
                AttributePoints.Add(point);
            }
        }

    }
}