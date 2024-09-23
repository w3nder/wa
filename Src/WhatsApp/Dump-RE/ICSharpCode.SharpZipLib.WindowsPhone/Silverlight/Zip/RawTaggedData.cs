// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.RawTaggedData
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public class RawTaggedData : ITaggedData
  {
    protected short tag_;
    private byte[] data_;

    public RawTaggedData(short tag) => this.tag_ = tag;

    public short TagID
    {
      get => this.tag_;
      set => this.tag_ = value;
    }

    public void SetData(byte[] data, int offset, int count)
    {
      if (data == null)
        throw new ArgumentNullException(nameof (data));
      this.data_ = new byte[count];
      Array.Copy((Array) data, offset, (Array) this.data_, 0, count);
    }

    public byte[] GetData() => this.data_;

    public byte[] Data
    {
      get => this.data_;
      set => this.data_ = value;
    }
  }
}
