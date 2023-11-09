using Luban.Pipeline;

namespace Luban.Custom;

[Pipeline("custom-dict")]
public class CustomDictOnlyPipeline : IPipeline
{
    public void Run(PipelineArguments args)
    {
        DictBinaryGeneration.Generate();
    }
}