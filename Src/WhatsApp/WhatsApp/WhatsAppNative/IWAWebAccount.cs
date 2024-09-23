// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWAWebAccount
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(720786507, 58145, 19181, 148, 207, 81, 24, 240, 57, 68, 31)]
  [Version(100794368)]
  public interface IWAWebAccount
  {
    WAWebAccountProvider WebAccountProvider { get; }

    string UserName { get; }

    WAWebAccountState State { get; }

    string Id { get; }

    IAsyncAction SignOutAsync();

    IAsyncAction SignOutWithClientIdAsync([In] string clientId);

    uint GetWrappedObject();
  }
}
