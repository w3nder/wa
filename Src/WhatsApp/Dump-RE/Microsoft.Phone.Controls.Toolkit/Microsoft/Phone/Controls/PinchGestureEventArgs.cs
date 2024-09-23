// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.PinchGestureEventArgs
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class PinchGestureEventArgs : MultiTouchGestureEventArgs
  {
    internal PinchGestureEventArgs(
      Point gestureOrigin,
      Point gestureOrigin2,
      Point position,
      Point position2)
      : base(gestureOrigin, gestureOrigin2, position, position2)
    {
    }

    public double DistanceRatio
    {
      get
      {
        double num = Math.Max(MathHelpers.GetDistance(this.GestureOrigin, this.GestureOrigin2), 1.0);
        return Math.Max(MathHelpers.GetDistance(this.TouchPosition, this.TouchPosition2), 1.0) / num;
      }
    }

    public double TotalAngleDelta
    {
      get
      {
        double angle = MathHelpers.GetAngle(this.GestureOrigin2.X - this.GestureOrigin.X, this.GestureOrigin2.Y - this.GestureOrigin.Y);
        return MathHelpers.GetAngle(this.TouchPosition2.X - this.TouchPosition.X, this.TouchPosition2.Y - this.TouchPosition.Y) - angle;
      }
    }
  }
}
