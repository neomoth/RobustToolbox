using NUnit.Framework;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.TypeParsers.Math;

// ReSharper disable CheckNamespace
namespace Robust.UnitTesting.Shared.Toolshed;

internal sealed partial class ToolshedTests
{
    [Test]
    public async Task TestNullableArgs()
    {
        await Server.WaitAssertion(() =>
        {
            AssertResult("testnullableargs 1", "1,null,null,5");
            AssertResult("testnullableargs 1 2", "1,2,null,5");
            AssertResult("testnullableargs 1 2 \"buh\"", "1,2,buh,5");
            AssertResult("testnullableargs 1 2 \"buh\" 3", "1,2,buh,3");
            ParseError<InvalidNumber<int>>("testnullableargs 1 a");

            // Variables & blocks still work for nullable value types.
            AssertResult("i 7 => $n", 7);
            AssertResult("testnullableargs 1 $n", "1,7,null,5");
            AssertResult("testnullableargs 1 { i 8 }", "1,8,null,5");

            AssertResult("teststringdefault", "buh");
            AssertResult("teststringdefault \"meow\"", "meow");

            AssertResult("testoptionalnodefault", "null,0");
            AssertResult("testoptionalnodefault 1 2", "1,2");

            AssertResult("testnullableparams", "");
            AssertResult("testnullableparams 1 2 $n", "1,2,7");

            // `null` keyword
            AssertResult("testnullableargs 1 null", "1,null,null,5");
            AssertResult("testnullableargs 1 2 null null", "1,2,null,null");
            AssertResult("testnullableargs 1 null; i 3", 3);
            AssertResult("testnullableparams 1 null 3", "1,null,3");
            ParseError<ToolshedCommandImplementor.ArgumentNotNullableError>("testnullableargs null");
            ParseError<ToolshedCommandImplementor.ArgumentNotNullableError>("teststringdefault null");
            ParseError<InvalidNumber<int>>("testnullableargs 1 nullx");

            // `_` keyword
            AssertResult("testnullableargs 1 _ \"x\"", "1,null,x,5");
            AssertResult("testnullableargs 1 2 _ _", "1,2,null,5");
            AssertResult("teststringdefault _", "buh");
            AssertResult("testoptionalnodefault _ _", "null,0");
            AssertResult("testoptionalnodefault _ 2", "null,2");
            AssertResult("testnullableparams 1 _ 3", "1,null,3");
            ParseError<ToolshedCommandImplementor.ArgumentNotNullableError>("testnullableargs _");
            ParseError<InvalidNumber<int>>("testnullableargs 1 _x");

            // Escaped keywords are parsed literally.
            AssertResult("testnullableenum Foo", "Foo");
            AssertResult("testnullableenum null", "<none>");
            AssertResult("testnullableenum _", "<none>");
            AssertResult("testnullableenum \\null", "Null");
            AssertResult("testnullableenum Null", "Null"); // Keywords are case-sensitive.
            AssertResult("testnullableenum \\_", "_");

            // Keywords are suggested as completions.
            AssertCompletionContains("testnullableargs 1 ", "null");
            AssertCompletionContains("testnullableargs 1 ", "_");
            AssertCompletionContains("testnullableargs 1 nu", "null");
            AssertCompletionContains("teststringdefault ", "_");
        });
    }
}
