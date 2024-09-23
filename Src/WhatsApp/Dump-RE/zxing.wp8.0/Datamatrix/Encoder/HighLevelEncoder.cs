// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Encoder.HighLevelEncoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text;

#nullable disable
namespace ZXing.Datamatrix.Encoder
{
  /// <summary>
  /// DataMatrix ECC 200 data encoder following the algorithm described in ISO/IEC 16022:200(E) in
  /// annex S.
  /// </summary>
  internal static class HighLevelEncoder
  {
    /// <summary>Padding character</summary>
    public const char PAD = '\u0081';
    /// <summary>mode latch to C40 encodation mode</summary>
    public const char LATCH_TO_C40 = 'æ';
    /// <summary>mode latch to Base 256 encodation mode</summary>
    public const char LATCH_TO_BASE256 = 'ç';
    /// <summary>FNC1 Codeword</summary>
    public const char FNC1 = 'è';
    /// <summary>Structured Append Codeword</summary>
    public const char STRUCTURED_APPEND = 'é';
    /// <summary>Reader Programming</summary>
    public const char READER_PROGRAMMING = 'ê';
    /// <summary>Upper Shift</summary>
    public const char UPPER_SHIFT = 'ë';
    /// <summary>05 Macro</summary>
    public const char MACRO_05 = 'ì';
    /// <summary>06 Macro</summary>
    public const char MACRO_06 = 'í';
    /// <summary>mode latch to ANSI X.12 encodation mode</summary>
    public const char LATCH_TO_ANSIX12 = 'î';
    /// <summary>mode latch to Text encodation mode</summary>
    public const char LATCH_TO_TEXT = 'ï';
    /// <summary>mode latch to EDIFACT encodation mode</summary>
    public const char LATCH_TO_EDIFACT = 'ð';
    /// <summary>ECI character (Extended Channel Interpretation)</summary>
    public const char ECI = 'ñ';
    /// <summary>Unlatch from C40 encodation</summary>
    public const char C40_UNLATCH = 'þ';
    /// <summary>Unlatch from X12 encodation</summary>
    public const char X12_UNLATCH = 'þ';
    /// <summary>05 Macro header</summary>
    public const string MACRO_05_HEADER = "[)>\u001E05\u001D";
    /// <summary>06 Macro header</summary>
    public const string MACRO_06_HEADER = "[)>\u001E06\u001D";
    /// <summary>Macro trailer</summary>
    public const string MACRO_TRAILER = "\u001E\u0004";

    private static char randomize253State(char ch, int codewordPosition)
    {
      int num1 = 149 * codewordPosition % 253 + 1;
      int num2 = (int) ch + num1;
      return num2 > 254 ? (char) (num2 - 254) : (char) num2;
    }

    /// <summary>
    /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
    /// of ISO/IEC 16022:2000(E).
    /// </summary>
    /// <param name="msg">the message</param>
    /// <returns>the encoded message (the char values range from 0 to 255)</returns>
    public static string encodeHighLevel(string msg)
    {
      return HighLevelEncoder.encodeHighLevel(msg, SymbolShapeHint.FORCE_NONE, (Dimension) null, (Dimension) null, 0);
    }

    /// <summary>
    /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
    /// of ISO/IEC 16022:2000(E).
    /// </summary>
    /// <param name="msg">the message</param>
    /// <param name="shape">requested shape. May be {@code SymbolShapeHint.FORCE_NONE},{@code SymbolShapeHint.FORCE_SQUARE} or {@code SymbolShapeHint.FORCE_RECTANGLE}.</param>
    /// <param name="minSize">the minimum symbol size constraint or null for no constraint</param>
    /// <param name="maxSize">the maximum symbol size constraint or null for no constraint</param>
    /// <returns>the encoded message (the char values range from 0 to 255)</returns>
    public static string encodeHighLevel(
      string msg,
      SymbolShapeHint shape,
      Dimension minSize,
      Dimension maxSize,
      int defaultEncodation)
    {
      ZXing.Datamatrix.Encoder.Encoder[] encoderArray = new ZXing.Datamatrix.Encoder.Encoder[6]
      {
        (ZXing.Datamatrix.Encoder.Encoder) new ASCIIEncoder(),
        (ZXing.Datamatrix.Encoder.Encoder) new C40Encoder(),
        (ZXing.Datamatrix.Encoder.Encoder) new TextEncoder(),
        (ZXing.Datamatrix.Encoder.Encoder) new X12Encoder(),
        (ZXing.Datamatrix.Encoder.Encoder) new EdifactEncoder(),
        (ZXing.Datamatrix.Encoder.Encoder) new Base256Encoder()
      };
      EncoderContext context = new EncoderContext(msg);
      context.setSymbolShape(shape);
      context.setSizeConstraints(minSize, maxSize);
      if (msg.StartsWith("[)>\u001E05\u001D") && msg.EndsWith("\u001E\u0004"))
      {
        context.writeCodeword('ì');
        context.setSkipAtEnd(2);
        context.Pos += "[)>\u001E05\u001D".Length;
      }
      else if (msg.StartsWith("[)>\u001E06\u001D") && msg.EndsWith("\u001E\u0004"))
      {
        context.writeCodeword('í');
        context.setSkipAtEnd(2);
        context.Pos += "[)>\u001E06\u001D".Length;
      }
      int index = defaultEncodation;
      switch (index)
      {
        case 0:
          while (context.HasMoreCharacters)
          {
            encoderArray[index].encode(context);
            if (context.NewEncoding >= 0)
            {
              index = context.NewEncoding;
              context.resetEncoderSignal();
            }
          }
          int length = context.Codewords.Length;
          context.updateSymbolInfo();
          int dataCapacity = context.SymbolInfo.dataCapacity;
          if (length < dataCapacity && index != 0 && index != 5)
            context.writeCodeword('þ');
          StringBuilder codewords = context.Codewords;
          if (codewords.Length < dataCapacity)
            codewords.Append('\u0081');
          while (codewords.Length < dataCapacity)
            codewords.Append(HighLevelEncoder.randomize253State('\u0081', codewords.Length + 1));
          return context.Codewords.ToString();
        case 1:
          context.writeCodeword('æ');
          goto case 0;
        case 2:
          context.writeCodeword('ï');
          goto case 0;
        case 3:
          context.writeCodeword('î');
          goto case 0;
        case 4:
          context.writeCodeword('ð');
          goto case 0;
        case 5:
          context.writeCodeword('ç');
          goto case 0;
        default:
          throw new InvalidOperationException("Illegal mode: " + (object) index);
      }
    }

    internal static int lookAheadTest(string msg, int startpos, int currentMode)
    {
      if (startpos >= msg.Length)
        return currentMode;
      float[] charCounts;
      if (currentMode == 0)
      {
        charCounts = new float[6]
        {
          0.0f,
          1f,
          1f,
          1f,
          1f,
          1.25f
        };
      }
      else
      {
        charCounts = new float[6]
        {
          1f,
          2f,
          2f,
          2f,
          2f,
          2.25f
        };
        charCounts[currentMode] = 0.0f;
      }
      int num = 0;
      while (startpos + num != msg.Length)
      {
        char ch1 = msg[startpos + num];
        ++num;
        if (HighLevelEncoder.isDigit(ch1))
          charCounts[0] += 0.5f;
        else if (HighLevelEncoder.isExtendedASCII(ch1))
        {
          charCounts[0] = (float) (int) Math.Ceiling((double) charCounts[0]);
          charCounts[0] += 2f;
        }
        else
        {
          charCounts[0] = (float) (int) Math.Ceiling((double) charCounts[0]);
          ++charCounts[0];
        }
        if (HighLevelEncoder.isNativeC40(ch1))
          charCounts[1] += 0.6666667f;
        else if (HighLevelEncoder.isExtendedASCII(ch1))
          charCounts[1] += 2.66666675f;
        else
          charCounts[1] += 1.33333337f;
        if (HighLevelEncoder.isNativeText(ch1))
          charCounts[2] += 0.6666667f;
        else if (HighLevelEncoder.isExtendedASCII(ch1))
          charCounts[2] += 2.66666675f;
        else
          charCounts[2] += 1.33333337f;
        if (HighLevelEncoder.isNativeX12(ch1))
          charCounts[3] += 0.6666667f;
        else if (HighLevelEncoder.isExtendedASCII(ch1))
          charCounts[3] += 4.33333349f;
        else
          charCounts[3] += 3.33333325f;
        if (HighLevelEncoder.isNativeEDIFACT(ch1))
          charCounts[4] += 0.75f;
        else if (HighLevelEncoder.isExtendedASCII(ch1))
          charCounts[4] += 4.25f;
        else
          charCounts[4] += 3.25f;
        if (HighLevelEncoder.isSpecialB256(ch1))
          charCounts[5] += 4f;
        else
          ++charCounts[5];
        if (num >= 4)
        {
          int[] intCharCounts = new int[6];
          byte[] mins = new byte[6];
          HighLevelEncoder.findMinimums(charCounts, intCharCounts, int.MaxValue, mins);
          int minimumCount = HighLevelEncoder.getMinimumCount(mins);
          if (intCharCounts[0] < intCharCounts[5] && intCharCounts[0] < intCharCounts[1] && intCharCounts[0] < intCharCounts[2] && intCharCounts[0] < intCharCounts[3] && intCharCounts[0] < intCharCounts[4])
            return 0;
          if (intCharCounts[5] < intCharCounts[0] || (int) mins[1] + (int) mins[2] + (int) mins[3] + (int) mins[4] == 0)
            return 5;
          if (minimumCount == 1 && mins[4] > (byte) 0)
            return 4;
          if (minimumCount == 1 && mins[2] > (byte) 0)
            return 2;
          if (minimumCount == 1 && mins[3] > (byte) 0)
            return 3;
          if (intCharCounts[1] + 1 < intCharCounts[0] && intCharCounts[1] + 1 < intCharCounts[5] && intCharCounts[1] + 1 < intCharCounts[4] && intCharCounts[1] + 1 < intCharCounts[2])
          {
            if (intCharCounts[1] < intCharCounts[3])
              return 1;
            if (intCharCounts[1] == intCharCounts[3])
            {
              for (int index = startpos + num + 1; index < msg.Length; ++index)
              {
                char ch2 = msg[index];
                if (HighLevelEncoder.isX12TermSep(ch2))
                  return 3;
                if (!HighLevelEncoder.isNativeX12(ch2))
                  break;
              }
              return 1;
            }
          }
        }
      }
      int maxValue = int.MaxValue;
      byte[] mins1 = new byte[6];
      int[] intCharCounts1 = new int[6];
      int minimums = HighLevelEncoder.findMinimums(charCounts, intCharCounts1, maxValue, mins1);
      int minimumCount1 = HighLevelEncoder.getMinimumCount(mins1);
      if (intCharCounts1[0] == minimums)
        return 0;
      if (minimumCount1 == 1 && mins1[5] > (byte) 0)
        return 5;
      if (minimumCount1 == 1 && mins1[4] > (byte) 0)
        return 4;
      if (minimumCount1 == 1 && mins1[2] > (byte) 0)
        return 2;
      return minimumCount1 == 1 && mins1[3] > (byte) 0 ? 3 : 1;
    }

    private static int findMinimums(float[] charCounts, int[] intCharCounts, int min, byte[] mins)
    {
      SupportClass.Fill<byte>(mins, (byte) 0);
      for (int index = 0; index < 6; ++index)
      {
        intCharCounts[index] = (int) Math.Ceiling((double) charCounts[index]);
        int intCharCount = intCharCounts[index];
        if (min > intCharCount)
        {
          min = intCharCount;
          SupportClass.Fill<byte>(mins, (byte) 0);
        }
        if (min == intCharCount)
          ++mins[index];
      }
      return min;
    }

    private static int getMinimumCount(byte[] mins)
    {
      int minimumCount = 0;
      for (int index = 0; index < 6; ++index)
        minimumCount += (int) mins[index];
      return minimumCount;
    }

    internal static bool isDigit(char ch) => ch >= '0' && ch <= '9';

    internal static bool isExtendedASCII(char ch) => ch >= '\u0080' && ch <= 'ÿ';

    internal static bool isNativeC40(char ch)
    {
      if (ch == ' ' || ch >= '0' && ch <= '9')
        return true;
      return ch >= 'A' && ch <= 'Z';
    }

    internal static bool isNativeText(char ch)
    {
      if (ch == ' ' || ch >= '0' && ch <= '9')
        return true;
      return ch >= 'a' && ch <= 'z';
    }

    internal static bool isNativeX12(char ch)
    {
      if (HighLevelEncoder.isX12TermSep(ch) || ch == ' ' || ch >= '0' && ch <= '9')
        return true;
      return ch >= 'A' && ch <= 'Z';
    }

    internal static bool isX12TermSep(char ch) => ch == '\r' || ch == '*' || ch == '>';

    internal static bool isNativeEDIFACT(char ch) => ch >= ' ' && ch <= '^';

    internal static bool isSpecialB256(char ch) => false;

    /// <summary>
    /// Determines the number of consecutive characters that are encodable using numeric compaction.
    /// </summary>
    /// <param name="msg">the message</param>
    /// <param name="startpos">the start position within the message</param>
    /// <returns>the requested character count</returns>
    public static int determineConsecutiveDigitCount(string msg, int startpos)
    {
      int consecutiveDigitCount = 0;
      int length = msg.Length;
      int index = startpos;
      if (index < length)
      {
        char ch = msg[index];
        while (HighLevelEncoder.isDigit(ch) && index < length)
        {
          ++consecutiveDigitCount;
          ++index;
          if (index < length)
            ch = msg[index];
        }
      }
      return consecutiveDigitCount;
    }

    internal static void illegalCharacter(char c)
    {
      throw new ArgumentException(string.Format("Illegal character: {0} (0x{1:X})", (object) c, (object) (int) c));
    }
  }
}
