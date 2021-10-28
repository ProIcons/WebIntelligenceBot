namespace WebIntelligence.Common.Helpers;

public static class Results
{
    public static IResult Fail(String message)
        => Fail<object>(message);

    public static Result<T> Fail<T>(String message)
        => Result<T>.FromError(new ServiceError(message));

    public static Result<T> Success<T>(T payload)
        => Result<T>.FromSuccess(payload);

    public static IResult Success(object payload)
        => Success<object>(payload);

    public static IResult Success()
        => Result.FromSuccess();
}