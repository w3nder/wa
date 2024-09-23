// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.EncryptionAlgorithm
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public enum EncryptionAlgorithm
  {
    None = 0,
    PkzipClassic = 1,
    Des = 26113, // 0x00006601
    RC2 = 26114, // 0x00006602
    TripleDes168 = 26115, // 0x00006603
    TripleDes112 = 26121, // 0x00006609
    Aes128 = 26126, // 0x0000660E
    Aes192 = 26127, // 0x0000660F
    Aes256 = 26128, // 0x00006610
    RC2Corrected = 26370, // 0x00006702
    Blowfish = 26400, // 0x00006720
    Twofish = 26401, // 0x00006721
    RC4 = 26625, // 0x00006801
    Unknown = 65535, // 0x0000FFFF
  }
}
