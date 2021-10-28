using WebIntelligence.Bot.Models;

namespace WebIntelligence.Bot.Models;

public record EmptyMessage : BaseMessage<EmptyMessage>
{
    private static EmptyMessage? _instance;
    public static EmptyMessage Instance => _instance ??= new EmptyMessage();
}