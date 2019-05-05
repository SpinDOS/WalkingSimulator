using System;

namespace WalkEngine
{
    public sealed class RandomGenomeCrossingoverOperator : CrossingoverOperatorBase
    {
        protected override double GetDouble(double parentA, double parentB) => 
            WalkModelRandomizer.Random.Next(2) == 0? parentA : parentB;

        protected override int GetPeriodicValuesCount(
            WalkModel.PeriodicValue[] parentA,
            WalkModel.PeriodicValue[] parentB)
        {
            var minLength = Math.Min(parentA.Length, parentB.Length);
            var maxLength = Math.Max(parentA.Length, parentB.Length);
            return WalkModelRandomizer.Random.Next(minLength, maxLength);
        }
    }
}