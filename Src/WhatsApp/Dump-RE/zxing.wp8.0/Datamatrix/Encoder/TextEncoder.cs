// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Encoder.TextEncoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.Datamatrix.Encoder
{
  internal sealed class TextEncoder : C40Encoder
  {
    public override int EncodingMode => 2;

    protected override int encodeChar(char c, StringBuilder sb)
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
      if (c >= 'a' && c <= 'z')
      {
        sb.Append((char) ((int) c - 97 + 14));
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
      if (c == '`')
      {
        sb.Append('\u0002');
        sb.Append((char) ((uint) c - 96U));
        return 2;
      }
      if (c >= 'A' && c <= 'Z')
      {
        sb.Append('\u0002');
        sb.Append((char) ((int) c - 65 + 1));
        return 2;
      }
      if (c >= '{' && c <= '\u007F')
      {
        sb.Append('\u0002');
        sb.Append((char) ((int) c - 123 + 27));
        return 2;
      }
      if (c >= '\u0080')
      {
        sb.Append("\u0001\u001E");
        return 2 + this.encodeChar((char) ((uint) c - 128U), sb);
      }
      HighLevelEncoder.illegalCharacter(c);
      return -1;
    }
  }
}
