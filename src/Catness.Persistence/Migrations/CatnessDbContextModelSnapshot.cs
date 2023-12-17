﻿// <auto-generated />
using System;
using Catness.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Catness.Persistence.Migrations
{
    [DbContext(typeof(CatnessDbContext))]
    partial class CatnessDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Catness.Persistence.Models.Follow", b =>
                {
                    b.Property<decimal>("FollowerId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("follower_id");

                    b.Property<decimal>("FollowedId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("followed_id");

                    b.HasKey("FollowerId", "FollowedId")
                        .HasName("pk_follows");

                    b.HasIndex("FollowedId")
                        .HasDatabaseName("ix_follows_followed_id");

                    b.ToTable("follows", (string)null);
                });

            modelBuilder.Entity("Catness.Persistence.Models.Guild", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<bool>("LevellingEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("levelling_enabled");

                    b.HasKey("GuildId")
                        .HasName("pk_guilds");

                    b.ToTable("guilds", (string)null);
                });

            modelBuilder.Entity("Catness.Persistence.Models.GuildUser", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<decimal>("Experience")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("experience");

                    b.Property<bool>("IsLevelBlacklisted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_level_blacklisted");

                    b.Property<decimal>("Level")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("level");

                    b.HasKey("GuildId", "UserId")
                        .HasName("pk_guild_users");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_guild_users_user_id");

                    b.ToTable("guild_users", (string)null);
                });

            modelBuilder.Entity("Catness.Persistence.Models.Reminder", b =>
                {
                    b.Property<Guid>("ReminderGuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("reminder_guid");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("body");

                    b.Property<decimal?>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<bool>("PrivateReminder")
                        .HasColumnType("boolean")
                        .HasColumnName("private_reminder");

                    b.Property<DateTime>("ReminderTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("reminder_time");

                    b.Property<DateTime>("TimeCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time_created");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.HasKey("ReminderGuid")
                        .HasName("pk_reminders");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_reminders_user_id");

                    b.ToTable("reminders", (string)null);
                });

            modelBuilder.Entity("Catness.Persistence.Models.User", b =>
                {
                    b.Property<decimal>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<DateOnly?>("Birthday")
                        .HasColumnType("date")
                        .HasColumnName("birthday");

                    b.Property<DateTime?>("LastRepTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_rep_time");

                    b.Property<string>("LastfmUsername")
                        .HasColumnType("text")
                        .HasColumnName("lastfm_username");

                    b.Property<bool>("LevellingEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("levelling_enabled");

                    b.Property<string>("Locale")
                        .HasColumnType("text")
                        .HasColumnName("locale");

                    b.Property<decimal>("Rep")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("rep");

                    b.Property<string>("SteamVanity")
                        .HasColumnType("text")
                        .HasColumnName("steam_vanity");

                    b.HasKey("UserId")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Catness.Persistence.Models.Follow", b =>
                {
                    b.HasOne("Catness.Persistence.Models.User", "Followed")
                        .WithMany("Followers")
                        .HasForeignKey("FollowedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_follows_users_followed_id");

                    b.HasOne("Catness.Persistence.Models.User", "Follower")
                        .WithMany("Following")
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_follows_users_follower_id");

                    b.Navigation("Followed");

                    b.Navigation("Follower");
                });

            modelBuilder.Entity("Catness.Persistence.Models.GuildUser", b =>
                {
                    b.HasOne("Catness.Persistence.Models.Guild", "Guild")
                        .WithMany("Users")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_guild_users_guilds_guild_id");

                    b.HasOne("Catness.Persistence.Models.User", "User")
                        .WithMany("Guilds")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_guild_users_users_user_id");

                    b.Navigation("Guild");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Catness.Persistence.Models.Reminder", b =>
                {
                    b.HasOne("Catness.Persistence.Models.User", "User")
                        .WithMany("Reminders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_reminders_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Catness.Persistence.Models.Guild", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Catness.Persistence.Models.User", b =>
                {
                    b.Navigation("Followers");

                    b.Navigation("Following");

                    b.Navigation("Guilds");

                    b.Navigation("Reminders");
                });
#pragma warning restore 612, 618
        }
    }
}
