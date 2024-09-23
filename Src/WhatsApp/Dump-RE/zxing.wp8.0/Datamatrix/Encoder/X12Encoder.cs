// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Encoder.X12Encoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.Datamatrix.Encoder
{
  internal sealed class X12Encoder : C40Encoder
  {
    public override int EncodingMode => 3;

    public override void encode(EncoderContext context)
    {
      StringBuilder stringBuilder = new StringBuilder();
      while (context.HasMoreCharacters)
      {
        char currentChar = context.CurrentChar;
        ++context.Pos;
        this.encodeChar(currentChar, stringBuilder);
        if (stringBuilder.Length % 3 == 0)
        {
          C40Encoder.writeNextTriplet(context, stringBuilder);
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

    protected override int encodeChar(char c, StringBuilder sb)
    {
      switch (c)
      {
        case '\r':
          sb.Append(char.MinValue);
          break;
        case ' ':
          sb.Append('\u0003');
          break;
        case '*':
          sb.Append('\u0001');
          break;
        case '>':
          sb.Append('\u0002');
          break;
        default:
          if (c >= '0' && c <= '9')
          {
            sb.Append((char) ((int) c - 48 + 4));
            break;
          }
          if (c >= 'A' && c <= 'Z')
          {
            sb.Append((char) ((int) c - 65 + 14));
            break;
          }
          HighLevelEncoder.illegalCharacter(c);
          break;
      }
      return 1;
    }

    protected override void handleEOD(EncoderContext context, StringBuilder buffer)
    {
      context.updateSymbolInfo();
      int num = context.SymbolInfo.dataCapacity - context.CodewordCount;
      switch (buffer.Length)
      {
        case 1:
          --context.Pos;
          if (num > 1)
            context.writeCodeword('þ');
          context.signalEncoderChange(0);
          break;
        case 2:
          context.writeCodeword('þ');
          context.Pos -= 2;
          context.signalEncoderChange(0);
          break;
      }
    }
  }
}
