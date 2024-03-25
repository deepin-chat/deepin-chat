using Deepin.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Deepin.Infrastructure.EntityTypeConfigurations;
public class ChatMemberEntityTypeConfiguration : IEntityTypeConfiguration<ChatMember>
{
    public void Configure(EntityTypeBuilder<ChatMember> builder)
    {
        builder.ToTable(ChatMember.TableName, DeepinDbContext.DEFAULT_SCHEMA);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.ChatId).HasColumnName("chat_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UserId).HasMaxLength(64).HasColumnName("user_id");
    }
}
