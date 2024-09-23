// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaStorageStreamExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using WhatsAppNative;
using Windows.Storage.Streams;


namespace WhatsApp
{
  public static class MediaStorageStreamExtensions
  {
    public static IRandomAccessStream AsWinRtStream(this IWAStream stream)
    {
      return (IRandomAccessStream) NativeInterfaces.Misc.WaStreamToWinRtStream(stream);
    }

    public static IWAStream ToWaStream(this Stream source, bool forceReadOnly = false)
    {
      return source is NativeStream nativeStream && (!forceReadOnly || !nativeStream.CanWrite) ? nativeStream.GetNative() : (IWAStream) new MediaStorageStreamExtensions.ManagedStreamWrapper(source, forceReadOnly);
    }

    private class ManagedStreamWrapper : IWAStream
    {
      private Stream stream;
      private StreamFlags streamFlags;
      private long? position;
      private MediaStorageStreamExtensions.ManagedStreamWrapper.SyncObject cloneHandler;
      private RefCountAction disposer;
      private IDisposable disposable;

      public ManagedStreamWrapper(Stream stream, bool forceReadOnly)
        : this(stream, forceReadOnly, new MediaStorageStreamExtensions.ManagedStreamWrapper.SyncObject(), new RefCountAction((Action) (() => { }), new Action(stream.Dispose)))
      {
      }

      private ManagedStreamWrapper(
        Stream stream,
        bool forceReadOnly,
        MediaStorageStreamExtensions.ManagedStreamWrapper.SyncObject cloneHandler,
        RefCountAction disposer)
      {
        this.stream = stream;
        this.cloneHandler = cloneHandler;
        this.disposer = disposer;
        this.disposable = this.disposer.Subscribe();
        if (stream.CanSeek)
          this.streamFlags |= StreamFlags.SEEKABLE;
        if (forceReadOnly || !stream.CanWrite)
          return;
        this.streamFlags |= StreamFlags.WRITABLE;
      }

      public IWAStream Clone()
      {
        if (this.stream is NativeStream stream)
          return (IWAStream) new MediaStorageStreamExtensions.ManagedStreamWrapper((Stream) new NativeStream(stream.GetNative().Clone()), false)
          {
            streamFlags = this.streamFlags
          };
        if (!this.position.HasValue)
          this.position = new long?(this.stream.Position);
        return (IWAStream) new MediaStorageStreamExtensions.ManagedStreamWrapper(this.stream, false, this.cloneHandler, this.disposer)
        {
          position = this.position
        };
      }

      public void Dispose() => this.disposable.Dispose();

      public void Flush() => this.stream.Flush();

      public StreamFlags GetFlags() => this.streamFlags;

      public long GetLength() => this.stream.Length;

      public long GetPosition() => this.cloneHandler.GetPosition(this.stream, this.position);

      public int Read(IByteBuffer buf, int Offset, int Length)
      {
        return this.ReadBase((uint) ((ulong) buf.GetPointer() + (ulong) Offset), Length);
      }

      public int ReadBase(uint Buf, int Length)
      {
        IntPtr destination = (IntPtr) (long) Buf;
        byte[] buf = new byte[Length];
        int n = 0;
        this.cloneHandler.Sync(this.stream, ref this.position, (Action) (() => n = this.stream.Read(buf, 0, Length)));
        if (n <= 0)
          return n;
        Marshal.Copy(buf, 0, destination, n);
        return n;
      }

      public long Seek(long Offset, uint Whence)
      {
        this.cloneHandler.Seek(this.stream, ref this.position, Offset, (SeekOrigin) Whence);
        return this.GetPosition();
      }

      public void SetLength(long Length) => this.stream.SetLength(Length);

      private void Write(byte[] buf, int offset, int len)
      {
        if ((this.streamFlags & StreamFlags.WRITABLE) == (StreamFlags) 0)
          throw new UnauthorizedAccessException("Read-only stream");
        this.cloneHandler.Sync(this.stream, ref this.position, (Action) (() => this.stream.Write(buf, offset, len)));
      }

      public void Write(IByteBuffer buf, int Offset, int Length)
      {
        this.WriteBase((uint) ((ulong) buf.GetPointer() + (ulong) Offset), Length);
      }

      public void WriteBase(uint Buf, int Length)
      {
        IntPtr source = (IntPtr) (long) Buf;
        byte[] buf = new byte[Length];
        byte[] destination = buf;
        int length = buf.Length;
        Marshal.Copy(source, destination, 0, length);
        this.Write(buf, 0, buf.Length);
      }

      private class SyncObject
      {
        private object @lock = new object();

        public long GetPosition(Stream s, long? perObjectPos) => perObjectPos ?? s.Position;

        public void Seek(Stream s, ref long? perObjectPos, long off, SeekOrigin whence)
        {
          if (perObjectPos.HasValue)
          {
            long length = s.Length;
            switch (whence)
            {
              case SeekOrigin.Begin:
                perObjectPos = new long?(Math.Min(length, off));
                break;
              case SeekOrigin.Current:
                perObjectPos = new long?(Math.Min(length, perObjectPos.Value + off));
                break;
              case SeekOrigin.End:
                perObjectPos = new long?(Math.Min(length, length + off));
                break;
            }
          }
          else
            s.Seek(off, whence);
        }

        public void Sync(Stream s, ref long? perObjectPos, Action a)
        {
          if (perObjectPos.HasValue)
          {
            lock (this.@lock)
            {
              long position = s.Position;
              s.Position = perObjectPos.Value;
              a();
              perObjectPos = new long?(s.Position);
            }
          }
          else
            a();
        }
      }
    }
  }
}
