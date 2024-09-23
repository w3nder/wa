// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.PlayAudioMessage8
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices.Sensors;
using Microsoft.Phone.Reactive;
using Microsoft.Xna.Framework;
using System;
using WhatsAppNative;

#nullable disable
namespace WhatsApp.CommonOps
{
  internal class PlayAudioMessage8 : PlayAudioMessage
  {
    private ISensor sensor_;
    private IDisposable proximityPollingSub_;
    private bool logProximityException_ = true;
    private bool proximityState_;
    private int goBackMs = -1000;
    private bool isSlowProximity;
    private DateTime? initialStallUntilTime;
    private const int FAST_PROX_STALL_MS = 750;
    private const int SLOW_PROX_STALL_MS = 1000;
    private Accelerometer proximityHelperAccelerometer;
    private const double ACCELERATION_TOLERANCE = 1.21;
    private bool hasBeenAccelerated;
    private const int EXTRA_STALL_IF_ACCELERATED_MS = 2000;
    private DateTime? detectionStartTime;
    private double maxAccelDetected;
    private static double weightedAverageFaceDetectionTime;
    private static int weightedAverageFaceDetectionCount;

    protected override void Init()
    {
      base.Init();
      this.isSlowProximity = (DeviceProfile.Instance.DeviceFlags & DeviceProfile.Flags.SlowProximitySensor) == DeviceProfile.Flags.SlowProximitySensor;
      this.goBackMs = this.isSlowProximity ? -3000 : -1000;
    }

    protected override void DisposeManagedResources()
    {
      this.DisposeProximitySensor();
      base.DisposeManagedResources();
    }

    private void InitProximitySensor()
    {
      if (this.sensor_ != null)
        return;
      try
      {
        this.sensor_ = (ISensor) NativeInterfaces.CreateInstance<Sensor>();
        if (!this.isSlowProximity || !Accelerometer.IsSupported || this.proximityHelperAccelerometer != null)
          return;
        this.proximityHelperAccelerometer = new Accelerometer();
        this.proximityHelperAccelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(50.0);
        this.proximityHelperAccelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(this.Accelerometer_CurrentValueChanged);
      }
      catch (Exception ex)
      {
        this.DisposeProximitySensor();
        Log.LogException(ex, "init proximity sensor");
      }
    }

    private void DisposeProximitySensor()
    {
      this.StopProximitySensor();
      this.sensor_ = (ISensor) null;
      this.safeDisposeAccelerometer(this.proximityHelperAccelerometer);
      this.proximityHelperAccelerometer = (Accelerometer) null;
    }

    private void StartProximitySensor(WaAudioRouting.Endpoint currEndpoint)
    {
      if (this.sensor_ == null)
      {
        this.InitProximitySensor();
        if (this.sensor_ == null)
          return;
      }
      try
      {
        this.proximityState_ = currEndpoint == WaAudioRouting.Endpoint.Earpiece;
        this.sensor_.Start();
        this.detectionStartTime = new DateTime?(DateTime.Now);
        if (this.proximityHelperAccelerometer != null)
        {
          this.hasBeenAccelerated = false;
          this.maxAccelDetected = 0.0;
          this.proximityHelperAccelerometer.Start();
          Log.l("ptt playback", "sensor and accelerometer start {0}", (object) this.proximityState_);
        }
        else
          Log.l("ptt playback", "sensor start {0}", (object) this.proximityState_);
        int num1 = this.isSlowProximity ? 1000 : 750;
        int num2 = 250;
        this.initialStallUntilTime = new DateTime?(DateTime.Now.AddMilliseconds((double) num1));
        this.proximityPollingSub_ = Observable.Timer(TimeSpan.FromMilliseconds((double) num1), TimeSpan.FromMilliseconds((double) num2)).Subscribe<long>((Action<long>) (_ =>
        {
          bool stallOver = false;
          if (this.initialStallUntilTime.HasValue && DateTime.Now >= this.initialStallUntilTime.Value.AddMilliseconds(this.hasBeenAccelerated ? 2000.0 : 0.0))
          {
            stallOver = true;
            this.initialStallUntilTime = new DateTime?();
            if (this.proximityHelperAccelerometer != null)
              this.proximityHelperAccelerometer.Stop();
            Log.l("ptt playback", "proximity sensor must fire");
          }
          this.SyncProximityState(stallOver);
        }));
      }
      catch (Exception ex)
      {
        this.DisposeProximitySensor();
        Log.LogException(ex, "starting proximity sensor");
      }
    }

    private void StopProximitySensor()
    {
      if (this.sensor_ != null)
      {
        try
        {
          this.sensor_.Stop();
          Log.l("ptt playback", "sensor stop");
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "stopping proximity sensor");
        }
      }
      this.proximityPollingSub_.SafeDispose();
      this.proximityPollingSub_ = (IDisposable) null;
      if (this.proximityHelperAccelerometer == null)
        return;
      try
      {
        this.proximityHelperAccelerometer.Stop();
        Log.l("ptt playback", "accelerometer stop");
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "stopping proximity accelerometer sensor");
      }
    }

    private void SyncProximityState(bool stallOver)
    {
      bool proximityState = this.GetProximityState();
      if (!(this.proximityState_ != proximityState | stallOver))
        return;
      Log.d("ptt playback", "proximity state change:{0} from {1}", (object) proximityState, (object) this.proximityState_);
      this.proximityState_ = proximityState;
      this.OnProximityChanged();
    }

    private bool GetProximityState()
    {
      bool proximityState = false;
      try
      {
        proximityState = this.sensor_.GetProximityState();
      }
      catch (Exception ex)
      {
        if (this.logProximityException_)
        {
          this.logProximityException_ = false;
          Log.LogException(ex, "checking proximity state");
        }
      }
      return proximityState;
    }

    private void OnProximityChanged()
    {
      if (!this.player_.IsActive)
        return;
      WaAudioRouting.Endpoint currentAudioEndpoint = WaAudioRouting.GetCurrentAudioEndpoint();
      if (this.proximityState_)
      {
        if (currentAudioEndpoint == WaAudioRouting.Endpoint.Speaker || this.player_.AudioOutput == WaAudioRouting.Endpoint.Speaker)
        {
          Log.l("ptt playback", "audio routing correction | speaker -> earpiece");
          if (!this.player_.IsStalled)
          {
            try
            {
              this.player_.RelativeSeek((double) this.goBackMs);
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "seeking back");
            }
          }
          this.player_.SwitchAudioOutput(WaAudioRouting.Endpoint.Earpiece);
        }
      }
      else if (currentAudioEndpoint == WaAudioRouting.Endpoint.Earpiece || this.player_.AudioOutput == WaAudioRouting.Endpoint.Earpiece)
      {
        Log.l("ptt playback", "audio routing correction | earpiece -> speaker");
        if (!this.player_.IsStalled)
          this.player_.Pause();
        this.player_.SwitchAudioOutput(WaAudioRouting.Endpoint.Speaker);
      }
      if (!this.player_.IsStalled)
        return;
      this.player_.UnStall();
      DateTime now = DateTime.Now;
      DateTime? detectionStartTime = this.detectionStartTime;
      double totalMilliseconds = (detectionStartTime.HasValue ? new TimeSpan?(now - detectionStartTime.GetValueOrDefault()) : new TimeSpan?()).Value.TotalMilliseconds;
      if (PlayAudioMessage8.weightedAverageFaceDetectionCount == 0)
      {
        PlayAudioMessage8.weightedAverageFaceDetectionCount = 1;
        PlayAudioMessage8.weightedAverageFaceDetectionTime = totalMilliseconds;
      }
      else
      {
        ++PlayAudioMessage8.weightedAverageFaceDetectionCount;
        double num = PlayAudioMessage8.weightedAverageFaceDetectionCount > 9 ? 0.1 : 1.0 / (double) PlayAudioMessage8.weightedAverageFaceDetectionCount;
        PlayAudioMessage8.weightedAverageFaceDetectionTime = PlayAudioMessage8.weightedAverageFaceDetectionTime * (1.0 - num) + num * totalMilliseconds;
      }
      Log.l("ptt playback", "unstalled figures: {0} {1} {2} {3} {4}", (object) totalMilliseconds, (object) this.hasBeenAccelerated, (object) this.maxAccelDetected, (object) PlayAudioMessage8.weightedAverageFaceDetectionCount, (object) PlayAudioMessage8.weightedAverageFaceDetectionTime);
    }

    public void Accelerometer_CurrentValueChanged(
      object sender,
      SensorReadingEventArgs<AccelerometerReading> e)
    {
      Vector3 acceleration = e.SensorReading.Acceleration;
      double num = Math.Pow((double) acceleration.X, 2.0) + Math.Pow((double) acceleration.Y, 2.0) + Math.Pow((double) acceleration.Z, 2.0);
      if (num > 1.21)
        this.hasBeenAccelerated = true;
      if (num <= this.maxAccelDetected)
        return;
      this.maxAccelDetected = num;
    }

    private void safeDisposeAccelerometer(Accelerometer ourAccelerometer)
    {
      if (ourAccelerometer != null)
      {
        try
        {
          ourAccelerometer.Stop();
          ourAccelerometer.Dispose();
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "disposing proximity acceleromter");
        }
        ourAccelerometer = (Accelerometer) null;
      }
      this.hasBeenAccelerated = false;
    }

    protected override void OnPlaybackStarted()
    {
      base.OnPlaybackStarted();
      if (WaAudioRouting.IsBluetoothAvailable())
      {
        this.StopProximitySensor();
      }
      else
      {
        WaAudioRouting.Endpoint currentAudioEndpoint = WaAudioRouting.GetCurrentAudioEndpoint();
        switch (currentAudioEndpoint)
        {
          case WaAudioRouting.Endpoint.Speaker:
          case WaAudioRouting.Endpoint.Earpiece:
            this.player_.Stall();
            this.StartProximitySensor(currentAudioEndpoint);
            break;
          default:
            this.StopProximitySensor();
            break;
        }
      }
    }

    protected override void OnPlaybackStopped()
    {
      base.OnPlaybackStopped();
      this.StopProximitySensor();
    }

    protected override void OnAudioOutputChanged(WaAudioRouting.Endpoint newEndpoint)
    {
      base.OnAudioOutputChanged(newEndpoint);
      if (newEndpoint == WaAudioRouting.Endpoint.Speaker || newEndpoint == WaAudioRouting.Endpoint.Earpiece)
        return;
      this.StopProximitySensor();
    }
  }
}
