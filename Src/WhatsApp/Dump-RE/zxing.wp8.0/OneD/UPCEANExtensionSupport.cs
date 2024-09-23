// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.UPCEANExtensionSupport
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  internal sealed class UPCEANExtensionSupport
  {
    private static readonly int[] EXTENSION_START_PATTERN = new int[3]
    {
      1,
      1,
      2
    };
    private readonly UPCEANExtension2Support twoSupport = new UPCEANExtension2Support();
    private readonly UPCEANExtension5Support fiveSupport = new UPCEANExtension5Support();

    internal Result decodeRow(int rowNumber, BitArray row, int rowOffset)
    {
      int[] guardPattern = UPCEANReader.findGuardPattern(row, rowOffset, false, UPCEANExtensionSupport.EXTENSION_START_PATTERN);
      return guardPattern == null ? (Result) null : this.fiveSupport.decodeRow(rowNumber, row, guardPattern) ?? this.twoSupport.decodeRow(rowNumber, row, guardPattern);
    }
  }
}
