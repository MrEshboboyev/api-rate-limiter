using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Users.Constants;

namespace Persistence.Users.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Map to the Users table
        builder.ToTable(UserTableNames.Users);

        // Configure the primary key
        builder.HasKey(x => x.Id);

        // Configure property conversions and constraints
        builder
            .Property(x => x.Email)
            .HasConversion(
                x => x.Value,
                v => Email.Create(v).Value);

        builder
            .Property(x => x.FullName)
            .HasConversion(
                x => x.Value,
                v => FullName.Create(v).Value)
            .HasMaxLength(FullName.MaxLength);

        // Configure unique constraint on Email
        builder.HasIndex(x => x.Email).IsUnique();
    }
}