// Decompiled with JetBrains decompiler
// Type: WhatsApp.WavWriter
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.IO;
using System.Text;


namespace WhatsApp
{
  public class WavWriter
  {
    private Stream output;
    private const long fileSizeOffset = 4;
    private long sampleSizeOffset;
    private long sampleSize;
    private int sampleRate;
    private ushort channels;
    private ushort bitsPerSample;
    private static byte[] fileMagic = Encoding.UTF8.GetBytes("RIFF");
    private static byte[] formatMagic = Encoding.UTF8.GetBytes("WAVE");
    private static byte[] formatChunkFmt = Encoding.UTF8.GetBytes("fmt ");
    private static byte[] formatChunkData = Encoding.UTF8.GetBytes("data");

    public WavWriter(Stream output, int sampleRate, Channels channels, ushort bitsPerSample)
    {
      this.output = output;
      this.sampleRate = sampleRate;
      this.channels = (ushort) channels;
      this.bitsPerSample = bitsPerSample;
      this.WriteHeader();
    }

    private void WriteHeader()
    {
      this.output.Position = 0L;
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
        {
          binaryWriter.Write(WavWriter.fileMagic);
          binaryWriter.Write(0);
          binaryWriter.Write(WavWriter.formatMagic);
          binaryWriter.Write(WavWriter.formatChunkFmt);
          binaryWriter.Write(16);
          binaryWriter.Write((ushort) 1);
          binaryWriter.Write(this.channels);
          binaryWriter.Write(this.sampleRate);
          binaryWriter.Write(this.sampleRate * (int) this.channels * (int) this.bitsPerSample / 8);
          binaryWriter.Write((ushort) ((int) this.channels * (int) this.bitsPerSample / 8));
          binaryWriter.Write(this.bitsPerSample);
          binaryWriter.Write(WavWriter.formatChunkData);
          this.sampleSizeOffset = output.Position;
          binaryWriter.Write(0);
          this.output.Write(output.GetBuffer(), 0, (int) output.Position);
        }
      }
    }

    public void AddSamples(byte[] payload)
    {
      this.output.Write(payload, 0, payload.Length);
      this.sampleSize += (long) payload.Length;
      this.UpdateSizes();
    }

    public void DiscardBuffer()
    {
      this.sampleSize = 0L;
      this.output.Position = this.OffsetToPayload;
      this.output.SetLength(this.OffsetToPayload);
      this.UpdateSizes();
    }

    private void UpdateSizes()
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
        {
          binaryWriter.Write((int) (this.output.Position - 8L));
          binaryWriter.Write((int) this.sampleSize);
          byte[] buffer = output.GetBuffer();
          long position = this.output.Position;
          this.output.Position = 4L;
          this.output.Write(buffer, 0, 4);
          this.output.Position = this.sampleSizeOffset;
          this.output.Write(buffer, 4, 4);
          this.output.Position = this.output.Length;
        }
      }
    }

    public long OffsetToPayload => this.sampleSizeOffset + 4L;

    public int BytesPerSample => (int) this.bitsPerSample * (int) this.channels / 8;

    public long LengthInSamples => this.sampleSize / (long) this.BytesPerSample;

    public double LengthInSeconds => (double) this.LengthInSamples / (double) this.sampleRate;

    public ulong TotalSampleSizeInBytes => this.sampleSize <= 0L ? 0UL : (ulong) this.sampleSize;
  }
}
