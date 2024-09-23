// Decompiled with JetBrains decompiler
// Type: WhatsApp.PcmAudioRecording
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.IO;


namespace WhatsApp
{
  public abstract class PcmAudioRecording : VoipStackAudioRecording
  {
    private MemoryStream stream;

    public override ulong FileSize
    {
      get
      {
        try
        {
          return this.stream == null ? 0UL : (ulong) this.stream.Length;
        }
        catch (ObjectDisposedException ex)
        {
          return 0;
        }
      }
    }

    protected override void OnMetadata(int sampleRate, Channels channelCount, int bitsPerSample)
    {
      this.stream = new MemoryStream();
    }

    protected override void OnSamples(byte[] b, StreamingUploadContext streamingContext)
    {
      this.Scheduler.PerformWithBuffer(b, (Action<byte[]>) (buf =>
      {
        buf = this.TransformBlock(buf);
        this.Write(buf, streamingContext);
      }));
    }

    protected override void NotifyAudioResult(StreamingUploadContext streamingContext)
    {
      this.Write(this.Scheduler.GetFinalBuffer(new Func<byte[]>(this.FinalBlock)), streamingContext);
      this.stream.Position = 0L;
      this.NotifyAudioResult(this.stream, this.Duration, streamingContext);
    }

    private void Write(byte[] b, StreamingUploadContext ctx)
    {
      if (b == null || b.Length == 0)
        return;
      this.stream.Write(b, 0, b.Length);
      ctx?.OnBytesIn(b);
    }

    protected virtual byte[] TransformBlock(byte[] b) => b;

    protected virtual byte[] FinalBlock() => (byte[]) null;
  }
}
