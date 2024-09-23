// Decompiled with JetBrains decompiler
// Type: WhatsApp.QrCrypto
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Security.Cryptography;


namespace WhatsApp
{
  public class QrCrypto
  {
    private byte[] cryptKey;
    private byte[] macKey;
    private static FunXMPP.MemoryStanzaWriter memoryStanzaWriter = new FunXMPP.MemoryStanzaWriter();
    private static TokenDictionary dict = new TokenDictionary((ITokenDictionary) new QrDictionary());
    private FunXMPP.BinTreeNodeReader reader = new FunXMPP.BinTreeNodeReader((FunXMPP.StanzaProvider) null, (FunXMPP.Connection) null, QrCrypto.dict);
    private FunXMPP.BinTreeNodeWriter writer;
    private object writeLock = new object();

    public QrCrypto(byte[] cryptKey, byte[] macKey)
    {
      this.cryptKey = cryptKey;
      this.macKey = macKey;
      this.writer = new FunXMPP.BinTreeNodeWriter((FunXMPP.StanzaWriter) QrCrypto.memoryStanzaWriter, QrCrypto.dict);
    }

    public FunXMPP.ProtocolTreeNode ParseBlob(byte[] buf, int offset, int length)
    {
      return this.reader.ParseTreeNode((Stream) this.Decode(buf, offset, length));
    }

    public MemoryStream CreateBlob(FunXMPP.ProtocolTreeNode node)
    {
      lock (this.writeLock)
        this.writer.Write(node, false);
      MemoryStream stanzaStream = QrCrypto.memoryStanzaWriter.StanzaStream;
      QrCrypto.memoryStanzaWriter.StanzaStream = (MemoryStream) null;
      return this.Encode(stanzaStream.GetBuffer(), 0, (int) stanzaStream.Length);
    }

    public byte[] GenPassword(byte[] pubKey)
    {
      byte[] pubKey1;
      byte[] privKey;
      Curve22519Extensions.GenKeyPair(out pubKey1, out privKey);
      byte[] buffer1 = HkdfSha256.Perform(80, Curve22519Extensions.Derive(pubKey, privKey));
      MemoryStream memoryStream1 = new MemoryStream(buffer1, 0, buffer1.Length, false);
      byte[] rgbKey = new byte[32];
      byte[] key = new byte[32];
      byte[] rgbIV = new byte[16];
      byte[][] numArray = new byte[3][]
      {
        rgbKey,
        key,
        rgbIV
      };
      foreach (byte[] buffer2 in numArray)
        memoryStream1.Read(buffer2, 0, buffer2.Length);
      MemoryStream memoryStream2 = new MemoryStream();
      using (SymmetricAlgorithm algo = this.CreateAlgo())
      {
        using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream2, algo.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write))
        {
          cryptoStream.Write(this.cryptKey, 0, this.cryptKey.Length);
          cryptoStream.Write(this.macKey, 0, this.macKey.Length);
          cryptoStream.FlushFinalBlock();
          MemoryStream memoryStream3 = new MemoryStream();
          byte[] hash;
          using (HMACSHA256 hmacshA256 = new HMACSHA256(key))
          {
            hmacshA256.TransformBlock(pubKey1, 0, pubKey1.Length, pubKey1, 0);
            hmacshA256.TransformFinalBlock(memoryStream2.GetBuffer(), 0, (int) memoryStream2.Length);
            hash = hmacshA256.Hash;
          }
          memoryStream3.Write(pubKey1, 0, pubKey1.Length);
          memoryStream3.Write(hash, 0, hash.Length);
          memoryStream3.Write(memoryStream2.GetBuffer(), 0, (int) memoryStream2.Length);
          memoryStream2 = memoryStream3;
        }
      }
      return memoryStream2.ToArray();
    }

    public static byte[] GenerateToken()
    {
      RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
      byte[] token = new byte[20];
      byte[] data = token;
      cryptoServiceProvider.GetBytes(data);
      return token;
    }

    private MemoryStream Decode(byte[] buf, int offset, int length)
    {
      int hmacLen = 32;
      int ivLen = 16;
      if (length < hmacLen + ivLen)
        throw new FunXMPP.CorruptStreamException("unexpected length " + (object) length);
      this.CheckHmac(buf, offset, hmacLen, buf, offset + hmacLen, length - hmacLen);
      return this.Decode(buf, offset + hmacLen, ivLen, buf, offset + hmacLen + ivLen, length - hmacLen - ivLen);
    }

    public byte[] GenHmac(byte[] payload, int offset, int length)
    {
      using (HMACSHA256 hmacshA256 = new HMACSHA256(this.macKey))
        return hmacshA256.ComputeHash(payload, offset, length);
    }

    private void CheckHmac(
      byte[] hmacBuffer,
      int hmacOffset,
      int hmacLen,
      byte[] payload,
      int offset,
      int length)
    {
      byte[] numArray = this.GenHmac(payload, offset, length);
      for (int index = 0; index < numArray.Length; ++index)
      {
        if ((int) hmacBuffer[hmacOffset + index] != (int) numArray[index])
          throw new FunXMPP.CorruptStreamException("invalid hmac");
      }
    }

    public SymmetricAlgorithm CreateAlgo()
    {
      AesManaged d = new AesManaged();
      try
      {
        d.KeySize = 256;
        return (SymmetricAlgorithm) d;
      }
      catch (Exception ex)
      {
        d.SafeDispose();
        throw;
      }
    }

    private MemoryStream Decode(
      byte[] ivBuffer,
      int ivOffset,
      int ivLen,
      byte[] payload,
      int offset,
      int length)
    {
      using (SymmetricAlgorithm algo = this.CreateAlgo())
      {
        byte[] numArray = new byte[ivLen];
        Array.Copy((Array) ivBuffer, ivOffset, (Array) numArray, 0, ivLen);
        using (CryptoStream cryptoStream = new CryptoStream((Stream) new MemoryStream(payload, offset, length, false), algo.CreateDecryptor(this.cryptKey, numArray), CryptoStreamMode.Read))
        {
          MemoryStream destination = new MemoryStream();
          cryptoStream.CopyTo((Stream) destination);
          destination.Position = 0L;
          return destination;
        }
      }
    }

    private MemoryStream Encode(byte[] payload, int offset, int length)
    {
      int offset1 = 32;
      MemoryStream memoryStream = new MemoryStream();
      memoryStream.SetLength((long) offset1);
      memoryStream.Position = memoryStream.Length;
      byte[] numArray = new byte[16];
      new RNGCryptoServiceProvider().GetBytes(numArray);
      memoryStream.Write(numArray, 0, numArray.Length);
      using (SymmetricAlgorithm algo = this.CreateAlgo())
      {
        using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, algo.CreateEncryptor(this.cryptKey, numArray), CryptoStreamMode.Write))
        {
          cryptoStream.Write(payload, offset, length);
          cryptoStream.FlushFinalBlock();
          MemoryStream destination = new MemoryStream();
          memoryStream.Position = 0L;
          memoryStream.CopyTo((Stream) destination);
          memoryStream = destination;
        }
      }
      byte[] sourceArray = this.GenHmac(memoryStream.GetBuffer(), offset1, (int) memoryStream.Length - offset1);
      Array.Copy((Array) sourceArray, (Array) memoryStream.GetBuffer(), sourceArray.Length);
      memoryStream.Position = 0L;
      return memoryStream;
    }
  }
}
