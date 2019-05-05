using System;

namespace WalkEngine
{
    public sealed class WalkModelDescription
    {
        public WalkModelDescription(WalkProgress finalProgress)
        {            
            finalProgress = finalProgress ?? throw new ArgumentNullException(nameof(finalProgress));

            WalkModel = finalProgress.Model;
            Distance = finalProgress.Distance;
            IsAlive = finalProgress.IsAlive;
            Time = finalProgress.Time;
        }
        
        public WalkModel WalkModel { get; }
        
        public double Distance { get; }
        
        public bool IsAlive { get; }
        
        public double Time { get; }
    }
}