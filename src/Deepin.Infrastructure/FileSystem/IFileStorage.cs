using Deepin.Domain.Entities;

namespace Deepin.Infrastructure.FileSystem;
public interface IFileStorage
{
    Task CreateAsync(IFileObject file, Stream stream);
    Task DeleteAsync(IFileObject file);
    Task<Stream> GetStreamAsync(IFileObject file);
}
