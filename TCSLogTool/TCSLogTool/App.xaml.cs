using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TCSLogTool.App.DI;
using TCSLogTool.App.Views;

namespace TCSLogTool.ToolApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var provider = ServiceConfigurator.Configure();

        var window = provider.GetRequiredService<MainWindow>();

        window.Show();
    }
}