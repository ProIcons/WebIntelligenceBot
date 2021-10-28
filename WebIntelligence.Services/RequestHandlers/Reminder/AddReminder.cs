using AutoMapper;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Common.Requests;
using WebIntelligence.Common.Responses;

namespace WebIntelligence.Services.RequestHandlers.Reminder;

public class AddReminderHandler : WebIntelligenceRequestHandler, IRequestHandler<AddReminderRequest, IResult>
{
    public AddReminderHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    public async Task<IResult> Handle(AddReminderRequest request, CancellationToken cancellationToken)
    {
        var userExists = await Mediator.Send(new UserExistsRequest(request.DiscordUserId), cancellationToken);

        if (!userExists)
            return Results.Fail<GetUserResponse>("User does not exist");

        var dateTime = DateTimeOffset.UtcNow;

        var reminder = new UserReminder
        {
            UserId = request.DiscordUserId,
            DiscordChannelId = request.DiscordChannelId,
            RemindAt = dateTime.Add(request.TimeSpan),
            CreatedAt = dateTime,
            Message = request.Message
        };

        Db.Add(reminder);

        await Db.SaveChangesAsync(cancellationToken);

        await Mediator.Send(new InvalidateGetRemindersRequest(request.DiscordUserId), cancellationToken);

        return Results.Success();
    }
}