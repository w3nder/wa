// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWAWebTokenResponse
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1493294051, 28784, 19607, 139, 136, 225, 10, 42, 208, 19, 250)]
  public interface IWAWebTokenResponse
  {
    string Token { get; }

    WAWebProviderError ProviderError { get; }

    WAWebAccount WebAccount { get; }

    IMap<string, string> Properties { get; }
  }
}
