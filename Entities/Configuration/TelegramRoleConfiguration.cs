using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entities.Configuration
{
    public class TelegramRoleConfiguration : IEntityTypeConfiguration<TelegramRole>
    {
        public void Configure(EntityTypeBuilder<TelegramRole> builder)
        {
            builder.HasData(
            new TelegramRole
            {
                Id = Guid.NewGuid(),
                Name = "Administrator",
            },
            new TelegramRole
            {
                Id = Guid.NewGuid(),
                Name = "Member",
            }
            );
        }
    }
}
