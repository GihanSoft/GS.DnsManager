using System.Windows;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GS.DnsManager;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
internal sealed partial class App : Application
{
    public static new App Current => (App)Application.Current;

    private IHost? _host;

    public IServiceProvider ServiceProvider => _host!.Services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var builder = Host.CreateApplicationBuilder(e.Args);
        ConfigureServices(builder);
        var app = builder.Build();
        _host = app;
        Resources.Add("services", ServiceProvider);
        app.StartAsync().GetAwaiter().GetResult();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        _host?.StopAsync().GetAwaiter().GetResult();
        _host?.Dispose();
    }

    private static void ConfigureServices(HostApplicationBuilder builder)
    {
        builder.Services.AddWpfBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
    }
}
