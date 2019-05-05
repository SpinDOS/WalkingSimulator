using System;
using System.Linq;

namespace WalkEngine
{
    public sealed class WalkModelRandomizer
    {
        [ThreadStatic]
        private static Random _random;

        public static Random Random => _random ?? (_random = GetRandom());
        
        public MinMaxAngleConfig TopAngleConfig { get; set; } = new MinMaxAngleConfig(Math.PI / 3);
        
        public MinMaxAngleConfig BottomAngleConfig { get; set; } = new MinMaxAngleConfig(Math.PI / 4);
        
        public double T0 { get; set; } = 2 * Math.PI;
        public double Speed { get; set; } = 1.5 * Math.PI;
        public int MinPeriodicValuesCount { get; set; } = 1;
        public int MaxPeriodicValuesCount { get; set; } = 3;
        
        public ParallelQuery<WalkModel> GetGeneration(int count) => 
            ParallelEnumerable.Range(0, count).Select(_ => GetWalkModel());

        public WalkModel GetWalkModel() => 
            new WalkModel(GetLegModel(), GetLegModel());
        
        public WalkModel.LegModel GetLegModel() => 
            new WalkModel.LegModel(
                GetLegPartModel(LegPartLocation.Top), 
                GetLegPartModel(LegPartLocation.Bottom));

        public WalkModel.LegPartModel GetLegPartModel(LegPartLocation location) => 
            new WalkModel.LegPartModel(
                GetLegMinAngle(location),
                GetLegMaxAngle(location), 
                Enumerable.Range(0, Random.Next(MinPeriodicValuesCount, MaxPeriodicValuesCount))
                    .Select(_ => GetPeriodicValue()));

        public WalkModel.PeriodicValue GetPeriodicValue() =>
            new WalkModel.PeriodicValue(GetK(), GetT0(), GetSpeed());
        
        public double GetLegMinAngle(LegPartLocation location) => 
            GetMinMaxAngleConfig(location).MinAngle * Random.NextDouble();

        public double GetLegMaxAngle(LegPartLocation location) => 
            GetMinMaxAngleConfig(location).MaxAngle * Random.NextDouble();

        public double GetK() => 1 - Random.NextDouble();

        public double GetT0() => T0 * Random.NextDouble();

        public double GetSpeed() => Speed * (1 - Random.NextDouble());

        public static Random GetRandom() => new Random(Guid.NewGuid().GetHashCode());

        private MinMaxAngleConfig GetMinMaxAngleConfig(LegPartLocation location)
        {
            switch (location)
            {
                case LegPartLocation.Top:
                    return TopAngleConfig;
                case LegPartLocation.Bottom:
                    return BottomAngleConfig;
                default:
                    throw new InvalidOperationException("Invalid leg part location: " + location);
            }
        }
        
        public enum LegPartLocation
        {
            Top,
            Bottom,
        }

        public struct MinMaxAngleConfig
        {
            public double MinAngle { get; set; }
            public double MaxAngle { get; set; }

            public MinMaxAngleConfig(double minAngle, double maxAngle)
            {
                MinAngle = minAngle;
                MaxAngle = maxAngle;
            }
            
            public MinMaxAngleConfig(double minMaxAngle) : this (-minMaxAngle, minMaxAngle) { }
        }
    }
}