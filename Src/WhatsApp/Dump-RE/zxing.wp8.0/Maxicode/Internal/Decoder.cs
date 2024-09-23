// Decompiled with JetBrains decompiler
// Type: ZXing.Maxicode.Internal.Decoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Common.ReedSolomon;

#nullable disable
namespace ZXing.Maxicode.Internal
{
  /// <summary>
  /// <p>The main class which implements MaxiCode decoding -- as opposed to locating and extracting
  /// the MaxiCode from an image.</p>
  /// 
  /// <author>Manuel Kasten</author>
  /// </summary>
  public sealed class Decoder
  {
    private const int ALL = 0;
    private const int EVEN = 1;
    private const int ODD = 2;
    private readonly ReedSolomonDecoder rsDecoder;

    public Decoder() => this.rsDecoder = new ReedSolomonDecoder(GenericGF.MAXICODE_FIELD_64);

    public DecoderResult decode(BitMatrix bits)
    {
      return this.decode(bits, (IDictionary<DecodeHintType, object>) null);
    }

    public DecoderResult decode(BitMatrix bits, IDictionary<DecodeHintType, object> hints)
    {
      byte[] numArray1 = new BitMatrixParser(bits).readCodewords();
      if (!this.correctErrors(numArray1, 0, 10, 10, 0))
        return (DecoderResult) null;
      int mode = (int) numArray1[0] & 15;
      byte[] numArray2;
      switch (mode)
      {
        case 2:
        case 3:
        case 4:
          if (!this.correctErrors(numArray1, 20, 84, 40, 1))
            return (DecoderResult) null;
          if (!this.correctErrors(numArray1, 20, 84, 40, 2))
            return (DecoderResult) null;
          numArray2 = new byte[94];
          break;
        case 5:
          if (!this.correctErrors(numArray1, 20, 68, 56, 1))
            return (DecoderResult) null;
          if (!this.correctErrors(numArray1, 20, 68, 56, 2))
            return (DecoderResult) null;
          numArray2 = new byte[78];
          break;
        default:
          return (DecoderResult) null;
      }
      Array.Copy((Array) numArray1, 0, (Array) numArray2, 0, 10);
      Array.Copy((Array) numArray1, 20, (Array) numArray2, 10, numArray2.Length - 10);
      return DecodedBitStreamParser.decode(numArray2, mode);
    }

    private bool correctErrors(
      byte[] codewordBytes,
      int start,
      int dataCodewords,
      int ecCodewords,
      int mode)
    {
      int num1 = dataCodewords + ecCodewords;
      int num2 = mode == 0 ? 1 : 2;
      int[] received = new int[num1 / num2];
      for (int index = 0; index < num1; ++index)
      {
        if (mode == 0 || index % 2 == mode - 1)
          received[index / num2] = (int) codewordBytes[index + start] & (int) byte.MaxValue;
      }
      if (!this.rsDecoder.decode(received, ecCodewords / num2))
        return false;
      for (int index = 0; index < dataCodewords; ++index)
      {
        if (mode == 0 || index % 2 == mode - 1)
          codewordBytes[index + start] = (byte) received[index / num2];
      }
      return true;
    }
  }
}
