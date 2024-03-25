using AutoMapper;
using Deepin.Application.Models.Files;
using Deepin.Domain.Entities;
using HeyRed.Mime;
using Microsoft.AspNetCore.Http;

namespace Deepin.Application.MappingProfiles;
public class FileMappingProfile : Profile
{
    public FileMappingProfile()
    {
        CreateMap<FileObject, FileModel>(MemberList.Destination);
        CreateMap<IFormFile, FileObject>(MemberList.Destination)
            .ForMember(d => d.Name, o => o.MapFrom(s => s.FileName))
            .ForMember(d => d.Path, o => o.MapFrom(s => Path.Join(DateTime.UtcNow.ToString("yyyy-MM-dd"), s.FileName)))
            .ForMember(d => d.Format, o => o.MapFrom(s => MimeTypesMap.GetExtension(s.ContentType)))
            .ForMember(d => d.Length, o => o.MapFrom(s => s.Length))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => DateTime.UtcNow));
    }
}
