using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SS14.Jetfish.Core.Types;

public record Result<TResult, TError>(TResult? Value, TError? Error) where TResult : class where TError : class
{
    [PublicAPI]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess => Error == null;

    public bool TryGetResult([NotNullWhen(true)] out TResult? result)
    {
        result = Value;
        return IsSuccess;
    }

    public static Result<TResult, TError> Success(TResult value)
    {
        return new Result<TResult, TError>(value, null);
    }

    public static Result<TResult, TError> Failure(TError error)
    {
        return new Result<TResult, TError>(null, error);
    }
}