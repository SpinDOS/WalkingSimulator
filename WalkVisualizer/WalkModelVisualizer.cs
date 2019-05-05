using System;
using System.Windows;
using System.Windows.Media.Animation;
using WalkEngine;

namespace WalkVisualizer
{
    public sealed partial class WalkModelVisualizer : FrameworkElement
    {
        private static readonly DependencyProperty CurrentTimeProperty = 
            DependencyProperty.Register(
                "CurrentTime", 
                typeof(double), 
                typeof(WalkModelVisualizer), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnCurrentTimeChanged));

        public static readonly DependencyProperty GenerationTimeProperty = 
            DependencyProperty.RegisterAttached(
                "GenerationTime", 
                typeof(double), 
                typeof(WalkModelVisualizer), 
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.Inherits, StartAnimation), 
                CheckNonNegativeFinite);
        
        public static readonly DependencyProperty TargetDistanceProperty = 
            DependencyProperty.RegisterAttached(
                "TargetDistance", 
                typeof(double), 
                typeof(WalkModelVisualizer), 
                new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits), 
                CheckNonNegativeFinite);

        public static readonly DependencyProperty AnimationDurationProperty = 
            DependencyProperty.RegisterAttached(
                "AnimationDuration", 
                typeof(TimeSpan), 
                typeof(WalkModelVisualizer), 
                new FrameworkPropertyMetadata(TimeSpan.FromSeconds(5), FrameworkPropertyMetadataOptions.Inherits, StartAnimation), 
                CheckNonNegativeTimeSpan);
        
        public static readonly DependencyProperty WalkModelDescriptionProperty = 
            DependencyProperty.Register(
                nameof(WalkModelDescription), 
                typeof(WalkModelDescription), 
                typeof(WalkModelVisualizer), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, StartAnimation));
        
        static WalkModelVisualizer()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(WalkModelVisualizer), new PropertyMetadata(true));
        }
        
        public static double GetGenerationTime(UIElement element) => 
            (double)element.GetValue(GenerationTimeProperty);
        
        public static void SetGenerationTime(UIElement element, double value) => 
            element.SetValue(GenerationTimeProperty, value);
        
        public static TimeSpan GetAnimationDuration(UIElement element) => 
            (TimeSpan)element.GetValue(AnimationDurationProperty);
        
        public static void SetAnimationDuration(UIElement element, TimeSpan value) => 
            element.SetValue(AnimationDurationProperty, value);
        
        public static double GetTargetDistance(UIElement element) => 
            (double)element.GetValue(TargetDistanceProperty);
        
        public static void SetTargetDistance(UIElement element, double value) => 
            element.SetValue(TargetDistanceProperty, value);

        private WalkProgress _walkProgress;

        public WalkModelDescription WalkModelDescription
        {
            get => (WalkModelDescription) GetValue(WalkModelDescriptionProperty);
            set => SetValue(WalkModelDescriptionProperty, value);
        }

        private static void OnCurrentTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var walkModelVisualizer = (WalkModelVisualizer) sender;
            var walkProgress = walkModelVisualizer._walkProgress;
            if (walkProgress == null)
                return;
            
            var targetTime = Math.Min((double) args.NewValue, walkModelVisualizer.WalkModelDescription.Time);
            while (walkProgress.Time < targetTime && walkProgress.MoveForward())
            {
            }
        }

        private static void StartAnimation(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {   
            if (!(sender is WalkModelVisualizer walkModelVisualizer))
                return;
            
            var targetTime = GetGenerationTime(walkModelVisualizer);
            var duration = GetAnimationDuration(walkModelVisualizer);
            var modelDescription = walkModelVisualizer.WalkModelDescription;

            if (double.IsNaN(targetTime) || modelDescription == null || duration == TimeSpan.Zero)
            {
                walkModelVisualizer._walkProgress = null;
                return;
            }
            
            walkModelVisualizer._walkProgress = new WalkProgress(modelDescription.WalkModel);

            var freezeAfterEndFactor = 1 + TimeSpan.FromSeconds(0.5).Ticks / Math.Max(duration.Ticks, 1.0);
            var animation = new DoubleAnimation(0.0, targetTime * freezeAfterEndFactor, duration);
            walkModelVisualizer.BeginAnimation(CurrentTimeProperty, animation);
        }

        private static bool CheckNonNegativeFinite(object generationTime) =>
            generationTime is double num && (double.IsNaN(num) || (num >= 0.0 && !double.IsInfinity(num)));

        private static bool CheckNonNegativeTimeSpan(object animationTime) => 
            animationTime is TimeSpan timeSpan && timeSpan >= TimeSpan.Zero;
    }
}