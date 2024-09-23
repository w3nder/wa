// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaHttpContent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace WhatsApp
{
  internal class WaHttpContent : HttpContent
  {
    private Stream content;

    public WaHttpContent(Stream content) => this.content = content;

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
      this.content.Position = 0L;
      await this.content.CopyToAsync(stream);
    }

    protected override bool TryComputeLength(out long length)
    {
      length = this.content.Length;
      return true;
    }
  }
}
