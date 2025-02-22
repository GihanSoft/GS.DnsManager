namespace GS.DnsManager.Features.Core;

internal sealed record DnsServer(
    string Title,
    IReadOnlyList<string> ServerIpList
)
{
    public static IReadOnlyList<DnsServer> DefaultDnsServers => [
        new("Shekan", ["178.22.122.100", "185.51.200.2"]),
        new("403.online", ["10.202.10.202", "10.202.10.102"]),
        new("radar.game", ["10.202.10.10", "10.202.10.11"]),

        new("Google", ["8.8.8.8", "8.8.4.4"]),
        new("1.1.1.1", ["1.1.1.1", "1.0.0.1"]),

        new("1.1.1.1 + block malware for families", ["1.1.1.2", "1.0.0.2"]),
        new("1.1.1.1 + block malware and adult content for families", ["1.1.1.3", "1.0.0.3"]),
    ];
}
