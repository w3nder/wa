// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactStore
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using WhatsApp.ContactClasses;
using WhatsAppCommon;

#nullable disable
namespace WhatsApp
{
  public static class ContactStore
  {
    private static readonly string LogHdr = nameof (ContactStore);
    public static Subject<string[]> ContactsUpdatedSubject = new Subject<string[]>();
    private static IDisposable fullSyncSubscription;
    private static ContactStore.Cache<Contact[]> AllContacts = new ContactStore.Cache<Contact[]>();
    private static Subject<Unit> AppLeavingSubject = new Subject<Unit>();
    private static string WA_ADD_NOTIF = nameof (WA_ADD_NOTIF);
    private static HashSet<string> inFlightSidelistNotificationRequests = new HashSet<string>();

    public static IObservable<Contact[]> GetAllContacts()
    {
      return ContactStore.AllContacts.Get(Observable.CreateWithDisposable<Contact[]>((Func<IObserver<Contact[]>, IDisposable>) (observer =>
      {
        DateTime? tStart = PerformanceTimer.Start(PerformanceTimer.Mode.All);
        return AddressBook.Instance.GetAllContacts().Select<AddressBookSearchArgs, Contact[]>((Func<AddressBookSearchArgs, Contact[]>) (args => args.Results.ToArray<Contact>())).SimpleSubscribeOn<Contact[]>((IScheduler) Scheduler.ThreadPool).SimpleObserveOn<Contact[]>((IScheduler) AppState.Worker).Do<Contact[]>((Action<Contact[]>) (res => PerformanceTimer.End(string.Format("Found {0} contacts", (object) res.Length), tStart))).Subscribe(observer);
      })));
    }

    public static Contact[] GetAllContactsTask()
    {
      DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.All);
      Task<AddressBookSearchArgs> allContactsAsync = AddressBook.Instance.GetAllContactsAsync();
      allContactsAsync.Wait();
      Contact[] array = allContactsAsync.Result.Results.ToArray<Contact>();
      PerformanceTimer.End(string.Format("Found {0} contacts", (object) array.Length), start);
      return array;
    }

    public static void InvalidateDeviceContacts() => ContactStore.AllContacts.Clear();

    public static IObservable<bool> ContactAddedObservable(string jid)
    {
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        ContactStore.InvalidateDeviceContacts();
        ContactSyncManager.EnqueueRequestPhonebookWithRetry(new FunXMPP.Connection.SyncMode?(FunXMPP.Connection.SyncMode.Delta), FunXMPP.Connection.SyncContext.Interactive, (Action<ContactSync.SyncProcessResult>) (errorCode =>
        {
          observer.OnError((Exception) new ContactSync.SyncException(errorCode));
          observer.OnCompleted();
        }), (Action) (() =>
        {
          observer.OnNext(ContactsContext.Instance<bool>((Func<ContactsContext, bool>) (contacts => contacts.GetUserStatus(jid).IsInDeviceContactList)));
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    public static void SyncBackground()
    {
      ContactSyncManager.EnqueueRequestPhonebookWithRetry(new FunXMPP.Connection.SyncMode?(), FunXMPP.Connection.SyncContext.Background);
    }

    public static void SyncReg()
    {
      ContactSyncManager.EnqueueRequestPhonebookNoDuplicateWithRetry(FunXMPP.Connection.SyncMode.Full, FunXMPP.Connection.SyncContext.Registration);
    }

    public static void EnsureSyncRegComplete(
      Action<ContactSync.SyncProcessResult> onError = null,
      Action onSuccess = null)
    {
      if (Settings.LastFullSyncUtc.HasValue)
        onSuccess();
      else
        ContactSyncManager.EnqueueRequestPhonebookNoDuplicate(FunXMPP.Connection.SyncMode.Full, FunXMPP.Connection.SyncContext.Registration, onError, onSuccess);
    }

    public static void EnsureSyncRegCompleteWithRetry()
    {
      if (Settings.LastFullSyncUtc.HasValue)
        return;
      ContactSyncManager.EnqueueRequestPhonebookNoDuplicateWithRetry(FunXMPP.Connection.SyncMode.Full, FunXMPP.Connection.SyncContext.Registration);
    }

    public static void SyncInteractive()
    {
      ContactSyncManager.EnqueueRequestPhonebookWithRetry(new FunXMPP.Connection.SyncMode?(), FunXMPP.Connection.SyncContext.Interactive);
    }

    public static void OnAppLeaving() => ContactStore.AppLeavingSubject.OnNext(new Unit());

    private static void DeleteContact(
      ContactsContext db,
      string jid,
      out WaScheduledTask postSubmitTask)
    {
      postSubmitTask = (WaScheduledTask) null;
      UserStatus userStatus = db.GetUserStatus(jid, false);
      if (userStatus == null)
        return;
      ContactStore.ClearContactDetails(db, userStatus, out postSubmitTask);
    }

    private static void ClearContactDetails(
      ContactsContext db,
      UserStatus user,
      out WaScheduledTask postSubmitTask)
    {
      postSubmitTask = (WaScheduledTask) null;
      if (user == null)
        return;
      user.IsInDeviceContactList = false;
      Settings.StatusRecipientsStateDirty = true;
      WaScheduledTask task = new WaScheduledTask(WaScheduledTask.Types.PostContactRemoved, user.Jid, (byte[]) null, WaScheduledTask.Restrictions.None, new TimeSpan?(TimeSpan.FromDays(7.0)));
      db.InsertScheduledTaskOnSubmit(task);
      postSubmitTask = task;
    }

    public static IObservable<Unit> PerformPostContactRemoved(WaScheduledTask task)
    {
      return task.TaskType != 1002 ? Observable.Empty<Unit>() : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        string jid = task.LookupKey;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
          if (jidInfo == null)
            return;
          jidInfo.IsSuspicious = new bool?();
        }));
        observer.OnNext(new Unit());
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public static void ProcessContactNotificationHash(
      string jid,
      byte[] jidHash,
      FunXMPP.Connection.ContactNotificationType type,
      Action<bool> ack)
    {
      List<string> matchingNumbers = new List<string>();
      if (string.IsNullOrEmpty(jid) && (jidHash == null || jidHash.Length == 0))
        Log.SendCrashLog((Exception) new InvalidDataException("Contact notification with no contact details"), "Contact notification with no contact details", logOnlyForRelease: true);
      else
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          List<string> jids = new List<string>();
          foreach (PhoneNumber allPhoneNumber in db.GetAllPhoneNumbers())
          {
            if (jidHash != null)
            {
              if (allPhoneNumber.Jid != null && ContactStore.IsHashMatching(allPhoneNumber.Jid, jidHash))
              {
                jids.Add(allPhoneNumber.Jid);
                matchingNumbers.Add(allPhoneNumber.RawPhoneNumber);
              }
            }
            else if (jid == allPhoneNumber.Jid)
            {
              jids.Add(jid);
              matchingNumbers.Add(allPhoneNumber.RawPhoneNumber);
            }
          }
          db.MarkNumbersAsNew((IEnumerable<string>) jids);
        }));
      if (matchingNumbers.Any<string>())
      {
        ContactSyncManager.EnqueueRequestNotificationWithRetry(matchingNumbers, type, (Action<ContactSync.SyncProcessResult>) (err => Log.l(ContactStore.LogHdr, "contact notification failed error={0}", (object) err)), (Action) (() => ack(true)));
      }
      else
      {
        Log.l(ContactStore.LogHdr, "No matches found for {0}, {1}.", (object) jid, (object) (jidHash?.Length.ToString() ?? "null"));
        ack(false);
      }
    }

    public static void OnContactNotification(string jid, string phoneNumber, bool added)
    {
      if (added)
      {
        ContactStore.GetAllContacts().ObserveOn<Contact[]>(WAThreadPool.Scheduler).Subscribe<Contact[]>((Action<Contact[]>) (contacts =>
        {
          Contact contact = ((IEnumerable<Contact>) contacts).Where<Contact>((Func<Contact, bool>) (c => c.PhoneNumbers.Where<ContactPhoneNumber>((Func<ContactPhoneNumber, bool>) (pn => pn.PhoneNumber == phoneNumber)).Any<ContactPhoneNumber>())).FirstOrDefault<Contact>();
          if (contact == null)
            return;
          UserStatus user = (UserStatus) null;
          using (IsolatedStorageFile fs = IsolatedStorageFile.GetUserStoreForApplication())
          {
            bool checkedDir = false;
            List<string> toDelete = new List<string>();
            MemoryStream mem = new MemoryStream();
            ContactStore.ContactWithKind nameInfo = new ContactStore.ContactWithKind()
            {
              Contact = contact,
              Kind = contact.PhoneNumbers.Where<ContactPhoneNumber>((Func<ContactPhoneNumber, bool>) (pn => pn.PhoneNumber == phoneNumber)).Select<ContactPhoneNumber, PhoneNumberKind>((Func<ContactPhoneNumber, PhoneNumberKind>) (pn => pn.Kind)).First<PhoneNumberKind>()
            };
            try
            {
              ContactsContext.Instance((Action<ContactsContext>) (db =>
              {
                user = db.GetUserStatus(jid);
                ContactSync.CopyContactDetails(nameInfo, user, mem, fs, ref checkedDir, toDelete, true, true);
                db.SubmitChanges();
              }));
            }
            catch (DatabaseInvalidatedException ex)
            {
              return;
            }
            toDelete.ForEach((Action<string>) (name =>
            {
              try
              {
                fs.DeleteFile(name);
              }
              catch (Exception ex)
              {
              }
            }));
          }
          UsyncQueryRequest.SendUsyncListUserQuery(jid, FunXMPP.Connection.SyncMode.Delta, FunXMPP.Connection.SyncContext.Notification);
        }));
      }
      else
      {
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          UserStatus userStatus = db.GetUserStatus(jid, false);
          if (userStatus == null)
            return;
          WaScheduledTask postSubmitTask = (WaScheduledTask) null;
          ContactStore.ClearContactDetails(db, userStatus, out postSubmitTask);
          db.SubmitChanges();
          db.AttemptScheduledTaskOnThreadPool(postSubmitTask, 500);
        }));
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          Message[] array = ((IEnumerable<WaStatus>) db.GetStatuses(jid, false, true, new TimeSpan?())).Select<WaStatus, Message>((Func<WaStatus, Message>) (s => db.GetMessageById(s.MessageId))).Where<Message>((Func<Message, bool>) (m => m != null)).ToArray<Message>();
          db.DeleteMessages(array);
        }));
      }
    }

    private static bool IsHashMatching(string jid, byte[] jidHash)
    {
      bool flag = true;
      string str = (string) null;
      try
      {
        str = JidHelper.GetPhoneNumber(jid, false);
      }
      catch (Exception ex)
      {
      }
      if (str != null)
      {
        byte[] hash = MD5Core.GetHash(str + ContactStore.WA_ADD_NOTIF);
        for (int index = 0; index < jidHash.Length; ++index)
        {
          if ((int) jidHash[index] != (int) hash[index])
          {
            flag = false;
            break;
          }
        }
      }
      return flag;
    }

    public static void ProcessSidelistNotification(
      string jidHashB64,
      UsyncQuery.UsyncProtocol usyncProtocol,
      Action<bool> ack)
    {
      if (!Settings.UsyncSidelist)
      {
        ack(true);
      }
      else
      {
        lock (ContactStore.inFlightSidelistNotificationRequests)
        {
          if (ContactStore.inFlightSidelistNotificationRequests.Contains(jidHashB64))
          {
            Log.d(ContactStore.LogHdr, "Throttled sidelist notification for {0}, request already in-flight", (object) jidHashB64);
            ack(true);
            return;
          }
          ContactStore.inFlightSidelistNotificationRequests.Add(jidHashB64);
        }
        AppState.Worker.Enqueue((Action) (() =>
        {
          DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.All);
          bool flag = false;
          try
          {
            byte[] jidHash = Convert.FromBase64String(jidHashB64);
            HashSet<string> checkConversationsJids = ContactSync.GetConversations();
            Dictionary<string, UserStatus> sidelistedJids = (Dictionary<string, UserStatus>) null;
            ContactsContext.Instance((Action<ContactsContext>) (db => ContactSync.GetAllUserDetails(db, true, checkConversationsJids, out Dictionary<string, UserStatus> _, out sidelistedJids, out Dictionary<string, UserStatus> _, out Dictionary<string, UserStatus> _)));
            string jid = (string) null;
            foreach (KeyValuePair<string, UserStatus> keyValuePair in sidelistedJids ?? new Dictionary<string, UserStatus>())
            {
              if (ContactStore.IsHashMatching(keyValuePair.Key, jidHash))
              {
                jid = keyValuePair.Key;
                break;
              }
            }
            if (jid != null)
            {
              UsyncQueryRequest.SendNotificationQueryForJid(jid);
              flag = true;
            }
            else
              Log.d(ContactStore.LogHdr, "SidelistJid not found");
          }
          catch (Exception ex)
          {
            Log.SendCrashLog(ex, "Exception processing SidelistNotification", logOnlyForRelease: true);
          }
          lock (ContactStore.inFlightSidelistNotificationRequests)
            ContactStore.inFlightSidelistNotificationRequests.Remove(jidHashB64);
          ack(flag);
          PerformanceTimer.End(nameof (ProcessSidelistNotification), start);
        }));
      }
    }

    public class ContactWithKind
    {
      public Contact Contact;
      public PhoneNumberKind Kind;
    }

    public class ContactWithKindAndJid
    {
      public Contact Contact;
      public PhoneNumberKind Kind;
      public UserStatus Status;
      public bool IsWAUser;
    }

    public class Cache<T>
    {
      private object @lock = new object();
      private T value;
      private List<IObserver<T>> observers = new List<IObserver<T>>();
      private bool active;

      public void Clear() => this.value = default (T);

      public IObservable<T> Get(IObservable<T> source)
      {
        return Observable.Create<T>((Func<IObserver<T>, Action>) (observer =>
        {
          T obj = default (T);
          bool flag = false;
          lock (this.@lock)
          {
            if ((object) this.value != null)
              obj = this.value;
            else if (this.active)
            {
              this.observers.Add(observer);
            }
            else
            {
              this.active = true;
              flag = true;
            }
          }
          if ((object) obj != null)
          {
            observer.OnNext(this.value);
            observer.OnCompleted();
            return (Action) (() => { });
          }
          if (flag)
          {
            Action<Action<IObserver<T>>> deliver = (Action<Action<IObserver<T>>>) (callback =>
            {
              lock (this.@lock)
              {
                this.active = false;
                callback(observer);
                while (this.observers.Count != 0)
                {
                  IObserver<T>[] array = this.observers.ToArray();
                  this.observers.Clear();
                  foreach (IObserver<T> observer1 in array)
                    callback(observer1);
                }
              }
            });
            source.Take<T>(1).Subscribe<T>((Action<T>) (_ =>
            {
              this.value = _;
              deliver((Action<IObserver<T>>) (o => o.OnNext(_)));
            }), (Action<Exception>) (ex => deliver((Action<IObserver<T>>) (o => o.OnError(ex)))));
          }
          return (Action) (() =>
          {
            lock (this.@lock)
              this.observers.Remove(observer);
          });
        }));
      }
    }
  }
}
