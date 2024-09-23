// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Core.FileSystemScanner
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Core
{
  public class FileSystemScanner
  {
    public CompletedFileHandler CompletedFile;
    public DirectoryFailureHandler DirectoryFailure;
    public FileFailureHandler FileFailure;
    public ProcessDirectoryHandler ProcessDirectory;
    public ProcessFileHandler ProcessFile;
    private readonly IScanFilter directoryFilter_;
    private readonly IScanFilter fileFilter_;
    private bool alive_;

    public FileSystemScanner(string filter)
    {
      this.fileFilter_ = (IScanFilter) new PathFilter(filter);
    }

    public FileSystemScanner(string fileFilter, string directoryFilter)
    {
      this.fileFilter_ = (IScanFilter) new PathFilter(fileFilter);
      this.directoryFilter_ = (IScanFilter) new PathFilter(directoryFilter);
    }

    public FileSystemScanner(IScanFilter fileFilter) => this.fileFilter_ = fileFilter;

    public FileSystemScanner(IScanFilter fileFilter, IScanFilter directoryFilter)
    {
      this.fileFilter_ = fileFilter;
      this.directoryFilter_ = directoryFilter;
    }

    private void OnDirectoryFailure(string directory, Exception e)
    {
      if (this.DirectoryFailure == null)
      {
        this.alive_ = false;
      }
      else
      {
        ScanFailureEventArgs e1 = new ScanFailureEventArgs(directory, e);
        this.DirectoryFailure((object) this, e1);
        this.alive_ = e1.ContinueRunning;
      }
    }

    private void OnFileFailure(string file, Exception e)
    {
      if (this.FileFailure == null)
      {
        this.alive_ = false;
      }
      else
      {
        ScanFailureEventArgs e1 = new ScanFailureEventArgs(file, e);
        this.FileFailure((object) this, e1);
        this.alive_ = e1.ContinueRunning;
      }
    }

    private void OnProcessFile(string file)
    {
      if (this.ProcessFile == null)
        return;
      ScanEventArgs e = new ScanEventArgs(file);
      this.ProcessFile((object) this, e);
      this.alive_ = e.ContinueRunning;
    }

    private void OnCompleteFile(string file)
    {
      if (this.CompletedFile == null)
        return;
      ScanEventArgs e = new ScanEventArgs(file);
      this.CompletedFile((object) this, e);
      this.alive_ = e.ContinueRunning;
    }

    private void OnProcessDirectory(string directory, bool hasMatchingFiles)
    {
      if (this.ProcessDirectory == null)
        return;
      DirectoryEventArgs e = new DirectoryEventArgs(directory, hasMatchingFiles);
      this.ProcessDirectory((object) this, e);
      this.alive_ = e.ContinueRunning;
    }

    public void Scan(string directory, bool recurse)
    {
      this.alive_ = true;
      this.ScanDir(directory, recurse);
    }

    private void ScanDir(string directory, bool recurse)
    {
      try
      {
        string[] files = Directory.GetFiles(directory);
        bool hasMatchingFiles = false;
        for (int index = 0; index < files.Length; ++index)
        {
          if (!this.fileFilter_.IsMatch(files[index]))
            files[index] = (string) null;
          else
            hasMatchingFiles = true;
        }
        this.OnProcessDirectory(directory, hasMatchingFiles);
        if (this.alive_)
        {
          if (hasMatchingFiles)
          {
            foreach (string file in files)
            {
              try
              {
                if (file != null)
                {
                  this.OnProcessFile(file);
                  if (!this.alive_)
                    break;
                }
              }
              catch (Exception ex)
              {
                this.OnFileFailure(file, ex);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.OnDirectoryFailure(directory, ex);
      }
      if (!this.alive_)
        return;
      if (!recurse)
        return;
      try
      {
        foreach (string directory1 in Directory.GetDirectories(directory))
        {
          if (this.directoryFilter_ == null || this.directoryFilter_.IsMatch(directory1))
          {
            this.ScanDir(directory1, true);
            if (!this.alive_)
              break;
          }
        }
      }
      catch (Exception ex)
      {
        this.OnDirectoryFailure(directory, ex);
      }
    }
  }
}
