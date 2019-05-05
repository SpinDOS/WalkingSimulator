using System.Collections.Generic;
using System.Linq;

namespace WalkEngine
{
    public sealed class RandomGenerationProducer : IGenerationProducer
    {
        private readonly WalkModelRandomizer _randomizer = new WalkModelRandomizer();
        
        public ParallelQuery<WalkModel> Init(int desiredCount) => _randomizer.GetGeneration(desiredCount);

        public ParallelQuery<WalkModel> GetNextGeneration(IReadOnlyList<WalkModelDescription> previousGeneration) =>
            Init(previousGeneration.Count)
                .Concat(previousGeneration.AsParallel().Select(modelDescription => modelDescription.WalkModel));
    }
}