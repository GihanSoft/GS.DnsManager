using System.Windows;

using GS.DnsManager.Features.Core;

using Microsoft.Extensions.DependencyInjection;

using MudBlazor.Services;

namespace GS.DnsManager;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
internal sealed partial class App : Application
{
    public new static App Current => (App)Application.Current;

    private ServiceProvider? _serviceProvider;
    public IServiceProvider ServiceProvider => _serviceProvider ?? new EmptyServiceProvider() as IServiceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ServiceCollection services = new();
        ConfigureDefaultServices(services);
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private static void ConfigureDefaultServices(IServiceCollection services)
    {
        services.AddWpfBlazorWebView();
#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IDnsService, DnsService>();
        services.AddMudServices();
    }
}

internal sealed class EmptyServiceProvider : IServiceProvider
{
    public object? GetService(Type serviceType) => null;
}
