using System;

namespace WalkEngine
{
    public sealed class WalkModelMutator
    {
        public const double BasicChanceToMutate = 2e-2;
        
        public double ChanceToMutateLeg { get; set; } = BasicChanceToMutate;
        public double ChanceToMutateLegPart { get; set; } = BasicChanceToMutate;
        public double ChanceToMutateMinAngle { get; set; } = BasicChanceToMutate;
        public double ChanceToMutateMaxAngle { get; set; } = BasicChanceToMutate;
        public double ChanceToMutateK { get; set; } = BasicChanceToMutate;
        public double ChanceToMutateT0 { get; set; } = BasicChanceToMutate;
        public double ChanceToMutateSpeed { get; set; } = BasicChanceToMutate;
        public double ChanceToMutatePeriodicValuesLength { get; set; } = BasicChanceToMutate;
        
        public WalkModelRandomizer Randomizer { get; set; }

        public WalkModelMutator() : this(new WalkModelRandomizer()) { }
        public WalkModelMutator(WalkModelRandomizer randomizer) => Randomizer = randomizer;
        
        public void MutateModel(WalkModel walkModel)
        {
            MutateLeg(ref walkModel.Left);
            MutateLeg(ref walkModel.Right);
        }

        private void MutateLeg(ref WalkModel.LegModel leg)
        {
            if (MutateValue(ref leg, Randomizer.GetLegModel, ChanceToMutateLeg))
                return;
            
            MutateLegPart(ref leg.Top, WalkModelRandomizer.LegPartLocation.Top);
            MutateLegPart(ref leg.Bottom, WalkModelRandomizer.LegPartLocation.Bottom);
        }

        private void MutateLegPart(ref WalkModel.LegPartModel legPart, WalkModelRandomizer.LegPartLocation location)
        {
            if (MutateValue(ref legPart, Randomizer.GetLegPartModel, location, ChanceToMutateLegPart))
                return;
            
            MutateValue(ref legPart.MinAngle, Randomizer.GetLegMinAngle, location, ChanceToMutateMinAngle);
            MutateValue(ref legPart.MaxAngle, Randomizer.GetLegMaxAngle, location, ChanceToMutateMaxAngle);
            
            for (var i = 0; i < legPart.PeriodicValues.Length; i++)
                MutatePeriodicValue(ref legPart.PeriodicValues[i]);

            MutatePeriodicValuesLength(ref legPart.PeriodicValues, ChanceToMutatePeriodicValuesLength);
        }

        private void MutatePeriodicValue(ref WalkModel.PeriodicValue periodicValue)
        {
            MutateValue(ref periodicValue.K, Randomizer.GetK, ChanceToMutateK);
            MutateValue(ref periodicValue.T0, Randomizer.GetT0, ChanceToMutateT0);
            MutateValue(ref periodicValue.Speed, Randomizer.GetSpeed, ChanceToMutateSpeed);
        }

        private void MutatePeriodicValuesLength(ref WalkModel.PeriodicValue[] periodicValues, double chance)
        {
            if (WalkModelRandomizer.Random.NextDouble() >= chance)
                return;
            
            var length = periodicValues.Length;
            var add = length <= 1 || (length < 4 && WalkModelRandomizer.Random.Next(2) == 0);
            if (add)
            {
                Array.Resize(ref periodicValues, length + 1);
                periodicValues[length] = Randomizer.GetPeriodicValue();
            }
            else
            {
                periodicValues[WalkModelRandomizer.Random.Next(length)] = periodicValues[length - 1];
                Array.Resize(ref periodicValues, length - 1);
            }
        }

        private static bool MutateValue<T>(ref T target, Func<T> valueProducer, double chance)
        {
            if (WalkModelRandomizer.Random.NextDouble() >= chance)
                return false;
            
            target = valueProducer();
            return true;
        }
        
        private static bool MutateValue<T>(
            ref T target, 
            Func<WalkModelRandomizer.LegPartLocation, T> valueProducer, 
            WalkModelRandomizer.LegPartLocation location, 
            double chance)
        {
            if (WalkModelRandomizer.Random.NextDouble() >= chance)
                return false;
            
            target = valueProducer(location);
            return true;
        }
    }
}