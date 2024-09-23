// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Encoder.C40Encoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text;

#nullable disable
namespace ZXing.Datamatrix.Encoder
{
  internal class C40Encoder : ZXing.Datamatrix.Encoder.Encoder
  {
    public virtual int EncodingMode => 1;

    public virtual void encode(EncoderContext context)
    {
      StringBuilder stringBuilder = new StringBuilder();
      while (context.HasMoreCharacters)
      {
        char currentChar = context.CurrentChar;
        ++context.Pos;
        int lastCharSize = this.encodeChar(currentChar, stringBuilder);
        int num1 = stringBuilder.Length / 3 * 2;
        int len = context.CodewordCount + num1;
        context.updateSymbolInfo(len);
        int num2 = context.SymbolInfo.dataCapacity - len;
        if (!context.HasMoreCharacters)
        {
          StringBuilder removed = new StringBuilder();
          if (stringBuilder.Length % 3 == 2 && (num2 < 2 || num2 > 2))
            lastCharSize = this.backtrackOneCharacter(context, stringBuilder, removed, lastCharSize);
          while (stringBuilder.Length % 3 == 1 && (lastCharSize <= 3 && num2 != 1 || lastCharSize > 3))
            lastCharSize = this.backtrackOneCharacter(context, stringBuilder, removed, lastCharSize);
          break;
        }
        if (stringBuilder.Length % 3 == 0)
        {
          int encoding = HighLevelEncoder.lookAheadTest(context.Message, context.Pos, this.EncodingMode);
          if (encoding != this.EncodingMode)
          {
            context.signalEncoderChange(encoding);
            break;
          }
        }
      }
      this.handleEOD(context, stringBuilder);
    }

    private int backtrackOneCharacter(
      EncoderContext context,
      StringBuilder buffer,
      StringBuilder removed,
      int lastCharSize)
    {
      int length = buffer.Length;
      buffer.Remove(length - lastCharSize, lastCharSize);
      --context.Pos;
      lastCharSize = this.encodeChar(context.CurrentChar, removed);
      context.resetSymbolInfo();
      return lastCharSize;
    }

    internal static void writeNextTriplet(EncoderContext context, StringBuilder buffer)
    {
      context.writeCodewords(C40Encoder.encodeToCodewords(buffer, 0));
      buffer.Remove(0, 3);
    }

    /// <summary>Handle "end of data" situations</summary>
    /// <param name="context">the encoder context</param>
    /// <param name="buffer">the buffer with the remaining encoded characters</param>
    protected virtual void handleEOD(EncoderContext context, StringBuilder buffer)
    {
      int num1 = buffer.Length / 3 * 2;
      int num2 = buffer.Length % 3;
      int len = context.CodewordCount + num1;
      context.updateSymbolInfo(len);
      int num3 = context.SymbolInfo.dataCapacity - len;
      if (num2 == 2)
      {
        buffer.Append(char.MinValue);
        while (buffer.Length >= 3)
          C40Encoder.writeNextTriplet(context, buffer);
        if (context.HasMoreCharacters)
          context.writeCodeword('þ');
      }
      else if (num3 == 1 && num2 == 1)
      {
        while (buffer.Length >= 3)
          C40Encoder.writeNextTriplet(context, buffer);
        if (context.HasMoreCharacters)
          context.writeCodeword('þ');
        --context.Pos;
      }
      else
      {
        if (num2 != 0)
          throw new InvalidOperationException("Unexpected case. Please report!");
        while (buffer.Length >= 3)
          C40Encoder.writeNextTriplet(context, buffer);
        if (num3 > 0 || context.HasMoreCharacters)
          context.writeCodeword('þ');
      }
      context.signalEncoderChange(0);
    }

    protected virtual int encodeChar(char c, StringBuilder sb)
    {
      if (c == ' ')
      {
        sb.Append('\u0003');
        return 1;
      }
      if (c >= '0' && c <= '9')
      {
        sb.Append((char) ((int) c - 48 + 4));
        return 1;
      }
      if (c >= 'A' && c <= 'Z')
      {
        sb.Append((char) ((int) c - 65 + 14));
        return 1;
      }
      if (c >= char.MinValue && c <= '\u001F')
      {
        sb.Append(char.MinValue);
        sb.Append(c);
        return 2;
      }
      if (c >= '!' && c <= '/')
      {
        sb.Append('\u0001');
        sb.Append((char) ((uint) c - 33U));
        return 2;
      }
      if (c >= ':' && c <= '@')
      {
        sb.Append('\u0001');
        sb.Append((char) ((int) c - 58 + 15));
        return 2;
      }
      if (c >= '[' && c <= '_')
      {
        sb.Append('\u0001');
        sb.Append((char) ((int) c - 91 + 22));
        return 2;
      }
      if (c >= '`' && c <= '\u007F')
      {
        sb.Append('\u0002');
        sb.Append((char) ((uint) c - 96U));
        return 2;
      }
      if (c < '\u0080')
        throw new InvalidOperationException("Illegal character: " + (object) c);
      sb.Append("\u0001\u001E");
      return 2 + this.encodeChar((char) ((uint) c - 128U), sb);
    }

    private static string encodeToCodewords(StringBuilder sb, int startPos)
    {
      int num = 1600 * (int) sb[startPos] + 40 * (int) sb[startPos + 1] + (int) sb[startPos + 2] + 1;
      return new string(new char[2]
      {
        (char) (num / 256),
        (char) (num % 256)
      });
    }
  }
}
