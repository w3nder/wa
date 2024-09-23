// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.OpacityToggleButton
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class OpacityToggleButton : ToggleButtonBase
  {
    private const string ButtonForegroundName = "ButtonForeground";
    public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register(nameof (AnimationDuration), typeof (TimeSpan), typeof (OpacityToggleButton), new PropertyMetadata((object) TimeSpan.FromMilliseconds(100.0)));
    public static readonly DependencyProperty UncheckedOpacityProperty = DependencyProperty.Register(nameof (UncheckedOpacity), typeof (double), typeof (OpacityToggleButton), new PropertyMetadata((object) 0.5));
    public static readonly DependencyProperty CheckedOpacityProperty = DependencyProperty.Register(nameof (CheckedOpacity), typeof (double), typeof (OpacityToggleButton), new PropertyMetadata((object) 1.0));

    public OpacityToggleButton()
    {
      ((Control) this).DefaultStyleKey = (object) typeof (OpacityToggleButton);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (!(((Control) this).GetTemplateChild("ButtonForeground") is FrameworkElement templateChild1))
        return;
      VisualState templateChild2 = ((Control) this).GetTemplateChild("Checked") as VisualState;
      VisualState templateChild3 = ((Control) this).GetTemplateChild("Unchecked") as VisualState;
      if (templateChild2 != null)
      {
        templateChild2.Storyboard = new Storyboard();
        this.CreateDoubleAnimations(templateChild2.Storyboard, (DependencyObject) templateChild1, "Opacity", this.CheckedOpacity);
      }
      if (templateChild3 == null)
        return;
      templateChild3.Storyboard = new Storyboard();
      this.CreateDoubleAnimations(templateChild3.Storyboard, (DependencyObject) templateChild1, "Opacity", this.UncheckedOpacity);
    }

    public TimeSpan AnimationDuration
    {
      get
      {
        return (TimeSpan) ((DependencyObject) this).GetValue(OpacityToggleButton.AnimationDurationProperty);
      }
      set
      {
        ((DependencyObject) this).SetValue(OpacityToggleButton.AnimationDurationProperty, (object) value);
      }
    }

    public double UncheckedOpacity
    {
      get
      {
        return (double) ((DependencyObject) this).GetValue(OpacityToggleButton.UncheckedOpacityProperty);
      }
      set
      {
        ((DependencyObject) this).SetValue(OpacityToggleButton.UncheckedOpacityProperty, (object) value);
      }
    }

    public double CheckedOpacity
    {
      get
      {
        return (double) ((DependencyObject) this).GetValue(OpacityToggleButton.CheckedOpacityProperty);
      }
      set
      {
        ((DependencyObject) this).SetValue(OpacityToggleButton.CheckedOpacityProperty, (object) value);
      }
    }

    private void CreateDoubleAnimations(
      Storyboard sb,
      DependencyObject target,
      string propertyPath,
      double toValue)
    {
      DoubleAnimation doubleAnimation1 = new DoubleAnimation();
      doubleAnimation1.To = new double?(toValue);
      ((Timeline) doubleAnimation1).Duration = Duration.op_Implicit(this.AnimationDuration);
      DoubleAnimation doubleAnimation2 = doubleAnimation1;
      Storyboard.SetTarget((Timeline) doubleAnimation2, target);
      Storyboard.SetTargetProperty((Timeline) doubleAnimation2, new PropertyPath(propertyPath, new object[0]));
      ((PresentationFrameworkCollection<Timeline>) sb.Children).Add((Timeline) doubleAnimation2);
    }
  }
}
