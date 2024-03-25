using Deepin.Application.Queries;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace Deepin.Application.Services;
public class FileService : IFileService
{
    private const string FILE_SERVICE_URL_KEY = "FileServiceUrl";
    private readonly IDataProtector _protector;
    private readonly IFileQueries _fileQueries;
    private readonly IConfiguration _configuration;
    public FileService(IDataProtectionProvider dataProtectionProvider, IFileQueries fileQueries, IConfiguration configuration)
    {
        _protector = dataProtectionProvider.CreateProtector(nameof(FileService));
        _fileQueries = fileQueries;
        _configuration = configuration;
    }
    public string GenerateTemporaryUrl(string id, int expirationSeconds = 7200)
    {
        if (string.IsNullOrEmpty(id)) return null;
        long expirationTime = DateTimeOffset.UtcNow.AddSeconds(expirationSeconds).ToUnixTimeSeconds();
        string token = _protector.Protect($"{id}:{expirationTime}");

        string temporaryUrl = $"{_configuration[FILE_SERVICE_URL_KEY]}/api/v1/files/{id}/temporary?token={token}&expires={expirationTime}";

        return temporaryUrl;
    }
    public bool ValidateToken(string id, string token, long expirationTime)
    {
        var plainText = _protector.Unprotect(token);
        if (plainText != $"{id}:{expirationTime}")
            return false;
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (now > expirationTime)
            return false;
        return true;
    }
}
