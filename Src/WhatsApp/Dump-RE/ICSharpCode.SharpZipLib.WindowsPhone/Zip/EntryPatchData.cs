// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Zip.EntryPatchData
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

#nullable disable
namespace ICSharpCode.SharpZipLib.Zip
{
  internal class EntryPatchData
  {
    private long sizePatchOffset_;
    private long crcPatchOffset_;

    public long SizePatchOffset
    {
      get => this.sizePatchOffset_;
      set => this.sizePatchOffset_ = value;
    }

    public long CrcPatchOffset
    {
      get => this.crcPatchOffset_;
      set => this.crcPatchOffset_ = value;
    }
  }
}
