// Decompiled with JetBrains decompiler
// Type: WhatsApp.SidecarBase
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Security.Cryptography;

#nullable disable
namespace WhatsApp
{
  public abstract class SidecarBase
  {
    private MemoryStream pendingBlock = new MemoryStream();
    private const int blockSize = 65536;
    private byte[] key;
    private byte[] iv;

    public SidecarBase(byte[] key, byte[] iv)
    {
      this.key = key;
      this.iv = new byte[iv.Length];
      Array.Copy((Array) iv, (Array) this.iv, iv.Length);
    }

    public void OnBytesIn(byte[] buffer, int offset, int length)
    {
      if (this.pendingBlock == null)
        throw new InvalidOperationException("Cannot insert to sidecar when already finalized");
      if (this.pendingBlock.Length != 0L)
      {
        int count = Math.Min(65536 - (int) this.pendingBlock.Length, length);
        this.pendingBlock.Write(buffer, offset, count);
        offset += count;
        length -= count;
        if (this.pendingBlock.Length == 65536L)
        {
          this.OnBlock(this.pendingBlock.GetBuffer(), 0, 65536);
          this.pendingBlock.SetLength(0L);
        }
      }
      for (; length >= 65536; length -= 65536)
      {
        this.OnBlock(buffer, offset, 65536);
        offset += 65536;
      }
      this.pendingBlock.Write(buffer, offset, length);
    }

    private void OnBlock(byte[] buffer, int offset, int length, bool final = false)
    {
      using (HMACSHA256 hmacshA256 = new HMACSHA256(this.key))
      {
        hmacshA256.TransformBlock(this.iv, 0, this.iv.Length, this.iv, 0);
        hmacshA256.TransformFinalBlock(buffer, offset, length);
        this.OnBytesOut(hmacshA256.Hash, 0, 10);
        if (!final)
          Array.Copy((Array) buffer, offset + length - this.iv.Length, (Array) this.iv, 0, this.iv.Length);
      }
      this.OnInputBytesProcessed(buffer, offset, length);
    }

    public void Final()
    {
      if (this.pendingBlock.Length != 0L)
        this.OnBlock(this.pendingBlock.GetBuffer(), 0, (int) this.pendingBlock.Length, true);
      this.pendingBlock.SafeDispose();
      this.pendingBlock = (MemoryStream) null;
      this.OnFinal();
    }

    protected abstract void OnBytesOut(byte[] buf, int offset, int len);

    protected abstract void OnInputBytesProcessed(byte[] buf, int offset, int len);

    protected abstract void OnFinal();
  }
}
