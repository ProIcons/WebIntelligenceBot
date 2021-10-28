using LazyCache;
using MediatR;

namespace WebIntelligence.Bot.RequestHandlers;

public abstract class WebIntelligenceRequestHandler
{
    protected readonly IMediator Mediator;
    protected readonly IAppCache AppCache;

    public WebIntelligenceRequestHandler(IMediator mediator, IAppCache appCache)
    {
        Mediator = mediator;
        AppCache = appCache;
    }
}

public abstract class WebIntelligenceRequestHandler<TRequest> : WebIntelligenceRequestHandler, IRequestHandler<TRequest, Unit> where TRequest : IRequest
{
    protected WebIntelligenceRequestHandler(IMediator mediator, IAppCache appCache) : base(mediator, appCache)
    {
    }

    Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        Handle(request);
        return Unit.Task;
    }

    protected abstract void Handle(TRequest request);
}

public abstract class WebIntelligenceAsyncRequestHandler<TRequest> : WebIntelligenceRequestHandler, IRequestHandler<TRequest> where TRequest : IRequest
{
    protected WebIntelligenceAsyncRequestHandler(IMediator mediator, IAppCache appCache) : base(mediator, appCache)
    {
    }

    async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await Handle(request, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }

    protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);
}