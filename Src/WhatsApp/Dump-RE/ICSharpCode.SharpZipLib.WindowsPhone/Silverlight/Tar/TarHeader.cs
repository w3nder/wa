// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Tar.TarHeader
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.Text;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Tar
{
  public class TarHeader
  {
    public const int CHKSUMLEN = 8;
    public const int CHKSUMOFS = 148;
    public const int DEVLEN = 8;
    public const int GIDLEN = 8;
    public const int GNAMELEN = 32;
    public const string GNU_TMAGIC = "ustar  ";
    public const byte LF_ACL = 65;
    public const byte LF_BLK = 52;
    public const byte LF_CHR = 51;
    public const byte LF_CONTIG = 55;
    public const byte LF_DIR = 53;
    public const byte LF_EXTATTR = 69;
    public const byte LF_FIFO = 54;
    public const byte LF_GHDR = 103;
    public const byte LF_GNU_DUMPDIR = 68;
    public const byte LF_GNU_LONGLINK = 75;
    public const byte LF_GNU_LONGNAME = 76;
    public const byte LF_GNU_MULTIVOL = 77;
    public const byte LF_GNU_NAMES = 78;
    public const byte LF_GNU_SPARSE = 83;
    public const byte LF_GNU_VOLHDR = 86;
    public const byte LF_LINK = 49;
    public const byte LF_META = 73;
    public const byte LF_NORMAL = 48;
    public const byte LF_OLDNORM = 0;
    public const byte LF_SYMLINK = 50;
    public const byte LF_XHDR = 120;
    public const int MAGICLEN = 6;
    public const int MODELEN = 8;
    public const int MODTIMELEN = 12;
    public const int NAMELEN = 100;
    public const int SIZELEN = 12;
    private const long timeConversionFactor = 10000000;
    public const string TMAGIC = "ustar ";
    public const int UIDLEN = 8;
    public const int UNAMELEN = 32;
    public const int VERSIONLEN = 2;
    private static readonly DateTime dateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    private string groupName;
    private string linkName;
    private string magic;
    private int mode;
    private DateTime modTime;
    private string name;
    private long _size;
    private string userName;
    private string version;
    internal static int defaultGroupId;
    internal static string defaultGroupName = "None";
    internal static string defaultUser;
    internal static int defaultUserId;
    internal static int groupIdAsSet;
    internal static string groupNameAsSet = "None";
    internal static int userIdAsSet;
    internal static string userNameAsSet;

    public TarHeader()
    {
      this.Magic = "ustar ";
      this.Version = " ";
      this.Name = "";
      this.LinkName = "";
      this.UserId = TarHeader.defaultUserId;
      this.GroupId = TarHeader.defaultGroupId;
      this.UserName = TarHeader.defaultUser;
      this.GroupName = TarHeader.defaultGroupName;
      this.Size = 0L;
    }

    public string Name
    {
      get => this.name;
      set => this.name = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public int Mode
    {
      get => this.mode;
      set => this.mode = value;
    }

    public int UserId { get; set; }

    public int GroupId { get; set; }

    public long Size
    {
      get => this._size;
      set
      {
        this._size = value >= 0L ? value : throw new ArgumentOutOfRangeException(nameof (value), "Cannot be less than zero");
      }
    }

    public DateTime ModTime
    {
      get => this.modTime;
      set
      {
        if (value < TarHeader.dateTime1970)
          throw new ArgumentOutOfRangeException(nameof (value), "ModTime cannot be before Jan 1st 1970");
        this.modTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
      }
    }

    public int Checksum { get; private set; }

    public bool IsChecksumValid { get; private set; }

    public byte TypeFlag { get; set; }

    public string LinkName
    {
      get => this.linkName;
      set
      {
        this.linkName = value != null ? value : throw new ArgumentNullException(nameof (value));
      }
    }

    public string Magic
    {
      get => this.magic;
      set => this.magic = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public string Version
    {
      get => this.version;
      set => this.version = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public string UserName
    {
      get => this.userName;
      set
      {
        if (value != null)
        {
          this.userName = value.Substring(0, Math.Min(32, value.Length));
        }
        else
        {
          string str = "Silverlight";
          if (str.Length > 32)
            str = str.Substring(0, 32);
          this.userName = str;
        }
      }
    }

    public string GroupName
    {
      get => this.groupName;
      set => this.groupName = value ?? "None";
    }

    public int DevMajor { get; set; }

    public int DevMinor { get; set; }

    [Obsolete("Use the Name property instead", true)]
    public string GetName() => this.name;

    public object Clone() => this.MemberwiseClone();

    public void ParseBuffer(byte[] header)
    {
      if (header == null)
        throw new ArgumentNullException(nameof (header));
      int offset1 = 0;
      this.name = TarHeader.ParseName(header, offset1, 100).ToString();
      int offset2 = offset1 + 100;
      this.mode = (int) TarHeader.ParseOctal(header, offset2, 8);
      int offset3 = offset2 + 8;
      this.UserId = (int) TarHeader.ParseOctal(header, offset3, 8);
      int offset4 = offset3 + 8;
      this.GroupId = (int) TarHeader.ParseOctal(header, offset4, 8);
      int offset5 = offset4 + 8;
      this.Size = TarHeader.ParseOctal(header, offset5, 12);
      int offset6 = offset5 + 12;
      this.ModTime = TarHeader.GetDateTimeFromCTime(TarHeader.ParseOctal(header, offset6, 12));
      int offset7 = offset6 + 12;
      this.Checksum = (int) TarHeader.ParseOctal(header, offset7, 8);
      int num = offset7 + 8;
      byte[] numArray = header;
      int index = num;
      int offset8 = index + 1;
      this.TypeFlag = numArray[index];
      this.LinkName = TarHeader.ParseName(header, offset8, 100).ToString();
      int offset9 = offset8 + 100;
      this.Magic = TarHeader.ParseName(header, offset9, 6).ToString();
      int offset10 = offset9 + 6;
      this.Version = TarHeader.ParseName(header, offset10, 2).ToString();
      int offset11 = offset10 + 2;
      this.UserName = TarHeader.ParseName(header, offset11, 32).ToString();
      int offset12 = offset11 + 32;
      this.GroupName = TarHeader.ParseName(header, offset12, 32).ToString();
      int offset13 = offset12 + 32;
      this.DevMajor = (int) TarHeader.ParseOctal(header, offset13, 8);
      int offset14 = offset13 + 8;
      this.DevMinor = (int) TarHeader.ParseOctal(header, offset14, 8);
      this.IsChecksumValid = this.Checksum == TarHeader.MakeCheckSum(header);
    }

    public void WriteHeader(byte[] outBuffer)
    {
      if (outBuffer == null)
        throw new ArgumentNullException(nameof (outBuffer));
      int offset1 = 0;
      int nameBytes1 = TarHeader.GetNameBytes(this.Name, outBuffer, offset1, 100);
      int octalBytes1 = TarHeader.GetOctalBytes((long) this.mode, outBuffer, nameBytes1, 8);
      int octalBytes2 = TarHeader.GetOctalBytes((long) this.UserId, outBuffer, octalBytes1, 8);
      int octalBytes3 = TarHeader.GetOctalBytes((long) this.GroupId, outBuffer, octalBytes2, 8);
      int longOctalBytes1 = TarHeader.GetLongOctalBytes(this.Size, outBuffer, octalBytes3, 12);
      int longOctalBytes2 = TarHeader.GetLongOctalBytes((long) TarHeader.GetCTime(this.ModTime), outBuffer, longOctalBytes1, 12);
      int offset2 = longOctalBytes2;
      for (int index = 0; index < 8; ++index)
        outBuffer[longOctalBytes2++] = (byte) 32;
      byte[] numArray = outBuffer;
      int index1 = longOctalBytes2;
      int offset3 = index1 + 1;
      int typeFlag = (int) this.TypeFlag;
      numArray[index1] = (byte) typeFlag;
      int nameBytes2 = TarHeader.GetNameBytes(this.LinkName, outBuffer, offset3, 100);
      int asciiBytes = TarHeader.GetAsciiBytes(this.Magic, 0, outBuffer, nameBytes2, 6);
      int nameBytes3 = TarHeader.GetNameBytes(this.Version, outBuffer, asciiBytes, 2);
      int nameBytes4 = TarHeader.GetNameBytes(this.UserName, outBuffer, nameBytes3, 32);
      int offset4 = TarHeader.GetNameBytes(this.GroupName, outBuffer, nameBytes4, 32);
      if (this.TypeFlag == (byte) 51 || this.TypeFlag == (byte) 52)
      {
        int octalBytes4 = TarHeader.GetOctalBytes((long) this.DevMajor, outBuffer, offset4, 8);
        offset4 = TarHeader.GetOctalBytes((long) this.DevMinor, outBuffer, octalBytes4, 8);
      }
      while (offset4 < outBuffer.Length)
        outBuffer[offset4++] = (byte) 0;
      this.Checksum = TarHeader.ComputeCheckSum(outBuffer);
      TarHeader.GetCheckSumOctalBytes((long) this.Checksum, outBuffer, offset2, 8);
      this.IsChecksumValid = true;
    }

    public override int GetHashCode() => this.Name.GetHashCode();

    public override bool Equals(object obj)
    {
      return obj is TarHeader tarHeader && this.name == tarHeader.name && this.mode == tarHeader.mode && this.UserId == tarHeader.UserId && this.GroupId == tarHeader.GroupId && this.Size == tarHeader.Size && this.ModTime == tarHeader.ModTime && this.Checksum == tarHeader.Checksum && (int) this.TypeFlag == (int) tarHeader.TypeFlag && this.LinkName == tarHeader.LinkName && this.Magic == tarHeader.Magic && this.Version == tarHeader.Version && this.UserName == tarHeader.UserName && this.GroupName == tarHeader.GroupName && this.DevMajor == tarHeader.DevMajor && this.DevMinor == tarHeader.DevMinor;
    }

    internal static void SetValueDefaults(
      int userId,
      string userName,
      int groupId,
      string groupName)
    {
      TarHeader.defaultUserId = TarHeader.userIdAsSet = userId;
      TarHeader.defaultUser = TarHeader.userNameAsSet = userName;
      TarHeader.defaultGroupId = TarHeader.groupIdAsSet = groupId;
      TarHeader.defaultGroupName = TarHeader.groupNameAsSet = groupName;
    }

    internal static void RestoreSetValues()
    {
      TarHeader.defaultUserId = TarHeader.userIdAsSet;
      TarHeader.defaultUser = TarHeader.userNameAsSet;
      TarHeader.defaultGroupId = TarHeader.groupIdAsSet;
      TarHeader.defaultGroupName = TarHeader.groupNameAsSet;
    }

    public static long ParseOctal(byte[] header, int offset, int length)
    {
      if (header == null)
        throw new NullReferenceException(nameof (header));
      long octal = 0;
      bool flag = true;
      int num = offset + length;
      for (int index = offset; index < num && header[index] != (byte) 0; ++index)
      {
        if (header[index] == (byte) 32 || header[index] == (byte) 48)
        {
          if (!flag)
          {
            if (header[index] == (byte) 32)
              break;
          }
          else
            continue;
        }
        flag = false;
        octal = (octal << 3) + (long) ((int) header[index] - 48);
      }
      return octal;
    }

    public static StringBuilder ParseName(byte[] header, int offset, int length)
    {
      if (header == null)
        throw new ArgumentNullException(nameof (header));
      if (offset < 0)
        throw new ArgumentOutOfRangeException(nameof (offset), "Cannot be less than zero");
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), "Cannot be less than zero");
      if (offset + length > header.Length)
        throw new ArgumentException("Exceeds header size", nameof (length));
      StringBuilder name = new StringBuilder(length);
      for (int index = offset; index < offset + length && header[index] != (byte) 0; ++index)
        name.Append((char) header[index]);
      return name;
    }

    public static int GetNameBytes(
      StringBuilder name,
      int nameOffset,
      byte[] buffer,
      int bufferOffset,
      int length)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      return TarHeader.GetNameBytes(name.ToString(), nameOffset, buffer, bufferOffset, length);
    }

    public static int GetNameBytes(
      string name,
      int nameOffset,
      byte[] buffer,
      int bufferOffset,
      int length)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      int num;
      for (num = 0; num < length - 1 && nameOffset + num < name.Length; ++num)
        buffer[bufferOffset + num] = (byte) name[nameOffset + num];
      for (; num < length; ++num)
        buffer[bufferOffset + num] = (byte) 0;
      return bufferOffset + length;
    }

    public static int GetNameBytes(StringBuilder name, byte[] buffer, int offset, int length)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      return TarHeader.GetNameBytes(name.ToString(), 0, buffer, offset, length);
    }

    public static int GetNameBytes(string name, byte[] buffer, int offset, int length)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      return TarHeader.GetNameBytes(name, 0, buffer, offset, length);
    }

    public static int GetAsciiBytes(
      string toAdd,
      int nameOffset,
      byte[] buffer,
      int bufferOffset,
      int length)
    {
      if (toAdd == null)
        throw new ArgumentNullException(nameof (toAdd));
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      for (int index = 0; index < length && nameOffset + index < toAdd.Length; ++index)
        buffer[bufferOffset + index] = (byte) toAdd[nameOffset + index];
      return bufferOffset + length;
    }

    public static int GetOctalBytes(long value, byte[] buffer, int offset, int length)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      int num1 = length - 1;
      buffer[offset + num1] = (byte) 0;
      int num2 = num1 - 1;
      if (value > 0L)
      {
        for (long index = value; num2 >= 0 && index > 0L; --num2)
        {
          buffer[offset + num2] = (byte) (48U + (uint) (byte) ((ulong) index & 7UL));
          index >>= 3;
        }
      }
      for (; num2 >= 0; --num2)
        buffer[offset + num2] = (byte) 48;
      return offset + length;
    }

    public static int GetLongOctalBytes(long value, byte[] buffer, int offset, int length)
    {
      return TarHeader.GetOctalBytes(value, buffer, offset, length);
    }

    private static int GetCheckSumOctalBytes(long value, byte[] buffer, int offset, int length)
    {
      TarHeader.GetOctalBytes(value, buffer, offset, length - 1);
      return offset + length;
    }

    private static int ComputeCheckSum(byte[] buffer)
    {
      int checkSum = 0;
      for (int index = 0; index < buffer.Length; ++index)
        checkSum += (int) buffer[index];
      return checkSum;
    }

    private static int MakeCheckSum(byte[] buffer)
    {
      int num = 0;
      for (int index = 0; index < 148; ++index)
        num += (int) buffer[index];
      for (int index = 0; index < 8; ++index)
        num += 32;
      for (int index = 156; index < buffer.Length; ++index)
        num += (int) buffer[index];
      return num;
    }

    private static int GetCTime(DateTime dateTime)
    {
      return (int) ((dateTime.Ticks - TarHeader.dateTime1970.Ticks) / 10000000L);
    }

    private static DateTime GetDateTimeFromCTime(long ticks)
    {
      try
      {
        return new DateTime(TarHeader.dateTime1970.Ticks + ticks * 10000000L);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        return TarHeader.dateTime1970;
      }
    }
  }
}
