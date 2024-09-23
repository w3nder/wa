// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.MuteChatPicker
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp.CommonOps
{
  public static class MuteChatPicker
  {
    public static IObservable<Unit> Launch(string jid)
    {
      return MuteChatPicker.Launch(new string[1]{ jid });
    }

    public static IObservable<Unit> Launch(string[] jids)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        DateTime? furthestExp = new DateTime?();
        bool sameExps = false;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          Dictionary<string, DateTime> availableMuteExpirations = MuteChatPicker.GetAvailableMuteExpirations(db, jids);
          if (availableMuteExpirations.Any<KeyValuePair<string, DateTime>>())
          {
            DateTime[] array = availableMuteExpirations.Values.Distinct<DateTime>().ToArray<DateTime>();
            sameExps = availableMuteExpirations.Count == jids.Length && array.Length == 1;
            furthestExp = new DateTime?(((IEnumerable<DateTime>) array).Max<DateTime>());
          }
          else
            sameExps = true;
        }));
        if (furthestExp.HasValue & sameExps)
          ListPickerPage.Start(new string[2]
          {
            string.Format(AppResources.Muted, (object) DateTimeUtils.FormatMuteEndTime(DateTimeUtils.FunTimeToPhoneTime(furthestExp.Value))),
            AppResources.Mute0
          }, 0, AppResources.MuteTitle).Subscribe<int>((Action<int>) (selected =>
          {
            if (selected == 1)
              MuteChatPicker.Mute(jids, new TimeSpan?()).SubscribeOn<Unit>((IScheduler) AppState.Worker).Subscribe(observer);
            else
              observer.OnCompleted();
          }));
        else
          ListPickerPage.Start(new string[4]
          {
            AppResources.Mute0,
            AppResources.Mute8,
            AppResources.Mute168,
            AppResources.Mute8670
          }, sameExps ? 0 : -1, AppResources.MuteTitle).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (selected =>
          {
            IObservable<Unit> source = (IObservable<Unit>) null;
            switch (selected)
            {
              case 0:
                source = MuteChatPicker.Mute(jids, new TimeSpan?());
                break;
              case 1:
                source = MuteChatPicker.Mute(jids, new TimeSpan?(TimeSpan.FromHours(8.0)));
                break;
              case 2:
                source = MuteChatPicker.Mute(jids, new TimeSpan?(TimeSpan.FromDays(7.0)));
                break;
              case 3:
                source = MuteChatPicker.Mute(jids, new TimeSpan?(TimeSpan.FromDays(365.0)));
                break;
            }
            if (source == null)
              observer.OnCompleted();
            else
              source.SubscribeOn<Unit>((IScheduler) AppState.Worker).Subscribe(observer);
          }));
        return (Action) (() => { });
      }));
    }

    private static IObservable<Unit> Mute(string[] jids, TimeSpan? duration)
    {
      return MuteChat.Mute(jids, duration, true);
    }

    public static Dictionary<string, DateTime> GetAvailableMuteExpirations(
      MessagesContext db,
      string[] jids)
    {
      Dictionary<string, DateTime> availableMuteExpirations = new Dictionary<string, DateTime>();
      foreach (string jid in jids)
      {
        JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
        if (jidInfo != null && jidInfo.IsMuted())
          availableMuteExpirations[jidInfo.Jid] = jidInfo.MuteExpirationUtc.Value;
      }
      return availableMuteExpirations;
    }
  }
}
