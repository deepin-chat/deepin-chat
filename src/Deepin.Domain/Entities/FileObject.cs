using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Deepin.Domain.Entities;
public interface IFileObject
{
    string Name { get; }
    string Format { get; }
    string Path { get; }
    long Length { get; }
    DateTime CreatedAt { get; }
    DateTime? ModifiedAt { get; }
}

public class FileObject : IDocument, IFileObject
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
