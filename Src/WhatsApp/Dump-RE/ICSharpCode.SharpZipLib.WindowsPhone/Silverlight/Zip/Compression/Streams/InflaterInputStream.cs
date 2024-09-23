// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.Compression.Streams.InflaterInputStream
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;
using System.Security.Cryptography;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip.Compression.Streams
{
  public class InflaterInputStream : Stream
  {
    protected Stream baseInputStream;
    protected long csize;
    protected Inflater inf;
    protected InflaterInputBuffer inputBuffer;
    private bool isClosed;
    private bool isStreamOwner = true;

    public InflaterInputStream(Stream baseInputStream)
      : this(baseInputStream, new Inflater(), 4096)
    {
    }

    public InflaterInputStream(Stream baseInputStream, Inflater inf)
      : this(baseInputStream, inf, 4096)
    {
    }

    public InflaterInputStream(Stream baseInputStream, Inflater inflater, int bufferSize)
    {
      if (baseInputStream == null)
        throw new ArgumentNullException(nameof (baseInputStream));
      if (inflater == null)
        throw new ArgumentNullException(nameof (inflater));
      if (bufferSize <= 0)
        throw new ArgumentOutOfRangeException(nameof (bufferSize));
      this.baseInputStream = baseInputStream;
      this.inf = inflater;
      this.inputBuffer = new InflaterInputBuffer(baseInputStream, bufferSize);
    }

    public bool IsStreamOwner
    {
      get => this.isStreamOwner;
      set => this.isStreamOwner = value;
    }

    public virtual int Available => !this.inf.IsFinished ? 1 : 0;

    public long Skip(long count)
    {
      if (count < 0L)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (this.baseInputStream.CanSeek)
      {
        this.baseInputStream.Seek(count, SeekOrigin.Current);
        return count;
      }
      int length = 2048;
      if (count < (long) length)
        length = (int) count;
      byte[] buffer = new byte[length];
      return (long) this.baseInputStream.Read(buffer, 0, buffer.Length);
    }

    protected void StopDecrypting() => this.inputBuffer.CryptoTransform = (ICryptoTransform) null;

    protected void Fill()
    {
      this.inputBuffer.Fill();
      this.inputBuffer.SetInflaterInput(this.inf);
    }

    public override bool CanRead => this.baseInputStream.CanRead;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => (long) this.inputBuffer.RawLength;

    public override long Position
    {
      get => this.baseInputStream.Position;
      set => throw new NotSupportedException("InflaterInputStream Position not supported");
    }

    public override void Flush() => this.baseInputStream.Flush();

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException("Seek not supported");
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException("InflaterInputStream SetLength not supported");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException("InflaterInputStream Write not supported");
    }

    public override void WriteByte(byte value)
    {
      throw new NotSupportedException("InflaterInputStream WriteByte not supported");
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
    }

    public override void Close()
    {
      if (this.isClosed)
        return;
      this.isClosed = true;
      if (!this.isStreamOwner)
        return;
      this.baseInputStream.Close();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this.inf.IsNeedingDictionary)
        throw new SharpZipBaseException("Need a dictionary");
      int count1 = count;
      int num;
      do
      {
        num = this.inf.Inflate(buffer, offset, count1);
        offset += num;
        count1 -= num;
        if (count1 != 0 && !this.inf.IsFinished)
        {
          if (this.inf.IsNeedingInput)
            this.Fill();
        }
        else
          goto label_8;
      }
      while (num != 0);
      throw new ZipException("Dont know what to do");
label_8:
      return count - count1;
    }
  }
}
