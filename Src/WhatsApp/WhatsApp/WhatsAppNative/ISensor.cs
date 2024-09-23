// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ISensor
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(2869490360, 23329, 18908, 149, 104, 76, 110, 142, 52, 36, 14)]
  public interface ISensor
  {
    void SetCallback([In] IAction action);

    void Start();

    void Stop();

    bool GetProximityState();

    bool DetectSupported();
  }
}
