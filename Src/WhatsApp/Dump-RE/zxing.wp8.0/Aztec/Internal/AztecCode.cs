// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.Internal.AztecCode
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;

#nullable disable
namespace ZXing.Aztec.Internal
{
  /// <summary>Aztec 2D code representation</summary>
  /// <author>Rustam Abdullaev</author>
  public sealed class AztecCode
  {
    /// <summary>Compact or full symbol indicator</summary>
    public bool isCompact { get; set; }

    /// <summary>Size in pixels (width and height)</summary>
    public int Size { get; set; }

    /// <summary>Number of levels</summary>
    public int Layers { get; set; }

    /// <summary>Number of data codewords</summary>
    public int CodeWords { get; set; }

    /// <summary>The symbol image</summary>
    public BitMatrix Matrix { get; set; }
  }
}
