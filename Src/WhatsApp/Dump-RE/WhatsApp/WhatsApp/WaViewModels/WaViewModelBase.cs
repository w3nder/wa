// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.WaViewModelBase
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public abstract class WaViewModelBase : WaDisposable, INotifyPropertyChanged
  {
    protected double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
    private Subject<KeyValuePair<string, object>> nofiySubj;

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      PropChangedHandlers.NotifyPropertyChanged((Action<PropertyChangedEventArgs>) (e =>
      {
        if (this.PropertyChanged == null)
          return;
        this.PropertyChanged((object) this, e);
      }), propertyName);
    }

    protected void Notify(string k, object tag = null)
    {
      if (this.nofiySubj == null)
        return;
      this.nofiySubj.OnNext(new KeyValuePair<string, object>(k, tag));
    }

    public virtual IObservable<KeyValuePair<string, object>> GetObservable()
    {
      return (IObservable<KeyValuePair<string, object>>) this.nofiySubj ?? (IObservable<KeyValuePair<string, object>>) (this.nofiySubj = new Subject<KeyValuePair<string, object>>());
    }
  }
}
