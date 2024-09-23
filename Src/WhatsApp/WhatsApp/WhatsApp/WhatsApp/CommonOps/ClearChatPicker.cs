// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.ClearChatPicker
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace WhatsApp.CommonOps
{
  public static class ClearChatPicker
  {
    public static IObservable<Unit> Launch(string jid)
    {
      return ClearChatPicker.Launch(new string[1]{ jid });
    }

    public static IObservable<Unit> Launch(string[] jids, string jidIntended = null)
    {
      return jids == null || !((IEnumerable<string>) jids).Any<string>() ? Observable.Empty<Unit>() : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        if (jids.Length > 1)
        {
          List<string> qualifiedJids = ((IEnumerable<string>) jids).Where<string>((Func<string, bool>) (jid => JidHelper.IsUserJid(jid))).ToList<string>();
          if (qualifiedJids.Any<string>())
            ListPickerPage.Start(qualifiedJids.Select<string, string>((Func<string, string>) (jid => JidHelper.GetPhoneNumber(jid, true))).ToArray<string>(), qualifiedJids.IndexOf(jidIntended), AppResources.ClearChatHistory).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (i => jidIntended = i != -1 ? qualifiedJids[i] : (string) null), (Action) (() =>
            {
              if (jidIntended == null)
                return;
              Deployment.Current.Dispatcher.BeginInvoke((Action) (() => ClearChatPicker.Launch(jidIntended).Subscribe(observer)));
            }));
          else
            observer.OnCompleted();
        }
        else
        {
          string jid = jids[0];
          ListPickerPage.Start(new string[2]
          {
            AppResources.DeleteAllExceptStarred,
            AppResources.DeleteAllMessages
          }, title: AppResources.ClearChatHistory).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (i =>
          {
            switch (i)
            {
              case 0:
                ClearChat.Clear(jid, true);
                break;
              case 1:
                ClearChat.Clear(jid, false);
                break;
            }
            observer.OnCompleted();
          }));
        }
        return (Action) (() => { });
      }));
    }
  }
}
