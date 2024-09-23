// Decompiled with JetBrains decompiler
// Type: System.IO.Compression.InflaterState
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.IO.Compression
{
  internal enum InflaterState
  {
    ReadingHeader = 0,
    ReadingBFinal = 2,
    ReadingBType = 3,
    ReadingNumLitCodes = 4,
    ReadingNumDistCodes = 5,
    ReadingNumCodeLengthCodes = 6,
    ReadingCodeLengthCodes = 7,
    ReadingTreeCodesBefore = 8,
    ReadingTreeCodesAfter = 9,
    DecodeTop = 10, // 0x0000000A
    HaveInitialLength = 11, // 0x0000000B
    HaveFullLength = 12, // 0x0000000C
    HaveDistCode = 13, // 0x0000000D
    UncompressedAligning = 15, // 0x0000000F
    UncompressedByte1 = 16, // 0x00000010
    UncompressedByte2 = 17, // 0x00000011
    UncompressedByte3 = 18, // 0x00000012
    UncompressedByte4 = 19, // 0x00000013
    DecodingUncompressed = 20, // 0x00000014
    StartReadingFooter = 21, // 0x00000015
    ReadingFooter = 22, // 0x00000016
    VerifyingFooter = 23, // 0x00000017
    Done = 24, // 0x00000018
  }
}
