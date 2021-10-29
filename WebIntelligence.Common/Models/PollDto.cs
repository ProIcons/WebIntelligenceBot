namespace WebIntelligence.Common.Models;

public record PollDto
(
    Guid Id,
    string Question,
    List<PollOptionDto> Options,
    DateTimeOffset StartedTime,
    DateTimeOffset EndingTime,
    ulong ChannelId,
    ulong MessageHandle,
    List<UserVoteDto> UserVotes,
    bool Finalized
)
{
    public virtual bool Equals(PollDto? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id) 
               && Options.SequenceEqual(other.Options) 
               && UserVotes.SequenceEqual(other.UserVotes)
               && Finalized == other.Finalized;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Options, UserVotes, Finalized);
    }
}