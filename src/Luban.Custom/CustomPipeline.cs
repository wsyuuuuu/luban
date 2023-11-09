using Luban.Pipeline;

namespace Luban.Custom;

[Pipeline("custom")]
public class CustomPipeline : DefaultPipeline
{
    public override void Run(PipelineArguments args)
    {
        base.Run(args);
        DictBinaryGeneration.Generate();
    }
}