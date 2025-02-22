using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace GS.DnsManager.Features.Core;

internal interface IDnsManager
{
    void SetDnsToDhcp();
    void SetDns(DnsServer dns);
}

internal sealed class DnsManager : IDnsManager
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
    public void SetDns(DnsServer dns) => SetDnsInternal(dns);

    private static void SetDnsInternal(DnsServer? server)
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

            methodParameters["DNSServerSearchOrder"] = server?.ServerIpList.ToArray();
            instance.InvokeMethod("SetDNSServerSearchOrder", methodParameters, null);
        }
    }
}
