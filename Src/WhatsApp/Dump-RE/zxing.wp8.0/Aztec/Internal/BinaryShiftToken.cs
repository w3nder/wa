// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.Internal.BinaryShiftToken
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using ZXing.Common;

#nullable disable
namespace ZXing.Aztec.Internal
{
  public sealed class BinaryShiftToken : Token
  {
    private readonly short binaryShiftStart;
    private readonly short binaryShiftByteCount;

    public BinaryShiftToken(Token previous, int binaryShiftStart, int binaryShiftByteCount)
      : base(previous)
    {
      this.binaryShiftStart = (short) binaryShiftStart;
      this.binaryShiftByteCount = (short) binaryShiftByteCount;
    }

    public override void appendTo(BitArray bitArray, byte[] text)
    {
      for (int index = 0; index < (int) this.binaryShiftByteCount; ++index)
      {
        switch (index)
        {
          case 0:
            bitArray.appendBits(31, 5);
            if (this.binaryShiftByteCount > (short) 62)
            {
              bitArray.appendBits((int) this.binaryShiftByteCount - 31, 16);
              break;
            }
            if (index == 0)
            {
              bitArray.appendBits((int) Math.Min(this.binaryShiftByteCount, (short) 31), 5);
              break;
            }
            bitArray.appendBits((int) this.binaryShiftByteCount - 31, 5);
            break;
          case 31:
            if (this.binaryShiftByteCount > (short) 62)
              break;
            goto case 0;
        }
        bitArray.appendBits((int) text[(int) this.binaryShiftStart + index], 8);
      }
    }

    public override string ToString()
    {
      return "<" + (object) this.binaryShiftStart + "::" + (object) ((int) this.binaryShiftStart + (int) this.binaryShiftByteCount - 1) + (object) '>';
    }
  }
}
