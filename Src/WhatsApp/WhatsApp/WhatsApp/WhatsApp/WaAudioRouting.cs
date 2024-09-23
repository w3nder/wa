// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaAudioRouting
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Runtime.InteropServices;
using WhatsAppNative;


namespace WhatsApp
{
  public abstract class WaAudioRouting : WaDisposable
  {
    private static object instanceLock = new object();
    private static WaAudioRouting retainedInstance = (WaAudioRouting) null;
    private static Subject<WaAudioRouting.Endpoint> endpointChangedSubj = new Subject<WaAudioRouting.Endpoint>();
    private object accessLock = new object();
    private WaAudioRouting.Endpoint setEndpointTarget = WaAudioRouting.Endpoint.Undefined;
    private int setEndpointRetriesRemaining;
    private WaAudioRouting.Endpoint lastKnownEndpoint = WaAudioRouting.Endpoint.Undefined;
    private static bool isProximitySupported = false;
    private static bool isProximitySupportedSet = false;

    public static bool ProximitySupported
    {
      get
      {
        if (!WaAudioRouting.isProximitySupportedSet)
        {
          Sensor instance = NativeInterfaces.CreateInstance<Sensor>();
          WaAudioRouting.isProximitySupported = ((ISensor) instance).DetectSupported() && (DeviceProfile.Instance.DeviceFlags & DeviceProfile.Flags.NoProximitySensor) == (DeviceProfile.Flags) 0;
          Marshal.ReleaseComObject((object) instance);
          WaAudioRouting.isProximitySupportedSet = true;
        }
        return WaAudioRouting.isProximitySupported;
      }
    }

    protected static WaAudioRouting GetInstance()
    {
      lock (WaAudioRouting.instanceLock)
        return WaAudioRouting.retainedInstance ?? (WaAudioRouting.retainedInstance = (WaAudioRouting) new WaAudioRouting8());
    }

    public static void DisposeInstance()
    {
      lock (WaAudioRouting.instanceLock)
      {
        WaAudioRouting.retainedInstance.SafeDispose();
        WaAudioRouting.retainedInstance = (WaAudioRouting) null;
      }
    }

    public static IObservable<WaAudioRouting.Endpoint> GetEndpointChangedObservable()
    {
      return (IObservable<WaAudioRouting.Endpoint>) WaAudioRouting.endpointChangedSubj;
    }

    public static WaAudioRouting.Endpoint GetCurrentAudioEndpoint()
    {
      return WaAudioRouting.GetInstance().GetCurrentEndpoint();
    }

    public static void SetAudioEndpoint(WaAudioRouting.Endpoint endpoint)
    {
      WaAudioRouting.GetInstance().SetEndpoint(endpoint);
    }

    public static bool IsBluetoothAvailable()
    {
      return WaAudioRouting.GetInstance().IsBluetoothAvailableImpl();
    }

    public static void ToggleBluetooth() => WaAudioRouting.GetInstance().ToggleBluetoothImpl();

    public static void ToggleSpeaker() => WaAudioRouting.GetInstance().ToggleSpeakerImpl();

    protected void NotifyEndpointChanged()
    {
      WaAudioRouting.Endpoint currentEndpoint = this.GetCurrentEndpoint();
      Log.d("audio routing", "endpoint changed to {0}", (object) currentEndpoint);
      WaAudioRouting.endpointChangedSubj.OnNext(currentEndpoint);
    }

    private WaAudioRouting.Endpoint GetCurrentEndpoint()
    {
      lock (this.accessLock)
      {
        if (this.setEndpointTarget != WaAudioRouting.Endpoint.Undefined)
          return this.setEndpointTarget;
        WaAudioRouting.Endpoint currentEndpoint;
        try
        {
          currentEndpoint = this.GetCurrentAudioEndpointImpl();
          this.lastKnownEndpoint = currentEndpoint;
        }
        catch (Exception ex)
        {
          Log.d(ex, "get current audio endpoint");
          currentEndpoint = this.lastKnownEndpoint;
        }
        return currentEndpoint;
      }
    }

    private void SetEndpoint(WaAudioRouting.Endpoint endpoint)
    {
      lock (this.accessLock)
      {
        this.setEndpointRetriesRemaining = 3;
        this.setEndpointTarget = endpoint;
        this.ApplyTargetEndpointNolock();
      }
    }

    private void ApplyTargetEndpoint()
    {
      lock (this.accessLock)
        this.ApplyTargetEndpointNolock();
    }

    private void ApplyTargetEndpointNolock()
    {
      if (this.setEndpointTarget == WaAudioRouting.Endpoint.Undefined || this.setEndpointRetriesRemaining <= 0)
      {
        this.setEndpointTarget = WaAudioRouting.Endpoint.Undefined;
        this.setEndpointRetriesRemaining = 0;
        Log.d("audio routing", "set endpoint | abort");
      }
      else
      {
        --this.setEndpointRetriesRemaining;
        try
        {
          this.SetAudioEndpointImpl(this.setEndpointTarget);
          this.lastKnownEndpoint = this.setEndpointTarget;
          this.setEndpointTarget = WaAudioRouting.Endpoint.Undefined;
          this.setEndpointRetriesRemaining = 0;
          Log.d("audio routing", "set endpoint | success | target:{0}", (object) this.lastKnownEndpoint);
        }
        catch (Exception ex)
        {
          Log.d(ex, "apply target endpoint");
          if (this.setEndpointRetriesRemaining > 0)
          {
            Log.d("audio routing", "set endpoint failed | schedule retry | target:{0},retries left:{1}", (object) this.setEndpointTarget, (object) this.setEndpointRetriesRemaining);
            WAThreadPool.RunAfterDelay(TimeSpan.FromMilliseconds(150.0), (Action) (() => this.ApplyTargetEndpoint()));
          }
          else
            Log.d("audio routing", "set endpoint failed | stop retry | target:{0}", (object) this.setEndpointTarget);
        }
      }
    }

    protected abstract WaAudioRouting.Endpoint GetCurrentAudioEndpointImpl();

    protected abstract void SetAudioEndpointImpl(WaAudioRouting.Endpoint endpoint);

    protected abstract void SubscribeToEndpointChanges();

    protected virtual bool IsBluetoothAvailableImpl() => false;

    protected virtual void ToggleSpeakerImpl()
    {
    }

    protected virtual void ToggleBluetoothImpl()
    {
    }

    public enum Endpoint
    {
      Undefined = -1, // 0xFFFFFFFF
      Speaker = 0,
      Earpiece = 1,
      WiredHeadset = 2,
      Bluetooth = 3,
      Other = 9,
    }
  }
}
