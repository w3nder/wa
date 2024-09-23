// Decompiled with JetBrains decompiler
// Type: WhatsApp.HkdfSha256
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Security.Cryptography;


namespace WhatsApp
{
  public class HkdfSha256
  {
    public static byte[] Perform(int keyLen, byte[] key, byte[] salt = null, byte[] info = null)
    {
      return Hkdf.Perform(key, salt, info, keyLen, 32, new Hkdf.HmacFunction(HkdfSha256.PerformHmac));
    }

    private static byte[] PerformHmac(byte[] key, byte[] payload, int offset, int length)
    {
      using (HMACSHA256 hmacshA256 = new HMACSHA256(key))
        return hmacshA256.ComputeHash(payload, offset, length);
    }
  }
}
