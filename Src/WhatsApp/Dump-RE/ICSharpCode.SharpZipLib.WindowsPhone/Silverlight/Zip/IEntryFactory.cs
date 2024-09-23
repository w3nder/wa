// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.IEntryFactory
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Silverlight.Core;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public interface IEntryFactory
  {
    ZipEntry MakeFileEntry(string fileName);

    ZipEntry MakeFileEntry(string fileName, bool useFileSystem);

    ZipEntry MakeDirectoryEntry(string directoryName);

    ZipEntry MakeDirectoryEntry(string directoryName, bool useFileSystem);

    INameTransform NameTransform { get; set; }
  }
}
