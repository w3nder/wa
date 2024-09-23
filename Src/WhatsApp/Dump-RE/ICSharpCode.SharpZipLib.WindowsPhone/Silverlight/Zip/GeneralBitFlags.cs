// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.GeneralBitFlags
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  [Flags]
  public enum GeneralBitFlags
  {
    Encrypted = 1,
    Method = 6,
    Descriptor = 8,
    ReservedPKware4 = 16, // 0x00000010
    Patched = 32, // 0x00000020
    StrongEncryption = 64, // 0x00000040
    Unused7 = 128, // 0x00000080
    Unused8 = 256, // 0x00000100
    Unused9 = 512, // 0x00000200
    Unused10 = 1024, // 0x00000400
    UnicodeText = 2048, // 0x00000800
    EnhancedCompress = 4096, // 0x00001000
    HeaderMasked = 8192, // 0x00002000
    ReservedPkware14 = 16384, // 0x00004000
    ReservedPkware15 = 32768, // 0x00008000
  }
}
