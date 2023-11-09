using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Defs;
using Luban.Lua.TemplateExtensions;
using Luban.Utils;
using Scriban;
using Scriban.Runtime;

namespace Luban.Custom.CodeTarget;

[CodeTarget("xLua")]
public class XLuaCodeTarget : TemplateCodeTargetBase
{
    public override string FileHeader { get; } = CommonFileHeaders.AUTO_GENERATE_LUA;

    protected override string FileSuffixName { get; } = "lua.txt";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.LuaDefaultCodeStyle;
    protected override ICodeStyle CodeStyle => CodeFormatManager.Ins.LuaDefaultCodeStyle;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new XLuaTemplateExtension());
    }

    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        var tasks = new List<Task<OutputFile>>();
        tasks.Add(Task.Run(() =>
        {
            var writer = new CodeWriter();
            GenerateTables(ctx, ctx.ExportTables, writer);
            return new OutputFile(){ File = $"{GetFileNameWithoutExtByTypeName(ctx.Target.Manager)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
        }));
        
        foreach (var table in ctx.ExportTables)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateTable(ctx, table, writer);
                return new OutputFile(){ File = $"{GetFileNameWithoutExtByTypeName(table.FullName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
        }

        foreach (var bean in ctx.ExportBeans)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateBean(ctx, bean, writer);
                return new OutputFile(){ File = $"{GetFileNameWithoutExtByTypeName(bean.FullName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
        }

        tasks.Add(Task.Run(() =>
        {
            var writer = new CodeWriter();
            GenerateEnum(ctx, writer);
            return new OutputFile(){ File = $"{GetFileNameWithoutExtByTypeName("Enums")}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
        }));

        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }
    }

    public void GenerateEnum(GenerationContext ctx, CodeWriter writer)
    {
        var template = GetTemplate("enum");
        var tplCtx = CreateTemplateContext(template);
        OnCreateTemplateContext(tplCtx);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", ctx.Target.Manager},
            { "__namespace", ctx.Target.TopModule},
            { "__full_name", TypeUtil.MakeFullName(ctx.Target.TopModule, ctx.Target.Manager)},
            { "__enums", ctx.ExportEnums},
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
        writer.ToResult(FileHeader);
    }
}