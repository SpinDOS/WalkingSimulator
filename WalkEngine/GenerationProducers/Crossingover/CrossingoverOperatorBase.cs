using System;

namespace WalkEngine
{
    public abstract class CrossingoverOperatorBase : ICrossingoverOperator
    {
        public virtual WalkModel CombineModels(WalkModel parentA, WalkModel parentB)
        {
            var result = new WalkModel();
            VisitLeg(parentA.Left, parentB.Left, ref result.Left);
            VisitLeg(parentA.Right, parentB.Right, ref result.Right);
            return result;
        }

        protected virtual void VisitLeg(
            in WalkModel.LegModel parentA,
            in WalkModel.LegModel parentB,
            ref WalkModel.LegModel result)
        {
            VisitLegPart(parentA.Top, parentA.Top, ref result.Top);
            VisitLegPart(parentB.Bottom, parentB.Bottom, ref result.Bottom);
        }

        protected virtual void VisitLegPart(
            in WalkModel.LegPartModel parentA,
            in WalkModel.LegPartModel parentB,
            ref WalkModel.LegPartModel result)
        {
            result.MinAngle = GetMinAngle(parentA.MinAngle, parentB.MinAngle);
            result.MaxAngle = GetMaxAngle(parentA.MaxAngle, parentB.MaxAngle);
            VisitPeriodicValues(parentA.PeriodicValues, parentB.PeriodicValues, out result.PeriodicValues);
        }

        protected virtual double GetMinAngle(double parentA, double parentB) => GetDouble(parentA, parentB);
        protected virtual double GetMaxAngle(double parentA, double parentB) => GetDouble(parentA, parentB);

        protected virtual void VisitPeriodicValues(
            WalkModel.PeriodicValue[] parentA,
            WalkModel.PeriodicValue[] parentB,
            out WalkModel.PeriodicValue[] result)
        {
            result = new WalkModel.PeriodicValue[GetPeriodicValuesCount(parentA, parentB)];
            for (var i = 0; i < result.Length; i++)
            {
                if (i >= parentA.Length)
                    VisitPeriodicValue(parentB[i], out result[i]);
                else if (i >= parentB.Length)
                    VisitPeriodicValue(parentA[i], out result[i]);
                else
                    VisitPeriodicValue(parentA[i], parentB[i], ref result[i]);
            }
        }

        protected virtual int GetPeriodicValuesCount(
            WalkModel.PeriodicValue[] parentA,
            WalkModel.PeriodicValue[] parentB)
        {
            return Math.Max(parentA.Length, parentB.Length);
        }
        
        protected virtual void VisitPeriodicValue(
            in WalkModel.PeriodicValue singleParent, 
            out WalkModel.PeriodicValue result)
        {
            result = singleParent;
        }

        protected virtual void VisitPeriodicValue(
            in WalkModel.PeriodicValue parentA, 
            in WalkModel.PeriodicValue parentB, 
            ref WalkModel.PeriodicValue result)
        {
            result.K = GetK(parentA.K, parentB.K);
            result.T0 = GetT0(parentA.T0, parentB.T0);
            result.Speed = GetSpeed(parentA.Speed, parentB.Speed);
        }

        protected virtual double GetK(double parentA, double parentB) => GetDouble(parentA, parentB);
        protected virtual double GetT0(double parentA, double parentB) => GetDouble(parentA, parentB);
        protected virtual double GetSpeed(double parentA, double parentB) => GetDouble(parentA, parentB);
        
        protected abstract double GetDouble(double parentA, double parentB);
    }
}