// Decompiled with JetBrains decompiler
// Type: WhatsApp.OpusAudioRecording
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using WhatsAppNative;


namespace WhatsApp
{
  public class OpusAudioRecording : PcmAudioRecording
  {
    private IOpusEnc opusEnc;
    private IByteBuffer bb;

    public override string MimeType => "audio/ogg; codecs=opus";

    public override string FileExtension => "opus";

    public override bool ShouldStream => true;

    public OpusAudioRecording()
    {
      this.Scheduler = (AudioRecordingScheduler) new EncodeThreadAudioRecordingScheduler();
    }

    protected override void OnMetadata(int sampleRate, Channels channelCount, int bitsPerSample)
    {
      if (bitsPerSample != 16)
        throw new InvalidOperationException("Only 16 bit audio supported");
      base.OnMetadata(sampleRate, channelCount, bitsPerSample);
      OpusEncoderParams EncoderParams = new OpusEncoderParams();
      EncoderParams.SampleRate = sampleRate;
      EncoderParams.ChannelCount = (int) channelCount;
      EncoderParams.Application = OpusApplication.Voip;
      if (sampleRate > 16000)
        EncoderParams.ForcedSampleRate = DeviceSpecificSampleRates.Get().ResampleRate;
      this.opusEnc = (IOpusEnc) NativeInterfaces.CreateInstance<OpusEnc>();
      this.opusEnc.Initialize(EncoderParams);
    }

    protected override byte[] TransformBlock(byte[] b)
    {
      b = base.TransformBlock(b);
      if (b == null || b.Length == 0)
        return b;
      if (this.bb == null)
        this.bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      this.bb.Put(b, 0, b.Length);
      this.opusEnc.OnSamples(this.bb);
      this.bb.Reset();
      IByteBuffer buffer = this.opusEnc.GetBuffer();
      return buffer.GetLength() != 0 ? buffer.Get() : (byte[]) null;
    }

    protected override byte[] FinalBlock()
    {
      this.opusEnc.Flush();
      IByteBuffer buffer = this.opusEnc.GetBuffer();
      try
      {
        if (buffer.GetLength() != 0)
          return buffer.Get();
      }
      finally
      {
        this.opusEnc.Dispose();
        this.opusEnc = (IOpusEnc) null;
      }
      return (byte[]) null;
    }
  }
}
