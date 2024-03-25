using Deepin.Application.Models.Files;

namespace Deepin.Application.Queries;
public interface IFileQueries
{
    Task<FileModel> GetById(string id);
}