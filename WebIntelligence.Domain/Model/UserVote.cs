using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebIntelligence.Domain.Model;

public class UserVote
{
    public User User { get; set; } = null!;
    public ulong UserId { get; set; }

    public Poll Poll { get; set; } = null!;
    public Guid PollId { get; set; }
    
    public PollOption PollOption { get; set; } = null!;
    public Guid PollOptionId { get; set; }
}

public class UserVoteConfiguration : IEntityTypeConfiguration<UserVote>
{
    public void Configure(EntityTypeBuilder<UserVote> builder)
    {
        builder
            .HasKey(bc => new { bc.UserId, bc.PollOptionId });

        builder
            .HasOne(userVote => userVote.User)
            .WithMany(user => user.Votes)
            .HasForeignKey(userVote => userVote.UserId);

        builder
            .HasOne(userVote => userVote.PollOption)
            .WithMany(pollOption => pollOption.UserVotes)
            .HasForeignKey(userVote => userVote.PollOptionId);

        builder
            .HasOne(userVote => userVote.Poll)
            .WithMany(user => user.UserVotes)
            .HasForeignKey(userVote => userVote.PollId);
    }
}