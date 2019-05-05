namespace WalkEngine
{
    public interface ICrossingoverOperator
    {
        WalkModel CombineModels(WalkModel parentA, WalkModel parentB);
    }
}