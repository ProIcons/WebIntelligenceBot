using AutoMapper;

namespace WebIntelligence.Services.RequestHandlers;

public abstract class WebIntelligenceRequestHandler
{
    protected readonly IMediator Mediator;
    protected readonly WebIntelligenceContext Db;
    protected readonly IAppCache AppCache;
    protected readonly IMapper Mapper;

    public WebIntelligenceRequestHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper)
    {
        Db = db;
        Mediator = mediator;
        AppCache = appCache;
        Mapper = mapper;
    }
}

public abstract class WebIntelligenceRequestHandler<TRequest> : WebIntelligenceRequestHandler, IRequestHandler<TRequest, Unit> where TRequest : IRequest
{
    protected WebIntelligenceRequestHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
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
    protected WebIntelligenceAsyncRequestHandler(WebIntelligenceContext db, IMediator mediator, IAppCache appCache, IMapper mapper) : base(db, mediator, appCache, mapper)
    {
    }

    async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await Handle(request, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }

    protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);
}