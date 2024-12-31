using System.Windows;

namespace GS.DnsManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
internal sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        ViewModel = new MainWindowViewModel(App.Current.ServiceProvider);
        InitializeComponent();
    }

    public MainWindowViewModel ViewModel { get; }
}

internal sealed class MainWindowViewModel
{
    public MainWindowViewModel(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public IServiceProvider ServiceProvider { get; }
}
