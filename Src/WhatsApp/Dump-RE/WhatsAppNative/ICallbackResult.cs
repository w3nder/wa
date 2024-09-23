// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ICallbackResult
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(873014893, 7705, 19370, 157, 217, 110, 31, 15, 66, 10, 221)]
  public interface ICallbackResult
  {
    void OnSuccess();

    void OnError([In] HResult hr);
  }
}
