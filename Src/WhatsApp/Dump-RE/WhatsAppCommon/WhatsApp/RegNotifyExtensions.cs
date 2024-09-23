// Decompiled with JetBrains decompiler
// Type: WhatsApp.RegNotifyExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class RegNotifyExtensions
  {
    private static Dictionary<string, RegNotifyExtensions.NotifyHelper> helpers = new Dictionary<string, RegNotifyExtensions.NotifyHelper>();
    private static object helpersLock = new object();

    public static IDisposable NotifyChanged(
      this IRegHelper reg,
      uint key,
      string subkey,
      Action callback)
    {
      string dictKey = key.ToString() + " " + subkey.ToLowerInvariant();
      RegNotifyExtensions.NotifyHelper helper;
      lock (RegNotifyExtensions.helpersLock)
      {
        if (!RegNotifyExtensions.helpers.TryGetValue(dictKey, out helper))
        {
          helper = new RegNotifyExtensions.NotifyHelper();
          Action a = (Action) (() =>
          {
            Action[] array;
            lock (RegNotifyExtensions.helpersLock)
              array = helper.Callbacks.ToArray();
            foreach (Action action in array)
              action();
          });
          helper.Dtor = reg.NotifyChangedImpl(key, subkey, a.AsComAction());
          RegNotifyExtensions.helpers.Add(dictKey, helper);
        }
        helper.Callbacks.Add(callback);
      }
      return (IDisposable) new DisposableAction((Action) (() =>
      {
        lock (RegNotifyExtensions.helpersLock)
        {
          helper.Callbacks.Remove(callback);
          if (helper.Callbacks.Count != 0)
            return;
          RegNotifyExtensions.helpers.Remove(dictKey);
          helper.Dtor.Perform();
        }
      }));
    }

    public class NotifyHelper
    {
      public IAction Dtor;
      public List<Action> Callbacks = new List<Action>();
    }
  }
}
