using System;
using System.Collections.Generic;
using System.Text;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

internal sealed partial class ToolshedCommandImplementor
{
    public sealed class ArgumentNotNullableError(Type type) : ConError
    {
        public override FormattedMessage DescribeInner()
        {
            return FormattedMessage.FromUnformatted(
                $@"Command argument of type {type.PrettyName()} can't be null or skipped. Use \null or \_ to pass them literally.");
        }
    }

    private enum ArgKeyword : byte
    {
        None,

        /// <summary>
        /// <c>null</c>: explicitly pass null to a nullable argument.
        /// </summary>
        Null,

        /// <summary>
        /// <c>_</c>: use the argument's default value, or null if it has none but is nullable.
        /// </summary>
        Skip,

        /// <summary>
        /// <c>\null</c> or <c>\_</c>: the backslash has been consumed and the word should be parsed literally.
        /// </summary>
        Escaped,
    }

    /// <summary>
    /// Checks whether the next word is one of the argument keywords (<c>null</c>, <c>_</c>) or an escaped keyword
    /// (<c>\null</c>, <c>\_</c>). Keywords are consumed; for escaped keywords only the backslash is consumed.
    /// </summary>
    private static ArgKeyword TryEatArgKeyword(ParserContext ctx)
    {
        var restore = ctx.Save();
        var escaped = ctx.EatMatch('\\');
        var afterEscape = ctx.Save();

        ArgKeyword keyword;
        if (ctx.TryMatch("null"))
            keyword = ArgKeyword.Null;
        else if (ctx.TryMatch("_"))
            keyword = ArgKeyword.Skip;
        else
            keyword = ArgKeyword.None;

        // Must be the whole word, e.g. "nullable" or "_foo" are not keywords.
        if (keyword != ArgKeyword.None
            && ctx.PeekRune() is { } next
            && !Rune.IsWhiteSpace(next)
            && !ctx.PeekCommandOrBlockTerminated())
        {
            keyword = ArgKeyword.None;
        }

        if (keyword == ArgKeyword.None)
        {
            ctx.Restore(restore);
            return ArgKeyword.None;
        }

        if (escaped)
        {
            ctx.Restore(afterEscape);
            return ArgKeyword.Escaped;
        }

        return keyword;
    }

    private bool FinishKeywordArgument(ParserContext ctx, CommandArgument arg, ParserRestorePoint save)
    {
        if (!ctx.GenerateCompletions || !ctx.OutOfInput)
            return true;

        // Same as TryParseArgument: at the end of the input, keep suggesting completions for this argument.
        ctx.Restore(save);
        ctx.Error = null;
        ctx.Completions ??= WithKeywordCompletions(arg, arg.Parser!.TryAutocomplete(ctx, arg));
        TrySetArgHint(ctx, arg);
        return false;
    }

    private static CompletionResult? WithKeywordCompletions(CommandArgument arg, CompletionResult? result)
    {
        var canSkip = arg.IsOptional || arg.IsNullable;
        if (!arg.IsNullable && !canSkip)
            return result;

        var opts = new List<CompletionOption>(result?.Options ?? []);
        if (arg.IsNullable)
            opts.Add(new CompletionOption("null", "null", CompletionOptionFlags.NoEscape));
        if (canSkip)
            opts.Add(new CompletionOption("_", arg.IsOptional ? "default" : "null", CompletionOptionFlags.NoEscape));

        return new CompletionResult(opts.ToArray(), result?.Hint);
    }
}
