// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.WamEvent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public abstract class WamEvent
  {
    public virtual void SaveEvent() => this.SaveEventWithWeight(1U);

    public virtual void SaveEventSampled(uint interval)
    {
      if (Wam.UniformRandom(interval) != 0)
        return;
      this.SaveEventWithWeight(interval);
    }

    public virtual void SaveEventKeepForAllUsers() => this.SaveEventWithWeight(0U);

    public virtual void SaveEventWithWeight(uint weight) => Wam.LogEvent(this, weight);

    public abstract uint GetCode();

    public abstract void SerializeFields();
  }
}
