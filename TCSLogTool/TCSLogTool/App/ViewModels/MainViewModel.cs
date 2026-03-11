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

        Logs.Clear();

        foreach (var log in logs)
            Logs.Add(log);
    }
}