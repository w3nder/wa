// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.Decoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Common;
using ZXing.Common.ReedSolomon;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  ///   <p>The main class which implements QR Code decoding -- as opposed to locating and extracting
  /// the QR Code from an image.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public sealed class Decoder
  {
    private readonly ReedSolomonDecoder rsDecoder;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.QrCode.Internal.Decoder" /> class.
    /// </summary>
    public Decoder() => this.rsDecoder = new ReedSolomonDecoder(GenericGF.QR_CODE_FIELD_256);

    /// <summary>
    ///   <p>Convenience method that can decode a QR Code represented as a 2D array of booleans.
    /// "true" is taken to mean a black module.</p>
    /// </summary>
    /// <param name="image">booleans representing white/black QR Code modules</param>
    /// <param name="hints">The hints.</param>
    /// <returns>text and bytes encoded within the QR Code</returns>
    public DecoderResult decode(bool[][] image, IDictionary<DecodeHintType, object> hints)
    {
      int length = image.Length;
      BitMatrix bits = new BitMatrix(length);
      for (int y = 0; y < length; ++y)
      {
        for (int x = 0; x < length; ++x)
          bits[x, y] = image[y][x];
      }
      return this.decode(bits, hints);
    }

    /// <summary>
    ///   <p>Decodes a QR Code represented as a {@link BitMatrix}. A 1 or "true" is taken to mean a black module.</p>
    /// </summary>
    /// <param name="bits">booleans representing white/black QR Code modules</param>
    /// <param name="hints">The hints.</param>
    /// <returns>text and bytes encoded within the QR Code</returns>
    public DecoderResult decode(BitMatrix bits, IDictionary<DecodeHintType, object> hints)
    {
      BitMatrixParser bitMatrixParser = BitMatrixParser.createBitMatrixParser(bits);
      if (bitMatrixParser == null)
        return (DecoderResult) null;
      DecoderResult decoderResult = this.decode(bitMatrixParser, hints);
      if (decoderResult == null)
      {
        bitMatrixParser.remask();
        bitMatrixParser.setMirror(true);
        if (bitMatrixParser.readVersion() == null)
          return (DecoderResult) null;
        if (bitMatrixParser.readFormatInformation() == null)
          return (DecoderResult) null;
        bitMatrixParser.mirror();
        decoderResult = this.decode(bitMatrixParser, hints);
        if (decoderResult != null)
          decoderResult.Other = (object) new QRCodeDecoderMetaData(true);
      }
      return decoderResult;
    }

    private DecoderResult decode(BitMatrixParser parser, IDictionary<DecodeHintType, object> hints)
    {
      Version version = parser.readVersion();
      if (version == null)
        return (DecoderResult) null;
      FormatInformation formatInformation = parser.readFormatInformation();
      if (formatInformation == null)
        return (DecoderResult) null;
      ErrorCorrectionLevel errorCorrectionLevel = formatInformation.ErrorCorrectionLevel;
      byte[] rawCodewords = parser.readCodewords();
      if (rawCodewords == null)
        return (DecoderResult) null;
      DataBlock[] dataBlocks = DataBlock.getDataBlocks(rawCodewords, version, errorCorrectionLevel);
      int length = 0;
      foreach (DataBlock dataBlock in dataBlocks)
        length += dataBlock.NumDataCodewords;
      byte[] bytes = new byte[length];
      int num = 0;
      foreach (DataBlock dataBlock in dataBlocks)
      {
        byte[] codewords = dataBlock.Codewords;
        int numDataCodewords = dataBlock.NumDataCodewords;
        if (!this.correctErrors(codewords, numDataCodewords))
          return (DecoderResult) null;
        for (int index = 0; index < numDataCodewords; ++index)
          bytes[num++] = codewords[index];
      }
      return DecodedBitStreamParser.decode(bytes, version, errorCorrectionLevel, hints);
    }

    /// <summary>
    ///   <p>Given data and error-correction codewords received, possibly corrupted by errors, attempts to
    /// correct the errors in-place using Reed-Solomon error correction.</p>
    /// </summary>
    /// <param name="codewordBytes">data and error correction codewords</param>
    /// <param name="numDataCodewords">number of codewords that are data bytes</param>
    /// <returns></returns>
    private bool correctErrors(byte[] codewordBytes, int numDataCodewords)
    {
      int length = codewordBytes.Length;
      int[] received = new int[length];
      for (int index = 0; index < length; ++index)
        received[index] = (int) codewordBytes[index] & (int) byte.MaxValue;
      int twoS = codewordBytes.Length - numDataCodewords;
      if (!this.rsDecoder.decode(received, twoS))
        return false;
      for (int index = 0; index < numDataCodewords; ++index)
        codewordBytes[index] = (byte) received[index];
      return true;
    }
  }
}
