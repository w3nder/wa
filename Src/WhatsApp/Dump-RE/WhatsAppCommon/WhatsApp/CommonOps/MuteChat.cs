// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.MuteChat
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.Events;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class MuteChat
  {
    public static IObservable<Unit> Mute(string jid, DateTime? muteExp, bool notifyQr)
    {
      return MuteChat.MuteAsync(new string[1]{ jid }, muteExp, notifyQr);
    }

    public static IObservable<Unit> Mute(string jid, TimeSpan? duration, bool notifyQr)
    {
      return MuteChat.Mute(new string[1]{ jid }, duration, notifyQr);
    }

    public static IObservable<Unit> Mute(string[] jids, TimeSpan? duration, bool notifyQr)
    {
      DateTime? muteExp = new DateTime?();
      if (duration.HasValue && duration.Value.TotalMinutes > 0.0)
        muteExp = new DateTime?(FunRunner.CurrentServerTimeUtc.Add(duration.Value));
      return MuteChat.MuteAsync(jids, muteExp, notifyQr);
    }

    private static IObservable<Unit> MuteAsync(string[] jids, DateTime? muteExp, bool notifyQr)
    {
      return jids == null || !((IEnumerable<string>) jids).Any<string>() ? Observable.Empty<Unit>() : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        MuteChat.MuteSynchronous(jids, muteExp, notifyQr);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public static void MuteSynchronous(string[] jids, DateTime? muteExp, bool notifyQr)
    {
      if (jids == null || !((IEnumerable<string>) jids).Any<string>())
        return;
      if (muteExp.HasValue)
      {
        DateTime? nullable = muteExp;
        DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() < currentServerTimeUtc ? 1 : 0) : 0) != 0)
          muteExp = new DateTime?();
      }
      MuteChat.SaveMuteExpirations(jids, muteExp);
      if (notifyQr)
      {
        foreach (string jid in jids)
          AppState.QrPersistentAction.NotifyChatStatus(jid, FunXMPP.ChatStatusForwardAction.Mute, muteExp);
      }
      Log.l("common ops", "{0}muted | exp:{1},jids:{2}", muteExp.HasValue ? (object) "" : (object) "un", muteExp.HasValue ? (object) muteExp.ToString() : (object) "", (object) string.Join(",", jids));
    }

    public static void SaveMuteExpirations(string[] jids, DateTime? muteExp)
    {
      Dictionary<string, List<string>> possiblyAddToFieldStats = new Dictionary<string, List<string>>();
      Action<bool, string, string> AddToFieldstatsCollection = (Action<bool, string, string>) ((settingHasChanged, keyJid, jidToCheck) =>
      {
        if (!settingHasChanged)
          return;
        List<string> stringList = (List<string>) null;
        if (!possiblyAddToFieldStats.TryGetValue(keyJid, out stringList))
        {
          stringList = new List<string>();
          possiblyAddToFieldStats[keyJid] = stringList;
        }
        stringList.Add(jidToCheck);
      });
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (string jid in jids)
        {
          JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.CreateToDbIfNotFound);
          bool flag = false;
          if (jidInfo != null)
          {
            DateTime? muteExpirationUtc = jidInfo.MuteExpirationUtc;
            DateTime? nullable = muteExp;
            flag = muteExpirationUtc.HasValue != nullable.HasValue || muteExpirationUtc.HasValue && muteExpirationUtc.GetValueOrDefault() != nullable.GetValueOrDefault();
            jidInfo.MuteExpirationUtc = muteExp;
            AddToFieldstatsCollection(flag, jid, jid);
          }
          Conversation conversation = db.GetConversation(jid, CreateOptions.None);
          if (conversation != null)
          {
            conversation.MuteExpiration = muteExp;
            if (JidHelper.IsGroupJid(jid))
            {
              string groupOwner = conversation.GroupOwner;
              AddToFieldstatsCollection(flag, jid, groupOwner);
              foreach (string participantJid in conversation.GetParticipantJids())
              {
                if (participantJid != groupOwner && conversation.UserIsAdmin(participantJid))
                  AddToFieldstatsCollection(flag, jid, participantJid);
              }
            }
          }
        }
        db.SubmitChanges();
      }));
      ContactsContext.Instance((Action<ContactsContext>) (cdb =>
      {
        foreach (string key in possiblyAddToFieldStats.Keys)
        {
          bool flag = false;
          foreach (string jid in possiblyAddToFieldStats[key])
          {
            if (JidHelper.IsUserJid(jid) && cdb.GetUserStatus(jid).IsVerified())
            {
              flag = true;
              break;
            }
          }
          if (flag)
          {
            if ((!muteExp.HasValue ? 0 : (muteExp.Value > DateTime.UtcNow ? 1 : 0)) != 0)
              new BusinessMute()
              {
                muteeId = key,
                muteT = new long?(muteExp.Value.ToUnixTime())
              }.SaveEvent();
            else
              new BusinessUnmute() { muteeId = key }.SaveEvent();
          }
        }
      }));
    }
  }
}
