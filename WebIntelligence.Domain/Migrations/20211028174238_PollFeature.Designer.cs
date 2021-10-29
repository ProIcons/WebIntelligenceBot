﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebIntelligence.Domain;

#nullable disable

namespace WebIntelligence.Domain.Migrations
{
    [DbContext(typeof(WebIntelligenceContext))]
    [Migration("20211028174238_PollFeature")]
    partial class PollFeature
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0-rc.2.21480.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WebIntelligence.Domain.Model.Poll", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("CachedFinishingMessage")
                        .HasColumnType("text");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("ChannelId");

                    b.Property<DateTimeOffset>("EndingTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("EndingTime");

                    b.Property<decimal>("MessageHandle")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("MessageHandle");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Question");

                    b.Property<DateTimeOffset>("StartedTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("StartedTime");

                    b.Property<int>("TotalVotes")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Polls");
                });

            modelBuilder.Entity("WebIntelligence.Domain.Model.PollOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("PollId")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Value");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("PollId");

                    b.ToTable("PollOptions");
                });

            modelBuilder.Entity("WebIntelligence.Domain.Model.User", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTimeOffset>("FirstSeenDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("JoinedGuildDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("LastSeenDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Nickname")
                        .HasColumnType("text");

                    b.Property<string>("UsernameWithDiscriminator")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebIntelligence.Domain.Model.UserReminder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("DiscordChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("RemindAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserReminders");
                });

            modelBuilder.Entity("WebIntelligence.Domain.Model.UserVote", b =>
                {
                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<Guid>("PollOptionId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PollId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "PollOptionId");

                    b.HasIndex("PollId");

                    b.HasIndex("PollOptionId");

                    b.ToTable("UserVotes");
                });

            modelBuilder.Entity("WebIntelligence.Domain.Model.PollOption", b =>
                {
                    b.HasOne("WebIntelligence.Domain.Model.Poll", "Poll")
                        .WithMany("Options")
                        .HasForeignKey("PollId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Poll");
                });

            modelBuilder.Entity("WebIntelligence.Domain.Model.UserReminder", b =>
                {
                    b.HasOne("WebIntelligence.Domain.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebIntelligence.Domain.Model.UserVote", b =>
                {
                    b.HasOne("WebIntelligence.Domain.Model.Poll", null)
                        .WithMany("Voters")
                        .HasForeignKey("PollId");

                    b.HasOne("WebIntelligence.Domain.Model.PollOption", "PollOption")
                        .WithMany("UserVotes")
                        .HasForeignKey("PollOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebIntelligence.Domain.Model.User", "User")
                        .WithMany("Votes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PollOption");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebIntelligence.Domain.Model.Poll", b =>
                {
                    b.Navigation("Options");

                    b.Navigation("Voters");
                });

            modelBuilder.Entity("WebIntelligence.Domain.Model.PollOption", b =>
                {
                    b.Navigation("UserVotes");
                });

            modelBuilder.Entity("WebIntelligence.Domain.Model.User", b =>
                {
                    b.Navigation("Votes");
                });
#pragma warning restore 612, 618
        }
    }
}
