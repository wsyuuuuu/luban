using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class XLuaDeserializeMethodNameVisitor : ITypeFuncVisitor<string, string>
{
    public static XLuaDeserializeMethodNameVisitor Ins { get; } = new ();

    public string Accept(TBool type, string bufName)
    {
        return $"{bufName}:ReadBool()";
    }

    public string Accept(TByte type, string bufName)
    {
        return $"{bufName}:ReadByte()";
    }

    public string Accept(TShort type, string bufName)
    {
        return $"{bufName}:ReadShort()";
    }

    public string Accept(TInt type, string bufName)
    {
        return $"{bufName}:ReadInt()";
    }

    public string Accept(TLong type, string bufName)
    {
        return $"{bufName}:ReadLong()";
    }

    public string Accept(TFloat type, string bufName)
    {
        return $"{bufName}:ReadFloat()";
    }

    public string Accept(TDouble type, string bufName)
    {
        return $"{bufName}:ReadDouble()";
    }

    public string Accept(TEnum type, string bufName)
    {
        return $"{bufName}:ReadInt()";
    }

    public string Accept(TString type, string bufName)
    {
        return $"{bufName}:ReadString()";
    }

    public string Accept(TBean type, string bufName)
    {
        return $"(require('{type.DefBean.Name}'))._deserialize()";
    }

    public string Accept(TArray type, string bufName)
    {
        return "**********";
    }

    public string Accept(TList type, string bufName)
    {
        return "**********";
    }

    public string Accept(TSet type, string bufName)
    {
        return "**********";
    }

    public string Accept(TMap type, string bufName)
    {
        return "**********";
    }

    public string Accept(TDateTime type, string bufName)
    {
        return $"{bufName}:ReadLong()";
    }
}