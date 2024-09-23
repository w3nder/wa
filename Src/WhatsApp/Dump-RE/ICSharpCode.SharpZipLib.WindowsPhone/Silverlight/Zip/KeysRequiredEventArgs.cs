// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.KeysRequiredEventArgs
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public class KeysRequiredEventArgs : EventArgs
  {
    private string fileName;
    private byte[] key;

    public KeysRequiredEventArgs(string name) => this.fileName = name;

    public KeysRequiredEventArgs(string name, byte[] keyValue)
    {
      this.fileName = name;
      this.key = keyValue;
    }

    public string FileName => this.fileName;

    public byte[] Key
    {
      get => this.key;
      set => this.key = value;
    }
  }
}
