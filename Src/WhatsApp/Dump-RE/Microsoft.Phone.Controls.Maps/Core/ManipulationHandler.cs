// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.ManipulationHandler
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Windows;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class ManipulationHandler
  {
    private const ushort TapDistanceThreshold = 30;
    private const ushort DoubleTapTimeThreshold = 500;
    private const ushort TapAndHoldTimeThreshold = 2000;
    private const ushort DragThresholdForFlick = 3;
    private bool isTap;
    private bool isTapPending;
    private Timeout lastTap;
    private Point lastTapOrigin;
    private long lastTapTimestamp;
    private long tapStartTimestamp;
    private UIElement root;
    private Point lastTranslation;

    private static bool IsDoubleTap(
      Point previousPoint,
      long previousTimestamp,
      Point currentPoint,
      long currentTimestamp)
    {
      return !PhysicsHelper.ExceedsThreshold(TimeSpan.FromTicks(currentTimestamp - previousTimestamp), 500L) && !PhysicsHelper.ExceedsThreshold(PhysicsHelper.Delta(previousPoint, currentPoint), 30L);
    }

    public event EventHandler<GestureEventArgs> Tap;

    public event EventHandler<GestureEventArgs> DoubleTap;

    public event EventHandler<GestureEventArgs> TapAndHold;

    public event EventHandler<GestureEventArgs> Pan;

    public event EventHandler<GestureEventArgs> Flick;

    public event EventHandler<GestureEventArgs> Pinch;

    public event EventHandler<GestureEventArgs> Stretch;

    public ManipulationHandler(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.OnManipulationStarted);
      element.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.OnManipulationCompleted);
      element.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.OnManipulationDelta);
      this.root = element;
    }

    internal void Unload()
    {
      if (this.lastTap == null)
        return;
      this.lastTap.Cancel();
    }

    private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      if (e.Handled)
        return;
      e.Handled = true;
      this.isTap = true;
      this.tapStartTimestamp = DateTime.Now.Ticks;
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      DateTime now = DateTime.Now;
      if (e.Handled)
        return;
      e.Handled = true;
      UIElement originalSource = e.OriginalSource as UIElement;
      UIElement visual = sender as UIElement;
      if (originalSource == null || visual == null)
        return;
      Point point = originalSource.TransformToVisual(visual).Transform(e.ManipulationOrigin);
      bool flag1 = false;
      bool flag2 = false;
      if (e.DeltaManipulation.Scale.X > 0.0 && e.DeltaManipulation.Scale.Y > 0.0 && (Math.Abs(1.0 - e.DeltaManipulation.Scale.X) > 0.02 || Math.Abs(1.0 - e.DeltaManipulation.Scale.Y) > 0.02))
        flag1 = true;
      if (!flag1 && (Math.Abs(e.DeltaManipulation.Translation.X) > 2.0 || Math.Abs(e.DeltaManipulation.Translation.Y) > 2.0))
        flag2 = Math.Abs(e.CumulativeManipulation.Translation.X) > 30.0 || Math.Abs(e.CumulativeManipulation.Translation.Y) > 30.0;
      if (flag1 || flag2)
      {
        this.isTap = false;
        if (this.lastTap != null)
        {
          this.lastTap.DoItNow();
          this.lastTap = (Timeout) null;
        }
      }
      if (this.isTap && PhysicsHelper.ExceedsThreshold(TimeSpan.FromTicks(now.Ticks - this.tapStartTimestamp), 2000L))
      {
        object sender1 = sender;
        TapAndHoldGestureEventArgs gestureEventArgs = new TapAndHoldGestureEventArgs();
        gestureEventArgs.Origin = point;
        TapAndHoldGestureEventArgs e1 = gestureEventArgs;
        this.FireTapAndHoldEvent(sender1, (GestureEventArgs) e1);
        this.isTap = false;
        e.Complete();
      }
      if (flag1)
      {
        if (e.DeltaManipulation.Scale.X < 1.0 || e.DeltaManipulation.Scale.Y < 1.0)
        {
          object sender2 = sender;
          PinchGestureEventArgs gestureEventArgs = new PinchGestureEventArgs();
          gestureEventArgs.Origin = point;
          gestureEventArgs.Scale = e.DeltaManipulation.Scale;
          PinchGestureEventArgs e2 = gestureEventArgs;
          this.FirePinchEvent(sender2, (GestureEventArgs) e2);
        }
        else
        {
          object sender3 = sender;
          StretchGestureEventArgs gestureEventArgs = new StretchGestureEventArgs();
          gestureEventArgs.Origin = point;
          gestureEventArgs.Scale = e.DeltaManipulation.Scale;
          StretchGestureEventArgs e3 = gestureEventArgs;
          this.FireStretchEvent(sender3, (GestureEventArgs) e3);
        }
      }
      else if (flag2)
      {
        object sender4 = sender;
        PanGestureEventArgs gestureEventArgs = new PanGestureEventArgs();
        gestureEventArgs.Origin = point;
        gestureEventArgs.Translation = e.DeltaManipulation.Translation;
        PanGestureEventArgs e4 = gestureEventArgs;
        this.FirePanEvent(sender4, (GestureEventArgs) e4);
      }
      this.lastTranslation = e.DeltaManipulation.Translation;
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      DateTime now = DateTime.Now;
      if (e.Handled)
        return;
      e.Handled = true;
      UIElement originalSource = e.OriginalSource as UIElement;
      UIElement visual = sender as UIElement;
      if (originalSource == null || visual == null)
        return;
      Point origin = originalSource.TransformToVisual(visual).Transform(e.ManipulationOrigin);
      if (this.isTap)
      {
        if (this.isTapPending && ManipulationHandler.IsDoubleTap(this.lastTapOrigin, this.lastTapTimestamp, origin, now.Ticks))
        {
          if (this.lastTap != null)
          {
            this.lastTap.Cancel();
            this.lastTap = (Timeout) null;
          }
          this.isTapPending = false;
          object sender1 = sender;
          DoubleTapGestureEventArgs gestureEventArgs = new DoubleTapGestureEventArgs();
          gestureEventArgs.Origin = origin;
          DoubleTapGestureEventArgs e1 = gestureEventArgs;
          this.FireDoubleTapEvent(sender1, (GestureEventArgs) e1);
        }
        else
        {
          if (this.lastTap != null)
          {
            this.lastTap.DoItNow();
            this.lastTap = (Timeout) null;
          }
          this.isTapPending = true;
          this.lastTapTimestamp = now.Ticks;
          this.lastTapOrigin = origin;
          this.lastTap = new Timeout((Action) (() =>
          {
            object sender2 = sender;
            TapGestureEventArgs e2 = new TapGestureEventArgs()
            {
              Origin = origin
            };
            this.FireTapEvent(sender2, (GestureEventArgs) e2);
          }), 600L);
        }
      }
      else
        this.isTapPending = false;
      if (!e.IsInertial || Math.Abs(this.lastTranslation.X) <= 3.0 && Math.Abs(this.lastTranslation.Y) <= 3.0 || e.TotalManipulation.Scale.X != 0.0 || e.TotalManipulation.Scale.Y != 0.0)
        return;
      object sender3 = sender;
      FlickGestureEventArgs gestureEventArgs1 = new FlickGestureEventArgs();
      gestureEventArgs1.Origin = origin;
      gestureEventArgs1.Velocity = e.FinalVelocities.LinearVelocity;
      FlickGestureEventArgs e3 = gestureEventArgs1;
      this.FireFlickEvent(sender3, (GestureEventArgs) e3);
    }

    private void FireTapEvent(object sender, GestureEventArgs e)
    {
      EventHandler<GestureEventArgs> tap = this.Tap;
      if (tap == null)
        return;
      tap(sender, e);
    }

    private void FireDoubleTapEvent(object sender, GestureEventArgs e)
    {
      EventHandler<GestureEventArgs> doubleTap = this.DoubleTap;
      if (doubleTap == null)
        return;
      doubleTap(sender, e);
    }

    private void FireTapAndHoldEvent(object sender, GestureEventArgs e)
    {
      EventHandler<GestureEventArgs> tapAndHold = this.TapAndHold;
      if (tapAndHold == null)
        return;
      tapAndHold(sender, e);
    }

    private void FirePanEvent(object sender, GestureEventArgs e)
    {
      EventHandler<GestureEventArgs> pan = this.Pan;
      if (pan == null)
        return;
      pan(sender, e);
    }

    private void FireFlickEvent(object sender, GestureEventArgs e)
    {
      EventHandler<GestureEventArgs> flick = this.Flick;
      if (flick == null)
        return;
      flick(sender, e);
    }

    private void FirePinchEvent(object sender, GestureEventArgs e)
    {
      EventHandler<GestureEventArgs> pinch = this.Pinch;
      if (pinch == null)
        return;
      pinch(sender, e);
    }

    private void FireStretchEvent(object sender, GestureEventArgs e)
    {
      EventHandler<GestureEventArgs> stretch = this.Stretch;
      if (stretch == null)
        return;
      stretch(sender, e);
    }
  }
}
