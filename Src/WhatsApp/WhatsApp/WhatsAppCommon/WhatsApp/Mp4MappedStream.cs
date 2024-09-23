// Decompiled with JetBrains decompiler
// Type: WhatsApp.Mp4MappedStream
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class Mp4MappedStream : IDisposable
  {
    private string filename;
    public int? Id;

    public string Filename
    {
      get => this.filename ?? "stream:" + (object) this.Id.Value;
      set => this.filename = value;
    }

    public void Dispose()
    {
      if (this.filename != null)
        NativeInterfaces.Mp4Utils.UnmapNamedStream(this.filename);
      if (!this.Id.HasValue)
        return;
      NativeInterfaces.Mp4Utils.UnmapStream(this.Id.Value);
    }
  }
}
