using AutoMapper;
using Deepin.Application.Models.Users;
using IdentityModel;
using System.Security.Claims;

namespace Deepin.Application.TypeConverters;
public class UserTypeConvertes :
    ITypeConverter<UserProfileRequest, IEnumerable<Claim>>,
    ITypeConverter<IEnumerable<Claim>, UserProfile>
{
    private static string BioClaimTypes => "bio";
    private static string TitleClaimTypes => "title";
    public IEnumerable<Claim> Convert(UserProfileRequest source, IEnumerable<Claim> destination, ResolutionContext context)
    {
        var claims = new List<Claim>();
        if (!string.IsNullOrEmpty(source.Bio))
        {
            claims.Add(new Claim(BioClaimTypes, source.Bio));
        }
        if (!string.IsNullOrEmpty(source.Title))
        {
            claims.Add(new Claim(TitleClaimTypes, source.Title));
        }
        if (!string.IsNullOrEmpty(source.PictureId))
        {
            claims.Add(new Claim(JwtClaimTypes.Picture, source.PictureId));
        }
        if (!string.IsNullOrEmpty(source.BirthDate))
        {
            claims.Add(new Claim(JwtClaimTypes.BirthDate, source.BirthDate));
        }
        if (!string.IsNullOrEmpty(source.FirstName))
        {
            claims.Add(new Claim(JwtClaimTypes.GivenName, source.FirstName));
        }

        if (!string.IsNullOrEmpty(source.LastName))
        {
            claims.Add(new Claim(JwtClaimTypes.FamilyName, source.LastName));
        }
        if (!string.IsNullOrEmpty(source.NickName))
        {
            claims.Add(new Claim(JwtClaimTypes.NickName, source.NickName));
        }
        if (!string.IsNullOrEmpty(source.ZoneInfo))
        {
            claims.Add(new Claim(JwtClaimTypes.ZoneInfo, source.ZoneInfo));
        }
        if (!string.IsNullOrEmpty(source.Name))
        {
            claims.Add(new Claim(JwtClaimTypes.Name, source.Name));
        }
        if (!string.IsNullOrEmpty(source.Gender))
        {
            claims.Add(new Claim(JwtClaimTypes.Gender, source.Gender));
        }
        if (!string.IsNullOrEmpty(source.Locale))
        {
            claims.Add(new Claim(JwtClaimTypes.Locale, source.Locale));
        }
        return claims;
    }
    public UserProfile Convert(IEnumerable<Claim> source, UserProfile destination, ResolutionContext context)
    {
        destination = new UserProfile
        {
            NickName = source.FirstOrDefault(s => s.Type == JwtClaimTypes.NickName)?.Value,
            FirstName = source.FirstOrDefault(s => s.Type == JwtClaimTypes.GivenName)?.Value,
            LastName = source.FirstOrDefault(s => s.Type == JwtClaimTypes.FamilyName)?.Value,
            Name = source.FirstOrDefault(s => s.Type == JwtClaimTypes.Name)?.Value,
            Picture = source.FirstOrDefault(s => s.Type == JwtClaimTypes.Picture)?.Value,
            ZoneInfo = source.FirstOrDefault(s => s.Type == JwtClaimTypes.ZoneInfo)?.Value,
            Locale = source.FirstOrDefault(s => s.Type == JwtClaimTypes.Locale)?.Value,
            BirthDate = source.FirstOrDefault(s => s.Type == JwtClaimTypes.BirthDate)?.Value,
            Gender = source.FirstOrDefault(s => s.Type == JwtClaimTypes.Gender)?.Value,
            Bio = source.FirstOrDefault(s => s.Type == BioClaimTypes)?.Value,
            Title = source.FirstOrDefault(s => s.Type == TitleClaimTypes)?.Value
        };
        return destination;
    }
}
