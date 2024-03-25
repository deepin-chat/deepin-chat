using Deepin.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Deepin.Infrastructure.EntityTypeConfigurations;
public class ChatEntityTypeConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable(Chat.TableName, DeepinDbContext.DEFAULT_SCHEMA);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();


        builder.Property(x => x.CreatedBy).HasMaxLength(64).HasColumnName("created_by");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");
        builder.Property(x => x.Type).HasColumnName("type");

        builder.OwnsOne(c => c.ChatInfo, s =>
        {
            s.ToTable("chat_info");
            s.Property(x => x.Name).HasMaxLength(32).HasColumnName("name");
            s.Property(x => x.Link).HasMaxLength(32).HasColumnName("link");
            s.Property(x => x.PictureId).HasMaxLength(64).HasColumnName("picture_id");
            s.Property(x => x.Description).HasMaxLength(256).HasColumnName("description");
            s.Property(x => x.IsPrivate).HasColumnName("is_private");
            s.Property(x => x.AdminIds).HasColumnName("admin_ids");
            s.Property(x => x.OwnerId).HasColumnName("owner_id");
            s.WithOwner().HasForeignKey("chat_id");
        });
        builder.HasMany(x => x.Members).WithOne().HasForeignKey(m => m.ChatId);
    }
}