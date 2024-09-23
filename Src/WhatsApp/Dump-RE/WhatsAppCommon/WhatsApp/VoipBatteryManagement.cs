// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipBatteryManagement
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Info;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Phone.Devices.Power;

#nullable disable
namespace WhatsApp
{
  public class VoipBatteryManagement
  {
    public const float MinimumDurationToConsiderSec = 60f;
    public const float MinimumPercentDropToConsider = 2f;
    public const float DropEpsilon = 0.1f;
    private int? startingBatteryPct;
    private Stopwatch time = new Stopwatch();
    private int lastSetPct = -1;
    private float lastSetDrop = -1f;
    private object lockRoot = new object();

    public int? LastBatteryChangedValue
    {
      get
      {
        int? startingBatteryPct = this.startingBatteryPct;
        if (DeviceStatus.PowerSource != PowerSource.Battery || !startingBatteryPct.HasValue)
          return new int?();
        return this.lastSetPct != -1 ? new int?(this.lastSetPct - startingBatteryPct.Value) : new int?(0);
      }
    }

    public void Start()
    {
      lock (this.lockRoot)
      {
        if (this.time.IsRunning)
          return;
        this.startInternal();
      }
    }

    public void Stop()
    {
      lock (this.lockRoot)
      {
        if (!this.time.IsRunning)
          return;
        this.stopInternal();
      }
    }

    public void Reset()
    {
      this.startingBatteryPct = new int?();
      this.Stop();
    }

    private void startInternal()
    {
      int remainingChargePercent = Battery.GetDefault().RemainingChargePercent;
      this.startingBatteryPct = new int?(remainingChargePercent);
      this.lastSetPct = -1;
      this.lastSetDrop = -1f;
      this.UpdateBatteryParams(-1f, remainingChargePercent);
      Battery battery = Battery.GetDefault();
      WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(battery.add_RemainingChargePercentChanged), new Action<EventRegistrationToken>(battery.remove_RemainingChargePercentChanged), new EventHandler<object>(this.OnRemainingChargePercentChanged));
      this.time.Restart();
    }

    private void stopInternal()
    {
      WindowsRuntimeMarshal.RemoveEventHandler<EventHandler<object>>(new Action<EventRegistrationToken>(Battery.GetDefault().remove_RemainingChargePercentChanged), new EventHandler<object>(this.OnRemainingChargePercentChanged));
      this.time.Stop();
    }

    private void OnRemainingChargePercentChanged(object sender, object arg)
    {
      int? startingBatteryPct = this.startingBatteryPct;
      if (!startingBatteryPct.HasValue)
        return;
      int remainingChargePercent = Battery.GetDefault().RemainingChargePercent;
      float num1 = (float) (this.time.ElapsedMilliseconds / 1000L);
      float drop = -1f;
      if ((double) num1 >= 60.0)
      {
        int? nullable1 = startingBatteryPct;
        int num2 = remainingChargePercent;
        float? nullable2 = nullable1.HasValue ? new float?((float) (nullable1.GetValueOrDefault() - num2)) : new float?();
        float num3 = 2f;
        if (((double) nullable2.GetValueOrDefault() >= (double) num3 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
        {
          int? nullable3 = startingBatteryPct;
          int num4 = remainingChargePercent;
          drop = (float) (nullable3.HasValue ? new int?(nullable3.GetValueOrDefault() - num4) : new int?()).Value / (num1 / 60f);
        }
      }
      this.UpdateBatteryParams(drop, remainingChargePercent);
    }

    private void UpdateBatteryParams(float drop, int percent)
    {
      if ((double) drop < 0.0)
        drop = -1f;
      if (percent < 0 || percent > 100)
        percent = -1;
      if ((double) Math.Abs(drop - this.lastSetDrop) < 0.10000000149011612 && percent == this.lastSetPct)
        return;
      this.lastSetPct = percent;
      this.lastSetDrop = drop;
      Voip.Worker.Enqueue((Action) (() => Voip.Instance.SetBatteryState(drop, (float) percent)));
    }
  }
}
