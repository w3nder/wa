// Decompiled with JetBrains decompiler
// Type: WhatsApp.QrSessionInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class QrSessionInfo : PropChangedBase
  {
    private DateTime? lastConnected;
    private string location;

    public string BrowserId { get; set; }

    public string OperatingSystem { get; set; }

    public string Browser { get; set; }

    public DateTime? LastConnected
    {
      get => this.lastConnected;
      set
      {
        DateTime? nullable = value;
        DateTime? lastConnected = this.lastConnected;
        if ((nullable.HasValue == lastConnected.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != lastConnected.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.lastConnected = value;
        this.NotifyPropertyChanged(nameof (LastConnected));
      }
    }

    public string Location
    {
      get => this.location;
      set
      {
        if (!(value != this.location))
          return;
        this.location = value;
        this.NotifyPropertyChanged(nameof (Location));
      }
    }

    public DateTime FirstConnected { get; set; }
  }
}
