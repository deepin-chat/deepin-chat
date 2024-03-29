﻿using Deepin.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace Deepin.Infrastructure.Repositories;

public class DocumentRepository<TDocument> : IDocumentRepository<TDocument> where TDocument : class, IDocument
{
    private IMongoCollection<TDocument> _collection;
    private readonly DocumentDbContext _dbContext;
    public DocumentRepository(DocumentDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public IMongoCollection<TDocument> Collection
    {
        get
        {
            if (_collection == null)
            {
                _collection = _dbContext.Database.GetCollection<TDocument>(typeof(TDocument).Name);
            }
            return _collection;
        }
    }
    public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        await Collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TDocument> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TDocument> InsertAsync([NotNull] TDocument document, CancellationToken cancellationToken = default)
    {
        await Collection.InsertOneAsync(document, new InsertOneOptions(), cancellationToken);
        return document;
    }

    public async Task<IEnumerable<TDocument>> InsertRangeAsync([NotNull] IEnumerable<TDocument> documents, CancellationToken cancellationToken = default)
    {
        await Collection.InsertManyAsync(documents, new InsertManyOptions(), cancellationToken);
        return documents;
    }
}
