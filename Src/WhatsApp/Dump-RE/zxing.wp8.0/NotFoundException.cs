// Decompiled with JetBrains decompiler
// Type: ZXing.NotFoundException
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// Thrown when a barcode was not found in the image. It might have been
  /// partially detected but could not be confirmed.
  /// <author>Sean Owen</author>
  /// </summary>
  [Obsolete("Isn't used anymore, will be removed with next version")]
  public sealed class NotFoundException : ReaderException
  {
    private static readonly NotFoundException instance = new NotFoundException();

    private NotFoundException()
    {
    }

    public static NotFoundException Instance => NotFoundException.instance;
  }
}
