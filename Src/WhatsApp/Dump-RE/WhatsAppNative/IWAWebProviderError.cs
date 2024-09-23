// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWAWebProviderError
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(1440609622, 15189, 19035, 165, 234, 190, 42, 181, 61, 109, 215)]
  [Version(100794368)]
  public interface IWAWebProviderError
  {
    uint ErrorCode { get; }

    string ErrorMessage { get; }

    IMap<string, string> Properties { get; }
  }
}
