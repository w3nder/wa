// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.PinchStartedGestureEventArgs
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class PinchStartedGestureEventArgs : MultiTouchGestureEventArgs
  {
    internal PinchStartedGestureEventArgs(
      Point gestureOrigin,
      Point gestureOrigin2,
      Point pinch,
      Point pinch2)
      : base(gestureOrigin, gestureOrigin2, pinch, pinch2)
    {
    }

    public double Distance => MathHelpers.GetDistance(this.TouchPosition, this.TouchPosition2);

    public double Angle
    {
      get
      {
        return MathHelpers.GetAngle(this.TouchPosition2.X - this.TouchPosition.X, this.TouchPosition2.Y - this.TouchPosition.Y);
      }
    }
  }
}
