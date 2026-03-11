using Microsoft.Extensions.DependencyInjection;
using TCSLogTool.Core.Interfaces;
using TCSLogTool.Core.Services;
using TCSLogTool.Infrastructure.Parsers;
using TCSLogTool.Infrastructure.Readers;
using TCSLogTool.App.ViewModels;
using TCSLogTool.App.Views;

namespace TCSLogTool.App.DI;

public static class ServiceConfigurator
{
    public static ServiceProvider Configure()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ILogReader, FileLogReader>();
        services.AddSingleton<ILogParser, TcsLogParser>();

        services.AddSingleton<LogAnalyzerService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        return services.BuildServiceProvider();
    }
}