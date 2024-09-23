// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.PinchStretchData
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This struct has been deprecated.  Use the Microsoft.Phone.Maps.Controls.Map class in Microsoft.Phone.Maps.dll instead.")]
  internal struct PinchStretchData
  {
    internal double Scale;

    internal Point ContactPoint1 { get; set; }

    internal Point ContactPoint1Delta { get; set; }

    internal Point ContactPoint2 { get; set; }

    internal Point ContactPoint2Delta { get; set; }

    internal Point CenterPoint { get; set; }

    internal Point CenterPointDelta { get; set; }
  }
}
