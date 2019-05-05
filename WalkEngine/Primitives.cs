namespace WalkEngine
{
    public readonly struct Point
    {
        public readonly double X, Y;
        
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public readonly struct Size
    {
        public readonly double Width, Height;

        public Size(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}