// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Tar.TarArchive
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;
using System.Text;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Tar
{
  public class TarArchive : IDisposable
  {
    private readonly TarInputStream tarIn;
    private readonly TarOutputStream tarOut;
    private bool _applyUserInfoOverrides;
    private bool _asciiTranslate;
    private int _groupId;
    private string _groupName = string.Empty;
    private bool isDisposed;
    private bool _keepOldFiles;
    private string pathPrefix;
    private string rootPath;
    private int _userId;
    private string _userName = string.Empty;

    public bool AsciiTranslate
    {
      get
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        return this._asciiTranslate;
      }
      set
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        this._asciiTranslate = value;
      }
    }

    public string PathPrefix
    {
      get
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        return this.pathPrefix;
      }
      set
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        this.pathPrefix = value;
      }
    }

    public string RootPath
    {
      get
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        return this.rootPath;
      }
      set
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        this.rootPath = value;
      }
    }

    public bool ApplyUserInfoOverrides
    {
      get
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        return this._applyUserInfoOverrides;
      }
      set
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        this._applyUserInfoOverrides = value;
      }
    }

    public int UserId
    {
      get
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        return this._userId;
      }
    }

    public string UserName
    {
      get
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        return this._userName;
      }
    }

    public int GroupId
    {
      get
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        return this._groupId;
      }
    }

    public string GroupName
    {
      get
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        return this._groupName;
      }
    }

    public int RecordSize
    {
      get
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (TarArchive));
        if (this.tarIn != null)
          return this.tarIn.RecordSize;
        return this.tarOut != null ? this.tarOut.RecordSize : 10240;
      }
    }

    void IDisposable.Dispose() => this.Close();

    public event ProgressMessageHandler ProgressMessageEvent;

    protected virtual void OnProgressMessageEvent(TarEntry entry, string message)
    {
      if (this.ProgressMessageEvent == null)
        return;
      this.ProgressMessageEvent(this, entry, message);
    }

    public void SetKeepOldFiles(bool keepOldFiles)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (TarArchive));
      this._keepOldFiles = keepOldFiles;
    }

    [Obsolete("Use the AsciiTranslate property")]
    public void SetAsciiTranslation(bool asciiTranslate)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (TarArchive));
      this._asciiTranslate = asciiTranslate;
    }

    public void SetUserInfo(int userId, string userName, int groupId, string groupName)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (TarArchive));
      this._userId = userId;
      this._userName = userName;
      this._groupId = groupId;
      this._groupName = groupName;
      this._applyUserInfoOverrides = true;
    }

    [Obsolete("Use Close instead")]
    public void CloseArchive() => this.Close();

    public void ListContents()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (TarArchive));
      while (true)
      {
        TarEntry nextEntry = this.tarIn.GetNextEntry();
        if (nextEntry != null)
          this.OnProgressMessageEvent(nextEntry, (string) null);
        else
          break;
      }
    }

    public void ExtractContents(string destinationDirectory)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (TarArchive));
      while (true)
      {
        TarEntry nextEntry = this.tarIn.GetNextEntry();
        if (nextEntry != null)
          this.ExtractEntry(destinationDirectory, nextEntry);
        else
          break;
      }
    }

    private void ExtractEntry(string destDir, TarEntry entry)
    {
      this.OnProgressMessageEvent(entry, (string) null);
      string path = entry.Name;
      if (Path.IsPathRooted(path))
        path = path.Substring(Path.GetPathRoot(path).Length);
      string path2 = path.Replace('/', Path.DirectorySeparatorChar);
      string str1 = Path.Combine(destDir, path2);
      if (entry.IsDirectory)
      {
        TarArchive.EnsureDirectoryExists(str1);
      }
      else
      {
        TarArchive.EnsureDirectoryExists(Path.GetDirectoryName(str1));
        bool flag1 = true;
        FileInfo fileInfo = new FileInfo(str1);
        if (fileInfo.Exists)
        {
          if (this._keepOldFiles)
          {
            this.OnProgressMessageEvent(entry, "Destination file already exists");
            flag1 = false;
          }
          else if ((fileInfo.Attributes & FileAttributes.ReadOnly) != (FileAttributes) 0)
          {
            this.OnProgressMessageEvent(entry, "Destination file already exists, and is read-only");
            flag1 = false;
          }
        }
        if (!flag1)
          return;
        bool flag2 = false;
        Stream stream = (Stream) File.Create(str1);
        if (this._asciiTranslate)
          flag2 = !TarArchive.IsBinary(str1);
        StreamWriter streamWriter = (StreamWriter) null;
        if (flag2)
          streamWriter = new StreamWriter(stream);
        byte[] numArray = new byte[32768];
label_15:
        int count;
        while (true)
        {
          count = this.tarIn.Read(numArray, 0, numArray.Length);
          if (count > 0)
          {
            if (!flag2)
              stream.Write(numArray, 0, count);
            else
              break;
          }
          else
            goto label_24;
        }
        int index1 = 0;
        for (int index2 = 0; index2 < count; ++index2)
        {
          if (numArray[index2] == (byte) 10)
          {
            string str2 = Encoding.UTF8.GetString(numArray, index1, index2 - index1);
            streamWriter.WriteLine(str2);
            index1 = index2 + 1;
          }
        }
        goto label_15;
label_24:
        if (flag2)
          streamWriter.Close();
        else
          stream.Close();
      }
    }

    public void WriteEntry(TarEntry sourceEntry, bool recurse)
    {
      if (sourceEntry == null)
        throw new ArgumentNullException(nameof (sourceEntry));
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (TarArchive));
      try
      {
        if (recurse)
          TarHeader.SetValueDefaults(sourceEntry.UserId, sourceEntry.UserName, sourceEntry.GroupId, sourceEntry.GroupName);
        this.InternalWriteEntry(sourceEntry, recurse);
      }
      finally
      {
        if (recurse)
          TarHeader.RestoreSetValues();
      }
    }

    private void InternalWriteEntry(TarEntry sourceEntry, bool recurse)
    {
      string str1 = (string) null;
      string str2 = sourceEntry.File;
      TarEntry entry = (TarEntry) sourceEntry.Clone();
      if (this._applyUserInfoOverrides)
      {
        entry.GroupId = this._groupId;
        entry.GroupName = this._groupName;
        entry.UserId = this._userId;
        entry.UserName = this._userName;
      }
      this.OnProgressMessageEvent(entry, (string) null);
      if (this._asciiTranslate && !entry.IsDirectory && !TarArchive.IsBinary(str2))
      {
        str1 = Path.GetTempFileName();
        using (StreamReader streamReader = File.OpenText(str2))
        {
          using (Stream stream = (Stream) File.Create(str1))
          {
            while (true)
            {
              string s = streamReader.ReadLine();
              if (s != null)
              {
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                stream.Write(bytes, 0, bytes.Length);
                stream.WriteByte((byte) 10);
              }
              else
                break;
            }
            stream.Flush();
          }
        }
        entry.Size = new FileInfo(str1).Length;
        str2 = str1;
      }
      string str3 = (string) null;
      if (this.rootPath != null && entry.Name.StartsWith(this.rootPath))
        str3 = entry.Name.Substring(this.rootPath.Length + 1);
      if (this.pathPrefix != null)
        str3 = str3 == null ? string.Format("{0}/{1}", (object) this.pathPrefix, (object) entry.Name) : this.pathPrefix + "/" + str3;
      if (str3 != null)
        entry.Name = str3;
      this.tarOut.PutNextEntry(entry);
      if (entry.IsDirectory)
      {
        if (!recurse)
          return;
        foreach (TarEntry directoryEntry in entry.GetDirectoryEntries())
          this.InternalWriteEntry(directoryEntry, recurse);
      }
      else
      {
        using (Stream stream = (Stream) File.OpenRead(str2))
        {
          byte[] buffer = new byte[32768];
          while (true)
          {
            int count = stream.Read(buffer, 0, buffer.Length);
            if (count > 0)
              this.tarOut.Write(buffer, 0, count);
            else
              break;
          }
        }
        if (!string.IsNullOrEmpty(str1))
          File.Delete(str1);
        this.tarOut.CloseEntry();
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      this.isDisposed = true;
      if (!disposing)
        return;
      if (this.tarOut != null)
      {
        this.tarOut.Flush();
        this.tarOut.Close();
      }
      if (this.tarIn == null)
        return;
      this.tarIn.Close();
    }

    public virtual void Close()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~TarArchive() => this.Dispose(false);

    private static void EnsureDirectoryExists(string directoryName)
    {
      if (Directory.Exists(directoryName))
        return;
      try
      {
        Directory.CreateDirectory(directoryName);
      }
      catch (Exception ex)
      {
        throw new TarException("Exception creating directory '" + directoryName + "', " + ex.Message, ex);
      }
    }

    private static bool IsBinary(string filename)
    {
      using (FileStream fileStream = File.OpenRead(filename))
      {
        int count = Math.Min(4096, (int) fileStream.Length);
        byte[] buffer = new byte[count];
        int num1 = fileStream.Read(buffer, 0, count);
        for (int index = 0; index < num1; ++index)
        {
          byte num2 = buffer[index];
          if (num2 < (byte) 8 || num2 > (byte) 13 && num2 < (byte) 32 || num2 == byte.MaxValue)
            return true;
        }
      }
      return false;
    }

    protected TarArchive()
    {
    }

    protected TarArchive(TarInputStream stream)
    {
      this.tarIn = stream != null ? stream : throw new ArgumentNullException(nameof (stream));
    }

    protected TarArchive(TarOutputStream stream)
    {
      this.tarOut = stream != null ? stream : throw new ArgumentNullException(nameof (stream));
    }

    public static TarArchive CreateInputTarArchive(Stream inputStream)
    {
      return inputStream != null ? TarArchive.CreateInputTarArchive(inputStream, 20) : throw new ArgumentNullException(nameof (inputStream));
    }

    public static TarArchive CreateInputTarArchive(Stream inputStream, int blockFactor)
    {
      return inputStream != null ? new TarArchive(new TarInputStream(inputStream, blockFactor)) : throw new ArgumentNullException(nameof (inputStream));
    }

    public static TarArchive CreateOutputTarArchive(Stream outputStream)
    {
      return outputStream != null ? TarArchive.CreateOutputTarArchive(outputStream, 20) : throw new ArgumentNullException(nameof (outputStream));
    }

    public static TarArchive CreateOutputTarArchive(Stream outputStream, int blockFactor)
    {
      return outputStream != null ? new TarArchive(new TarOutputStream(outputStream, blockFactor)) : throw new ArgumentNullException(nameof (outputStream));
    }
  }
}
