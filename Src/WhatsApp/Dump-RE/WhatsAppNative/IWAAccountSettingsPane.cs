// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWAAccountSettingsPane
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(3050275133, 4713, 17940, 139, 10, 220, 127, 37, 76, 252, 10)]
  [Version(100794368)]
  public interface IWAAccountSettingsPane : IClosable
  {
    void OnCurrentViewNavigatedTo();

    void OnCurrentViewNavigatedFrom();

    void Show();

    event TypedEventHandler<WAAccountSettingsPane, WAWebAccountProviderInvokedEventArgs> WebAccountProviderInvoked;
  }
}
