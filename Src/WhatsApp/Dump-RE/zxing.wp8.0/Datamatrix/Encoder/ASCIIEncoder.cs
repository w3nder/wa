// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Encoder.ASCIIEncoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.Datamatrix.Encoder
{
  internal sealed class ASCIIEncoder : ZXing.Datamatrix.Encoder.Encoder
  {
    public int EncodingMode => 0;

    public void encode(EncoderContext context)
    {
      if (HighLevelEncoder.determineConsecutiveDigitCount(context.Message, context.Pos) >= 2)
      {
        context.writeCodeword(ASCIIEncoder.encodeASCIIDigits(context.Message[context.Pos], context.Message[context.Pos + 1]));
        context.Pos += 2;
      }
      else
      {
        char currentChar = context.CurrentChar;
        int num = HighLevelEncoder.lookAheadTest(context.Message, context.Pos, this.EncodingMode);
        if (num != this.EncodingMode)
        {
          switch (num)
          {
            case 1:
              context.writeCodeword('æ');
              context.signalEncoderChange(1);
              break;
            case 2:
              context.writeCodeword('ï');
              context.signalEncoderChange(2);
              break;
            case 3:
              context.writeCodeword('î');
              context.signalEncoderChange(3);
              break;
            case 4:
              context.writeCodeword('ð');
              context.signalEncoderChange(4);
              break;
            case 5:
              context.writeCodeword('ç');
              context.signalEncoderChange(5);
              break;
            default:
              throw new InvalidOperationException("Illegal mode: " + (object) num);
          }
        }
        else if (HighLevelEncoder.isExtendedASCII(currentChar))
        {
          context.writeCodeword('ë');
          context.writeCodeword((char) ((int) currentChar - 128 + 1));
          ++context.Pos;
        }
        else
        {
          context.writeCodeword((char) ((uint) currentChar + 1U));
          ++context.Pos;
        }
      }
    }

    private static char encodeASCIIDigits(char digit1, char digit2)
    {
      if (HighLevelEncoder.isDigit(digit1) && HighLevelEncoder.isDigit(digit2))
        return (char) (((int) digit1 - 48) * 10 + ((int) digit2 - 48) + 130);
      throw new ArgumentException("not digits: " + (object) digit1 + (object) digit2);
    }
  }
}
