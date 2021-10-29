namespace WebIntelligence.Common.Models;

public record UserVoteDto(
    ulong UserId,
    Guid PollId,
    Guid PollOptionId
)
{
    public virtual bool Equals(UserVoteDto? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return UserId == other.UserId && PollId.Equals(other.PollId) && PollOptionId.Equals(other.PollOptionId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, PollId, PollOptionId);
    }
}
