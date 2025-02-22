using System.IO;
using System.Text.Json;

using GS.DnsManager.Shared.IO;

namespace GS.DnsManager.Features.Core;

internal interface IDnsServersRepository
{
    public ValueTask<IReadOnlyList<DnsServer>> GetAll();
    public ValueTask SaveAsync(IEnumerable<DnsServer> servers);
}

internal sealed class JsonFileDnsServersRepository : IDnsServersRepository
{
    private readonly string _dataPath =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GihanSoft",
            "DnsManager",
            "dnsServers.json");

    private IReadOnlyList<DnsServer>? _cached;

    public async ValueTask<IReadOnlyList<DnsServer>> GetAll()
    {
        if (_cached is not null)
        {
            return _cached;
        }

        if (!File.Exists(_dataPath))
        {
            return DnsServer.DefaultDnsServers;
        }

        var dnsServersFile = File.OpenRead(_dataPath);
        await using var dnsServersFileDisposable = dnsServersFile.ConfigureAwait(false);
        var options = await JsonSerializer.DeserializeAsync<List<DnsServer>>(dnsServersFile)
            .ConfigureAwait(false);
        _cached = (options ?? []).AsReadOnly();
        return _cached;
    }

    public async ValueTask SaveAsync(IEnumerable<DnsServer> servers)
    {
        var dataDir = Directory.GetParent(_dataPath);
        if (dataDir?.Exists ?? false)
        {
            dataDir.EnsureDirectoryExists();
        }

        var dnsServersFile = File.Create(_dataPath);
        await using var dnsServersFileDisposable = dnsServersFile.ConfigureAwait(false);
        _cached = servers.ToList().AsReadOnly();
        await JsonSerializer.SerializeAsync(dnsServersFile, _cached).ConfigureAwait(false);
    }
}
