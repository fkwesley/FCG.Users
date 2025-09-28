using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.UserId).HasMaxLength(20);

            builder.Property(u => u.Name).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(100);
            builder.Property(u => u.IsActive).HasDefaultValue(true);
            builder.Property(u => u.IsAdmin).HasDefaultValue(false);
            builder.Property(u => u.IsTechAccount).HasDefaultValue(false);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasConversion(
                    v => v, // Grava no banco normalmente  
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Força Kind como UTC ao ler  
                );
            builder.Property(u => u.UpdatedAt)
                .IsRequired(false)
                .HasConversion(
                    v => v, // Grava no banco normalmente  
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null // Força Kind como UTC ao ler  
                );

        }
    }
}
