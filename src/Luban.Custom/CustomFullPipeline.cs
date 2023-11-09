using Luban.Pipeline;

namespace Luban.Custom;

[Pipeline("custom-full")]
public class CustomFullPipeline : DefaultPipeline
{
    public override void Run(PipelineArguments args)
    {
        base.Run(args);
        DictBinaryGeneration.Generate();
        DataManifestGeneration.Generate();
    }
}