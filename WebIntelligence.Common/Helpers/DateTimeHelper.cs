namespace WebIntelligence.Common.Helpers;

public class DateTimeHelper
{
    public static DateTimeOffset Max(params DateTimeOffset[] inputs)
    {
        return inputs.Max();
    }

    public static DateTimeOffset Min(params DateTimeOffset[] inputs)
    {
        return inputs.Min();
    }
}