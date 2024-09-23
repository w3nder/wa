// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.BaseArchiveStorage
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public abstract class BaseArchiveStorage : IArchiveStorage
  {
    private FileUpdateMode updateMode_;

    public BaseArchiveStorage(FileUpdateMode updateMode) => this.updateMode_ = updateMode;

    public abstract Stream GetTemporaryOutput();

    public abstract Stream ConvertTemporaryToFinal();

    public abstract Stream MakeTemporaryCopy(Stream stream);

    public abstract Stream OpenForDirectUpdate(Stream stream);

    public abstract void Dispose();

    public FileUpdateMode UpdateMode => this.updateMode_;
  }
}
