// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.TiltEffect
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class TiltEffect : DependencyObject
  {
    private const double MaxAngle = 0.3;
    private const double MaxDepression = 25.0;
    private static Dictionary<DependencyObject, CacheMode> _originalCacheMode = new Dictionary<DependencyObject, CacheMode>();
    private static readonly TimeSpan TiltReturnAnimationDelay = TimeSpan.FromMilliseconds(200.0);
    private static readonly TimeSpan TiltReturnAnimationDuration = TimeSpan.FromMilliseconds(100.0);
    private static FrameworkElement currentTiltElement;
    private static Storyboard tiltReturnStoryboard;
    private static DoubleAnimation tiltReturnXAnimation;
    private static DoubleAnimation tiltReturnYAnimation;
    private static DoubleAnimation tiltReturnZAnimation;
    private static Point currentTiltElementCenter;
    private static bool wasPauseAnimation = false;
    public static readonly DependencyProperty IsTiltEnabledProperty = DependencyProperty.RegisterAttached("IsTiltEnabled", typeof (bool), typeof (TiltEffect), new PropertyMetadata(new PropertyChangedCallback(TiltEffect.OnIsTiltEnabledChanged)));
    public static readonly DependencyProperty SuppressTiltProperty = DependencyProperty.RegisterAttached("SuppressTilt", typeof (bool), typeof (TiltEffect), (PropertyMetadata) null);

    public static bool UseLogarithmicEase { get; set; }

    public static List<Type> TiltableItems { get; private set; }

    private TiltEffect()
    {
    }

    static TiltEffect()
    {
      TiltEffect.TiltableItems = new List<Type>()
      {
        typeof (ButtonBase),
        typeof (ListBoxItem),
        typeof (ListPicker),
        typeof (MenuItem),
        typeof (LongListSelector)
      };
    }

    public static bool GetIsTiltEnabled(DependencyObject source)
    {
      return (bool) source.GetValue(TiltEffect.IsTiltEnabledProperty);
    }

    public static void SetIsTiltEnabled(DependencyObject source, bool value)
    {
      source.SetValue(TiltEffect.IsTiltEnabledProperty, (object) value);
    }

    public static bool GetSuppressTilt(DependencyObject source)
    {
      return (bool) source.GetValue(TiltEffect.SuppressTiltProperty);
    }

    public static void SetSuppressTilt(DependencyObject source, bool value)
    {
      source.SetValue(TiltEffect.SuppressTiltProperty, (object) value);
    }

    private static void OnIsTiltEnabledChanged(
      DependencyObject target,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(target is FrameworkElement frameworkElement))
        return;
      if ((bool) args.NewValue)
        frameworkElement.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(TiltEffect.TiltEffect_ManipulationStarted);
      else
        frameworkElement.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(TiltEffect.TiltEffect_ManipulationStarted);
    }

    private static void TiltEffect_ManipulationStarted(
      object sender,
      ManipulationStartedEventArgs e)
    {
      TiltEffect.TryStartTiltEffect(sender as FrameworkElement, e);
    }

    private static void TiltEffect_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      TiltEffect.ContinueTiltEffect(sender as FrameworkElement, e);
    }

    private static void TiltEffect_ManipulationCompleted(
      object sender,
      ManipulationCompletedEventArgs e)
    {
      TiltEffect.EndTiltEffect(TiltEffect.currentTiltElement);
    }

    private static void TryStartTiltEffect(FrameworkElement source, ManipulationStartedEventArgs e)
    {
      foreach (FrameworkElement visualAncestor in (e.OriginalSource as FrameworkElement).GetVisualAncestors())
      {
        foreach (Type tiltableItem in TiltEffect.TiltableItems)
        {
          if (tiltableItem.IsAssignableFrom(visualAncestor.GetType()))
          {
            FrameworkElement frameworkElement = !(visualAncestor.ReadLocalValue(TiltEffect.SuppressTiltProperty) is bool) ? visualAncestor.GetVisualAncestors().FirstOrDefault<FrameworkElement>((Func<FrameworkElement, bool>) (x => x.ReadLocalValue(TiltEffect.SuppressTiltProperty) is bool)) : visualAncestor;
            if (frameworkElement == null || !(bool) frameworkElement.ReadLocalValue(TiltEffect.SuppressTiltProperty))
            {
              if (tiltableItem == typeof (LongListSelector))
              {
                TiltEffect.StartTiltEffectOnLLS((LongListSelector) visualAncestor, e);
                return;
              }
              FrameworkElement child = VisualTreeHelper.GetChild((DependencyObject) visualAncestor, 0) as FrameworkElement;
              FrameworkElement manipulationContainer = e.ManipulationContainer as FrameworkElement;
              if (child == null || manipulationContainer == null)
                return;
              Point touchPoint = manipulationContainer.TransformToVisual((UIElement) child).Transform(e.ManipulationOrigin);
              Point centerPoint = new Point(child.ActualWidth / 2.0, child.ActualHeight / 2.0);
              Point centerToCenterDelta = TiltEffect.GetCenterToCenterDelta(child, source);
              TiltEffect.BeginTiltEffect(child, touchPoint, centerPoint, centerToCenterDelta);
              return;
            }
          }
        }
      }
    }

    private static void StartTiltEffectOnLLS(LongListSelector lls, ManipulationStartedEventArgs e)
    {
      FrameworkElement node = (FrameworkElement) e.OriginalSource;
      ContentPresenter[] contentPresenterArray = new ContentPresenter[2];
      int num = 0;
      do
      {
        if (node is ContentPresenter contentPresenter)
          contentPresenterArray[num++ % 2] = contentPresenter;
        node = node.GetVisualParent();
      }
      while (node != lls && node.GetType() != typeof (ViewportControl));
      if (node == lls || num < 2 || !(contentPresenterArray[num % 2].GetVisualChildren().FirstOrDefault<DependencyObject>() is FrameworkElement frameworkElement) || frameworkElement.ReadLocalValue(TiltEffect.SuppressTiltProperty) is bool flag && flag)
        return;
      Point touchPoint = (e.ManipulationContainer as FrameworkElement).TransformToVisual((UIElement) frameworkElement).Transform(e.ManipulationOrigin);
      Point centerPoint = new Point(frameworkElement.ActualWidth / 2.0, frameworkElement.ActualHeight / 2.0);
      TiltEffect.BeginTiltEffect(frameworkElement, touchPoint, centerPoint, new Point(0.0, 0.0));
    }

    private static Point GetCenterToCenterDelta(
      FrameworkElement element,
      FrameworkElement container)
    {
      Point point1 = new Point(element.ActualWidth / 2.0, element.ActualHeight / 2.0);
      Point point2 = !(container is PhoneApplicationFrame applicationFrame) ? new Point(container.ActualWidth / 2.0, container.ActualHeight / 2.0) : ((applicationFrame.Orientation & PageOrientation.Landscape) != PageOrientation.Landscape ? new Point(container.ActualWidth / 2.0, container.ActualHeight / 2.0) : new Point(container.ActualHeight / 2.0, container.ActualWidth / 2.0));
      Point point3 = element.TransformToVisual((UIElement) container).Transform(point1);
      return new Point(point2.X - point3.X, point2.Y - point3.Y);
    }

    private static void BeginTiltEffect(
      FrameworkElement element,
      Point touchPoint,
      Point centerPoint,
      Point centerDelta)
    {
      if (TiltEffect.tiltReturnStoryboard != null)
        TiltEffect.StopTiltReturnStoryboardAndCleanup();
      if (!TiltEffect.PrepareControlForTilt(element, centerDelta))
        return;
      TiltEffect.currentTiltElement = element;
      TiltEffect.currentTiltElementCenter = centerPoint;
      TiltEffect.PrepareTiltReturnStoryboard(element);
      TiltEffect.ApplyTiltEffect(TiltEffect.currentTiltElement, touchPoint, TiltEffect.currentTiltElementCenter);
    }

    private static bool PrepareControlForTilt(FrameworkElement element, Point centerDelta)
    {
      if (element.Projection != null || element.RenderTransform != null && element.RenderTransform.GetType() != typeof (MatrixTransform))
        return false;
      TiltEffect._originalCacheMode[(DependencyObject) element] = element.CacheMode;
      element.CacheMode = (CacheMode) new BitmapCache();
      element.RenderTransform = (Transform) new TranslateTransform()
      {
        X = centerDelta.X,
        Y = centerDelta.Y
      };
      element.Projection = (Projection) new PlaneProjection()
      {
        GlobalOffsetX = (-1.0 * centerDelta.X),
        GlobalOffsetY = (-1.0 * centerDelta.Y)
      };
      element.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(TiltEffect.TiltEffect_ManipulationDelta);
      element.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(TiltEffect.TiltEffect_ManipulationCompleted);
      return true;
    }

    private static void RevertPrepareControlForTilt(FrameworkElement element)
    {
      element.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(TiltEffect.TiltEffect_ManipulationDelta);
      element.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(TiltEffect.TiltEffect_ManipulationCompleted);
      element.Projection = (Projection) null;
      element.RenderTransform = (Transform) null;
      CacheMode cacheMode;
      if (TiltEffect._originalCacheMode.TryGetValue((DependencyObject) element, out cacheMode))
      {
        element.CacheMode = cacheMode;
        TiltEffect._originalCacheMode.Remove((DependencyObject) element);
      }
      else
        element.CacheMode = (CacheMode) null;
    }

    private static void PrepareTiltReturnStoryboard(FrameworkElement element)
    {
      if (TiltEffect.tiltReturnStoryboard == null)
      {
        TiltEffect.tiltReturnStoryboard = new Storyboard();
        TiltEffect.tiltReturnStoryboard.Completed += new EventHandler(TiltEffect.TiltReturnStoryboard_Completed);
        TiltEffect.tiltReturnXAnimation = new DoubleAnimation();
        Storyboard.SetTargetProperty((Timeline) TiltEffect.tiltReturnXAnimation, new PropertyPath((object) PlaneProjection.RotationXProperty));
        TiltEffect.tiltReturnXAnimation.BeginTime = new TimeSpan?(TiltEffect.TiltReturnAnimationDelay);
        TiltEffect.tiltReturnXAnimation.To = new double?(0.0);
        TiltEffect.tiltReturnXAnimation.Duration = (Duration) TiltEffect.TiltReturnAnimationDuration;
        TiltEffect.tiltReturnYAnimation = new DoubleAnimation();
        Storyboard.SetTargetProperty((Timeline) TiltEffect.tiltReturnYAnimation, new PropertyPath((object) PlaneProjection.RotationYProperty));
        TiltEffect.tiltReturnYAnimation.BeginTime = new TimeSpan?(TiltEffect.TiltReturnAnimationDelay);
        TiltEffect.tiltReturnYAnimation.To = new double?(0.0);
        TiltEffect.tiltReturnYAnimation.Duration = (Duration) TiltEffect.TiltReturnAnimationDuration;
        TiltEffect.tiltReturnZAnimation = new DoubleAnimation();
        Storyboard.SetTargetProperty((Timeline) TiltEffect.tiltReturnZAnimation, new PropertyPath((object) PlaneProjection.GlobalOffsetZProperty));
        TiltEffect.tiltReturnZAnimation.BeginTime = new TimeSpan?(TiltEffect.TiltReturnAnimationDelay);
        TiltEffect.tiltReturnZAnimation.To = new double?(0.0);
        TiltEffect.tiltReturnZAnimation.Duration = (Duration) TiltEffect.TiltReturnAnimationDuration;
        if (TiltEffect.UseLogarithmicEase)
        {
          TiltEffect.tiltReturnXAnimation.EasingFunction = (IEasingFunction) new TiltEffect.LogarithmicEase();
          TiltEffect.tiltReturnYAnimation.EasingFunction = (IEasingFunction) new TiltEffect.LogarithmicEase();
          TiltEffect.tiltReturnZAnimation.EasingFunction = (IEasingFunction) new TiltEffect.LogarithmicEase();
        }
        TiltEffect.tiltReturnStoryboard.Children.Add((Timeline) TiltEffect.tiltReturnXAnimation);
        TiltEffect.tiltReturnStoryboard.Children.Add((Timeline) TiltEffect.tiltReturnYAnimation);
        TiltEffect.tiltReturnStoryboard.Children.Add((Timeline) TiltEffect.tiltReturnZAnimation);
      }
      Storyboard.SetTarget((Timeline) TiltEffect.tiltReturnXAnimation, (DependencyObject) element.Projection);
      Storyboard.SetTarget((Timeline) TiltEffect.tiltReturnYAnimation, (DependencyObject) element.Projection);
      Storyboard.SetTarget((Timeline) TiltEffect.tiltReturnZAnimation, (DependencyObject) element.Projection);
    }

    private static void ContinueTiltEffect(FrameworkElement element, ManipulationDeltaEventArgs e)
    {
      if (!(e.ManipulationContainer is FrameworkElement manipulationContainer) || element == null)
        return;
      Point point = manipulationContainer.TransformToVisual((UIElement) element).Transform(e.ManipulationOrigin);
      if (!new Rect(0.0, 0.0, TiltEffect.currentTiltElement.ActualWidth, TiltEffect.currentTiltElement.ActualHeight).Contains(point))
        TiltEffect.PauseTiltEffect();
      else
        TiltEffect.ApplyTiltEffect(TiltEffect.currentTiltElement, point, TiltEffect.currentTiltElementCenter);
    }

    private static void EndTiltEffect(FrameworkElement element)
    {
      if (element != null)
      {
        element.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(TiltEffect.TiltEffect_ManipulationCompleted);
        element.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(TiltEffect.TiltEffect_ManipulationDelta);
      }
      if (TiltEffect.tiltReturnStoryboard != null)
      {
        TiltEffect.wasPauseAnimation = false;
        if (TiltEffect.tiltReturnStoryboard.GetCurrentState() == ClockState.Active)
          return;
        TiltEffect.tiltReturnStoryboard.Begin();
      }
      else
        TiltEffect.StopTiltReturnStoryboardAndCleanup();
    }

    private static void TiltReturnStoryboard_Completed(object sender, EventArgs e)
    {
      if (TiltEffect.wasPauseAnimation)
        TiltEffect.ResetTiltEffect(TiltEffect.currentTiltElement);
      else
        TiltEffect.StopTiltReturnStoryboardAndCleanup();
    }

    private static void ResetTiltEffect(FrameworkElement element)
    {
      PlaneProjection projection = element.Projection as PlaneProjection;
      projection.RotationY = 0.0;
      projection.RotationX = 0.0;
      projection.GlobalOffsetZ = 0.0;
    }

    private static void StopTiltReturnStoryboardAndCleanup()
    {
      if (TiltEffect.tiltReturnStoryboard != null)
        TiltEffect.tiltReturnStoryboard.Stop();
      if (TiltEffect.currentTiltElement == null)
        return;
      TiltEffect.RevertPrepareControlForTilt(TiltEffect.currentTiltElement);
      TiltEffect.currentTiltElement = (FrameworkElement) null;
    }

    private static void PauseTiltEffect()
    {
      if (TiltEffect.tiltReturnStoryboard == null || TiltEffect.wasPauseAnimation)
        return;
      TiltEffect.tiltReturnStoryboard.Stop();
      TiltEffect.wasPauseAnimation = true;
      TiltEffect.tiltReturnStoryboard.Begin();
    }

    private static void ResetTiltReturnStoryboard()
    {
      TiltEffect.tiltReturnStoryboard.Stop();
      TiltEffect.wasPauseAnimation = false;
    }

    private static void ApplyTiltEffect(
      FrameworkElement element,
      Point touchPoint,
      Point centerPoint)
    {
      TiltEffect.ResetTiltReturnStoryboard();
      Point point = new Point(Math.Min(Math.Max(touchPoint.X / (centerPoint.X * 2.0), 0.0), 1.0), Math.Min(Math.Max(touchPoint.Y / (centerPoint.Y * 2.0), 0.0), 1.0));
      if (double.IsNaN(point.X) || double.IsNaN(point.Y))
        return;
      double num1 = Math.Abs(point.X - 0.5);
      double num2 = Math.Abs(point.Y - 0.5);
      double num3 = (double) -Math.Sign(point.X - 0.5);
      double num4 = (double) Math.Sign(point.Y - 0.5);
      double num5 = num1 + num2;
      double num6 = num1 + num2 > 0.0 ? num1 / (num1 + num2) : 0.0;
      double num7 = num5 * 0.3 * 180.0 / Math.PI;
      double num8 = (1.0 - num5) * 25.0;
      PlaneProjection projection = element.Projection as PlaneProjection;
      projection.RotationY = num7 * num6 * num3;
      projection.RotationX = num7 * (1.0 - num6) * num4;
      projection.GlobalOffsetZ = -num8;
    }

    private class LogarithmicEase : EasingFunctionBase
    {
      protected override double EaseInCore(double normalizedTime)
      {
        return Math.Log(normalizedTime + 1.0) / 0.693147181;
      }
    }
  }
}
