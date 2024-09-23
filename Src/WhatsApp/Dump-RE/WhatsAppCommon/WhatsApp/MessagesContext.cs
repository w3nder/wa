// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessagesContext
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public class MessagesContext : SqliteMessagesContext
  {
    public static string[] TablesOnlyCopyRepair = new string[4]
    {
      "MessagesFts_segments",
      "MessagesFts_segdir",
      "MessagesFts_docsize",
      "MessagesFts_stat"
    };
    public static string[] TablesOnlyCreateRepair = new string[1]
    {
      "MessagesFts"
    };
    public static string[] TablesNeedTokenizer = new string[1]
    {
      "MessagesFts"
    };
    public static MutexWithWatchdog Mutex = new MutexWithWatchdog("WhatsAppMsgLock");
    public static object BgLock = new object();
    private static volatile MessagesContext instance = (MessagesContext) null;

    public static void RunRecursive(MessagesContext.MessagesCallback action)
    {
      if (MessagesContext.Mutex.IsOwner())
        action(MessagesContext.instance);
      else
        MessagesContext.Run(action);
    }

    public static void Run(MessagesContext.MessagesCallback action)
    {
      List<MutexWithWatchdog> locks = new List<MutexWithWatchdog>();
      Action<MutexWithWatchdog> action1 = (Action<MutexWithWatchdog>) (m =>
      {
        if (m.IsOwner())
          return;
        m.WaitOne();
        locks.Add(m);
      });
      if (MessagesContext.instance == null)
        AppState.CheckDbValid();
      if (AppState.IsBackgroundAgent)
      {
        MessagesContext.MessagesCallback old = action;
        action = (MessagesContext.MessagesCallback) (p =>
        {
          lock (MessagesContext.BgLock)
          {
            AppState.CheckDbValid();
            old(p);
          }
        });
      }
      try
      {
        action1(MessagesContext.Mutex);
        if (MessagesContext.instance == null)
        {
          AppState.CheckDbValid();
          MessagesContext.instance = MessagesContext.GetInstance();
        }
        DateTime? start = PerformanceTimer.Start();
        action(MessagesContext.instance);
        PerformanceTimer.End("messages DB query", start);
        PerformanceTimer.LogIfExcessive(start, "Took too long holding message lock.");
      }
      catch (Exception ex)
      {
        if ((int) ex.GetHResult() == (int) Sqlite.HRForError(11U))
        {
          Settings.CorruptDb = true;
          AppState.OnDatabaseCorruption();
        }
        throw;
      }
      finally
      {
        for (int index = locks.Count - 1; index >= 0; --index)
          locks[index].ReleaseMutex();
      }
    }

    public static T Select<T>(Func<MessagesContext, T> func)
    {
      T r = default (T);
      MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db => r = func(db)));
      return r;
    }

    private static MessagesContext GetInstance()
    {
      MessagesContext instance = new MessagesContext();
      if (!instance.DatabaseExists())
        instance.CreateDatabase();
      return instance;
    }

    public static void Reset(bool force = false)
    {
      if (MessagesContext.instance == null || !force && !MessagesContext.instance.ResetImpl())
        return;
      MessagesContext.instance.Dispose();
      MessagesContext.instance = (MessagesContext) null;
    }

    public static void Delete()
    {
      MessagesContext.Mutex.WaitOne();
      try
      {
        if (MessagesContext.instance == null)
          return;
        MessagesContext.instance.DeleteDatabase();
        MessagesContext.instance = (MessagesContext) null;
      }
      finally
      {
        MessagesContext.Mutex.ReleaseMutex();
      }
    }

    public MessagesContext()
      : base()
    {
    }

    public static class Events
    {
      public static Subject<Sticker> SavedStickerChangedSubject = new Subject<Sticker>();
      public static Subject<Message> NewMessagesSubject = new Subject<Message>();
      public static Subject<DbDataUpdate> MessageUpdateSubject = new Subject<DbDataUpdate>();
      public static Subject<Message> UpdatedMessagesMediaWaTypeSubject = new Subject<Message>();
      public static Subject<Message> DeletedMessagesSubject = new Subject<Message>();
      public static Subject<ConvoAndMessage> UpdatedConversationSubject = new Subject<ConvoAndMessage>();
      public static Subject<Conversation> DeletedConversationSubject = new Subject<Conversation>();
      public static Subject<WhatsApp.ReceiptState> NewReceiptSubject = new Subject<WhatsApp.ReceiptState>();
      public static Subject<DbDataUpdate> JidInfoUpdateSubject = new Subject<DbDataUpdate>();
    }

    public delegate void MessagesCallback(MessagesContext context);
  }
}
