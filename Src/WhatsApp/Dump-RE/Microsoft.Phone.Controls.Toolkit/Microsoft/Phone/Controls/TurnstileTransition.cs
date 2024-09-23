// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.TurnstileTransition
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class TurnstileTransition : TransitionElement
  {
    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof (Mode), typeof (TurnstileTransitionMode), typeof (TurnstileTransition), (PropertyMetadata) null);

    public TurnstileTransitionMode Mode
    {
      get => (TurnstileTransitionMode) this.GetValue(TurnstileTransition.ModeProperty);
      set => this.SetValue(TurnstileTransition.ModeProperty, (object) value);
    }

    public override ITransition GetTransition(UIElement element)
    {
      return Transitions.Turnstile(element, this.Mode);
    }
  }
}
