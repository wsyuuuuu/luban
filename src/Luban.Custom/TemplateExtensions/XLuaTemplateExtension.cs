using System.Text;
using Luban.Lua.TypVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Lua.TemplateExtensions;

public class XLuaTemplateExtension : ScriptObject
{
    public static string DeserializeByteBuffer(TType type, string bufName, string fieldName, string tableName)
    {
        return type.Apply(XLuaUnderlyingDeserializeVisitor.Ins, bufName, fieldName, tableName);
    }

    public static string ParseLuaTable(TType type, string sourceName, string fieldName, string tableName)
    {
        switch (type)
        {
            case TBool:
            case TByte:
            case TShort:
            case TInt:
            case TLong:
            case TFloat:
            case TDouble:
            case TEnum:
            case TString:
            case TDateTime:
                return $"{tableName}.{fieldName} = {sourceName}.{fieldName}";
            case TArray:
            case TList:
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{tableName}.{fieldName} = {{}}");
                sb.AppendLine($"for _,v in ipairs({sourceName}.{fieldName}) do");
                if (type.ElementType.IsBean)
                {
                    var beanType = (TBean)type.ElementType;
                    sb.AppendLine($"    table.insert({tableName}.{fieldName}, (require('{beanType.DefBean.Name}'))._parse(v))");
                }
                else
                {
                    sb.AppendLine($"    table.insert({tableName}.{fieldName}, v)");
                }
                sb.AppendLine("end");
                sb.Append($"setmetatable({tableName}.{fieldName}, {{ __newindex = function(t, k, v) error('attempt to update {fieldName}') end }})");
                return sb.ToString();
            }
            case TSet:
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{tableName}.{fieldName} = {{}}");
                sb.AppendLine($"for _,v in ipairs({sourceName}.{fieldName}) do");
                if (type.ElementType.IsBean)
                {
                    var beanType = (TBean)type.ElementType;
                    sb.AppendLine($"    {tableName}.{fieldName}[ (require('{beanType.DefBean.Name}'))._parse(v) ] = true");
                }
                else
                {
                    sb.AppendLine($"    {tableName}.{fieldName}[ v ] = true");
                }
                sb.AppendLine("end");
                sb.Append($"setmetatable({tableName}.{fieldName}, {{ __newindex = function(t, k, v) error('attempt to update {fieldName}') end }})");
                return sb.ToString();
            }
            case TMap:
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{tableName}.{fieldName} = {{}}");
                sb.AppendLine($"for k,v in ipairs({sourceName}.{fieldName}) do");
                if (type.ElementType.IsBean)
                {
                    var beanType = (TBean)type.ElementType;
                    sb.AppendLine($"    {tableName}.{fieldName}[ k ] = (require('{beanType.DefBean.Name}'))._parse(v)");
                }
                else
                {
                    sb.AppendLine($"    {tableName}.{fieldName}[ k ] = v");
                }
                sb.AppendLine("end");
                sb.Append($"setmetatable({tableName}.{fieldName}, {{ __newindex = function(t, k, v) error('attempt to update {fieldName}') end }})");
                return sb.ToString();
            }
            case TBean bean:
                return $"{tableName}.{fieldName} = (require('{bean.DefBean.Name}'))._parse({sourceName}.{fieldName})";
        }

        return "*********";
    }

    public static string CommentType(TType type)
    {
        return type.Apply(XLuaCommentTypeVisitor.Ins);
    }
}