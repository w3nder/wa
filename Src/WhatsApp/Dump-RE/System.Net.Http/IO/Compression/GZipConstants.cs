// Decompiled with JetBrains decompiler
// Type: System.IO.Compression.GZipConstants
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.IO.Compression
{
  internal static class GZipConstants
  {
    internal const int CompressionLevel_3 = 3;
    internal const int CompressionLevel_10 = 10;
    internal const long FileLengthModulo = 4294967296;
    internal const byte ID1 = 31;
    internal const byte ID2 = 139;
    internal const byte Deflate = 8;
    internal const int Xfl_HeaderPos = 8;
    internal const byte Xfl_FastestAlgorithm = 4;
    internal const byte Xfl_MaxCompressionSlowestAlgorithm = 2;
  }
}
