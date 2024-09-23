// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.TurnstileFeatherTransition
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class TurnstileFeatherTransition : TransitionElement
  {
    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof (Mode), typeof (TurnstileFeatherTransitionMode), typeof (TurnstileFeatherTransition), (PropertyMetadata) null);
    public static readonly DependencyProperty BeginTimeProperty = DependencyProperty.Register(nameof (BeginTime), typeof (TimeSpan?), typeof (TurnstileFeatherTransition), new PropertyMetadata((object) TimeSpan.Zero));

    public TurnstileFeatherTransitionMode Mode
    {
      get
      {
        return (TurnstileFeatherTransitionMode) this.GetValue(TurnstileFeatherTransition.ModeProperty);
      }
      set => this.SetValue(TurnstileFeatherTransition.ModeProperty, (object) value);
    }

    public TimeSpan? BeginTime
    {
      get => (TimeSpan?) this.GetValue(TurnstileFeatherTransition.BeginTimeProperty);
      set => this.SetValue(TurnstileFeatherTransition.BeginTimeProperty, (object) value);
    }

    public override ITransition GetTransition(UIElement element)
    {
      return Transitions.TurnstileFeather(element, this.Mode, this.BeginTime);
    }
  }
}
