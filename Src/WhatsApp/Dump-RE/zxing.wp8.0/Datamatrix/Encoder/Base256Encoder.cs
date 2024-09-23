// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Encoder.Base256Encoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text;

#nullable disable
namespace ZXing.Datamatrix.Encoder
{
  internal sealed class Base256Encoder : ZXing.Datamatrix.Encoder.Encoder
  {
    public int EncodingMode => 5;

    public void encode(EncoderContext context)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(char.MinValue);
      while (context.HasMoreCharacters)
      {
        char currentChar = context.CurrentChar;
        stringBuilder.Append(currentChar);
        ++context.Pos;
        int encoding = HighLevelEncoder.lookAheadTest(context.Message, context.Pos, this.EncodingMode);
        if (encoding != this.EncodingMode)
        {
          context.signalEncoderChange(encoding);
          break;
        }
      }
      int num = stringBuilder.Length - 1;
      int len = context.CodewordCount + num + 1;
      context.updateSymbolInfo(len);
      bool flag = context.SymbolInfo.dataCapacity - len > 0;
      if (context.HasMoreCharacters || flag)
      {
        if (num <= 249)
        {
          stringBuilder[0] = (char) num;
        }
        else
        {
          if (num <= 249 || num > 1555)
            throw new InvalidOperationException("Message length not in valid ranges: " + (object) num);
          stringBuilder[0] = (char) (num / 250 + 249);
          stringBuilder.Insert(1, new char[1]
          {
            (char) (num % 250)
          });
        }
      }
      int length = stringBuilder.Length;
      for (int index = 0; index < length; ++index)
        context.writeCodeword(Base256Encoder.randomize255State(stringBuilder[index], context.CodewordCount + 1));
    }

    private static char randomize255State(char ch, int codewordPosition)
    {
      int num1 = 149 * codewordPosition % (int) byte.MaxValue + 1;
      int num2 = (int) ch + num1;
      return num2 <= (int) byte.MaxValue ? (char) num2 : (char) (num2 - 256);
    }
  }
}
