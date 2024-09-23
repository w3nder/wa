// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.WAWebProviderError
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class WAWebProviderError : IWAWebProviderError
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern WAWebProviderError();

    [Version(100794368)]
    public extern uint ErrorCode { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern string ErrorMessage { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern IMap<string, string> Properties { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}
