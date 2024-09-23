// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.TimedPoint
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal struct TimedPoint
  {
    private readonly Point p;
    private readonly DateTime stamp;

    public TimedPoint(Point p, DateTime stamp)
    {
      this.p = p;
      this.stamp = stamp;
    }

    public DateTime Timestamp => this.stamp;

    public Point Point => this.p;

    public double AgeInSeconds(DateTime now) => now.Subtract(this.Timestamp).TotalSeconds;
  }
}
