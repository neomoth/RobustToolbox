using System;
using System.Collections.Generic;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared._Starlight.Toolshed.Syntax;

internal static class NullableValueRef<T> where T : struct
{
    internal static T? EvaluateParameter(object? obj, IInvocationContext ctx)
    {
        return obj switch
        {
            null => null,
            T cast => cast,
            ValueRef<T> @ref => @ref.Evaluate(ctx),
            ValueRef<T?> @ref => @ref.Evaluate(ctx),
            _ => throw new Exception(
                $"Failed to parse command parameter. This likely is a toolshed bug and should be reported.\n" +
                $"Target type: {typeof(T?).PrettyName()}.\n" +
                $"Input type: {obj.GetType()}.\n" +
                $"Input: {obj}")
        };
    }

    internal static T?[] EvaluateParamsCollection(object? obj, IInvocationContext ctx)
    {
        if (obj is not List<object?> parsedValues)
            throw new Exception("Failed to parse command parameter. This likely is a toolshed bug and should be reported.");

        var i = 0;
        var arr = new T?[parsedValues.Count];
        foreach (var parsed in parsedValues)
        {
            arr[i++] = EvaluateParameter(parsed, ctx);
        }

        return arr;
    }
}
