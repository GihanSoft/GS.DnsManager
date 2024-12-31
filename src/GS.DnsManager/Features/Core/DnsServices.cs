using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;

using MudBlazor;

namespace GS.DnsManager.Features.Core;

internal interface IDnsService
{
    void SetDnsToDhcp();
    void SetDns(string[] dns);
}

internal sealed class DnsService : IDnsService
{
    private static NetworkInterface? GetActiveNetworkInterface()
    {
        var activeNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces().Where(a =>
                a.OperationalStatus == OperationalStatus.Up &&
                a.NetworkInterfaceType is NetworkInterfaceType.Wireless80211 or NetworkInterfaceType.Ethernet &&
                a.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily == AddressFamily.InterNetwork))
            .ToList();
        if (activeNetworkInterfaces.Count > 1)
        {
            // todo: throw or log
        }

        return activeNetworkInterfaces.FirstOrDefault();
    }

    public void SetDnsToDhcp() => SetDnsInternal(null);
    public void SetDns(string[] dns) => SetDnsInternal(dns);

    private static void SetDnsInternal(string[]? dns)
    {
        var networkInterface = GetActiveNetworkInterface();
        if (networkInterface == null)
        {
            return;
        }

        using var networkAdapterConfiguration = new ManagementClass("Win32_NetworkAdapterConfiguration");
        foreach (var instance in networkAdapterConfiguration.GetInstances().Cast<ManagementObject>())
        {
            if (!(bool)instance["IPEnabled"] ||
                !((string)instance["Description"]).Equals(networkInterface.Description, StringComparison.Ordinal))
            {
                continue;
            }

            var methodParameters = instance.GetMethodParameters("SetDNSServerSearchOrder");
            if (methodParameters == null)
            {
                continue;
            }

            methodParameters["DNSServerSearchOrder"] = dns;
            instance.InvokeMethod("SetDNSServerSearchOrder", methodParameters, null);
        }
    }
}
