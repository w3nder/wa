// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.WAWebAccount
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Activatable(100794368)]
  [MarshalingBehavior]
  [Version(100794368)]
  public sealed class WAWebAccount : IWAWebAccount
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern WAWebAccount();

    [Version(100794368)]
    public extern WAWebAccountProvider WebAccountProvider { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern string UserName { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern WAWebAccountState State { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [Version(100794368)]
    public extern string Id { [MethodImpl(MethodCodeType = MethodCodeType.Runtime)] get; }

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAsyncAction SignOutAsync();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAsyncAction SignOutWithClientIdAsync([In] string clientId);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetWrappedObject();
  }
}
