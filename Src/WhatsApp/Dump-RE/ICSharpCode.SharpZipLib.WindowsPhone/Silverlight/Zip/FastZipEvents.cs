// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.FastZipEvents
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Silverlight.Core;
using System;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public class FastZipEvents
  {
    public ProcessDirectoryHandler ProcessDirectory;
    public ProcessFileHandler ProcessFile;
    public ProgressHandler Progress;
    public CompletedFileHandler CompletedFile;
    public DirectoryFailureHandler DirectoryFailure;
    public FileFailureHandler FileFailure;
    private TimeSpan progressInterval_ = TimeSpan.FromSeconds(3.0);

    public bool OnDirectoryFailure(string directory, Exception e)
    {
      bool flag = false;
      if (this.DirectoryFailure != null)
      {
        ScanFailureEventArgs e1 = new ScanFailureEventArgs(directory, e);
        this.DirectoryFailure((object) this, e1);
        flag = e1.ContinueRunning;
      }
      return flag;
    }

    public bool OnFileFailure(string file, Exception e)
    {
      bool flag = false;
      if (this.FileFailure != null)
      {
        ScanFailureEventArgs e1 = new ScanFailureEventArgs(file, e);
        this.FileFailure((object) this, e1);
        flag = e1.ContinueRunning;
      }
      return flag;
    }

    public bool OnProcessFile(string file)
    {
      bool flag = true;
      if (this.ProcessFile != null)
      {
        ScanEventArgs e = new ScanEventArgs(file);
        this.ProcessFile((object) this, e);
        flag = e.ContinueRunning;
      }
      return flag;
    }

    public bool OnCompletedFile(string file)
    {
      bool flag = true;
      if (this.CompletedFile != null)
      {
        ScanEventArgs e = new ScanEventArgs(file);
        this.CompletedFile((object) this, e);
        flag = e.ContinueRunning;
      }
      return flag;
    }

    public bool OnProcessDirectory(string directory, bool hasMatchingFiles)
    {
      bool flag = true;
      if (this.ProcessDirectory != null)
      {
        DirectoryEventArgs e = new DirectoryEventArgs(directory, hasMatchingFiles);
        this.ProcessDirectory((object) this, e);
        flag = e.ContinueRunning;
      }
      return flag;
    }

    public TimeSpan ProgressInterval
    {
      get => this.progressInterval_;
      set => this.progressInterval_ = value;
    }
  }
}
