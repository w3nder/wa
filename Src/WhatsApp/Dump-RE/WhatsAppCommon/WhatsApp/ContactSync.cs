// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactSync
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WhatsApp.ContactClasses;
using WhatsApp.WaCollections;
using WhatsAppCommon;

#nullable disable
namespace WhatsApp
{
  public class ContactSync
  {
    private static readonly string LogHdr = nameof (ContactSync);
    private static readonly int thresholdForAddressbookSizeLogging = 50;
    private static readonly int waitForIQResponseTimeoutMs = 20000;
    private static readonly int waitForDeltaResponseParseTimeoutMs = 30000;
    private static readonly int waitForFullResponseParseTimeoutMs = 120000;
    private static readonly TaskFactory taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
    private static TimeSpan ioTime = TimeSpan.FromSeconds(0.0);
    private static TimeSpan hashTime = TimeSpan.FromSeconds(0.0);
    private static bool sentCrashlogOnNullUser = false;

    public static bool IsSuccessSyncError(ContactSync.SyncProcessResult err)
    {
      return err == ContactSync.SyncProcessResult.Success || err == ContactSync.SyncProcessResult.SuccessNoop;
    }

    public static bool isRetryableError(ContactSync.SyncProcessResult processResult)
    {
      switch (processResult)
      {
        case ContactSync.SyncProcessResult.ConnectionError:
        case ContactSync.SyncProcessResult.InCompleteParsedError:
        case ContactSync.SyncProcessResult.NoResponseReceivedError:
          return true;
        default:
          return false;
      }
    }

    public static ContactSync.SyncProcessResult NotificationSync(
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context,
      List<string> matchingNumbers,
      FunXMPP.Connection.ContactNotificationType type)
    {
      if (!Settings.PhoneNumberVerificationState.Equals((object) PhoneNumberVerificationState.Verified))
      {
        Log.l(ContactSync.LogHdr, "abort | not registered");
        return ContactSync.SyncProcessResult.NotRegisteredError;
      }
      FunXMPP.Connection connection = AppState.GetConnection();
      if (connection != null)
      {
        if (connection.IsConnected)
        {
          ContactSync.PerformSyncResult performSyncResult;
          try
          {
            performSyncResult = ContactSync.PerformSync(mode, context, (IEnumerable<string>) matchingNumbers, (IEnumerable<string>) new string[0], (IEnumerable<string>) new string[0]);
          }
          catch (ContactSync.SyncException ex)
          {
            Log.LogException((Exception) ex, "PerformSync");
            return ContactSync.SyncProcessResult.ServerError;
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "PerformSync");
            return ContactSync.SyncProcessResult.OtherError;
          }
          performSyncResult.CheckNotNull<ContactSync.PerformSyncResult>("Checking result from PerformSync");
          if (!performSyncResult.IsSuccess())
            return performSyncResult.SyncResult;
          FunXMPP.Connection.SyncResult listResult = performSyncResult.ListResult;
          listResult.CheckNotNull<FunXMPP.Connection.SyncResult>("Unexpected empty result from server");
          List<string> processedJids = new List<string>();
          switch (type)
          {
            case FunXMPP.Connection.ContactNotificationType.Add:
              foreach (FunXMPP.Connection.SyncResult.User swellFolk in listResult.SwellFolks)
              {
                ContactStore.OnContactNotification(swellFolk.Jid, swellFolk.OriginalNumber, true);
                processedJids.Add(swellFolk.Jid);
              }
              break;
            case FunXMPP.Connection.ContactNotificationType.Remove:
              foreach (FunXMPP.Connection.SyncResult.User holdout in listResult.Holdouts)
              {
                ContactStore.OnContactNotification(holdout.Jid, holdout.OriginalNumber, false);
                processedJids.Add(holdout.Jid);
              }
              break;
            case FunXMPP.Connection.ContactNotificationType.Update:
              foreach (FunXMPP.Connection.SyncResult.User swellFolk in listResult.SwellFolks)
              {
                ChatPictureStore.Reset(swellFolk.Jid);
                PresenceState.Instance.ResetForUser(swellFolk.Jid);
                UsyncQueryRequest.SendUsyncListUserQuery(swellFolk.Jid, FunXMPP.Connection.SyncMode.Delta, FunXMPP.Connection.SyncContext.Notification);
                processedJids.Add(swellFolk.Jid);
              }
              break;
          }
          ContactsContext.Instance((Action<ContactsContext>) (db => db.MarkNumbersAsOld((IEnumerable<string>) processedJids)));
          return ContactSync.SyncProcessResult.Success;
        }
      }
      Log.l(ContactSync.LogHdr, "abort | not connected");
      return ContactSync.SyncProcessResult.ConnectionError;
    }

    public static ContactSync.SyncProcessResult PhonebookSync(
      FunXMPP.Connection.SyncMode? modep,
      FunXMPP.Connection.SyncContext context)
    {
      if (ContactSync.ShouldBackoff(Settings.SyncBackoffUtc))
      {
        Log.l(ContactSync.LogHdr, "abort | back off until {0} (utc)", (object) Settings.SyncBackoffUtc);
        return ContactSync.SyncProcessResult.BackOffError;
      }
      if (ContactSync.ShouldBackoff(Settings.UsyncBackoffUtc))
      {
        Log.l(ContactSync.LogHdr, "abort | back off until {0} (utc)", (object) Settings.UsyncBackoffUtc);
        return ContactSync.SyncProcessResult.BackOffError;
      }
      if (!Settings.PhoneNumberVerificationState.Equals((object) PhoneNumberVerificationState.Verified))
      {
        Log.l(ContactSync.LogHdr, "abort | not registered");
        return ContactSync.SyncProcessResult.NotRegisteredError;
      }
      List<string> addedOut = new List<string>();
      List<string> removedOut = new List<string>();
      List<string> sidelistOut = (List<string>) null;
      List<DateTime> syncHistory = ContactSync.ParseSyncHistory(Settings.SyncHistory);
      FunXMPP.Connection.SyncMode? nullable = modep;
      FunXMPP.Connection.SyncMode syncMode = FunXMPP.Connection.SyncMode.Full;
      if ((nullable.GetValueOrDefault() == syncMode ? (nullable.HasValue ? 1 : 0) : 0) != 0 && !ContactSync.CheckHistory(syncHistory, context))
        return ContactSync.SyncProcessResult.BackOffError;
      nullable = modep;
      FunXMPP.Connection.SyncMode mode = (FunXMPP.Connection.SyncMode) ((int) nullable ?? (int) ContactSync.SelectSyncMode());
      Contact[] allContactsTask = ContactStore.GetAllContactsTask();
      if (AppState.GetConnection() == null || !AppState.GetConnection().IsConnected)
      {
        Log.l(ContactSync.LogHdr, "abort | not connected");
        return ContactSync.SyncProcessResult.ConnectionError;
      }
      if (allContactsTask.Length == 0)
      {
        Pair<int, DateTime> lastAddressbookSize = ContactSync.ParseLastAddressbookSize(Settings.LastAddressbookSize);
        if (lastAddressbookSize != null && lastAddressbookSize.First != 0)
        {
          TimeSpan timeSpan = DateTime.UtcNow.Subtract(lastAddressbookSize.Second);
          Log.l(ContactSync.LogHdr, "Addressbook Change dropped to 0, previous={0}, new=0, timeBetween={1}", (object) lastAddressbookSize.First, (object) timeSpan);
          Settings.LastAddressbookSize = ContactSync.EncodeLastAddressbookSize(allContactsTask.Length);
        }
        else if (lastAddressbookSize == null)
          Settings.LastAddressbookSize = ContactSync.EncodeLastAddressbookSize(allContactsTask.Length);
      }
      else if (allContactsTask.Length > ContactSync.thresholdForAddressbookSizeLogging)
      {
        Pair<int, DateTime> lastAddressbookSize = ContactSync.ParseLastAddressbookSize(Settings.LastAddressbookSize);
        if (lastAddressbookSize != null && lastAddressbookSize.First == 0)
        {
          TimeSpan timeSpan = DateTime.UtcNow.Subtract(lastAddressbookSize.Second);
          Log.l(ContactSync.LogHdr, "Addressbook Change returned back to normal, previous=0, new={0}, timeBetween={1}", (object) allContactsTask.Length, (object) timeSpan);
          Settings.LastAddressbookSize = ContactSync.EncodeLastAddressbookSize(allContactsTask.Length);
        }
        else if (lastAddressbookSize == null)
          Settings.LastAddressbookSize = ContactSync.EncodeLastAddressbookSize(allContactsTask.Length);
      }
      ContactSync.CreateDeltas(context, mode, allContactsTask, out addedOut, out removedOut, out sidelistOut);
      Log.l(ContactSync.LogHdr, "Create Delta, mode={0}, addressbook={1}, added={2}, removed={3}, sidelist={4}", (object) mode, (object) allContactsTask.Length, (object) addedOut.Count, (object) removedOut.Count, (object) sidelistOut.Count);
      if (removedOut.Any<string>())
        ContactStore.ContactsUpdatedSubject.OnNext(removedOut.Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))).ToArray<string>());
      string[] array = (mode == FunXMPP.Connection.SyncMode.Delta ? ((IEnumerable<PhoneNumber>) ContactsContext.Instance<PhoneNumber[]>((Func<ContactsContext, PhoneNumber[]>) (db => db.GetNewNumbers()))).Select<PhoneNumber, string>((Func<PhoneNumber, string>) (pn => pn.RawPhoneNumber)).MakeUnique<string, string>((Func<string, string>) (_ => _)) : ((IEnumerable<Contact>) allContactsTask).SelectMany<Contact, ContactPhoneNumber>((Func<Contact, IEnumerable<ContactPhoneNumber>>) (c => c.PhoneNumbers)).Select<ContactPhoneNumber, string>((Func<ContactPhoneNumber, string>) (p => p.PhoneNumber)).MakeUnique<string, string>((Func<string, string>) (_ => _))).ToArray<string>();
      ContactSync.SyncProcessResult syncProcessResult;
      if (mode != FunXMPP.Connection.SyncMode.Delta || ((IEnumerable<string>) array).Any<string>() || removedOut.Any<string>() || sidelistOut != null && sidelistOut.Any<string>())
      {
        Log.d(ContactSync.LogHdr, "Processing sync");
        ContactSync.PerformSyncResult performSyncResult;
        try
        {
          performSyncResult = ((IEnumerable<string>) array).Any<string>() || removedOut.Any<string>() || (sidelistOut == null ? 1 : (!sidelistOut.Any<string>() ? 1 : 0)) == 0 ? (mode != FunXMPP.Connection.SyncMode.Full ? ContactSync.PerformSync(mode, context, (IEnumerable<string>) array, (IEnumerable<string>) removedOut, (IEnumerable<string>) sidelistOut) : ContactSync.PerformSync(mode, context, (IEnumerable<string>) array, (IEnumerable<string>) new List<string>(), (IEnumerable<string>) sidelistOut)) : new ContactSync.PerformSyncResult(ContactSync.SyncProcessResult.NoContactsFound);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "PerformSync - non delta or delta with data");
          return ContactSync.SyncProcessResult.OtherError;
        }
        if (!performSyncResult.IsSuccess())
          return performSyncResult.SyncResult;
        FunXMPP.Connection.SyncResult listResult = performSyncResult.ListResult;
        listResult.CheckNotNull<FunXMPP.Connection.SyncResult>("Server response doesn't contain aresult!");
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          ContactSync.HandleResponse(listResult, performSyncResult.SidelistResult, allContactsTask, mode, context, storeForApplication);
        if (!Settings.LastFullSyncUtc.HasValue)
        {
          Log.l("PassiveMode", "Finished sync. Going active");
          AppState.GetConnection().SendSetPassiveMode(false);
        }
        if (mode == FunXMPP.Connection.SyncMode.Full)
          ContactSync.SetFullSyncComplete();
        ContactsContext.Instance((Action<ContactsContext>) (db => db.StatusUpdateSubject.OnNext(new Unit())));
        syncProcessResult = ContactSync.SyncProcessResult.Success;
      }
      else
      {
        Log.d(ContactSync.LogHdr, "Processing noop");
        syncProcessResult = ContactSync.SyncProcessResult.SuccessNoop;
      }
      return syncProcessResult;
    }

    public static void SetFullSyncComplete()
    {
      List<DateTime> syncHistory = ContactSync.ParseSyncHistory(Settings.SyncHistory);
      Settings.LastFullSyncUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
      DateTime[] second = new DateTime[1]{ DateTime.UtcNow };
      Settings.SyncHistory = ContactSync.EncodeSyncHistory(syncHistory.Concat<DateTime>((IEnumerable<DateTime>) second));
    }

    private static FunXMPP.Connection.SyncMode SelectSyncMode()
    {
      DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
      DateTime? lastFullSyncUtc = Settings.LastFullSyncUtc;
      DateTime? nextFullSyncUtc = Settings.NextFullSyncUtc;
      Log.l("sync", "last full sync: {0}, next scheduled: {1}", !lastFullSyncUtc.HasValue ? (object) "never" : (object) lastFullSyncUtc.ToString(), !nextFullSyncUtc.HasValue ? (object) "24 hours after previous" : (object) nextFullSyncUtc.ToString());
      if (lastFullSyncUtc.HasValue)
      {
        DateTime? nullable = lastFullSyncUtc;
        DateTime dateTime = currentServerTimeUtc;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() > dateTime ? 1 : 0) : 0) == 0 && !((nextFullSyncUtc ?? lastFullSyncUtc.Value + TimeSpan.FromDays(1.0)) < currentServerTimeUtc))
          return FunXMPP.Connection.SyncMode.Delta;
      }
      return FunXMPP.Connection.SyncMode.Full;
    }

    private static ContactSync.PerformSyncResult PerformSync(
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context,
      IEnumerable<string> numbers,
      IEnumerable<string> removedJids,
      IEnumerable<string> sideList)
    {
      CountdownEvent cde = (CountdownEvent) null;
      if (numbers.Any<string>() && removedJids.Any<string>())
      {
        ContactSync.PerformSyncResult performSyncResult1;
        try
        {
          performSyncResult1 = ContactSync.PerformSync(mode, context, numbers, (IEnumerable<string>) new List<string>(), sideList);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "PerformSync - non delta or delta with data");
          performSyncResult1 = new ContactSync.PerformSyncResult(ContactSync.SyncProcessResult.OtherError);
        }
        if (!performSyncResult1.IsSuccess())
          return new ContactSync.PerformSyncResult(performSyncResult1.SyncResult);
        HashSet<string> stringSet = new HashSet<string>();
        foreach (string removedJid in removedJids)
          stringSet.Add(removedJid);
        foreach (FunXMPP.Connection.SyncResult.User swellFolk in performSyncResult1.ListResult.SwellFolks)
          stringSet.Remove(swellFolk.Jid);
        foreach (FunXMPP.Connection.SyncResult.User holdout in performSyncResult1.ListResult.Holdouts)
          stringSet.Remove(holdout.Jid);
        if (!stringSet.Any<string>())
          return performSyncResult1;
        ContactSync.PerformSyncResult performSyncResult2 = ContactSync.PerformSync(mode, context, (IEnumerable<string>) new List<string>(), (IEnumerable<string>) stringSet, (IEnumerable<string>) new List<string>());
        if (!performSyncResult2.IsSuccess())
          return new ContactSync.PerformSyncResult(performSyncResult2.SyncResult);
        return new ContactSync.PerformSyncResult(new FunXMPP.Connection.SyncResult()
        {
          SwellFolks = ((IEnumerable<FunXMPP.Connection.SyncResult.User>) performSyncResult1.ListResult.SwellFolks).Concat<FunXMPP.Connection.SyncResult.User>((IEnumerable<FunXMPP.Connection.SyncResult.User>) performSyncResult2.ListResult.SwellFolks).ToArray<FunXMPP.Connection.SyncResult.User>(),
          Holdouts = ((IEnumerable<FunXMPP.Connection.SyncResult.User>) performSyncResult1.ListResult.Holdouts).Concat<FunXMPP.Connection.SyncResult.User>((IEnumerable<FunXMPP.Connection.SyncResult.User>) performSyncResult2.ListResult.Holdouts).ToArray<FunXMPP.Connection.SyncResult.User>(),
          NormalizationErrors = ((IEnumerable<string>) performSyncResult1.ListResult.NormalizationErrors).Concat<string>((IEnumerable<string>) performSyncResult2.ListResult.NormalizationErrors).ToArray<string>(),
          NextFullSyncUtc = performSyncResult2.ListResult.NextFullSyncUtc
        }, performSyncResult1.SidelistResult);
      }
      if (removedJids.Any<string>())
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          List<WaScheduledTask> waScheduledTaskList = new List<WaScheduledTask>();
          WaScheduledTask postSubmitTask = (WaScheduledTask) null;
          foreach (string removedJid in removedJids)
          {
            ContactSync.DeleteContact(db, removedJid, out postSubmitTask);
            waScheduledTaskList.Add(postSubmitTask);
          }
          db.SubmitChanges();
          int num = 0;
          foreach (WaScheduledTask task in waScheduledTaskList)
            db.AttemptScheduledTaskOnThreadPool(task, ++num * 500);
        }));
      FunXMPP.Connection conn = AppState.GetConnection();
      if (conn == null)
        return new ContactSync.PerformSyncResult(ContactSync.SyncProcessResult.OtherError);
      int num1 = 0;
      string sid = DateTime.UtcNow.ToFileTimeUtc().ToString();
      FunXMPP.Connection.SyncResult contactResult = (FunXMPP.Connection.SyncResult) null;
      FunXMPP.Connection.SyncResult sidelistResult = (FunXMPP.Connection.SyncResult) null;
      ContactSync.SyncException syncException = (ContactSync.SyncException) null;
      UsyncQuery query = new UsyncQuery(mode, context);
      int initialCount = num1 + 1;
      Log.d(ContactSync.LogHdr, "Waiting for contacts");
      conn.AddUsyncGetContacts(query, sid, 0, true, numbers, removedJids, (Action<FunXMPP.Connection.SyncResult>) (res =>
      {
        contactResult = res;
        Log.d(ContactSync.LogHdr, "Completed getting contacts");
        cde?.Signal();
      }), (Action<int>) (err =>
      {
        Log.l(ContactSync.LogHdr, "Error getting contacts {0}", (object) err);
        syncException = new ContactSync.SyncException(err);
      }));
      if (Settings.UsyncSidelist && sideList != null && sideList.Count<string>() > 0)
      {
        if (mode == FunXMPP.Connection.SyncMode.Full && (context == FunXMPP.Connection.SyncContext.Registration || ContactSync.ShouldRefresh(Settings.NextUsyncSidelistRefreshUtc)) && !ContactSync.ShouldBackoff(Settings.UsyncSidelistBackoffUtc))
        {
          ++initialCount;
          Log.d(ContactSync.LogHdr, "Waiting for sidelist");
          conn.AddUsyncGetSidelist(query, sideList, (Action<FunXMPP.Connection.SyncResult>) (res =>
          {
            sidelistResult = res;
            Log.d(ContactSync.LogHdr, "Completed getting sidelist");
            cde?.Signal();
          }), (Action<int>) (err => Log.l(ContactSync.LogHdr, "Error getting sidelist {0}", (object) err)));
        }
        else if (mode == FunXMPP.Connection.SyncMode.Delta)
        {
          ++initialCount;
          Log.d(ContactSync.LogHdr, "Waiting for delta sidelist");
          conn.AddUsyncGetSidelist(query, sideList, (Action<FunXMPP.Connection.SyncResult>) (res =>
          {
            sidelistResult = res;
            Log.d(ContactSync.LogHdr, "Completed getting delta sidelist");
            cde?.Signal();
          }), (Action<int>) (err => Log.l(ContactSync.LogHdr, "Error getting delta sidelist {0}", (object) err)));
        }
      }
      bool flag1 = numbers.Count<string>() > 0 || sideList != null && sideList.Any<string>();
      if ((flag1 || ContactSync.ShouldRefresh(Settings.NextUsyncStatusRefreshUtc)) && !ContactSync.ShouldBackoff(Settings.UsyncStatusBackoffUtc))
      {
        ++initialCount;
        Log.d(ContactSync.LogHdr, "Waiting for status");
        conn.AddUsyncGetStatuses(query, onComplete: (Action) (() =>
        {
          Log.d(ContactSync.LogHdr, "Completed getting status");
          cde?.Signal();
        }), onError: (Action<string, int>) ((str, err) => Log.l(ContactSync.LogHdr, "Error getting status {0}, {1}.", (object) err, str == null ? (object) "null" : (object) str)));
      }
      if ((flag1 || mode == FunXMPP.Connection.SyncMode.Full || ContactSync.ShouldRefresh(Settings.NextUsyncFeatureRefreshUtc)) && !ContactSync.ShouldBackoff(Settings.UsyncFeatureBackoffUtc))
      {
        IEnumerable<string> strings = (IEnumerable<string>) new string[0];
        if (mode == FunXMPP.Connection.SyncMode.Full)
          strings = strings.Concat<string>((IEnumerable<string>) MessagesContext.Select<string[]>((Func<MessagesContext, string[]>) (mdb => mdb.GetAllGroupParticipantsJids())));
        string[] array = strings.MakeUnique<string>().ToArray<string>();
        if (!((IEnumerable<string>) array).Any<string>() ? conn.AddUsyncGetRemoteCapabilities(query, onComplete: (Action) (() =>
        {
          Log.d(ContactSync.LogHdr, "Completed getting remote (no jids) capabilites");
          cde?.Signal();
        }), onError: (Action<int>) (err => Log.l(ContactSync.LogHdr, "Error getting remote capabilites (no jids) {0}", (object) err))) : conn.AddUsyncGetRemoteCapabilities(query, (IEnumerable<string>) array, onComplete: (Action) (() =>
        {
          Log.d(ContactSync.LogHdr, "Completed getting remote capabilites");
          cde?.Signal();
        }), onError: (Action<int>) (err => Log.l(ContactSync.LogHdr, "Error getting remote capabilites {0}", (object) err))))
        {
          ++initialCount;
          Log.d(ContactSync.LogHdr, "Waiting for remote capabilities");
        }
      }
      if ((context == FunXMPP.Connection.SyncContext.Registration & flag1 || ContactSync.ShouldRefresh(Settings.NextUsyncPictureRefreshUtc)) && !ContactSync.ShouldBackoff(Settings.UsyncPictureBackoffUtc))
      {
        ++initialCount;
        Log.d(ContactSync.LogHdr, "Waiting for photosync");
        UsyncQueryRequest.AddUsyncGetPhotoIds(conn, query, (Action) (() => cde?.Signal()), (Action<int>) (err => Log.l(ContactSync.LogHdr, "photosync eager fetch: error " + (object) err)));
      }
      if ((flag1 || ContactSync.ShouldRefresh(Settings.NextUsyncBusinessRefreshUtc)) && !ContactSync.ShouldBackoff(Settings.UsyncBusinessBackoffUtc))
      {
        List<FunXMPP.Connection.BusinessRequest> sidelistJids = new List<FunXMPP.Connection.BusinessRequest>();
        if (sideList != null && sideList.Count<string>() > 0)
        {
          foreach (string side in sideList)
            sidelistJids.Add(new FunXMPP.Connection.BusinessRequest()
            {
              Jid = side
            });
        }
        conn.AddUsyncGetBusinesses(query, (IEnumerable<FunXMPP.Connection.BusinessRequest>) new FunXMPP.Connection.BusinessRequest[0], (IEnumerable<FunXMPP.Connection.BusinessRequest>) sidelistJids, (Action) (() =>
        {
          Log.d(ContactSync.LogHdr, "Completed getting businesses");
          cde?.Signal();
        }), (Action<string, int>) ((str, err) => Log.l(ContactSync.LogHdr, "Error getting businesses {0}, {1}.", (object) err, str == null ? (object) "null" : (object) str)));
        ++initialCount;
        Log.d(ContactSync.LogHdr, "Waiting for businesses");
      }
      ManualResetEvent receiveIQ = new ManualResetEvent(false);
      cde = new CountdownEvent(initialCount);
      try
      {
        if (!conn.InvokeIfConnected((Action) (() =>
        {
          if (!UsyncQueryRequest.Send(query, conn, (Action) (() => receiveIQ.Set())))
            throw new ArgumentException("improper sync query attempted to send in ContactSync");
        })))
          return new ContactSync.PerformSyncResult(ContactSync.SyncProcessResult.ConnectionError);
        Log.l(ContactSync.LogHdr, "Sync waiting {0} secs to receive iq for {1} protocols", (object) (ContactSync.waitForIQResponseTimeoutMs / 1000), (object) initialCount);
        bool flag2 = receiveIQ.WaitOne(ContactSync.waitForIQResponseTimeoutMs);
        if (!flag2)
          return new ContactSync.PerformSyncResult(ContactSync.SyncProcessResult.ConnectionError);
        int millisecondsTimeout = context == FunXMPP.Connection.SyncContext.Registration || mode == FunXMPP.Connection.SyncMode.Full ? ContactSync.waitForFullResponseParseTimeoutMs : ContactSync.waitForDeltaResponseParseTimeoutMs;
        Log.l(ContactSync.LogHdr, "Sync waiting {0} secs to parse result for {1} protocols", (object) (millisecondsTimeout / 1000), (object) initialCount);
        bool flag3 = cde.Wait(millisecondsTimeout);
        int currentCount = cde.CurrentCount;
        Log.l(ContactSync.LogHdr, "Sync wait completed, iqReceived={0}, finishedParse={1}, unfinishedParse={2}, exception={3}", (object) flag2, (object) (initialCount - currentCount), (object) currentCount, (object) (syncException != null));
        if (syncException != null)
          throw syncException;
        if (!flag3)
        {
          if (currentCount <= 0 || currentCount >= initialCount)
            return new ContactSync.PerformSyncResult(ContactSync.SyncProcessResult.NoResponseReceivedError);
          Log.SendCrashLog((Exception) new InvalidDataException("Incomplete synchronization"), "Sync failure", logOnlyForRelease: true);
          return new ContactSync.PerformSyncResult(ContactSync.SyncProcessResult.InCompleteParsedError);
        }
        contactResult.CheckNotNull<FunXMPP.Connection.SyncResult>("Result is null after all :(");
        return new ContactSync.PerformSyncResult(contactResult, sidelistResult);
      }
      finally
      {
        cde.SafeDispose();
        cde = (CountdownEvent) null;
      }
    }

    private static void HandleResponse(
      FunXMPP.Connection.SyncResult listResult,
      FunXMPP.Connection.SyncResult sidelistResult,
      Contact[] contacts,
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context,
      IsolatedStorageFile fs)
    {
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        List<Action<ContactsContext>> postSubmitActions = (List<Action<ContactsContext>>) null;
        Dictionary<string, ContactStore.ContactWithKind> dictionary1 = ((IEnumerable<Contact>) contacts).SelectMany((Func<Contact, IEnumerable<ContactPhoneNumber>>) (contact => contact.PhoneNumbers), (contact, num) => new
        {
          contact = contact,
          num = num
        }).GroupBy(_param1 => _param1.num.PhoneNumber, _param1 => new ContactStore.ContactWithKind()
        {
          Contact = _param1.contact,
          Kind = _param1.num.Kind
        }).ToDictionary<IGrouping<string, ContactStore.ContactWithKind>, string, ContactStore.ContactWithKind>((Func<IGrouping<string, ContactStore.ContactWithKind>, string>) (g => g.Key), (Func<IGrouping<string, ContactStore.ContactWithKind>, ContactStore.ContactWithKind>) (g => g.First<ContactStore.ContactWithKind>()));
        List<Action> postActions = (List<Action>) null;
        Dictionary<string, UserStatus> result = ContactSync.ParseResult(db, dictionary1, listResult, sidelistResult, fs, out postActions, out postSubmitActions);
        postActions?.ForEach((Action<Action>) (a => a()));
        Dictionary<string, IGrouping<string, PhoneNumber>> dictionary2 = ((IEnumerable<PhoneNumber>) db.GetAllPhoneNumbers()).GroupBy<PhoneNumber, string>((Func<PhoneNumber, string>) (pn => pn.RawPhoneNumber)).ToDictionary<IGrouping<string, PhoneNumber>, string>((Func<IGrouping<string, PhoneNumber>, string>) (g => g.Key));
        List<string> stringList = new List<string>();
        foreach (KeyValuePair<string, UserStatus> keyValuePair in result)
        {
          if (keyValuePair.Value?.Jid == null)
          {
            Log.l(ContactSync.LogHdr, "Null value found found for {0}, {1}", (object) keyValuePair.Key, (object) (keyValuePair.Value == null));
            if (!ContactSync.sentCrashlogOnNullUser)
            {
              ContactSync.sentCrashlogOnNullUser = true;
              Log.SendCrashLog((Exception) new InvalidDataException("user or user jid is null"), "Contact sync issue", logOnlyForRelease: true);
            }
            stringList.Add(keyValuePair.Key);
          }
        }
        foreach (string key in stringList)
          result.Remove(key);
        foreach (var data in result.Select(kv => new
        {
          Key = kv.Key,
          Value = kv.Value.Jid
        }))
        {
          IGrouping<string, PhoneNumber> source;
          if (!dictionary2.TryGetValue(data.Key, out source))
          {
            db.InsertPhoneNumberOnSubmit(new PhoneNumber()
            {
              Jid = data.Value,
              RawPhoneNumber = data.Key
            });
          }
          else
          {
            PhoneNumber phoneNumber = source.First<PhoneNumber>();
            if (phoneNumber.Jid != data.Value)
              phoneNumber.Jid = data.Value;
            foreach (PhoneNumber n in source.Skip<PhoneNumber>(1))
              db.DeletePhoneNumberOnSubmit(n);
          }
        }
        db.SubmitChanges();
        db.MarkNumbersAsOld();
        if (listResult.NextFullSyncUtc.HasValue)
          Settings.NextFullSyncUtc = new DateTime?(listResult.NextFullSyncUtc.Value);
        Settings.RemovedJids = (string[]) null;
        if (postSubmitActions != null && postSubmitActions.Any<Action<ContactsContext>>())
        {
          foreach (Action<ContactsContext> action in postSubmitActions)
            action(db);
        }
        db.ClearCache();
        db.ReadAllUserStatuses();
      }));
    }

    private static Dictionary<string, UserStatus> ParseResult(
      ContactsContext cdb,
      Dictionary<string, ContactStore.ContactWithKind> rawToNames,
      FunXMPP.Connection.SyncResult listResult,
      FunXMPP.Connection.SyncResult sidelistResult,
      IsolatedStorageFile fs,
      out List<Action> postActions,
      out List<Action<ContactsContext>> postSubmitActions)
    {
      List<Action> actions = new List<Action>();
      postSubmitActions = new List<Action<ContactsContext>>();
      List<string> stringList = new List<string>();
      bool checkedDir = false;
      cdb.ReadAllUserStatuses();
      Dictionary<string, UserStatus> ret = new Dictionary<string, UserStatus>();
      MemoryStream photoBuffer = new MemoryStream();
      List<string> changedJids = new List<string>();
      changedJids.AddRange((IEnumerable<string>) ContactSync.SyncUserStatusWithContact(cdb, rawToNames, listResult.SwellFolks, photoBuffer, fs, ref checkedDir, stringList, actions, true, ret, ref postSubmitActions));
      changedJids.AddRange((IEnumerable<string>) ContactSync.SyncUserStatusWithContact(cdb, rawToNames, listResult.Holdouts, photoBuffer, fs, ref checkedDir, stringList, actions, false, ret, ref postSubmitActions));
      if (sidelistResult != null)
      {
        changedJids.AddRange((IEnumerable<string>) ContactSync.SyncUserStatusWithContact(cdb, rawToNames, sidelistResult.SwellFolks, photoBuffer, fs, ref checkedDir, stringList, actions, true, ret, ref postSubmitActions));
        changedJids.AddRange((IEnumerable<string>) ContactSync.SyncUserStatusWithContact(cdb, rawToNames, sidelistResult.Holdouts, photoBuffer, fs, ref checkedDir, stringList, actions, false, ret, ref postSubmitActions));
      }
      Log.l("sync", "parsed {0} results, db_changes={1}", (object) ret.Count, (object) actions.Count);
      postSubmitActions.AddRange(stringList.Select<string, Action<ContactsContext>>((Func<string, Action<ContactsContext>>) (filename => (Action<ContactsContext>) (_ =>
      {
        try
        {
          fs.DeleteFile(filename);
        }
        catch (Exception ex)
        {
        }
      }))));
      if (changedJids.Any<string>())
        postSubmitActions.Add((Action<ContactsContext>) (_ => ContactStore.ContactsUpdatedSubject.OnNext(changedJids.ToArray())));
      postActions = actions;
      return ret;
    }

    private static List<string> SyncUserStatusWithContact(
      ContactsContext cdb,
      Dictionary<string, ContactStore.ContactWithKind> rawToNames,
      FunXMPP.Connection.SyncResult.User[] users,
      MemoryStream photoBuffer,
      IsolatedStorageFile fs,
      ref bool checkedDir,
      List<string> toDelete,
      List<Action> actions,
      bool isWAUser,
      Dictionary<string, UserStatus> ret,
      ref List<Action<ContactsContext>> postSubmitActions)
    {
      int i = 1;
      List<string> stringList = new List<string>();
      WaScheduledTask postSubmitTask = (WaScheduledTask) null;
      foreach (FunXMPP.Connection.SyncResult.User user in users)
      {
        string originalNumber = user.OriginalNumber;
        string jid = user.Jid;
        UserStatus userStatus = cdb.GetUserStatus(jid);
        if (userStatus == null)
        {
          Log.l(ContactSync.LogHdr, "no user found for {0} - {1}", (object) jid, (object) originalNumber);
          Log.SendCrashLog((Exception) new InvalidDataException("Missing user in contact sync"), "Missing user in contact sync", logOnlyForRelease: true);
        }
        else
        {
          ContactStore.ContactWithKind nameInfo;
          bool flag;
          if (originalNumber != null && rawToNames.TryGetValue(originalNumber, out nameInfo))
          {
            flag = ContactSync.CopyContactDetails(nameInfo, userStatus, photoBuffer, fs, ref checkedDir, toDelete, true, isWAUser, (Action<Action>) (a => actions.Add(a)));
          }
          else
          {
            flag = ContactSync.ClearContactDetails(cdb, userStatus, out postSubmitTask, (Action<Action>) (a => actions.Add(a)));
            if (flag)
              postSubmitActions.Add((Action<ContactsContext>) (postSubmitDb => postSubmitDb.AttemptScheduledTaskOnThreadPool(postSubmitTask, ++i * 500)));
          }
          if (flag)
            stringList.Add(jid);
          if (originalNumber != null)
            ret[originalNumber] = userStatus;
        }
      }
      return stringList;
    }

    private static bool ShouldBackoff(DateTime? backoffTillUtc)
    {
      return backoffTillUtc.HasValue && FunRunner.CurrentServerTimeUtc < backoffTillUtc.Value;
    }

    private static bool ShouldRefresh(DateTime? refreshTimeUtc)
    {
      return refreshTimeUtc.HasValue && FunRunner.CurrentServerTimeUtc > refreshTimeUtc.Value;
    }

    private static Pair<int, DateTime> ParseLastAddressbookSize(byte[] b)
    {
      if (b == null || b.Length == 0)
        return (Pair<int, DateTime>) null;
      BinaryReader binaryReader = new BinaryReader((Stream) new MemoryStream(b, false));
      int first = binaryReader.ReadInt32();
      long seconds = binaryReader.ReadInt64();
      DateTime second;
      try
      {
        second = DateTimeUtils.FromUnixTime(seconds);
      }
      catch (Exception ex)
      {
        return (Pair<int, DateTime>) null;
      }
      return new Pair<int, DateTime>(first, second);
    }

    private static byte[] EncodeLastAddressbookSize(int size)
    {
      MemoryStream output = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
      binaryWriter.Write(size);
      binaryWriter.Write(DateTime.UtcNow.ToUnixTime());
      return output.ToArray();
    }

    private static List<DateTime> ParseSyncHistory(byte[] b)
    {
      List<DateTime> syncHistory = new List<DateTime>();
      DateTime dateTime1 = DateTime.UtcNow - TimeSpan.FromHours(1.0);
      if (b != null && b.Length != 0)
      {
        BinaryReader binaryReader = new BinaryReader((Stream) new MemoryStream(b, false));
        int num = b.Length / 8;
        while (num-- != 0)
        {
          long seconds = binaryReader.ReadInt64();
          DateTime dateTime2;
          try
          {
            dateTime2 = DateTimeUtils.FromUnixTime(seconds);
          }
          catch (Exception ex)
          {
            continue;
          }
          if (dateTime2 > dateTime1)
            syncHistory.Add(dateTime2);
        }
      }
      return syncHistory;
    }

    public static byte[] EncodeSyncHistory(IEnumerable<DateTime> dts)
    {
      MemoryStream output = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
      foreach (DateTime dt in dts)
      {
        try
        {
          binaryWriter.Write(dt.ToUnixTime());
        }
        catch (Exception ex)
        {
        }
      }
      return output.ToArray();
    }

    private static void CreateDeltas(
      FunXMPP.Connection.SyncContext context,
      FunXMPP.Connection.SyncMode mode,
      Contact[] src,
      out List<string> addedOut,
      out List<string> removedOut,
      out List<string> sidelistOut)
    {
      Dictionary<string, ContactStore.ContactWithKind> rawNumbers = new Dictionary<string, ContactStore.ContactWithKind>();
      Dictionary<string, ContactStore.ContactWithKindAndJid> contactsByJid = new Dictionary<string, ContactStore.ContactWithKindAndJid>();
      DateTime? start = PerformanceTimer.Start();
      foreach (Contact contact in src)
      {
        foreach (ContactPhoneNumber phoneNumber1 in contact.PhoneNumbers)
        {
          string phoneNumber2 = phoneNumber1.PhoneNumber;
          if (!rawNumbers.ContainsKey(phoneNumber2))
          {
            PhoneNumberKind kind = phoneNumber1.Kind;
            rawNumbers.Add(phoneNumber2, new ContactStore.ContactWithKind()
            {
              Contact = contact,
              Kind = kind
            });
          }
        }
      }
      PerformanceTimer.End("initial processing", start);
      List<PhoneNumber> added = new List<PhoneNumber>();
      List<PhoneNumber> source = new List<PhoneNumber>();
      List<PhoneNumber> removedPhoneNumbers = new List<PhoneNumber>();
      Dictionary<string, PhoneNumber> oldRawNumbers = new Dictionary<string, PhoneNumber>();
      Dictionary<string, int> jidsCount = new Dictionary<string, int>();
      List<string> localSidelist = new List<string>();
      HashSet<string> checkConversationsJids = new HashSet<string>();
      bool createSidelist = Settings.UsyncSidelist;
      if (createSidelist)
        checkConversationsJids = ContactSync.GetConversations();
      start = PerformanceTimer.Start();
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        Dictionary<string, UserStatus> phonebookJids;
        Dictionary<string, UserStatus> sidelistedJids;
        Dictionary<string, UserStatus> sidelistPendingJids;
        Dictionary<string, UserStatus> others;
        ContactSync.GetAllUserDetails(db, createSidelist, checkConversationsJids, out phonebookJids, out sidelistedJids, out sidelistPendingJids, out others);
        if (mode == FunXMPP.Connection.SyncMode.Full)
        {
          foreach (KeyValuePair<string, UserStatus> keyValuePair in sidelistedJids)
            localSidelist.Add(keyValuePair.Key);
        }
        foreach (KeyValuePair<string, UserStatus> keyValuePair in sidelistPendingJids)
          localSidelist.Add(keyValuePair.Key);
        PerformanceTimer.End("load contacts", start);
        start = PerformanceTimer.Start();
        foreach (PhoneNumber allPhoneNumber in db.GetAllPhoneNumbers())
        {
          if (!oldRawNumbers.ContainsKey(allPhoneNumber.RawPhoneNumber))
            oldRawNumbers.Add(allPhoneNumber.RawPhoneNumber, allPhoneNumber);
          if (allPhoneNumber.Jid != null)
          {
            if (jidsCount.ContainsKey(allPhoneNumber.Jid))
              jidsCount[allPhoneNumber.Jid]++;
            else
              jidsCount.Add(allPhoneNumber.Jid, 1);
            UserStatus userStatus = (UserStatus) null;
            ContactStore.ContactWithKind contactWithKind = (ContactStore.ContactWithKind) null;
            if (rawNumbers.TryGetValue(allPhoneNumber.RawPhoneNumber, out contactWithKind))
            {
              if (phonebookJids.TryGetValue(allPhoneNumber.Jid, out userStatus))
                contactsByJid[allPhoneNumber.Jid] = new ContactStore.ContactWithKindAndJid()
                {
                  Status = userStatus,
                  Contact = contactWithKind.Contact,
                  Kind = contactWithKind.Kind,
                  IsWAUser = true
                };
              else if (others.TryGetValue(allPhoneNumber.Jid, out userStatus) || sidelistedJids.TryGetValue(allPhoneNumber.Jid, out userStatus) || sidelistPendingJids.TryGetValue(allPhoneNumber.Jid, out userStatus))
                contactsByJid[allPhoneNumber.Jid] = new ContactStore.ContactWithKindAndJid()
                {
                  Status = userStatus,
                  Contact = contactWithKind.Contact,
                  Kind = contactWithKind.Kind,
                  IsWAUser = userStatus.IsWaUser
                };
            }
          }
        }
        PerformanceTimer.End("load phone numbers", start);
      }));
      start = PerformanceTimer.Start();
      foreach (string key in rawNumbers.Keys)
      {
        if (!oldRawNumbers.ContainsKey(key))
          added.Add(new PhoneNumber()
          {
            RawPhoneNumber = key,
            IsNew = new bool?(true)
          });
      }
      foreach (PhoneNumber phoneNumber in oldRawNumbers.Values)
      {
        if (!rawNumbers.ContainsKey(phoneNumber.RawPhoneNumber))
        {
          removedPhoneNumbers.Add(phoneNumber);
          if (phoneNumber != null && phoneNumber.Jid != null)
          {
            int num = 0;
            jidsCount.TryGetValue(phoneNumber.Jid, out num);
            if (jidsCount[phoneNumber.Jid] == 1)
              source.Add(phoneNumber);
          }
        }
      }
      PerformanceTimer.End("address book delta", start);
      start = PerformanceTimer.Start();
      using (IsolatedStorageFile fs = IsolatedStorageFile.GetUserStoreForApplication())
      {
        List<Action> actions = new List<Action>();
        List<string> toDelete = new List<string>();
        MemoryStream photoBuffer = new MemoryStream();
        bool checkedDir = false;
        ContactSync.ioTime = TimeSpan.FromSeconds(0.0);
        ContactSync.hashTime = TimeSpan.FromSeconds(0.0);
        foreach (KeyValuePair<string, ContactStore.ContactWithKindAndJid> keyValuePair in contactsByJid)
          ContactSync.CopyContactDetails(new ContactStore.ContactWithKind()
          {
            Contact = keyValuePair.Value.Contact,
            Kind = keyValuePair.Value.Kind
          }, keyValuePair.Value.Status, photoBuffer, fs, ref checkedDir, toDelete, true, keyValuePair.Value.IsWAUser, (Action<Action>) (a => actions.Add(a)));
        Log.d("sync", "io time: {0}, hash time: {1}", (object) (int) ContactSync.ioTime.TotalMilliseconds, (object) (int) ContactSync.hashTime.TotalMilliseconds);
        PerformanceTimer.End("scan for metadata changes", start);
        if (!added.Any<PhoneNumber>() && !removedPhoneNumbers.Any<PhoneNumber>())
        {
          if (!actions.Any<Action>())
            goto label_39;
        }
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          Settings.UpdateContactsChecksum();
          foreach (PhoneNumber n in added)
            db.InsertPhoneNumberOnSubmit(n);
          foreach (PhoneNumber n in removedPhoneNumbers)
            db.DeletePhoneNumberOnSubmit(n);
          actions.ForEach((Action<Action>) (a => a()));
          db.SubmitChanges();
          toDelete.ForEach((Action<string>) (filename =>
          {
            try
            {
              fs.DeleteFile(filename);
            }
            catch (Exception ex)
            {
            }
          }));
        }));
      }
label_39:
      List<string> addedNumbers = added.Select<PhoneNumber, string>((Func<PhoneNumber, string>) (pn => pn.RawPhoneNumber)).ToList<string>();
      IEnumerable<string> removedEnumerator = source.Select<PhoneNumber, string>((Func<PhoneNumber, string>) (pn => pn.Jid));
      List<string> removedJids = removedEnumerator.ToList<string>();
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        string[] array = ((IEnumerable<PhoneNumber>) db.GetNewNumbers()).Select<PhoneNumber, string>((Func<PhoneNumber, string>) (pn => pn.RawPhoneNumber)).ToArray<string>();
        if (((IEnumerable<string>) array).Any<string>())
          addedNumbers = addedNumbers.Concat<string>((IEnumerable<string>) array).Where<string>((Func<string, bool>) (s => s != null)).MakeUnique<string>().ToList<string>();
        if (removedEnumerator.Any<string>() & createSidelist)
        {
          foreach (UserStatus userStatuse in db.GetUserStatuses(removedEnumerator, false, false))
          {
            if ((checkConversationsJids.Contains(userStatuse.Jid) || userStatuse.IsVerified()) && !localSidelist.Contains(userStatuse.Jid))
              localSidelist.Add(userStatuse.Jid);
          }
        }
        string[] removedJids1 = Settings.RemovedJids;
        if (removedJids1 != null && ((IEnumerable<string>) removedJids1).Any<string>())
        {
          removedJids = removedJids.Concat<string>((IEnumerable<string>) removedJids1).Where<string>((Func<string, bool>) (s => s != null)).MakeUnique<string>().ToList<string>();
          if (createSidelist)
          {
            foreach (UserStatus userStatuse in db.GetUserStatuses((IEnumerable<string>) removedJids1, false, false))
            {
              if ((checkConversationsJids.Contains(userStatuse.Jid) || userStatuse.IsVerified()) && !localSidelist.Contains(userStatuse.Jid))
                localSidelist.Add(userStatuse.Jid);
            }
          }
        }
        Settings.RemovedJids = removedJids.ToArray();
      }));
      Log.l(ContactSync.LogHdr, "usync create delta: mode={0}, added={1}, removed={2}, sidelist={3}", (object) mode, (object) addedNumbers.Count<string>(), (object) removedJids.Count<string>(), (object) localSidelist.Count<string>());
      addedOut = addedNumbers;
      removedOut = removedJids;
      sidelistOut = localSidelist;
    }

    private static bool CheckHistory(
      List<DateTime> history,
      FunXMPP.Connection.SyncContext context,
      bool log = true)
    {
      int num = context == FunXMPP.Connection.SyncContext.Interactive || context == FunXMPP.Connection.SyncContext.Registration ? 50 : 4;
      if (history.Count <= num)
        return true;
      if (log)
        Log.l("sync", "saw {0} recent syncs, above the limit of {1}", (object) history.Count, (object) num);
      return false;
    }

    public static void GetAllUserDetails(
      ContactsContext db,
      bool createSidelist,
      HashSet<string> checkConversationsJids,
      out Dictionary<string, UserStatus> phonebookJids,
      out Dictionary<string, UserStatus> sidelistedJids,
      out Dictionary<string, UserStatus> sidelistPendingJids,
      out Dictionary<string, UserStatus> others)
    {
      DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
      sidelistedJids = new Dictionary<string, UserStatus>();
      sidelistPendingJids = new Dictionary<string, UserStatus>();
      phonebookJids = new Dictionary<string, UserStatus>();
      others = new Dictionary<string, UserStatus>();
      foreach (UserStatus cachedUser in db.CachedUsers)
      {
        if (cachedUser.IsInDeviceContactList)
          phonebookJids[cachedUser.Jid] = cachedUser;
        else if (createSidelist && (cachedUser.IsVerified() || checkConversationsJids.Contains(cachedUser.Jid)))
        {
          if (cachedUser.IsSidelistSynced)
            sidelistedJids[cachedUser.Jid] = cachedUser;
          else
            sidelistPendingJids[cachedUser.Jid] = cachedUser;
        }
        else
          others[cachedUser.Jid] = cachedUser;
      }
      PerformanceTimer.End(nameof (GetAllUserDetails), start);
      Log.l(ContactSync.LogHdr, "Local Db state users found inAppPhonebook={0}, createSidelist={1}, sidelisted={2}, pendingSidelist={3}, others={4}", (object) phonebookJids.Count<KeyValuePair<string, UserStatus>>(), (object) createSidelist, (object) sidelistedJids.Count<KeyValuePair<string, UserStatus>>(), (object) sidelistPendingJids.Count<KeyValuePair<string, UserStatus>>(), (object) others.Count<KeyValuePair<string, UserStatus>>());
    }

    public static HashSet<string> GetConversations()
    {
      DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
      HashSet<string> source = new HashSet<string>();
      try
      {
        List<Conversation> convos = (List<Conversation>) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => convos = db.GetConversations(new JidHelper.JidTypes[1]
        {
          JidHelper.JidTypes.User
        }, true)));
        foreach (Conversation conversation in convos)
          source.Add(conversation.Jid);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Contact Store CreateDeltas");
        source = new HashSet<string>();
      }
      PerformanceTimer.End(nameof (GetConversations), start);
      Log.d(ContactSync.LogHdr, "Conversations found {0}", (object) source.Count<string>());
      return source;
    }

    public static bool CopyContactDetails(
      ContactStore.ContactWithKind nameInfo,
      UserStatus userStatus,
      MemoryStream photoBuffer,
      IsolatedStorageFile fs,
      ref bool checkedDir,
      List<string> toDelete,
      bool isInDevicePhonebook,
      bool isWAUser,
      Action<Action> perform = null)
    {
      if (userStatus == null || nameInfo?.Contact == null)
      {
        Log.l(ContactSync.LogHdr, "Null data supplied to CopyContactDetails - {0}, {1}, {2}", (object) (userStatus == null), (object) (nameInfo == null), (object) (nameInfo?.Contact == null));
        Log.SendCrashLog((Exception) new NullReferenceException("Missing data detected in CopyContactDetails"), "Missing data detected in CopyContactDetails", logOnlyForRelease: true);
        return false;
      }
      bool isInDeviceList = isInDevicePhonebook & isWAUser;
      bool flag1 = false;
      Settings.StatusRecipientsStateDirty = true;
      int num1 = 0;
      try
      {
        if (perform == null)
          perform = (Action<Action>) (a => a());
        if (userStatus.PhoneNumberKind != nameInfo.Kind)
        {
          perform((Action) (() => userStatus.PhoneNumberKind = nameInfo.Kind));
          flag1 = true;
        }
        string displayName = nameInfo.Contact.DisplayName;
        if (displayName != userStatus.ContactName)
        {
          perform((Action) (() => userStatus.ContactName = displayName));
          flag1 = true;
        }
        num1 = 10;
        CompleteName completeName = nameInfo.Contact.CompleteName;
        string firstName = (string) null;
        if (completeName != null)
          firstName = completeName.FirstName;
        if (firstName != null && (firstName.Length == 0 || firstName.All<char>(new Func<char, bool>(char.IsWhiteSpace))))
          firstName = (string) null;
        if (firstName != userStatus.FirstName)
        {
          perform((Action) (() => userStatus.FirstName = firstName));
          flag1 = true;
        }
        num1 = 20;
        Stream picture = nameInfo.Contact.GetPicture();
        if (picture != null)
        {
          Log.d(ContactSync.LogHdr, "received photostream {0}", (object) picture.Length);
          bool flag2 = true;
          byte[] newHash = (byte[]) null;
          photoBuffer.Position = 0L;
          photoBuffer.SetLength(0L);
          DateTime utcNow = DateTime.UtcNow;
          picture.CopyTo((Stream) photoBuffer);
          ContactSync.ioTime += DateTime.UtcNow - utcNow;
          num1 = 30;
          using (SHA1Managed shA1Managed = new SHA1Managed())
          {
            DateTime now = DateTime.Now;
            newHash = shA1Managed.ComputeHash(photoBuffer.GetBuffer(), 0, (int) photoBuffer.Length);
            ContactSync.hashTime += DateTime.Now - now;
            Stream stream = (Stream) null;
            try
            {
              if (userStatus.PhotoPath != null)
              {
                long num2 = 0;
                try
                {
                  stream = (Stream) fs.OpenFile(userStatus.PhotoPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
                  num2 = stream.Length;
                }
                catch (Exception ex)
                {
                  stream = (Stream) null;
                }
                if (stream != null && num2 == photoBuffer.Length)
                {
                  byte[] numArray = userStatus.PhotoHash ?? shA1Managed.ComputeHash(stream);
                  bool flag3 = true;
                  if (numArray.Length == newHash.Length)
                  {
                    for (int index = 0; index < numArray.Length; ++index)
                    {
                      if ((int) numArray[index] != (int) newHash[index])
                      {
                        flag3 = false;
                        break;
                      }
                    }
                    flag2 = !flag3;
                  }
                }
                else
                  Log.l(ContactSync.LogHdr, "filestream or length error {0}, {1}", (object) (stream != null), (object) num2);
              }
              else
                Log.l(ContactSync.LogHdr, "Current user file path is null");
            }
            finally
            {
              stream.SafeDispose();
            }
          }
          num1 = 40;
          if (flag2)
          {
            string str = (string) null;
            if (userStatus.PhotoPath != null)
            {
              str = userStatus.PhotoPath;
              perform((Action) (() => userStatus.PhotoPath = (string) null));
              flag1 = true;
            }
            if (photoBuffer != null && photoBuffer.Length != 0L)
            {
              if (!checkedDir)
              {
                if (!fs.DirectoryExists("cphotos"))
                  fs.CreateDirectory("cphotos");
                checkedDir = true;
              }
              num1 = 50;
              int length = userStatus.Jid.IndexOf('@');
              string newFileName = "cphotos/" + (length > 0 ? userStatus.Jid.Substring(0, length) : userStatus.Jid);
              DateTime? start = PerformanceTimer.Start();
              using (IsolatedStorageFileStream storageFileStream = fs.OpenFile(newFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
              {
                storageFileStream.Position = 0L;
                storageFileStream.SetLength(photoBuffer.Length);
                storageFileStream.Write(photoBuffer.GetBuffer(), 0, (int) photoBuffer.Length);
              }
              PerformanceTimer.End("write " + newFileName, start);
              num1 = 60;
              ITile tile = TileHelper.GetChatTile(userStatus.Jid);
              if (tile != null)
                Deployment.Current.Dispatcher.BeginInvoke((Action) (() => TileHelper.UpdateChatTilePicture(tile, userStatus.Jid, newFileName)));
              perform((Action) (() =>
              {
                userStatus.PhotoPath = newFileName;
                userStatus.PhotoHash = newHash;
              }));
              if (newFileName == str)
                str = (string) null;
            }
            else
            {
              perform((Action) (() =>
              {
                userStatus.PhotoPath = (string) null;
                userStatus.PhotoHash = (byte[]) null;
              }));
              flag1 = true;
            }
            if (str != null)
            {
              toDelete.Add(str);
              flag1 = true;
            }
          }
        }
        else if (userStatus.PhotoPath != null)
        {
          perform((Action) (() => userStatus.PhotoHash = (byte[]) null));
          if (userStatus.PhotoPath != null)
          {
            toDelete.Add(userStatus.PhotoPath);
            perform((Action) (() => userStatus.PhotoPath = (string) null));
          }
          flag1 = true;
        }
        num1 = 70;
        if (isInDevicePhonebook != userStatus.IsInDevicePhonebook)
        {
          perform((Action) (() => userStatus.IsInDevicePhonebook = isInDevicePhonebook));
          flag1 = true;
        }
        if (isInDeviceList != userStatus.IsInDeviceContactList)
        {
          perform((Action) (() => userStatus.IsInDeviceContactList = isInDeviceList));
          flag1 = true;
        }
      }
      catch (Exception ex)
      {
        Log.l(ContactSync.LogHdr, "Exception trapped, locInd:{0}, changed:{1}", (object) num1, (object) flag1);
        Log.SendCrashLog(ex, ContactSync.LogHdr + " ex in CopyContactDetails", logOnlyForRelease: true);
      }
      return flag1;
    }

    private static bool DeleteContact(
      ContactsContext db,
      string jid,
      out WaScheduledTask postSubmitTask,
      Action<Action> perform = null)
    {
      postSubmitTask = (WaScheduledTask) null;
      UserStatus userStatus = db.GetUserStatus(jid, false);
      return userStatus != null && ContactSync.ClearContactDetails(db, userStatus, out postSubmitTask, perform);
    }

    private static bool ClearContactDetails(
      ContactsContext db,
      UserStatus user,
      out WaScheduledTask postSubmitTask,
      Action<Action> perform = null)
    {
      postSubmitTask = (WaScheduledTask) null;
      if (user == null)
        return false;
      if (perform == null)
        perform = (Action<Action>) (a => a());
      if (!user.IsInDeviceContactList && !user.IsInDevicePhonebook)
        return false;
      perform((Action) (() =>
      {
        user.IsInDeviceContactList = false;
        user.IsInDevicePhonebook = false;
        user.ContactName = (string) null;
        user.FirstName = (string) null;
      }));
      Settings.StatusRecipientsStateDirty = true;
      WaScheduledTask task = new WaScheduledTask(WaScheduledTask.Types.PostContactRemoved, user.Jid, (byte[]) null, WaScheduledTask.Restrictions.None, new TimeSpan?(TimeSpan.FromDays(7.0)));
      db.InsertScheduledTaskOnSubmit(task);
      postSubmitTask = task;
      return true;
    }

    public class SyncException : Exception
    {
      public SyncException(int code)
        : base("Sync failed with code " + (object) code)
      {
      }

      public SyncException(ContactSync.SyncProcessResult processResult)
        : base("Sync failed with error=" + (object) processResult)
      {
      }
    }

    public enum SyncProcessResult
    {
      Success,
      SuccessNoop,
      ConnectionError,
      InCompleteParsedError,
      NoResponseReceivedError,
      NotRegisteredError,
      BackOffError,
      ServerError,
      OtherError,
      NoContactsFound,
    }

    private class PerformSyncResult
    {
      public ContactSync.SyncProcessResult SyncResult { get; private set; }

      public FunXMPP.Connection.SyncResult ListResult { get; private set; }

      public FunXMPP.Connection.SyncResult SidelistResult { get; private set; }

      public PerformSyncResult(ContactSync.SyncProcessResult syncResult)
        : this(syncResult, (FunXMPP.Connection.SyncResult) null, (FunXMPP.Connection.SyncResult) null)
      {
      }

      public PerformSyncResult(
        FunXMPP.Connection.SyncResult listResult,
        FunXMPP.Connection.SyncResult sidelistResult)
        : this(ContactSync.SyncProcessResult.Success, listResult, sidelistResult)
      {
        listResult.CheckNotNull<FunXMPP.Connection.SyncResult>("Apparently successful Sync Result supplied without a Result?!");
      }

      private PerformSyncResult(
        ContactSync.SyncProcessResult syncResult,
        FunXMPP.Connection.SyncResult listResult,
        FunXMPP.Connection.SyncResult sidelistResult)
      {
        this.SyncResult = syncResult;
        this.ListResult = listResult;
        this.SidelistResult = sidelistResult;
      }

      public bool IsSuccess() => ContactSync.IsSuccessSyncError(this.SyncResult);
    }
  }
}
