// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.IArchiveStorage
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public interface IArchiveStorage
  {
    FileUpdateMode UpdateMode { get; }

    Stream GetTemporaryOutput();

    Stream ConvertTemporaryToFinal();

    Stream MakeTemporaryCopy(Stream stream);

    Stream OpenForDirectUpdate(Stream stream);

    void Dispose();
  }
}
