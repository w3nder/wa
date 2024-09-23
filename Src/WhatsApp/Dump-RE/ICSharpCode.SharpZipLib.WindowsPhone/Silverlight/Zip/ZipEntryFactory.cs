// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.ZipEntryFactory
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Silverlight.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public class ZipEntryFactory : IEntryFactory
  {
    private INameTransform nameTransform_;
    private DateTime fixedDateTime_ = DateTime.Now;
    private ZipEntryFactory.TimeSetting timeSetting_;
    private bool isUnicodeText_;
    private int getAttributes_ = -1;
    private int setAttributes_;

    public ZipEntryFactory() => this.nameTransform_ = (INameTransform) new ZipNameTransform();

    public ZipEntryFactory(ZipEntryFactory.TimeSetting timeSetting)
    {
      this.timeSetting_ = timeSetting;
      this.nameTransform_ = (INameTransform) new ZipNameTransform();
    }

    public ZipEntryFactory(DateTime time)
    {
      this.timeSetting_ = ZipEntryFactory.TimeSetting.Fixed;
      this.FixedDateTime = time;
      this.nameTransform_ = (INameTransform) new ZipNameTransform();
    }

    public INameTransform NameTransform
    {
      get => this.nameTransform_;
      set => this.nameTransform_ = value ?? (INameTransform) new ZipNameTransform();
    }

    public ZipEntryFactory.TimeSetting Setting
    {
      get => this.timeSetting_;
      set => this.timeSetting_ = value;
    }

    public DateTime FixedDateTime
    {
      get => this.fixedDateTime_;
      set
      {
        this.fixedDateTime_ = value.Year >= 1970 ? value : throw new ArgumentException("Value is too old to be valid", nameof (value));
      }
    }

    public int GetAttributes
    {
      get => this.getAttributes_;
      set => this.getAttributes_ = value;
    }

    public int SetAttributes
    {
      get => this.setAttributes_;
      set => this.setAttributes_ = value;
    }

    public bool IsUnicodeText
    {
      get => this.isUnicodeText_;
      set => this.isUnicodeText_ = value;
    }

    public ZipEntry MakeFileEntry(string fileName) => this.MakeFileEntry(fileName, true);

    public ZipEntry MakeFileEntry(string fileName, bool useFileSystem)
    {
      ZipEntry zipEntry = new ZipEntry(this.nameTransform_.TransformFile(fileName))
      {
        IsUnicodeText = this.isUnicodeText_
      };
      int num1 = 0;
      bool flag = this.setAttributes_ != 0;
      FileInfo fileInfo = (FileInfo) null;
      if (useFileSystem)
        fileInfo = new FileInfo(fileName);
      if (fileInfo != null && fileInfo.Exists)
      {
        switch (this.timeSetting_)
        {
          case ZipEntryFactory.TimeSetting.LastWriteTime:
            zipEntry.DateTime = fileInfo.LastWriteTime;
            break;
          case ZipEntryFactory.TimeSetting.CreateTime:
            zipEntry.DateTime = fileInfo.CreationTime;
            break;
          case ZipEntryFactory.TimeSetting.LastAccessTime:
            zipEntry.DateTime = fileInfo.LastAccessTime;
            break;
          case ZipEntryFactory.TimeSetting.Fixed:
            zipEntry.DateTime = this.fixedDateTime_;
            break;
          default:
            throw new ZipException("Unhandled time setting in MakeFileEntry");
        }
        zipEntry.Size = fileInfo.Length;
        flag = true;
        num1 = (int) (fileInfo.Attributes & (FileAttributes) this.getAttributes_);
      }
      else if (this.timeSetting_ == ZipEntryFactory.TimeSetting.Fixed)
        zipEntry.DateTime = this.fixedDateTime_;
      if (flag)
      {
        int num2 = num1 | this.setAttributes_;
        zipEntry.ExternalFileAttributes = num2;
      }
      return zipEntry;
    }

    public ZipEntry MakeDirectoryEntry(string directoryName)
    {
      return this.MakeDirectoryEntry(directoryName, true);
    }

    public ZipEntry MakeDirectoryEntry(string directoryName, bool useFileSystem)
    {
      ZipEntry zipEntry = new ZipEntry(this.nameTransform_.TransformDirectory(directoryName))
      {
        Size = 0
      };
      int num1 = 0;
      DirectoryInfo directoryInfo = (DirectoryInfo) null;
      if (useFileSystem)
        directoryInfo = new DirectoryInfo(directoryName);
      if (directoryInfo != null && directoryInfo.Exists)
      {
        switch (this.timeSetting_)
        {
          case ZipEntryFactory.TimeSetting.LastWriteTime:
            zipEntry.DateTime = directoryInfo.LastWriteTime;
            break;
          case ZipEntryFactory.TimeSetting.CreateTime:
            zipEntry.DateTime = directoryInfo.CreationTime;
            break;
          case ZipEntryFactory.TimeSetting.LastAccessTime:
            zipEntry.DateTime = directoryInfo.LastAccessTime;
            break;
          case ZipEntryFactory.TimeSetting.Fixed:
            zipEntry.DateTime = this.fixedDateTime_;
            break;
          default:
            throw new ZipException("Unhandled time setting in MakeDirectoryEntry");
        }
        num1 = (int) (directoryInfo.Attributes & (FileAttributes) this.getAttributes_);
      }
      else if (this.timeSetting_ == ZipEntryFactory.TimeSetting.Fixed)
        zipEntry.DateTime = this.fixedDateTime_;
      int num2 = num1 | this.setAttributes_ | 16;
      zipEntry.ExternalFileAttributes = num2;
      return zipEntry;
    }

    public enum TimeSetting
    {
      LastWriteTime,
      LastWriteTimeUtc,
      CreateTime,
      CreateTimeUtc,
      LastAccessTime,
      LastAccessTimeUtc,
      Fixed,
    }
  }
}
