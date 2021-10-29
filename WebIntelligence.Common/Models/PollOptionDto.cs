namespace WebIntelligence.Common.Models;

public record PollOptionDto(string Value, string Id, List<UserVoteDto> UserVotes)
{
    public virtual bool Equals(PollOptionDto? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && Id == other.Id && UserVotes.SequenceEqual(other.UserVotes);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Id, UserVotes);
    }
}