using System;
using System.Diagnostics;
using System.Linq;
using WalkEngine;

namespace WalkEngineStatistics
{
    internal static class Program
    {
        public static void Main()
        {
            var generationProducerTypes = ReflectionHelper.GetImplementationTypes(typeof(IGenerationProducer))
                .OrderBy(OrderAttribute.GetOrder)
                .ThenBy(type => type.Name)
                .ThenBy(type => type.FullName);
            
            foreach (var generationProducerType in generationProducerTypes)
                ShowStatistics(generationProducerType);
        }

        private static void ShowStatistics(Type generationProducerType)
        {
            const int IterationsCount = 100;
            
            Console.WriteLine($"Collecting statistics for {generationProducerType.Name} ({IterationsCount} iterations): ");
            var sw = Stopwatch.StartNew();
            
            var averageGenerationsNumber = ParallelEnumerable.Range(0, IterationsCount)
                .Select(_ => GetGenerationsNumber(generationProducerType))
                .Average();
            
            sw.Stop();

            Console.WriteLine($"Average {averageGenerationsNumber} generations");
            Console.WriteLine($"Total time: {sw.ElapsedMilliseconds:N0} ms");
        }

        private static int GetGenerationsNumber(Type generationProducerType)
        {
            var generationProducer = (IGenerationProducer) Activator.CreateInstance(generationProducerType);
            var evolution = new Evolution(generationProducer) {ModelsToTakeFromGeneration = 4,};
            while (!evolution.Finished)
                evolution.ProduceNextGeneration();
            return evolution.GenerationNumber;
        }
    }
}