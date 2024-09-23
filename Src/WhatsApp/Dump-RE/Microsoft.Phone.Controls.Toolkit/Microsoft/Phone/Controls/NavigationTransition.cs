// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.NavigationTransition
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class NavigationTransition : DependencyObject
  {
    public static readonly DependencyProperty BackwardProperty = DependencyProperty.Register(nameof (Backward), typeof (TransitionElement), typeof (NavigationTransition), (PropertyMetadata) null);
    public static readonly DependencyProperty ForwardProperty = DependencyProperty.Register(nameof (Forward), typeof (TransitionElement), typeof (NavigationTransition), (PropertyMetadata) null);

    public event RoutedEventHandler BeginTransition;

    public event RoutedEventHandler EndTransition;

    public TransitionElement Backward
    {
      get => (TransitionElement) this.GetValue(NavigationTransition.BackwardProperty);
      set => this.SetValue(NavigationTransition.BackwardProperty, (object) value);
    }

    public TransitionElement Forward
    {
      get => (TransitionElement) this.GetValue(NavigationTransition.ForwardProperty);
      set => this.SetValue(NavigationTransition.ForwardProperty, (object) value);
    }

    internal void OnBeginTransition()
    {
      if (this.BeginTransition == null)
        return;
      this.BeginTransition((object) this, new RoutedEventArgs());
    }

    internal void OnEndTransition()
    {
      if (this.EndTransition == null)
        return;
      this.EndTransition((object) this, new RoutedEventArgs());
    }
  }
}
