// Decompiled with JetBrains decompiler
// Type: ZXing.ResultPointCallback
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing
{
  /// <summary> Callback which is invoked when a possible result point (significant
  /// point in the barcode image such as a corner) is found.
  /// 
  /// </summary>
  /// <seealso cref="F:ZXing.DecodeHintType.NEED_RESULT_POINT_CALLBACK">
  /// </seealso>
  public delegate void ResultPointCallback(ResultPoint point);
}
