// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.PDF417ResultMetadata
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.PDF417
{
  /// <summary>
  /// PDF 417 result meta data.
  /// <author>Guenther Grau</author>
  /// </summary>
  public sealed class PDF417ResultMetadata
  {
    public int SegmentIndex { get; set; }

    public string FileId { get; set; }

    public int[] OptionalData { get; set; }

    public bool IsLastSegment { get; set; }
  }
}
