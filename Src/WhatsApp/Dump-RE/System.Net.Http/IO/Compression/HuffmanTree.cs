// Decompiled with JetBrains decompiler
// Type: System.IO.Compression.HuffmanTree
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.IO.Compression
{
  internal class HuffmanTree
  {
    internal const int MaxLiteralTreeElements = 288;
    internal const int MaxDistTreeElements = 32;
    internal const int EndOfBlockCode = 256;
    internal const int NumberOfCodeLengthTreeElements = 19;
    private int tableBits;
    private short[] table;
    private short[] left;
    private short[] right;
    private byte[] codeLengthArray;
    private int tableMask;
    private static HuffmanTree staticLiteralLengthTree = new HuffmanTree(HuffmanTree.GetStaticLiteralTreeLength());
    private static HuffmanTree staticDistanceTree = new HuffmanTree(HuffmanTree.GetStaticDistanceTreeLength());

    public static HuffmanTree StaticLiteralLengthTree => HuffmanTree.staticLiteralLengthTree;

    public static HuffmanTree StaticDistanceTree => HuffmanTree.staticDistanceTree;

    public HuffmanTree(byte[] codeLengths)
    {
      this.codeLengthArray = codeLengths;
      this.tableBits = this.codeLengthArray.Length != 288 ? 7 : 9;
      this.tableMask = (1 << this.tableBits) - 1;
      this.CreateTable();
    }

    private static byte[] GetStaticLiteralTreeLength()
    {
      byte[] literalTreeLength = new byte[288];
      for (int index = 0; index <= 143; ++index)
        literalTreeLength[index] = (byte) 8;
      for (int index = 144; index <= (int) byte.MaxValue; ++index)
        literalTreeLength[index] = (byte) 9;
      for (int index = 256; index <= 279; ++index)
        literalTreeLength[index] = (byte) 7;
      for (int index = 280; index <= 287; ++index)
        literalTreeLength[index] = (byte) 8;
      return literalTreeLength;
    }

    private static byte[] GetStaticDistanceTreeLength()
    {
      byte[] distanceTreeLength = new byte[32];
      for (int index = 0; index < 32; ++index)
        distanceTreeLength[index] = (byte) 5;
      return distanceTreeLength;
    }

    private uint[] CalculateHuffmanCode()
    {
      uint[] numArray1 = new uint[17];
      foreach (int codeLength in this.codeLengthArray)
        ++numArray1[codeLength];
      numArray1[0] = 0U;
      uint[] numArray2 = new uint[17];
      uint num = 0;
      for (int index = 1; index <= 16; ++index)
      {
        num = (uint) ((int) num + (int) numArray1[index - 1] << 1);
        numArray2[index] = num;
      }
      uint[] huffmanCode = new uint[288];
      for (int index = 0; index < this.codeLengthArray.Length; ++index)
      {
        int codeLength = (int) this.codeLengthArray[index];
        if (codeLength > 0)
        {
          huffmanCode[index] = FastEncoderStatics.BitReverse(numArray2[codeLength], codeLength);
          ++numArray2[codeLength];
        }
      }
      return huffmanCode;
    }

    private void CreateTable()
    {
      uint[] huffmanCode = this.CalculateHuffmanCode();
      this.table = new short[1 << this.tableBits];
      this.left = new short[2 * this.codeLengthArray.Length];
      this.right = new short[2 * this.codeLengthArray.Length];
      short length = (short) this.codeLengthArray.Length;
      for (int index1 = 0; index1 < this.codeLengthArray.Length; ++index1)
      {
        int codeLength = (int) this.codeLengthArray[index1];
        if (codeLength > 0)
        {
          int index2 = (int) huffmanCode[index1];
          if (codeLength <= this.tableBits)
          {
            int num1 = 1 << codeLength;
            if (index2 >= num1)
              throw new InvalidDataException(Resources.InvalidHuffmanData);
            int num2 = 1 << this.tableBits - codeLength;
            for (int index3 = 0; index3 < num2; ++index3)
            {
              this.table[index2] = (short) index1;
              index2 += num1;
            }
          }
          else
          {
            int num3 = codeLength - this.tableBits;
            int num4 = 1 << this.tableBits;
            int index4 = index2 & (1 << this.tableBits) - 1;
            short[] numArray = this.table;
            do
            {
              short num5 = numArray[index4];
              if (num5 == (short) 0)
              {
                numArray[index4] = -length;
                num5 = -length;
                ++length;
              }
              if (num5 > (short) 0)
                throw new InvalidDataException(Resources.InvalidHuffmanData);
              numArray = (index2 & num4) != 0 ? this.right : this.left;
              index4 = (int) -num5;
              num4 <<= 1;
              --num3;
            }
            while (num3 != 0);
            numArray[index4] = (short) index1;
          }
        }
      }
    }

    public int GetNextSymbol(InputBuffer input)
    {
      uint num1 = input.TryLoad16Bits();
      if (input.AvailableBits == 0)
        return -1;
      int nextSymbol = (int) this.table[(long) num1 & (long) this.tableMask];
      if (nextSymbol < 0)
      {
        uint num2 = (uint) (1 << this.tableBits);
        do
        {
          int index = -nextSymbol;
          nextSymbol = ((int) num1 & (int) num2) != 0 ? (int) this.right[index] : (int) this.left[index];
          num2 <<= 1;
        }
        while (nextSymbol < 0);
      }
      int codeLength = (int) this.codeLengthArray[nextSymbol];
      if (codeLength <= 0)
        throw new InvalidDataException(Resources.InvalidHuffmanData);
      if (codeLength > input.AvailableBits)
        return -1;
      input.SkipBits(codeLength);
      return nextSymbol;
    }
  }
}
