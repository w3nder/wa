// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Tar.TarOutputStream
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Tar
{
  public class TarOutputStream : Stream
  {
    protected byte[] assemblyBuffer;
    private int assemblyBufferLength;
    protected byte[] blockBuffer;
    protected TarBuffer _buffer;
    private long currBytes;
    protected long currSize;
    private bool isClosed;
    protected Stream outputStream;

    public TarOutputStream(Stream outputStream)
      : this(outputStream, 20)
    {
    }

    public TarOutputStream(Stream outputStream, int blockFactor)
    {
      this.outputStream = outputStream != null ? outputStream : throw new ArgumentNullException(nameof (outputStream));
      this._buffer = TarBuffer.CreateOutputTarBuffer(outputStream, blockFactor);
      this.assemblyBuffer = new byte[512];
      this.blockBuffer = new byte[512];
    }

    public override bool CanRead => this.outputStream.CanRead;

    public override bool CanSeek => this.outputStream.CanSeek;

    public override bool CanWrite => this.outputStream.CanWrite;

    public override long Length => this.outputStream.Length;

    public override long Position
    {
      get => this.outputStream.Position;
      set => this.outputStream.Position = value;
    }

    public int RecordSize => this._buffer.RecordSize;

    private bool IsEntryOpen => this.currBytes < this.currSize;

    public override long Seek(long offset, SeekOrigin origin)
    {
      return this.outputStream.Seek(offset, origin);
    }

    public override void SetLength(long value) => this.outputStream.SetLength(value);

    public override int ReadByte() => this.outputStream.ReadByte();

    public override int Read(byte[] buffer, int offset, int count)
    {
      return this.outputStream.Read(buffer, offset, count);
    }

    public override void Flush() => this.outputStream.Flush();

    public void Finish()
    {
      if (this.IsEntryOpen)
        this.CloseEntry();
      this.WriteEofBlock();
    }

    public override void Close()
    {
      if (this.isClosed)
        return;
      this.isClosed = true;
      this.Finish();
      this._buffer.Close();
    }

    [Obsolete("Use RecordSize property instead")]
    public int GetRecordSize() => this._buffer.RecordSize;

    public void PutNextEntry(TarEntry entry)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      if (entry.TarHeader.Name.Length >= 100)
      {
        TarHeader tarHeader = new TarHeader()
        {
          TypeFlag = 76
        };
        tarHeader.Name += "././@LongLink";
        tarHeader.UserId = 0;
        tarHeader.GroupId = 0;
        tarHeader.GroupName = string.Empty;
        tarHeader.UserName = string.Empty;
        tarHeader.LinkName = string.Empty;
        tarHeader.Size = (long) entry.TarHeader.Name.Length;
        tarHeader.WriteHeader(this.blockBuffer);
        this._buffer.WriteBlock(this.blockBuffer);
        int nameOffset = 0;
        while (nameOffset < entry.TarHeader.Name.Length)
        {
          Array.Clear((Array) this.blockBuffer, 0, this.blockBuffer.Length);
          TarHeader.GetAsciiBytes(entry.TarHeader.Name, nameOffset, this.blockBuffer, 0, 512);
          nameOffset += 512;
          this._buffer.WriteBlock(this.blockBuffer);
        }
      }
      entry.WriteEntryHeader(this.blockBuffer);
      this._buffer.WriteBlock(this.blockBuffer);
      this.currBytes = 0L;
      this.currSize = entry.IsDirectory ? 0L : entry.Size;
    }

    public void CloseEntry()
    {
      if (this.assemblyBufferLength > 0)
      {
        Array.Clear((Array) this.assemblyBuffer, this.assemblyBufferLength, this.assemblyBuffer.Length - this.assemblyBufferLength);
        this._buffer.WriteBlock(this.assemblyBuffer);
        this.currBytes += (long) this.assemblyBufferLength;
        this.assemblyBufferLength = 0;
      }
      if (this.currBytes < this.currSize)
        throw new TarException(string.Format("Entry closed at '{0}' before the '{1}' bytes specified in the header were written", (object) this.currBytes, (object) this.currSize));
    }

    public override void WriteByte(byte value)
    {
      this.Write(new byte[1]{ value }, 0, 1);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (offset < 0)
        throw new ArgumentOutOfRangeException(nameof (offset), "Cannot be negative");
      if (buffer.Length - offset < count)
        throw new ArgumentException("offset and count combination is invalid");
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count), "Cannot be negative");
      if (this.currBytes + (long) count > this.currSize)
        throw new ArgumentOutOfRangeException(nameof (count), string.Format("request to write '{0}' bytes exceeds size in header of '{1}' bytes", (object) count, (object) this.currSize));
      if (this.assemblyBufferLength > 0)
      {
        if (this.assemblyBufferLength + count >= this.blockBuffer.Length)
        {
          int length = this.blockBuffer.Length - this.assemblyBufferLength;
          Array.Copy((Array) this.assemblyBuffer, 0, (Array) this.blockBuffer, 0, this.assemblyBufferLength);
          Array.Copy((Array) buffer, offset, (Array) this.blockBuffer, this.assemblyBufferLength, length);
          this._buffer.WriteBlock(this.blockBuffer);
          this.currBytes += (long) this.blockBuffer.Length;
          offset += length;
          count -= length;
          this.assemblyBufferLength = 0;
        }
        else
        {
          Array.Copy((Array) buffer, offset, (Array) this.assemblyBuffer, this.assemblyBufferLength, count);
          offset += count;
          this.assemblyBufferLength += count;
          count -= count;
        }
      }
      while (count > 0)
      {
        if (count < this.blockBuffer.Length)
        {
          Array.Copy((Array) buffer, offset, (Array) this.assemblyBuffer, this.assemblyBufferLength, count);
          this.assemblyBufferLength += count;
          break;
        }
        this._buffer.WriteBlock(buffer, offset);
        int length = this.blockBuffer.Length;
        this.currBytes += (long) length;
        count -= length;
        offset += length;
      }
    }

    private void WriteEofBlock()
    {
      Array.Clear((Array) this.blockBuffer, 0, this.blockBuffer.Length);
      this._buffer.WriteBlock(this.blockBuffer);
    }
  }
}
