﻿// <auto-generated />
using System;
using Catness.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Catness.Persistence.Migrations
{
    [DbContext(typeof(CatnessDbContext))]
    [Migration("20231215224305_AddTimeCreated")]
    partial class AddTimeCreated
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Catness.Persistence.Models.Follow", b =>
                {
                    b.Property<decimal>("FollowerId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("FollowedId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("FollowerId", "FollowedId");

                    b.HasIndex("FollowedId");

                    b.ToTable("Follows");
                });

            modelBuilder.Entity("Catness.Persistence.Models.Guild", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("LevellingEnabled")
                        .HasColumnType("boolean");

                    b.HasKey("GuildId");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("Catness.Persistence.Models.GuildUser", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("IsLevelBlacklisted")
                        .HasColumnType("boolean");

                    b.HasKey("GuildId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GuildUsers");
                });

            modelBuilder.Entity("Catness.Persistence.Models.Reminder", b =>
                {
                    b.Property<decimal>("ReminderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal?>("ChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("PrivateReminder")
                        .HasColumnType("boolean");

                    b.Property<int>("Reminded")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("ReminderTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("ReminderId");

                    b.HasIndex("UserId");

                    b.ToTable("Reminders");
                });

            modelBuilder.Entity("Catness.Persistence.Models.User", b =>
                {
                    b.Property<decimal>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateOnly?>("Birthday")
                        .HasColumnType("date");

                    b.Property<decimal>("Experience")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTimeOffset>("LastRepTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastfmUsername")
                        .HasColumnType("text");

                    b.Property<short>("Level")
                        .HasColumnType("smallint");

                    b.Property<bool>("LevellingEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Locale")
                        .HasColumnType("text");

                    b.Property<bool>("PrivateUser")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Rep")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("SteamVanity")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Catness.Persistence.Models.Follow", b =>
                {
                    b.HasOne("Catness.Persistence.Models.User", "Followed")
                        .WithMany("Followers")
                        .HasForeignKey("FollowedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Catness.Persistence.Models.User", "Follower")
                        .WithMany("Following")
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Followed");

                    b.Navigation("Follower");
                });

            modelBuilder.Entity("Catness.Persistence.Models.GuildUser", b =>
                {
                    b.HasOne("Catness.Persistence.Models.Guild", "Guild")
                        .WithMany("Users")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Catness.Persistence.Models.User", "User")
                        .WithMany("Guilds")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Catness.Persistence.Models.Reminder", b =>
                {
                    b.HasOne("Catness.Persistence.Models.User", "User")
                        .WithMany("Reminders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
