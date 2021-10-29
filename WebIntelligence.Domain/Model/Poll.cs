using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebIntelligence.Domain.Constants;

namespace WebIntelligence.Domain.Model;

public class Poll
{
    public Guid Id { get; set; } = default!;

    public string Question { get; set; } = null!;

    public List<PollOption> Options { get; set; } = null!;
    public DateTimeOffset StartedTime { get; set; }
    public DateTimeOffset EndingTime { get; set; }
    public ICollection<UserVote> UserVotes { get; set; }
    public ulong ChannelId { get; set; }
    public ulong MessageHandle { get; set; }
    public bool Finalized { get; set; } = false;
}

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder
            .HasKey(p => p.Id);

        builder
            .HasIndex(p => p.Id)
            .IsUnique();

        builder.Property(p => p.Id)
            .HasColumnName(nameof(Poll.Id))
            .HasColumnType(ColumnTypes.Uuid)
            .HasDefaultValueSql(ColumnDefaults.Uuid);

        builder.Property(p => p.Question)
            .HasColumnName(nameof(Poll.Question))
            .HasColumnType(ColumnTypes.Text)
            .IsRequired();

        builder.Property(p => p.ChannelId)
            .HasColumnName(nameof(Poll.ChannelId))
            .HasColumnType(ColumnTypes.UnsignedLong)
            .IsRequired();

        builder.Property(p => p.StartedTime)
            .HasColumnName(nameof(Poll.StartedTime))
            .HasColumnType(ColumnTypes.DateTimeOffset)
            .IsRequired();

        builder.Property(p => p.EndingTime)
            .HasColumnName(nameof(Poll.EndingTime))
            .HasColumnType(ColumnTypes.DateTimeOffset)
            .IsRequired();

        builder.Property(p => p.MessageHandle)
            .HasColumnName(nameof(Poll.MessageHandle))
            .HasColumnType(ColumnTypes.UnsignedLong)
            .IsRequired();

        builder.Property(p => p.Finalized)
            .HasColumnName(nameof(Poll.Finalized))
            .HasColumnType(ColumnTypes.Bool)
            .HasDefaultValue(false)
            .IsRequired();
    }
}