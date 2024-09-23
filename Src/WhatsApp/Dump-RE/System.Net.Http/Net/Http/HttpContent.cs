// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpContent
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http
{
  /// <summary>A base class representing an HTTP entity body and content headers.</summary>
  public abstract class HttpContent : IDisposable
  {
    internal const long MaxBufferSize = 2147483647;
    private HttpContentHeaders headers;
    private MemoryStream bufferedContent;
    private bool disposed;
    private Stream contentReadStream;
    private bool canCalculateLength;
    internal static readonly Encoding DefaultStringEncoding = Encoding.UTF8;
    private static Encoding[] EncodingsWithBom = new Encoding[3]
    {
      Encoding.UTF8,
      Encoding.Unicode,
      Encoding.BigEndianUnicode
    };

    /// <summary>Gets the HTTP content headers as defined in RFC 2616.</summary>
    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpContentHeaders" />.The content headers as defined in RFC 2616.</returns>
    public HttpContentHeaders Headers
    {
      get
      {
        if (this.headers == null)
          this.headers = new HttpContentHeaders(new Func<long?>(this.GetComputedOrBufferLength));
        return this.headers;
      }
    }

    private bool IsBuffered => this.bufferedContent != null;

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpContent" /> class.</summary>
    protected HttpContent()
    {
      if (Logging.On)
        Logging.Enter(Logging.Http, (object) this, ".ctor", (object) null);
      this.canCalculateLength = true;
      if (!Logging.On)
        return;
      Logging.Exit(Logging.Http, (object) this, ".ctor", (object) null);
    }

    public Task<string> ReadAsStringAsync()
    {
      this.CheckDisposed();
      TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
      this.LoadIntoBufferAsync().ContinueWithStandard((Action<Task>) (task =>
      {
        if (HttpUtilities.HandleFaultsAndCancelation<string>(task, tcs))
          return;
        if (this.bufferedContent.Length == 0L)
        {
          tcs.TrySetResult(string.Empty);
        }
        else
        {
          Encoding encoding1 = (Encoding) null;
          int index = -1;
          byte[] buffer = this.bufferedContent.GetBuffer();
          int length = (int) this.bufferedContent.Length;
          if (this.Headers.ContentType != null)
          {
            if (this.Headers.ContentType.CharSet != null)
            {
              try
              {
                encoding1 = Encoding.GetEncoding(this.Headers.ContentType.CharSet);
              }
              catch (ArgumentException ex)
              {
                tcs.TrySetException((Exception) new InvalidOperationException(SR.net_http_content_invalid_charset, (Exception) ex));
                return;
              }
            }
          }
          if (encoding1 == null)
          {
            foreach (Encoding encoding2 in HttpContent.EncodingsWithBom)
            {
              byte[] preamble = encoding2.GetPreamble();
              if (HttpContent.ByteArrayHasPrefix(buffer, length, preamble))
              {
                encoding1 = encoding2;
                index = preamble.Length;
                break;
              }
            }
          }
          Encoding encoding3 = encoding1 ?? HttpContent.DefaultStringEncoding;
          if (index == -1)
          {
            byte[] preamble = encoding3.GetPreamble();
            index = !HttpContent.ByteArrayHasPrefix(buffer, length, preamble) ? 0 : preamble.Length;
          }
          try
          {
            tcs.TrySetResult(encoding3.GetString(buffer, index, length - index));
          }
          catch (Exception ex)
          {
            tcs.TrySetException(ex);
          }
        }
      }));
      return tcs.Task;
    }

    public Task<byte[]> ReadAsByteArrayAsync()
    {
      this.CheckDisposed();
      TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();
      this.LoadIntoBufferAsync().ContinueWithStandard((Action<Task>) (task =>
      {
        if (HttpUtilities.HandleFaultsAndCancelation<byte[]>(task, tcs))
          return;
        tcs.TrySetResult(this.bufferedContent.ToArray());
      }));
      return tcs.Task;
    }

    public Task<Stream> ReadAsStreamAsync()
    {
      this.CheckDisposed();
      TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
      if (this.contentReadStream == null && this.IsBuffered)
        this.contentReadStream = (Stream) new MemoryStream(this.bufferedContent.GetBuffer(), 0, (int) this.bufferedContent.Length, false);
      if (this.contentReadStream != null)
      {
        tcs.TrySetResult(this.contentReadStream);
        return tcs.Task;
      }
      this.CreateContentReadStreamAsync().ContinueWithStandard<Stream>((Action<Task<Stream>>) (task =>
      {
        if (HttpUtilities.HandleFaultsAndCancelation<Stream>((Task) task, tcs))
          return;
        this.contentReadStream = task.Result;
        tcs.TrySetResult(this.contentReadStream);
      }));
      return tcs.Task;
    }

    /// <summary>Serialize the HTTP content to a stream as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.</returns>
    /// <param name="stream">The target stream.</param>
    /// <param name="context">Information about the transport (channel binding token, for example). This parameter may be null.</param>
    protected abstract Task SerializeToStreamAsync(Stream stream, TransportContext context);

    /// <summary>Write the HTTP content to a stream as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.</returns>
    /// <param name="stream">The target stream.</param>
    /// <param name="context">Information about the transport (channel binding token, for example). This parameter may be null.</param>
    public Task CopyToAsync(Stream stream, TransportContext context)
    {
      this.CheckDisposed();
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
      try
      {
        Task task;
        if (this.IsBuffered)
        {
          task = Task.Factory.FromAsync<byte[], int, int>(new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(stream.BeginWrite), new Action<IAsyncResult>(stream.EndWrite), this.bufferedContent.GetBuffer(), 0, (int) this.bufferedContent.Length, (object) null);
        }
        else
        {
          task = this.SerializeToStreamAsync(stream, context);
          this.CheckTaskNotNull(task);
        }
        task.ContinueWithStandard((Action<Task>) (copyTask =>
        {
          if (copyTask.IsFaulted)
            tcs.TrySetException(HttpContent.GetStreamCopyException(((Exception) copyTask.Exception).GetBaseException()));
          else if (copyTask.IsCanceled)
            tcs.TrySetCanceled();
          else
            tcs.TrySetResult((object) null);
        }));
      }
      catch (IOException ex)
      {
        tcs.TrySetException(HttpContent.GetStreamCopyException((Exception) ex));
      }
      catch (ObjectDisposedException ex)
      {
        tcs.TrySetException(HttpContent.GetStreamCopyException((Exception) ex));
      }
      return (Task) tcs.Task;
    }

    /// <summary>Write the HTTP content to a stream as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.</returns>
    /// <param name="stream">The target stream.</param>
    public Task CopyToAsync(Stream stream) => this.CopyToAsync(stream, (TransportContext) null);

    /// <summary>Write the HTTP content to a stream.</summary>
    /// <param name="stream">The target stream.</param>
    internal void CopyTo(Stream stream) => this.CopyToAsync(stream).Wait();

    /// <summary>Serialize the HTTP content to a memory buffer as an asynchronous operation.</summary>
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.</returns>
    public Task LoadIntoBufferAsync() => this.LoadIntoBufferAsync((long) int.MaxValue);

    public Task LoadIntoBufferAsync(long maxBufferSize)
    {
      this.CheckDisposed();
      if (maxBufferSize > (long) int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (maxBufferSize), (object) maxBufferSize, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit, (object) (long) int.MaxValue));
      if (this.IsBuffered)
        return HttpContent.CreateCompletedTask();
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
      Exception error = (Exception) null;
      MemoryStream tempBuffer = this.CreateMemoryStream(maxBufferSize, out error);
      if (tempBuffer == null)
      {
        Contract.Assert(error != null);
        tcs.TrySetException(error);
      }
      else
      {
        try
        {
          Task streamAsync = this.SerializeToStreamAsync((Stream) tempBuffer, (TransportContext) null);
          this.CheckTaskNotNull(streamAsync);
          streamAsync.ContinueWithStandard((Action<Task>) (copyTask =>
          {
            try
            {
              if (copyTask.IsFaulted)
              {
                tempBuffer.Dispose();
                tcs.TrySetException(HttpContent.GetStreamCopyException(((Exception) copyTask.Exception).GetBaseException()));
              }
              else if (copyTask.IsCanceled)
              {
                tempBuffer.Dispose();
                tcs.TrySetCanceled();
              }
              else
              {
                tempBuffer.Seek(0L, SeekOrigin.Begin);
                this.bufferedContent = tempBuffer;
                tcs.TrySetResult((object) null);
              }
            }
            catch (Exception ex)
            {
              tcs.TrySetException(ex);
              if (!Logging.On)
                return;
              Logging.Exception(Logging.Http, (object) this, nameof (LoadIntoBufferAsync), ex);
            }
          }));
        }
        catch (IOException ex)
        {
          tcs.TrySetException(HttpContent.GetStreamCopyException((Exception) ex));
        }
        catch (ObjectDisposedException ex)
        {
          tcs.TrySetException(HttpContent.GetStreamCopyException((Exception) ex));
        }
      }
      return (Task) tcs.Task;
    }

    protected virtual Task<Stream> CreateContentReadStreamAsync()
    {
      TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
      this.LoadIntoBufferAsync().ContinueWithStandard((Action<Task>) (task =>
      {
        if (HttpUtilities.HandleFaultsAndCancelation<Stream>(task, tcs))
          return;
        tcs.TrySetResult((Stream) this.bufferedContent);
      }));
      return tcs.Task;
    }

    /// <summary>Determines whether the HTTP content has a valid length in bytes.</summary>
    /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="length" /> is a valid length; otherwise, false.</returns>
    /// <param name="length">The length in bytes of the HHTP content.</param>
    protected internal abstract bool TryComputeLength(out long length);

    private long? GetComputedOrBufferLength()
    {
      this.CheckDisposed();
      if (this.IsBuffered)
        return new long?(this.bufferedContent.Length);
      if (this.canCalculateLength)
      {
        long length = 0;
        if (this.TryComputeLength(out length))
          return new long?(length);
        this.canCalculateLength = false;
      }
      return new long?();
    }

    private MemoryStream CreateMemoryStream(long maxBufferSize, out Exception error)
    {
      Contract.Ensures(Contract.Result<MemoryStream>() != null || Contract.ValueAtReturn<Exception>(out error) != null);
      error = (Exception) null;
      long? contentLength = this.Headers.ContentLength;
      if (!contentLength.HasValue)
        return (MemoryStream) new HttpContent.LimitMemoryStream((int) maxBufferSize, 0);
      long? nullable1 = contentLength;
      Contract.Assert(nullable1.GetValueOrDefault() >= 0L && nullable1.HasValue);
      long? nullable2 = contentLength;
      long num = maxBufferSize;
      if ((nullable2.GetValueOrDefault() <= num ? 0 : (nullable2.HasValue ? 1 : 0)) == 0)
        return (MemoryStream) new HttpContent.LimitMemoryStream((int) maxBufferSize, (int) contentLength.Value);
      error = (Exception) new HttpRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_buffersize_exceeded, (object) maxBufferSize));
      return (MemoryStream) null;
    }

    /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpContent" /> and optionally disposes of the managed resources.</summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.disposed)
        return;
      this.disposed = true;
      if (this.contentReadStream != null)
        this.contentReadStream.Dispose();
      if (!this.IsBuffered)
        return;
      this.bufferedContent.Dispose();
    }

    /// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:System.Net.Http.HttpContent" />.</summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void CheckDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    private void CheckTaskNotNull(Task task)
    {
      if (task == null)
      {
        if (Logging.On)
          Logging.PrintError(Logging.Http, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_log_content_no_task_returned_copytoasync, (object) this.GetType().FullName));
        throw new InvalidOperationException(SR.net_http_content_no_task_returned);
      }
    }

    private static Task CreateCompletedTask()
    {
      TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
      Contract.Assert(completionSource.TrySetResult((object) null), "Can't set Task as completed.");
      return (Task) completionSource.Task;
    }

    private static Exception GetStreamCopyException(Exception originalException)
    {
      Exception inner = originalException;
      switch (inner)
      {
        case IOException _:
        case ObjectDisposedException _:
          inner = (Exception) new HttpRequestException(SR.net_http_content_stream_copy_error, inner);
          break;
      }
      return inner;
    }

    private static bool ByteArrayHasPrefix(byte[] byteArray, int dataLength, byte[] prefix)
    {
      if (prefix == null || byteArray == null || prefix.Length > dataLength || prefix.Length == 0)
        return false;
      for (int index = 0; index < prefix.Length; ++index)
      {
        if ((int) prefix[index] != (int) byteArray[index])
          return false;
      }
      return true;
    }

    private class LimitMemoryStream : MemoryStream
    {
      private int maxSize;

      public LimitMemoryStream(int maxSize, int capacity)
        : base(capacity)
      {
        this.maxSize = maxSize;
      }

      public override IAsyncResult BeginWrite(
        byte[] buffer,
        int offset,
        int count,
        AsyncCallback callback,
        object state)
      {
        this.CheckSize(count);
        return base.BeginWrite(buffer, offset, count, callback, state);
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        this.CheckSize(count);
        base.Write(buffer, offset, count);
      }

      public override void WriteByte(byte value)
      {
        this.CheckSize(1);
        base.WriteByte(value);
      }

      private void CheckSize(int countToAdd)
      {
        if ((long) this.maxSize - this.Length < (long) countToAdd)
          throw new HttpRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_buffersize_exceeded, (object) this.maxSize));
      }
    }
  }
}
