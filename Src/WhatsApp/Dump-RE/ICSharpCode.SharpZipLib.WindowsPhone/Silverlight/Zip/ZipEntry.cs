// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.ZipEntry
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public class ZipEntry
  {
    private ZipEntry.Known known;
    private int externalFileAttributes = -1;
    private ushort versionMadeBy;
    private string name;
    private ulong size;
    private ulong compressedSize;
    private ushort versionToExtract;
    private uint crc;
    private uint dosTime;
    private CompressionMethod method = CompressionMethod.Deflated;
    private byte[] extra;
    private string comment;
    private int flags;
    private long zipFileIndex = -1;
    private long offset;
    private bool forceZip64_;
    private byte cryptoCheckValue_;

    public ZipEntry(string name)
      : this(name, 0, 45, CompressionMethod.Deflated)
    {
    }

    internal ZipEntry(string name, int versionRequiredToExtract)
      : this(name, versionRequiredToExtract, 45, CompressionMethod.Deflated)
    {
    }

    internal ZipEntry(
      string name,
      int versionRequiredToExtract,
      int madeByInfo,
      CompressionMethod method)
    {
      if (name == null)
        throw new ArgumentNullException("ZipEntry name");
      if (name.Length > (int) ushort.MaxValue)
        throw new ArgumentException("Name is too long", nameof (name));
      if (versionRequiredToExtract != 0 && versionRequiredToExtract < 10)
        throw new ArgumentOutOfRangeException(nameof (versionRequiredToExtract));
      this.DateTime = DateTime.Now;
      this.name = name;
      this.versionMadeBy = (ushort) madeByInfo;
      this.versionToExtract = (ushort) versionRequiredToExtract;
      this.method = method;
    }

    [Obsolete("Use Clone instead")]
    public ZipEntry(ZipEntry entry)
    {
      this.known = entry != null ? entry.known : throw new ArgumentNullException(nameof (entry));
      this.name = entry.name;
      this.size = entry.size;
      this.compressedSize = entry.compressedSize;
      this.crc = entry.crc;
      this.dosTime = entry.dosTime;
      this.method = entry.method;
      this.comment = entry.comment;
      this.versionToExtract = entry.versionToExtract;
      this.versionMadeBy = entry.versionMadeBy;
      this.externalFileAttributes = entry.externalFileAttributes;
      this.flags = entry.flags;
      this.zipFileIndex = entry.zipFileIndex;
      this.offset = entry.offset;
      this.forceZip64_ = entry.forceZip64_;
      if (entry.extra == null)
        return;
      this.extra = new byte[entry.extra.Length];
      Array.Copy((Array) entry.extra, 0, (Array) this.extra, 0, entry.extra.Length);
    }

    public bool HasCrc => (this.known & ZipEntry.Known.Crc) != ZipEntry.Known.None;

    public bool IsCrypted
    {
      get => (this.flags & 1) != 0;
      set
      {
        if (value)
          this.flags |= 1;
        else
          this.flags &= -2;
      }
    }

    public bool IsUnicodeText
    {
      get => (this.flags & 2048) != 0;
      set
      {
        if (value)
          this.flags |= 2048;
        else
          this.flags &= -2049;
      }
    }

    internal byte CryptoCheckValue
    {
      get => this.cryptoCheckValue_;
      set => this.cryptoCheckValue_ = value;
    }

    public int Flags
    {
      get => this.flags;
      set => this.flags = value;
    }

    public long ZipFileIndex
    {
      get => this.zipFileIndex;
      set => this.zipFileIndex = value;
    }

    public long Offset
    {
      get => this.offset;
      set => this.offset = value;
    }

    public int ExternalFileAttributes
    {
      get
      {
        return (this.known & ZipEntry.Known.ExternalAttributes) == ZipEntry.Known.None ? -1 : this.externalFileAttributes;
      }
      set
      {
        this.externalFileAttributes = value;
        this.known |= ZipEntry.Known.ExternalAttributes;
      }
    }

    public int VersionMadeBy => (int) this.versionMadeBy & (int) byte.MaxValue;

    public bool IsDOSEntry => this.HostSystem == 0 || this.HostSystem == 10;

    private bool HasDosAttributes(int attributes)
    {
      bool flag = false;
      if ((this.known & ZipEntry.Known.ExternalAttributes) != ZipEntry.Known.None && (this.HostSystem == 0 || this.HostSystem == 10) && (this.ExternalFileAttributes & attributes) == attributes)
        flag = true;
      return flag;
    }

    public int HostSystem
    {
      get => (int) this.versionMadeBy >> 8 & (int) byte.MaxValue;
      set
      {
        this.versionMadeBy &= (ushort) byte.MaxValue;
        this.versionMadeBy |= (ushort) ((value & (int) byte.MaxValue) << 8);
      }
    }

    public int Version
    {
      get
      {
        if (this.versionToExtract != (ushort) 0)
          return (int) this.versionToExtract;
        int version = 10;
        if (this.CentralHeaderRequiresZip64)
          version = 45;
        else if (CompressionMethod.Deflated == this.method)
          version = 20;
        else if (this.IsDirectory)
          version = 20;
        else if (this.IsCrypted)
          version = 20;
        else if (this.HasDosAttributes(8))
          version = 11;
        return version;
      }
    }

    public bool CanDecompress
    {
      get
      {
        return this.Version <= 45 && (this.Version == 10 || this.Version == 11 || this.Version == 20 || this.Version == 45) && this.IsCompressionMethodSupported();
      }
    }

    public void ForceZip64() => this.forceZip64_ = true;

    public bool IsZip64Forced() => this.forceZip64_;

    public bool LocalHeaderRequiresZip64
    {
      get
      {
        bool headerRequiresZip64 = this.forceZip64_;
        if (!headerRequiresZip64)
        {
          ulong compressedSize = this.compressedSize;
          if (this.versionToExtract == (ushort) 0 && this.IsCrypted)
            compressedSize += 12UL;
          headerRequiresZip64 = (this.size >= (ulong) uint.MaxValue || compressedSize >= (ulong) uint.MaxValue) && (this.versionToExtract == (ushort) 0 || this.versionToExtract >= (ushort) 45);
        }
        return headerRequiresZip64;
      }
    }

    public bool CentralHeaderRequiresZip64
    {
      get => this.LocalHeaderRequiresZip64 || this.offset >= (long) uint.MaxValue;
    }

    public long DosTime
    {
      get => (this.known & ZipEntry.Known.Time) == ZipEntry.Known.None ? 0L : (long) this.dosTime;
      set
      {
        this.dosTime = (uint) value;
        this.known |= ZipEntry.Known.Time;
      }
    }

    public DateTime DateTime
    {
      get
      {
        int second = Math.Min(59, 2 * ((int) this.dosTime & 31));
        int minute = Math.Min(59, (int) (this.dosTime >> 5) & 63);
        int hour = Math.Min(23, (int) (this.dosTime >> 11) & 31);
        int month = Math.Max(1, Math.Min(12, (int) (this.dosTime >> 21) & 15));
        uint year = (uint) (((int) (this.dosTime >> 25) & (int) sbyte.MaxValue) + 1980);
        int day = Math.Max(1, Math.Min(DateTime.DaysInMonth((int) year, month), (int) (this.dosTime >> 16) & 31));
        return new DateTime((int) year, month, day, hour, minute, second);
      }
      set
      {
        uint num1 = (uint) value.Year;
        uint num2 = (uint) value.Month;
        uint num3 = (uint) value.Day;
        uint num4 = (uint) value.Hour;
        uint num5 = (uint) value.Minute;
        uint num6 = (uint) value.Second;
        if (num1 < 1980U)
        {
          num1 = 1980U;
          num2 = 1U;
          num3 = 1U;
          num4 = 0U;
          num5 = 0U;
          num6 = 0U;
        }
        else if (num1 > 2107U)
        {
          num1 = 2107U;
          num2 = 12U;
          num3 = 31U;
          num4 = 23U;
          num5 = 59U;
          num6 = 59U;
        }
        this.DosTime = (long) ((uint) (((int) num1 - 1980 & (int) sbyte.MaxValue) << 25 | (int) num2 << 21 | (int) num3 << 16 | (int) num4 << 11 | (int) num5 << 5) | num6 >> 1);
      }
    }

    public string Name => this.name;

    public long Size
    {
      get => (this.known & ZipEntry.Known.Size) == ZipEntry.Known.None ? -1L : (long) this.size;
      set
      {
        this.size = (ulong) value;
        this.known |= ZipEntry.Known.Size;
      }
    }

    public long CompressedSize
    {
      get
      {
        return (this.known & ZipEntry.Known.CompressedSize) == ZipEntry.Known.None ? -1L : (long) this.compressedSize;
      }
      set
      {
        this.compressedSize = (ulong) value;
        this.known |= ZipEntry.Known.CompressedSize;
      }
    }

    public long Crc
    {
      get
      {
        return (this.known & ZipEntry.Known.Crc) == ZipEntry.Known.None ? -1L : (long) this.crc & (long) uint.MaxValue;
      }
      set
      {
        this.crc = ((long) this.crc & -4294967296L) == 0L ? (uint) value : throw new ArgumentOutOfRangeException(nameof (value));
        this.known |= ZipEntry.Known.Crc;
      }
    }

    public CompressionMethod CompressionMethod
    {
      get => this.method;
      set
      {
        this.method = ZipEntry.IsCompressionMethodSupported(value) ? value : throw new NotSupportedException("Compression method not supported");
      }
    }

    public byte[] ExtraData
    {
      get => this.extra;
      set
      {
        if (value == null)
        {
          this.extra = (byte[]) null;
        }
        else
        {
          this.extra = value.Length <= (int) ushort.MaxValue ? new byte[value.Length] : throw new ArgumentOutOfRangeException(nameof (value));
          Array.Copy((Array) value, 0, (Array) this.extra, 0, value.Length);
        }
      }
    }

    internal void ProcessExtraData(bool localHeader)
    {
      ZipExtraData zipExtraData = new ZipExtraData(this.extra);
      if (zipExtraData.Find(1))
      {
        if (((int) this.versionToExtract & (int) byte.MaxValue) < 45)
          throw new ZipException("Zip64 Extended information found but version is not valid");
        this.forceZip64_ = true;
        if (zipExtraData.ValueLength < 4)
          throw new ZipException("Extra data extended Zip64 information length is invalid");
        if (localHeader || this.size == (ulong) uint.MaxValue)
          this.size = (ulong) zipExtraData.ReadLong();
        if (localHeader || this.compressedSize == (ulong) uint.MaxValue)
          this.compressedSize = (ulong) zipExtraData.ReadLong();
        if (!localHeader && this.offset == (long) uint.MaxValue)
          this.offset = zipExtraData.ReadLong();
      }
      else if (((int) this.versionToExtract & (int) byte.MaxValue) >= 45 && (this.size == (ulong) uint.MaxValue || this.compressedSize == (ulong) uint.MaxValue))
        throw new ZipException("Zip64 Extended information required but is missing.");
      if (zipExtraData.Find(10))
      {
        if (zipExtraData.ValueLength < 8)
          throw new ZipException("NTFS Extra data invalid");
        zipExtraData.ReadInt();
        while (zipExtraData.UnreadCount >= 4)
        {
          int num = zipExtraData.ReadShort();
          int amount = zipExtraData.ReadShort();
          if (num == 1)
          {
            if (amount < 24)
              break;
            long fileTime = zipExtraData.ReadLong();
            zipExtraData.ReadLong();
            zipExtraData.ReadLong();
            this.DateTime = DateTime.FromFileTime(fileTime);
            break;
          }
          zipExtraData.Skip(amount);
        }
      }
      else
      {
        if (!zipExtraData.Find(21589))
          return;
        int valueLength = zipExtraData.ValueLength;
        if ((zipExtraData.ReadByte() & 1) == 0 || valueLength < 5)
          return;
        int seconds = zipExtraData.ReadInt();
        this.DateTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds, 0)).ToLocalTime();
      }
    }

    public string Comment
    {
      get => this.comment;
      set
      {
        this.comment = value == null || value.Length <= (int) ushort.MaxValue ? value : throw new ArgumentOutOfRangeException(nameof (value), "cannot exceed 65535");
      }
    }

    public bool IsDirectory
    {
      get
      {
        int length = this.name.Length;
        return length > 0 && (this.name[length - 1] == '/' || this.name[length - 1] == '\\') || this.HasDosAttributes(16);
      }
    }

    public bool IsFile => !this.IsDirectory && !this.HasDosAttributes(8);

    public bool IsCompressionMethodSupported()
    {
      return ZipEntry.IsCompressionMethodSupported(this.CompressionMethod);
    }

    public object Clone()
    {
      ZipEntry zipEntry = (ZipEntry) this.MemberwiseClone();
      if (this.extra != null)
      {
        zipEntry.extra = new byte[this.extra.Length];
        Array.Copy((Array) this.extra, 0, (Array) zipEntry.extra, 0, this.extra.Length);
      }
      return (object) zipEntry;
    }

    public override string ToString() => this.name;

    public static bool IsCompressionMethodSupported(CompressionMethod method)
    {
      return method == CompressionMethod.Deflated || method == CompressionMethod.Stored;
    }

    public static string CleanName(string name)
    {
      if (name == null)
        return string.Empty;
      if (Path.IsPathRooted(name))
        name = name.Substring(Path.GetPathRoot(name).Length);
      name = name.Replace("\\", "/");
      while (name.Length > 0 && name[0] == '/')
        name = name.Remove(0, 1);
      return name;
    }

    [System.Flags]
    private enum Known : byte
    {
      None = 0,
      Size = 1,
      CompressedSize = 2,
      Crc = 4,
      Time = 8,
      ExternalAttributes = 16, // 0x10
    }
  }
}
