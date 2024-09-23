// Decompiled with JetBrains decompiler
// Type: WhatsApp.Hkdf
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.IO;

#nullable disable
namespace WhatsApp
{
  public class Hkdf
  {
    public static byte[] Perform(
      byte[] key,
      byte[] salt,
      byte[] info,
      int keyLen,
      int hashLen,
      Hkdf.HmacFunction hmac)
    {
      return Hkdf.Expand(Hkdf.Extract(key, salt, hashLen, hmac), info ?? new byte[0], keyLen, hashLen, hmac);
    }

    private static byte[] Extract(byte[] key, byte[] salt, int hashLen, Hkdf.HmacFunction hmac)
    {
      if (salt == null)
        salt = new byte[hashLen];
      return hmac(salt, key, 0, key.Length);
    }

    private static byte[] Expand(
      byte[] prk,
      byte[] info,
      int keyLen,
      int hashLen,
      Hkdf.HmacFunction hmac)
    {
      MemoryStream memoryStream1 = new MemoryStream();
      MemoryStream memoryStream2 = new MemoryStream();
      byte[] buffer1 = new byte[1];
      byte[] buffer2 = new byte[0];
      int num = 1;
      while (memoryStream1.Length < (long) keyLen)
      {
        memoryStream2.Write(buffer2, 0, buffer2.Length);
        memoryStream2.Write(info, 0, info.Length);
        buffer1[0] = (byte) num++;
        memoryStream2.Write(buffer1, 0, buffer1.Length);
        buffer2 = hmac(prk, memoryStream2.GetBuffer(), 0, (int) memoryStream2.Length);
        memoryStream2.SetLength(0L);
        memoryStream1.Write(buffer2, 0, buffer2.Length);
      }
      memoryStream2.Capacity = 0;
      memoryStream1.SetLength((long) keyLen);
      return memoryStream1.ToArray();
    }

    public delegate byte[] HmacFunction(byte[] key, byte[] payload, int offset, int length);
  }
}
