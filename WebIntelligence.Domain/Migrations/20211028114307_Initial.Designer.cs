// <auto-generated />
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
    [Migration("20211028114307_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0-rc.2.21480.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

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
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("RemindAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserReminders");
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
#pragma warning restore 612, 618
        }
    }
}
