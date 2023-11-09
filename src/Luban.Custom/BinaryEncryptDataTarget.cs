using Luban.DataExporter.Builtin.Binary;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Serialization;

namespace Luban.Custom;

[DataTarget("enc-bin")]
public class BinaryEncryptDataTarget : DataTargetBase
{
    protected override string OutputFileExt => "bytes";

    private void WriteList(DefTable table, List<Record> datas, ByteBuf x)
    {
        x.WriteSize(datas.Count);
        foreach (var d in datas)
        {
            d.Data.Apply(BinaryDataVisitor.Ins, x);
        }
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var bytes = new ByteBuf();
        WriteList(table, records, bytes);
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = EncryptionUtil.Encrypt(table.OutputDataFile, bytes.CopyData())
        };
    }
}