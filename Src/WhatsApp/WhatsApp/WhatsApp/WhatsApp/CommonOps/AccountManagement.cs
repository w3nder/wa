// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.AccountManagement
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
  public static class AccountManagement
  {
    public static void ChangePhoneNumber()
    {
      FunXMPP.Connection conn = AppState.ClientInstance.GetConnection();
      string oldnum = PhoneNumberFormatter.FormatInternationalNumber(Settings.OldChatID);
      string newnum = PhoneNumberFormatter.FormatInternationalNumber(Settings.ChatID);
      List<string> numberNotifyJids = Settings.ChangeNumberNotifyJids;
      conn.SendChangeNumber(Settings.OldChatID, (IEnumerable<string>) numberNotifyJids).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        Log.l("change number", "success | {0} -> {1}", (object) oldnum, (object) newnum);
        AccountManagement.AfterChangeNumber(conn);
        string oldChatId = Settings.OldChatID;
        Settings.OldChatID = (string) null;
        int num = (int) MessageBox.Show(string.Format(AppResources.ChangeNumberSuccess, (object) oldnum, (object) newnum));
      }), (Action<Exception>) (ex =>
      {
        Log.l("change number", "returned error {0}", (object) ex.Message);
        string oldChatId = Settings.OldChatID;
        Settings.OldChatID = (string) null;
        int int32 = Convert.ToInt32(ex.Message);
        string messageBoxText;
        switch (int32)
        {
          case 400:
            messageBoxText = AppResources.ChangeNumberFailSameNumber;
            break;
          case 401:
            messageBoxText = string.Format(AppResources.ChangeNumberFailOldNumberTaken, (object) oldnum, (object) newnum);
            break;
          case 405:
            messageBoxText = (string) null;
            break;
          default:
            if (int32 < 500 && int32 != 409)
            {
              messageBoxText = string.Format(AppResources.ChangeNumberFailOther, (object) oldnum, (object) newnum);
              break;
            }
            messageBoxText = (string) null;
            Settings.OldChatID = oldChatId;
            break;
        }
        AccountManagement.AfterChangeNumber(conn);
        if (string.IsNullOrEmpty(messageBoxText))
          return;
        int num = (int) MessageBox.Show(messageBoxText);
      }));
    }

    public static List<string> GetChangeNumberNotifyRecipients(bool withChatsOnly)
    {
      List<string> jidsToNotify = (List<string>) null;
      if (withChatsOnly)
      {
        List<Conversation> convos = (List<Conversation>) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => convos = db.GetConversations(new JidHelper.JidTypes[1]
        {
          JidHelper.JidTypes.User
        }, true)));
        HashSet<string> jidsSet = new HashSet<string>();
        foreach (Conversation conversation in convos)
          jidsSet.Add(conversation.Jid);
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          Dictionary<string, bool> blocked = db.BlockListSet;
          jidsToNotify = db.GetWaContactJids(false).Where<string>((Func<string, bool>) (jid => jidsSet.Contains(jid) && !blocked.ContainsKey(jid))).ToList<string>();
        }));
      }
      else
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          Dictionary<string, bool> blocked = db.BlockListSet;
          jidsToNotify = db.GetWaContactJids(false).Where<string>((Func<string, bool>) (jid => !blocked.ContainsKey(jid))).ToList<string>();
        }));
      return jidsToNotify;
    }

    public static void AfterChangeNumber(FunXMPP.Connection conn)
    {
      conn.Encryption.Reset();
      conn.SendGetGroups();
      conn.SendGetPrivacyList();
      AppState.SchedulePersistentAction(PersistentAction.SendVerifyAxolotlDigest());
    }

    public static void AbortChangePhoneNumber()
    {
      if (Settings.OldChatID == null)
        return;
      Settings.ChatID = Settings.OldChatID;
      Settings.PhoneNumber = Settings.OldPhoneNumber;
      Settings.CountryCode = Settings.OldCountryCode;
      Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.Verified;
      Settings.OldChatID = (string) null;
      Settings.OldPhoneNumber = (string) null;
      Settings.OldCountryCode = (string) null;
      Settings.SuppressRestoreFromBackupAtReg = false;
      App.CurrentApp.ConnectionResetSubject.OnNext(new Unit());
    }
  }
}
