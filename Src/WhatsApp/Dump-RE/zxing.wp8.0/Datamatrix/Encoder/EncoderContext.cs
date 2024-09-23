// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Encoder.EncoderContext
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text;

#nullable disable
namespace ZXing.Datamatrix.Encoder
{
  internal sealed class EncoderContext
  {
    private readonly string msg;
    private SymbolShapeHint shape;
    private Dimension minSize;
    private Dimension maxSize;
    private readonly StringBuilder codewords;
    private int pos;
    private int newEncoding;
    private SymbolInfo symbolInfo;
    private int skipAtEnd;
    private static readonly Encoding encoding = Encoding.GetEncoding("ISO-8859-1");

    public EncoderContext(string msg)
    {
      byte[] bytes = EncoderContext.encoding.GetBytes(msg);
      StringBuilder stringBuilder = new StringBuilder(bytes.Length);
      int length = bytes.Length;
      for (int index = 0; index < length; ++index)
      {
        char ch = (char) ((uint) bytes[index] & (uint) byte.MaxValue);
        if (ch == '?' && msg[index] != '?')
          throw new ArgumentException("Message contains characters outside " + EncoderContext.encoding.WebName + " encoding.");
        stringBuilder.Append(ch);
      }
      this.msg = stringBuilder.ToString();
      this.shape = SymbolShapeHint.FORCE_NONE;
      this.codewords = new StringBuilder(msg.Length);
      this.newEncoding = -1;
    }

    public void setSymbolShape(SymbolShapeHint shape) => this.shape = shape;

    public void setSizeConstraints(Dimension minSize, Dimension maxSize)
    {
      this.minSize = minSize;
      this.maxSize = maxSize;
    }

    public void setSkipAtEnd(int count) => this.skipAtEnd = count;

    public char CurrentChar => this.msg[this.pos];

    public char Current => this.msg[this.pos];

    public void writeCodewords(string codewords) => this.codewords.Append(codewords);

    public void writeCodeword(char codeword) => this.codewords.Append(codeword);

    public int CodewordCount => this.codewords.Length;

    public void signalEncoderChange(int encoding) => this.newEncoding = encoding;

    public void resetEncoderSignal() => this.newEncoding = -1;

    public bool HasMoreCharacters => this.pos < this.TotalMessageCharCount;

    private int TotalMessageCharCount => this.msg.Length - this.skipAtEnd;

    public int RemainingCharacters => this.TotalMessageCharCount - this.pos;

    public void updateSymbolInfo() => this.updateSymbolInfo(this.CodewordCount);

    public void updateSymbolInfo(int len)
    {
      if (this.symbolInfo != null && len <= this.symbolInfo.dataCapacity)
        return;
      this.symbolInfo = SymbolInfo.lookup(len, this.shape, this.minSize, this.maxSize, true);
    }

    public void resetSymbolInfo() => this.symbolInfo = (SymbolInfo) null;

    public int Pos
    {
      get => this.pos;
      set => this.pos = value;
    }

    public StringBuilder Codewords => this.codewords;

    public SymbolInfo SymbolInfo => this.symbolInfo;

    public int NewEncoding => this.newEncoding;

    public string Message => this.msg;
  }
}
