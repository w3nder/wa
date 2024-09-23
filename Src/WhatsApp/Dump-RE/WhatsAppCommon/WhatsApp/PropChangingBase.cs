// Decompiled with JetBrains decompiler
// Type: WhatsApp.PropChangingBase
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.ComponentModel;

#nullable disable
namespace WhatsApp
{
  public class PropChangingBase : INotifyPropertyChanging
  {
    public event PropertyChangingEventHandler PropertyChanging;

    public void NotifyPropertyChanging(string prop)
    {
      PropChangedHandlers.NotifyPropertyChanging((Action<PropertyChangingEventArgs>) (ev =>
      {
        if (this.PropertyChanging == null)
          return;
        this.PropertyChanging((object) this, ev);
      }), prop);
    }
  }
}
