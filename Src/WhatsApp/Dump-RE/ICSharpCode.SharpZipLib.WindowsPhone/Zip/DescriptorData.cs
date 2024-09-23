// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Zip.DescriptorData
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

#nullable disable
namespace ICSharpCode.SharpZipLib.Zip
{
  public class DescriptorData
  {
    private long size;
    private long compressedSize;
    private long crc;

    public long CompressedSize
    {
      get => this.compressedSize;
      set => this.compressedSize = value;
    }

    public long Size
    {
      get => this.size;
      set => this.size = value;
    }

    public long Crc
    {
      get => this.crc;
      set => this.crc = value & (long) uint.MaxValue;
    }
  }
}
