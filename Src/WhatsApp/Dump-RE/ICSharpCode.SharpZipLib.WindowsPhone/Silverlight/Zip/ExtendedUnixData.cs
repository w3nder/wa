// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.ExtendedUnixData
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public class ExtendedUnixData : ITaggedData
  {
    private ExtendedUnixData.Flags flags_;
    private DateTime modificationTime_ = new DateTime(1970, 1, 1);
    private DateTime lastAccessTime_ = new DateTime(1970, 1, 1);
    private DateTime createTime_ = new DateTime(1970, 1, 1);

    public short TagID => 21589;

    public void SetData(byte[] data, int index, int count)
    {
      using (MemoryStream memoryStream = new MemoryStream(data, index, count, false))
      {
        using (ZipHelperStream zipHelperStream = new ZipHelperStream((Stream) memoryStream))
        {
          this.flags_ = (ExtendedUnixData.Flags) zipHelperStream.ReadByte();
          if ((this.flags_ & ExtendedUnixData.Flags.ModificationTime) != (ExtendedUnixData.Flags) 0 && count >= 5)
          {
            int seconds = zipHelperStream.ReadLEInt();
            this.modificationTime_ = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds, 0)).ToLocalTime();
          }
          if ((this.flags_ & ExtendedUnixData.Flags.AccessTime) != (ExtendedUnixData.Flags) 0)
          {
            int seconds = zipHelperStream.ReadLEInt();
            this.lastAccessTime_ = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds, 0)).ToLocalTime();
          }
          if ((this.flags_ & ExtendedUnixData.Flags.CreateTime) == (ExtendedUnixData.Flags) 0)
            return;
          int seconds1 = zipHelperStream.ReadLEInt();
          this.createTime_ = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds1, 0)).ToLocalTime();
        }
      }
    }

    public byte[] GetData()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (ZipHelperStream zipHelperStream = new ZipHelperStream((Stream) memoryStream))
        {
          zipHelperStream.IsStreamOwner = false;
          zipHelperStream.WriteByte((byte) this.flags_);
          if ((this.flags_ & ExtendedUnixData.Flags.ModificationTime) != (ExtendedUnixData.Flags) 0)
          {
            int totalSeconds = (int) (this.modificationTime_.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
            zipHelperStream.WriteLEInt(totalSeconds);
          }
          if ((this.flags_ & ExtendedUnixData.Flags.AccessTime) != (ExtendedUnixData.Flags) 0)
          {
            int totalSeconds = (int) (this.lastAccessTime_.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
            zipHelperStream.WriteLEInt(totalSeconds);
          }
          if ((this.flags_ & ExtendedUnixData.Flags.CreateTime) != (ExtendedUnixData.Flags) 0)
          {
            int totalSeconds = (int) (this.createTime_.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
            zipHelperStream.WriteLEInt(totalSeconds);
          }
          return memoryStream.ToArray();
        }
      }
    }

    public static bool IsValidValue(DateTime value)
    {
      return value >= new DateTime(1901, 12, 13, 20, 45, 52) || value <= new DateTime(2038, 1, 19, 3, 14, 7);
    }

    public DateTime ModificationTime
    {
      get => this.modificationTime_;
      set
      {
        if (!ExtendedUnixData.IsValidValue(value))
          throw new ArgumentOutOfRangeException(nameof (value));
        this.flags_ |= ExtendedUnixData.Flags.ModificationTime;
        this.modificationTime_ = value;
      }
    }

    public DateTime AccessTime
    {
      get => this.lastAccessTime_;
      set
      {
        if (!ExtendedUnixData.IsValidValue(value))
          throw new ArgumentOutOfRangeException(nameof (value));
        this.flags_ |= ExtendedUnixData.Flags.AccessTime;
        this.lastAccessTime_ = value;
      }
    }

    public DateTime CreateTime
    {
      get => this.createTime_;
      set
      {
        if (!ExtendedUnixData.IsValidValue(value))
          throw new ArgumentOutOfRangeException(nameof (value));
        this.flags_ |= ExtendedUnixData.Flags.CreateTime;
        this.createTime_ = value;
      }
    }

    private ExtendedUnixData.Flags Include
    {
      get => this.flags_;
      set => this.flags_ = value;
    }

    [System.Flags]
    public enum Flags : byte
    {
      ModificationTime = 1,
      AccessTime = 2,
      CreateTime = 4,
    }
  }
}
