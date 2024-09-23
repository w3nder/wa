// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.DragDeltaGestureEventArgs
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class DragDeltaGestureEventArgs : GestureEventArgs
  {
    internal DragDeltaGestureEventArgs(
      Point gestureOrigin,
      Point currentPosition,
      Point change,
      Orientation direction)
      : base(gestureOrigin, currentPosition)
    {
      this.HorizontalChange = change.X;
      this.VerticalChange = change.Y;
      this.Direction = direction;
    }

    public double HorizontalChange { get; private set; }

    public double VerticalChange { get; private set; }

    public Orientation Direction { get; private set; }
  }
}
