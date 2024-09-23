// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Tar.TarInputStream
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;
using System.Text;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Tar
{
  public class TarInputStream : Stream
  {
    private readonly Stream inputStream;
    protected TarBuffer _buffer;
    private TarEntry currentEntry;
    protected TarInputStream.IEntryFactory entryFactory;
    protected long entryOffset;
    protected long entrySize;
    protected bool hasHitEOF;
    protected byte[] readBuffer;

    public TarInputStream(Stream inputStream)
      : this(inputStream, 20)
    {
    }

    public TarInputStream(Stream inputStream, int blockFactor)
    {
      this.inputStream = inputStream;
      this._buffer = TarBuffer.CreateInputTarBuffer(inputStream, blockFactor);
    }

    public override bool CanRead => this.inputStream.CanRead;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => this.inputStream.Length;

    public override long Position
    {
      get => this.inputStream.Position;
      set => throw new NotSupportedException("TarInputStream Seek not supported");
    }

    public override void Flush() => this.inputStream.Flush();

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException("TarInputStream Seek not supported");
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException("TarInputStream SetLength not supported");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException("TarInputStream Write not supported");
    }

    public override void WriteByte(byte value)
    {
      throw new NotSupportedException("TarInputStream WriteByte not supported");
    }

    public override int ReadByte()
    {
      byte[] buffer = new byte[1];
      return this.Read(buffer, 0, 1) <= 0 ? -1 : (int) buffer[0];
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      int num1 = 0;
      if (this.entryOffset >= this.entrySize)
        return 0;
      long num2 = (long) count;
      if (num2 + this.entryOffset > this.entrySize)
        num2 = this.entrySize - this.entryOffset;
      if (this.readBuffer != null)
      {
        int num3 = num2 > (long) this.readBuffer.Length ? this.readBuffer.Length : (int) num2;
        Array.Copy((Array) this.readBuffer, 0, (Array) buffer, offset, num3);
        if (num3 >= this.readBuffer.Length)
        {
          this.readBuffer = (byte[]) null;
        }
        else
        {
          int length = this.readBuffer.Length - num3;
          byte[] destinationArray = new byte[length];
          Array.Copy((Array) this.readBuffer, num3, (Array) destinationArray, 0, length);
          this.readBuffer = destinationArray;
        }
        num1 += num3;
        num2 -= (long) num3;
        offset += num3;
      }
      while (num2 > 0L)
      {
        byte[] sourceArray = this._buffer.ReadBlock();
        if (sourceArray == null)
          throw new TarException("unexpected EOF with " + (object) num2 + " bytes unread");
        int num4 = (int) num2;
        int length = sourceArray.Length;
        if (length > num4)
        {
          Array.Copy((Array) sourceArray, 0, (Array) buffer, offset, num4);
          this.readBuffer = new byte[length - num4];
          Array.Copy((Array) sourceArray, num4, (Array) this.readBuffer, 0, length - num4);
        }
        else
        {
          num4 = length;
          Array.Copy((Array) sourceArray, 0, (Array) buffer, offset, length);
        }
        num1 += num4;
        num2 -= (long) num4;
        offset += num4;
      }
      this.entryOffset += (long) num1;
      return num1;
    }

    public override void Close() => this._buffer.Close();

    public int RecordSize => this._buffer.RecordSize;

    public long Available => this.entrySize - this.entryOffset;

    public bool IsMarkSupported => false;

    public void SetEntryFactory(TarInputStream.IEntryFactory factory)
    {
      this.entryFactory = factory;
    }

    [Obsolete("Use RecordSize property instead")]
    public int GetRecordSize() => this._buffer.RecordSize;

    public void Skip(long skipCount)
    {
      byte[] buffer = new byte[8192];
      int num;
      for (long index = skipCount; index > 0L; index -= (long) num)
      {
        int count = index > (long) buffer.Length ? buffer.Length : (int) index;
        num = this.Read(buffer, 0, count);
        if (num == -1)
          break;
      }
    }

    public void Mark(int markLimit)
    {
    }

    public void Reset()
    {
    }

    public TarEntry GetNextEntry()
    {
      if (this.hasHitEOF)
        return (TarEntry) null;
      if (this.currentEntry != null)
        this.SkipToNextEntry();
      byte[] numArray1 = this._buffer.ReadBlock();
      if (numArray1 == null)
        this.hasHitEOF = true;
      else if (this._buffer.IsEOFBlock(numArray1))
        this.hasHitEOF = true;
      if (this.hasHitEOF)
      {
        this.currentEntry = (TarEntry) null;
      }
      else
      {
        try
        {
          TarHeader tarHeader = new TarHeader();
          tarHeader.ParseBuffer(numArray1);
          if (!tarHeader.IsChecksumValid)
            throw new TarException("Header checksum is invalid");
          this.entryOffset = 0L;
          this.entrySize = tarHeader.Size;
          StringBuilder stringBuilder = (StringBuilder) null;
          if (tarHeader.TypeFlag == (byte) 76)
          {
            byte[] numArray2 = new byte[512];
            long entrySize = this.entrySize;
            stringBuilder = new StringBuilder();
            int length;
            for (; entrySize > 0L; entrySize -= (long) length)
            {
              length = this.Read(numArray2, 0, entrySize > (long) numArray2.Length ? numArray2.Length : (int) entrySize);
              if (length == -1)
                throw new InvalidHeaderException("Failed to read long name entry");
              stringBuilder.Append(TarHeader.ParseName(numArray2, 0, length).ToString());
            }
            this.SkipToNextEntry();
            numArray1 = this._buffer.ReadBlock();
          }
          else if (tarHeader.TypeFlag == (byte) 103)
          {
            this.SkipToNextEntry();
            numArray1 = this._buffer.ReadBlock();
          }
          else if (tarHeader.TypeFlag == (byte) 120)
          {
            this.SkipToNextEntry();
            numArray1 = this._buffer.ReadBlock();
          }
          else if (tarHeader.TypeFlag == (byte) 86)
          {
            this.SkipToNextEntry();
            numArray1 = this._buffer.ReadBlock();
          }
          else if (tarHeader.TypeFlag != (byte) 48 && tarHeader.TypeFlag != (byte) 0 && tarHeader.TypeFlag != (byte) 53)
          {
            this.SkipToNextEntry();
            numArray1 = this._buffer.ReadBlock();
          }
          if (this.entryFactory == null)
          {
            this.currentEntry = new TarEntry(numArray1);
            if (stringBuilder != null)
              this.currentEntry.Name = stringBuilder.ToString();
          }
          else
            this.currentEntry = this.entryFactory.CreateEntry(numArray1);
          this.entryOffset = 0L;
          this.entrySize = this.currentEntry.Size;
        }
        catch (InvalidHeaderException ex)
        {
          this.entrySize = 0L;
          this.entryOffset = 0L;
          this.currentEntry = (TarEntry) null;
          throw new InvalidHeaderException(string.Format("Bad header in record {0} block {1} {2}", (object) this._buffer.CurrentRecord, (object) this._buffer.CurrentBlock, (object) ex.Message));
        }
      }
      return this.currentEntry;
    }

    public void CopyEntryContents(Stream outputStream)
    {
      byte[] buffer = new byte[32768];
      while (true)
      {
        int count = this.Read(buffer, 0, buffer.Length);
        if (count > 0)
          outputStream.Write(buffer, 0, count);
        else
          break;
      }
    }

    private void SkipToNextEntry()
    {
      long skipCount = this.entrySize - this.entryOffset;
      if (skipCount > 0L)
        this.Skip(skipCount);
      this.readBuffer = (byte[]) null;
    }

    public interface IEntryFactory
    {
      TarEntry CreateEntry(string name);

      TarEntry CreateEntryFromFile(string fileName);

      TarEntry CreateEntry(byte[] headerBuf);
    }

    public class EntryFactoryAdapter : TarInputStream.IEntryFactory
    {
      public TarEntry CreateEntry(string name) => TarEntry.CreateTarEntry(name);

      public TarEntry CreateEntryFromFile(string fileName)
      {
        return TarEntry.CreateEntryFromFile(fileName);
      }

      public TarEntry CreateEntry(byte[] headerBuffer) => new TarEntry(headerBuffer);
    }
  }
}
