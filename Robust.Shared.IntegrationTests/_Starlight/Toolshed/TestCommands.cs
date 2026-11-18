using Robust.Shared.Toolshed;

// ReSharper disable CheckNamespace
namespace Robust.UnitTesting.Shared.Toolshed;

[ToolshedCommand]
internal sealed class TestNullableArgsCommand : ToolshedCommand
{
    [CommandImplementation]
    public string Impl(int x, int? y = null, string? s = null, int? z = 5)
        => $"{x},{y?.ToString() ?? "null"},{s ?? "null"},{z?.ToString() ?? "null"}";
}

[ToolshedCommand]
internal sealed class TestStringDefaultCommand : ToolshedCommand
{
    [CommandImplementation]
    public string Impl(string a = "buh") => a;
}

[ToolshedCommand]
internal sealed class TestOptionalNoDefaultCommand : ToolshedCommand
{
    [CommandImplementation]
    public string Impl([System.Runtime.InteropServices.Optional] int? a, [System.Runtime.InteropServices.Optional] int b)
        => $"{a?.ToString() ?? "null"},{b}";
}

[ToolshedCommand]
internal sealed class TestNullableEnumCommand : ToolshedCommand
{
    public enum TestEnum
    {
        Foo,
        Null,
        _,
    }

    [CommandImplementation]
    public string Impl(TestEnum? e) => e?.ToString() ?? "<none>";
}

[ToolshedCommand]
internal sealed class TestNullableParamsCommand : ToolshedCommand
{
    [CommandImplementation]
    public string Impl(params int?[] values) => string.Join(",", values.Select(x => x?.ToString() ?? "null"));
}
