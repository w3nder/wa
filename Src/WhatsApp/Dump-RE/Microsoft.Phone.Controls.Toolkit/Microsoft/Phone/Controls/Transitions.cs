// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Transitions
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal static class Transitions
  {
    private static Dictionary<string, string> _storyboardXamlCache;

    private static ITransition GetEnumStoryboard<T>(UIElement element, string name, T mode)
    {
      Storyboard storyboard = Transitions.GetStoryboard(name + Enum.GetName(typeof (T), (object) mode));
      if (storyboard == null)
        return (ITransition) null;
      Storyboard.SetTarget((Timeline) storyboard, (DependencyObject) element);
      return (ITransition) new Transition(element, storyboard);
    }

    private static Storyboard GetStoryboard(string name)
    {
      if (Transitions._storyboardXamlCache == null)
        Transitions._storyboardXamlCache = new Dictionary<string, string>();
      string xaml = (string) null;
      if (Transitions._storyboardXamlCache.ContainsKey(name))
      {
        xaml = Transitions._storyboardXamlCache[name];
      }
      else
      {
        using (StreamReader streamReader = new StreamReader(Application.GetResourceStream(new Uri("/Microsoft.Phone.Controls.Toolkit;component/Transitions/Storyboards/" + name + ".xaml", UriKind.Relative)).Stream))
        {
          xaml = streamReader.ReadToEnd();
          Transitions._storyboardXamlCache[name] = xaml;
        }
      }
      return XamlReader.Load(xaml) as Storyboard;
    }

    public static ITransition Roll(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      Storyboard storyboard = Transitions.GetStoryboard(nameof (Roll));
      Storyboard.SetTarget((Timeline) storyboard, (DependencyObject) element);
      element.Projection = (Projection) new PlaneProjection()
      {
        CenterOfRotationX = 0.5,
        CenterOfRotationY = 0.5
      };
      return (ITransition) new Transition(element, storyboard);
    }

    public static ITransition Rotate(UIElement element, RotateTransitionMode rotateTransitionMode)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      if (!Enum.IsDefined(typeof (RotateTransitionMode), (object) rotateTransitionMode))
        throw new ArgumentOutOfRangeException(nameof (rotateTransitionMode));
      element.Projection = (Projection) new PlaneProjection()
      {
        CenterOfRotationX = 0.5,
        CenterOfRotationY = 0.5
      };
      rotateTransitionMode = Transitions.AdjustRotateTransitionModeForFlowDirection(element, rotateTransitionMode);
      return Transitions.GetEnumStoryboard<RotateTransitionMode>(element, nameof (Rotate), rotateTransitionMode);
    }

    public static ITransition Slide(UIElement element, SlideTransitionMode slideTransitionMode)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      if (!Enum.IsDefined(typeof (SlideTransitionMode), (object) slideTransitionMode))
        throw new ArgumentOutOfRangeException(nameof (slideTransitionMode));
      element.RenderTransform = (Transform) new TranslateTransform();
      return Transitions.GetEnumStoryboard<SlideTransitionMode>(element, string.Empty, slideTransitionMode);
    }

    public static ITransition Swivel(UIElement element, SwivelTransitionMode swivelTransitionMode)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      if (!Enum.IsDefined(typeof (SwivelTransitionMode), (object) swivelTransitionMode))
        throw new ArgumentOutOfRangeException(nameof (swivelTransitionMode));
      element.Projection = (Projection) new PlaneProjection();
      return Transitions.GetEnumStoryboard<SwivelTransitionMode>(element, nameof (Swivel), swivelTransitionMode);
    }

    public static ITransition Turnstile(
      UIElement element,
      TurnstileTransitionMode turnstileTransitionMode)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      if (!Enum.IsDefined(typeof (TurnstileTransitionMode), (object) turnstileTransitionMode))
        throw new ArgumentOutOfRangeException(nameof (turnstileTransitionMode));
      element.Projection = (Projection) new PlaneProjection()
      {
        CenterOfRotationX = 0.0
      };
      return Transitions.GetEnumStoryboard<TurnstileTransitionMode>(element, nameof (Turnstile), turnstileTransitionMode);
    }

    public static ITransition TurnstileFeather(
      UIElement element,
      TurnstileFeatherTransitionMode turnstileFeatherTransitionMode,
      TimeSpan? beginTime)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      if (!Enum.IsDefined(typeof (TurnstileFeatherTransitionMode), (object) turnstileFeatherTransitionMode))
        throw new ArgumentOutOfRangeException(nameof (turnstileFeatherTransitionMode));
      element.Projection = (Projection) new PlaneProjection()
      {
        CenterOfRotationX = 0.0
      };
      return (ITransition) new FeatheredTransition(element, new Storyboard(), turnstileFeatherTransitionMode, beginTime);
    }

    private static RotateTransitionMode AdjustRotateTransitionModeForFlowDirection(
      UIElement element,
      RotateTransitionMode rotateTransitionMode)
    {
      FrameworkElement frameworkElement = element as FrameworkElement;
      RotateTransitionMode rotateTransitionMode1 = rotateTransitionMode;
      if (frameworkElement != null && frameworkElement.FlowDirection == FlowDirection.RightToLeft)
      {
        switch (rotateTransitionMode)
        {
          case RotateTransitionMode.In90Clockwise:
            rotateTransitionMode1 = RotateTransitionMode.In90Counterclockwise;
            break;
          case RotateTransitionMode.In90Counterclockwise:
            rotateTransitionMode1 = RotateTransitionMode.In90Clockwise;
            break;
          case RotateTransitionMode.In180Clockwise:
            rotateTransitionMode1 = RotateTransitionMode.In180Counterclockwise;
            break;
          case RotateTransitionMode.In180Counterclockwise:
            rotateTransitionMode1 = RotateTransitionMode.In180Clockwise;
            break;
          case RotateTransitionMode.Out90Clockwise:
            rotateTransitionMode1 = RotateTransitionMode.Out90Counterclockwise;
            break;
          case RotateTransitionMode.Out90Counterclockwise:
            rotateTransitionMode1 = RotateTransitionMode.Out90Clockwise;
            break;
          case RotateTransitionMode.Out180Clockwise:
            rotateTransitionMode1 = RotateTransitionMode.Out180Counterclockwise;
            break;
          case RotateTransitionMode.Out180Counterclockwise:
            rotateTransitionMode1 = RotateTransitionMode.Out180Clockwise;
            break;
        }
      }
      return rotateTransitionMode1;
    }
  }
}
