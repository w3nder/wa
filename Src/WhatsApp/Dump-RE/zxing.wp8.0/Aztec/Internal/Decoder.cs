// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.Internal.Decoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using System.Text;
using ZXing.Common;
using ZXing.Common.ReedSolomon;

#nullable disable
namespace ZXing.Aztec.Internal
{
  /// <summary>
  /// The main class which implements Aztec Code decoding -- as opposed to locating and extracting
  /// the Aztec Code from an image.
  /// </summary>
  /// <author>David Olivier</author>
  public sealed class Decoder
  {
    private static readonly string[] UPPER_TABLE = new string[32]
    {
      "CTRL_PS",
      " ",
      "A",
      "B",
      "C",
      "D",
      "E",
      "F",
      "G",
      "H",
      "I",
      "J",
      "K",
      "L",
      "M",
      "N",
      "O",
      "P",
      "Q",
      "R",
      "S",
      "T",
      "U",
      "V",
      "W",
      "X",
      "Y",
      "Z",
      "CTRL_LL",
      "CTRL_ML",
      "CTRL_DL",
      "CTRL_BS"
    };
    private static readonly string[] LOWER_TABLE = new string[32]
    {
      "CTRL_PS",
      " ",
      "a",
      "b",
      "c",
      "d",
      "e",
      "f",
      "g",
      "h",
      "i",
      "j",
      "k",
      "l",
      "m",
      "n",
      "o",
      "p",
      "q",
      "r",
      "s",
      "t",
      "u",
      "v",
      "w",
      "x",
      "y",
      "z",
      "CTRL_US",
      "CTRL_ML",
      "CTRL_DL",
      "CTRL_BS"
    };
    private static readonly string[] MIXED_TABLE = new string[32]
    {
      "CTRL_PS",
      " ",
      "\u0001",
      "\u0002",
      "\u0003",
      "\u0004",
      "\u0005",
      "\u0006",
      "\a",
      "\b",
      "\t",
      "\n",
      "\r",
      "\f",
      "\r",
      "!",
      "\"",
      "#",
      "$",
      "%",
      "@",
      "\\",
      "^",
      "_",
      "`",
      "|",
      "~",
      "±",
      "CTRL_LL",
      "CTRL_UL",
      "CTRL_PL",
      "CTRL_BS"
    };
    private static readonly string[] PUNCT_TABLE = new string[32]
    {
      "",
      "\r",
      "\r\n",
      ". ",
      ", ",
      ": ",
      "!",
      "\"",
      "#",
      "$",
      "%",
      "&",
      "'",
      "(",
      ")",
      "*",
      "+",
      ",",
      "-",
      ".",
      "/",
      ":",
      ";",
      "<",
      "=",
      ">",
      "?",
      "[",
      "]",
      "{",
      "}",
      "CTRL_UL"
    };
    private static readonly string[] DIGIT_TABLE = new string[16]
    {
      "CTRL_PS",
      " ",
      "0",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
      ",",
      ".",
      "CTRL_UL",
      "CTRL_US"
    };
    private static readonly IDictionary<Decoder.Table, string[]> codeTables = (IDictionary<Decoder.Table, string[]>) new Dictionary<Decoder.Table, string[]>()
    {
      {
        Decoder.Table.UPPER,
        Decoder.UPPER_TABLE
      },
      {
        Decoder.Table.LOWER,
        Decoder.LOWER_TABLE
      },
      {
        Decoder.Table.MIXED,
        Decoder.MIXED_TABLE
      },
      {
        Decoder.Table.PUNCT,
        Decoder.PUNCT_TABLE
      },
      {
        Decoder.Table.DIGIT,
        Decoder.DIGIT_TABLE
      },
      {
        Decoder.Table.BINARY,
        (string[]) null
      }
    };
    private static readonly IDictionary<char, Decoder.Table> codeTableMap = (IDictionary<char, Decoder.Table>) new Dictionary<char, Decoder.Table>()
    {
      {
        'U',
        Decoder.Table.UPPER
      },
      {
        'L',
        Decoder.Table.LOWER
      },
      {
        'M',
        Decoder.Table.MIXED
      },
      {
        'P',
        Decoder.Table.PUNCT
      },
      {
        'D',
        Decoder.Table.DIGIT
      },
      {
        'B',
        Decoder.Table.BINARY
      }
    };
    private AztecDetectorResult ddata;

    /// <summary>Decodes the specified detector result.</summary>
    /// <param name="detectorResult">The detector result.</param>
    /// <returns></returns>
    public DecoderResult decode(AztecDetectorResult detectorResult)
    {
      this.ddata = detectorResult;
      bool[] bits = this.extractBits(detectorResult.Bits);
      if (bits == null)
        return (DecoderResult) null;
      bool[] correctedBits = this.correctBits(bits);
      if (correctedBits == null)
        return (DecoderResult) null;
      string encodedData = Decoder.getEncodedData(correctedBits);
      return encodedData == null ? (DecoderResult) null : new DecoderResult((byte[]) null, encodedData, (IList<byte[]>) null, (string) null);
    }

    public static string highLevelDecode(bool[] correctedBits)
    {
      return Decoder.getEncodedData(correctedBits);
    }

    /// <summary>Gets the string encoded in the aztec code bits</summary>
    /// <param name="correctedBits">The corrected bits.</param>
    /// <returns>the decoded string</returns>
    private static string getEncodedData(bool[] correctedBits)
    {
      int length1 = correctedBits.Length;
      Decoder.Table table1 = Decoder.Table.UPPER;
      Decoder.Table key = Decoder.Table.UPPER;
      string[] table2 = Decoder.UPPER_TABLE;
      StringBuilder stringBuilder = new StringBuilder(20);
      int startIndex = 0;
      while (startIndex < length1)
      {
        int num1;
        switch (key)
        {
          case Decoder.Table.DIGIT:
            num1 = 4;
            break;
          case Decoder.Table.BINARY:
            if (length1 - startIndex >= 5)
            {
              int num2 = Decoder.readCode(correctedBits, startIndex, 5);
              startIndex += 5;
              if (num2 == 0)
              {
                if (length1 - startIndex >= 11)
                {
                  num2 = Decoder.readCode(correctedBits, startIndex, 11) + 31;
                  startIndex += 11;
                }
                else
                  goto label_20;
              }
              for (int index = 0; index < num2; ++index)
              {
                if (length1 - startIndex < 8)
                {
                  startIndex = length1;
                  break;
                }
                int num3 = Decoder.readCode(correctedBits, startIndex, 8);
                stringBuilder.Append((char) num3);
                startIndex += 8;
              }
              key = table1;
              table2 = Decoder.codeTables[key];
              continue;
            }
            goto label_20;
          default:
            num1 = 5;
            break;
        }
        int length2 = num1;
        if (length1 - startIndex >= length2)
        {
          int code = Decoder.readCode(correctedBits, startIndex, length2);
          startIndex += length2;
          string character = Decoder.getCharacter(table2, code);
          if (character.StartsWith("CTRL_"))
          {
            key = Decoder.getTable(character[5]);
            table2 = Decoder.codeTables[key];
            if (character[6] == 'L')
              table1 = key;
          }
          else
          {
            stringBuilder.Append(character);
            key = table1;
            table2 = Decoder.codeTables[key];
          }
        }
        else
          break;
      }
label_20:
      return stringBuilder.ToString();
    }

    /// <summary>gets the table corresponding to the char passed</summary>
    /// <param name="t">The t.</param>
    /// <returns></returns>
    private static Decoder.Table getTable(char t)
    {
      return !Decoder.codeTableMap.ContainsKey(t) ? Decoder.codeTableMap['U'] : Decoder.codeTableMap[t];
    }

    /// <summary>
    /// Gets the character (or string) corresponding to the passed code in the given table
    /// </summary>
    /// <param name="table">the table used</param>
    /// <param name="code">the code of the character</param>
    /// <returns></returns>
    private static string getCharacter(string[] table, int code) => table[code];

    /// <summary>Performs RS error correction on an array of bits.</summary>
    /// <param name="rawbits">The rawbits.</param>
    /// <returns>the corrected array</returns>
    private bool[] correctBits(bool[] rawbits)
    {
      int length1;
      GenericGF field;
      if (this.ddata.NbLayers <= 2)
      {
        length1 = 6;
        field = GenericGF.AZTEC_DATA_6;
      }
      else if (this.ddata.NbLayers <= 8)
      {
        length1 = 8;
        field = GenericGF.AZTEC_DATA_8;
      }
      else if (this.ddata.NbLayers <= 22)
      {
        length1 = 10;
        field = GenericGF.AZTEC_DATA_10;
      }
      else
      {
        length1 = 12;
        field = GenericGF.AZTEC_DATA_12;
      }
      int nbDatablocks = this.ddata.NbDatablocks;
      int length2 = rawbits.Length / length1;
      int startIndex1 = rawbits.Length % length1;
      int twoS = length2 - nbDatablocks;
      int[] received = new int[length2];
      int index1 = 0;
      while (index1 < length2)
      {
        received[index1] = Decoder.readCode(rawbits, startIndex1, length1);
        ++index1;
        startIndex1 += length1;
      }
      if (!new ReedSolomonDecoder(field).decode(received, twoS))
        return (bool[]) null;
      int num1 = (1 << length1) - 1;
      int num2 = 0;
      for (int index2 = 0; index2 < nbDatablocks; ++index2)
      {
        int num3 = received[index2];
        if (num3 == 0 || num3 == num1)
          return (bool[]) null;
        if (num3 == 1 || num3 == num1 - 1)
          ++num2;
      }
      bool[] array = new bool[nbDatablocks * length1 - num2];
      int startIndex2 = 0;
      for (int index3 = 0; index3 < nbDatablocks; ++index3)
      {
        int num4 = received[index3];
        if (num4 == 1 || num4 == num1 - 1)
        {
          SupportClass.Fill<bool>(array, startIndex2, startIndex2 + length1 - 1, num4 > 1);
          startIndex2 += length1 - 1;
        }
        else
        {
          for (int index4 = length1 - 1; index4 >= 0; --index4)
            array[startIndex2++] = (num4 & 1 << index4) != 0;
        }
      }
      return startIndex2 != array.Length ? (bool[]) null : array;
    }

    /// <summary>Gets the array of bits from an Aztec Code matrix</summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns>the array of bits</returns>
    private bool[] extractBits(BitMatrix matrix)
    {
      bool compact = this.ddata.Compact;
      int nbLayers = this.ddata.NbLayers;
      int length = compact ? 11 + nbLayers * 4 : 14 + nbLayers * 4;
      int[] numArray = new int[length];
      bool[] bits = new bool[Decoder.totalBitsInLayer(nbLayers, compact)];
      if (compact)
      {
        for (int index = 0; index < numArray.Length; ++index)
          numArray[index] = index;
      }
      else
      {
        int num1 = length + 1 + 2 * ((length / 2 - 1) / 15);
        int num2 = length / 2;
        int num3 = num1 / 2;
        for (int index = 0; index < num2; ++index)
        {
          int num4 = index + index / 15;
          numArray[num2 - index - 1] = num3 - num4 - 1;
          numArray[num2 + index] = num3 + num4 + 1;
        }
      }
      int num5 = 0;
      int num6 = 0;
      for (; num5 < nbLayers; ++num5)
      {
        int num7 = compact ? (nbLayers - num5) * 4 + 9 : (nbLayers - num5) * 4 + 12;
        int num8 = num5 * 2;
        int num9 = length - 1 - num8;
        for (int index1 = 0; index1 < num7; ++index1)
        {
          int num10 = index1 * 2;
          for (int index2 = 0; index2 < 2; ++index2)
          {
            bits[num6 + num10 + index2] = matrix[numArray[num8 + index2], numArray[num8 + index1]];
            bits[num6 + 2 * num7 + num10 + index2] = matrix[numArray[num8 + index1], numArray[num9 - index2]];
            bits[num6 + 4 * num7 + num10 + index2] = matrix[numArray[num9 - index2], numArray[num9 - index1]];
            bits[num6 + 6 * num7 + num10 + index2] = matrix[numArray[num9 - index1], numArray[num8 + index2]];
          }
        }
        num6 += num7 * 8;
      }
      return bits;
    }

    /// <summary>
    /// Reads a code of given length and at given index in an array of bits
    /// </summary>
    /// <param name="rawbits">The rawbits.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="length">The length.</param>
    /// <returns></returns>
    private static int readCode(bool[] rawbits, int startIndex, int length)
    {
      int num = 0;
      for (int index = startIndex; index < startIndex + length; ++index)
      {
        num <<= 1;
        if (rawbits[index])
          ++num;
      }
      return num;
    }

    private static int totalBitsInLayer(int layers, bool compact)
    {
      return ((compact ? 88 : 112) + 16 * layers) * layers;
    }

    private enum Table
    {
      UPPER,
      LOWER,
      MIXED,
      DIGIT,
      PUNCT,
      BINARY,
    }
  }
}
