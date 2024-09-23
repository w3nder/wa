// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.DiskArchiveStorage
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public class DiskArchiveStorage : BaseArchiveStorage
  {
    private Stream temporaryStream_;
    private string fileName_;
    private string temporaryName_;

    public DiskArchiveStorage(ZipFile file, FileUpdateMode updateMode)
      : base(updateMode)
    {
      this.fileName_ = file.Name != null ? file.Name : throw new ZipException("Cant handle non file archives");
    }

    public DiskArchiveStorage(ZipFile file)
      : this(file, FileUpdateMode.Safe)
    {
    }

    public override Stream GetTemporaryOutput()
    {
      if (this.temporaryName_ != null)
      {
        this.temporaryName_ = this.GetTempFileName(this.temporaryName_, true);
        this.temporaryStream_ = (Stream) File.OpenWrite(this.temporaryName_);
      }
      else
      {
        this.temporaryName_ = Path.GetTempFileName();
        this.temporaryStream_ = (Stream) File.OpenWrite(this.temporaryName_);
      }
      return this.temporaryStream_;
    }

    public override Stream ConvertTemporaryToFinal()
    {
      if (this.temporaryStream_ == null)
        throw new ZipException("No temporary stream has been created");
      Stream stream = (Stream) null;
      string tempFileName = this.GetTempFileName(this.fileName_, false);
      bool flag = false;
      try
      {
        this.temporaryStream_.Close();
        File.Move(this.fileName_, tempFileName);
        File.Move(this.temporaryName_, this.fileName_);
        flag = true;
        File.Delete(tempFileName);
        return (Stream) File.OpenRead(this.fileName_);
      }
      catch (Exception ex)
      {
        stream = (Stream) null;
        if (!flag)
        {
          File.Move(tempFileName, this.fileName_);
          File.Delete(this.temporaryName_);
        }
        throw;
      }
    }

    public override Stream MakeTemporaryCopy(Stream stream)
    {
      stream.Close();
      this.temporaryName_ = this.GetTempFileName(this.fileName_, true);
      File.Copy(this.fileName_, this.temporaryName_, true);
      this.temporaryStream_ = (Stream) new FileStream(this.temporaryName_, FileMode.Open, FileAccess.ReadWrite);
      return this.temporaryStream_;
    }

    public override Stream OpenForDirectUpdate(Stream current)
    {
      Stream stream;
      if (current == null || !current.CanWrite)
      {
        current?.Close();
        stream = (Stream) new FileStream(this.fileName_, FileMode.Open, FileAccess.ReadWrite);
      }
      else
        stream = current;
      return stream;
    }

    public override void Dispose()
    {
      if (this.temporaryStream_ == null)
        return;
      this.temporaryStream_.Close();
    }

    private string GetTempFileName(string original, bool makeTempFile)
    {
      string tempFileName = (string) null;
      if (original == null)
      {
        tempFileName = Path.GetTempFileName();
      }
      else
      {
        int num = 0;
        int second = DateTime.Now.Second;
        while (tempFileName == null)
        {
          ++num;
          string path = string.Format("{0}.{1}{2}.tmp", (object) original, (object) second, (object) num);
          if (!File.Exists(path))
          {
            if (makeTempFile)
            {
              try
              {
                using (File.Create(path))
                  ;
                tempFileName = path;
              }
              catch
              {
                second = DateTime.Now.Second;
              }
            }
            else
              tempFileName = path;
          }
        }
      }
      return tempFileName;
    }
  }
}
