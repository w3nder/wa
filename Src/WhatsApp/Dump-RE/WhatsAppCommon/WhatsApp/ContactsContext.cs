// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactsContext
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public class ContactsContext : SqliteContactsContext
  {
    public static MutexWithWatchdog Mutex = new MutexWithWatchdog("WhatsAppContactsLock");
    public static object BgLock = new object();
    private static ContactsContext instance = (ContactsContext) null;

    public static T Instance<T>(Func<ContactsContext, T> func)
    {
      if (ContactsContext.instance == null)
        AppState.CheckDbValid();
      if (AppState.IsBackgroundAgent)
      {
        Func<ContactsContext, T> old = func;
        func = (Func<ContactsContext, T>) (p =>
        {
          lock (ContactsContext.BgLock)
          {
            AppState.CheckDbValid();
            return old(p);
          }
        });
      }
      ContactsContext.Mutex.WaitOne();
      try
      {
        DateTime? start = PerformanceTimer.Start();
        if (ContactsContext.instance == null)
        {
          AppState.CheckDbValid();
          ContactsContext.instance = ContactsContext.GetInstance();
        }
        T obj = func(ContactsContext.instance);
        PerformanceTimer.End("Contacts DB query", start);
        PerformanceTimer.LogIfExcessive(start, "Took too long holding contacts lock.");
        return obj;
      }
      finally
      {
        ContactsContext.Mutex.ReleaseMutex();
      }
    }

    public static void Instance(Action<ContactsContext> func)
    {
      ContactsContext.Instance<Unit>((Func<ContactsContext, Unit>) (c =>
      {
        func(c);
        return new Unit();
      }));
    }

    public static void Reset(bool force = false)
    {
      if (ContactsContext.instance == null || !force && !ContactsContext.instance.ResetImpl())
        return;
      ContactsContext.instance.Dispose();
      ContactsContext.instance = (ContactsContext) null;
    }

    public static void Delete()
    {
      ContactsContext.Mutex.WaitOne();
      try
      {
        if (ContactsContext.instance == null)
          return;
        ContactsContext.instance.DeleteDatabase();
        ContactsContext.instance = (ContactsContext) null;
      }
      finally
      {
        ContactsContext.Mutex.ReleaseMutex();
      }
    }

    private static ContactsContext GetInstance()
    {
      ContactsContext instance = new ContactsContext();
      if (!instance.DatabaseExists())
        instance.CreateDatabase();
      return instance;
    }

    public ContactsContext()
      : base()
    {
    }

    public static class Events
    {
      public static Subject<DbDataUpdate> UserStatusUpdatedSubject = new Subject<DbDataUpdate>();
      public static Subject<Unit> StatusUpdateSubject = new Subject<Unit>();
      public static Subject<SqliteContactsContext.ChangeNumberAction> ChangeNumberActionSubject = new Subject<SqliteContactsContext.ChangeNumberAction>();
      public static ReplaySubject<Unit> ColdSyncErrorSubject = new ReplaySubject<Unit>();
      public static Subject<IEnumerable<string>> BlockListUpdateSubject = new Subject<IEnumerable<string>>();
    }
  }
}
