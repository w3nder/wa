// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Encryption.PkzipClassicManaged
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.Security.Cryptography;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Encryption
{
  public sealed class PkzipClassicManaged : PkzipClassic
  {
    private byte[] key_;

    public override int BlockSize
    {
      get => 8;
      set
      {
        if (value != 8)
          throw new CryptographicException("Block size is invalid");
      }
    }

    public override KeySizes[] LegalKeySizes
    {
      get => new KeySizes[1]{ new KeySizes(96, 96, 0) };
    }

    public override KeySizes[] LegalBlockSizes
    {
      get => new KeySizes[1]{ new KeySizes(8, 8, 0) };
    }

    public override byte[] Key
    {
      get
      {
        if (this.key_ == null)
          this.GenerateKey();
        return (byte[]) this.key_.Clone();
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        this.key_ = value.Length == 12 ? (byte[]) value.Clone() : throw new CryptographicException("Key size is illegal");
      }
    }

    public override void GenerateIV()
    {
    }

    public override void GenerateKey()
    {
      this.key_ = new byte[12];
      new Random().NextBytes(this.key_);
    }

    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
    {
      this.key_ = rgbKey;
      return (ICryptoTransform) new PkzipClassicEncryptCryptoTransform(this.Key);
    }

    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
    {
      this.key_ = rgbKey;
      return (ICryptoTransform) new PkzipClassicDecryptCryptoTransform(this.Key);
    }
  }
}
