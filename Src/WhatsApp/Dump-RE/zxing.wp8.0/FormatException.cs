// Decompiled with JetBrains decompiler
// Type: ZXing.FormatException
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing
{
  /// <summary>
  /// Thrown when a barcode was successfully detected, but some aspect of
  /// the content did not conform to the barcode's format rules. This could have
  /// been due to a mis-detection.
  /// <author>Sean Owen</author>
  /// </summary>
  public sealed class FormatException : ReaderException
  {
    private static readonly FormatException instance = new FormatException();

    private FormatException()
    {
    }

    public static FormatException Instance => FormatException.instance;
  }
}
