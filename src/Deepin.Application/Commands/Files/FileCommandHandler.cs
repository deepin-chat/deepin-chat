using AutoMapper;
using Deepin.Application.Models.Files;
using Deepin.Application.Services;
using Deepin.Domain.Entities;
using Deepin.Infrastructure.FileSystem;
using Deepin.Infrastructure.Repositories;
using MediatR;
using MongoDB.Bson;

namespace Deepin.Application.Commands.Files;
public class FileCommandHandler(
    IMapper mapper,
    IFileStorage fileStorage,
    IDocumentRepository<FileObject> fileRepository,
    IUserContext userContext) :
    IRequestHandler<UploadFileCommand, FileModel>,
    IRequestHandler<ReadFileCommand, Stream>,
    IRequestHandler<DeleteFileCommand, Unit>
{
    private readonly IMapper _mapper = mapper;
    private readonly IFileStorage _fileStorage = fileStorage;
    private readonly IUserContext _userContext = userContext;
    private readonly IDocumentRepository<FileObject> _fileRepository = fileRepository;
    public async Task<FileModel> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var file = _mapper.Map<FileObject>(request.File);
        file.CreatedBy = _userContext.UserId;
        await _fileStorage.CreateAsync(file, request.File.OpenReadStream());
        await _fileRepository.InsertAsync(file, cancellationToken);
        return _mapper.Map<FileModel>(file);
    }

    public async Task<Stream> Handle(ReadFileCommand request, CancellationToken cancellationToken)
    { 
        var stream = await _fileStorage.GetStreamAsync(request.File);
        return stream;
    }

    public async Task<Unit> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var file = await _fileRepository.FindByIdAsync(request.Id, cancellationToken);
        await _fileStorage.DeleteAsync(file);
        await _fileRepository.DeleteByIdAsync(request.Id);
        return Unit.Value;
    }
}
