namespace Deepin.Application.Services;

public interface IFileService
{
    string GenerateTemporaryUrl(string id, int expirationSeconds = 7200);
    bool ValidateToken(string id, string token, long expirationTime);
}