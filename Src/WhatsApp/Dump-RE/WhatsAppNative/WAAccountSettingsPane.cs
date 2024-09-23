// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.WAAccountSettingsPane
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Activatable(100794368)]
  [MarshalingBehavior]
  [Version(100794368)]
  public sealed class WAAccountSettingsPane : IWAAccountSettingsPane, IClosable
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern WAAccountSettingsPane();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void OnCurrentViewNavigatedTo();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void OnCurrentViewNavigatedFrom();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Show();

    public extern event TypedEventHandler<WAAccountSettingsPane, WAWebAccountProviderInvokedEventArgs> WebAccountProviderInvoked;

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Close();
  }
}
