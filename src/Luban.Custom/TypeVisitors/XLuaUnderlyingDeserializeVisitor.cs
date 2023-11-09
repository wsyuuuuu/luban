using System.Text;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class XLuaUnderlyingDeserializeVisitor : DecoratorFuncVisitor<string, string, string, string>
{
    public static XLuaUnderlyingDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string bufName, string fieldName, string tableName)
    {
        return $"{tableName}.{fieldName} = {type.Apply(XLuaDeserializeMethodNameVisitor.Ins, bufName)}";
    }

    public override string Accept(TArray type, string bufName, string fieldName, string tableName)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{tableName}.{fieldName} = {{}}");
        sb.AppendLine($"for _ = 1, {bufName}:ReadSize() do");
        sb.AppendLine($"    table.insert({tableName}.{fieldName}, {type.ElementType.Apply(XLuaDeserializeMethodNameVisitor.Ins, bufName)})");
        sb.AppendLine("end");
        sb.Append($"setmetatable({tableName}.{fieldName}, {{ __newindex = function(t, k, v) error('attempt to update {fieldName}') end }})");
        return sb.ToString();
    }

    public override string Accept(TList type, string bufName, string fieldName, string tableName)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{tableName}.{fieldName} = {{}}");
        sb.AppendLine($"for _ = 1, {bufName}:ReadSize() do");
        sb.AppendLine($"    table.insert({tableName}.{fieldName}, {type.ElementType.Apply(XLuaDeserializeMethodNameVisitor.Ins, bufName)})");
        sb.AppendLine("end");
        sb.Append($"setmetatable({tableName}.{fieldName}, {{ __newindex = function(t, k, v) error('attempt to update {fieldName}') end }})");
        return sb.ToString();
    }

    public override string Accept(TSet type, string bufName, string fieldName, string tableName)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{tableName}.{fieldName} = {{}}");
        sb.AppendLine($"for _ = 1, {bufName}:ReadSize() do");
        sb.AppendLine($"    {tableName}.{fieldName}[ {type.ElementType.Apply(XLuaDeserializeMethodNameVisitor.Ins, bufName)} ] = true");
        sb.AppendLine("end");
        sb.Append($"setmetatable({tableName}.{fieldName}, {{ __newindex = function(t, k, v) error('attempt to update {fieldName}') end }})");
        return sb.ToString();
    }

    public override string Accept(TMap type, string bufName, string fieldName, string tableName)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{tableName}.{fieldName} = {{}}");
        sb.AppendLine($"for _ = 1, {bufName}:ReadSize() do");
        sb.AppendLine($"    {tableName}.{fieldName}[ {type.KeyType.Apply(XLuaDeserializeMethodNameVisitor.Ins, bufName)} ] = {type.ElementType.Apply(XLuaDeserializeMethodNameVisitor.Ins, bufName)}");
        sb.AppendLine("end");
        sb.Append($"setmetatable({tableName}.{fieldName}, {{ __newindex = function(t, k, v) error('attempt to update {fieldName}') end }})");
        return sb.ToString();
    }
}