namespace WalkEngine
{
    public static class WalkModelSizes
    {
        public static readonly Size Head = new Size(45, 5);
        
        public static readonly Size LegTop = new Size(5, 35);
        
        public static readonly LegBottomSize LegBottom = new LegBottomSize(5, 15, 5);
        
        public readonly struct LegBottomSize
        {
            public LegBottomSize(double widthTop, double widthBottom, double height)
            {
                WidthTop = widthTop;
                WidthBottom = widthBottom;
                Height = height;
            }
            
            public double WidthTop { get; }
            public double WidthBottom { get; }
            public double Height { get; }
        }
    }
}