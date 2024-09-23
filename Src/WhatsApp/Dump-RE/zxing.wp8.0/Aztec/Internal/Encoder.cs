// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.Internal.Encoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using ZXing.Common;
using ZXing.Common.ReedSolomon;

#nullable disable
namespace ZXing.Aztec.Internal
{
  /// <summary>Generates Aztec 2D barcodes.</summary>
  /// <author>Rustam Abdullaev</author>
  public static class Encoder
  {
    public const int DEFAULT_EC_PERCENT = 33;
    public const int DEFAULT_AZTEC_LAYERS = 0;
    private const int MAX_NB_BITS = 32;
    private const int MAX_NB_BITS_COMPACT = 4;
    private static readonly int[] WORD_SIZE = new int[33]
    {
      4,
      6,
      6,
      8,
      8,
      8,
      8,
      8,
      8,
      10,
      10,
      10,
      10,
      10,
      10,
      10,
      10,
      10,
      10,
      10,
      10,
      10,
      10,
      12,
      12,
      12,
      12,
      12,
      12,
      12,
      12,
      12,
      12
    };

    /// <summary>Encodes the given binary content as an Aztec symbol</summary>
    /// <param name="data">input data string</param>
    /// <returns>Aztec symbol matrix with metadata</returns>
    public static AztecCode encode(byte[] data) => Encoder.encode(data, 33, 0);

    /// <summary>Encodes the given binary content as an Aztec symbol</summary>
    /// <param name="data">input data string</param>
    /// <param name="minECCPercent">minimal percentage of error check words (According to ISO/IEC 24778:2008,
    /// a minimum of 23% + 3 words is recommended)</param>
    /// <param name="userSpecifiedLayers">if non-zero, a user-specified value for the number of layers</param>
    /// <returns>Aztec symbol matrix with metadata</returns>
    public static AztecCode encode(byte[] data, int minECCPercent, int userSpecifiedLayers)
    {
      BitArray bits = new HighLevelEncoder(data).encode();
      int num1 = bits.Size * minECCPercent / 100 + 11;
      int num2 = bits.Size + num1;
      bool compact;
      int layers;
      int totalBits;
      int wordSize;
      BitArray bitArray;
      if (userSpecifiedLayers != 0)
      {
        compact = userSpecifiedLayers < 0;
        layers = Math.Abs(userSpecifiedLayers);
        totalBits = layers <= (compact ? 4 : 32) ? Encoder.TotalBitsInLayer(layers, compact) : throw new ArgumentException(string.Format("Illegal value {0} for layers", (object) userSpecifiedLayers));
        wordSize = Encoder.WORD_SIZE[layers];
        int num3 = totalBits - totalBits % wordSize;
        bitArray = Encoder.stuffBits(bits, wordSize);
        if (bitArray.Size + num1 > num3)
          throw new ArgumentException("Data to large for user specified layer");
        if (compact && bitArray.Size > wordSize * 64)
          throw new ArgumentException("Data to large for user specified layer");
      }
      else
      {
        wordSize = 0;
        bitArray = (BitArray) null;
        for (int index = 0; index <= 32; ++index)
        {
          compact = index <= 3;
          layers = compact ? index + 1 : index;
          totalBits = Encoder.TotalBitsInLayer(layers, compact);
          if (num2 <= totalBits)
          {
            if (wordSize != Encoder.WORD_SIZE[layers])
            {
              wordSize = Encoder.WORD_SIZE[layers];
              bitArray = Encoder.stuffBits(bits, wordSize);
            }
            if (bitArray != null)
            {
              int num4 = totalBits - totalBits % wordSize;
              if ((!compact || bitArray.Size <= wordSize * 64) && bitArray.Size + num1 <= num4)
                goto label_16;
            }
          }
        }
        throw new ArgumentException("Data too large for an Aztec code");
      }
label_16:
      BitArray checkWords = Encoder.generateCheckWords(bitArray, totalBits, wordSize);
      int messageSizeInWords = bitArray.Size / wordSize;
      BitArray modeMessage = Encoder.generateModeMessage(compact, layers, messageSizeInWords);
      int length = compact ? 11 + layers * 4 : 14 + layers * 4;
      int[] numArray = new int[length];
      int num5;
      if (compact)
      {
        num5 = length;
        for (int index = 0; index < numArray.Length; ++index)
          numArray[index] = index;
      }
      else
      {
        num5 = length + 1 + 2 * ((length / 2 - 1) / 15);
        int num6 = length / 2;
        int num7 = num5 / 2;
        for (int index = 0; index < num6; ++index)
        {
          int num8 = index + index / 15;
          numArray[num6 - index - 1] = num7 - num8 - 1;
          numArray[num6 + index] = num7 + num8 + 1;
        }
      }
      BitMatrix matrix = new BitMatrix(num5);
      int num9 = 0;
      int num10 = 0;
      for (; num9 < layers; ++num9)
      {
        int num11 = compact ? (layers - num9) * 4 + 9 : (layers - num9) * 4 + 12;
        for (int index1 = 0; index1 < num11; ++index1)
        {
          int num12 = index1 * 2;
          for (int index2 = 0; index2 < 2; ++index2)
          {
            if (checkWords[num10 + num12 + index2])
              matrix[numArray[num9 * 2 + index2], numArray[num9 * 2 + index1]] = true;
            if (checkWords[num10 + num11 * 2 + num12 + index2])
              matrix[numArray[num9 * 2 + index1], numArray[length - 1 - num9 * 2 - index2]] = true;
            if (checkWords[num10 + num11 * 4 + num12 + index2])
              matrix[numArray[length - 1 - num9 * 2 - index2], numArray[length - 1 - num9 * 2 - index1]] = true;
            if (checkWords[num10 + num11 * 6 + num12 + index2])
              matrix[numArray[length - 1 - num9 * 2 - index1], numArray[num9 * 2 + index2]] = true;
          }
        }
        num10 += num11 * 8;
      }
      Encoder.drawModeMessage(matrix, compact, num5, modeMessage);
      if (compact)
      {
        Encoder.drawBullsEye(matrix, num5 / 2, 5);
      }
      else
      {
        Encoder.drawBullsEye(matrix, num5 / 2, 7);
        int num13 = 0;
        int num14 = 0;
        while (num13 < length / 2 - 1)
        {
          for (int index = num5 / 2 & 1; index < num5; index += 2)
          {
            matrix[num5 / 2 - num14, index] = true;
            matrix[num5 / 2 + num14, index] = true;
            matrix[index, num5 / 2 - num14] = true;
            matrix[index, num5 / 2 + num14] = true;
          }
          num13 += 15;
          num14 += 16;
        }
      }
      return new AztecCode()
      {
        isCompact = compact,
        Size = num5,
        Layers = layers,
        CodeWords = messageSizeInWords,
        Matrix = matrix
      };
    }

    private static void drawBullsEye(BitMatrix matrix, int center, int size)
    {
      for (int index1 = 0; index1 < size; index1 += 2)
      {
        for (int index2 = center - index1; index2 <= center + index1; ++index2)
        {
          matrix[index2, center - index1] = true;
          matrix[index2, center + index1] = true;
          matrix[center - index1, index2] = true;
          matrix[center + index1, index2] = true;
        }
      }
      matrix[center - size, center - size] = true;
      matrix[center - size + 1, center - size] = true;
      matrix[center - size, center - size + 1] = true;
      matrix[center + size, center - size] = true;
      matrix[center + size, center - size + 1] = true;
      matrix[center + size, center + size - 1] = true;
    }

    internal static BitArray generateModeMessage(bool compact, int layers, int messageSizeInWords)
    {
      BitArray bitArray = new BitArray();
      BitArray checkWords;
      if (compact)
      {
        bitArray.appendBits(layers - 1, 2);
        bitArray.appendBits(messageSizeInWords - 1, 6);
        checkWords = Encoder.generateCheckWords(bitArray, 28, 4);
      }
      else
      {
        bitArray.appendBits(layers - 1, 5);
        bitArray.appendBits(messageSizeInWords - 1, 11);
        checkWords = Encoder.generateCheckWords(bitArray, 40, 4);
      }
      return checkWords;
    }

    private static void drawModeMessage(
      BitMatrix matrix,
      bool compact,
      int matrixSize,
      BitArray modeMessage)
    {
      int num1 = matrixSize / 2;
      if (compact)
      {
        for (int i = 0; i < 7; ++i)
        {
          int num2 = num1 - 3 + i;
          if (modeMessage[i])
            matrix[num2, num1 - 5] = true;
          if (modeMessage[i + 7])
            matrix[num1 + 5, num2] = true;
          if (modeMessage[20 - i])
            matrix[num2, num1 + 5] = true;
          if (modeMessage[27 - i])
            matrix[num1 - 5, num2] = true;
        }
      }
      else
      {
        for (int i = 0; i < 10; ++i)
        {
          int num3 = num1 - 5 + i + i / 5;
          if (modeMessage[i])
            matrix[num3, num1 - 7] = true;
          if (modeMessage[i + 10])
            matrix[num1 + 7, num3] = true;
          if (modeMessage[29 - i])
            matrix[num3, num1 + 7] = true;
          if (modeMessage[39 - i])
            matrix[num1 - 7, num3] = true;
        }
      }
    }

    private static BitArray generateCheckWords(BitArray bitArray, int totalBits, int wordSize)
    {
      if (bitArray.Size % wordSize != 0)
        throw new InvalidOperationException("size of bit array is not a multiple of the word size");
      int num1 = bitArray.Size / wordSize;
      ReedSolomonEncoder reedSolomonEncoder = new ReedSolomonEncoder(Encoder.getGF(wordSize));
      int totalWords = totalBits / wordSize;
      int[] words = Encoder.bitsToWords(bitArray, wordSize, totalWords);
      reedSolomonEncoder.encode(words, totalWords - num1);
      int numBits = totalBits % wordSize;
      BitArray checkWords = new BitArray();
      checkWords.appendBits(0, numBits);
      foreach (int num2 in words)
        checkWords.appendBits(num2, wordSize);
      return checkWords;
    }

    private static int[] bitsToWords(BitArray stuffedBits, int wordSize, int totalWords)
    {
      int[] words = new int[totalWords];
      int index1 = 0;
      for (int index2 = stuffedBits.Size / wordSize; index1 < index2; ++index1)
      {
        int num = 0;
        for (int index3 = 0; index3 < wordSize; ++index3)
          num |= stuffedBits[index1 * wordSize + index3] ? 1 << wordSize - index3 - 1 : 0;
        words[index1] = num;
      }
      return words;
    }

    private static GenericGF getGF(int wordSize)
    {
      switch (wordSize)
      {
        case 4:
          return GenericGF.AZTEC_PARAM;
        case 6:
          return GenericGF.AZTEC_DATA_6;
        case 8:
          return GenericGF.AZTEC_DATA_8;
        case 10:
          return GenericGF.AZTEC_DATA_10;
        case 12:
          return GenericGF.AZTEC_DATA_12;
        default:
          return (GenericGF) null;
      }
    }

    internal static BitArray stuffBits(BitArray bits, int wordSize)
    {
      BitArray bitArray = new BitArray();
      int size = bits.Size;
      int num1 = (1 << wordSize) - 2;
      for (int index1 = 0; index1 < size; index1 += wordSize)
      {
        int num2 = 0;
        for (int index2 = 0; index2 < wordSize; ++index2)
        {
          if (index1 + index2 >= size || bits[index1 + index2])
            num2 |= 1 << wordSize - 1 - index2;
        }
        if ((num2 & num1) == num1)
        {
          bitArray.appendBits(num2 & num1, wordSize);
          --index1;
        }
        else if ((num2 & num1) == 0)
        {
          bitArray.appendBits(num2 | 1, wordSize);
          --index1;
        }
        else
          bitArray.appendBits(num2, wordSize);
      }
      return bitArray;
    }

    private static int TotalBitsInLayer(int layers, bool compact)
    {
      return ((compact ? 88 : 112) + 16 * layers) * layers;
    }
  }
}
