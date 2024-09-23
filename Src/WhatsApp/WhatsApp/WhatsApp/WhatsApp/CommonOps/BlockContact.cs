// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.BlockContact
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
  public static class BlockContact
  {
    private static Subject<Unit> blockListChangedSubject = new Subject<Unit>();

    public static Subject<Unit> BlockListChangedSubject => BlockContact.blockListChangedSubject;

    public static bool OnBlockDialogSelection(
      Pair<bool, bool> selection,
      string reportSpamSource,
      string jid,
      Message[] msgsToReport = null)
    {
      bool first = selection.First;
      bool second = selection.Second;
      if (first & second)
      {
        DeleteChat.Delete(jid);
        Deployment.Current.Dispatcher.BeginInvoke(new Action(NavUtils.NavigateHome));
        App.CurrentApp.Connection.InvokeWhenConnected((Action) (() =>
        {
          try
          {
            App.CurrentApp.Connection.SendSpamReport(msgsToReport, reportSpamSource, jid);
          }
          catch (Exception ex)
          {
            string context = string.Format("Sending spam report for {0}", (object) jid);
            Log.LogException(ex, context);
          }
        }));
        return true;
      }
      if (reportSpamSource == "block_dialog")
      {
        int num = (int) MessageBox.Show(first ? AppResources.ContactBlocked : AppResources.ContactUnblocked);
      }
      return false;
    }

    public static IObservable<Pair<bool, bool>> Block(string jid)
    {
      return Observable.Create<Pair<bool, bool>>((Func<IObserver<Pair<bool, bool>>, Action>) (observer =>
      {
        string contactName = (string) null;
        ContactsContext.Instance((Action<ContactsContext>) (cdb => contactName = cdb.GetUserStatus(jid).GetDisplayName(true)));
        string message = string.Format(AppResources.ConfirmBlock, string.IsNullOrEmpty(contactName) ? (object) JidHelper.GetPhoneNumber(jid, true) : (object) contactName);
        Observable.Return<bool>(true).Decisions(message, new string[3]
        {
          AppResources.Block,
          AppResources.ReportAndBlock,
          AppResources.Cancel
        }).Take<int>(1).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (result =>
        {
          if (result == 0)
            BlockContact.BlockImpl(observer, jid, false);
          else if (result == 1)
            BlockContact.ReportSpamImpl(observer, jid);
          else
            observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    public static IObservable<Pair<bool, bool>> ReportSpam(string jid)
    {
      return Observable.Create<Pair<bool, bool>>((Func<IObserver<Pair<bool, bool>>, Action>) (observer =>
      {
        BlockContact.ReportSpamImpl(observer, jid);
        return (Action) (() => { });
      }));
    }

    private static void ReportSpamImpl(IObserver<Pair<bool, bool>> observer, string jid)
    {
      string contactName = (string) null;
      ContactsContext.Instance((Action<ContactsContext>) (cdb => contactName = cdb.GetUserStatus(jid).GetDisplayName(true)));
      string message = string.Format(AppResources.ConfirmReportAndBlockNoHistory, string.IsNullOrEmpty(contactName) ? (object) JidHelper.GetPhoneNumber(jid, true) : (object) contactName);
      string reportAndBlock = AppResources.ReportAndBlock;
      Observable.Return<bool>(true).Decision(message, reportAndBlock, AppResources.CancelButton).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (confirmed)
        {
          BlockContact.BlockImpl(observer, jid, true);
        }
        else
        {
          observer.OnNext(new Pair<bool, bool>(false, false));
          observer.OnCompleted();
        }
      }));
    }

    private static void BlockImpl(
      IObserver<Pair<bool, bool>> observer,
      string jid,
      bool reportSpam)
    {
      Action onError = (Action) (() =>
      {
        observer.OnNext(new Pair<bool, bool>(false, false));
        observer.OnCompleted();
        BlockContact.ShowBlockListError();
      });
      Dictionary<string, string> jidsToBlock = (Dictionary<string, string>) null;
      ContactsContext.Instance((Action<ContactsContext>) (cdb =>
      {
        jidsToBlock = cdb.BlockListSet.ToDictionary<KeyValuePair<string, bool>, string, string>((Func<KeyValuePair<string, bool>, string>) (p => p.Key), (Func<KeyValuePair<string, bool>, string>) (p => (string) null));
        if (!reportSpam && jidsToBlock.ContainsKey(jid))
          return;
        jidsToBlock[jid] = reportSpam ? "spam" : (string) null;
      }));
      if (App.CurrentApp.Connection.IsConnected)
      {
        Log.d("block contact", "Blocking:{0}, ReportSpam:{1}", (object) jid, (object) reportSpam);
        LiveLocationManager.Instance.ProcessUserBlock(jid);
        App.CurrentApp.Connection.SendSetBlockList(jidsToBlock, (Action) (() =>
        {
          string[] jidsToBlockArray = jidsToBlock.Keys.ToArray<string>();
          ContactsContext.Instance((Action<ContactsContext>) (cdb =>
          {
            cdb.BlockListSet.Clear();
            foreach (string key in jidsToBlockArray)
              cdb.BlockListSet.Add(key, true);
            cdb.FlushBlockList();
            cdb.SubmitChanges();
          }));
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.ClearWaStatuses(jidsToBlockArray)));
          observer.OnNext(new Pair<bool, bool>(jidsToBlock.ContainsKey(jid), reportSpam));
          observer.OnCompleted();
          BlockContact.BlockListChangedSubject.OnNext(new Unit());
        }), (Action<int>) (errCode => onError()));
      }
      else
        onError();
    }

    private static IObservable<bool> Unblock(string jid, string confirmBody = null)
    {
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        Dictionary<string, string> jidsToBlock = (Dictionary<string, string>) null;
        string contactName = (string) null;
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          jidsToBlock = cdb.BlockListSet.ToDictionary<KeyValuePair<string, bool>, string, string>((Func<KeyValuePair<string, bool>, string>) (p => p.Key), (Func<KeyValuePair<string, bool>, string>) (p => (string) null));
          if (jidsToBlock.ContainsKey(jid))
            jidsToBlock.Remove(jid);
          contactName = cdb.GetUserStatus(jid).GetDisplayName(true);
        }));
        if (confirmBody == null)
          confirmBody = string.Format(AppResources.ConfirmUnblockBody, string.IsNullOrEmpty(contactName) ? (object) JidHelper.GetPhoneNumber(jid, true) : (object) contactName);
        Action onError = (Action) (() =>
        {
          observer.OnNext(false);
          observer.OnCompleted();
          BlockContact.ShowBlockListError();
        });
        UIUtils.Decision(confirmBody, AppResources.UnblockContact, AppResources.CancelButton, AppResources.ConfirmUnblockTitle).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
        {
          if (confirmed)
          {
            if (App.CurrentApp.Connection.IsConnected)
              App.CurrentApp.Connection.SendSetBlockList(jidsToBlock, (Action) (() =>
              {
                ContactsContext.Instance((Action<ContactsContext>) (cdb =>
                {
                  cdb.BlockListSet.Clear();
                  foreach (KeyValuePair<string, string> keyValuePair in jidsToBlock)
                    cdb.BlockListSet.Add(keyValuePair.Key, true);
                  cdb.FlushBlockList();
                  cdb.SubmitChanges();
                }));
                observer.OnNext(!jidsToBlock.ContainsKey(jid));
                observer.OnCompleted();
                BlockContact.BlockListChangedSubject.OnNext(new Unit());
              }), (Action<int>) (errCode => onError()));
            else
              onError();
          }
          else
          {
            observer.OnNext(false);
            observer.OnCompleted();
          }
        }));
        return (Action) (() => { });
      }));
    }

    private static void ShowBlockListError()
    {
      int num;
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() => num = (int) MessageBox.Show(string.Format("{0}\n{1}", (object) AppResources.BlockListError, (object) AppResources.TryAgainGeneric))));
    }

    public static IObservable<Pair<bool, bool>> ToggleBlock(string jid, string reportSpamSource = "block_dialog")
    {
      bool toBlockedState = false;
      ContactsContext.Instance((Action<ContactsContext>) (cdb =>
      {
        if (cdb.BlockListSet.ContainsKey(jid))
          toBlockedState = false;
        else
          toBlockedState = true;
      }));
      if (reportSpamSource != "block_dialog")
        return BlockContact.ReportSpam(jid);
      return !toBlockedState ? BlockContact.Unblock(jid).Where<bool>((Func<bool, bool>) (unblocked => unblocked)).Select<bool, Pair<bool, bool>>((Func<bool, Pair<bool, bool>>) (unblocked => new Pair<bool, bool>(!unblocked, false))) : BlockContact.Block(jid).Where<Pair<bool, bool>>((Func<Pair<bool, bool>, bool>) (blocked => blocked.First));
    }

    public static IObservable<bool> PromptUnblockIfBlocked(string jid, string promptBody = null)
    {
      if (jid == null)
        return Observable.Return<bool>(false);
      if (!JidHelper.IsUserJid(jid))
        return Observable.Return<bool>(true);
      UserStatus blockedContact = (UserStatus) null;
      ContactsContext.Instance((Action<ContactsContext>) (cdb =>
      {
        if (!cdb.BlockListSet.ContainsKey(jid))
          return;
        blockedContact = cdb.GetUserStatus(jid);
      }));
      return blockedContact == null ? Observable.Return<bool>(true) : BlockContact.Unblock(jid, promptBody);
    }

    public static void SetBlockedContacts(string[] jidsToBlock)
    {
      FunXMPP.Connection connection = App.CurrentApp.Connection;
      if (connection == null)
        return;
      if (connection.IsConnected)
        connection.SendSetBlockList(jidsToBlock, (Action) (() =>
        {
          ContactsContext.Instance((Action<ContactsContext>) (cdb =>
          {
            cdb.BlockListSet.Clear();
            foreach (string key in jidsToBlock)
              cdb.BlockListSet[key] = true;
            cdb.FlushBlockList();
            cdb.SubmitChanges();
          }));
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.ClearWaStatuses(jidsToBlock)));
          BlockContact.BlockListChangedSubject.OnNext(new Unit());
        }), (Action<int>) (errCode => BlockContact.ShowBlockListError()));
      else
        BlockContact.ShowBlockListError();
    }
  }
}
