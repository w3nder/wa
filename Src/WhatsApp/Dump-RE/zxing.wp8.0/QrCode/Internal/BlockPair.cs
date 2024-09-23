// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.BlockPair
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.QrCode.Internal
{
  internal sealed class BlockPair
  {
    private readonly byte[] dataBytes;
    private readonly byte[] errorCorrectionBytes;

    public BlockPair(byte[] data, byte[] errorCorrection)
    {
      this.dataBytes = data;
      this.errorCorrectionBytes = errorCorrection;
    }

    public byte[] DataBytes => this.dataBytes;

    public byte[] ErrorCorrectionBytes => this.errorCorrectionBytes;
  }
}
