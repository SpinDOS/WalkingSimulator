using System.Collections.Generic;
using System.Linq;

namespace WalkEngine
{
    [Order(-100)]
    public sealed class RandomMutationProducer : IGenerationProducer
    {
        private double ChanceToCreateRandomModel { get; } = WalkModelMutator.BasicChanceToMutate;
        
        private readonly WalkModelMutator _mutator = new WalkModelMutator();
        private readonly WalkModelSimilarityComparer _similarityComparer = new WalkModelSimilarityComparer();

        public ParallelQuery<WalkModel> Init(int desiredCount) => _mutator.Randomizer.GetGeneration(desiredCount);

        public ParallelQuery<WalkModel> GetNextGeneration(IReadOnlyList<WalkModelDescription> previousGeneration) =>
            previousGeneration
                .AsParallel()
                .Select(modelDescription => modelDescription.WalkModel)
                .SelectMany(GetMutatedCloneAndSelf);

        private IEnumerable<WalkModel> GetMutatedCloneAndSelf(WalkModel walkModel)
        {
            if (WalkModelRandomizer.Random.NextDouble() < ChanceToCreateRandomModel)
                yield return _mutator.Randomizer.GetWalkModel();
            
            var clone = walkModel.Clone();
            _mutator.MutateModel(clone);
            yield return clone;
            
            if (!_similarityComparer.AreSimilar(clone, walkModel))
                yield return walkModel;
        }
    }
}