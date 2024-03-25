using AutoMapper;
using Deepin.Application.Constants;
using Deepin.Application.Models.Files;
using Deepin.Domain.Entities;
using Deepin.Infrastructure.Caching;
using Deepin.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;

namespace Deepin.Application.Queries;
public class FileQueries(
    IMapper mapper,
    ICacheManager cacheManager,
    IDocumentRepository<FileObject> fileRepository) : IFileQueries
{
    private readonly IMapper _mapper = mapper;
    private readonly ICacheManager _cacheManager = cacheManager;
    private readonly IDocumentRepository<FileObject> _fileRepository = fileRepository;
    public async Task<FileModel> GetById(string id)
    {
        return await _cacheManager.GetOrCreateAsync(CacheKeys.GetFileByIdCacheKey(id), async () =>
        {
            var file = await _fileRepository.FindByIdAsync(id);
            if (file == null) return null;
            return _mapper.Map<FileModel>(file);
        });
    }
}