namespace WebIntelligence.Services.Helpers;

public static class CacheKey
{
    public static string GetPollCacheKey(Guid id)
        => $"{nameof(Poll)}/{id}";

    public static string GetActivePollsCacheKey()
        => $"{nameof(Poll)}/active";

    public static string GetPollProjectionsCacheKey()
        => $"{nameof(Poll)}/projection";
}