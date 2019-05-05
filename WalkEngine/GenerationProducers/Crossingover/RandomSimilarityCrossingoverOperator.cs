using System;

namespace WalkEngine
{
    public sealed class RandomSimilarityCrossingoverOperator : CrossingoverOperatorBase
    {
        protected override double GetDouble(double parentA, double parentB)
        {
            var min = Math.Min(parentA, parentB);
            var max = Math.Max(parentA, parentB);
            return min + (max - min) * WalkModelRandomizer.Random.NextDouble();
        }
    }
}