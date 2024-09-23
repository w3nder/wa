// Decompiled with JetBrains decompiler
// Type: System.Net.Http.ByteArrayContent
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
  /// <summary>Provides HTTP content based on a byte array.</summary>
  public class ByteArrayContent : HttpContent
  {
    private byte[] content;
    private int offset;
    private int count;

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.ByteArrayContent" /> class.</summary>
    /// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> parameter is null. </exception>
    public ByteArrayContent(byte[] content)
    {
      this.content = content != null ? content : throw new ArgumentNullException(nameof (content));
      this.offset = 0;
      this.count = content.Length;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.ByteArrayContent" /> class.</summary>
    /// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
    /// <param name="offset">The offset, in bytes, in the <paramref name="content" />  parameter used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
    /// <param name="count">The number of bytes in the <paramref name="content" /> starting from the <paramref name="offset" /> parameter used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> parameter is null. </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="offset" /> parameter is less than zero.-or-The <paramref name="offset" /> parameter is greater than the length of content specified by the <paramref name="content" /> parameter.-or-The <paramref name="count " /> parameter is less than zero.-or-The <paramref name="count" /> parameter is greater than the length of content specified by the <paramref name="content" /> parameter - minus the <paramref name="offset" /> parameter.</exception>
    public ByteArrayContent(byte[] content, int offset, int count)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (offset < 0 || offset > content.Length)
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (count < 0 || count > content.Length - offset)
        throw new ArgumentOutOfRangeException(nameof (count));
      this.content = content;
      this.offset = offset;
      this.count = count;
    }

    /// <summary>Serialize and write the byte array provided in the constructor to an HTTP content stream as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />. The task object representing the asynchronous operation.</returns>
    /// <param name="stream">The target stream.</param>
    /// <param name="context">Information about the transport, like channel binding token. This parameter may be null.</param>
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
      Contract.Assert(stream != null);
      return Task.Factory.FromAsync<byte[], int, int>(new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(stream.BeginWrite), new Action<IAsyncResult>(stream.EndWrite), this.content, this.offset, this.count, (object) null);
    }

    /// <summary>Determines whether a byte array has a valid length in bytes.</summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="length" /> is a valid length; otherwise, false.</returns>
    /// <param name="length">The length in bytes of the byte array.</param>
    protected internal override bool TryComputeLength(out long length)
    {
      length = (long) this.count;
      return true;
    }

    protected override Task<Stream> CreateContentReadStreamAsync()
    {
      TaskCompletionSource<Stream> completionSource = new TaskCompletionSource<Stream>();
      completionSource.TrySetResult((Stream) new MemoryStream(this.content, this.offset, this.count, false));
      return completionSource.Task;
    }
  }
}
