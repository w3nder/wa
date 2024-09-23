// Decompiled with JetBrains decompiler
// Type: WhatsApp.Streaming.StreamingDownloadFileSource
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Threading;


namespace WhatsApp.Streaming
{
  public class StreamingDownloadFileSource : IStreamingFileSource, IDisposable
  {
    private StreamingDownload parent;
    private long fileSize;
    private long position;
    private Action @throw;
    private Stream streamBuffer;
    private object streamLock = new object();
    private bool deleteOnClose = true;

    public string PlaintextFilename { get; private set; }

    public StreamingDownloadFileSource(StreamingDownload parent, long expectedFileSize)
    {
      this.parent = parent;
      this.fileSize = expectedFileSize;
      this.PlaintextFilename = MediaDownload.GetDirectoryPath() + "\\" + MediaUpload.GenerateMediaFilename("mp4");
      this.streamBuffer = MediaStorage.OpenFile(this.PlaintextFilename, FileMode.CreateNew, FileAccess.ReadWrite);
    }

    public void Dispose()
    {
      this.parent.SafeDispose();
      this.parent = (StreamingDownload) null;
      if (this.streamBuffer == null)
        return;
      lock (this.streamLock)
      {
        this.streamBuffer.SafeDispose();
        this.streamBuffer = (Stream) null;
        if (this.@throw == null)
          this.@throw = (Action) (() =>
          {
            throw new OperationCanceledException();
          });
      }
      if (!this.deleteOnClose)
        return;
      try
      {
        using (IMediaStorage mediaStorage = MediaStorage.Create(this.PlaintextFilename))
          mediaStorage.DeleteFile(this.PlaintextFilename);
      }
      catch (Exception ex)
      {
      }
    }

    public void Write(byte[] buf, int pos, int len)
    {
      lock (this.streamLock)
      {
        this.CheckIoError();
        this.streamBuffer.Write(buf, pos, len);
      }
    }

    public void OnEndOfFile()
    {
      lock (this.streamLock)
      {
        if (this.streamBuffer != null)
        {
          if (this.streamBuffer.Length != this.fileSize)
            throw new IOException("Premature end of file");
        }
      }
      this.deleteOnClose = false;
    }

    public void OnError(Action @throw)
    {
      if (this.@throw != null)
        return;
      this.@throw = @throw;
    }

    public void CheckIoError()
    {
      Action action = this.@throw;
      if (action == null)
        return;
      action();
    }

    private T Sync<T>(Func<T> op)
    {
      lock (this.streamLock)
      {
        this.CheckIoError();
        return op();
      }
    }

    public int Read(byte[] buf, int pos, int len)
    {
      long num1 = Math.Min(this.position + (long) len, this.fileSize);
      while (this.Sync<long>((Func<long>) (() => this.streamBuffer.Length)) < num1)
        Thread.Sleep(100);
      this.CheckIoError();
      lock (this.streamLock)
      {
        this.CheckIoError();
        long position = this.streamBuffer.Position;
        this.streamBuffer.Position = this.position;
        int num2 = this.streamBuffer.Read(buf, pos, len);
        this.position = this.streamBuffer.Position;
        this.streamBuffer.Position = position;
        return num2;
      }
    }

    public void Seek(long pos)
    {
      this.position = pos >= 0L && pos <= this.fileSize ? pos : throw new InvalidOperationException("Invalid seek offset");
    }

    public long GetFullFileSize() => this.fileSize;
  }
}
