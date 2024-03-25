using Microsoft.AspNetCore.Identity;

namespace Deepin.Domain.Entities;

public class User : IdentityUser, IEntity
{
    public const string VerifyUserEmailTokenPurpose = "VerifyUserEmailToken";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
}