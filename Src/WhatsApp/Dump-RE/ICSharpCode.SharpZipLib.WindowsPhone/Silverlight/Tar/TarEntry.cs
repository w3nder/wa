// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Tar.TarEntry
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Tar
{
  public class TarEntry
  {
    private string _file;
    private TarHeader _header;

    private TarEntry() => this._header = new TarHeader();

    public TarEntry(byte[] headerBuffer)
    {
      this._header = new TarHeader();
      this._header.ParseBuffer(headerBuffer);
    }

    public TarEntry(TarHeader header)
    {
      this._header = header != null ? (TarHeader) header.Clone() : throw new ArgumentNullException(nameof (header));
    }

    public TarHeader TarHeader => this._header;

    public string Name
    {
      get => this._header.Name;
      set => this._header.Name = value;
    }

    public int UserId
    {
      get => this._header.UserId;
      set => this._header.UserId = value;
    }

    public int GroupId
    {
      get => this._header.GroupId;
      set => this._header.GroupId = value;
    }

    public string UserName
    {
      get => this._header.UserName;
      set => this._header.UserName = value;
    }

    public string GroupName
    {
      get => this._header.GroupName;
      set => this._header.GroupName = value;
    }

    public DateTime ModTime
    {
      get => this._header.ModTime;
      set => this._header.ModTime = value;
    }

    public string File => this._file;

    public long Size
    {
      get => this._header.Size;
      set => this._header.Size = value;
    }

    public bool IsDirectory
    {
      get
      {
        if (this._file != null)
          return Directory.Exists(this._file);
        return this._header != null && (this._header.TypeFlag == (byte) 53 || this.Name.EndsWith("/"));
      }
    }

    public object Clone()
    {
      return (object) new TarEntry()
      {
        _file = this._file,
        _header = (TarHeader) this._header.Clone(),
        Name = this.Name
      };
    }

    public static TarEntry CreateTarEntry(string name)
    {
      TarEntry tarEntry = new TarEntry();
      TarEntry.NameTarHeader(tarEntry._header, name);
      return tarEntry;
    }

    public static TarEntry CreateEntryFromFile(string fileName)
    {
      TarEntry entryFromFile = new TarEntry();
      entryFromFile.GetFileTarHeader(entryFromFile._header, fileName);
      return entryFromFile;
    }

    public override bool Equals(object obj)
    {
      return obj is TarEntry tarEntry && this.Name.Equals(tarEntry.Name);
    }

    public override int GetHashCode() => this.Name.GetHashCode();

    public bool IsDescendent(TarEntry toTest)
    {
      return toTest != null ? toTest.Name.StartsWith(this.Name) : throw new ArgumentNullException(nameof (toTest));
    }

    public void SetIds(int userId, int groupId)
    {
      this.UserId = userId;
      this.GroupId = groupId;
    }

    public void SetNames(string userName, string groupName)
    {
      this.UserName = userName;
      this.GroupName = groupName;
    }

    public void GetFileTarHeader(TarHeader header, string file)
    {
      if (header == null)
        throw new ArgumentNullException(nameof (header));
      this._file = file != null ? file : throw new ArgumentNullException(nameof (file));
      string str1 = file;
      if (str1.IndexOf(Environment.CurrentDirectory) == 0)
        str1 = str1.Substring(Environment.CurrentDirectory.Length);
      string str2 = str1.Replace(Path.DirectorySeparatorChar, '/');
      while (str2.StartsWith("/"))
        str2 = str2.Substring(1);
      header.LinkName = string.Empty;
      header.Name = str2;
      if (Directory.Exists(file))
      {
        header.Mode = 1003;
        header.TypeFlag = (byte) 53;
        if (header.Name.Length == 0 || header.Name[header.Name.Length - 1] != '/')
          header.Name += "/";
        header.Size = 0L;
      }
      else
      {
        header.Mode = 33216;
        header.TypeFlag = (byte) 48;
        header.Size = new FileInfo(file.Replace('/', Path.DirectorySeparatorChar)).Length;
      }
      header.ModTime = System.IO.File.GetLastWriteTime(file.Replace('/', Path.DirectorySeparatorChar)).ToUniversalTime();
      header.DevMajor = 0;
      header.DevMinor = 0;
    }

    public TarEntry[] GetDirectoryEntries()
    {
      if (this._file == null || !Directory.Exists(this._file))
        return new TarEntry[0];
      string[] fileSystemEntries = Directory.GetFileSystemEntries(this._file);
      TarEntry[] directoryEntries = new TarEntry[fileSystemEntries.Length];
      for (int index = 0; index < fileSystemEntries.Length; ++index)
        directoryEntries[index] = TarEntry.CreateEntryFromFile(fileSystemEntries[index]);
      return directoryEntries;
    }

    public void WriteEntryHeader(byte[] outBuffer) => this._header.WriteHeader(outBuffer);

    public static void AdjustEntryName(byte[] buffer, string newName)
    {
      int offset = 0;
      TarHeader.GetNameBytes(newName, buffer, offset, 100);
    }

    public static void NameTarHeader(TarHeader header, string name)
    {
      if (header == null)
        throw new ArgumentNullException(nameof (header));
      bool flag = name != null ? name.EndsWith("/") : throw new ArgumentNullException(nameof (name));
      header.Name = name;
      header.Mode = flag ? 1003 : 33216;
      header.UserId = 0;
      header.GroupId = 0;
      header.Size = 0L;
      header.ModTime = DateTime.UtcNow;
      header.TypeFlag = flag ? (byte) 53 : (byte) 48;
      header.LinkName = string.Empty;
      header.UserName = string.Empty;
      header.GroupName = string.Empty;
      header.DevMajor = 0;
      header.DevMinor = 0;
    }
  }
}
