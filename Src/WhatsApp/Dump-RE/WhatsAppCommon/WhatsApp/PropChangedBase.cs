// Decompiled with JetBrains decompiler
// Type: WhatsApp.PropChangedBase
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.ComponentModel;

#nullable disable
namespace WhatsApp
{
  public class PropChangedBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string prop)
    {
      PropChangedHandlers.NotifyPropertyChanged((Action<PropertyChangedEventArgs>) (ev =>
      {
        if (this.PropertyChanged == null)
          return;
        this.PropertyChanged((object) this, ev);
      }), prop);
    }
  }
}
