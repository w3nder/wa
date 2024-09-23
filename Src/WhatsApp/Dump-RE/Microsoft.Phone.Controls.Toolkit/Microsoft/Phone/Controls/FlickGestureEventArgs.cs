// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.FlickGestureEventArgs
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class FlickGestureEventArgs : GestureEventArgs
  {
    private Point _velocity;

    internal FlickGestureEventArgs(Point hostOrigin, Point velocity)
      : base(hostOrigin, hostOrigin)
    {
      this._velocity = velocity;
    }

    public double HorizontalVelocity => this._velocity.X;

    public double VerticalVelocity => this._velocity.Y;

    public double Angle => MathHelpers.GetAngle(this._velocity.X, this._velocity.Y);

    public Orientation Direction
    {
      get
      {
        return Math.Abs(this._velocity.X) < Math.Abs(this._velocity.Y) ? Orientation.Vertical : Orientation.Horizontal;
      }
    }
  }
}
