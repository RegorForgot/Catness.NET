﻿using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Catness.Persistence;

public class CatnessDbContext : DbContext
{
    public virtual DbSet<BotPersistence> BotPersistence { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Guild> Guilds { get; set; }

    public virtual DbSet<GuildUser> GuildUsers { get; set; }
    public virtual DbSet<GuildChannel> GuildChannels { get; set; }

    public virtual DbSet<Follow> Follows { get; set; }
    public virtual DbSet<Reminder> Reminders { get; set; }

    public CatnessDbContext() { }

    public CatnessDbContext(DbContextOptions<CatnessDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // for migrations
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=admin;Database=catnessdb;Command Timeout=60;Timeout=60;Persist Security Info=True");
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BotPersistence>(persistence =>
            {
                persistence.HasKey(p => p.Key);
            }
        );

        modelBuilder.Entity<User>(user =>
            {
                user.HasKey(userEntity => userEntity.UserId);
            }
        );

        modelBuilder.Entity<Follow>(follow =>
            {
                follow.HasKey(f => new
                {
                    f.FollowerId,
                    f.FollowedId
                });

                follow.HasOne(f => f.Follower)
                    .WithMany(f => f.Following)
                    .HasForeignKey(f => f.FollowerId)
                    .OnDelete(DeleteBehavior.Cascade);

                follow.HasOne(f => f.Followed)
                    .WithMany(f => f.Followers)
                    .HasForeignKey(f => f.FollowedId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        );

        modelBuilder.Entity<GuildUser>(gu =>
            {
                gu.HasKey(g => new
                {
                    g.GuildId,
                    g.UserId
                });

                gu.HasOne(g => g.User).WithMany(g => g.Guilds).HasForeignKey(g => g.UserId).OnDelete(DeleteBehavior.Cascade);

                gu.HasOne(g => g.Guild).WithMany(g => g.Users).HasForeignKey(g => g.GuildId).OnDelete(DeleteBehavior.Cascade);
            }
        );

        modelBuilder.Entity<Reminder>(reminder =>
            {
                reminder.HasKey(r => r.ReminderGuid);

                reminder.HasOne(r => r.User).WithMany(u => u.Reminders).OnDelete(DeleteBehavior.Cascade);
            }
        );

        modelBuilder.Entity<GuildChannel>(channel =>
            {
                channel.HasKey(c => c.ChannelId);
                channel.HasOne(c => c.Guild).WithMany(g => g.Channels).OnDelete(DeleteBehavior.Cascade);
            }
        );

        modelBuilder.Entity<Guild>(guild =>
            {
                guild.HasKey(g => g.GuildId);
            }
        );
    }
}