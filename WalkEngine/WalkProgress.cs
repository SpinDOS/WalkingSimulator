using System;

namespace WalkEngine
{
    public sealed partial class WalkProgress
    {
        private const double TimeStep = 1e-3;
        
        private LegsPair _legs;

        public WalkProgress(WalkModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            CorrectPositionForTime(0);
            AbsolutePosition = new Point(0, AbsolutePosition.Y);
        }
        
        public WalkModel Model { get; }

        public LegsPair Legs
        {
            get => _legs;
            private set => _legs = value;
        }

        public Point AbsolutePosition { get; private set; }
        
        public double Time { get; private set; }

        public bool IsAlive { get; private set; }

        public double Distance => AbsolutePosition.X;

        public bool MoveForward()
        {
            if (IsAlive)
                CorrectPositionForTime(Time + TimeStep);
            return IsAlive;
        }

        private void CorrectPositionForTime(double time)
        {
            var legs = new LegsPair();
            GetLeftLegPosition(time, ref legs.Left);
            GetRightLegPosition(time, ref legs.Right);
            
            IsAlive = IsHeadBetweenLegs(legs) && IsValidAngles(legs) && !AreLegsIntersecting(legs);
            
            var deltaX = GetMovementX(_legs, legs, out var newY);
            AbsolutePosition = new Point(AbsolutePosition.X + deltaX, newY);
            Time = time;
            Legs = legs;
        }

        private void GetLeftLegPosition(double time, ref LegPosition result) =>
            GetLegPosition(WalkModelSizes.LegTop.Width / 2, time, Model.Left, ref result);

        private void GetRightLegPosition(double time, ref LegPosition result) =>
            GetLegPosition(WalkModelSizes.Head.Width - WalkModelSizes.LegTop.Width / 2, time, Model.Right, ref result);
        
        private static void GetLegPosition(double topCenterX, double time, in WalkModel.LegModel legModel, ref LegPosition result)
        {
            result.Top.Angle = legModel.Top.GetAngle(time);
            result.Top.TopCenter = new Point(topCenterX, WalkModelSizes.Head.Height);
            result.Top.BottomCenter = WalkProgressHelper.RotateByAngle(
                result.Top.TopCenter, 0, WalkModelSizes.LegTop.Height, result.Top.Angle);

            var bottomCenterTop = result.Top.BottomCenter;
            var topHalfWidth = WalkModelSizes.LegBottom.WidthTop / 2;
            var height = WalkModelSizes.LegBottom.Height;

            result.Bottom.Angle = legModel.Bottom.GetAngle(time);
            var absoluteBottomAngle = result.Top.Angle + result.Bottom.Angle;
            result.Bottom.TopLeft = RotateBottomPoint(-topHalfWidth, 0);
            result.Bottom.TopRight = RotateBottomPoint(topHalfWidth, 0);
            result.Bottom.BottomLeft = RotateBottomPoint(-topHalfWidth, height);
            result.Bottom.BottomRight = RotateBottomPoint(WalkModelSizes.LegBottom.WidthBottom - topHalfWidth, height);
            
            Point RotateBottomPoint(double x, double y) => 
                WalkProgressHelper.RotateByAngle(bottomCenterTop, x, y, absoluteBottomAngle);
        }

        private static double GetMovementX(in LegsPair from, in LegsPair to, out double bottomY)
        {
            var bottomLeftPoint = WalkProgressHelper.GetBottomPoint(to.Left, out var leftSource);
            var bottomRightPoint = WalkProgressHelper.GetBottomPoint(to.Right, out var rightSource);
            bottomY = Math.Max(bottomLeftPoint.Y, bottomRightPoint.Y);
            return bottomLeftPoint.Y > bottomRightPoint.Y?
                WalkProgressHelper.GetMovementByLeg(from.Left, to.Left, leftSource) :
                WalkProgressHelper.GetMovementByLeg(from.Right, to.Right, rightSource);
        }

        private static bool IsHeadBetweenLegs(in LegsPair legs)
        {
            var headCenterX = WalkModelSizes.Head.Width / 2;
            return legs.Left.Bottom.BottomLeft.X <= headCenterX && headCenterX <= legs.Right.Bottom.BottomRight.X;
        }

        private static bool IsValidAngles(in LegsPair legs) => IsValidAngles(legs.Left) && IsValidAngles(legs.Right);
        
        private static bool IsValidAngles(in LegPosition leg)
        {
            return Math.Abs(leg.Top.Angle) <= Math.PI / 3 && Math.Abs(leg.Bottom.Angle) <= Math.PI / 4;
        }

        private static bool AreLegsIntersecting(in LegsPair legs)
        {
            var halfWidthOfTOpLeg = WalkModelSizes.LegTop.Width / 2;
            
            var rightmostXOfLeftLeg = Math.Max(
                legs.Left.Top.BottomCenter.X + halfWidthOfTOpLeg, // approximation for fast check
                Math.Max(legs.Left.Bottom.TopRight.X, legs.Left.Bottom.BottomRight.X));
            
            var leftmostXOfRightLeg = Math.Min(
                legs.Right.Top.BottomCenter.X - halfWidthOfTOpLeg, // approximation for fast check
                Math.Min(legs.Right.Bottom.TopLeft.X, legs.Right.Bottom.BottomLeft.X));

            if (rightmostXOfLeftLeg < leftmostXOfRightLeg)
                return false;
            
            Span<LineSection> lines = stackalloc LineSection[6];
            var leftLines = lines.Slice(0, 3);
            var rightLines = lines.Slice(3, 3);
            
            WalkProgressHelper.FillRightLinesOfLeftLeg(leftLines, legs.Left);
            WalkProgressHelper.FillLeftLinesOfRightLeg(rightLines, legs.Right);

            foreach (var leftLine in leftLines)
                foreach (var rightLine in rightLines)
                    if (leftLine.Intersects(rightLine))
                        return true;

            return false;
        }
        
        public struct LegsPair
        {
            public LegPosition Left, Right;
        }

        public struct LegPosition
        {
            public LegTopPartPosition Top;
            public LegBottomPartPosition Bottom;
        }
        
        public struct LegTopPartPosition
        {
            public double Angle;
            public Point TopCenter, BottomCenter;
        }
        
        public struct LegBottomPartPosition
        {
            public double Angle;
            public Point TopLeft, TopRight, BottomLeft, BottomRight;
        }
    }
}