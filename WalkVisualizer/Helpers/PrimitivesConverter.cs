namespace WalkVisualizer
{
    public static class PrimitivesConverter
    {
        public static System.Windows.Point ToWpfPoint(this WalkEngine.Point point) => 
            new System.Windows.Point(point.X, point.Y);
        
        public static System.Windows.Size ToWpfSize(this WalkEngine.Size size) => 
            new System.Windows.Size(size.Width, size.Height);
    }
}