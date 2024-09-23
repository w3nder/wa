// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.AddContact
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Windows;
using System.Windows.Threading;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class AddContact
  {
    public static void Launch(
      string jid,
      bool toExisting,
      Action onStarted = null,
      Action onFinished = null,
      Action onSuccess = null)
    {
      Dispatcher dispatcher = Deployment.Current.Dispatcher;
      if (!dispatcher.CheckAccess())
      {
        dispatcher.BeginInvoke((Action) (() => AddContact.Launch(jid, toExisting, onStarted, onFinished, onSuccess)));
      }
      else
      {
        if (string.IsNullOrEmpty(jid) || !jid.IsUserJid())
          return;
        int num = jid.IndexOf('@');
        if (num < 0)
          return;
        if (onStarted == null)
          onStarted = (Action) (() => { });
        if (onFinished == null)
          onFinished = (Action) (() => { });
        if (onSuccess == null)
          onSuccess = (Action) (() => { });
        int startIndex = jid[0] == '+' ? 1 : 0;
        string str = jid.Substring(startIndex, num - startIndex);
        Action<TaskEventArgs> taskResultHandler = (Action<TaskEventArgs>) (result =>
        {
          if (result.TaskResult == TaskResult.OK)
            onStarted();
          ContactStore.ContactAddedObservable(jid).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (inDb =>
          {
            onFinished();
            App.SuppressSync = false;
            if (!inDb)
              return;
            onSuccess();
          }), (Action<Exception>) (ex =>
          {
            Log.LogException(ex, "sync after contact added");
            onFinished();
            App.SuppressSync = false;
          }));
        });
        Action action;
        if (toExisting)
        {
          SavePhoneNumberTask savePhoneNumberTask = new SavePhoneNumberTask();
          savePhoneNumberTask.PhoneNumber = string.Format("+{0}", (object) str);
          savePhoneNumberTask.Completed += (EventHandler<TaskEventArgs>) ((sender, result) => taskResultHandler(result));
          // ISSUE: virtual method pointer
          action = new Action((object) savePhoneNumberTask, __vmethodptr(savePhoneNumberTask, Show));
        }
        else
        {
          SaveContactTask saveContactTask = new SaveContactTask();
          string verifiedName = (string) null;
          ContactsContext.Instance((Action<ContactsContext>) (db =>
          {
            UserStatus userStatus = db.GetUserStatus(jid, false);
            verifiedName = userStatus != null ? userStatus.GetVerifiedNameForDisplay() : (string) null;
          }));
          if (!string.IsNullOrEmpty(verifiedName))
            saveContactTask.Company = verifiedName;
          saveContactTask.FirstName = "";
          saveContactTask.MobilePhone = string.Format("+{0}", (object) str);
          saveContactTask.Completed += (EventHandler<SaveContactResult>) ((sender, result) => taskResultHandler((TaskEventArgs) result));
          action = new Action(((ChooserBase<SaveContactResult>) saveContactTask).Show);
        }
        App.SuppressSync = true;
        action();
      }
    }
  }
}
