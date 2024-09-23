// Decompiled with JetBrains decompiler
// Type: WhatsApp.NativeStream
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using WhatsAppNative;
using Windows.Storage.Streams;


namespace WhatsApp
{
  public class NativeStream : Stream
  {
    private IWAStream native;
    private StreamFlags flags;

    public NativeStream(IWAStream native)
    {
      this.native = native;
      this.flags = native.GetFlags();
    }

    public IWAStream GetNative() => this.native;

    public IRandomAccessStream AsWinRtStream() => this.native.AsWinRtStream();

    public override bool CanRead => true;

    public override bool CanWrite => (this.flags & StreamFlags.WRITABLE) != 0;

    public override bool CanSeek => (this.flags & StreamFlags.SEEKABLE) != 0;

    public override long Position
    {
      get
      {
        this.CheckDisposed();
        return this.native.GetPosition();
      }
      set => this.Seek(value, SeekOrigin.Begin);
    }

    public override long Length
    {
      get
      {
        this.CheckDisposed();
        return this.native.GetLength();
      }
    }

    public override void SetLength(long value)
    {
      this.CheckDisposed();
      this.native.SetLength(value);
    }

    public override void Write(byte[] buf, int offset, int len)
    {
      this.CheckDisposed();
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(buf);
      this.native.Write(instance, offset, len);
      instance.Reset();
    }

    public override int Read(byte[] buf, int offset, int len)
    {
      this.CheckDisposed();
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(buf);
      int num = this.native.Read(instance, offset, len);
      instance.Reset();
      return num;
    }

    public override void Flush()
    {
      this.CheckDisposed();
      this.native.Flush();
    }

    public override long Seek(long off, SeekOrigin origin)
    {
      this.CheckDisposed();
      return this.native.Seek(off, (uint) origin);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (this.native == null)
        return;
      this.native.Dispose();
      this.native = (IWAStream) null;
    }

    public override void Close()
    {
      base.Close();
      if (this.native == null)
        return;
      this.native.Dispose();
      this.native = (IWAStream) null;
    }

    private void CheckDisposed()
    {
      if (this.native == null)
        throw new ObjectDisposedException("Cannot access a disposed object");
    }
  }
}
