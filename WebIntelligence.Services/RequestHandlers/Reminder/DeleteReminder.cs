using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebIntelligence.Common.Helpers;
using WebIntelligence.Common.Requests;

namespace WebIntelligence.Services.RequestHandlers.Reminder;

public class DeleteReminderHandler :
    WebIntelligenceRequestHandler,
    IRequestHandler<DeleteReminderRequest, IResult>,
    IRequestHandler<DeleteAllRemindersRequest, IResult>
{
    public DeleteReminderHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    public async Task<IResult> Handle(DeleteReminderRequest request, CancellationToken cancellationToken)
    {
        var hasReminder = await Mediator.Send(new UserHasReminderRequest(request.DiscordUserId, request.ReminderId), cancellationToken);
        if (!hasReminder.IsSuccess || !hasReminder.Entity)
        {
            return Results.Fail("Reminder does not exist");
        }

        var reminder = await Db.UserReminders.SingleAsync(
            x => x.UserId == request.DiscordUserId && x.Id == request.ReminderId,
            cancellationToken);


        Db.Remove(reminder);

        await Db.SaveChangesAsync(cancellationToken);

        await Mediator.Send(new InvalidateGetRemindersRequest(request.DiscordUserId), cancellationToken);

        return Results.Success();
    }

    public async Task<IResult> Handle(DeleteAllRemindersRequest request, CancellationToken cancellationToken)
    {
        var reminds = await Db.UserReminders
            .Where(x => x.UserId == request.DiscordUserId)
            .ToListAsync(cancellationToken);

        Db.RemoveRange(reminds);

        await Db.SaveChangesAsync(cancellationToken);

        await Mediator.Send(new InvalidateGetRemindersRequest(request.DiscordUserId), cancellationToken);

        return Results.Success();
    }
}