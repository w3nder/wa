// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.WAWebAccountProvider
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class WAWebAccountProvider : IWAWebAccountProvider
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern WAWebAccountProvider();

    [Version(100794368)]
    public extern string Id { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern string DisplayName { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern string DisplayPurpose { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern string Authority { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetWrappedObject();
  }
}
