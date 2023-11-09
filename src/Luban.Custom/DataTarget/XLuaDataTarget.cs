using System.Text;
using Luban.Datas;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Lua.DataVisitors;

namespace Luban.Lua.DataTarget;

[DataTarget("xLua")]
public class XLuaDataTarget : DataTargetBase
{
    public void ExportTableList(DefTable t, List<Record> records, StringBuilder s)
    {
        s.Append("return").AppendLine();
        s.Append('{').AppendLine();
        foreach (Record r in records)
        {
            DBean d = r.Data;
            s.Append(d.Apply(ToLuaLiteralVisitor.Ins));
            s.Append(',').AppendLine();
        }
        s.Append('}');
    }

    protected override string OutputFileExt => "lua";
    
    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var ss = new StringBuilder();
        ExportTableList(table, records, ss);
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}_data.{OutputFileExt}",
            Content = ss.ToString(),
        };
    }
}