// Decompiled with JetBrains decompiler
// Type: WhatsApp.RC4
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp
{
  public class RC4
  {
    private int[] s = new int[256];
    private int i;
    private int j;

    public RC4(byte[] key, int drop)
    {
      for (this.i = 0; this.i < this.s.Length; ++this.i)
        this.s[this.i] = this.i;
      this.j = 0;
      for (this.i = 0; this.i < this.s.Length; ++this.i)
      {
        this.j = (int) (byte) ((uint) (this.j + this.s[this.i]) + (uint) key[this.i % key.Length]);
        RC4.Swap<int>(this.s, this.i, this.j);
      }
      this.i = this.j = 0;
      this.Cipher(new byte[drop]);
    }

    private static void Swap<T>(T[] a, int i, int j)
    {
      T obj = a[i];
      a[i] = a[j];
      a[j] = obj;
    }

    public void Cipher(byte[] data, int offset, int length)
    {
      while (length-- != 0)
      {
        this.i = (this.i + 1) % 256;
        this.j = (this.j + this.s[this.i]) % 256;
        int num = this.s[this.i];
        this.s[this.i] = this.s[this.j];
        this.s[this.j] = num;
        data[offset++] ^= (byte) this.s[(this.s[this.i] + this.s[this.j]) % 256];
      }
    }

    public void Cipher(byte[] data) => this.Cipher(data, 0, data.Length);
  }
}
