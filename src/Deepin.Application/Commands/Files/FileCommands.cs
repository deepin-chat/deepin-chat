using Deepin.Application.Models.Files;
using Deepin.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Deepin.Application.Commands.Files;
public class UploadFileCommand : IRequest<FileModel>
{
    public IFormFile File { get; set; }
    public UploadFileCommand(IFormFile formFile)
    {
        File = formFile;
    }
}
public class DeleteFileCommand : IRequest<Unit>
{
    public DeleteFileCommand(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
}
public class ReadFileCommand : IRequest<Stream>
{
    public ReadFileCommand(IFileObject file)
    {
        File = file;
    }
    public IFileObject File { get; set; }
}