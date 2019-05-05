using System.Collections.Generic;
using System.Linq;

namespace WalkEngine
{
    public interface IGenerationProducer
    {
        ParallelQuery<WalkModel> Init(int desiredCount);
        
        ParallelQuery<WalkModel> GetNextGeneration(IReadOnlyList<WalkModelDescription> previousGeneration);
    }
}