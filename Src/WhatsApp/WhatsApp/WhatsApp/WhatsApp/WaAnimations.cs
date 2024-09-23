// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaAnimations
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace WhatsApp
{
  public static class WaAnimations
  {
    public static Storyboard CreateStoryboard(DoubleAnimation animation)
    {
      return WaAnimations.CreateStoryboard((IEnumerable<DoubleAnimation>) new DoubleAnimation[1]
      {
        animation
      });
    }

    public static Storyboard CreateStoryboard(IEnumerable<DoubleAnimation> animations)
    {
      Storyboard storyboard = new Storyboard();
      foreach (DoubleAnimation doubleAnimation in animations.Where<DoubleAnimation>((Func<DoubleAnimation, bool>) (a => a != null)))
        storyboard.Children.Add((Timeline) doubleAnimation);
      return storyboard;
    }

    public static DoubleAnimation Fade(
      double startOpacity,
      double endOpacity,
      TimeSpan duration,
      DependencyObject target = null,
      EasingFunctionBase easeFunc = null)
    {
      return WaAnimations.CreateDoubleAnimation(startOpacity, endOpacity, duration, target, easeFunc, "Opacity");
    }

    public static DoubleAnimation Fade(
      WaAnimations.FadeType fadeType,
      TimeSpan duration,
      DependencyObject target = null,
      EasingFunctionBase easeFunc = null)
    {
      return WaAnimations.Fade(fadeType == WaAnimations.FadeType.FadeIn ? 0.0 : 1.0, fadeType == WaAnimations.FadeType.FadeIn ? 1.0 : 0.0, duration, target, easeFunc);
    }

    private static DoubleAnimation CreateDoubleAnimation(
      double from,
      double to,
      TimeSpan duration,
      DependencyObject target,
      EasingFunctionBase easeFunc,
      string propertyPath)
    {
      DoubleAnimation doubleAnimation = new DoubleAnimation();
      doubleAnimation.From = new double?(from);
      doubleAnimation.To = new double?(to);
      doubleAnimation.Duration = (Duration) duration;
      doubleAnimation.EasingFunction = (IEasingFunction) easeFunc;
      DoubleAnimation element = doubleAnimation;
      if (target != null)
        Storyboard.SetTarget((Timeline) element, target);
      Storyboard.SetTargetProperty((Timeline) element, new PropertyPath(propertyPath, new object[0]));
      return element;
    }

    public static DoubleAnimation VerticalSlide(
      double fromY,
      double toY,
      TimeSpan duration,
      DependencyObject target = null,
      EasingFunctionBase easeFunc = null,
      string propertyPath = "(UIElement.RenderTransform).TranslateY")
    {
      return WaAnimations.CreateDoubleAnimation(fromY, toY, duration, target, easeFunc, propertyPath);
    }

    public static DoubleAnimation HorizontalSlide(
      double fromX,
      double toX,
      TimeSpan duration,
      DependencyObject target = null,
      EasingFunctionBase easeFunc = null,
      string propertyPath = "(UIElement.RenderTransform).TranslateX")
    {
      return WaAnimations.CreateDoubleAnimation(fromX, toX, duration, target, easeFunc, propertyPath);
    }

    public static Storyboard PageTransition(PageTransitionAnimation transition)
    {
      List<DoubleAnimation> animations = new List<DoubleAnimation>(5);
      switch (transition)
      {
        case PageTransitionAnimation.FadeOut:
          animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(200.0)));
          break;
        case PageTransitionAnimation.SlideUpFadeIn:
          animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(450.0)));
          List<DoubleAnimation> doubleAnimationList1 = animations;
          TimeSpan duration1 = TimeSpan.FromMilliseconds(500.0);
          BackEase easeFunc1 = new BackEase();
          easeFunc1.EasingMode = EasingMode.EaseOut;
          easeFunc1.Amplitude = 0.1;
          DoubleAnimation doubleAnimation1 = WaAnimations.VerticalSlide(750.0, 0.0, duration1, easeFunc: (EasingFunctionBase) easeFunc1);
          doubleAnimationList1.Add(doubleAnimation1);
          break;
        case PageTransitionAnimation.SlideDownFadeOut:
          animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(300.0)));
          List<DoubleAnimation> doubleAnimationList2 = animations;
          TimeSpan duration2 = TimeSpan.FromMilliseconds(300.0);
          BackEase easeFunc2 = new BackEase();
          easeFunc2.EasingMode = EasingMode.EaseIn;
          easeFunc2.Amplitude = 0.1;
          DoubleAnimation doubleAnimation2 = WaAnimations.VerticalSlide(0.0, 500.0, duration2, easeFunc: (EasingFunctionBase) easeFunc2);
          doubleAnimationList2.Add(doubleAnimation2);
          break;
        case PageTransitionAnimation.ContinuumBackwardOut:
          List<DoubleAnimation> doubleAnimationList3 = animations;
          TimeSpan duration3 = TimeSpan.FromMilliseconds(200.0);
          ExponentialEase easeFunc3 = new ExponentialEase();
          easeFunc3.Exponent = 6.0;
          easeFunc3.EasingMode = EasingMode.EaseIn;
          DoubleAnimation doubleAnimation3 = WaAnimations.VerticalSlide(0.0, 200.0, duration3, easeFunc: (EasingFunctionBase) easeFunc3);
          doubleAnimationList3.Add(doubleAnimation3);
          DoubleAnimation doubleAnimation4 = WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(100.0));
          doubleAnimation4.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(150.0));
          animations.Add(doubleAnimation4);
          break;
        case PageTransitionAnimation.ContinuumForwardOut:
          List<DoubleAnimation> doubleAnimationList4 = animations;
          TimeSpan duration4 = TimeSpan.FromMilliseconds(200.0);
          ExponentialEase easeFunc4 = new ExponentialEase();
          easeFunc4.Exponent = 4.0;
          easeFunc4.EasingMode = EasingMode.EaseIn;
          DoubleAnimation doubleAnimation5 = WaAnimations.VerticalSlide(0.0, 200.0, duration4, easeFunc: (EasingFunctionBase) easeFunc4);
          doubleAnimationList4.Add(doubleAnimation5);
          DoubleAnimation doubleAnimation6 = WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(100.0));
          doubleAnimation6.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(150.0));
          animations.Add(doubleAnimation6);
          break;
        case PageTransitionAnimation.ContinuumBackwardIn:
          List<DoubleAnimation> doubleAnimationList5 = animations;
          TimeSpan duration5 = TimeSpan.FromMilliseconds(200.0);
          ExponentialEase easeFunc5 = new ExponentialEase();
          easeFunc5.Exponent = 6.0;
          easeFunc5.EasingMode = EasingMode.EaseOut;
          DoubleAnimation doubleAnimation7 = WaAnimations.VerticalSlide(200.0, 0.0, duration5, easeFunc: (EasingFunctionBase) easeFunc5);
          doubleAnimationList5.Add(doubleAnimation7);
          animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(100.0)));
          break;
        case PageTransitionAnimation.SlideUpFadeOut:
          List<DoubleAnimation> doubleAnimationList6 = animations;
          TimeSpan duration6 = TimeSpan.FromMilliseconds(350.0);
          BackEase easeFunc6 = new BackEase();
          easeFunc6.EasingMode = EasingMode.EaseIn;
          easeFunc6.Amplitude = 0.2;
          DoubleAnimation doubleAnimation8 = WaAnimations.VerticalSlide(0.0, -500.0, duration6, easeFunc: (EasingFunctionBase) easeFunc6);
          doubleAnimationList6.Add(doubleAnimation8);
          animations.Add(WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(350.0)));
          break;
        case PageTransitionAnimation.TurnstileForwardOut:
          List<DoubleAnimation> doubleAnimationList7 = animations;
          TimeSpan duration7 = TimeSpan.FromMilliseconds(350.0);
          PowerEase easeFunc7 = new PowerEase();
          easeFunc7.EasingMode = EasingMode.EaseIn;
          easeFunc7.Power = 3.0;
          DoubleAnimation doubleAnimation9 = WaAnimations.CreateDoubleAnimation(0.0, 100.0, duration7, (DependencyObject) null, (EasingFunctionBase) easeFunc7, "(UIElement.Projection).(PlaneProjection.RotationY)");
          doubleAnimationList7.Add(doubleAnimation9);
          List<DoubleAnimation> doubleAnimationList8 = animations;
          TimeSpan duration8 = TimeSpan.FromMilliseconds(350.0);
          PowerEase easeFunc8 = new PowerEase();
          easeFunc8.EasingMode = EasingMode.EaseIn;
          easeFunc8.Power = 6.0;
          DoubleAnimation doubleAnimation10 = WaAnimations.Fade(WaAnimations.FadeType.FadeOut, duration8, easeFunc: (EasingFunctionBase) easeFunc8);
          doubleAnimationList8.Add(doubleAnimation10);
          break;
        case PageTransitionAnimation.TurnstileForwardIn:
          List<DoubleAnimation> doubleAnimationList9 = animations;
          TimeSpan duration9 = TimeSpan.FromMilliseconds(350.0);
          PowerEase easeFunc9 = new PowerEase();
          easeFunc9.EasingMode = EasingMode.EaseOut;
          easeFunc9.Power = 3.0;
          DoubleAnimation doubleAnimation11 = WaAnimations.CreateDoubleAnimation(-100.0, 0.0, duration9, (DependencyObject) null, (EasingFunctionBase) easeFunc9, "(UIElement.Projection).(PlaneProjection.RotationY)");
          doubleAnimationList9.Add(doubleAnimation11);
          List<DoubleAnimation> doubleAnimationList10 = animations;
          TimeSpan duration10 = TimeSpan.FromMilliseconds(350.0);
          PowerEase easeFunc10 = new PowerEase();
          easeFunc10.EasingMode = EasingMode.EaseIn;
          easeFunc10.Power = 6.0;
          DoubleAnimation doubleAnimation12 = WaAnimations.Fade(WaAnimations.FadeType.FadeIn, duration10, easeFunc: (EasingFunctionBase) easeFunc10);
          doubleAnimationList10.Add(doubleAnimation12);
          break;
        case PageTransitionAnimation.TurnstileBackwardOut:
          List<DoubleAnimation> doubleAnimationList11 = animations;
          TimeSpan duration11 = TimeSpan.FromMilliseconds(350.0);
          PowerEase easeFunc11 = new PowerEase();
          easeFunc11.EasingMode = EasingMode.EaseIn;
          easeFunc11.Power = 3.0;
          DoubleAnimation doubleAnimation13 = WaAnimations.CreateDoubleAnimation(0.0, -100.0, duration11, (DependencyObject) null, (EasingFunctionBase) easeFunc11, "(UIElement.Projection).(PlaneProjection.RotationY)");
          doubleAnimationList11.Add(doubleAnimation13);
          List<DoubleAnimation> doubleAnimationList12 = animations;
          TimeSpan duration12 = TimeSpan.FromMilliseconds(350.0);
          PowerEase easeFunc12 = new PowerEase();
          easeFunc12.EasingMode = EasingMode.EaseIn;
          easeFunc12.Power = 6.0;
          DoubleAnimation doubleAnimation14 = WaAnimations.Fade(WaAnimations.FadeType.FadeOut, duration12, easeFunc: (EasingFunctionBase) easeFunc12);
          doubleAnimationList12.Add(doubleAnimation14);
          break;
        case PageTransitionAnimation.TurnstileBackwardIn:
          List<DoubleAnimation> doubleAnimationList13 = animations;
          TimeSpan duration13 = TimeSpan.FromMilliseconds(350.0);
          PowerEase easeFunc13 = new PowerEase();
          easeFunc13.EasingMode = EasingMode.EaseOut;
          easeFunc13.Power = 3.0;
          DoubleAnimation doubleAnimation15 = WaAnimations.CreateDoubleAnimation(100.0, 0.0, duration13, (DependencyObject) null, (EasingFunctionBase) easeFunc13, "(UIElement.Projection).(PlaneProjection.RotationY)");
          doubleAnimationList13.Add(doubleAnimation15);
          List<DoubleAnimation> doubleAnimationList14 = animations;
          TimeSpan duration14 = TimeSpan.FromMilliseconds(350.0);
          PowerEase easeFunc14 = new PowerEase();
          easeFunc14.EasingMode = EasingMode.EaseIn;
          easeFunc14.Power = 6.0;
          DoubleAnimation doubleAnimation16 = WaAnimations.Fade(WaAnimations.FadeType.FadeIn, duration14, easeFunc: (EasingFunctionBase) easeFunc14);
          doubleAnimationList14.Add(doubleAnimation16);
          break;
        case PageTransitionAnimation.SwivelIn:
          List<DoubleAnimation> doubleAnimationList15 = animations;
          TimeSpan duration15 = TimeSpan.FromMilliseconds(300.0);
          ExponentialEase easeFunc15 = new ExponentialEase();
          easeFunc15.EasingMode = EasingMode.EaseIn;
          easeFunc15.Exponent = 6.0;
          DoubleAnimation doubleAnimation17 = WaAnimations.CreateDoubleAnimation(-95.0, 0.0, duration15, (DependencyObject) null, (EasingFunctionBase) easeFunc15, "(UIElement.Projection).(PlaneProjection.RotationX)");
          doubleAnimationList15.Add(doubleAnimation17);
          break;
        case PageTransitionAnimation.SwivelOut:
          List<DoubleAnimation> doubleAnimationList16 = animations;
          TimeSpan duration16 = TimeSpan.FromMilliseconds(300.0);
          ExponentialEase easeFunc16 = new ExponentialEase();
          easeFunc16.EasingMode = EasingMode.EaseIn;
          easeFunc16.Exponent = 6.0;
          DoubleAnimation doubleAnimation18 = WaAnimations.CreateDoubleAnimation(0.0, 90.0, duration16, (DependencyObject) null, (EasingFunctionBase) easeFunc16, "(UIElement.Projection).(PlaneProjection.RotationX)");
          doubleAnimationList16.Add(doubleAnimation18);
          break;
      }
      return WaAnimations.CreateStoryboard((IEnumerable<DoubleAnimation>) animations);
    }

    public static void AnimateTo(
      FrameworkElement target,
      double? translateFromX,
      double? translateToX,
      double? translateFromY,
      double? translateToY,
      double? scaleFromX,
      double? scaleToX,
      double? scaleFromY,
      double? scaleToY,
      Action onComplete)
    {
      if (!(target.RenderTransform is CompositeTransform))
        target.RenderTransform = (Transform) new CompositeTransform();
      Storyboard sb = new Storyboard();
      Duration duration = new Duration(TimeSpan.FromMilliseconds(400.0));
      ExponentialEase exponentialEase1 = new ExponentialEase();
      exponentialEase1.Exponent = 6.0;
      exponentialEase1.EasingMode = EasingMode.EaseOut;
      ExponentialEase exponentialEase2 = exponentialEase1;
      if (translateToX.HasValue)
      {
        DoubleAnimation doubleAnimation = new DoubleAnimation();
        doubleAnimation.From = translateFromX;
        doubleAnimation.To = translateToX;
        doubleAnimation.Duration = duration;
        doubleAnimation.EasingFunction = (IEasingFunction) exponentialEase2;
        DoubleAnimation element = doubleAnimation;
        Storyboard.SetTarget((Timeline) element, (DependencyObject) target);
        Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)", new object[0]));
        sb.Children.Add((Timeline) element);
      }
      if (translateToY.HasValue)
      {
        DoubleAnimation doubleAnimation = new DoubleAnimation();
        doubleAnimation.From = translateFromY;
        doubleAnimation.To = translateToY;
        doubleAnimation.Duration = duration;
        doubleAnimation.EasingFunction = (IEasingFunction) exponentialEase2;
        DoubleAnimation element = doubleAnimation;
        Storyboard.SetTarget((Timeline) element, (DependencyObject) target);
        Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)", new object[0]));
        sb.Children.Add((Timeline) element);
      }
      if (scaleToX.HasValue)
      {
        DoubleAnimation doubleAnimation = new DoubleAnimation();
        doubleAnimation.From = scaleFromX;
        doubleAnimation.To = scaleToX;
        doubleAnimation.Duration = duration;
        doubleAnimation.EasingFunction = (IEasingFunction) exponentialEase2;
        DoubleAnimation element = doubleAnimation;
        Storyboard.SetTarget((Timeline) element, (DependencyObject) target);
        Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)", new object[0]));
        sb.Children.Add((Timeline) element);
      }
      if (scaleToY.HasValue)
      {
        DoubleAnimation doubleAnimation = new DoubleAnimation();
        doubleAnimation.From = scaleFromY;
        doubleAnimation.To = scaleToY;
        doubleAnimation.Duration = duration;
        doubleAnimation.EasingFunction = (IEasingFunction) exponentialEase2;
        DoubleAnimation element = doubleAnimation;
        Storyboard.SetTarget((Timeline) element, (DependencyObject) target);
        Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)", new object[0]));
        sb.Children.Add((Timeline) element);
      }
      Storyboarder.Perform(sb, false, onComplete);
    }

    public static IDisposable PerformFade(
      DependencyObject target,
      WaAnimations.FadeType fadeType,
      TimeSpan duration,
      Action onComplete)
    {
      return Storyboarder.PerformWithDisposable(WaAnimations.CreateStoryboard(WaAnimations.Fade(fadeType, duration, target)), onComplete: onComplete, onDisposing: onComplete, context: "perform fade");
    }

    public enum FadeType
    {
      FadeIn,
      FadeOut,
    }

    public class FadeOutAndInTransition
    {
      private IDisposable sbSub;
      private Storyboard sb0;
      private Storyboard sb1;
      private UIElement target;
      private double regularOpacity = 1.0;
      private Action update;

      public FadeOutAndInTransition(UIElement target, Action update, double regularOpacity = 1.0)
      {
        this.target = target;
        this.update = update;
        this.regularOpacity = regularOpacity;
      }

      public void Perform()
      {
        if (this.update == null)
          return;
        if (this.sbSub != null)
        {
          this.update();
        }
        else
        {
          if (this.sb0 == null || this.sb1 == null)
          {
            DoubleAnimation animation1 = WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(300.0), (DependencyObject) this.target);
            animation1.From = new double?(this.regularOpacity);
            this.sb0 = WaAnimations.CreateStoryboard(animation1);
            DoubleAnimation animation2 = WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(300.0), (DependencyObject) this.target);
            animation2.To = new double?(this.regularOpacity);
            this.sb1 = WaAnimations.CreateStoryboard(animation2);
          }
          this.target.Opacity = this.regularOpacity;
          this.sbSub = Storyboarder.PerformWithDisposable(this.sb0, (DependencyObject) this.target, onComplete: (Action) (() =>
          {
            this.update();
            this.target.Opacity = 0.0;
            this.sbSub = Storyboarder.PerformWithDisposable(this.sb1, onComplete: (Action) (() =>
            {
              this.target.Opacity = this.regularOpacity;
              this.sbSub = (IDisposable) null;
            }), callOnCompleteOnDisposing: true, context: "fade out/in part 2");
          }), onDisposing: (Action) (() =>
          {
            this.sbSub = (IDisposable) null;
            this.update();
            this.target.Opacity = this.regularOpacity;
          }), context: "fade out/in part 1");
        }
      }

      public void Abort()
      {
        this.sbSub.SafeDispose();
        this.sbSub = (IDisposable) null;
      }
    }
  }
}
