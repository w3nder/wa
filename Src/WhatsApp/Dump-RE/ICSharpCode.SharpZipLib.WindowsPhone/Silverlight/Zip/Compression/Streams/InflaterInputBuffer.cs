// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.Compression.Streams.InflaterInputBuffer
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;
using System.Security.Cryptography;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip.Compression.Streams
{
  public class InflaterInputBuffer
  {
    private readonly Stream inputStream;
    private readonly byte[] rawData;
    private int available;
    private byte[] clearText;
    private int clearTextLength;
    private ICryptoTransform cryptoTransform;
    private byte[] internalClearText;
    private int rawLength;

    public InflaterInputBuffer(Stream stream)
      : this(stream, 4096)
    {
    }

    public InflaterInputBuffer(Stream stream, int bufferSize)
    {
      this.inputStream = stream;
      if (bufferSize < 1024)
        bufferSize = 1024;
      this.rawData = new byte[bufferSize];
      this.clearText = this.rawData;
    }

    public int RawLength => this.rawLength;

    public byte[] RawData => this.rawData;

    public int ClearTextLength => this.clearTextLength;

    public byte[] ClearText => this.clearText;

    public int Available
    {
      get => this.available;
      set => this.available = value;
    }

    public ICryptoTransform CryptoTransform
    {
      set
      {
        this.cryptoTransform = value;
        if (this.cryptoTransform != null)
        {
          if (this.rawData == this.clearText)
          {
            if (this.internalClearText == null)
              this.internalClearText = new byte[4096];
            this.clearText = this.internalClearText;
          }
          this.clearTextLength = this.rawLength;
          if (this.available <= 0)
            return;
          this.cryptoTransform.TransformBlock(this.rawData, this.rawLength - this.available, this.available, this.clearText, this.rawLength - this.available);
        }
        else
        {
          this.clearText = this.rawData;
          this.clearTextLength = this.rawLength;
        }
      }
    }

    public void SetInflaterInput(Inflater inflater)
    {
      if (this.available <= 0)
        return;
      inflater.SetInput(this.clearText, this.clearTextLength - this.available, this.available);
      this.available = 0;
    }

    public void Fill()
    {
      this.rawLength = 0;
      int num;
      for (int length = this.rawData.Length; length > 0; length -= num)
      {
        num = this.inputStream.Read(this.rawData, this.rawLength, length);
        if (num <= 0)
        {
          if (this.rawLength == 0)
            throw new SharpZipBaseException("Unexpected EOF");
          break;
        }
        this.rawLength += num;
      }
      this.clearTextLength = this.cryptoTransform != null ? this.cryptoTransform.TransformBlock(this.rawData, 0, this.rawLength, this.clearText, 0) : this.rawLength;
      this.available = this.clearTextLength;
    }

    public int ReadRawBuffer(byte[] buffer) => this.ReadRawBuffer(buffer, 0, buffer.Length);

    public int ReadRawBuffer(byte[] outBuffer, int offset, int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length));
      int destinationIndex = offset;
      int val1 = length;
      while (val1 > 0)
      {
        if (this.available <= 0)
        {
          this.Fill();
          if (this.available <= 0)
            return 0;
        }
        int length1 = Math.Min(val1, this.available);
        Array.Copy((Array) this.rawData, this.rawLength - this.available, (Array) outBuffer, destinationIndex, length1);
        destinationIndex += length1;
        val1 -= length1;
        this.available -= length1;
      }
      return length;
    }

    public int ReadClearTextBuffer(byte[] outBuffer, int offset, int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length));
      int destinationIndex = offset;
      int val1 = length;
      while (val1 > 0)
      {
        if (this.available <= 0)
        {
          this.Fill();
          if (this.available <= 0)
            return 0;
        }
        int length1 = Math.Min(val1, this.available);
        Array.Copy((Array) this.clearText, this.clearTextLength - this.available, (Array) outBuffer, destinationIndex, length1);
        destinationIndex += length1;
        val1 -= length1;
        this.available -= length1;
      }
      return length;
    }

    public int ReadLeByte()
    {
      if (this.available <= 0)
      {
        this.Fill();
        if (this.available <= 0)
          throw new ZipException("EOF in header");
      }
      byte num = (byte) ((uint) this.rawData[this.rawLength - this.available] & (uint) byte.MaxValue);
      --this.available;
      return (int) num;
    }

    public int ReadLeShort() => this.ReadLeByte() | this.ReadLeByte() << 8;

    public int ReadLeInt() => this.ReadLeShort() | this.ReadLeShort() << 16;

    public long ReadLeLong() => (long) (uint) this.ReadLeInt() | (long) this.ReadLeInt() << 32;
  }
}
