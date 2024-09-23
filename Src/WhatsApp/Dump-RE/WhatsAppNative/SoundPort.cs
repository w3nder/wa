// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.SoundPort
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class SoundPort : ISoundPort
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern SoundPort();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Initialize([In] ISoundSource Source, [In] ISampleSink Sink, [In] AudioMetadata Metadata);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Start();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Stop();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetOnComplete([In] IAction Callback);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Seek([In] long Millis);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern long GetPosition();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Pause();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern long GetDuration();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetOnDispose([In] IAction Callback);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Resume();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetVolume();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetVolume([In] int Value);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void EnumerateDevices([In] bool Capture, [In] IStringSelector DeviceSelector);
  }
}
