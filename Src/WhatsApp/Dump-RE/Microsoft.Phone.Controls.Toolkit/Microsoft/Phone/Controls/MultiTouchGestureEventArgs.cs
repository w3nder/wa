// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.MultiTouchGestureEventArgs
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class MultiTouchGestureEventArgs : GestureEventArgs
  {
    protected Point GestureOrigin2 { get; private set; }

    protected Point TouchPosition2 { get; private set; }

    internal MultiTouchGestureEventArgs(
      Point gestureOrigin,
      Point gestureOrigin2,
      Point position,
      Point position2)
      : base(gestureOrigin, position)
    {
      this.GestureOrigin2 = gestureOrigin2;
      this.TouchPosition2 = position2;
    }

    public Point GetPosition(UIElement relativeTo, int index)
    {
      if (index == 0)
        return this.GetPosition(relativeTo);
      if (index == 1)
        return GestureEventArgs.GetPosition(relativeTo, this.TouchPosition2);
      throw new ArgumentOutOfRangeException(nameof (index));
    }
  }
}
