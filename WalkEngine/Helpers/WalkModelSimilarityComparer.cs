using System;

namespace WalkEngine
{
    public sealed class WalkModelSimilarityComparer
    {
        private static readonly WalkModel.PeriodicValue DefaultPeriodicValue = default;

        public double AngleEpsilon { get; set; } = Math.PI / 12;
        public double KEpsilon { get; set; } = 0.15;
        
        public bool AreSimilar(WalkModel walkModelA, WalkModel walkModelB) =>
            AreSimilar(walkModelA.Left, walkModelB.Left) && AreSimilar(walkModelA.Right, walkModelB.Right);

        private bool AreSimilar(in WalkModel.LegModel legA, in WalkModel.LegModel legB) =>
            AreSimilar(legA.Top, legB.Top) && AreSimilar(legA.Bottom, legB.Bottom);

        private bool AreSimilar(in WalkModel.LegPartModel legPartA, in WalkModel.LegPartModel legPartB)
        {
            var areSimilar = 
                AreSimilar(legPartA.MinAngle, legPartB.MinAngle, AngleEpsilon) && 
                AreSimilar(legPartA.MaxAngle, legPartB.MaxAngle, AngleEpsilon);

            var maxLength = Math.Max(legPartA.PeriodicValues.Length, legPartB.PeriodicValues.Length);
            for (var i = 0; i < maxLength && areSimilar; i++)
            {
                ref readonly var periodicValueA = ref GetValueOrDefault(legPartA.PeriodicValues, i);
                ref readonly var periodicValueB = ref GetValueOrDefault(legPartB.PeriodicValues, i);
                areSimilar = AreSimilar(periodicValueA, periodicValueB);
            }

            return areSimilar;
        }

        private bool AreSimilar(in WalkModel.PeriodicValue periodicValueA, in WalkModel.PeriodicValue periodicValueB)
        {
            if (AreSimilar(periodicValueA.K, 0, KEpsilon) && AreSimilar(periodicValueB.K, 0, KEpsilon))
                return true;
            
            return AreSimilar(periodicValueA.K, periodicValueB.K, KEpsilon) && 
                   AreSimilar(periodicValueA.T0, periodicValueB.T0, AngleEpsilon) &&
                   AreSimilarSpeeds(periodicValueA.Speed, periodicValueB.Speed);
        }

        private bool AreSimilarSpeeds(double speedA, double speedB)
        {
            return AreSimilar(speedB, 0, KEpsilon)?
                AreSimilar(speedA, speedB, KEpsilon) :
                AreSimilar(1, speedA / speedB, KEpsilon);
        }

        private static ref readonly WalkModel.PeriodicValue GetValueOrDefault(WalkModel.PeriodicValue[] arr, int index) =>
            ref index < arr.Length? ref arr[index] : ref DefaultPeriodicValue;

        private static bool AreSimilar(double a, double b, double epsilon) => Math.Abs(a - b) < epsilon;
    }
}