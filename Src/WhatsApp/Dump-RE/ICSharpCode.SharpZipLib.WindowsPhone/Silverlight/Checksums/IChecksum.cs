// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Checksums.IChecksum
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Checksums
{
  public interface IChecksum
  {
    long Value { get; }

    void Reset();

    void Update(int value);

    void Update(byte[] buffer);

    void Update(byte[] buffer, int offset, int count);
  }
}
