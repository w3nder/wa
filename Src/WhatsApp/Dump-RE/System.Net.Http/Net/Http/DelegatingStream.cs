// Decompiled with JetBrains decompiler
// Type: System.Net.Http.DelegatingStream
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.IO;

#nullable disable
namespace System.Net.Http
{
  internal abstract class DelegatingStream : Stream
  {
    private Stream innerStream;

    public override bool CanRead => this.innerStream.CanRead;

    public override bool CanSeek => this.innerStream.CanSeek;

    public override bool CanWrite => this.innerStream.CanWrite;

    public override long Length => this.innerStream.Length;

    public override long Position
    {
      get => this.innerStream.Position;
      set => this.innerStream.Position = value;
    }

    public override int ReadTimeout
    {
      get => this.innerStream.ReadTimeout;
      set => this.innerStream.ReadTimeout = value;
    }

    public override bool CanTimeout => this.innerStream.CanTimeout;

    public override int WriteTimeout
    {
      get => this.innerStream.WriteTimeout;
      set => this.innerStream.WriteTimeout = value;
    }

    protected DelegatingStream(Stream innerStream)
    {
      Contract.Assert(innerStream != null);
      this.innerStream = innerStream;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.innerStream.Dispose();
      base.Dispose(disposing);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return this.innerStream.Seek(offset, origin);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return this.innerStream.Read(buffer, offset, count);
    }

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.innerStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult) => this.innerStream.EndRead(asyncResult);

    public override int ReadByte() => this.innerStream.ReadByte();

    public override void Flush() => this.innerStream.Flush();

    public override void SetLength(long value) => this.innerStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.innerStream.Write(buffer, offset, count);
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.innerStream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
      this.innerStream.EndWrite(asyncResult);
    }

    public override void WriteByte(byte value) => this.innerStream.WriteByte(value);
  }
}
