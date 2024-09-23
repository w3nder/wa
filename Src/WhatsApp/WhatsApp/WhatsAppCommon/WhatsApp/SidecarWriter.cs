// Decompiled with JetBrains decompiler
// Type: WhatsApp.SidecarWriter
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;


namespace WhatsApp
{
  public class SidecarWriter : SidecarBase
  {
    private byte[] sidecarBytes;
    private MemoryStream sidecarStream = new MemoryStream();

    public SidecarWriter(byte[] key, byte[] iv)
      : base(key, iv)
    {
    }

    protected override void OnBytesOut(byte[] buf, int offset, int len)
    {
      this.sidecarStream.Write(buf, offset, len);
    }

    protected override void OnInputBytesProcessed(byte[] buf, int offset, int len)
    {
    }

    protected override void OnFinal()
    {
    }

    public byte[] Value
    {
      get
      {
        return Utils.LazyInit<byte[]>(ref this.sidecarBytes, (Func<byte[]>) (() =>
        {
          this.Final();
          byte[] array = this.sidecarStream.ToArray();
          this.sidecarStream.SafeDispose();
          this.sidecarStream = (MemoryStream) null;
          return array;
        }), (object) this);
      }
    }
  }
}
