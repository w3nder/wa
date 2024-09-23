// Decompiled with JetBrains decompiler
// Type: WhatsApp.PropChangedHandlers
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.ComponentModel;
using System.Windows;

#nullable disable
namespace WhatsApp
{
  public static class PropChangedHandlers
  {
    public static void NotifyPropertyChanging(
      Action<PropertyChangingEventArgs> callback,
      string prop)
    {
      try
      {
        callback(new PropertyChangingEventArgs(prop));
      }
      catch (Exception ex)
      {
        string context = string.Format("PropertyChanging({0}) hit exception", (object) prop);
        Log.LogException(ex, context);
        throw;
      }
    }

    public static void NotifyPropertyChanged(Action<PropertyChangedEventArgs> callback, string prop)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        try
        {
          callback(new PropertyChangedEventArgs(prop));
        }
        catch (Exception ex)
        {
          string context = string.Format("PropertyChanged({0}) hit exception", (object) prop);
          Log.LogException(ex, context);
          throw;
        }
      }));
    }
  }
}
