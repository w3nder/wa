// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.Sensor
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class Sensor : ISensor
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern Sensor();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetCallback([In] IAction action);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Start();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Stop();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetProximityState();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool DetectSupported();
  }
}
