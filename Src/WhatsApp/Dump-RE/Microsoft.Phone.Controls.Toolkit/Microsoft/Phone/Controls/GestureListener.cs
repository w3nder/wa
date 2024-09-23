// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.GestureListener
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class GestureListener
  {
    private static DispatcherTimer _timer;
    private static bool _isInTouch;
    private static List<UIElement> _elements;
    private static Point _gestureOrigin;
    private static bool _gestureOriginChanged;
    private static Orientation? _gestureOrientation;
    private static Point _cumulativeDelta;
    private static Point _cumulativeDelta2;
    private static Point _finalVelocity;
    private static Point _pinchOrigin;
    private static Point _pinchOrigin2;
    private static Point _lastSamplePosition;
    private static Point _lastSamplePosition2;
    private static bool _isPinching;
    private static bool _flicked;
    private static bool _isDragging;

    public event EventHandler<GestureEventArgs> GestureBegin;

    public event EventHandler<GestureEventArgs> GestureCompleted;

    public event EventHandler<GestureEventArgs> Tap;

    public event EventHandler<GestureEventArgs> DoubleTap;

    public event EventHandler<GestureEventArgs> Hold;

    public event EventHandler<DragStartedGestureEventArgs> DragStarted;

    public event EventHandler<DragDeltaGestureEventArgs> DragDelta;

    public event EventHandler<DragCompletedGestureEventArgs> DragCompleted;

    public event EventHandler<FlickGestureEventArgs> Flick;

    public event EventHandler<PinchStartedGestureEventArgs> PinchStarted;

    public event EventHandler<PinchGestureEventArgs> PinchDelta;

    public event EventHandler<PinchGestureEventArgs> PinchCompleted;

    static GestureListener()
    {
      System.Windows.Input.Touch.FrameReported += new TouchFrameEventHandler(GestureListener.OnTouchFrameReported);
      TouchPanel.EnabledGestures = GestureType.DoubleTap | GestureType.DragComplete | GestureType.Flick | GestureType.FreeDrag | GestureType.Hold | GestureType.Pinch | GestureType.PinchComplete | GestureType.Tap;
      GestureListener._timer = new DispatcherTimer()
      {
        Interval = TimeSpan.FromMilliseconds(100.0)
      };
      GestureListener._timer.Tick += new EventHandler(GestureListener.OnTimerTick);
    }

    private static void OnTouchFrameReported(object sender, TouchFrameEventArgs e)
    {
      bool flag = false;
      Point point = new Point(0.0, 0.0);
      foreach (TouchPoint touchPoint in (PresentationFrameworkCollection<TouchPoint>) e.GetTouchPoints((UIElement) null))
      {
        if (touchPoint.Action != TouchAction.Up)
        {
          point = touchPoint.Position;
          flag = true;
          break;
        }
      }
      if (!GestureListener._isInTouch && flag)
      {
        GestureListener._gestureOrigin = point;
        GestureListener.TouchStart();
      }
      else if (GestureListener._isInTouch && !flag)
        GestureListener.TouchComplete();
      else if (GestureListener._isInTouch)
        GestureListener.TouchDelta();
      else
        GestureListener.TouchStart();
      GestureListener._isInTouch = flag;
    }

    private static void TouchStart()
    {
      GestureListener._cumulativeDelta.X = GestureListener._cumulativeDelta.Y = GestureListener._cumulativeDelta2.X = GestureListener._cumulativeDelta2.Y = 0.0;
      GestureListener._finalVelocity.X = GestureListener._finalVelocity.Y = 0.0;
      GestureListener._isDragging = GestureListener._flicked = false;
      GestureListener._elements = new List<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(GestureListener._gestureOrigin, Application.Current.RootVisual));
      GestureListener._gestureOriginChanged = false;
      GestureListener.RaiseGestureEvent<GestureEventArgs>((Func<GestureListener, EventHandler<GestureEventArgs>>) (helper => helper.GestureBegin), (Func<GestureEventArgs>) (() => new GestureEventArgs(GestureListener._gestureOrigin, GestureListener._gestureOrigin)), false);
      GestureListener.ProcessTouchPanelEvents();
      GestureListener._timer.Start();
    }

    private static void TouchDelta() => GestureListener.ProcessTouchPanelEvents();

    private static void TouchComplete()
    {
      GestureListener.ProcessTouchPanelEvents();
      GestureListener.RaiseGestureEvent<GestureEventArgs>((Func<GestureListener, EventHandler<GestureEventArgs>>) (helper => helper.GestureCompleted), (Func<GestureEventArgs>) (() => new GestureEventArgs(GestureListener._gestureOrigin, GestureListener._lastSamplePosition)), false);
      GestureListener._elements = (List<UIElement>) null;
      GestureListener._gestureOrientation = new Orientation?();
      GestureListener._timer.Stop();
    }

    private static void OnTimerTick(object sender, EventArgs e)
    {
      try
      {
        GestureListener.ProcessTouchPanelEvents();
      }
      catch
      {
      }
    }

    private static void ProcessTouchPanelEvents()
    {
      Point delta = new Point(0.0, 0.0);
      GeneralTransform deltaTransform = (GeneralTransform) null;
      while (TouchPanel.IsGestureAvailable)
      {
        GestureSample gestureSample = TouchPanel.ReadGesture();
        Point samplePosition = gestureSample.Position.ToPoint();
        Point samplePosition2 = gestureSample.Position2.ToPoint();
        Point sampleDelta = gestureSample.Delta.ToPoint();
        GestureListener.GetTranslatedDelta(ref deltaTransform, ref sampleDelta, ref GestureListener._cumulativeDelta, gestureSample.GestureType != GestureType.Flick);
        Point point = gestureSample.Delta2.ToPoint();
        GestureListener.GetTranslatedDelta(ref deltaTransform, ref point, ref GestureListener._cumulativeDelta2, gestureSample.GestureType != GestureType.Flick);
        if (GestureListener._elements == null || GestureListener._gestureOriginChanged)
        {
          GestureListener._gestureOrigin = samplePosition;
          GestureListener._elements = new List<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(GestureListener._gestureOrigin, Application.Current.RootVisual));
          GestureListener._gestureOriginChanged = false;
        }
        if (!GestureListener._gestureOrientation.HasValue && (sampleDelta.X != 0.0 || sampleDelta.Y != 0.0))
          GestureListener._gestureOrientation = new Orientation?(Math.Abs(sampleDelta.X) >= Math.Abs(sampleDelta.Y) ? Orientation.Horizontal : Orientation.Vertical);
        switch (gestureSample.GestureType)
        {
          case GestureType.Tap:
            GestureListener.RaiseGestureEvent<GestureEventArgs>((Func<GestureListener, EventHandler<GestureEventArgs>>) (helper => helper.Tap), (Func<GestureEventArgs>) (() => new GestureEventArgs(GestureListener._gestureOrigin, samplePosition)), false);
            continue;
          case GestureType.DoubleTap:
            GestureListener.RaiseGestureEvent<GestureEventArgs>((Func<GestureListener, EventHandler<GestureEventArgs>>) (helper => helper.DoubleTap), (Func<GestureEventArgs>) (() => new GestureEventArgs(GestureListener._gestureOrigin, samplePosition)), false);
            continue;
          case GestureType.Hold:
            GestureListener.RaiseGestureEvent<GestureEventArgs>((Func<GestureListener, EventHandler<GestureEventArgs>>) (helper => helper.Hold), (Func<GestureEventArgs>) (() => new GestureEventArgs(GestureListener._gestureOrigin, samplePosition)), false);
            continue;
          case GestureType.FreeDrag:
            if (sampleDelta.X != 0.0 || sampleDelta.Y != 0.0)
            {
              if (!GestureListener._isDragging)
              {
                GestureListener.RaiseGestureEvent<DragStartedGestureEventArgs>((Func<GestureListener, EventHandler<DragStartedGestureEventArgs>>) (helper => helper.DragStarted), (Func<DragStartedGestureEventArgs>) (() => new DragStartedGestureEventArgs(GestureListener._gestureOrigin, GestureListener._gestureOrientation.Value)), true);
                GestureListener._isDragging = true;
              }
              delta.X += sampleDelta.X;
              delta.Y += sampleDelta.Y;
              GestureListener._lastSamplePosition = samplePosition;
              continue;
            }
            continue;
          case GestureType.Pinch:
            if (!GestureListener._isPinching)
            {
              GestureListener._isPinching = true;
              GestureListener._pinchOrigin = samplePosition;
              GestureListener._pinchOrigin2 = samplePosition2;
              GestureListener.RaiseGestureEvent<PinchStartedGestureEventArgs>((Func<GestureListener, EventHandler<PinchStartedGestureEventArgs>>) (helper => helper.PinchStarted), (Func<PinchStartedGestureEventArgs>) (() => new PinchStartedGestureEventArgs(GestureListener._pinchOrigin, GestureListener._pinchOrigin2, GestureListener._pinchOrigin, GestureListener._pinchOrigin2)), true);
            }
            GestureListener._lastSamplePosition = samplePosition;
            GestureListener._lastSamplePosition2 = samplePosition2;
            GestureListener.RaiseGestureEvent<PinchGestureEventArgs>((Func<GestureListener, EventHandler<PinchGestureEventArgs>>) (helper => helper.PinchDelta), (Func<PinchGestureEventArgs>) (() => new PinchGestureEventArgs(GestureListener._pinchOrigin, GestureListener._pinchOrigin2, samplePosition, samplePosition2)), false);
            continue;
          case GestureType.Flick:
            GestureListener._flicked = true;
            GestureListener._finalVelocity = sampleDelta;
            GestureListener.RaiseGestureEvent<FlickGestureEventArgs>((Func<GestureListener, EventHandler<FlickGestureEventArgs>>) (helper => helper.Flick), (Func<FlickGestureEventArgs>) (() => new FlickGestureEventArgs(GestureListener._gestureOrigin, sampleDelta)), true);
            continue;
          case GestureType.DragComplete:
            if (!GestureListener._flicked && (delta.X != 0.0 || delta.Y != 0.0))
            {
              GestureListener.RaiseGestureEvent<DragDeltaGestureEventArgs>((Func<GestureListener, EventHandler<DragDeltaGestureEventArgs>>) (helper => helper.DragDelta), (Func<DragDeltaGestureEventArgs>) (() => new DragDeltaGestureEventArgs(GestureListener._gestureOrigin, samplePosition, delta, GestureListener._gestureOrientation.Value)), false);
              delta.X = delta.Y = 0.0;
            }
            if (GestureListener._isDragging)
            {
              GestureListener.RaiseGestureEvent<DragCompletedGestureEventArgs>((Func<GestureListener, EventHandler<DragCompletedGestureEventArgs>>) (helper => helper.DragCompleted), (Func<DragCompletedGestureEventArgs>) (() => new DragCompletedGestureEventArgs(GestureListener._gestureOrigin, GestureListener._lastSamplePosition, GestureListener._cumulativeDelta, GestureListener._gestureOrientation.Value, GestureListener._finalVelocity)), false);
              delta.X = delta.Y = 0.0;
            }
            GestureListener._cumulativeDelta.X = GestureListener._cumulativeDelta.Y = 0.0;
            GestureListener._flicked = GestureListener._isDragging = false;
            GestureListener._gestureOriginChanged = true;
            continue;
          case GestureType.PinchComplete:
            GestureListener._isPinching = false;
            GestureListener.RaiseGestureEvent<PinchGestureEventArgs>((Func<GestureListener, EventHandler<PinchGestureEventArgs>>) (helper => helper.PinchCompleted), (Func<PinchGestureEventArgs>) (() => new PinchGestureEventArgs(GestureListener._pinchOrigin, GestureListener._pinchOrigin2, GestureListener._lastSamplePosition, GestureListener._lastSamplePosition2)), false);
            GestureListener._cumulativeDelta.X = GestureListener._cumulativeDelta.Y = GestureListener._cumulativeDelta2.X = GestureListener._cumulativeDelta2.Y = 0.0;
            GestureListener._gestureOriginChanged = true;
            continue;
          default:
            continue;
        }
      }
      if (GestureListener._flicked || delta.X == 0.0 && delta.Y == 0.0)
        return;
      GestureListener.RaiseGestureEvent<DragDeltaGestureEventArgs>((Func<GestureListener, EventHandler<DragDeltaGestureEventArgs>>) (helper => helper.DragDelta), (Func<DragDeltaGestureEventArgs>) (() => new DragDeltaGestureEventArgs(GestureListener._gestureOrigin, GestureListener._lastSamplePosition, delta, GestureListener._gestureOrientation.Value)), false);
    }

    private static void GetTranslatedDelta(
      ref GeneralTransform deltaTransform,
      ref Point sampleDelta,
      ref Point cumulativeDelta,
      bool addToCumulative)
    {
      if (sampleDelta.X == 0.0 && sampleDelta.Y == 0.0)
        return;
      if (deltaTransform == null && Application.Current.RootVisual != null)
        deltaTransform = GestureListener.GetInverseRootTransformNoOffset();
      if (deltaTransform == null)
        return;
      sampleDelta = deltaTransform.Transform(sampleDelta);
      if (!addToCumulative)
        return;
      cumulativeDelta.X += sampleDelta.X;
      cumulativeDelta.Y += sampleDelta.Y;
    }

    private static GeneralTransform GetInverseRootTransformNoOffset()
    {
      GeneralTransform inverse = Application.Current.RootVisual.TransformToVisual((UIElement) null).Inverse;
      if (inverse is MatrixTransform matrixTransform)
      {
        Matrix matrix = matrixTransform.Matrix;
        matrix.OffsetX = matrix.OffsetY = 0.0;
        matrixTransform.Matrix = matrix;
      }
      return inverse;
    }

    private static void RaiseGestureEvent<T>(
      Func<GestureListener, EventHandler<T>> eventGetter,
      Func<T> argsGetter,
      bool releaseMouseCapture)
      where T : GestureEventArgs
    {
      T args = default (T);
      FrameworkElement originalSource = (FrameworkElement) null;
      bool flag = false;
      foreach (FrameworkElement element in GestureListener._elements)
      {
        if (releaseMouseCapture)
          element.ReleaseMouseCapture();
        if (!flag)
        {
          if (originalSource == null)
            originalSource = element;
          GestureListener listenerInternal = GestureService.GetGestureListenerInternal((DependencyObject) element, false);
          if (listenerInternal != null)
            SafeRaise.Raise<T>(eventGetter(listenerInternal), (object) element, (SafeRaise.GetEventArgs<T>) (() =>
            {
              if ((object) args == null)
              {
                args = argsGetter();
                args.OriginalSource = (object) originalSource;
              }
              return args;
            }));
          if ((object) (T) args != null && args.Handled)
            flag = true;
        }
      }
    }
  }
}
