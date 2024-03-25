using Deepin.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace Deepin.Infrastructure.Repositories;
public interface IDocumentRepository<TDocument> where TDocument : class, IDocument
{
    IMongoCollection<TDocument> Collection { get; }
    Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TDocument> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TDocument> InsertAsync([NotNull] TDocument document, CancellationToken cancellationToken = default);
    Task<IEnumerable<TDocument>> InsertRangeAsync([NotNull] IEnumerable<TDocument> documents, CancellationToken cancellationToken = default);
}