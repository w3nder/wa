// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Range`1
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class Range<T>
  {
    private readonly T fromValue;
    private readonly T toValue;

    public Range(T fromValue, T toValue)
    {
      this.fromValue = fromValue;
      this.toValue = toValue;
    }

    public T From => this.fromValue;

    public T To => this.toValue;
  }
}
