// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.TouchHelper
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal static class TouchHelper
  {
    private static bool isEnabled;
    private static readonly Dictionary<UIElement, TouchHandlers> currentHandlers = new Dictionary<UIElement, TouchHandlers>();
    private static readonly Dictionary<int, UIElement> currentCaptures = new Dictionary<int, UIElement>();
    private static readonly Dictionary<int, TouchPoint> currentTouchPoints = new Dictionary<int, TouchPoint>();

    public static bool AreAnyTouches => TouchHelper.currentTouchPoints.Count != 0;

    public static bool Capture(this TouchDevice touchDevice, UIElement element)
    {
      if (touchDevice == null)
        throw new ArgumentNullException(nameof (element));
      UIElement uiElement;
      TouchHandlers touchHandlers;
      if (TouchHelper.currentCaptures.TryGetValue(touchDevice.Id, out uiElement) && !object.ReferenceEquals((object) uiElement, (object) element) && TouchHelper.currentHandlers.TryGetValue(uiElement, out touchHandlers))
      {
        EventHandler<TouchEventArgs> lostTouchCapture = touchHandlers.LostTouchCapture;
        TouchPoint touchPoint;
        if (lostTouchCapture != null && TouchHelper.currentTouchPoints.TryGetValue(touchDevice.Id, out touchPoint))
          lostTouchCapture((object) uiElement, new TouchEventArgs(touchPoint));
      }
      if (element != null)
        TouchHelper.currentCaptures[touchDevice.Id] = element;
      else
        TouchHelper.currentCaptures.Remove(touchDevice.Id);
      return true;
    }

    public static void AddHandlers(UIElement element, TouchHandlers handlers)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      TouchHelper.currentHandlers[element] = handlers != null ? handlers : throw new ArgumentNullException(nameof (handlers));
      TouchHelper.EnableInput(true);
    }

    public static void RemoveHandlers(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      TouchHelper.currentHandlers.Remove(element);
      if (TouchHelper.currentHandlers.Count != 0)
        return;
      TouchHelper.EnableInput(false);
    }

    private static void EnableInput(bool enable)
    {
      if (enable)
      {
        if (TouchHelper.isEnabled)
          return;
        TouchHelper.EnableInput();
        TouchHelper.isEnabled = true;
      }
      else
      {
        if (!TouchHelper.isEnabled)
          return;
        TouchHelper.DisableInput();
        TouchHelper.isEnabled = false;
      }
    }

    private static void EnableInput()
    {
      Touch.FrameReported += new TouchFrameEventHandler(TouchHelper.TouchFrameReported);
    }

    private static void DisableInput()
    {
      Touch.FrameReported -= new TouchFrameEventHandler(TouchHelper.TouchFrameReported);
      TouchHelper.currentCaptures.Clear();
      TouchHelper.currentHandlers.Clear();
      TouchHelper.currentTouchPoints.Clear();
    }

    private static void TouchFrameReported(object sender, TouchFrameEventArgs e)
    {
      UIElement rootVisual = Application.Current.RootVisual;
      if (rootVisual == null)
        return;
      Collection<int> collection1 = new Collection<int>();
      foreach (TouchPoint touchPoint in (PresentationFrameworkCollection<TouchPoint>) e.GetTouchPoints(rootVisual))
      {
        int id = touchPoint.TouchDevice.Id;
        collection1.Add(id);
        switch (touchPoint.Action)
        {
          case TouchAction.Down:
            TouchHelper.HitTestAndRaiseDownEvent(rootVisual, touchPoint);
            TouchHelper.currentTouchPoints[id] = touchPoint;
            continue;
          case TouchAction.Move:
            TouchHelper.currentTouchPoints[id] = touchPoint;
            continue;
          case TouchAction.Up:
            UIElement element;
            TouchHelper.currentCaptures.TryGetValue(id, out element);
            if (element != null)
            {
              TouchHelper.RaiseUpEvent(element, touchPoint);
              touchPoint.TouchDevice.Capture((UIElement) null);
              element = (UIElement) null;
            }
            TouchHelper.currentTouchPoints.Remove(id);
            continue;
          default:
            continue;
        }
      }
      Collection<int> collection2 = new Collection<int>();
      foreach (int key in TouchHelper.currentTouchPoints.Keys)
      {
        if (!collection1.Contains(key))
          collection2.Add(key);
      }
      foreach (int key in collection2)
        TouchHelper.currentTouchPoints.Remove(key);
      if (TouchHelper.currentTouchPoints.Count == 0)
        TouchHelper.currentCaptures.Clear();
      TouchHelper.RaiseCapturedReportEvent(rootVisual);
    }

    private static void RaiseCapturedReportEvent(UIElement root)
    {
      foreach (KeyValuePair<UIElement, TouchHandlers> currentHandler in TouchHelper.currentHandlers)
      {
        EventHandler<TouchReportedEventArgs> capturedTouchReported = currentHandler.Value.CapturedTouchReported;
        if (capturedTouchReported != null)
        {
          UIElement key = currentHandler.Key;
          if (key != null)
          {
            GeneralTransform visual;
            try
            {
              visual = root.TransformToVisual(key);
            }
            catch (ArgumentException ex)
            {
              continue;
            }
            List<Point> touchPoints = new List<Point>();
            foreach (KeyValuePair<int, UIElement> currentCapture in TouchHelper.currentCaptures)
            {
              TouchPoint touchPoint;
              if (object.ReferenceEquals((object) currentCapture.Value, (object) currentHandler.Key) && TouchHelper.currentTouchPoints.TryGetValue(currentCapture.Key, out touchPoint))
                touchPoints.Add(visual.Transform(touchPoint.Position));
            }
            capturedTouchReported((object) currentHandler.Key, new TouchReportedEventArgs((IEnumerable<Point>) touchPoints));
          }
        }
      }
    }

    private static void HitTestAndRaiseDownEvent(UIElement root, TouchPoint touchPoint)
    {
      Point position = root.TransformToVisual((UIElement) null).Transform(touchPoint.Position);
      foreach (UIElement uiElement in TouchHelper.InputHitTest(root, position))
      {
        TouchHandlers touchHandlers;
        if (TouchHelper.currentHandlers.TryGetValue(uiElement, out touchHandlers))
        {
          EventHandler<TouchEventArgs> touchDown = touchHandlers.TouchDown;
          if (touchDown != null)
          {
            touchDown((object) uiElement, new TouchEventArgs(touchPoint));
            break;
          }
        }
      }
    }

    private static IEnumerable<UIElement> InputHitTest(UIElement root, Point position)
    {
      foreach (UIElement element in VisualTreeHelper.FindElementsInHostCoordinates(position, root))
      {
        yield return element;
        for (UIElement parent = VisualTreeHelper.GetParent((DependencyObject) element) as UIElement; parent != null; parent = VisualTreeHelper.GetParent((DependencyObject) parent) as UIElement)
          yield return parent;
      }
    }

    private static void RaiseUpEvent(UIElement element, TouchPoint touchPoint)
    {
      TouchHandlers touchHandlers;
      if (!TouchHelper.currentHandlers.TryGetValue(element, out touchHandlers))
        return;
      EventHandler<TouchEventArgs> capturedTouchUp = touchHandlers.CapturedTouchUp;
      if (capturedTouchUp == null)
        return;
      capturedTouchUp((object) element, new TouchEventArgs(touchPoint));
    }
  }
}
