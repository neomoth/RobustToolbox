using System;

// ReSharper disable CheckNamespace
namespace Robust.Client.UserInterface.CustomControls;

public sealed partial class DebugConsole
{
    private static int MatchingPrefixLength(string text, int argStart, int cursor, string completion)
    {
        var max = Math.Min(completion.Length, cursor - argStart);
        for (var len = max; len > 0; len--)
        {
            if (text.AsSpan(cursor - len, len).Equals(completion.AsSpan(0, len), StringComparison.OrdinalIgnoreCase))
                return len;
        }

        return 0;
    }
}
