using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebIntelligence.Domain.Constants;

namespace WebIntelligence.Domain.Model;

public class PollOption
{
    public Guid Id { get; set; }

    public Poll Poll { get; set; } = null!;
    public Guid PollId { get; set; }

    public ICollection<UserVote> UserVotes { get; set; } = null!;

    public string Value { get; set; } = null!;
}

public class PollOptionConfiguration : IEntityTypeConfiguration<PollOption>
{
    public void Configure(EntityTypeBuilder<PollOption> builder)
    {
        builder
            .HasKey(p => p.Id);

        builder
            .HasIndex(p => p.Id)
            .IsUnique();

        builder.Property(p => p.Id)
            .HasColumnName(nameof(PollOption.Id))
            .HasColumnType(ColumnTypes.Uuid)
            .HasDefaultValueSql(ColumnDefaults.Uuid);

        builder.Property(p => p.Value)
            .HasColumnName(nameof(PollOption.Value))
            .HasColumnType(ColumnTypes.Text)
            .IsRequired();
    }
}