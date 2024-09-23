// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWAScheduler
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(2455180224, 57235, 16650, 174, 107, 48, 16, 225, 64, 34, 139)]
  [Version(100794368)]
  public interface IWAScheduler
  {
    void Schedule([In] IAction Action);
  }
}
