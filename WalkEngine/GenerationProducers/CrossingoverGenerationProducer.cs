using System;
using System.Collections.Generic;
using System.Linq;

namespace WalkEngine
{
    [Order(-1000)]
    public sealed class CrossingoverGenerationProducer : IGenerationProducer
    {
        private readonly IReadOnlyCollection<ICrossingoverOperator> _crossingoverOperators =
            ReflectionHelper.GetImplementations<ICrossingoverOperator>().ToList();
        
        private readonly WalkModelMutator _mutator = new WalkModelMutator();

        public ParallelQuery<WalkModel> Init(int desiredCount) => _mutator.Randomizer.GetGeneration(desiredCount);

        public ParallelQuery<WalkModel> GetNextGeneration(IReadOnlyList<WalkModelDescription> previousGeneration)
        {
            return ParallelEnumerable.Range(0, previousGeneration.Count - 1)
                .SelectMany(
                    i => Enumerable.Range(i + 1, previousGeneration.Count - (i + 1)),
                    (i, j) => (previousGeneration[i].WalkModel, previousGeneration[j].WalkModel))
                .SelectMany(_ => _crossingoverOperators,
                    (parents, crossingoverOperator) =>
                    {
                        var child = crossingoverOperator.CombineModels(parents.Item1, parents.Item2);
                        _mutator.MutateModel(child);
                        return child;
                    });
        }
    }
}