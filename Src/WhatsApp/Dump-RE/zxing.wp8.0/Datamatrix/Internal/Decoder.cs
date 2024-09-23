// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Internal.Decoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;
using ZXing.Common.ReedSolomon;

#nullable disable
namespace ZXing.Datamatrix.Internal
{
  /// <summary>
  /// <p>The main class which implements Data Matrix Code decoding -- as opposed to locating and extracting
  /// the Data Matrix Code from an image.</p>
  /// 
  /// <author>bbrown@google.com (Brian Brown)</author>
  /// </summary>
  public sealed class Decoder
  {
    private readonly ReedSolomonDecoder rsDecoder;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Datamatrix.Internal.Decoder" /> class.
    /// </summary>
    public Decoder() => this.rsDecoder = new ReedSolomonDecoder(GenericGF.DATA_MATRIX_FIELD_256);

    /// <summary>
    /// <p>Convenience method that can decode a Data Matrix Code represented as a 2D array of booleans.
    /// "true" is taken to mean a black module.</p>
    /// 
    /// <param name="image">booleans representing white/black Data Matrix Code modules</param>
    /// <returns>text and bytes encoded within the Data Matrix Code</returns>
    /// <exception cref="T:ZXing.FormatException">if the Data Matrix Code cannot be decoded</exception>
    /// <exception cref="!:ChecksumException">if error correction fails</exception>
    /// </summary>
    public DecoderResult decode(bool[][] image)
    {
      int length = image.Length;
      BitMatrix bits = new BitMatrix(length);
      for (int y = 0; y < length; ++y)
      {
        for (int x = 0; x < length; ++x)
        {
          if (image[y][x])
            bits[x, y] = true;
        }
      }
      return this.decode(bits);
    }

    /// <summary>
    /// <p>Decodes a Data Matrix Code represented as a <see cref="T:ZXing.Common.BitMatrix" />. A 1 or "true" is taken
    /// to mean a black module.</p>
    /// </summary>
    /// <param name="bits">booleans representing white/black Data Matrix Code modules</param>
    /// <returns>text and bytes encoded within the Data Matrix Code</returns>
    public DecoderResult decode(BitMatrix bits)
    {
      BitMatrixParser bitMatrixParser = new BitMatrixParser(bits);
      if (bitMatrixParser.Version == null)
        return (DecoderResult) null;
      byte[] rawCodewords = bitMatrixParser.readCodewords();
      if (rawCodewords == null)
        return (DecoderResult) null;
      DataBlock[] dataBlocks = DataBlock.getDataBlocks(rawCodewords, bitMatrixParser.Version);
      int length1 = dataBlocks.Length;
      int length2 = 0;
      foreach (DataBlock dataBlock in dataBlocks)
        length2 += dataBlock.NumDataCodewords;
      byte[] bytes = new byte[length2];
      for (int index1 = 0; index1 < length1; ++index1)
      {
        DataBlock dataBlock = dataBlocks[index1];
        byte[] codewords = dataBlock.Codewords;
        int numDataCodewords = dataBlock.NumDataCodewords;
        if (!this.correctErrors(codewords, numDataCodewords))
          return (DecoderResult) null;
        for (int index2 = 0; index2 < numDataCodewords; ++index2)
          bytes[index2 * length1 + index1] = codewords[index2];
      }
      return DecodedBitStreamParser.decode(bytes);
    }

    /// <summary>
    /// <p>Given data and error-correction codewords received, possibly corrupted by errors, attempts to
    /// correct the errors in-place using Reed-Solomon error correction.</p>
    /// 
    /// <param name="codewordBytes">data and error correction codewords</param>
    /// <param name="numDataCodewords">number of codewords that are data bytes</param>
    /// </summary>
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
