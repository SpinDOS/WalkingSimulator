using System;
using System.Collections.Generic;
using System.Linq;

namespace WalkEngine
{
    public sealed class WalkModel
    {
        public LegModel Left, Right;
        
        public WalkModel() { }
        
        public WalkModel(LegModel left, LegModel right)
        {
            Left = left;
            Right = right;
        }
        
        public struct LegModel
        {
            public LegPartModel Top, Bottom;

            public LegModel(LegPartModel top, LegPartModel bottom)
            {
                Top = top;
                Bottom = bottom;
            }
        }
        
        public struct LegPartModel
        {
            public PeriodicValue[] PeriodicValues;
            public double MinAngle, MaxAngle;
            
            public LegPartModel(double minAngle, double maxAngle, IEnumerable<PeriodicValue> periodicValues)
            {
                if (minAngle > maxAngle)
                    throw new ArgumentException("Min angle must be less or equal to max angle");
                
                MinAngle = minAngle;
                MaxAngle = maxAngle;
                
                PeriodicValues = periodicValues?.ToArray() ??
                                 throw new ArgumentNullException(nameof(periodicValues));
                
                if (PeriodicValues.Length == 0)
                    throw new ArgumentException("Periodic values are empty", nameof(periodicValues));
            }

            public double GetAngle(double time)
            {
                double value = 0, maxPossibleValue = 0;
                foreach (var periodicValue in PeriodicValues)
                {
                    value += periodicValue.K * (1 + Math.Sin(periodicValue.T0 + periodicValue.Speed * time));
                    maxPossibleValue += Math.Abs(periodicValue.K);
                }

                return MinAngle + (MaxAngle - MinAngle) * value / (2 * maxPossibleValue);
            }
        }
        
        public struct PeriodicValue
        {
            public double K, T0, Speed;

            public PeriodicValue(double k, double t0, double speed)
            {
                K = k;
                T0 = t0;
                Speed = speed;
            }
        }
    }
}