// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.PDF417DetectorResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Common;

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// PDF 417 Detector Result class.  Skipped private backing stores.
  /// <author>Guenther Grau</author>
  /// </summary>
  public sealed class PDF417DetectorResult
  {
    public BitMatrix Bits { get; private set; }

    public List<ResultPoint[]> Points { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.PDF417.Internal.PDF417DetectorResult" /> class.
    /// </summary>
    /// <param name="bits">Bits.</param>
    /// <param name="points">Points.</param>
    public PDF417DetectorResult(BitMatrix bits, List<ResultPoint[]> points)
    {
      this.Bits = bits;
      this.Points = points;
    }
  }
}
