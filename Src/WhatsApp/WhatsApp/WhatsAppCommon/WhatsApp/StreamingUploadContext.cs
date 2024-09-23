// Decompiled with JetBrains decompiler
// Type: WhatsApp.StreamingUploadContext
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace WhatsApp
{
  public class StreamingUploadContext : UploadContext
  {
    protected Message msg;
    private FunXMPP.Connection.UploadResult res;
    protected bool active = true;
    protected bool error;
    protected bool canceled;
    protected object @lock = new object();
    public AxolotlMediaCipher MediaCipher;
    public byte[] MediaKey;
    public string ParticipantHash;
    public IDisposable TransferSubscription;
    private object obsLock = new object();
    private MemoryStream stream = new MemoryStream();
    private IObserver<byte[]> observer;
    private bool eof;
    public long TotalBytes;

    public Message Message
    {
      get => this.msg;
      set
      {
        lock (this.@lock)
        {
          this.msg = value;
          this.CheckComplete();
        }
      }
    }

    public StreamingUploadContext()
      : base(UploadContext.UploadContextType.Streaming)
    {
      this.Hash = StreamingUploadContext.GenerateCustomHash();
    }

    public static byte[] GenerateCustomHash()
    {
      byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}{1}", (object) Settings.MyJid, (object) DateTime.Now.Ticks));
      using (SHA256Managed shA256Managed = new SHA256Managed())
        return shA256Managed.ComputeHash(bytes);
    }

    public byte[] Hash { get; set; }

    public FunXMPP.Connection.UploadResult UploadResult
    {
      get => this.res;
      set
      {
        lock (this.@lock)
        {
          this.res = value;
          this.CheckComplete();
        }
      }
    }

    public bool Active => (this.active || this.eof) && !this.error;

    public void Cancel()
    {
      if (this.canceled)
        return;
      this.canceled = true;
      if (!this.TryCancel() && this.TransferSubscription != null)
      {
        this.TransferSubscription.Dispose();
        this.TransferSubscription = (IDisposable) null;
      }
      this.active = false;
    }

    public virtual void CheckComplete()
    {
      if (this.res == null || this.msg == null)
        return;
      MediaUpload.ProcessUploadResponse(this.msg, this.MediaCipher, this.res, (WhatsApp.Events.MediaUpload) null);
      this.active = false;
    }

    public void OnUploadError(Exception ex)
    {
      Log.LogException(ex, "streaming upload");
      this.active = false;
      this.error = true;
    }

    public IObservable<byte[]> AsObservable()
    {
      return Observable.Create<byte[]>((Func<IObserver<byte[]>, Action>) (observer =>
      {
        lock (this.obsLock)
        {
          if (this.stream.Length != 0L)
          {
            this.stream.Position = 0L;
            observer.OnNext(this.stream.ToArray());
            this.stream.SetLength(0L);
            this.stream.Capacity = 0;
            this.stream = (MemoryStream) null;
          }
          if (this.eof)
            observer.OnCompleted();
          this.observer = observer;
        }
        return (Action) (() =>
        {
          lock (this.obsLock)
          {
            if (this.stream != null)
            {
              this.stream.SetLength(0L);
              this.stream.Capacity = 0;
              this.stream = (MemoryStream) null;
            }
            observer = (IObserver<byte[]>) null;
          }
        });
      }));
    }

    public void OnBytesIn(byte[] b)
    {
      lock (this.obsLock)
      {
        if (this.observer != null)
          this.observer.OnNext(b);
        else if (this.stream != null && !this.eof)
          this.stream.Write(b, 0, b.Length);
        this.TotalBytes += (long) b.Length;
      }
    }

    public void OnEof()
    {
      Action action = (Action) (() => { });
      lock (this.obsLock)
      {
        this.eof = true;
        if (this.observer != null)
          action = new Action(this.observer.OnCompleted);
      }
      action();
    }

    public bool TryCancel()
    {
      bool flag = false;
      lock (this.obsLock)
      {
        if (this.observer != null)
        {
          if (!this.eof)
          {
            this.observer.OnError((Exception) new OperationCanceledException());
            flag = true;
          }
        }
      }
      return flag;
    }
  }
}
