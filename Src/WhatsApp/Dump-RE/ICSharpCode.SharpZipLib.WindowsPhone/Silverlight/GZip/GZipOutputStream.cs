// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.GZip.GZipOutputStream
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
  public class GZipOutputStream : DeflaterOutputStream
  {
    protected Crc32 crc = new Crc32();
    private bool headerWritten_;

    public GZipOutputStream(Stream baseOutputStream)
      : this(baseOutputStream, 4096)
    {
    }

    public GZipOutputStream(Stream baseOutputStream, int size)
      : base(baseOutputStream, new Deflater(-1, true), size)
    {
    }

    public void SetLevel(int level)
    {
      if (level < 1)
        throw new ArgumentOutOfRangeException(nameof (level));
      this.deflater_.SetLevel(level);
    }

    public int GetLevel() => this.deflater_.GetLevel();

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (!this.headerWritten_)
        this.WriteHeader();
      this.crc.Update(buffer, offset, count);
      base.Write(buffer, offset, count);
    }

    public override void Close()
    {
      try
      {
        this.Finish();
      }
      finally
      {
        if (this.IsStreamOwner)
          this.baseOutputStream_.Close();
      }
    }

    public override void Finish()
    {
      if (!this.headerWritten_)
        this.WriteHeader();
      base.Finish();
      int totalIn = this.deflater_.TotalIn;
      int num = (int) (this.crc.Value & (long) uint.MaxValue);
      byte[] buffer = new byte[8]
      {
        (byte) num,
        (byte) (num >> 8),
        (byte) (num >> 16),
        (byte) (num >> 24),
        (byte) totalIn,
        (byte) (totalIn >> 8),
        (byte) (totalIn >> 16),
        (byte) (totalIn >> 24)
      };
      this.baseOutputStream_.Write(buffer, 0, buffer.Length);
    }

    private void WriteHeader()
    {
      if (this.headerWritten_)
        return;
      this.headerWritten_ = true;
      int num = (int) ((DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000L);
      byte[] buffer = new byte[10]
      {
        (byte) 31,
        (byte) 139,
        (byte) 8,
        (byte) 0,
        (byte) num,
        (byte) (num >> 8),
        (byte) (num >> 16),
        (byte) (num >> 24),
        (byte) 0,
        byte.MaxValue
      };
      this.baseOutputStream_.Write(buffer, 0, buffer.Length);
    }
  }
}
