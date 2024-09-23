// Decompiled with JetBrains decompiler
// Type: WhatsApp.Stats
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using WhatsApp.Events;
using WhatsAppNative;
using Windows.System;

#nullable disable
namespace WhatsApp
{
  public class Stats
  {
    public Dictionary<string, Dictionary<string, string>> Values = new Dictionary<string, Dictionary<string, string>>();
    private static MutexWithWatchdog Mutex = new MutexWithWatchdog("WhatsApp.StatsMutex", false);
    private static string DbName = Constants.IsoStorePath + "\\stats.db";
    private static string StatsFlatFile = "stats";
    private const int ReceivedIdx = 0;
    private const int DroppedIdx = 1;
    public static int DroppedThisSession = 0;
    private static bool CareAboutOffline = true;
    private static long fsCollectionTimeTicks = 0;
    private const int fsStartDelayMins = 1;
    private const int fsIntervalMins = 15;

    private static IObservable<T> WrapDisposable<T>(IObservable<T> source) where T : IDisposable
    {
      return Observable.CreateWithDisposable<T>((Func<IObserver<T>, IDisposable>) (observer => source.Subscribe<T>((Action<T>) (_ =>
      {
        using (_)
        {
          try
          {
            observer.OnNext(_);
          }
          catch (Exception ex)
          {
            observer.OnError(ex);
          }
        }
      }), (Action<Exception>) (ex => observer.OnError(ex)), (Action) (() => observer.OnCompleted()))));
    }

    private static IObservable<Sqlite> PerformWithDb(bool write = false)
    {
      IObservable<Sqlite> source = Stats.WrapDisposable<Sqlite>(Observable.Defer<Sqlite>((Func<IObservable<Sqlite>>) (() => Observable.Return<Sqlite>(new Sqlite(Stats.DbName, write ? SqliteOpenFlags.Defaults : SqliteOpenFlags.READONLY)))));
      if (write)
      {
        IObservable<Sqlite> old = source.Do<Sqlite>((Action<Sqlite>) (sql =>
        {
          bool flag = false;
          using (Sqlite.PreparedStatement stmt = sql.PrepareStatement("select exists(select tbl_name from sqlite_master where type = 'table' and tbl_name = 'PushesReceived')"))
            flag = (Stats.ParseIntegerQuery(stmt) ?? 0L) != 0L;
          if (flag)
            return;
          string[] strArray = new string[4]
          {
            "BEGIN TRANSACTION",
            "CREATE TABLE IF NOT EXISTS PushesReceived\n(Jid TEXT, Id TEXT)",
            "CREATE UNIQUE INDEX IF NOT EXISTS PushesReceivedIdx\nON PushesReceived (Jid, Id)",
            "COMMIT TRANSACTION"
          };
          foreach (string sql1 in strArray)
          {
            using (Sqlite.PreparedStatement preparedStatement = sql.PrepareStatement(sql1))
              preparedStatement.Step();
          }
        }));
        source = Observable.CreateWithDisposable<Sqlite>((Func<IObserver<Sqlite>, IDisposable>) (observer =>
        {
          IDisposable disp = (IDisposable) null;
          Stats.Mutex.PerformWithLock((Action) (() => disp = old.Subscribe(observer)));
          return disp;
        }));
      }
      return source;
    }

    private static void PerformWithDb(Action<Sqlite> a, bool write = false)
    {
      DateTime? start = PerformanceTimer.Start();
      Stats.PerformWithDb(write).Subscribe<Sqlite>(a);
      PerformanceTimer.End("stats db", start);
    }

    public static void OnUpgrade()
    {
      try
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile(Stats.StatsFlatFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
            storageFileStream.SetLength(0L);
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "reset stats");
      }
    }

    private static void Inc(int idx)
    {
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile(Stats.StatsFlatFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
        {
          long num = 0;
          if (storageFileStream.Length >= (long) ((idx + 1) * 8))
          {
            storageFileStream.Position = (long) (idx * 8);
            num = new BinaryReader((Stream) storageFileStream).ReadInt64();
          }
          storageFileStream.Position = (long) (idx * 8);
          BinaryWriter binaryWriter = new BinaryWriter((Stream) storageFileStream);
          binaryWriter.Write(num + 1L);
          binaryWriter.Flush();
        }
      }
    }

    public static void OnPush(string jid, string id)
    {
      Stats.PerformWithDb((Action<Sqlite>) (sql =>
      {
        using (Sqlite.PreparedStatement preparedStatement = sql.PrepareStatement("INSERT INTO PushesReceived (Jid, Id) VALUES (?, ?)"))
        {
          preparedStatement.Bind(0, jid);
          preparedStatement.Bind(1, id);
          try
          {
            preparedStatement.Step();
          }
          catch (Exception ex)
          {
            if ((int) ex.GetHResult() == (int) Sqlite.HRForError(19U))
              return;
            throw;
          }
        }
      }), true);
      Stats.Inc(0);
    }

    public static void OnOfflineMessage(string jid, string id)
    {
      if (!Stats.CareAboutOffline)
        return;
      Action increment = (Action) (() =>
      {
        Log.WriteLineDebug("Stats ib - recording push as dropped <{0},{1}>", (object) jid, (object) id);
        Stats.Inc(1);
        ++Stats.DroppedThisSession;
      });
      Stats.PerformWithDb().Subscribe<Sqlite>((Action<Sqlite>) (sql =>
      {
        int num = 0;
        using (Sqlite.PreparedStatement preparedStatement = sql.PrepareStatement("SELECT EXISTS(SELECT * FROM PushesReceived WHERE Jid = ? AND Id = ?)"))
        {
          preparedStatement.Bind(0, jid);
          preparedStatement.Bind(1, id);
          if (preparedStatement.Step())
            num = (int) (long) preparedStatement.Columns[0];
        }
        if (num == 0)
          increment();
        else
          Stats.CareAboutOffline = false;
      }), (Action<Exception>) (ex =>
      {
        increment();
        if (ex.GetHResults().Where<uint>((Func<uint, bool>) (x => (int) x == (int) Sqlite.HRForError(14U) || (int) x == (int) Sqlite.HRForError(1U))).Any<uint>())
          return;
        Log.LogException(ex, "stats");
      }));
    }

    public static void OnConnected() => Stats.CareAboutOffline = AppState.IsVoipScheduled();

    public static void OnOfflineBurstReset() => Stats.DroppedThisSession = 0;

    private static long? ParseIntegerQuery(Sqlite.PreparedStatement stmt)
    {
      long? integerQuery = new long?();
      if (stmt.Step())
      {
        object[] columns = stmt.Columns;
        if (columns != null)
        {
          object obj = columns[0];
          if (obj != null)
            integerQuery = new long?((long) obj);
        }
      }
      return integerQuery;
    }

    private static void GetMemoryUsage(out long current, out long peak, out long max)
    {
      if (!AppState.IsWP10OrLater)
      {
        try
        {
          NativeInterfaces.Misc.GetMemoryUsage(out current, out peak, out max);
          max = (long) MemoryManager.AppMemoryUsageLimit;
          return;
        }
        catch (Exception ex)
        {
        }
      }
      max = (long) MemoryManager.AppMemoryUsageLimit;
      current = (long) MemoryManager.AppMemoryUsage;
      peak = 0L;
    }

    public static Stats.MemoryUsage LogMemoryUsage(string context = null, bool gc = false)
    {
      if (gc)
        GC.Collect();
      Stats.MemoryUsage memoryUsage = new Stats.MemoryUsage();
      try
      {
        long current;
        long peak;
        long max;
        Stats.GetMemoryUsage(out current, out peak, out max);
        if (max != 0L)
        {
          long num1 = current * 100L / max;
          long num2 = peak * 100L / max;
          memoryUsage.Current = num1;
          memoryUsage.Peak = num2;
          if (context == null)
            Log.WriteLineDebug("Memory usage: {0}% current, {1}% peak", (object) num1, (object) num2);
          else
            Log.WriteLineDebug("Memory usage: {0}% current, {1}% peak | {2}", (object) num1, (object) num2, (object) context);
          if (Stats.fsCollectionTimeTicks == 0L)
          {
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddMinutes(1.0);
            Stats.fsCollectionTimeTicks = dateTime.Ticks;
          }
          else
          {
            DateTime dateTime = DateTime.Now;
            if (dateTime.Ticks > Stats.fsCollectionTimeTicks)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddMinutes(15.0);
              Stats.fsCollectionTimeTicks = dateTime.Ticks;
              new MemoryStat()
              {
                processType = (AppState.IsBackgroundAgent ? "BG" : "FG"),
                workingSetSize = new double?((double) current),
                workingSetPeakSize = new double?((double) max)
              }.SaveEvent();
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return memoryUsage;
    }

    public static Stats CollectPttStats(Message m, string remarks = null)
    {
      Stats stats = new Stats();
      if (!m.IsPtt())
        return stats;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("sender_jid", m.GetSenderJid() ?? "");
      dictionary.Add("convo_jid", m.KeyRemoteJid);
      dictionary.Add("media_url", m.MediaUrl);
      dictionary.Add("local_file", m.LocalFileUri);
      if (!string.IsNullOrEmpty(remarks))
        dictionary.Add(nameof (remarks), remarks);
      if (dictionary.Count > 0)
        stats.Values.Add("wp_ptt", dictionary);
      return stats;
    }

    public struct MemoryUsage
    {
      public long Current;
      public long Peak;
    }
  }
}
