namespace WebIntelligence.Bot;

public class BotState
{
    public bool IsCacheReady { get; set; }
    public bool IsReady => IsCacheReady;
}