// Decompiled with JetBrains decompiler
// Type: WhatsApp.ManagedSink
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class ManagedSink : ISampleSink
  {
    public Action<byte[]> OnBytes;
    public Action<long> OnTimestamp;

    public void OnSampleAvailable(IByteBuffer bb, long timestamp, long duration)
    {
      this.OnBytes(bb.Get());
      Action<long> onTimestamp = this.OnTimestamp;
      if (onTimestamp == null)
        return;
      onTimestamp(timestamp);
    }
  }
}
