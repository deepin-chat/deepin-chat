namespace Deepin.Infrastructure.Caching;

public class RedisCacheOptions
{
    public int DefaultCacheTimeMinutes { get; set; } = 30;
    public static string ProviderKey => "Redis";
    public string ConnectionString { get; set; }
    public int DefaultDatabase { get; set; }
}