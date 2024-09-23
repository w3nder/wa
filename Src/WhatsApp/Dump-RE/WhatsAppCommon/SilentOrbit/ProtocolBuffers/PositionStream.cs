// Decompiled with JetBrains decompiler
// Type: SilentOrbit.ProtocolBuffers.PositionStream
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;

#nullable disable
namespace SilentOrbit.ProtocolBuffers
{
  public class PositionStream : Stream
  {
    private Stream stream;

    public int BytesRead { get; private set; }

    public PositionStream(Stream baseStream) => this.stream = baseStream;

    public override void Flush() => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
      int num = this.stream.Read(buffer, offset, count);
      this.BytesRead += num;
      return num;
    }

    public override int ReadByte()
    {
      int num = this.stream.ReadByte();
      ++this.BytesRead;
      return num;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => this.stream.Length;

    public override long Position
    {
      get => (long) this.BytesRead;
      set => throw new NotImplementedException();
    }

    protected override void Dispose(bool disposing)
    {
      this.stream.Dispose();
      base.Dispose(disposing);
    }
  }
}
