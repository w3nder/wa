// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ISoundPort
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1825913080, 52399, 19827, 168, 49, 86, 196, 58, 72, 7, 42)]
  public interface ISoundPort
  {
    void Initialize([In] ISoundSource Source, [In] ISampleSink Sink, [In] AudioMetadata Metadata);

    void Start();

    void Stop();

    void SetOnComplete([In] IAction Callback);

    void Seek([In] long Millis);

    long GetPosition();

    void Pause();

    long GetDuration();

    void SetOnDispose([In] IAction Callback);

    void Resume();

    int GetVolume();

    void SetVolume([In] int Value);

    void EnumerateDevices([In] bool Capture, [In] IStringSelector DeviceSelector);
  }
}
