// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.Internal.Token
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;

#nullable disable
namespace ZXing.Aztec.Internal
{
  public abstract class Token
  {
    public static Token EMPTY = (Token) new SimpleToken((Token) null, 0, 0);
    private readonly Token previous;

    protected Token(Token previous) => this.previous = previous;

    public Token Previous => this.previous;

    public Token add(int value, int bitCount) => (Token) new SimpleToken(this, value, bitCount);

    public Token addBinaryShift(int start, int byteCount)
    {
      return (Token) new BinaryShiftToken(this, start, byteCount);
    }

    public abstract void appendTo(BitArray bitArray, byte[] text);
  }
}
