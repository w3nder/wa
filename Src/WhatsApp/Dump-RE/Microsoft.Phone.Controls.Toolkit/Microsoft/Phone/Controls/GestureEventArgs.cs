// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.GestureEventArgs
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class GestureEventArgs : EventArgs
  {
    protected Point GestureOrigin { get; private set; }

    protected Point TouchPosition { get; private set; }

    internal GestureEventArgs(Point gestureOrigin, Point position)
    {
      this.GestureOrigin = gestureOrigin;
      this.TouchPosition = position;
    }

    public object OriginalSource { get; internal set; }

    public bool Handled { get; set; }

    public Point GetPosition(UIElement relativeTo)
    {
      return GestureEventArgs.GetPosition(relativeTo, this.TouchPosition);
    }

    protected static Point GetPosition(UIElement relativeTo, Point point)
    {
      if (relativeTo == null)
        relativeTo = Application.Current.RootVisual;
      return relativeTo != null ? relativeTo.TransformToVisual((UIElement) null).Inverse.Transform(point) : point;
    }
  }
}
