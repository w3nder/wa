// Decompiled with JetBrains decompiler
// Type: ZXing.Common.ECI
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.Common
{
  /// <summary> Superclass of classes encapsulating types ECIs, according to "Extended Channel Interpretations"
  /// 5.3 of ISO 18004.
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public abstract class ECI
  {
    private int value_Renamed;

    public virtual int Value => this.value_Renamed;

    internal ECI(int value_Renamed) => this.value_Renamed = value_Renamed;

    /// <param name="value">ECI value</param>
    /// <returns> {@link ECI} representing ECI of given value, or null if it is legal but unsupported
    /// </returns>
    /// <throws>  IllegalArgumentException if ECI value is invalid </throws>
    public static ECI getECIByValue(int value_Renamed)
    {
      if (value_Renamed < 0 || value_Renamed > 999999)
        throw new ArgumentException("Bad ECI value: " + (object) value_Renamed);
      return value_Renamed < 900 ? (ECI) CharacterSetECI.getCharacterSetECIByValue(value_Renamed) : (ECI) null;
    }
  }
}
