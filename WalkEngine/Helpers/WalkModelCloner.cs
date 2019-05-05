using System.Linq;

namespace WalkEngine
{
    public static class WalkModelCloner
    {
        public static WalkModel Clone(this WalkModel walkModel)
        {
            var clone = new WalkModel(walkModel.Left, walkModel.Right);
            CloneLeg(ref clone.Left);
            CloneLeg(ref clone.Right);
            return clone;
        }

        private static void CloneLeg(ref WalkModel.LegModel leg)
        {
            CloneArray(ref leg.Top.PeriodicValues);
            CloneArray(ref leg.Bottom.PeriodicValues);
        }

        private static void CloneArray(ref WalkModel.PeriodicValue[] arr) => arr = arr.ToArray();
    }
}