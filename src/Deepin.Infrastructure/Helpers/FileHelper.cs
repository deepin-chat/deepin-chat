using System.Security.Cryptography;

namespace Deepin.Infrastructure.Helpers;
public static class FileHelper
{
    public static async Task<string> CalculateChecksum(Stream stream)
    {
        return await CalculateChecksum(stream, SHA256.Create());
    }
    public static async Task<string> CalculateChecksum(Stream stream, HashAlgorithm algorithm)
    {
        var checkSumBytes = await algorithm.ComputeHashAsync(stream);
        return BitConverter.ToString(checkSumBytes);
    }
    public static string ConvertToCommonPath(string path)
    {
        return path.Replace("\\", "/");
    }
}