// Decompiled with JetBrains decompiler
// Type: System.IO.Compression.GZipStream
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.IO.Compression
{
  internal class GZipStream : Stream
  {
    private DeflateStream deflateStream;

    public GZipStream(Stream stream, CompressionMode mode)
      : this(stream, mode, false)
    {
    }

    public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
    {
      this.deflateStream = new DeflateStream(stream, mode, leaveOpen);
      this.SetDeflateStreamFileFormatter(mode);
    }

    private void SetDeflateStreamFileFormatter(CompressionMode mode)
    {
      if (mode == CompressionMode.Compress)
        throw new NotSupportedException(Resources.NotSupported);
      this.deflateStream.SetFileFormatReader((IFileFormatReader) new GZipDecoder());
    }

    public override bool CanRead => this.deflateStream != null && this.deflateStream.CanRead;

    public override bool CanWrite => this.deflateStream != null && this.deflateStream.CanWrite;

    public override bool CanSeek => this.deflateStream != null && this.deflateStream.CanSeek;

    public override long Length => throw new NotSupportedException(Resources.NotSupported);

    public override long Position
    {
      get => throw new NotSupportedException(Resources.NotSupported);
      set => throw new NotSupportedException(Resources.NotSupported);
    }

    public override void Flush()
    {
      if (this.deflateStream == null)
        throw new ObjectDisposedException((string) null, Resources.ObjectDisposed_StreamClosed);
      this.deflateStream.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException(Resources.NotSupported);
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException(Resources.NotSupported);
    }

    public override IAsyncResult BeginRead(
      byte[] array,
      int offset,
      int count,
      AsyncCallback asyncCallback,
      object asyncState)
    {
      if (this.deflateStream == null)
        throw new InvalidOperationException(Resources.ObjectDisposed_StreamClosed);
      return this.deflateStream.BeginRead(array, offset, count, asyncCallback, asyncState);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      return this.deflateStream != null ? this.deflateStream.EndRead(asyncResult) : throw new InvalidOperationException(Resources.ObjectDisposed_StreamClosed);
    }

    public override IAsyncResult BeginWrite(
      byte[] array,
      int offset,
      int count,
      AsyncCallback asyncCallback,
      object asyncState)
    {
      if (this.deflateStream == null)
        throw new InvalidOperationException(Resources.ObjectDisposed_StreamClosed);
      return this.deflateStream.BeginWrite(array, offset, count, asyncCallback, asyncState);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
      if (this.deflateStream == null)
        throw new InvalidOperationException(Resources.ObjectDisposed_StreamClosed);
      this.deflateStream.EndWrite(asyncResult);
    }

    public override int Read(byte[] array, int offset, int count)
    {
      if (this.deflateStream == null)
        throw new ObjectDisposedException((string) null, Resources.ObjectDisposed_StreamClosed);
      return this.deflateStream.Read(array, offset, count);
    }

    public override void Write(byte[] array, int offset, int count)
    {
      if (this.deflateStream == null)
        throw new ObjectDisposedException((string) null, Resources.ObjectDisposed_StreamClosed);
      this.deflateStream.Write(array, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (disposing && this.deflateStream != null)
          this.deflateStream.Dispose();
        this.deflateStream = (DeflateStream) null;
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    public Stream BaseStream
    {
      get => this.deflateStream != null ? this.deflateStream.BaseStream : (Stream) null;
    }
  }
}
