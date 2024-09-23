// Decompiled with JetBrains decompiler
// Type: WhatsAppCommon.ContactSyncManager
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp;

#nullable disable
namespace WhatsAppCommon
{
  internal class ContactSyncManager
  {
    private static readonly string LogHdr = "ContactSyncMgr";
    private static WorkQueue contactSyncWorker = (WorkQueue) null;
    private static readonly object lockObject = new object();
    private static bool IsOfflineListenerInit = false;
    private static ContactSyncManager.UserRequest currentRequest = (ContactSyncManager.UserRequest) null;
    private static Queue<ContactSyncManager.UserRequest> requests = new Queue<ContactSyncManager.UserRequest>();
    private static long requestProcessingStartTimeTicks = -1;

    private static WorkQueue ContactSyncWorker
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref ContactSyncManager.contactSyncWorker, (Func<WorkQueue>) (() => new WorkQueue(identifierString: nameof (ContactSyncWorker))));
      }
    }

    public static void EnqueueRequestPhonebookWithRetry(
      FunXMPP.Connection.SyncMode? mode,
      FunXMPP.Connection.SyncContext context,
      Action<ContactSync.SyncProcessResult> onError = null,
      Action onSuccess = null)
    {
      ContactSyncManager.EnqueueRequestPhonebook(mode, context, true, onError, onSuccess);
    }

    private static void EnqueueRequestPhonebook(
      FunXMPP.Connection.SyncMode? mode,
      FunXMPP.Connection.SyncContext context,
      bool allowRetry,
      Action<ContactSync.SyncProcessResult> onError = null,
      Action onSuccess = null)
    {
      ContactSyncManager.Callbacks listener = new ContactSyncManager.Callbacks(allowRetry, onError, onSuccess);
      ContactSyncManager.UserRequest phonebookSyncRequest = ContactSyncManager.UserRequest.createPhonebookSyncRequest(mode, context, listener);
      lock (ContactSyncManager.lockObject)
        ContactSyncManager.requests.Enqueue(phonebookSyncRequest);
      Log.l(ContactSyncManager.LogHdr, "mode={0}, context={1}, enqueue new sync request", (object) mode, (object) context);
      ContactSyncManager.KickThreadProcessor();
    }

    public static void EnqueueRequestPhonebookNoDuplicate(
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context,
      Action<ContactSync.SyncProcessResult> onError = null,
      Action onSuccess = null)
    {
      ContactSyncManager.EnqueueRequestPhonebookNoDuplicate(mode, context, false, onError, onSuccess);
    }

    public static void EnqueueRequestPhonebookNoDuplicateWithRetry(
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context)
    {
      ContactSyncManager.EnqueueRequestPhonebookNoDuplicate(mode, context, true);
    }

    private static void EnqueueRequestPhonebookNoDuplicate(
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context,
      bool allowRetry,
      Action<ContactSync.SyncProcessResult> onError = null,
      Action onSuccess = null)
    {
      ContactSyncManager.UserRequest userRequest = ContactSyncManager.LookupRequest(mode, context);
      if (userRequest != null)
      {
        ContactSyncManager.Callbacks listener = new ContactSyncManager.Callbacks(allowRetry, onError, onSuccess);
        if (userRequest.Add(listener))
        {
          Log.l(ContactSyncManager.LogHdr, "mode={0}, context={1}, dedup merge", (object) mode, (object) context);
          return;
        }
      }
      ContactSyncManager.EnqueueRequestPhonebook(new FunXMPP.Connection.SyncMode?(mode), context, allowRetry, onError, onSuccess);
    }

    public static void EnqueueRequestNotificationWithRetry(
      List<string> matchingNumbers,
      FunXMPP.Connection.ContactNotificationType notificationType,
      Action<ContactSync.SyncProcessResult> onError = null,
      Action onSuccess = null)
    {
      ContactSyncManager.Callbacks listener = new ContactSyncManager.Callbacks(true, onError, onSuccess);
      ContactSyncManager.UserRequest notificationRequest = ContactSyncManager.UserRequest.createNotificationRequest(matchingNumbers, listener, notificationType);
      lock (ContactSyncManager.lockObject)
        ContactSyncManager.requests.Enqueue(notificationRequest);
      Log.l(ContactSyncManager.LogHdr, "enqueue new notification request, numbers={0}", (object) matchingNumbers.Count<string>());
      ContactSyncManager.KickThreadProcessor();
    }

    private static ContactSyncManager.UserRequest LookupRequest(
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context)
    {
      lock (ContactSyncManager.lockObject)
      {
        FunXMPP.Connection.SyncMode? mode1;
        FunXMPP.Connection.SyncContext context1;
        if (ContactSyncManager.currentRequest != null)
        {
          mode1 = ContactSyncManager.currentRequest.Mode;
          if (mode1.Equals((object) mode))
          {
            context1 = ContactSyncManager.currentRequest.Context;
            if (context1.Equals((object) context))
              return ContactSyncManager.currentRequest;
          }
        }
        foreach (ContactSyncManager.UserRequest request in ContactSyncManager.requests)
        {
          mode1 = request.Mode;
          if (mode1.Equals((object) mode))
          {
            context1 = request.Context;
            if (context1.Equals((object) context))
              return request;
          }
        }
      }
      return (ContactSyncManager.UserRequest) null;
    }

    private static void MaybeSetupOfflineListener()
    {
      lock (ContactSyncManager.lockObject)
      {
        if (!ContactSyncManager.IsOfflineListenerInit)
          FunXMPP.OfflineMarkerSubject.SimpleObserveOn<Unit>((IScheduler) AppState.Worker).Subscribe<Unit>((Action<Unit>) (u =>
          {
            Log.d(ContactSyncManager.LogHdr, "offline marker run");
            ContactSyncManager.KickThreadProcessor();
          }));
        ContactSyncManager.IsOfflineListenerInit = true;
      }
    }

    private static void KickThreadProcessor()
    {
      ContactSyncManager.ContactSyncWorker.Enqueue((Action) (() =>
      {
        try
        {
          ContactSyncManager.Process(DateTime.UtcNow.Ticks);
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "Exception running Sync Process");
        }
      }), filterException: false);
      ContactSyncManager.MaybeSetupOfflineListener();
    }

    private static void Process(long startTimeTicks)
    {
      ContactSyncManager.UserRequest userRequest;
      while (true)
      {
        ContactSync.SyncProcessResult syncProcessResult;
        do
        {
          userRequest = (ContactSyncManager.UserRequest) null;
          lock (ContactSyncManager.lockObject)
          {
            bool flag = ContactSyncManager.requestProcessingStartTimeTicks == -1L || ContactSyncManager.requestProcessingStartTimeTicks == startTimeTicks;
            if (flag && ContactSyncManager.requests.Count<ContactSyncManager.UserRequest>() > 0)
            {
              if (ContactSyncManager.requestProcessingStartTimeTicks == -1L)
              {
                Log.l(ContactSyncManager.LogHdr, "Starting process for {0}", (object) startTimeTicks);
                ContactSyncManager.requestProcessingStartTimeTicks = startTimeTicks;
              }
              ContactSyncManager.currentRequest = ContactSyncManager.requests.Dequeue();
              userRequest = ContactSyncManager.currentRequest;
            }
            else
            {
              if (!flag)
              {
                Log.l(ContactSyncManager.LogHdr, "Not running process {0} in favor of {1}", (object) startTimeTicks, (object) ContactSyncManager.requestProcessingStartTimeTicks);
                return;
              }
              Log.d(ContactSyncManager.LogHdr, "Stopping process {0} as there is nothing to do", (object) startTimeTicks);
              ContactSyncManager.currentRequest = (ContactSyncManager.UserRequest) null;
              ContactSyncManager.requestProcessingStartTimeTicks = -1L;
              return;
            }
          }
          Log.l(ContactSyncManager.LogHdr, "type={0}, mode={1}, context={2}, attempting", (object) userRequest.Type, (object) userRequest.Mode, (object) userRequest.Context);
          try
          {
            switch (userRequest.Type)
            {
              case ContactSyncManager.SyncType.Phonebook:
                syncProcessResult = ContactSync.PhonebookSync(userRequest.Mode, userRequest.Context);
                break;
              case ContactSyncManager.SyncType.Notification:
                userRequest.MatchingNumbers.CheckNotNull<List<string>>("MatchingNumbers is unexpectedly null");
                userRequest.NotificationType.CheckNotNull<FunXMPP.Connection.ContactNotificationType?>("NotificationType is unexpectedly null");
                if (userRequest.MatchingNumbers.Count == 0)
                  throw new ArgumentException("notification sync has no matching numbers");
                syncProcessResult = ContactSync.NotificationSync(userRequest.Mode.Value, userRequest.Context, userRequest.MatchingNumbers, userRequest.NotificationType.Value);
                break;
              default:
                syncProcessResult = ContactSync.SyncProcessResult.OtherError;
                break;
            }
          }
          catch (Exception ex)
          {
            string context = ContactSyncManager.LogHdr + " Exception Processing " + userRequest.Type.ToString();
            Log.l(ex, context);
            Log.SendCrashLog(ex, "crash in contactsync", logOnlyForRelease: true);
            syncProcessResult = ContactSync.SyncProcessResult.OtherError;
          }
          Log.l(ContactSyncManager.LogHdr, "type={0}, mode={1}, context={2}, result={3}", (object) userRequest.Type, (object) userRequest.Mode, (object) userRequest.Context, (object) syncProcessResult);
          if (ContactSyncManager.currentRequest != userRequest)
          {
            Log.l(ContactSyncManager.LogHdr, "Current({0}) != Request({1})", ContactSyncManager.currentRequest == null ? (object) "null" : (object) "exists", userRequest == null ? (object) "null" : (object) "exists");
            Log.SendCrashLog(new Exception("unexpected race condition processing Sync"), "Async race condition", logOnlyForRelease: true);
          }
          if (ContactSync.IsSuccessSyncError(syncProcessResult))
            userRequest.NotifySuccess();
          else if (!ContactSync.isRetryableError(syncProcessResult))
            goto label_33;
        }
        while (userRequest.NotifyFailureIfNotRetryable(syncProcessResult));
        break;
label_33:
        userRequest.NotifyFailure(syncProcessResult);
      }
      Log.l(ContactSyncManager.LogHdr, "type={0}, mode={1}, context={2}, requeueing", (object) userRequest.Type, (object) userRequest.Mode, (object) userRequest.Context);
      lock (ContactSyncManager.lockObject)
      {
        ContactSyncManager.requests.Enqueue(userRequest);
        Log.d(ContactSyncManager.LogHdr, "Stopping process {0} as there is no connection", (object) startTimeTicks);
        ContactSyncManager.currentRequest = (ContactSyncManager.UserRequest) null;
        ContactSyncManager.requestProcessingStartTimeTicks = -1L;
      }
      AppState.GetConnection()?.InvokeWhenConnected((Action) (() =>
      {
        Log.l(ContactSyncManager.LogHdr, "regained connection, restarting processor");
        ContactSyncManager.KickThreadProcessor();
      }));
    }

    private class Callbacks
    {
      public Action OnComplete { get; private set; }

      public Action<ContactSync.SyncProcessResult> OnError { get; private set; }

      public bool AllowRetry { get; private set; }

      public Callbacks(
        bool allowRetry,
        Action<ContactSync.SyncProcessResult> onError = null,
        Action onComplete = null)
      {
        this.AllowRetry = allowRetry;
        this.OnComplete = onComplete;
        this.OnError = onError;
      }
    }

    private enum SyncType
    {
      Phonebook,
      Notification,
    }

    private sealed class UserRequest
    {
      private bool isDone;

      public ContactSyncManager.SyncType Type { get; private set; }

      public FunXMPP.Connection.SyncMode? Mode { get; private set; }

      public FunXMPP.Connection.SyncContext Context { get; private set; }

      public List<ContactSyncManager.Callbacks> Listeners { get; }

      public FunXMPP.Connection.ContactNotificationType? NotificationType { get; private set; }

      public List<string> MatchingNumbers { get; private set; }

      public static ContactSyncManager.UserRequest createPhonebookSyncRequest(
        FunXMPP.Connection.SyncMode? mode,
        FunXMPP.Connection.SyncContext context,
        ContactSyncManager.Callbacks listener)
      {
        Assert.IsFalse(context == FunXMPP.Connection.SyncContext.Notification, "notification type not allowed in phonebook sync requests");
        return new ContactSyncManager.UserRequest(ContactSyncManager.SyncType.Phonebook, mode, context, listener, (List<string>) null, new FunXMPP.Connection.ContactNotificationType?());
      }

      public static ContactSyncManager.UserRequest createNotificationRequest(
        List<string> matchingNumbers,
        ContactSyncManager.Callbacks listener,
        FunXMPP.Connection.ContactNotificationType notificationType)
      {
        Assert.IsTrue(matchingNumbers != null, "null matching numbers input");
        Assert.IsTrue(matchingNumbers.Count<string>() > 0, "empty matching numbers input");
        int num = (int) notificationType.CheckNotNull<FunXMPP.Connection.ContactNotificationType>("notification action type is null");
        return new ContactSyncManager.UserRequest(ContactSyncManager.SyncType.Notification, new FunXMPP.Connection.SyncMode?(FunXMPP.Connection.SyncMode.Delta), FunXMPP.Connection.SyncContext.Notification, listener, matchingNumbers, new FunXMPP.Connection.ContactNotificationType?(notificationType));
      }

      public UserRequest(
        ContactSyncManager.SyncType type,
        FunXMPP.Connection.SyncMode? mode,
        FunXMPP.Connection.SyncContext context,
        ContactSyncManager.Callbacks listener,
        List<string> matchingNumbers,
        FunXMPP.Connection.ContactNotificationType? notificationType)
      {
        this.Type = type;
        this.Mode = mode;
        this.Context = context;
        this.Listeners = new List<ContactSyncManager.Callbacks>();
        this.Listeners.Add(listener);
        this.MatchingNumbers = matchingNumbers;
        this.NotificationType = notificationType;
        this.isDone = false;
      }

      public bool Add(ContactSyncManager.Callbacks listener)
      {
        lock (this)
        {
          if (this.isDone)
            return false;
          this.Listeners.Add(listener);
          return true;
        }
      }

      public void NotifySuccess()
      {
        lock (this)
        {
          foreach (ContactSyncManager.Callbacks listener in this.Listeners)
          {
            try
            {
              Action onComplete = listener.OnComplete;
              if (onComplete != null)
                onComplete();
            }
            catch (Exception ex)
            {
              Log.l(ContactSyncManager.LogHdr, "Exception notifying success to listener for {0} {1}", (object) this.Type, (object) this.Mode, (object) this.Context);
              Log.SendCrashLog(ex, "Exception notifying success to listener", logOnlyForRelease: true);
            }
          }
          this.isDone = true;
        }
      }

      public void NotifyFailure(ContactSync.SyncProcessResult processResult)
      {
        lock (this)
        {
          foreach (ContactSyncManager.Callbacks listener in this.Listeners)
          {
            try
            {
              Action<ContactSync.SyncProcessResult> onError = listener.OnError;
              if (onError != null)
                onError(processResult);
            }
            catch (Exception ex)
            {
              Log.l(ContactSyncManager.LogHdr, "Exception notifying failure to listener for {0} {1}", (object) this.Type, (object) this.Mode, (object) this.Context);
              Log.SendCrashLog(ex, "Exception notifying failure to listener", logOnlyForRelease: true);
            }
          }
          this.isDone = true;
        }
      }

      public bool NotifyFailureIfNotRetryable(ContactSync.SyncProcessResult processResult)
      {
        lock (this)
        {
          List<ContactSyncManager.Callbacks> callbacksList = new List<ContactSyncManager.Callbacks>();
          foreach (ContactSyncManager.Callbacks listener in this.Listeners)
          {
            if (!listener.AllowRetry)
            {
              Action<ContactSync.SyncProcessResult> onError = listener.OnError;
              if (onError != null)
                onError(processResult);
              callbacksList.Add(listener);
            }
          }
          foreach (ContactSyncManager.Callbacks callbacks in callbacksList)
            this.Listeners.Remove(callbacks);
          this.isDone = this.Listeners.Count<ContactSyncManager.Callbacks>() == 0;
          return this.isDone;
        }
      }
    }
  }
}
