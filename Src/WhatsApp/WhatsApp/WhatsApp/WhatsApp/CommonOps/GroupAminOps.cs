// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.GroupAminOps
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WhatsApp.WaCollections;


namespace WhatsApp.CommonOps
{
  public static class GroupAminOps
  {
    public static void RemoveParticipant(
      string groupJid,
      UserStatus user,
      GlobalProgressIndicator progressIndicator)
    {
      Observable.Return<bool>(true).Decision(string.Format(AppResources.RemoveFromGroupConfirm, (object) user.GetDisplayName())).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (!confirmed)
          return;
        GroupAminOps.RemoveParticipantImpl(groupJid, user.Jid, progressIndicator);
      }));
    }

    private static void RemoveParticipantImpl(
      string groupJid,
      string participantJid,
      GlobalProgressIndicator progressIndicator)
    {
      if (progressIndicator != null)
        progressIndicator.Acquire();
      Action release = (Action) (() =>
      {
        release = (Action) (() => { });
        if (progressIndicator == null)
          return;
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => progressIndicator?.Release()));
      });
      App.CurrentApp.Connection.SendRemoveParticipants(groupJid, (IEnumerable<string>) new string[1]
      {
        participantJid
      }, (Action<List<Pair<string, int>>>) (failedParticipants =>
      {
        GroupAminOps.OnRemoveParticipantsErrors(failedParticipants);
        release();
      }), (Action<int>) (errCode =>
      {
        GroupAminOps.OnRemoveParticipantsRequestError(errCode);
        release();
      }));
    }

    public static void MakePaticipantAdmin(
      string groupJid,
      string participantJid,
      GlobalProgressIndicator progressIndicator)
    {
      if (progressIndicator != null)
        progressIndicator.Acquire();
      Action release = (Action) (() =>
      {
        release = (Action) (() => { });
        if (progressIndicator == null)
          return;
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => progressIndicator?.Release()));
      });
      App.CurrentApp.Connection.SendPromoteParticipant(groupJid, (IEnumerable<string>) new string[1]
      {
        participantJid
      }, (Action<List<Pair<string, int>>>) (failedParticipants =>
      {
        Pair<string, int> pair = failedParticipants.FirstOrDefault<Pair<string, int>>();
        Log.l(nameof (GroupAminOps), "make admin | group: {0} user: {1} | {2}", (object) groupJid, (object) participantJid, pair == null ? (object) "success" : (object) string.Format("error: {0}", (object) pair.Second));
        release();
      }), (Action<int>) (errCode =>
      {
        Log.l(nameof (GroupAminOps), "make admin | group: {0} user: {1} | error: {2}", (object) groupJid, (object) participantJid, (object) errCode);
        release();
      }));
    }

    public static void MakePaticipantNotAdmin(
      string groupJid,
      string participantJid,
      GlobalProgressIndicator progressIndicator)
    {
      if (progressIndicator != null)
        progressIndicator.Acquire();
      Action release = (Action) (() =>
      {
        release = (Action) (() => { });
        if (progressIndicator == null)
          return;
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => progressIndicator?.Release()));
      });
      App.CurrentApp.Connection.SendDemoteParticipant(groupJid, (IEnumerable<string>) new string[1]
      {
        participantJid
      }, (Action<List<Pair<string, int>>>) (failedParticipants =>
      {
        Pair<string, int> pair = failedParticipants.FirstOrDefault<Pair<string, int>>();
        Log.l(nameof (GroupAminOps), "make not admin | group: {0} user: {1} | {2}", (object) groupJid, (object) participantJid, pair == null ? (object) "success" : (object) string.Format("error: {0}", (object) pair.Second));
        GroupAminOps.OnDemoteParticipantsErrors(failedParticipants);
        release();
      }), (Action<int>) (errCode =>
      {
        Log.l(nameof (GroupAminOps), "make not admin | group: {0} user: {1} | error: {2}", (object) groupJid, (object) participantJid, (object) errCode);
        release();
      }));
    }

    public static void OnDemoteParticipantsErrors(List<Pair<string, int>> failedParticipants)
    {
      string str = (string) null;
      if (failedParticipants.Count == 1)
      {
        Pair<string, int> pair = failedParticipants.FirstOrDefault<Pair<string, int>>();
        if (pair != null)
          str = pair.Second != 406 ? string.Format("{0} {1}", (object) string.Format(AppResources.GroupDismissParticipantFail, (object) JidHelper.GetDisplayNameForContactJid(pair.First)), (object) AppResources.TryAgainGeneric) : string.Format(AppResources.GroupDemoteCreatorFail, (object) JidHelper.GetDisplayNameForContactJid(pair.First));
      }
      if (str == null)
        return;
      AppState.ClientInstance.ShowMessageBox(str);
    }

    public static void OnRemoveParticipantsErrors(List<Pair<string, int>> failedParticipants)
    {
      string str = (string) null;
      int count = failedParticipants.Count;
      if (count == 1)
      {
        Pair<string, int> pair = failedParticipants.FirstOrDefault<Pair<string, int>>();
        if (pair != null)
        {
          switch (pair.Second)
          {
            case 404:
              break;
            case 406:
              str = string.Format(AppResources.GroupRemoveCreatorFail, (object) JidHelper.GetDisplayNameForContactJid(pair.First));
              break;
            default:
              str = string.Format("{0} {1}", (object) string.Format(AppResources.GroupRemoveParticipantFail, (object) JidHelper.GetDisplayNameForContactJid(pair.First)), (object) AppResources.TryAgainGeneric);
              break;
          }
        }
      }
      else if (count > 1)
      {
        failedParticipants = failedParticipants.Where<Pair<string, int>>((Func<Pair<string, int>, bool>) (p => p.Second != 404)).ToList<Pair<string, int>>();
        if (failedParticipants.Any<Pair<string, int>>())
          str = string.Format("{0} {1}", (object) string.Format(AppResources.GroupRemoveParticipantsFail, (object) string.Join(",", failedParticipants.Select<Pair<string, int>, string>((Func<Pair<string, int>, string>) (p => JidHelper.GetDisplayNameForContactJid(p.First))))), (object) AppResources.TryAgainGeneric);
      }
      if (str == null)
        return;
      AppState.ClientInstance.ShowMessageBox(str);
    }

    public static void OnRemoveParticipantsRequestError(int errCode)
    {
      string str;
      switch (errCode)
      {
        case 401:
        case 403:
          str = AppResources.GroupRemoveParticipantsFailNotAdmin;
          break;
        case 404:
          str = AppResources.GroupRemoveParticipantsFailGroupEnded;
          break;
        default:
          str = string.Format("{0} {1}", (object) AppResources.GroupRemoveParticipantFailGeneric, (object) AppResources.TryAgainGeneric);
          break;
      }
      if (str == null)
        return;
      AppState.ClientInstance.ShowMessageBox(str);
    }
  }
}
