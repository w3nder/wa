// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.DataBlock
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary> <p>Encapsulates a block of data within a QR Code. QR Codes may split their data into
  /// multiple blocks, each of which is a unit of data and error-correction codewords. Each
  /// is represented by an instance of this class.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class DataBlock
  {
    private readonly int numDataCodewords;
    private readonly byte[] codewords;

    private DataBlock(int numDataCodewords, byte[] codewords)
    {
      this.numDataCodewords = numDataCodewords;
      this.codewords = codewords;
    }

    /// <summary> <p>When QR Codes use multiple data blocks, they are actually interleaved.
    /// That is, the first byte of data block 1 to n is written, then the second bytes, and so on. This
    /// method will separate the data into original blocks.</p>
    /// 
    /// </summary>
    /// <param name="rawCodewords">bytes as read directly from the QR Code</param>
    /// <param name="version">version of the QR Code</param>
    /// <param name="ecLevel">error-correction level of the QR Code</param>
    /// <returns> {@link DataBlock}s containing original bytes, "de-interleaved" from representation in the
    /// QR Code
    /// </returns>
    internal static DataBlock[] getDataBlocks(
      byte[] rawCodewords,
      Version version,
      ErrorCorrectionLevel ecLevel)
    {
      if (rawCodewords.Length != version.TotalCodewords)
        throw new ArgumentException();
      Version.ECBlocks ecBlocksForLevel = version.getECBlocksForLevel(ecLevel);
      int length1 = 0;
      Version.ECB[] ecBlocks = ecBlocksForLevel.getECBlocks();
      foreach (Version.ECB ecb in ecBlocks)
        length1 += ecb.Count;
      DataBlock[] dataBlocks = new DataBlock[length1];
      int num1 = 0;
      foreach (Version.ECB ecb in ecBlocks)
      {
        for (int index = 0; index < ecb.Count; ++index)
        {
          int dataCodewords = ecb.DataCodewords;
          int length2 = ecBlocksForLevel.ECCodewordsPerBlock + dataCodewords;
          dataBlocks[num1++] = new DataBlock(dataCodewords, new byte[length2]);
        }
      }
      int length3 = dataBlocks[0].codewords.Length;
      int index1 = dataBlocks.Length - 1;
      while (index1 >= 0 && dataBlocks[index1].codewords.Length != length3)
        --index1;
      int num2 = index1 + 1;
      int index2 = length3 - ecBlocksForLevel.ECCodewordsPerBlock;
      int num3 = 0;
      for (int index3 = 0; index3 < index2; ++index3)
      {
        for (int index4 = 0; index4 < num1; ++index4)
          dataBlocks[index4].codewords[index3] = rawCodewords[num3++];
      }
      for (int index5 = num2; index5 < num1; ++index5)
        dataBlocks[index5].codewords[index2] = rawCodewords[num3++];
      int length4 = dataBlocks[0].codewords.Length;
      for (int index6 = index2; index6 < length4; ++index6)
      {
        for (int index7 = 0; index7 < num1; ++index7)
        {
          int index8 = index7 < num2 ? index6 : index6 + 1;
          dataBlocks[index7].codewords[index8] = rawCodewords[num3++];
        }
      }
      return dataBlocks;
    }

    internal int NumDataCodewords => this.numDataCodewords;

    internal byte[] Codewords => this.codewords;
  }
}
