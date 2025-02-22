using System.IO;

namespace GS.DnsManager.Shared.IO;

internal static class DirectoryHelper
{
    public static ValueTuple EnsureDirectoryExists(this DirectoryInfo directoryInfo)
    {
        if (directoryInfo.Exists || directoryInfo.Parent is null)
        {
            return default;
        }

        while (directoryInfo.Parent.Exists != true)
        {
            EnsureDirectoryExists(directoryInfo.Parent);
        }

        directoryInfo.Create();
        return default;
    }

    public static ValueTuple EnsureDirectoryExists(string directoryPath)
    {
        return EnsureDirectoryExists(new DirectoryInfo(directoryPath));
    }
}
