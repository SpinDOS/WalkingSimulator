using System;

namespace WalkEngine
{
    partial class WalkProgress
    {
        private readonly struct LineSection
        {
            private const double Epsilon = 1e-10;
            private readonly double k, b, x1, x2;

            public LineSection(Point p1, Point p2)
            {
                if (AreEqual(p1.X, p2.X))
                {
                    k = double.PositiveInfinity;
                    b = p1.X;
                    x1 = p1.Y;
                    x2 = p2.Y;
                }
                else
                {
                    k = (p2.Y - p1.Y) / (p2.X - p1.X);
                    b = p1.Y - k * p1.X;
                    x1 = p1.X;
                    x2 = p2.X;
                }

                if (x1 > x2)
                    (x1, x2) = (x2, x1);
            }

            private bool IsVertical => double.IsPositiveInfinity(k);

            public bool Intersects(LineSection another)
            {
                if ((IsVertical && another.IsVertical) || AreEqual(k, another.k))
                    return AreEqual(b, another.b) && (IsBetween(another.x1, x1, x2) || IsBetween(another.x2, x1, x2));
                if (IsVertical)
                    return IntersectVertical(this, another);
                if (another.IsVertical)
                    return IntersectVertical(another, this);

                var intersectionX = (another.b - b) / (k - another.k);
                return IsBetween(intersectionX, x1, x2) && IsBetween(intersectionX, another.x1, another.x2);
            }

            private double GetY(double x) => b + k * x;

            private static bool IntersectVertical(LineSection vertical, LineSection another) => 
                IsBetween(another.GetY(vertical.b), vertical.x1, vertical.x2);

            private static bool IsBetween(double a, double min, double max) => a >= min && a <= max;
            private static bool AreEqual(double a, double b) => Math.Abs(a - b) < Epsilon;
        }
    }
}