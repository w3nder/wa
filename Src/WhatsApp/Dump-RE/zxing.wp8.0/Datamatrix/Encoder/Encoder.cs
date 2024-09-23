// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Encoder.Encoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Datamatrix.Encoder
{
  internal interface Encoder
  {
    int EncodingMode { get; }

    void encode(EncoderContext context);
  }
}
