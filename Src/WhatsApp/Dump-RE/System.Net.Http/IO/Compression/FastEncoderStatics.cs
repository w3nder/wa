// Decompiled with JetBrains decompiler
// Type: System.IO.Compression.FastEncoderStatics
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.IO.Compression
{
  internal static class FastEncoderStatics
  {
    public static uint BitReverse(uint code, int length)
    {
      uint num = 0;
      do
      {
        num = (num | code & 1U) << 1;
        code >>= 1;
      }
      while (--length > 0);
      return num >> 1;
    }
  }
}
