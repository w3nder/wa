// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.TurnstileFeatherEffect
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public sealed class TurnstileFeatherEffect : DependencyObject
  {
    private const double FeatheringCenterOfRotationX = -0.2;
    private const double ForwardInFeatheringDuration = 350.0;
    private const double ForwardInFeatheringAngle = -80.0;
    private const double ForwardInFeatheringDelay = 40.0;
    private const double ForwardOutFeatheringDuration = 250.0;
    private const double ForwardOutFeatheringAngle = 50.0;
    private const double ForwardOutFeatheringDelay = 50.0;
    private const double BackwardInFeatheringDuration = 350.0;
    private const double BackwardInFeatheringAngle = 50.0;
    private const double BackwardInFeatheringDelay = 50.0;
    private const double BackwardOutFeatheringDuration = 250.0;
    private const double BackwardOutFeatheringAngle = -80.0;
    private const double BackwardOutFeatheringDelay = 40.0;
    private static readonly ExponentialEase TurnstileFeatheringExponentialEaseIn;
    private static readonly ExponentialEase TurnstileFeatheringExponentialEaseOut;
    private static readonly PropertyPath RotationYPropertyPath;
    private static readonly PropertyPath OpacityPropertyPath;
    private static readonly Point Origin;
    private static Dictionary<PhoneApplicationPage, List<WeakReference>> _pagesToReferences;
    private static IList<WeakReference> _featheringTargets;
    private static bool _pendingRestore;
    private static IList<Type> _nonPermittedTypes;
    public static readonly DependencyProperty FeatheringIndexProperty;
    private static readonly DependencyProperty ParentPageProperty;
    private static readonly DependencyProperty IsSubscribedProperty;
    private static readonly DependencyProperty HasEventsAttachedProperty;
    private static readonly DependencyProperty OriginalProjectionProperty;
    private static readonly DependencyProperty OriginalRenderTransformProperty;
    private static readonly DependencyProperty OriginalOpacityProperty;

    public static IList<Type> NonPermittedTypes => TurnstileFeatherEffect._nonPermittedTypes;

    public static int GetFeatheringIndex(DependencyObject obj)
    {
      return (int) obj.GetValue(TurnstileFeatherEffect.FeatheringIndexProperty);
    }

    public static void SetFeatheringIndex(DependencyObject obj, int value)
    {
      obj.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, (object) value);
    }

    private static void OnFeatheringIndexPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(obj is FrameworkElement target))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The dependency object must be of the type {0}.", (object) typeof (FrameworkElement)));
      TurnstileFeatherEffect.CheckForTypePermission((object) target);
      if ((int) e.NewValue < 0)
      {
        if (TurnstileFeatherEffect.GetHasEventsAttached((DependencyObject) target))
        {
          target.SizeChanged -= new SizeChangedEventHandler(TurnstileFeatherEffect.Target_SizeChanged);
          target.Unloaded -= new RoutedEventHandler(TurnstileFeatherEffect.Target_Unloaded);
          TurnstileFeatherEffect.SetHasEventsAttached((DependencyObject) target, false);
        }
        TurnstileFeatherEffect.UnsubscribeFrameworkElement(target);
      }
      else
      {
        if (!TurnstileFeatherEffect.GetHasEventsAttached((DependencyObject) target))
        {
          target.SizeChanged += new SizeChangedEventHandler(TurnstileFeatherEffect.Target_SizeChanged);
          target.Unloaded += new RoutedEventHandler(TurnstileFeatherEffect.Target_Unloaded);
          TurnstileFeatherEffect.SetHasEventsAttached((DependencyObject) target, true);
        }
        TurnstileFeatherEffect.SubscribeFrameworkElement(target);
      }
    }

    private static PhoneApplicationPage GetParentPage(DependencyObject obj)
    {
      return (PhoneApplicationPage) obj.GetValue(TurnstileFeatherEffect.ParentPageProperty);
    }

    private static void SetParentPage(DependencyObject obj, PhoneApplicationPage value)
    {
      obj.SetValue(TurnstileFeatherEffect.ParentPageProperty, (object) value);
    }

    private static void OnParentPagePropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement target = (FrameworkElement) obj;
      PhoneApplicationPage oldValue = (PhoneApplicationPage) e.OldValue;
      PhoneApplicationPage newValue = (PhoneApplicationPage) e.NewValue;
      if (newValue != null)
      {
        List<WeakReference> references;
        if (!TurnstileFeatherEffect._pagesToReferences.TryGetValue(newValue, out references))
        {
          references = new List<WeakReference>();
          TurnstileFeatherEffect._pagesToReferences.Add(newValue, references);
        }
        else
          WeakReferenceHelper.RemoveNullTargetReferences((IList<WeakReference>) references);
        if (!WeakReferenceHelper.ContainsTarget((IEnumerable<WeakReference>) references, (object) target))
          references.Add(new WeakReference((object) target));
        references.Sort(new Comparison<WeakReference>(TurnstileFeatherEffect.SortReferencesByIndex));
      }
      else
      {
        List<WeakReference> references;
        if (!TurnstileFeatherEffect._pagesToReferences.TryGetValue(oldValue, out references))
          return;
        WeakReferenceHelper.TryRemoveTarget((IList<WeakReference>) references, (object) target);
        if (references.Count != 0)
          return;
        TurnstileFeatherEffect._pagesToReferences.Remove(oldValue);
      }
    }

    private static bool GetIsSubscribed(DependencyObject obj)
    {
      return (bool) obj.GetValue(TurnstileFeatherEffect.IsSubscribedProperty);
    }

    private static void SetIsSubscribed(DependencyObject obj, bool value)
    {
      obj.SetValue(TurnstileFeatherEffect.IsSubscribedProperty, (object) value);
    }

    private static bool GetHasEventsAttached(DependencyObject obj)
    {
      return (bool) obj.GetValue(TurnstileFeatherEffect.HasEventsAttachedProperty);
    }

    private static void SetHasEventsAttached(DependencyObject obj, bool value)
    {
      obj.SetValue(TurnstileFeatherEffect.HasEventsAttachedProperty, (object) value);
    }

    private static Projection GetOriginalProjection(DependencyObject obj)
    {
      return (Projection) obj.GetValue(TurnstileFeatherEffect.OriginalProjectionProperty);
    }

    private static void SetOriginalProjection(DependencyObject obj, Projection value)
    {
      obj.SetValue(TurnstileFeatherEffect.OriginalProjectionProperty, (object) value);
    }

    private static Transform GetOriginalRenderTransform(DependencyObject obj)
    {
      return (Transform) obj.GetValue(TurnstileFeatherEffect.OriginalRenderTransformProperty);
    }

    private static void SetOriginalRenderTransform(DependencyObject obj, Transform value)
    {
      obj.SetValue(TurnstileFeatherEffect.OriginalRenderTransformProperty, (object) value);
    }

    private static double GetOriginalOpacity(DependencyObject obj)
    {
      return (double) obj.GetValue(TurnstileFeatherEffect.OriginalOpacityProperty);
    }

    private static void SetOriginalOpacity(DependencyObject obj, double value)
    {
      obj.SetValue(TurnstileFeatherEffect.OriginalOpacityProperty, (object) value);
    }

    private static void Target_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      TurnstileFeatherEffect.SubscribeFrameworkElement((FrameworkElement) sender);
    }

    private static void Target_Unloaded(object sender, RoutedEventArgs e)
    {
      TurnstileFeatherEffect.UnsubscribeFrameworkElement((FrameworkElement) sender);
    }

    private static void CheckForTypePermission(object obj)
    {
      Type type = obj.GetType();
      if (TurnstileFeatherEffect.NonPermittedTypes.Contains(type))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Objects of the type {0} cannot be feathered.", (object) type));
    }

    private static int SortReferencesByIndex(WeakReference x, WeakReference y)
    {
      DependencyObject target1 = x.Target as DependencyObject;
      DependencyObject target2 = y.Target as DependencyObject;
      return target1 == null ? (target2 == null ? 0 : -1) : (target2 == null ? 1 : TurnstileFeatherEffect.GetFeatheringIndex(target1).CompareTo(TurnstileFeatherEffect.GetFeatheringIndex(target2)));
    }

    private static IList<WeakReference> GetTargetsToAnimate()
    {
      List<WeakReference> items = new List<WeakReference>();
      PhoneApplicationPage key = (PhoneApplicationPage) null;
      if (Application.Current.RootVisual is PhoneApplicationFrame rootVisual)
        key = rootVisual.Content as PhoneApplicationPage;
      if (key == null)
        return (IList<WeakReference>) null;
      List<WeakReference> weakReferenceList;
      if (!TurnstileFeatherEffect._pagesToReferences.TryGetValue(key, out weakReferenceList))
        return (IList<WeakReference>) null;
      foreach (WeakReference weakReference in weakReferenceList)
      {
        if (weakReference.Target is FrameworkElement target && TurnstileFeatherEffect.IsOnScreen(target))
        {
          ListBox target1 = weakReference.Target as ListBox;
          LongListSelector target2 = weakReference.Target as LongListSelector;
          Pivot target3 = weakReference.Target as Pivot;
          if (target1 != null)
            target1.GetItemsInViewPort((IList<WeakReference>) items);
          else if (target2 != null)
            target2.GetItemsInViewPort((IList<WeakReference>) items);
          else if (target3 != null)
          {
            ContentPresenter logicalChildByType1 = target3.GetFirstLogicalChildByType<ContentPresenter>(false);
            if (logicalChildByType1 != null)
              items.Add(new WeakReference((object) logicalChildByType1));
            PivotHeadersControl logicalChildByType2 = target3.GetFirstLogicalChildByType<PivotHeadersControl>(false);
            if (logicalChildByType2 != null)
              items.Add(new WeakReference((object) logicalChildByType2));
          }
          else
            items.Add(weakReference);
        }
      }
      return (IList<WeakReference>) items;
    }

    private static void SubscribeFrameworkElement(FrameworkElement target)
    {
      if (TurnstileFeatherEffect.GetIsSubscribed((DependencyObject) target))
        return;
      PhoneApplicationPage parentByType = target.GetParentByType<PhoneApplicationPage>();
      if (parentByType == null)
        return;
      TurnstileFeatherEffect.SetParentPage((DependencyObject) target, parentByType);
      TurnstileFeatherEffect.SetIsSubscribed((DependencyObject) target, true);
    }

    private static void UnsubscribeFrameworkElement(FrameworkElement target)
    {
      if (!TurnstileFeatherEffect.GetIsSubscribed((DependencyObject) target))
        return;
      TurnstileFeatherEffect.SetParentPage((DependencyObject) target, (PhoneApplicationPage) null);
      TurnstileFeatherEffect.SetIsSubscribed((DependencyObject) target, false);
    }

    private static bool TryAttachProjectionAndTransform(
      PhoneApplicationFrame root,
      FrameworkElement element)
    {
      GeneralTransform visual;
      try
      {
        visual = element.TransformToVisual((UIElement) root);
      }
      catch (ArgumentException ex)
      {
        return false;
      }
      double num1 = visual.Transform(TurnstileFeatherEffect.Origin).Y + element.ActualHeight / 2.0;
      double num2 = root.ActualHeight / 2.0 - num1;
      TurnstileFeatherEffect.SetOriginalProjection((DependencyObject) element, element.Projection);
      TurnstileFeatherEffect.SetOriginalRenderTransform((DependencyObject) element, element.RenderTransform);
      element.Projection = (Projection) new PlaneProjection()
      {
        GlobalOffsetY = (num2 * -1.0),
        CenterOfRotationX = -0.2
      };
      Transform renderTransform = element.RenderTransform;
      TranslateTransform translateTransform = new TranslateTransform();
      translateTransform.Y = num2;
      TransformGroup transformGroup = new TransformGroup();
      transformGroup.Children.Add(renderTransform);
      transformGroup.Children.Add((Transform) translateTransform);
      element.RenderTransform = (Transform) transformGroup;
      return true;
    }

    private static void RestoreProjectionsAndTransforms()
    {
      if (TurnstileFeatherEffect._featheringTargets == null || !TurnstileFeatherEffect._pendingRestore)
        return;
      foreach (WeakReference featheringTarget in (IEnumerable<WeakReference>) TurnstileFeatherEffect._featheringTargets)
      {
        if (featheringTarget.Target is FrameworkElement target)
        {
          Projection originalProjection = TurnstileFeatherEffect.GetOriginalProjection((DependencyObject) target);
          Transform originalRenderTransform = TurnstileFeatherEffect.GetOriginalRenderTransform((DependencyObject) target);
          target.Projection = originalProjection;
          target.RenderTransform = originalRenderTransform;
        }
      }
      TurnstileFeatherEffect._pendingRestore = false;
    }

    private static bool IsOnScreen(FrameworkElement element)
    {
      if (!(Application.Current.RootVisual is PhoneApplicationFrame rootVisual))
        return false;
      double actualHeight = rootVisual.ActualHeight;
      double actualWidth = rootVisual.ActualWidth;
      GeneralTransform visual;
      try
      {
        visual = element.TransformToVisual((UIElement) rootVisual);
      }
      catch (ArgumentException ex)
      {
        return false;
      }
      Rect rect = new Rect(visual.Transform(TurnstileFeatherEffect.Origin), visual.Transform(new Point(element.ActualWidth, element.ActualHeight)));
      bool flag = false;
      IList<FrameworkElement> list = (IList<FrameworkElement>) element.GetVisualAncestors().ToList<FrameworkElement>();
      if (list != null)
      {
        for (int index = 0; index < list.Count; ++index)
        {
          if (list[index].Opacity <= 0.001)
          {
            flag = true;
            break;
          }
        }
      }
      return rect.Bottom > 0.0 && rect.Top < actualHeight && rect.Right > 0.0 && rect.Left < actualWidth && !flag;
    }

    private static void ComposeForwardInStoryboard(Storyboard storyboard)
    {
      int num = 0;
      PhoneApplicationFrame rootVisual = Application.Current.RootVisual as PhoneApplicationFrame;
      foreach (WeakReference featheringTarget in (IEnumerable<WeakReference>) TurnstileFeatherEffect._featheringTargets)
      {
        FrameworkElement target = (FrameworkElement) featheringTarget.Target;
        double opacity = target.Opacity;
        TurnstileFeatherEffect.SetOriginalOpacity((DependencyObject) target, opacity);
        target.Opacity = 0.0;
        if (TurnstileFeatherEffect.TryAttachProjectionAndTransform(rootVisual, target))
        {
          DoubleAnimation doubleAnimation1 = new DoubleAnimation();
          doubleAnimation1.Duration = (Duration) TimeSpan.FromMilliseconds(350.0);
          doubleAnimation1.From = new double?(-80.0);
          doubleAnimation1.To = new double?(0.0);
          doubleAnimation1.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(40.0 * (double) num));
          doubleAnimation1.EasingFunction = (IEasingFunction) TurnstileFeatherEffect.TurnstileFeatheringExponentialEaseOut;
          DoubleAnimation element1 = doubleAnimation1;
          Storyboard.SetTarget((Timeline) element1, (DependencyObject) target);
          Storyboard.SetTargetProperty((Timeline) element1, TurnstileFeatherEffect.RotationYPropertyPath);
          storyboard.Children.Add((Timeline) element1);
          DoubleAnimation doubleAnimation2 = new DoubleAnimation();
          doubleAnimation2.Duration = (Duration) TimeSpan.Zero;
          doubleAnimation2.From = new double?(0.0);
          doubleAnimation2.To = new double?(TurnstileFeatherEffect.GetOriginalOpacity((DependencyObject) target));
          doubleAnimation2.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(40.0 * (double) num));
          DoubleAnimation element2 = doubleAnimation2;
          Storyboard.SetTarget((Timeline) element2, (DependencyObject) target);
          Storyboard.SetTargetProperty((Timeline) element2, TurnstileFeatherEffect.OpacityPropertyPath);
          storyboard.Children.Add((Timeline) element2);
          ++num;
        }
      }
    }

    private static void ComposeForwardOutStoryboard(Storyboard storyboard)
    {
      int num = 0;
      PhoneApplicationFrame rootVisual = Application.Current.RootVisual as PhoneApplicationFrame;
      foreach (WeakReference featheringTarget in (IEnumerable<WeakReference>) TurnstileFeatherEffect._featheringTargets)
      {
        FrameworkElement target = (FrameworkElement) featheringTarget.Target;
        double opacity = target.Opacity;
        TurnstileFeatherEffect.SetOriginalOpacity((DependencyObject) target, opacity);
        if (TurnstileFeatherEffect.TryAttachProjectionAndTransform(rootVisual, target))
        {
          DoubleAnimation doubleAnimation1 = new DoubleAnimation();
          doubleAnimation1.Duration = (Duration) TimeSpan.FromMilliseconds(250.0);
          doubleAnimation1.From = new double?(0.0);
          doubleAnimation1.To = new double?(50.0);
          doubleAnimation1.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(50.0 * (double) num));
          doubleAnimation1.EasingFunction = (IEasingFunction) TurnstileFeatherEffect.TurnstileFeatheringExponentialEaseIn;
          DoubleAnimation element1 = doubleAnimation1;
          Storyboard.SetTarget((Timeline) element1, (DependencyObject) target);
          Storyboard.SetTargetProperty((Timeline) element1, TurnstileFeatherEffect.RotationYPropertyPath);
          storyboard.Children.Add((Timeline) element1);
          DoubleAnimation doubleAnimation2 = new DoubleAnimation();
          doubleAnimation2.Duration = (Duration) TimeSpan.Zero;
          doubleAnimation2.From = new double?(opacity);
          doubleAnimation2.To = new double?(0.0);
          doubleAnimation2.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(50.0 * (double) num + 250.0));
          DoubleAnimation element2 = doubleAnimation2;
          Storyboard.SetTarget((Timeline) element2, (DependencyObject) target);
          Storyboard.SetTargetProperty((Timeline) element2, TurnstileFeatherEffect.OpacityPropertyPath);
          storyboard.Children.Add((Timeline) element2);
          ++num;
        }
      }
    }

    private static void ComposeBackwardInStoryboard(Storyboard storyboard)
    {
      int num = 0;
      PhoneApplicationFrame rootVisual = Application.Current.RootVisual as PhoneApplicationFrame;
      foreach (WeakReference featheringTarget in (IEnumerable<WeakReference>) TurnstileFeatherEffect._featheringTargets)
      {
        FrameworkElement target = (FrameworkElement) featheringTarget.Target;
        double opacity = target.Opacity;
        TurnstileFeatherEffect.SetOriginalOpacity((DependencyObject) target, opacity);
        target.Opacity = 0.0;
        if (TurnstileFeatherEffect.TryAttachProjectionAndTransform(rootVisual, target))
        {
          DoubleAnimation doubleAnimation1 = new DoubleAnimation();
          doubleAnimation1.Duration = (Duration) TimeSpan.FromMilliseconds(350.0);
          doubleAnimation1.From = new double?(50.0);
          doubleAnimation1.To = new double?(0.0);
          doubleAnimation1.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(50.0 * (double) num));
          doubleAnimation1.EasingFunction = (IEasingFunction) TurnstileFeatherEffect.TurnstileFeatheringExponentialEaseOut;
          DoubleAnimation element1 = doubleAnimation1;
          Storyboard.SetTarget((Timeline) element1, (DependencyObject) target);
          Storyboard.SetTargetProperty((Timeline) element1, TurnstileFeatherEffect.RotationYPropertyPath);
          storyboard.Children.Add((Timeline) element1);
          DoubleAnimation doubleAnimation2 = new DoubleAnimation();
          doubleAnimation2.Duration = (Duration) TimeSpan.Zero;
          doubleAnimation2.From = new double?(0.0);
          doubleAnimation2.To = new double?(opacity);
          doubleAnimation2.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(50.0 * (double) num));
          DoubleAnimation element2 = doubleAnimation2;
          Storyboard.SetTarget((Timeline) element2, (DependencyObject) target);
          Storyboard.SetTargetProperty((Timeline) element2, TurnstileFeatherEffect.OpacityPropertyPath);
          storyboard.Children.Add((Timeline) element2);
          ++num;
        }
      }
    }

    private static void ComposeBackwardOutStoryboard(Storyboard storyboard)
    {
      int num = 0;
      PhoneApplicationFrame rootVisual = Application.Current.RootVisual as PhoneApplicationFrame;
      foreach (WeakReference featheringTarget in (IEnumerable<WeakReference>) TurnstileFeatherEffect._featheringTargets)
      {
        FrameworkElement target = (FrameworkElement) featheringTarget.Target;
        double opacity = target.Opacity;
        TurnstileFeatherEffect.SetOriginalOpacity((DependencyObject) target, opacity);
        if (TurnstileFeatherEffect.TryAttachProjectionAndTransform(rootVisual, target))
        {
          DoubleAnimation doubleAnimation1 = new DoubleAnimation();
          doubleAnimation1.Duration = (Duration) TimeSpan.FromMilliseconds(250.0);
          doubleAnimation1.From = new double?(0.0);
          doubleAnimation1.To = new double?(-80.0);
          doubleAnimation1.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(40.0 * (double) num));
          doubleAnimation1.EasingFunction = (IEasingFunction) TurnstileFeatherEffect.TurnstileFeatheringExponentialEaseIn;
          DoubleAnimation element1 = doubleAnimation1;
          Storyboard.SetTarget((Timeline) element1, (DependencyObject) target);
          Storyboard.SetTargetProperty((Timeline) element1, TurnstileFeatherEffect.RotationYPropertyPath);
          storyboard.Children.Add((Timeline) element1);
          DoubleAnimation doubleAnimation2 = new DoubleAnimation();
          doubleAnimation2.Duration = (Duration) TimeSpan.Zero;
          doubleAnimation2.From = new double?(opacity);
          doubleAnimation2.To = new double?(0.0);
          doubleAnimation2.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(40.0 * (double) num + 250.0));
          DoubleAnimation element2 = doubleAnimation2;
          Storyboard.SetTarget((Timeline) element2, (DependencyObject) target);
          Storyboard.SetTargetProperty((Timeline) element2, TurnstileFeatherEffect.OpacityPropertyPath);
          storyboard.Children.Add((Timeline) element2);
          ++num;
        }
      }
    }

    internal static void ComposeStoryboard(
      Storyboard storyboard,
      TimeSpan? beginTime,
      TurnstileFeatherTransitionMode mode)
    {
      TurnstileFeatherEffect.RestoreProjectionsAndTransforms();
      TurnstileFeatherEffect._featheringTargets = TurnstileFeatherEffect.GetTargetsToAnimate();
      if (TurnstileFeatherEffect._featheringTargets == null)
        return;
      TurnstileFeatherEffect._pendingRestore = true;
      switch (mode)
      {
        case TurnstileFeatherTransitionMode.ForwardIn:
          TurnstileFeatherEffect.ComposeForwardInStoryboard(storyboard);
          break;
        case TurnstileFeatherTransitionMode.ForwardOut:
          TurnstileFeatherEffect.ComposeForwardOutStoryboard(storyboard);
          break;
        case TurnstileFeatherTransitionMode.BackwardIn:
          TurnstileFeatherEffect.ComposeBackwardInStoryboard(storyboard);
          break;
        case TurnstileFeatherTransitionMode.BackwardOut:
          TurnstileFeatherEffect.ComposeBackwardOutStoryboard(storyboard);
          break;
      }
      storyboard.BeginTime = beginTime;
      storyboard.Completed += (EventHandler) ((s, e) =>
      {
        foreach (WeakReference featheringTarget in (IEnumerable<WeakReference>) TurnstileFeatherEffect._featheringTargets)
        {
          FrameworkElement target = (FrameworkElement) featheringTarget.Target;
          double originalOpacity = TurnstileFeatherEffect.GetOriginalOpacity((DependencyObject) target);
          target.Opacity = originalOpacity;
        }
        TurnstileFeatherEffect.RestoreProjectionsAndTransforms();
      });
    }

    static TurnstileFeatherEffect()
    {
      ExponentialEase exponentialEase1 = new ExponentialEase();
      exponentialEase1.EasingMode = EasingMode.EaseIn;
      exponentialEase1.Exponent = 6.0;
      TurnstileFeatherEffect.TurnstileFeatheringExponentialEaseIn = exponentialEase1;
      ExponentialEase exponentialEase2 = new ExponentialEase();
      exponentialEase2.EasingMode = EasingMode.EaseOut;
      exponentialEase2.Exponent = 6.0;
      TurnstileFeatherEffect.TurnstileFeatheringExponentialEaseOut = exponentialEase2;
      TurnstileFeatherEffect.RotationYPropertyPath = new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)", new object[0]);
      TurnstileFeatherEffect.OpacityPropertyPath = new PropertyPath("(UIElement.Opacity)", new object[0]);
      TurnstileFeatherEffect.Origin = new Point(0.0, 0.0);
      TurnstileFeatherEffect._pagesToReferences = new Dictionary<PhoneApplicationPage, List<WeakReference>>();
      TurnstileFeatherEffect._nonPermittedTypes = (IList<Type>) new List<Type>()
      {
        typeof (PhoneApplicationFrame),
        typeof (PhoneApplicationPage),
        typeof (PivotItem),
        typeof (Panorama),
        typeof (PanoramaItem)
      };
      TurnstileFeatherEffect.FeatheringIndexProperty = DependencyProperty.RegisterAttached("FeatheringIndex", typeof (int), typeof (TurnstileFeatherEffect), new PropertyMetadata((object) -1, new PropertyChangedCallback(TurnstileFeatherEffect.OnFeatheringIndexPropertyChanged)));
      TurnstileFeatherEffect.ParentPageProperty = DependencyProperty.RegisterAttached("ParentPage", typeof (PhoneApplicationPage), typeof (TurnstileFeatherEffect), new PropertyMetadata((object) null, new PropertyChangedCallback(TurnstileFeatherEffect.OnParentPagePropertyChanged)));
      TurnstileFeatherEffect.IsSubscribedProperty = DependencyProperty.RegisterAttached("IsSubscribed", typeof (bool), typeof (TurnstileFeatherEffect), new PropertyMetadata((object) false));
      TurnstileFeatherEffect.HasEventsAttachedProperty = DependencyProperty.RegisterAttached("HasEventsAttached", typeof (bool), typeof (TurnstileFeatherEffect), new PropertyMetadata((object) false));
      TurnstileFeatherEffect.OriginalProjectionProperty = DependencyProperty.RegisterAttached("OriginalProjection", typeof (Projection), typeof (TurnstileFeatherEffect), new PropertyMetadata((PropertyChangedCallback) null));
      TurnstileFeatherEffect.OriginalRenderTransformProperty = DependencyProperty.RegisterAttached("OriginalRenderTransform", typeof (Transform), typeof (TurnstileFeatherEffect), new PropertyMetadata((PropertyChangedCallback) null));
      TurnstileFeatherEffect.OriginalOpacityProperty = DependencyProperty.RegisterAttached("OriginalOpacity", typeof (double), typeof (TurnstileFeatherEffect), new PropertyMetadata((object) 0.0));
    }
  }
}
