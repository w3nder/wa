// Decompiled with JetBrains decompiler
// Type: System.IO.Compression.DeflateStream
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Threading;

#nullable disable
namespace System.IO.Compression
{
  internal class DeflateStream : Stream
  {
    internal const int DefaultBufferSize = 8192;
    private Stream _stream;
    private CompressionMode _mode;
    private bool _leaveOpen;
    private Inflater inflater;
    private byte[] buffer;
    private int asyncOperations;
    private readonly AsyncCallback m_CallBack;

    public DeflateStream(Stream stream, CompressionMode mode)
      : this(stream, mode, false)
    {
    }

    public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      if (CompressionMode.Compress != mode && mode != CompressionMode.Decompress)
        throw new ArgumentException(Resources.ArgumentOutOfRange_Enum, nameof (mode));
      this._stream = stream;
      this._mode = mode;
      this._leaveOpen = leaveOpen;
      switch (this._mode)
      {
        case CompressionMode.Decompress:
          if (!this._stream.CanRead)
            throw new ArgumentException(Resources.NotReadableStream, nameof (stream));
          this.inflater = new Inflater();
          this.m_CallBack = new AsyncCallback(this.ReadCallback);
          break;
        case CompressionMode.Compress:
          throw new NotSupportedException(Resources.NotSupported);
      }
      this.buffer = new byte[8192];
    }

    internal void SetFileFormatReader(IFileFormatReader reader)
    {
      if (reader == null)
        return;
      this.inflater.SetFileFormatReader(reader);
    }

    public Stream BaseStream => this._stream;

    public override bool CanRead
    {
      get
      {
        return this._stream != null && this._mode == CompressionMode.Decompress && this._stream.CanRead;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return this._stream != null && this._mode == CompressionMode.Compress && this._stream.CanWrite;
      }
    }

    public override bool CanSeek => false;

    public override long Length => throw new NotSupportedException(Resources.NotSupported);

    public override long Position
    {
      get => throw new NotSupportedException(Resources.NotSupported);
      set => throw new NotSupportedException(Resources.NotSupported);
    }

    public override void Flush() => this.EnsureNotDisposed();

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException(Resources.NotSupported);
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException(Resources.NotSupported);
    }

    public override int Read(byte[] array, int offset, int count)
    {
      this.EnsureDecompressionMode();
      this.ValidateParameters(array, offset, count);
      this.EnsureNotDisposed();
      int offset1 = offset;
      int length1 = count;
      while (true)
      {
        int num = this.inflater.Inflate(array, offset1, length1);
        offset1 += num;
        length1 -= num;
        if (length1 != 0 && !this.inflater.Finished())
        {
          int length2 = this._stream.Read(this.buffer, 0, this.buffer.Length);
          if (length2 != 0)
            this.inflater.SetInput(this.buffer, 0, length2);
          else
            break;
        }
        else
          break;
      }
      return count - length1;
    }

    private void ValidateParameters(byte[] array, int offset, int count)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (offset < 0)
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (array.Length - offset < count)
        throw new ArgumentException(Resources.InvalidArgumentOffsetCount);
    }

    private void EnsureNotDisposed()
    {
      if (this._stream == null)
        throw new ObjectDisposedException((string) null, Resources.ObjectDisposed_StreamClosed);
    }

    private void EnsureDecompressionMode()
    {
      if (this._mode != CompressionMode.Decompress)
        throw new InvalidOperationException(Resources.CannotReadFromDeflateStream);
    }

    private void EnsureCompressionMode()
    {
      if (this._mode != CompressionMode.Compress)
        throw new InvalidOperationException(Resources.CannotWriteToDeflateStream);
    }

    public override IAsyncResult BeginRead(
      byte[] array,
      int offset,
      int count,
      AsyncCallback asyncCallback,
      object asyncState)
    {
      this.EnsureDecompressionMode();
      if (this.asyncOperations != 0)
        throw new InvalidOperationException(Resources.InvalidBeginCall);
      this.ValidateParameters(array, offset, count);
      this.EnsureNotDisposed();
      Interlocked.Increment(ref this.asyncOperations);
      try
      {
        DeflateStreamAsyncResult state = new DeflateStreamAsyncResult((object) this, asyncState, asyncCallback, array, offset, count);
        state.isWrite = false;
        int result = this.inflater.Inflate(array, offset, count);
        if (result != 0)
        {
          state.InvokeCallback(true, (object) result);
          return (IAsyncResult) state;
        }
        if (this.inflater.Finished())
        {
          state.InvokeCallback(true, (object) 0);
          return (IAsyncResult) state;
        }
        this._stream.BeginRead(this.buffer, 0, this.buffer.Length, this.m_CallBack, (object) state);
        state.m_CompletedSynchronously &= state.IsCompleted;
        return (IAsyncResult) state;
      }
      catch
      {
        Interlocked.Decrement(ref this.asyncOperations);
        throw;
      }
    }

    private void ReadCallback(IAsyncResult baseStreamResult)
    {
      DeflateStreamAsyncResult asyncState = (DeflateStreamAsyncResult) baseStreamResult.AsyncState;
      asyncState.m_CompletedSynchronously &= baseStreamResult.CompletedSynchronously;
      try
      {
        this.EnsureNotDisposed();
        int length = this._stream.EndRead(baseStreamResult);
        if (length <= 0)
        {
          asyncState.InvokeCallback((object) 0);
        }
        else
        {
          this.inflater.SetInput(this.buffer, 0, length);
          int result = this.inflater.Inflate(asyncState.buffer, asyncState.offset, asyncState.count);
          if (result == 0 && !this.inflater.Finished())
            this._stream.BeginRead(this.buffer, 0, this.buffer.Length, this.m_CallBack, (object) asyncState);
          else
            asyncState.InvokeCallback((object) result);
        }
      }
      catch (Exception ex)
      {
        asyncState.InvokeCallback((object) ex);
      }
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      this.EnsureDecompressionMode();
      this.CheckEndXxxxLegalStateAndParams(asyncResult);
      DeflateStreamAsyncResult asyncResult1 = (DeflateStreamAsyncResult) asyncResult;
      this.AwaitAsyncResultCompletion(asyncResult1);
      if (asyncResult1.Result is Exception result)
        throw result;
      return (int) asyncResult1.Result;
    }

    public override void Write(byte[] array, int offset, int count)
    {
      throw new NotSupportedException(Resources.NotSupported);
    }

    private void PurgeBuffers(bool disposing)
    {
      if (!disposing || this._stream == null)
        return;
      this.Flush();
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        this.PurgeBuffers(disposing);
      }
      finally
      {
        try
        {
          if (disposing)
          {
            if (!this._leaveOpen)
            {
              if (this._stream != null)
                this._stream.Dispose();
            }
          }
        }
        finally
        {
          this._stream = (Stream) null;
          base.Dispose(disposing);
        }
      }
    }

    private void CheckEndXxxxLegalStateAndParams(IAsyncResult asyncResult)
    {
      if (this.asyncOperations != 1)
        throw new InvalidOperationException(Resources.InvalidEndCall);
      if (asyncResult == null)
        throw new ArgumentNullException(nameof (asyncResult));
      this.EnsureNotDisposed();
      if (!(asyncResult is DeflateStreamAsyncResult))
        throw new ArgumentNullException(nameof (asyncResult));
    }

    private void AwaitAsyncResultCompletion(DeflateStreamAsyncResult asyncResult)
    {
      try
      {
        if (asyncResult.IsCompleted)
          return;
        asyncResult.AsyncWaitHandle.WaitOne();
      }
      finally
      {
        Interlocked.Decrement(ref this.asyncOperations);
        asyncResult.Close();
      }
    }
  }
}
