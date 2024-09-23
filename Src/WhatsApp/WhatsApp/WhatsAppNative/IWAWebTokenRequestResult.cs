// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWAWebTokenRequestResult
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(2415140084, 14256, 19045, 188, 40, 53, 224, 24, 235, 161, 140)]
  [Version(100794368)]
  public interface IWAWebTokenRequestResult
  {
    WAWebTokenResponse ResponseData { get; }

    WAWebTokenRequestStatus ResponseStatus { get; }

    WAWebProviderError ResponseError { get; }

    IAsyncAction InvalidateCacheAsync();
  }
}
