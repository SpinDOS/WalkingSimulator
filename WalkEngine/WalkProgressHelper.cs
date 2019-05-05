using System;

namespace WalkEngine
{
    partial class WalkProgress
    {
        private static class WalkProgressHelper
        {
            public static Point RotateByAngle(Point startPosition, double x, double y, double angle)
            {
                var cos = Math.Cos(angle);
                var sin = Math.Sin(angle);
                var newX = x * cos + y * sin;
                var newY = -x * sin + y * cos;
                return new Point(startPosition.X + newX, startPosition.Y + newY);
            }

            public static void FillRightLinesOfLeftLeg(Span<LineSection> lines, in LegPosition leftLeg)
            {
                var topPartHalfWidth = WalkModelSizes.LegTop.Width / 2;

                var leftLegPointMove = RotateByAngle(new Point(0, 0), topPartHalfWidth, 0, leftLeg.Top.Angle);
                lines[2] = new LineSection(leftLeg.Bottom.BottomLeft, leftLeg.Bottom.BottomRight);
                lines[1] = new LineSection(leftLeg.Bottom.BottomRight, leftLeg.Bottom.TopRight);
                lines[0] = GetLegLineWithOffset(leftLeg, leftLegPointMove);
            }

            public static void FillLeftLinesOfRightLeg(Span<LineSection> lines, in LegPosition rightLeg)
            {
                var topPartHalfWidth = WalkModelSizes.LegTop.Width / 2;

                var rightLegPointMove = RotateByAngle(new Point(0, 0), -topPartHalfWidth, 0, rightLeg.Top.Angle);
                lines[2] = new LineSection(rightLeg.Bottom.BottomRight, rightLeg.Bottom.BottomLeft);
                lines[1] = new LineSection(rightLeg.Bottom.BottomLeft, rightLeg.Bottom.TopLeft);
                lines[0] = GetLegLineWithOffset(rightLeg, rightLegPointMove);
            }

            public static Point GetBottomPoint(in LegPosition leg, out BottomPointSource source)
            {
                if (leg.Bottom.BottomRight.Y > leg.Bottom.BottomLeft.Y)
                {
                    source = BottomPointSource.BottomRight;
                    return leg.Bottom.BottomRight;
                }
                else
                {
                    source = BottomPointSource.BottomLeft;
                    return leg.Bottom.BottomLeft;
                }
            }

            public static double GetMovementByLeg(in LegPosition from, in LegPosition to, BottomPointSource pointSource)
            {
                switch (pointSource)
                {
                case BottomPointSource.BottomLeft:
                    return from.Bottom.BottomLeft.X - to.Bottom.BottomLeft.X;
                case BottomPointSource.BottomRight:
                    return from.Bottom.BottomRight.X - to.Bottom.BottomRight.X;
                default:
                    throw new ArgumentOutOfRangeException("Unexpected bottom point source: " + pointSource);
                }
            }

            private static LineSection GetLegLineWithOffset(LegPosition leg, Point offset) =>
                new LineSection(Add(leg.Top.TopCenter, offset), Add(leg.Top.BottomCenter, offset));

            private static Point Add(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);

            public enum BottomPointSource
            {
                BottomLeft,
                BottomRight,
            }
        }
    }
}