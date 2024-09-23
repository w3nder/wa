// Decompiled with JetBrains decompiler
// Type: WhatsApp.WriteToReadPipe
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Threading;


namespace WhatsApp
{
  internal class WriteToReadPipe
  {
    private object @lock = new object();
    private WriteToReadPipe.ReaderBuffer currentRead;
    private WriteToReadPipe.RingBuffer pendingBytes = new WriteToReadPipe.RingBuffer(65536);
    private bool eof;

    public long TotalBytesRead { get; private set; }

    public int Read(byte[] buffer, int offset, int len)
    {
      int num1 = 0;
      WriteToReadPipe.ReaderBuffer readerBuffer = (WriteToReadPipe.ReaderBuffer) null;
      lock (this.@lock)
      {
        if (this.currentRead != null)
          throw new InvalidOperationException("Concurrent readers not allowed");
        int num2 = this.pendingBytes.Read(buffer, offset, len);
        if (num2 != 0)
        {
          offset += num2;
          len -= num2;
          num1 += num2;
        }
        if (len != 0)
        {
          if (!this.eof)
          {
            readerBuffer = new WriteToReadPipe.ReaderBuffer()
            {
              Buf = buffer,
              Offset = offset,
              Length = len,
              Event = new ManualResetEventSlim(false)
            };
            this.currentRead = readerBuffer;
          }
        }
      }
      if (readerBuffer != null)
      {
        using (ManualResetEventSlim ev = readerBuffer.Event)
          ev.WaitOne();
        int num3 = readerBuffer.Offset - offset;
        num1 += num3;
      }
      this.TotalBytesRead += (long) num1;
      return num1;
    }

    public void Write(byte[] buffer, int offset, int len)
    {
      lock (this.@lock)
      {
        if (this.currentRead != null)
        {
          int length = Math.Min(this.currentRead.Length, len);
          Array.Copy((Array) buffer, offset, (Array) this.currentRead.Buf, this.currentRead.Offset, length);
          offset += length;
          len -= length;
          this.currentRead.Offset += length;
          this.currentRead.Length -= length;
          if (this.currentRead.Length == 0)
          {
            this.currentRead.Event.Set();
            this.currentRead = (WriteToReadPipe.ReaderBuffer) null;
          }
        }
        if (len == 0)
          return;
        this.pendingBytes.Write(buffer, offset, len);
      }
    }

    public void OnEndOfFile()
    {
      lock (this.@lock)
      {
        this.eof = true;
        if (this.currentRead == null)
          return;
        this.currentRead.Event.Set();
        this.currentRead = (WriteToReadPipe.ReaderBuffer) null;
      }
    }

    private class ReaderBuffer
    {
      public byte[] Buf;
      public int Offset;
      public int Length;
      public ManualResetEventSlim Event;
    }

    private class RingBuffer
    {
      private byte[] buf;
      private int readOff;
      private int writeOff;
      private int streamLen;

      public RingBuffer(int capacity) => this.buf = new byte[capacity];

      private int OffsetInc(ref int off)
      {
        int num = off++;
        off %= this.buf.Length;
        return num;
      }

      public int Read(byte[] buffer, int off, int len)
      {
        int num = 0;
        while (len != 0 && this.readOff != this.writeOff)
        {
          buffer[off++] = this.buf[this.OffsetInc(ref this.readOff)];
          --len;
          --this.streamLen;
          ++num;
        }
        return num;
      }

      public void Write(byte[] buffer, int off, int len)
      {
        int num = this.buf.Length - this.streamLen;
        if (len > num)
        {
          long length = (long) this.buf.Length;
          while ((long) len > length - (long) this.streamLen)
            length *= 2L;
          byte[] buffer1 = length < 2147483648L ? new byte[(int) length] : throw new InvalidOperationException("Integer overflow");
          int streamLen = this.streamLen;
          this.Read(buffer1, 0, streamLen);
          this.buf = buffer1;
          this.readOff = 0;
          this.writeOff = streamLen;
          this.streamLen = streamLen;
        }
        while (len != 0)
        {
          this.buf[this.OffsetInc(ref this.writeOff)] = buffer[off++];
          --len;
          ++this.streamLen;
        }
      }
    }
  }
}
