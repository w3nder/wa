// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.FastZip
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Silverlight.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public class FastZip
  {
    private bool continueRunning_;
    private byte[] buffer_;
    private ZipOutputStream outputStream_;
    private ZipFile zipFile_;
    private string targetDirectory_;
    private string sourceDirectory_;
    private NameFilter fileFilter_;
    private NameFilter directoryFilter_;
    private FastZip.Overwrite overwrite_;
    private FastZip.ConfirmOverwriteDelegate confirmDelegate_;
    private bool restoreDateTimeOnExtract_;
    private readonly FastZipEvents events_;
    private IEntryFactory entryFactory_ = (IEntryFactory) new ZipEntryFactory();
    private string password_;

    public FastZip()
    {
    }

    public FastZip(FastZipEvents events) => this.events_ = events;

    public bool CreateEmptyDirectories { get; set; }

    public string Password
    {
      get => this.password_;
      set => this.password_ = value;
    }

    public INameTransform NameTransform
    {
      get => this.entryFactory_.NameTransform;
      set => this.entryFactory_.NameTransform = value;
    }

    public IEntryFactory EntryFactory
    {
      get => this.entryFactory_;
      set => this.entryFactory_ = value ?? (IEntryFactory) new ZipEntryFactory();
    }

    public bool RestoreDateTimeOnExtract
    {
      get => this.restoreDateTimeOnExtract_;
      set => this.restoreDateTimeOnExtract_ = value;
    }

    public bool RestoreAttributesOnExtract { get; set; }

    public void CreateZip(
      string zipFileName,
      string sourceDirectory,
      bool recurse,
      string fileFilter,
      string directoryFilter)
    {
      this.CreateZip((Stream) File.Create(zipFileName), sourceDirectory, recurse, fileFilter, directoryFilter);
    }

    public void CreateZip(
      string zipFileName,
      string sourceDirectory,
      bool recurse,
      string fileFilter)
    {
      this.CreateZip((Stream) File.Create(zipFileName), sourceDirectory, recurse, fileFilter, (string) null);
    }

    public void CreateZip(
      Stream outputStream,
      string sourceDirectory,
      bool recurse,
      string fileFilter,
      string directoryFilter)
    {
      this.NameTransform = (INameTransform) new ZipNameTransform(sourceDirectory);
      this.sourceDirectory_ = sourceDirectory;
      using (this.outputStream_ = new ZipOutputStream(outputStream))
      {
        if (this.password_ != null)
          this.outputStream_.Password = this.password_;
        FileSystemScanner fileSystemScanner = new FileSystemScanner(fileFilter, directoryFilter);
        fileSystemScanner.ProcessFile += new ProcessFileHandler(this.ProcessFile);
        if (this.CreateEmptyDirectories)
          fileSystemScanner.ProcessDirectory += new ProcessDirectoryHandler(this.ProcessDirectory);
        if (this.events_ != null)
        {
          if (this.events_.FileFailure != null)
            fileSystemScanner.FileFailure += this.events_.FileFailure;
          if (this.events_.DirectoryFailure != null)
            fileSystemScanner.DirectoryFailure += this.events_.DirectoryFailure;
        }
        fileSystemScanner.Scan(sourceDirectory, recurse);
      }
    }

    public void ExtractZip(string zipFileName, string targetDirectory, string fileFilter)
    {
      this.ExtractZip(zipFileName, targetDirectory, FastZip.Overwrite.Always, (FastZip.ConfirmOverwriteDelegate) null, fileFilter, (string) null, this.restoreDateTimeOnExtract_);
    }

    public void ExtractZip(
      string zipFileName,
      string targetDirectory,
      FastZip.Overwrite overwrite,
      FastZip.ConfirmOverwriteDelegate confirmDelegate,
      string fileFilter,
      string directoryFilter,
      bool restoreDateTime)
    {
      if (overwrite == FastZip.Overwrite.Prompt && confirmDelegate == null)
        throw new ArgumentNullException(nameof (confirmDelegate));
      this.continueRunning_ = true;
      this.overwrite_ = overwrite;
      this.confirmDelegate_ = confirmDelegate;
      this.targetDirectory_ = targetDirectory;
      this.fileFilter_ = new NameFilter(fileFilter);
      this.directoryFilter_ = new NameFilter(directoryFilter);
      this.restoreDateTimeOnExtract_ = restoreDateTime;
      using (this.zipFile_ = new ZipFile(zipFileName))
      {
        if (this.password_ != null)
          this.zipFile_.Password = this.password_;
        IEnumerator enumerator = this.zipFile_.GetEnumerator();
        while (this.continueRunning_ && enumerator.MoveNext())
        {
          ZipEntry current = (ZipEntry) enumerator.Current;
          if (current.IsFile)
          {
            if (this.directoryFilter_.IsMatch(Path.GetDirectoryName(current.Name)) && this.fileFilter_.IsMatch(current.Name))
              this.ExtractEntry(current);
          }
          else if (current.IsDirectory && this.directoryFilter_.IsMatch(current.Name) && this.CreateEmptyDirectories)
            this.ExtractEntry(current);
        }
      }
    }

    private void ProcessDirectory(object sender, DirectoryEventArgs e)
    {
      if (e.HasMatchingFiles || !this.CreateEmptyDirectories)
        return;
      if (this.events_ != null)
        this.events_.OnProcessDirectory(e.Name, e.HasMatchingFiles);
      if (!e.ContinueRunning || !(e.Name != this.sourceDirectory_))
        return;
      this.outputStream_.PutNextEntry(this.entryFactory_.MakeDirectoryEntry(e.Name));
    }

    private void ProcessFile(object sender, ScanEventArgs e)
    {
      if (this.events_ != null && this.events_.ProcessFile != null)
        this.events_.ProcessFile(sender, e);
      if (!e.ContinueRunning)
        return;
      this.outputStream_.PutNextEntry(this.entryFactory_.MakeFileEntry(e.Name));
      this.AddFileContents(e.Name);
    }

    private void AddFileContents(string name)
    {
      if (this.buffer_ == null)
        this.buffer_ = new byte[4096];
      using (FileStream source = File.OpenRead(name))
      {
        if (this.events_ != null && this.events_.Progress != null)
          StreamUtils.Copy((Stream) source, (Stream) this.outputStream_, this.buffer_, this.events_.Progress, this.events_.ProgressInterval, (object) this, name);
        else
          StreamUtils.Copy((Stream) source, (Stream) this.outputStream_, this.buffer_);
      }
      if (this.events_ == null)
        return;
      this.continueRunning_ = this.events_.OnCompletedFile(name);
    }

    private void ExtractFileEntry(ZipEntry entry, string targetName)
    {
      bool flag = true;
      if (this.overwrite_ != FastZip.Overwrite.Always && File.Exists(targetName))
        flag = this.overwrite_ == FastZip.Overwrite.Prompt && this.confirmDelegate_ != null && this.confirmDelegate_(targetName);
      if (!flag)
        return;
      if (this.events_ != null)
        this.continueRunning_ = this.events_.OnProcessFile(entry.Name);
      if (!this.continueRunning_)
        return;
      try
      {
        using (FileStream destination = File.Create(targetName))
        {
          if (this.buffer_ == null)
            this.buffer_ = new byte[4096];
          if (this.events_ != null && this.events_.Progress != null)
            StreamUtils.Copy(this.zipFile_.GetInputStream(entry), (Stream) destination, this.buffer_, this.events_.Progress, this.events_.ProgressInterval, (object) this, entry.Name);
          else
            StreamUtils.Copy(this.zipFile_.GetInputStream(entry), (Stream) destination, this.buffer_);
          if (this.events_ != null)
            this.continueRunning_ = this.events_.OnCompletedFile(entry.Name);
        }
        if (!this.RestoreAttributesOnExtract || !entry.IsDOSEntry || entry.ExternalFileAttributes == -1)
          return;
        FileAttributes fileAttributes = (FileAttributes) (entry.ExternalFileAttributes & 163);
        File.SetAttributes(targetName, fileAttributes);
      }
      catch (Exception ex)
      {
        this.continueRunning_ = this.events_ != null && this.events_.OnFileFailure(targetName, ex);
      }
    }

    private void ExtractEntry(ZipEntry entry)
    {
      bool flag = false;
      string str1 = entry.Name;
      if (entry.IsFile)
        flag = FastZip.NameIsValid(str1) && entry.IsCompressionMethodSupported();
      else if (entry.IsDirectory)
        flag = FastZip.NameIsValid(str1);
      string path = (string) null;
      string str2 = (string) null;
      if (flag)
      {
        if (Path.IsPathRooted(str1))
        {
          string pathRoot = Path.GetPathRoot(str1);
          str1 = str1.Substring(pathRoot.Length);
        }
        if (str1.Length > 0)
        {
          str2 = Path.Combine(this.targetDirectory_, str1);
          path = entry.IsDirectory ? str2 : Path.GetDirectoryName(Path.GetFullPath(str2));
        }
        else
          flag = false;
      }
      if (flag && !Directory.Exists(path))
      {
        if (entry.IsDirectory)
        {
          if (!this.CreateEmptyDirectories)
            goto label_16;
        }
        try
        {
          Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
          flag = false;
          this.continueRunning_ = this.events_ != null && (entry.IsDirectory ? this.events_.OnDirectoryFailure(str2, ex) : this.events_.OnFileFailure(str2, ex));
        }
      }
label_16:
      if (!flag || !entry.IsFile)
        return;
      this.ExtractFileEntry(entry, str2);
    }

    private static int MakeExternalAttributes(FileSystemInfo info) => (int) info.Attributes;

    private static bool NameIsValid(string name)
    {
      return !string.IsNullOrEmpty(name) && name.IndexOfAny(Path.GetInvalidPathChars()) < 0;
    }

    public enum Overwrite
    {
      Prompt,
      Never,
      Always,
    }

    public delegate bool ConfirmOverwriteDelegate(string fileName);
  }
}
