// Decompiled with JetBrains decompiler
// Type: System.Net.Http.StreamContent
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http
{
  /// <summary>Provides HTTP content based on a stream.</summary>
  public class StreamContent : HttpContent
  {
    private const int defaultBufferSize = 4096;
    private Stream content;
    private int bufferSize;
    private bool contentConsumed;
    private long start;

    public StreamContent(Stream content)
      : this(content, 4096)
    {
    }

    public StreamContent(Stream content, int bufferSize)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (bufferSize <= 0)
        throw new ArgumentOutOfRangeException(nameof (bufferSize));
      this.content = content;
      this.bufferSize = bufferSize;
      if (content.CanSeek)
        this.start = content.Position;
      if (!Logging.On)
        return;
      Logging.Associate(Logging.Http, (object) this, (object) content);
    }

    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.</returns>
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
      Contract.Assert(stream != null);
      this.PrepareContent();
      return new StreamToStreamCopy(this.content, stream, this.bufferSize, !this.content.CanSeek).StartAsync();
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    protected internal override bool TryComputeLength(out long length)
    {
      if (this.content.CanSeek)
      {
        length = this.content.Length - this.start;
        return true;
      }
      length = 0L;
      return false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.content.Dispose();
      base.Dispose(disposing);
    }

    protected override Task<Stream> CreateContentReadStreamAsync()
    {
      TaskCompletionSource<Stream> completionSource = new TaskCompletionSource<Stream>();
      completionSource.TrySetResult((Stream) new StreamContent.ReadOnlyStream(this.content));
      return completionSource.Task;
    }

    private void PrepareContent()
    {
      if (this.contentConsumed)
      {
        if (!this.content.CanSeek)
          throw new InvalidOperationException(SR.net_http_content_stream_already_read);
        this.content.Position = this.start;
      }
      this.contentConsumed = true;
    }

    private class ReadOnlyStream : DelegatingStream
    {
      public override bool CanWrite => false;

      public override int WriteTimeout
      {
        get => throw new NotSupportedException(SR.net_http_content_readonly_stream);
        set => throw new NotSupportedException(SR.net_http_content_readonly_stream);
      }

      public ReadOnlyStream(Stream innerStream)
        : base(innerStream)
      {
      }

      public override void Flush()
      {
        throw new NotSupportedException(SR.net_http_content_readonly_stream);
      }

      public override void SetLength(long value)
      {
        throw new NotSupportedException(SR.net_http_content_readonly_stream);
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        throw new NotSupportedException(SR.net_http_content_readonly_stream);
      }

      public override IAsyncResult BeginWrite(
        byte[] buffer,
        int offset,
        int count,
        AsyncCallback callback,
        object state)
      {
        throw new NotSupportedException(SR.net_http_content_readonly_stream);
      }

      public override void EndWrite(IAsyncResult asyncResult)
      {
        throw new NotSupportedException(SR.net_http_content_readonly_stream);
      }

      public override void WriteByte(byte value)
      {
        throw new NotSupportedException(SR.net_http_content_readonly_stream);
      }
    }
  }
}
