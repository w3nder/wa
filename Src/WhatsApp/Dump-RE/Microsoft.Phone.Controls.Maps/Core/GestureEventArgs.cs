// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.GestureEventArgs
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal abstract class GestureEventArgs : EventArgs
  {
    public Point Origin { get; internal set; }

    public abstract GestureType GestureType { get; }
  }
}
