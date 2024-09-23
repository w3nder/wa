// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.GZip.GZipInputStream
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Silverlight.Checksums;
using ICSharpCode.SharpZipLib.Silverlight.Zip.Compression;
using ICSharpCode.SharpZipLib.Silverlight.Zip.Compression.Streams;
using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.GZip
{
  public class GZipInputStream : InflaterInputStream
  {
    protected Crc32 crc = new Crc32();
    protected bool eos;
    private bool readGZIPHeader;

    public GZipInputStream(Stream baseInputStream)
      : this(baseInputStream, 4096)
    {
    }

    public GZipInputStream(Stream baseInputStream, int size)
      : base(baseInputStream, new Inflater(true), size)
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (!this.readGZIPHeader)
        this.ReadHeader();
      if (this.eos)
        return 0;
      int count1 = base.Read(buffer, offset, count);
      if (count1 > 0)
        this.crc.Update(buffer, offset, count1);
      if (this.inf.IsFinished)
        this.ReadFooter();
      return count1;
    }

    private void ReadHeader()
    {
      Crc32 crc32 = new Crc32();
      int num1 = this.baseInputStream.ReadByte();
      if (num1 < 0)
        throw new EndOfStreamException("EOS reading GZIP header");
      crc32.Update(num1);
      if (num1 != 31)
        throw new GZipException("Error GZIP header, first magic byte doesn't match");
      int num2 = this.baseInputStream.ReadByte();
      if (num2 < 0)
        throw new EndOfStreamException("EOS reading GZIP header");
      if (num2 != 139)
        throw new GZipException("Error GZIP header,  second magic byte doesn't match");
      crc32.Update(num2);
      int num3 = this.baseInputStream.ReadByte();
      if (num3 < 0)
        throw new EndOfStreamException("EOS reading GZIP header");
      if (num3 != 8)
        throw new GZipException("Error GZIP header, data not in deflate format");
      crc32.Update(num3);
      int num4 = this.baseInputStream.ReadByte();
      if (num4 < 0)
        throw new EndOfStreamException("EOS reading GZIP header");
      crc32.Update(num4);
      if ((num4 & 208) != 0)
        throw new GZipException("Reserved flag bits in GZIP header != 0");
      for (int index = 0; index < 6; ++index)
      {
        int num5 = this.baseInputStream.ReadByte();
        if (num5 < 0)
          throw new EndOfStreamException("EOS reading GZIP header");
        crc32.Update(num5);
      }
      if ((num4 & 4) != 0)
      {
        for (int index = 0; index < 2; ++index)
        {
          int num6 = this.baseInputStream.ReadByte();
          if (num6 < 0)
            throw new EndOfStreamException("EOS reading GZIP header");
          crc32.Update(num6);
        }
        int num7 = this.baseInputStream.ReadByte() >= 0 && this.baseInputStream.ReadByte() >= 0 ? this.baseInputStream.ReadByte() : throw new EndOfStreamException("EOS reading GZIP header");
        int num8 = this.baseInputStream.ReadByte();
        if (num7 < 0 || num8 < 0)
          throw new EndOfStreamException("EOS reading GZIP header");
        crc32.Update(num7);
        crc32.Update(num8);
        int num9 = num7 << 8 | num8;
        for (int index = 0; index < num9; ++index)
        {
          int num10 = this.baseInputStream.ReadByte();
          if (num10 < 0)
            throw new EndOfStreamException("EOS reading GZIP header");
          crc32.Update(num10);
        }
      }
      if ((num4 & 8) != 0)
      {
        int num11;
        while ((num11 = this.baseInputStream.ReadByte()) > 0)
          crc32.Update(num11);
        if (num11 < 0)
          throw new EndOfStreamException("EOS reading GZIP header");
        crc32.Update(num11);
      }
      if ((num4 & 16) != 0)
      {
        int num12;
        while ((num12 = this.baseInputStream.ReadByte()) > 0)
          crc32.Update(num12);
        if (num12 < 0)
          throw new EndOfStreamException("EOS reading GZIP header");
        crc32.Update(num12);
      }
      if ((num4 & 2) != 0)
      {
        int num13 = this.baseInputStream.ReadByte();
        if (num13 < 0)
          throw new EndOfStreamException("EOS reading GZIP header");
        int num14 = this.baseInputStream.ReadByte();
        if (num14 < 0)
          throw new EndOfStreamException("EOS reading GZIP header");
        if ((num13 << 8 | num14) != ((int) crc32.Value & (int) ushort.MaxValue))
          throw new GZipException("Header CRC value mismatch");
      }
      this.readGZIPHeader = true;
    }

    private void ReadFooter()
    {
      byte[] numArray = new byte[8];
      int length = this.inf.RemainingInput;
      if (length > 8)
        length = 8;
      Array.Copy((Array) this.inputBuffer.RawData, this.inputBuffer.RawLength - this.inf.RemainingInput, (Array) numArray, 0, length);
      int num1;
      for (int count = 8 - length; count > 0; count -= num1)
      {
        num1 = this.baseInputStream.Read(numArray, 8 - count, count);
        if (num1 <= 0)
          throw new EndOfStreamException("EOS reading GZIP footer");
      }
      int num2 = (int) numArray[0] & (int) byte.MaxValue | ((int) numArray[1] & (int) byte.MaxValue) << 8 | ((int) numArray[2] & (int) byte.MaxValue) << 16 | (int) numArray[3] << 24;
      if (num2 != (int) this.crc.Value)
        throw new GZipException("GZIP crc sum mismatch, theirs \"" + (object) num2 + "\" and ours \"" + (object) (int) this.crc.Value);
      if ((this.inf.TotalOut & (long) uint.MaxValue) != (long) (uint) ((int) numArray[4] & (int) byte.MaxValue | ((int) numArray[5] & (int) byte.MaxValue) << 8 | ((int) numArray[6] & (int) byte.MaxValue) << 16 | (int) numArray[7] << 24))
        throw new GZipException("Number of bytes mismatch in footer");
      this.eos = true;
    }
  }
}
