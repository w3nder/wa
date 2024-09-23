// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Internal.DataBlock
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.Datamatrix.Internal
{
  /// <summary>
  /// <p>Encapsulates a block of data within a Data Matrix Code. Data Matrix Codes may split their data into
  /// multiple blocks, each of which is a unit of data and error-correction codewords. Each
  /// is represented by an instance of this class.</p>
  /// 
  /// <author>bbrown@google.com (Brian Brown)</author>
  /// </summary>
  internal sealed class DataBlock
  {
    private readonly int numDataCodewords;
    private readonly byte[] codewords;

    private DataBlock(int numDataCodewords, byte[] codewords)
    {
      this.numDataCodewords = numDataCodewords;
      this.codewords = codewords;
    }

    /// <summary>
    /// <p>When Data Matrix Codes use multiple data blocks, they actually interleave the bytes of each of them.
    /// That is, the first byte of data block 1 to n is written, then the second bytes, and so on. This
    /// method will separate the data into original blocks.</p>
    /// 
    /// <param name="rawCodewords">bytes as read directly from the Data Matrix Code</param>
    /// <param name="version">version of the Data Matrix Code</param>
    /// <returns>DataBlocks containing original bytes, "de-interleaved" from representation in the</returns>
    ///         Data Matrix Code
    /// </summary>
    internal static DataBlock[] getDataBlocks(byte[] rawCodewords, Version version)
    {
      Version.ECBlocks ecBlocks = version.getECBlocks();
      int length1 = 0;
      Version.ECB[] ecBlocksValue = ecBlocks.ECBlocksValue;
      foreach (Version.ECB ecb in ecBlocksValue)
        length1 += ecb.Count;
      DataBlock[] dataBlocks = new DataBlock[length1];
      int num1 = 0;
      foreach (Version.ECB ecb in ecBlocksValue)
      {
        for (int index = 0; index < ecb.Count; ++index)
        {
          int dataCodewords = ecb.DataCodewords;
          int length2 = ecBlocks.ECCodewords + dataCodewords;
          dataBlocks[num1++] = new DataBlock(dataCodewords, new byte[length2]);
        }
      }
      int num2 = dataBlocks[0].codewords.Length - ecBlocks.ECCodewords;
      int num3 = num2 - 1;
      int num4 = 0;
      for (int index1 = 0; index1 < num3; ++index1)
      {
        for (int index2 = 0; index2 < num1; ++index2)
          dataBlocks[index2].codewords[index1] = rawCodewords[num4++];
      }
      bool flag = version.getVersionNumber() == 24;
      int num5 = flag ? 8 : num1;
      for (int index = 0; index < num5; ++index)
        dataBlocks[index].codewords[num2 - 1] = rawCodewords[num4++];
      int length3 = dataBlocks[0].codewords.Length;
      for (int index3 = num2; index3 < length3; ++index3)
      {
        for (int index4 = 0; index4 < num1; ++index4)
        {
          int index5 = !flag || index4 <= 7 ? index3 : index3 - 1;
          dataBlocks[index4].codewords[index5] = rawCodewords[num4++];
        }
      }
      if (num4 != rawCodewords.Length)
        throw new ArgumentException();
      return dataBlocks;
    }

    internal int NumDataCodewords => this.numDataCodewords;

    internal byte[] Codewords => this.codewords;
  }
}
