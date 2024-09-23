// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.Internal.SimpleToken
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;

#nullable disable
namespace ZXing.Aztec.Internal
{
  public sealed class SimpleToken : Token
  {
    private readonly short value;
    private readonly short bitCount;

    public SimpleToken(Token previous, int value, int bitCount)
      : base(previous)
    {
      this.value = (short) value;
      this.bitCount = (short) bitCount;
    }

    public override void appendTo(BitArray bitArray, byte[] text)
    {
      bitArray.appendBits((int) this.value, (int) this.bitCount);
    }

    public override string ToString()
    {
      return '<'.ToString() + SupportClass.ToBinaryString((int) this.value & (1 << (int) this.bitCount) - 1 | 1 << (int) this.bitCount | 1 << (int) this.bitCount).Substring(1) + (object) '>';
    }
  }
}
