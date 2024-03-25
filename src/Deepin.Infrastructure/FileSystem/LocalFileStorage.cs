using Deepin.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Deepin.Infrastructure.FileSystem;
public class LocalFileStorage(IConfiguration configuration) : IFileStorage
{
    private readonly string _dataFolder = configuration["DataDir"];

    private string GetFullPath(string relativePath)
    {
        return Path.Combine(_dataFolder, relativePath);
    }
    public async Task CreateAsync(IFileObject file, Stream stream)
    {
        var fullPath = GetFullPath(file.Path);
        var dir = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        var fileInfo = new FileInfo(fullPath);
        using var fs = fileInfo.Create();
        await stream.CopyToAsync(fs);
        await fs.FlushAsync();
    }

    public async Task DeleteAsync(IFileObject file)
    {
        var fullPath = GetFullPath(file.Path);
        var fileInfo = new FileInfo(fullPath);
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }
        await Task.FromResult(0);
    }

    //public async Task<IFileObject> GetAsync(string path)
    //{
    //    var fullPath = GetFullPath(path);
    //    var fileInfo = new FileInfo(fullPath);
    //    if (fileInfo.Exists)
    //    {
    //        return new LocalFileObject(_dataFolder, fileInfo);
    //    }
    //    return await Task.FromResult(default(IFileObject));
    //}

    public async Task<Stream> GetStreamAsync(IFileObject file)
    {
        var fullPath = GetFullPath(file.Path);
        var fileInfo = new FileInfo(fullPath);
        if (!fileInfo.Exists)
            return null;
        return await Task.FromResult(fileInfo.OpenRead());
    }
}
