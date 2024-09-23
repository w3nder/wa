// Decompiled with JetBrains decompiler
// Type: WhatsApp.SidecarVerifier
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class SidecarVerifier : SidecarBase
  {
    private byte[] toVerify;
    private int offset;
    private int len;
    public Action<byte[], int, int> OnBytes;

    protected override void OnBytesOut(byte[] buf, int offset, int len)
    {
      if (len == 0)
        return;
      int num = Math.Min(len, this.len);
      if (num != 0)
      {
        if (!Extensions.IsEqualBytes(this.toVerify, this.offset, num, buf, offset, num))
          throw new SidecarVerifierException("HMAC does not match");
        this.offset += num;
        this.len -= num;
        offset += num;
        len -= num;
      }
      if (len != 0)
        throw new SidecarVerifierException("Computed sidecar is longer than stored value");
    }

    protected override void OnInputBytesProcessed(byte[] buf, int offset, int len)
    {
      Action<byte[], int, int> onBytes = this.OnBytes;
      if (onBytes == null)
        return;
      onBytes(buf, offset, len);
    }

    protected override void OnFinal()
    {
      if (this.len != 0)
        throw new SidecarVerifierException("Stored sidecar is longer than computed copy");
    }

    public SidecarVerifier(
      byte[] key,
      byte[] iv,
      byte[] toVerify,
      int toVerifyOffset,
      int toVerifyLen)
      : base(key, iv)
    {
      this.toVerify = toVerify;
      this.offset = toVerifyOffset;
      this.len = toVerifyLen;
    }

    public SidecarVerifier(byte[] key, byte[] iv, byte[] toVerify)
      : this(key, iv, toVerify, 0, toVerify.Length)
    {
    }
  }
}
