using Entities.Configuration;
using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class DataBaseContext : IdentityDbContext<User>
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options)
        {
        }

        public DbSet<TelegramUser> TelegramUsers { get; set; }

        public DbSet<TelegramRole> TelegramRoles { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Topic> Topics { get; set; }

        public DbSet<TelegramUserTopic> TelegramUserTopics { get; set; }

        public DbSet<TelegramUserGroup> TelegramUserGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>().HasKey(sc => sc.Id);

            modelBuilder.Entity<Group>().HasAlternateKey(sc => sc.Name);


            modelBuilder.Entity<Group>()
                .Property(b => b.InviteCode)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Group>()
                .Property(b => b.Id)
                .HasColumnName("GroupId");

            modelBuilder.Entity<Group>()
                .Property(b => b.Name)
                .HasMaxLength(30)
                .IsRequired();



            modelBuilder.Entity<Topic>().HasKey(sc => sc.Id);

            modelBuilder.Entity<Topic>()
                .Property(b => b.Id)
                .HasColumnName("TopicId");

            modelBuilder.Entity<Topic>()
               .Property(b => b.Name)
               .HasMaxLength(60)
               .IsRequired();

            modelBuilder.Entity<Topic>()
                .HasOne<Group>(s => s.Group)
                .WithMany(g => g.Topics)
                .HasForeignKey(s => s.GroupId);



            modelBuilder.Entity<TelegramUserTopic>().HasKey(sc => new { sc.TelegramUserId, sc.TopicId });

            modelBuilder.Entity<TelegramUserTopic>()
               .Property(b => b.Date)
               .IsRequired();

            modelBuilder.Entity<TelegramUserTopic>()
              .Property(b => b.IsConfirm)
              .IsRequired();

            modelBuilder.Entity<TelegramUserTopic>()
                .HasOne<TelegramUser>(sc => sc.TelegramUser)
                .WithMany(s => s.TelegramUserTopics)
                .HasForeignKey(sc => sc.TelegramUserId);

            modelBuilder.Entity<TelegramUserTopic>()
                .HasOne<Topic>(sc => sc.Topic)
                .WithMany(s => s.TelegramUserTopics)
                .HasForeignKey(sc => sc.TopicId);



            modelBuilder.Entity<TelegramUserGroup>().HasKey(sc => new { sc.TelegramUserId, sc.GroupId, sc.TelegramRoleId });

            modelBuilder.Entity<TelegramUserGroup>()
               .Property(b => b.UserName)
               .HasMaxLength(40);

            modelBuilder.Entity<TelegramUserGroup>()
                .HasOne<TelegramUser>(sc => sc.TelegramUser)
                .WithMany(s => s.TelegramUserGroups)
                .HasForeignKey(sc => sc.TelegramUserId);

            modelBuilder.Entity<TelegramUserGroup>()
                .HasOne<TelegramRole>(sc => sc.TelegramRole)
                .WithMany(s => s.TelegramUserGroups)
                .HasForeignKey(sc => sc.TelegramRoleId);

            modelBuilder.Entity<TelegramUserGroup>()
               .HasOne<Group>(sc => sc.Group)
               .WithMany(s => s.TelegramUserGroups)
               .HasForeignKey(sc => sc.GroupId);



            modelBuilder.Entity<TelegramUser>().HasKey(sc => new { sc.TelegramUserId });

            modelBuilder.Entity<TelegramUser>()
              .Property(b => b.TelegramUserId)
              .HasColumnName("TelegramUserId")
              .ValueGeneratedNever();


            modelBuilder.Entity<TelegramRole>().HasKey(sc => new { sc.Id });

            modelBuilder.Entity<TelegramRole>()
             .Property(b => b.Id)
             .HasColumnName("TelegramRoleId");

            modelBuilder.Entity<TelegramRole>()
              .Property(b => b.Name)
              .HasMaxLength(60)
              .IsRequired();


            modelBuilder.ApplyConfiguration(new TelegramRoleConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
