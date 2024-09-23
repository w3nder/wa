// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Encoder.EdifactEncoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text;

#nullable disable
namespace ZXing.Datamatrix.Encoder
{
  internal sealed class EdifactEncoder : ZXing.Datamatrix.Encoder.Encoder
  {
    public int EncodingMode => 4;

    public void encode(EncoderContext context)
    {
      StringBuilder stringBuilder = new StringBuilder();
      while (context.HasMoreCharacters)
      {
        EdifactEncoder.encodeChar(context.CurrentChar, stringBuilder);
        ++context.Pos;
        if (stringBuilder.Length >= 4)
        {
          context.writeCodewords(EdifactEncoder.encodeToCodewords(stringBuilder, 0));
          stringBuilder.Remove(0, 4);
          if (HighLevelEncoder.lookAheadTest(context.Message, context.Pos, this.EncodingMode) != this.EncodingMode)
          {
            context.signalEncoderChange(0);
            break;
          }
        }
      }
      stringBuilder.Append('\u001F');
      EdifactEncoder.handleEOD(context, stringBuilder);
    }

    /// <summary>Handle "end of data" situations</summary>
    /// <param name="context">the encoder context</param>
    /// <param name="buffer">the buffer with the remaining encoded characters</param>
    private static void handleEOD(EncoderContext context, StringBuilder buffer)
    {
      try
      {
        int length = buffer.Length;
        switch (length)
        {
          case 0:
            return;
          case 1:
            context.updateSymbolInfo();
            int num1 = context.SymbolInfo.dataCapacity - context.CodewordCount;
            if (context.RemainingCharacters == 0 && num1 <= 2)
              return;
            break;
        }
        if (length > 4)
          throw new InvalidOperationException("Count must not exceed 4");
        int num2 = length - 1;
        string codewords = EdifactEncoder.encodeToCodewords(buffer, 0);
        bool flag = !context.HasMoreCharacters && num2 <= 2;
        if (num2 <= 2)
        {
          context.updateSymbolInfo(context.CodewordCount + num2);
          if (context.SymbolInfo.dataCapacity - context.CodewordCount >= 3)
          {
            flag = false;
            context.updateSymbolInfo(context.CodewordCount + codewords.Length);
          }
        }
        if (flag)
        {
          context.resetSymbolInfo();
          context.Pos -= num2;
        }
        else
          context.writeCodewords(codewords);
      }
      finally
      {
        context.signalEncoderChange(0);
      }
    }

    private static void encodeChar(char c, StringBuilder sb)
    {
      if (c >= ' ' && c <= '?')
        sb.Append(c);
      else if (c >= '@' && c <= '^')
        sb.Append((char) ((uint) c - 64U));
      else
        HighLevelEncoder.illegalCharacter(c);
    }

    private static string encodeToCodewords(StringBuilder sb, int startPos)
    {
      int num1 = sb.Length - startPos;
      if (num1 == 0)
        throw new InvalidOperationException("StringBuilder must not be empty");
      int num2 = ((int) sb[startPos] << 18) + ((num1 >= 2 ? (int) sb[startPos + 1] : (int) char.MinValue) << 12) + ((num1 >= 3 ? (int) sb[startPos + 2] : (int) char.MinValue) << 6) + (num1 >= 4 ? (int) sb[startPos + 3] : (int) char.MinValue);
      char ch1 = (char) (num2 >> 16 & (int) byte.MaxValue);
      char ch2 = (char) (num2 >> 8 & (int) byte.MaxValue);
      char ch3 = (char) (num2 & (int) byte.MaxValue);
      StringBuilder stringBuilder = new StringBuilder(3);
      stringBuilder.Append(ch1);
      if (num1 >= 2)
        stringBuilder.Append(ch2);
      if (num1 >= 3)
        stringBuilder.Append(ch3);
      return stringBuilder.ToString();
    }
  }
}
