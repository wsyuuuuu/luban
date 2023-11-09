using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class XLuaCommentTypeVisitor : ITypeFuncVisitor<string>
{
    public static XLuaCommentTypeVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "boolean";
    }

    public string Accept(TByte type)
    {
        return "number";
    }

    public string Accept(TShort type)
    {
        return "number";
    }

    public string Accept(TInt type)
    {
        return "number";
    }

    public string Accept(TLong type)
    {
        return "number";
    }

    public string Accept(TFloat type)
    {
        return "number";
    }

    public string Accept(TDouble type)
    {
        return "number";
    }

    public string Accept(TEnum type)
    {
        return "number";
    }

    public string Accept(TString type)
    {
        return "string";
    }

    public string Accept(TBean type)
    {
        return type.DefBean.FullName;
    }

    public string Accept(TArray type)
    {
        return $"{type.ElementType.Apply(this)}[]";
    }

    public string Accept(TList type)
    {
        return $"{type.ElementType.Apply(this)}[]";
    }

    public string Accept(TSet type)
    {
        return $"{type.ElementType.Apply(this)}[]";
    }

    public string Accept(TMap type)
    {
        return $"table<{type.KeyType.Apply(this)},{type.ValueType.Apply(this)}>";
    }

    public string Accept(TDateTime type)
    {
        return "number";
    }
}