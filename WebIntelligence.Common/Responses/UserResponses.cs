namespace WebIntelligence.Common.Responses;

public sealed record GetUserResponse(UserDto User);
public sealed record GetUserDiffResponse(bool HasDiff, List<string> Messages);