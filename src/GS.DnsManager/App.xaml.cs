using System.Windows;

using GS.DnsManager.Features.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MudBlazor.Services;

namespace GS.DnsManager;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
internal sealed partial class App : Application
{
    public new static App Current => (App)Application.Current;

    private IHost? _host;

    public IServiceProvider ServiceProvider => _host!.Services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var builder = Host.CreateApplicationBuilder(e.Args);
        ConfigureDefaultServices(builder);
        ConfigureServices(builder);
        _host = builder.Build();
        _host.StartAsync().GetAwaiter().GetResult();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        _host?.StopAsync().GetAwaiter().GetResult();
        _host?.Dispose();
    }


    private static void ConfigureDefaultServices(HostApplicationBuilder builder)
    {
        builder.Services.AddWpfBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
    }

    private static void ConfigureServices(HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDnsService, DnsService>();
        builder.Services.AddMudServices();
    }
}
