// Decompiled with JetBrains decompiler
// Type: WhatsApp.StaleMediaSweeper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO.IsolatedStorage;


namespace WhatsApp
{
  public class StaleMediaSweeper
  {
    private StaleMediaSweeper.IConfig config;
    private static bool sweepInProgress;

    public StaleMediaSweeper(StaleMediaSweeper.IConfig config = null)
    {
      this.config = config ?? (StaleMediaSweeper.IConfig) new StaleMediaSweeper.SettingsConfig();
    }

    public void Sweep(Action onComplete = null, Action<Exception> onError = null)
    {
      if (StaleMediaSweeper.sweepInProgress)
      {
        Action action = onComplete;
        if (action == null)
          return;
        action();
      }
      else
      {
        DateTime end = DateTime.UtcNow.AddDays(-1.0);
        DateTime? start = (DateTime?) this.config?.LastSweep;
        DateTime? nullable1 = start;
        DateTime utcNow = DateTime.UtcNow;
        if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() > utcNow ? 1 : 0) : 0) != 0)
        {
          Log.l("sweeper", "Last sweep in the future; assuming full scan");
          start = new DateTime?();
        }
        DateTime dateTime = end;
        DateTime? nullable2 = start;
        if ((nullable2.HasValue ? (dateTime < nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          Log.l("sweeper", "Last sweep too recent; nothing to do");
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }
        else
        {
          string transfersPath = "shared\\transfers";
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            if (!storeForApplication.DirectoryExists(transfersPath))
            {
              Log.l("sweeper", "Transfers dir does not exist; exiting");
              Action action = onComplete;
              if (action == null)
                return;
              action();
              return;
            }
          }
          bool hitException = false;
          WorkQueue queue = new WorkQueue(flags: WorkQueue.StartFlags.WatchdogExcempt);
          try
          {
            StaleMediaSweeper.sweepInProgress = true;
            Log.l("sweeper", "Starting scan of iso store for orphaned download files...");
            Action<Action> action1 = this.config == null ? (Action<Action>) (a => a()) : new Action<Action>(this.config.Perform);
            StaleMediaSweeper.IConfig config = this.config;
            bool scanned = (config != null ? (config.LastSweep.HasValue ? 1 : 0) : 0) != 0;
            action1((Action) (() => NativeInterfaces.Misc.EnumerateFilesInRange(Constants.IsoStorePath + "\\" + transfersPath, start, new DateTime?(end), (Action<string>) (filename =>
            {
              if (!scanned)
              {
                MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.ScanLocalFiles(schedule: (Action<Action>) (a => queue.Enqueue(a, WorkQueue.Priority.Interrupt)))));
                scanned = true;
              }
              queue.Enqueue((Action) (() =>
              {
                try
                {
                  this.ProcessFile(filename);
                }
                catch (Exception ex)
                {
                  Action<Exception> action3 = onError;
                  if (action3 == null)
                    return;
                  action3(ex);
                }
              }));
            }))));
          }
          catch (Exception ex)
          {
            Action<Exception> action = onError;
            if (action != null)
              action(ex);
            hitException = true;
          }
          finally
          {
            StaleMediaSweeper.sweepInProgress = false;
            queue.Stop(onStop: (Action) (() =>
            {
              Log.l("sweeper", "Shutting down. Errors: " + hitException.ToString());
              if (!hitException && this.config != null)
                this.config.LastSweep = new DateTime?(end);
              Action action = onComplete;
              if (action == null)
                return;
              action();
            }));
          }
        }
      }
    }

    private void ProcessFile(string filename)
    {
      bool fileExistsInDb = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        LocalFile localFileByUri = db.GetLocalFileByUri(filename);
        int num1;
        if (localFileByUri != null)
        {
          int? referenceCount = localFileByUri.ReferenceCount;
          int num2 = 0;
          num1 = referenceCount.GetValueOrDefault() == num2 ? (!referenceCount.HasValue ? 1 : 0) : 1;
        }
        else
          num1 = 0;
        fileExistsInDb = num1 != 0;
      }));
      if (fileExistsInDb)
        return;
      Log.l("sweeper", "deleting stale file: " + filename);
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        nativeMediaStorage.DeleteFile(filename);
    }

    public interface IConfig
    {
      DateTime? LastSweep { get; set; }

      void Perform(Action a);
    }

    public class SettingsConfig : StaleMediaSweeper.IConfig
    {
      public DateTime? LastSweep
      {
        get => Settings.LastMediaSweepTime;
        set => Settings.LastMediaSweepTime = value;
      }

      public void Perform(Action a)
      {
        if (!AppState.IsBackgroundAgent)
          NativeInterfaces.Misc.LowerPriority(a.AsComAction());
        else
          a();
      }
    }
  }
}
