// Decompiled with JetBrains decompiler
// Type: WhatsApp.WavSource
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Text;
using WhatsAppNative;


namespace WhatsApp
{
  public class WavSource : ISoundSource
  {
    private AudioMetadata metadata;
    private Stream stream;
    private uint length;
    private byte[] sampleBuffer;
    private IByteBuffer sampleBufferCom;
    private long startOfSamples;
    private uint fileLength;

    public WavSource(Stream str)
    {
      this.stream = str;
      if ((int) this.Read32() != (int) this.StringToU32("RIFF"))
        throw new IOException("Not a RIFF file");
      int num1 = (int) this.Read32();
      if ((int) this.Read32() != (int) this.StringToU32("WAVE"))
        throw new IOException("Not a WAVE file");
      uint u32_1 = this.StringToU32("fmt ");
      uint u32_2 = this.StringToU32("data");
      bool flag1 = false;
      uint num2;
      ushort num3;
      ushort num4;
      while (true)
      {
        uint num5 = this.Read32();
        num2 = this.Read32();
        long position = this.stream.Position;
        if ((int) num5 == (int) u32_1)
        {
          if (num2 >= 16U)
          {
            num3 = this.Read16();
            ushort num6 = this.Read16();
            uint num7 = this.Read32();
            int num8 = (int) this.Read32();
            int num9 = (int) this.Read16();
            num4 = this.Read16();
            if (num3 == (ushort) 1)
            {
              if (num4 != (ushort) 0 && (int) num4 % 8 == 0)
              {
                this.metadata.Channels = (int) num6;
                this.metadata.BitsPerSample = (int) num4;
                this.metadata.SampleRate = (int) num7;
                flag1 = true;
              }
              else
                goto label_11;
            }
            else
              goto label_9;
          }
          else
            break;
        }
        else if ((int) num5 == (int) u32_2)
          goto label_14;
        this.stream.Position = position + (long) num2;
      }
      throw new IOException("Format header too short");
label_9:
      throw new IOException("Unsupported codec " + (object) num3 + "; we only do PCM");
label_11:
      throw new IOException("Unexpected bits per sample: " + (object) num4);
label_14:
      if (!flag1)
        throw new IOException("Did not see fmt header");
      bool flag2 = true;
      this.length = num2;
      if (!flag2)
        throw new IOException("Did not see data start");
      this.metadata.SamplesPerFrame = Math.Min(this.metadata.SampleRate * 50 / 1000, (int) ((long) this.length / (long) (this.metadata.BitsPerSample / 8)));
      this.sampleBuffer = new byte[this.metadata.SamplesPerFrame * this.metadata.BitsPerSample / 8];
      this.sampleBufferCom = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      this.sampleBufferCom.Put(this.sampleBuffer);
      this.startOfSamples = this.stream.Position;
      this.fileLength = this.length;
    }

    private byte ReadByte(Stream s = null)
    {
      int num = (s ?? this.stream).ReadByte();
      return num >= 0 ? (byte) num : throw new IOException("Unexpected end of file");
    }

    private ushort Read16(Stream s = null)
    {
      return (ushort) ((uint) this.ReadByte(s) | (uint) this.ReadByte(s) << 8);
    }

    private uint Read32(Stream s = null)
    {
      int num1 = (int) this.ReadByte(s);
      uint num2 = (uint) this.ReadByte(s);
      uint num3 = (uint) this.ReadByte(s);
      uint num4 = (uint) this.ReadByte(s);
      int num5 = (int) num2 << 8;
      return (uint) (num1 | num5 | (int) num3 << 16 | (int) num4 << 24);
    }

    private uint StringToU32(string s)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(s);
      if (bytes.Length != 4)
        throw new ArgumentException("must be 4 ascii chars");
      using (MemoryStream s1 = new MemoryStream(bytes, false))
        return this.Read32((Stream) s1);
    }

    public bool FillBuffer(IByteBuffer buf)
    {
      if (this.length == 0U)
        return false;
      int index = this.stream.Read(this.sampleBuffer, 0, Math.Min(this.sampleBuffer.Length, (int) this.length));
      if (index < 0)
        throw new IOException("unexpected read length");
      if (index < this.sampleBuffer.Length)
        Array.Clear((Array) this.sampleBuffer, index, this.sampleBuffer.Length - index);
      this.length -= (uint) index;
      buf.CopyFrom(this.sampleBufferCom);
      return this.length > 0U;
    }

    public AudioMetadata GetMetadata() => this.metadata;

    public void Seek(long millis)
    {
      if (millis < 0L)
        throw new InvalidOperationException("milliseconds is negative");
      long num = millis * (long) this.metadata.SampleRate / 1000L / (long) this.metadata.SamplesPerFrame * (long) this.metadata.Channels * (long) this.metadata.SamplesPerFrame * (long) this.metadata.BitsPerSample / 8L;
      if (num < 0L || num >= (long) this.length)
      {
        this.length = 0U;
      }
      else
      {
        this.stream.Position = this.startOfSamples + num;
        this.length = (uint) ((ulong) this.fileLength - (ulong) num);
      }
    }

    public long GetPosition()
    {
      long position = this.stream.Position;
      return position <= this.startOfSamples ? 0L : 1000L * ((position - this.startOfSamples) / (long) (this.metadata.BitsPerSample / 8 * this.metadata.Channels)) / (long) this.metadata.SampleRate;
    }

    public long GetDuration()
    {
      return 1000L * ((long) this.fileLength - this.startOfSamples) / (long) this.metadata.SampleRate;
    }
  }
}
