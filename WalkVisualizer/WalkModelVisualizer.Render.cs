using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WalkEngine;
using Point = System.Windows.Point;

namespace WalkVisualizer
{
    partial class WalkModelVisualizer
    {
        private static readonly Pen 
            GroundLinePen = Freeze(new Pen(Brushes.Black, 4)), 
            StrokeLinePen = Freeze(new Pen(Brushes.Black, 0.5)), 
            StartLinePen = Freeze(new Pen(Brushes.Green, 2)), 
            FinishLinePen = Freeze(new Pen(Brushes.Red, 2)), 
            AliveModelLinePen = Freeze(new Pen(Brushes.Black, 1)),
            DeadModelLinePen = Freeze(new Pen(Brushes.Red, 1));

        private static readonly Geometry LegBottomGeometry = Freeze(Geometry.Parse(
            $"M 0,0" + 
            $" L 0, {WalkModelSizes.LegBottom.Height}" + 
            $" L {WalkModelSizes.LegBottom.WidthBottom}, {WalkModelSizes.LegBottom.Height}" + 
            $" L {WalkModelSizes.LegBottom.WidthTop}, 0" + 
            $" Z"));
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var walkProgress = _walkProgress;
            DrawLabels(drawingContext, walkProgress);

            if (walkProgress == null)
                return;

            drawingContext.PushTransform(new TranslateTransform(0, ActualHeight));

            var width = ActualWidth;
            var translateX = GetTranslateXToCenterModel(walkProgress, width);
            DrawGround(drawingContext, translateX, width);
            
            drawingContext.PushTransform(new TranslateTransform(translateX, 0));
            
            DrawStartFinish(drawingContext, GetTargetDistance(this));
            DrawWalkProgress(drawingContext, walkProgress);
            
            drawingContext.Pop();
            drawingContext.Pop();
        }

        private void DrawLabels(DrawingContext drawingContext, WalkProgress walkProgress)
        {
            var typeFace = new Typeface(
                TextElement.GetFontFamily(this),
                TextElement.GetFontStyle(this),
                TextElement.GetFontWeight(this),
                TextElement.GetFontStretch(this));

            var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            
            var maxTime = WalkModelDescription?.Time ?? 0;
            var currentTime = walkProgress?.Time ?? maxTime;
            var timeText = $"Time: {currentTime:N2} / {maxTime:N2}";
            drawingContext.DrawText(CreateFormattedText(timeText), new Point(5, 0));

            var targetDistance = GetTargetDistance(this);
            var currentDistance = walkProgress?.Distance ?? WalkModelDescription?.Distance ?? 0;
            var distanceText = $"Distance: {currentDistance:N2} / {targetDistance:N2}";
            var formattedText = CreateFormattedText(distanceText);
            drawingContext.DrawText(formattedText, new Point(ActualWidth - formattedText.Width - 5, 0));

            FormattedText CreateFormattedText(string text) =>
                new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                    typeFace, 4, Brushes.Black, pixelsPerDip);
        }

        private static double GetTranslateXToCenterModel(WalkProgress walkProgress, double width)
        {
            var leg = walkProgress.Legs.Left;
            var leftmostModelX = Min(0, leg.Top.BottomCenter.X, leg.Bottom.BottomLeft.X, leg.Bottom.TopLeft.X);
            return Math.Max(50, width / 10) - (walkProgress.Distance + leftmostModelX);
        }

        private static void DrawGround(DrawingContext drawingContext, double translateX, double width)
        {
            drawingContext.DrawLine(GroundLinePen, new Point(0, 0), new Point(width, 0));
            
            var height = GroundLinePen.Thickness;
            const double Step = 10;

            for (var x = translateX % Step - Step; x < width + Step; x += Step)
                drawingContext.DrawLine(StrokeLinePen, new Point(x, 0), new Point(x, -height));
        }

        private static void DrawStartFinish(DrawingContext drawingContext, double targetDistance)
        {
            var height = GroundLinePen.Thickness;
            drawingContext.DrawLine(StartLinePen, new Point(0, 0), new Point(0, -height));
            
            if (!double.IsNaN(targetDistance) && !double.IsInfinity(targetDistance))
                drawingContext.DrawLine(FinishLinePen, new Point(targetDistance, 0), new Point(targetDistance, -height));
        }

        private static void DrawWalkProgress(DrawingContext drawingContext, WalkProgress walkProgress)
        {
            var pen = walkProgress.IsAlive? AliveModelLinePen : DeadModelLinePen;

            var translateY = -walkProgress.AbsolutePosition.Y - GroundLinePen.Thickness / 2;
            drawingContext.PushTransform(new TranslateTransform(walkProgress.AbsolutePosition.X, translateY));
            
            drawingContext.DrawRectangle(null, pen, new Rect(WalkModelSizes.Head.ToWpfSize()));
            
            var legs = walkProgress.Legs;
            DrawLeg(drawingContext, legs.Left, pen);
            DrawLeg(drawingContext, legs.Right, pen);
            
            drawingContext.Pop();
        }

        private static void DrawLeg(DrawingContext drawingContext, in WalkProgress.LegPosition leg, Pen pen)
        {
            const double RadianToDegree = -180 / Math.PI;
            
            var legTopPosition = leg.Top.TopCenter;
            var halfWidthTop = WalkModelSizes.LegTop.Width / 2;
            drawingContext.PushTransform(new TranslateTransform(legTopPosition.X - halfWidthTop, legTopPosition.Y));
            drawingContext.PushTransform(new RotateTransform(leg.Top.Angle * RadianToDegree, halfWidthTop, 0));
            drawingContext.DrawRectangle(null, pen, new Rect(WalkModelSizes.LegTop.ToWpfSize()));
            drawingContext.Pop();
            drawingContext.Pop();
            
            var legBottomPosition = leg.Top.BottomCenter;
            var halfWidthBottom = WalkModelSizes.LegBottom.WidthTop / 2;
            var bottomAngle = leg.Top.Angle + leg.Bottom.Angle;
            drawingContext.PushTransform(new TranslateTransform(legBottomPosition.X - halfWidthBottom, legBottomPosition.Y));
            drawingContext.PushTransform(new RotateTransform(bottomAngle * RadianToDegree, halfWidthBottom, 0));
            drawingContext.DrawGeometry(null, pen, LegBottomGeometry);
            drawingContext.Pop();
            drawingContext.Pop();
        }

        private static double Min(double x1, double x2, double x3, double x4) =>
            Math.Min(x1, Math.Min(x2, Math.Min(x3, x4)));

        private static T Freeze<T>(T freezable) where T : Freezable
        {
            freezable.Freeze();
            return freezable;
        }
    }
}