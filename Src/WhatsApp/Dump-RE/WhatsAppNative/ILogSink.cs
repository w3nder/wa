// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ILogSink
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1983090722, 9508, 20320, 173, 146, 110, 12, 166, 248, 153, 229)]
  public interface ILogSink
  {
    void OnLogMessage([In] string Log);
  }
}
