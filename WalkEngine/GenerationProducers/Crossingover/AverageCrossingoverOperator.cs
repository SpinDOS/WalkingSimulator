namespace WalkEngine
{
    public sealed class AverageCrossingoverOperator : CrossingoverOperatorBase
    {
        protected override double GetDouble(double parentA, double parentB) => (parentA + parentB) / 2;

        protected override void VisitPeriodicValue(
            in WalkModel.PeriodicValue singleParent,
            out WalkModel.PeriodicValue result)
        {
            result = new WalkModel.PeriodicValue(singleParent.K / 2, singleParent.T0, singleParent.Speed);
        }
    }
}