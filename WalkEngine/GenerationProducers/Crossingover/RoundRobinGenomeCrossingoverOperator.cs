using System;

namespace WalkEngine
{
    public sealed class RoundRobinGenomeCrossingoverOperator : CrossingoverOperatorBase
    {
        // only for max and min angles
        protected override double GetDouble(double parentA, double parentB) => (parentA + parentB) / 2;

        protected override void VisitPeriodicValues(
            WalkModel.PeriodicValue[] parentA,
            WalkModel.PeriodicValue[] parentB,
            out WalkModel.PeriodicValue[] result)
        {
            int length;
            bool takeA;
            
            if (parentA.Length == parentB.Length)
            {
                length = parentA.Length;
                takeA = WalkModelRandomizer.Random.Next(2) == 0;
            }
            else
            {
                length = Math.Min(parentA.Length, parentB.Length) + 1;
                takeA = parentA.Length > parentB.Length;
            }

            result = new WalkModel.PeriodicValue[length];
            for (var i = length - 1; i >= 0; i--)
            {
                result[i] = takeA? parentA[i] : parentB[i];
                takeA = !takeA;
            }
        }
    }
}