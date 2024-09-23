// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.TileSourcePropertyChangedEventArgs
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class TileSourcePropertyChangedEventArgs : EventArgs
  {
    private readonly string propertyName;
    private readonly TileSource tileSource;

    public TileSourcePropertyChangedEventArgs(TileSource source, string property)
    {
      this.tileSource = source;
      this.propertyName = property;
    }

    public TileSource TileSource => this.tileSource;

    public string PropertyName => this.propertyName;
  }
}
