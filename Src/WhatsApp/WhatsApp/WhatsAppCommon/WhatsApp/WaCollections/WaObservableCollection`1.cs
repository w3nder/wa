// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCollections.WaObservableCollection`1
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;


namespace WhatsApp.WaCollections
{
  public class WaObservableCollection<T> : ObservableCollection<T>
  {
    private bool notify_ = true;

    public WaObservableCollection()
    {
    }

    public WaObservableCollection(IEnumerable<T> collection)
      : base(collection)
    {
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (!this.notify_)
        return;
      base.OnCollectionChanged(e);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (!this.notify_)
        return;
      base.OnPropertyChanged(e);
    }

    public void SuppressNotification() => this.notify_ = false;

    public void ResumeNotification()
    {
      if (this.notify_)
        return;
      this.notify_ = true;
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public void AddRange(IEnumerable<T> items)
    {
      this.SuppressNotification();
      try
      {
        foreach (T obj in items)
          this.Add(obj);
      }
      finally
      {
        this.ResumeNotification();
      }
    }

    public void InsertRange(int index, IEnumerable<T> items)
    {
      this.SuppressNotification();
      try
      {
        int num = 0;
        foreach (T obj in items)
        {
          this.InsertItem(index + num, obj);
          ++num;
        }
      }
      finally
      {
        this.ResumeNotification();
      }
    }
  }
}
