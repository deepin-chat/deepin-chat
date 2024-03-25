using Deepin.Domain.Entities;

namespace Deepin.Application.Models.Files;
public class FileModel : IFileObject
{
    public string Id { get; set; }
    public string BlobId { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Format { get; set; }
    public long Length { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
