// Decompiled with JetBrains decompiler
// Type: WhatsApp.MultiPartUploader
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using WhatsAppNative;


namespace WhatsApp
{
  public static class MultiPartUploader
  {
    private static TicketCounter tick = new TicketCounter();
    private static byte[] crlf = Encoding.UTF8.GetBytes("\r\n");

    private static string GenerateBoundary()
    {
      return string.Format("{0}{1}", (object) DateTime.Now.Ticks, (object) MultiPartUploader.tick.NextTicket());
    }

    internal static bool TryWriteMultipart(
      string boundary,
      StringBuilder sb,
      IEnumerable<MultiPartUploader.FormData> formData,
      Action<byte[], int, int, int> write)
    {
      byte[] bytes1 = Encoding.UTF8.GetBytes("\r\n");
      bool flag = false;
      MultiPartUploader.FormData formData1 = (MultiPartUploader.FormData) null;
      foreach (MultiPartUploader.FormData formData2 in formData)
      {
        MultiPartUploader.FormData formData3 = formData2;
        if (flag)
        {
          if (formData1 != null)
          {
            formData3 = formData1;
            formData1 = (MultiPartUploader.FormData) null;
            Log.WriteLineDebug("WriteMultipart: {0} replaced by {1}", (object) formData2.Name, (object) formData3.Name);
          }
          else
          {
            Log.WriteLineDebug("WriteMultipart: {0} bypassed after cancel", (object) formData2.Name);
            continue;
          }
        }
        sb.Length = 0;
        sb.Append("--");
        sb.Append(boundary);
        sb.Append("\r\n");
        sb.Append("Content-Disposition: ");
        sb.Append("form-data");
        sb.Append("; name=\"");
        sb.Append(formData3.Name);
        sb.Append('"');
        if (formData3.Filename != null)
        {
          sb.Append("; filename=\"");
          sb.Append(formData3.Filename);
          sb.Append('"');
        }
        sb.Append("\r\n");
        if (formData3.ContentType != null)
        {
          sb.Append("Content-Type: ");
          sb.Append(formData3.ContentType);
          sb.Append("\r\n");
        }
        if (formData3.ContentRange != null)
        {
          sb.Append("Content-Range: ");
          sb.Append(formData3.ContentRange);
          sb.Append("\r\n");
        }
        sb.Append("\r\n");
        byte[] bytes2 = Encoding.UTF8.GetBytes(sb.ToString());
        write(bytes2, 0, bytes2.Length, 0);
        formData3.Write(write);
        write(bytes1, 0, bytes1.Length, 0);
        if (formData3 is MultiPartUploader.FormDataCancellableAsync cancellableAsync)
        {
          flag = cancellableAsync.canceled;
          if (flag)
          {
            Log.WriteLineDebug("WriteMultipart: detected cancel");
            formData1 = cancellableAsync.CancelFormData;
          }
        }
      }
      byte[] bytes3 = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");
      write(bytes3, 0, bytes3.Length, 0);
      return flag;
    }

    public static IObservable<MultiPartUploader.Args> Open(
      string url,
      MultiPartUploader.FormData[] formData,
      bool chunked = false,
      bool progress = false)
    {
      return NativeWeb.Create<MultiPartUploader.Args>((Action<IWebRequest, IObserver<MultiPartUploader.Args>>) ((req, observer) =>
      {
        string boundary = MultiPartUploader.GenerateBoundary();
        MultiPartUploader.NativeWebCallback callbackObject = new MultiPartUploader.NativeWebCallback(observer, progress, chunked, formData, boundary);
        StringBuilder stringBuilder = new StringBuilder();
        if (chunked)
        {
          stringBuilder.Append("Transfer-Encoding: chunked\r\n");
        }
        else
        {
          bool showProgress = callbackObject.showProgress;
          callbackObject.showProgress = false;
          MultiPartUploader.ContentLengthCounter writer = new MultiPartUploader.ContentLengthCounter();
          callbackObject.Write((IWebWriter) writer);
          callbackObject.showProgress = showProgress;
          stringBuilder.AppendFormat("Content-Length: {0}\r\n", (object) writer.ContentLength);
        }
        stringBuilder.AppendFormat("Content-Type: multipart/form-data; boundary={0}\r\n", (object) boundary);
        try
        {
          req.Open(url, (IWebCallback) callbackObject, "POST", headers: stringBuilder.ToString());
        }
        finally
        {
          long bytesWritten = callbackObject.BytesWritten;
          if (bytesWritten != 0L)
            Log.WriteLineDebug("HTTP POST: wrote {0} bytes of post data", (object) bytesWritten);
        }
      }));
    }

    public class Args
    {
      public long CurrentProgress;
      public long TotalProgress;
      public int ResponseCode;
      public Stream Result;
      public long ConnectTimeMs;
      public long NetworkTimeMs;
    }

    public class FormData
    {
      public string Name;
      public string ContentType;
      public string Filename;
      public string ContentRange;

      public virtual void Write(Action<byte[], int, int, int> write)
      {
      }

      public virtual long Length => 0;
    }

    public class FormDataString : MultiPartUploader.FormData
    {
      public string Content;
      public Func<string> ContentGenerator;

      public override void Write(Action<byte[], int, int, int> write)
      {
        if (this.Content == null && this.ContentGenerator != null)
          this.Content = this.ContentGenerator();
        byte[] bytes = Encoding.UTF8.GetBytes(this.Content);
        write(bytes, 0, bytes.Length, bytes.Length);
      }

      public override long Length => this.Content != null ? (long) this.Content.Length : 0L;
    }

    public static class FormDataCryptoWrapper
    {
      public static void Create(
        AxolotlMediaCipher mediaCipher,
        ref Action<byte[], int, int, int> write,
        out Action<Stream, long> seek,
        out Action flush)
      {
        if (mediaCipher == null)
        {
          seek = (Action<Stream, long>) ((s, p) => s.Position = p);
          flush = (Action) null;
        }
        else
        {
          mediaCipher.EnsureCrypto();
          int blockSize = mediaCipher.InputBlockSize;
          long seekedBytes = 0;
          Action<byte[], int, int, int> innerWrite = write;
          byte[] bufOut = new byte[8192];
          byte[] bufferedPlaintext = (byte[]) null;
          int pendingProgLen = 0;
          seek = (Action<Stream, long>) ((s, pos) => seekedBytes = pos);
          write = (Action<byte[], int, int, int>) ((buf, offset, len, progLen) =>
          {
            Action<byte[], int, int> action = (Action<byte[], int, int>) ((toWriteBuf, toWriteOffset, toWriteLen) =>
            {
              int num = (int) Math.Min(seekedBytes, (long) toWriteLen);
              seekedBytes -= (long) num;
              toWriteOffset += num;
              toWriteLen -= num;
              if (toWriteLen == 0)
                return;
              innerWrite(toWriteBuf, toWriteOffset, toWriteLen, pendingProgLen + progLen);
              pendingProgLen = progLen = 0;
            });
            if (bufferedPlaintext != null)
            {
              byte[] destinationArray = new byte[bufferedPlaintext.Length + len];
              Array.Copy((Array) bufferedPlaintext, (Array) destinationArray, bufferedPlaintext.Length);
              Array.Copy((Array) buf, offset, (Array) destinationArray, bufferedPlaintext.Length, len);
              buf = destinationArray;
              offset = 0;
              len = buf.Length;
              bufferedPlaintext = (byte[]) null;
            }
            byte[] numArray = new byte[blockSize];
            while (len >= blockSize)
            {
              int length = blockSize;
              Array.Copy((Array) buf, offset, (Array) numArray, 0, length);
              int num = mediaCipher.EncryptMedia(numArray, bufOut);
              offset += length;
              len -= length;
              if (num != 0)
                action(bufOut, 0, num);
            }
            if (len == 0)
              return;
            pendingProgLen += progLen;
            bufferedPlaintext = new byte[len];
            Array.Copy((Array) buf, offset, (Array) bufferedPlaintext, 0, len);
          });
          flush = (Action) (() =>
          {
            byte[] plaintextBlock = bufferedPlaintext ?? new byte[0];
            byte[] numArray = mediaCipher.EncryptMediaFinal(plaintextBlock, plaintextBlock.Length);
            seekedBytes = Math.Min((long) numArray.Length, seekedBytes);
            innerWrite(numArray, (int) seekedBytes, (int) ((long) numArray.Length - seekedBytes), pendingProgLen);
          });
        }
      }

      public static void Create(
        AxolotlMediaCipher mediaCipher,
        ref Action<byte[], int, int, int> write,
        out Action flush)
      {
        MultiPartUploader.FormDataCryptoWrapper.Create(mediaCipher, ref write, out Action<Stream, long> _, out flush);
      }
    }

    public class FormDataFile : MultiPartUploader.FormData
    {
      public Stream Content;
      public long Offset;
      public long? OverrideLength;
      public AxolotlMediaCipher MediaCipher;

      public override void Write(Action<byte[], int, int, int> write)
      {
        long num1 = this.OverrideLength ?? this.Content.Length;
        byte[] buffer = new byte[4096];
        Action<Stream, long> seek;
        Action flush;
        MultiPartUploader.FormDataCryptoWrapper.Create(this.MediaCipher, ref write, out seek, out flush);
        seek(this.Content, this.Offset);
        long val1 = num1 - this.Content.Position;
        while (val1 != 0L)
        {
          int num2 = this.Content.Read(buffer, 0, (int) Math.Min(val1, (long) buffer.Length));
          if (num2 <= 0)
            throw new IOException("Unexpected value " + (object) num2);
          val1 -= (long) num2;
          write(buffer, 0, num2, num2);
        }
        if (flush == null)
          return;
        flush();
      }

      public override long Length => this.OverrideLength ?? this.Content.Length;
    }

    public class FormDataCancellableAsync : MultiPartUploader.FormData
    {
      private IObservable<byte[]> obs;
      private Action terminate = (Action) (() => { });
      public AxolotlMediaCipher MediaCipher;
      public bool canceled;

      public MultiPartUploader.FormData CancelFormData
      {
        get
        {
          MultiPartUploader.FormDataString cancelFormData = new MultiPartUploader.FormDataString();
          cancelFormData.Name = "cancel";
          cancelFormData.Content = "true";
          return (MultiPartUploader.FormData) cancelFormData;
        }
      }

      public FormDataCancellableAsync(IObservable<byte[]> obs) => this.obs = obs;

      public override void Write(Action<byte[], int, int, int> write)
      {
        ManualResetEvent ev = new ManualResetEvent(false);
        Action error = (Action) null;
        IDisposable disp = (IDisposable) null;
        object @lock = new object();
        Action setUnlocked = (Action) (() =>
        {
          if (disp != null)
          {
            disp.Dispose();
            disp = (IDisposable) null;
          }
          ev?.Set();
        });
        Action set = (Action) (() =>
        {
          lock (@lock)
            setUnlocked();
        });
        Action flush;
        MultiPartUploader.FormDataCryptoWrapper.Create(this.MediaCipher, ref write, out flush);
        WorkQueue workQueue = new WorkQueue();
        disp = this.obs.ObserveOn<byte[]>((IScheduler) workQueue).Subscribe<byte[]>((Action<byte[]>) (payload =>
        {
          try
          {
            write(payload, 0, payload.Length, 0);
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "write async");
            error = ex.GetRethrowAction();
            set();
          }
        }), (Action<Exception>) (ex =>
        {
          Log.LogException(ex, "write async - observable OnError");
          if (ex is OperationCanceledException)
            this.canceled = true;
          else
            error = ex.GetRethrowAction();
          set();
        }), set);
        bool terminated = false;
        this.terminate = (Action) (() =>
        {
          lock (@lock)
          {
            terminated = true;
            setUnlocked();
          }
        });
        ev.WaitOne();
        if (flush != null)
          flush();
        lock (@lock)
        {
          ev.Dispose();
          ev = (ManualResetEvent) null;
        }
        workQueue.Stop();
        this.terminate = (Action) (() => { });
        if (error != null)
          error();
        if (terminated)
          throw new OperationCanceledException();
      }

      public void Terminate() => this.terminate();
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class NativeWebCallback : IWebCallback
    {
      private MemoryStream response = new MemoryStream();
      private IObserver<MultiPartUploader.Args> observer;
      internal bool showProgress;
      private MultiPartUploader.FormData[] formData;
      private string boundary;
      private bool chunked;
      private long bytesWritten;
      public long createTimeUtcTicks = DateTime.UtcNow.Ticks;
      public long writeStartTimeUtcTicks = -1;
      private IByteBuffer bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();

      public long BytesWritten => this.bytesWritten;

      public NativeWebCallback(
        IObserver<MultiPartUploader.Args> observer,
        bool showProgress,
        bool chunked,
        MultiPartUploader.FormData[] formData,
        string boundary)
      {
        this.observer = observer;
        this.showProgress = showProgress;
        this.chunked = chunked;
        this.formData = formData;
        this.boundary = boundary;
      }

      public void Write(IWebWriter writer)
      {
        if (this.writeStartTimeUtcTicks < 0L)
          this.writeStartTimeUtcTicks = DateTime.UtcNow.Ticks;
        long progress = 0;
        long totalLength = 0;
        foreach (MultiPartUploader.FormData formData in this.formData)
          totalLength += formData.Length;
        this.observer.OnNext(new MultiPartUploader.Args()
        {
          CurrentProgress = progress,
          TotalProgress = totalLength
        });
        bool ignoreByteCount = writer is MultiPartUploader.ContentLengthCounter;
        Action<byte[], int, int> baseWrite = (Action<byte[], int, int>) ((bytes, offset, len) =>
        {
          this.bb.Put(bytes, offset, len);
          writer.Write(this.bb);
          this.bb.Reset();
          if (ignoreByteCount)
            return;
          this.bytesWritten += (long) len;
        });
        Action<byte[], int, int, int> write = (Action<byte[], int, int, int>) ((buf, offset, len, progLen) =>
        {
          if (len == 0)
            return;
          baseWrite(buf, offset, len);
          progress += (long) progLen;
          if (!this.showProgress || progLen == 0)
            return;
          this.observer.OnNext(new MultiPartUploader.Args()
          {
            CurrentProgress = progress,
            TotalProgress = totalLength
          });
        });
        Action flushOut = (Action) null;
        if (this.chunked)
          write = this.CreateBufferFunc(4096, this.CreateChunkingFunc(this.CreateBufferFunc(4128, write, ref flushOut), flushOut), ref flushOut);
        int num = MultiPartUploader.TryWriteMultipart(this.boundary, new StringBuilder(), (IEnumerable<MultiPartUploader.FormData>) this.formData, write) ? 1 : 0;
        if (flushOut != null)
          flushOut();
        if (this.chunked)
        {
          byte[] bytes = Encoding.UTF8.GetBytes("0\r\n\r\n");
          baseWrite(bytes, 0, bytes.Length);
        }
        if (num != 0)
          throw new OperationCanceledException();
      }

      private Action<byte[], int, int, int> CreateChunkingFunc(
        Action<byte[], int, int, int> write,
        Action onChunk = null)
      {
        return (Action<byte[], int, int, int>) ((buf, offset, length, progLen) =>
        {
          if (length == 0)
            return;
          byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", (object) length.ToString("x")));
          write(bytes, 0, bytes.Length, 0);
          write(buf, offset, length, progLen);
          write(bytes, bytes.Length - 2, 2, 0);
          if (onChunk == null)
            return;
          onChunk();
        });
      }

      private Action<byte[], int, int, int> CreateBufferFunc(
        int bufsz,
        Action<byte[], int, int, int> write,
        ref Action flushOut)
      {
        byte[] buffer = new byte[bufsz];
        int currentLength = 0;
        int progress = 0;
        Action innerFlush = flushOut;
        Action flush = (Action) (() =>
        {
          write(buffer, 0, currentLength, progress);
          currentLength = 0;
          progress = 0;
          if (innerFlush == null)
            return;
          innerFlush();
        });
        flushOut = flush;
        return (Action<byte[], int, int, int>) ((buf, offset, length, progLen) =>
        {
          progress += progLen;
          while (length != 0)
          {
            int length1 = Math.Min(length, buffer.Length - currentLength);
            if (length1 != 0)
            {
              Array.Copy((Array) buf, offset, (Array) buffer, currentLength, length1);
              currentLength += length1;
              offset += length1;
              length -= length1;
            }
            if (currentLength == buffer.Length)
              flush();
          }
        });
      }

      public int ResponseCode { get; private set; }

      public void OnResponseCode(int code) => this.ResponseCode = code;

      public void OnHeaders(string headers)
      {
      }

      public void ResponseBytesIn(IByteBuffer buf)
      {
        byte[] buffer = buf.Get();
        buf = (IByteBuffer) null;
        this.response.Write(buffer, 0, buffer.Length);
      }

      public void EndResponse()
      {
        this.response.Position = 0L;
        long num1 = -1;
        long num2 = -1;
        if (this.writeStartTimeUtcTicks > 0L)
        {
          num2 = (this.writeStartTimeUtcTicks - this.createTimeUtcTicks) / 10000L;
          num1 = (DateTime.UtcNow.Ticks - this.createTimeUtcTicks) / 10000L;
        }
        this.observer.OnNext(new MultiPartUploader.Args()
        {
          CurrentProgress = 1L,
          TotalProgress = 1L,
          Result = (Stream) this.response,
          ResponseCode = this.ResponseCode,
          NetworkTimeMs = num1,
          ConnectTimeMs = num2
        });
      }
    }

    public class ContentLengthCounter : IWebWriter
    {
      public long ContentLength;

      public void Write(IByteBuffer b) => this.ContentLength += (long) b.GetLength();
    }
  }
}
