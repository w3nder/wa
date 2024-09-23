// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.FeatheredTransition
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class FeatheredTransition : Transition
  {
    private TurnstileFeatherTransitionMode _mode;
    private TimeSpan? _beginTime;

    public FeatheredTransition(
      UIElement element,
      Storyboard storyboard,
      TurnstileFeatherTransitionMode mode,
      TimeSpan? beginTime)
      : base(element, storyboard)
    {
      this._mode = mode;
      this._beginTime = beginTime;
    }

    public override void Begin()
    {
      TurnstileFeatherEffect.ComposeStoryboard(this.Storyboard, this._beginTime, this._mode);
      base.Begin();
    }
  }
}
