// Decompiled with JetBrains decompiler
// Type: WhatsApp.AudioRecordingScheduler
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;


namespace WhatsApp
{
  public class AudioRecordingScheduler
  {
    public virtual void PerformWithBuffer(byte[] buf, Action<byte[]> a) => a(buf);

    public virtual byte[] GetFinalBuffer(Func<byte[]> get) => get();
  }
}
