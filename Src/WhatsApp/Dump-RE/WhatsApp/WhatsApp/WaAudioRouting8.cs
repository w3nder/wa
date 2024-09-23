// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaAudioRouting8
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Phone.Media.Devices;

#nullable disable
namespace WhatsApp
{
  public class WaAudioRouting8 : WaAudioRouting
  {
    private AudioRoutingManager audioRoutingManager;
    private bool endpointChangesSubscribed;

    public WaAudioRouting8()
    {
      this.audioRoutingManager = AudioRoutingManager.GetDefault();
      this.SubscribeToEndpointChanges();
    }

    protected override void DisposeManagedResources()
    {
      // ISSUE: method pointer
      WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<AudioRoutingManager, object>>(new Action<EventRegistrationToken>(this.audioRoutingManager.remove_AudioEndpointChanged), new TypedEventHandler<AudioRoutingManager, object>((object) this, __methodptr(OnAudioEndpointChanged8)));
      this.endpointChangesSubscribed = false;
      base.DisposeManagedResources();
    }

    protected override WaAudioRouting.Endpoint GetCurrentAudioEndpointImpl()
    {
      return this.Wp8ToWaAudioEndpoint(this.audioRoutingManager.GetAudioEndpoint());
    }

    protected override void SetAudioEndpointImpl(WaAudioRouting.Endpoint endpoint)
    {
      this.audioRoutingManager.SetAudioEndpoint(this.WaToWp8AudioEndpoint(endpoint));
    }

    protected override void SubscribeToEndpointChanges()
    {
      if (this.endpointChangesSubscribed || this.audioRoutingManager == null)
        return;
      // ISSUE: method pointer
      WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<AudioRoutingManager, object>>(new Action<EventRegistrationToken>(this.audioRoutingManager.remove_AudioEndpointChanged), new TypedEventHandler<AudioRoutingManager, object>((object) this, __methodptr(OnAudioEndpointChanged8)));
      AudioRoutingManager audioRoutingManager = this.audioRoutingManager;
      // ISSUE: method pointer
      WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<AudioRoutingManager, object>>(new Func<TypedEventHandler<AudioRoutingManager, object>, EventRegistrationToken>(audioRoutingManager.add_AudioEndpointChanged), new Action<EventRegistrationToken>(audioRoutingManager.remove_AudioEndpointChanged), new TypedEventHandler<AudioRoutingManager, object>((object) this, __methodptr(OnAudioEndpointChanged8)));
      this.endpointChangesSubscribed = true;
    }

    protected override bool IsBluetoothAvailableImpl()
    {
      return (this.audioRoutingManager.AvailableAudioEndpoints & 4) > 0;
    }

    protected override void ToggleBluetoothImpl()
    {
      AudioRoutingEndpoint audioEndpoint = this.audioRoutingManager.GetAudioEndpoint();
      AudioRoutingEndpoint audioRoutingEndpoint;
      switch (audioEndpoint - 3)
      {
        case 0:
        case 3:
        case 4:
          audioRoutingEndpoint = (AudioRoutingEndpoint) 1;
          break;
        default:
          audioRoutingEndpoint = (AudioRoutingEndpoint) 3;
          break;
      }
      Log.d("audio routing", "set endpoint {0} -> {1}", (object) audioEndpoint, (object) audioRoutingEndpoint);
      try
      {
        this.audioRoutingManager.SetAudioEndpoint(audioRoutingEndpoint);
      }
      catch (Exception ex)
      {
        Log.l(ex, "audio routing | toggle bluetooth");
      }
    }

    protected override void ToggleSpeakerImpl()
    {
      AudioRoutingEndpoint audioEndpoint = this.audioRoutingManager.GetAudioEndpoint();
      AudioRoutingEndpoint audioRoutingEndpoint = audioEndpoint != 2 ? (AudioRoutingEndpoint) 2 : (AudioRoutingEndpoint) 1;
      Log.d("audio routing", "set endpoint {0} -> {1}", (object) audioEndpoint, (object) audioRoutingEndpoint);
      try
      {
        this.audioRoutingManager.SetAudioEndpoint(audioRoutingEndpoint);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "audio routing | toggle speaker");
      }
    }

    private void OnAudioEndpointChanged8(AudioRoutingManager sender, object result)
    {
      this.NotifyEndpointChanged();
    }

    private AudioRoutingEndpoint WaToWp8AudioEndpoint(WaAudioRouting.Endpoint endpoint)
    {
      AudioRoutingEndpoint wp8AudioEndpoint = (AudioRoutingEndpoint) 0;
      switch (endpoint)
      {
        case WaAudioRouting.Endpoint.Speaker:
          wp8AudioEndpoint = (AudioRoutingEndpoint) 2;
          break;
        case WaAudioRouting.Endpoint.Earpiece:
          wp8AudioEndpoint = (AudioRoutingEndpoint) 1;
          break;
        case WaAudioRouting.Endpoint.WiredHeadset:
          wp8AudioEndpoint = (AudioRoutingEndpoint) 4;
          break;
        case WaAudioRouting.Endpoint.Bluetooth:
          wp8AudioEndpoint = (AudioRoutingEndpoint) 3;
          break;
      }
      return wp8AudioEndpoint;
    }

    private WaAudioRouting.Endpoint Wp8ToWaAudioEndpoint(AudioRoutingEndpoint endpoint)
    {
      WaAudioRouting.Endpoint waAudioEndpoint = WaAudioRouting.Endpoint.Undefined;
      switch ((int) endpoint)
      {
        case 0:
        case 2:
          waAudioEndpoint = WaAudioRouting.Endpoint.Speaker;
          break;
        case 1:
          waAudioEndpoint = WaAudioRouting.Endpoint.Earpiece;
          break;
        case 3:
        case 6:
        case 7:
          waAudioEndpoint = WaAudioRouting.Endpoint.Bluetooth;
          break;
        case 4:
        case 5:
          waAudioEndpoint = WaAudioRouting.Endpoint.WiredHeadset;
          break;
      }
      return waAudioEndpoint;
    }
  }
}
