// Decompiled with JetBrains decompiler
// Type: System.Net.Http.StreamToStreamCopy
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
  internal class StreamToStreamCopy
  {
    private byte[] buffer;
    private int bufferSize;
    private Stream source;
    private Stream destination;
    private AsyncCallback bufferReadCallback;
    private AsyncCallback bufferWrittenCallback;
    private TaskCompletionSource<object> tcs;
    private bool sourceIsMemoryStream;
    private bool destinationIsMemoryStream;
    private bool disposeSource;

    public StreamToStreamCopy(
      Stream source,
      Stream destination,
      int bufferSize,
      bool disposeSource)
    {
      Contract.Requires(source != null);
      Contract.Requires(destination != null);
      Contract.Requires(bufferSize > 0);
      this.buffer = new byte[bufferSize];
      this.source = source;
      this.destination = destination;
      this.sourceIsMemoryStream = source is MemoryStream;
      this.destinationIsMemoryStream = destination is MemoryStream;
      this.bufferSize = bufferSize;
      this.bufferReadCallback = new AsyncCallback(this.BufferReadCallback);
      this.bufferWrittenCallback = new AsyncCallback(this.BufferWrittenCallback);
      this.disposeSource = disposeSource;
      this.tcs = new TaskCompletionSource<object>();
    }

    public Task StartAsync()
    {
      if (this.sourceIsMemoryStream && this.destinationIsMemoryStream)
      {
        MemoryStream source = this.source as MemoryStream;
        Contract.Assert(source != null);
        try
        {
          int position = (int) source.Position;
          this.destination.Write(source.ToArray(), position, (int) this.source.Length - position);
          this.SetCompleted((Exception) null);
        }
        catch (Exception ex)
        {
          this.SetCompleted(ex);
        }
      }
      else
        this.StartRead();
      return (Task) this.tcs.Task;
    }

    private void StartRead()
    {
      try
      {
        bool flag;
        do
        {
          if (this.sourceIsMemoryStream)
          {
            int bytesRead = this.source.Read(this.buffer, 0, this.bufferSize);
            if (bytesRead == 0)
            {
              this.SetCompleted((Exception) null);
              break;
            }
            flag = this.TryStartWriteSync(bytesRead);
          }
          else
          {
            IAsyncResult asyncResult = this.source.BeginRead(this.buffer, 0, this.bufferSize, this.bufferReadCallback, (object) null);
            flag = asyncResult.CompletedSynchronously;
            if (flag)
            {
              int bytesRead = this.source.EndRead(asyncResult);
              if (bytesRead == 0)
              {
                this.SetCompleted((Exception) null);
                break;
              }
              flag = this.TryStartWriteSync(bytesRead);
            }
          }
        }
        while (flag);
      }
      catch (Exception ex)
      {
        this.SetCompleted(ex);
      }
    }

    private bool TryStartWriteSync(int bytesRead)
    {
      Contract.Requires(bytesRead > 0);
      if (this.destinationIsMemoryStream)
      {
        this.destination.Write(this.buffer, 0, bytesRead);
        return true;
      }
      IAsyncResult asyncResult = this.destination.BeginWrite(this.buffer, 0, bytesRead, this.bufferWrittenCallback, (object) null);
      if (!asyncResult.CompletedSynchronously)
        return false;
      this.destination.EndWrite(asyncResult);
      return true;
    }

    private void BufferReadCallback(IAsyncResult ar)
    {
      if (ar.CompletedSynchronously)
        return;
      try
      {
        int bytesRead = this.source.EndRead(ar);
        if (bytesRead == 0)
        {
          this.SetCompleted((Exception) null);
        }
        else
        {
          if (!this.TryStartWriteSync(bytesRead))
            return;
          this.StartRead();
        }
      }
      catch (Exception ex)
      {
        this.SetCompleted(ex);
      }
    }

    private void BufferWrittenCallback(IAsyncResult ar)
    {
      if (ar.CompletedSynchronously)
        return;
      try
      {
        this.destination.EndWrite(ar);
        this.StartRead();
      }
      catch (Exception ex)
      {
        this.SetCompleted(ex);
      }
    }

    private void SetCompleted(Exception error)
    {
      try
      {
        if (this.disposeSource)
          this.source.Dispose();
      }
      catch (Exception ex)
      {
        if (Logging.On)
          Logging.Exception(Logging.Http, (object) this, nameof (SetCompleted), ex);
      }
      if (error == null)
        this.tcs.TrySetResult((object) null);
      else
        this.tcs.TrySetException(error);
    }
  }
}
