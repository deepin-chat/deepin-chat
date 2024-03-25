namespace Deepin.Infrastructure.FileSystem;
public class LocalFileObject 
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string Format { get; set; }
    public long Length { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }

    public LocalFileObject(string location, FileInfo file)
    {
        this.Name = file.Name;
        this.Path = System.IO.Path.GetRelativePath(location, file.FullName);
        this.Length = file.Length;
        this.CreatedAt = file.CreationTimeUtc;
        this.ModifiedAt = file.LastWriteTimeUtc;
        this.Format = file.Extension.Replace(".", string.Empty).ToLower();
    }
}
