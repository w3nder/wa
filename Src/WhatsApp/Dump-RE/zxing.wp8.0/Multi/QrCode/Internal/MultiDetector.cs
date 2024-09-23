// Decompiled with JetBrains decompiler
// Type: ZXing.Multi.QrCode.Internal.MultiDetector
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Common;
using ZXing.QrCode.Internal;

#nullable disable
namespace ZXing.Multi.QrCode.Internal
{
  /// <summary>
  /// <p>Encapsulates logic that can detect one or more QR Codes in an image, even if the QR Code
  /// is rotated or skewed, or partially obscured.</p>
  /// 
  /// <author>Sean Owen</author>
  /// <author>Hannes Erven</author>
  /// </summary>
  public sealed class MultiDetector : Detector
  {
    private static readonly DetectorResult[] EMPTY_DETECTOR_RESULTS = new DetectorResult[0];

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Multi.QrCode.Internal.MultiDetector" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    public MultiDetector(BitMatrix image)
      : base(image)
    {
    }

    /// <summary>Detects the multi.</summary>
    /// <param name="hints">The hints.</param>
    /// <returns></returns>
    public DetectorResult[] detectMulti(IDictionary<DecodeHintType, object> hints)
    {
      FinderPatternInfo[] multi = new MultiFinderPatternFinder(this.Image, hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? (ResultPointCallback) null : (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK]).findMulti(hints);
      if (multi.Length == 0)
        return MultiDetector.EMPTY_DETECTOR_RESULTS;
      List<DetectorResult> detectorResultList = new List<DetectorResult>();
      foreach (FinderPatternInfo info in multi)
      {
        DetectorResult detectorResult = this.processFinderPatternInfo(info);
        if (detectorResult != null)
          detectorResultList.Add(detectorResult);
      }
      return detectorResultList.Count == 0 ? MultiDetector.EMPTY_DETECTOR_RESULTS : detectorResultList.ToArray();
    }
  }
}
