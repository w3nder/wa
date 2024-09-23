// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.CompressionMethod
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public enum CompressionMethod
  {
    Stored = 0,
    Deflated = 8,
    Deflate64 = 9,
    BZip2 = 11, // 0x0000000B
    WinZipAES = 99, // 0x00000063
  }
}
