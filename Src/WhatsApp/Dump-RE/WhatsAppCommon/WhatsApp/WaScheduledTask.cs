// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaScheduledTask
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "TaskType,LookupKey", Name = "TypeAndLookupKey")]
  public class WaScheduledTask : PropChangingBase
  {
    private const string LogHeader = "watask";
    private int taskType;
    private string lookupKey;
    private byte[] binaryData;
    private int attempts;
    private int? attemptsLimit;
    private DateTime? expirationUtc;
    private int restriction;
    private static Set<string> pendingAttempts = new Set<string>(true);

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int TaskID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int TaskType
    {
      get => this.taskType;
      set
      {
        if (this.taskType != 0)
          return;
        this.NotifyPropertyChanging(nameof (TaskType));
        this.taskType = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string LookupKey
    {
      get => this.lookupKey;
      set
      {
        if (!(this.lookupKey != value))
          return;
        this.NotifyPropertyChanging(nameof (LookupKey));
        this.lookupKey = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] BinaryData
    {
      get => this.binaryData;
      set
      {
        if (this.binaryData == value)
          return;
        this.NotifyPropertyChanging(nameof (BinaryData));
        this.binaryData = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int Attempts
    {
      get => this.attempts;
      set
      {
        if (this.attempts == value)
          return;
        this.NotifyPropertyChanging(nameof (Attempts));
        this.attempts = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? AttemptsLimit
    {
      get => this.attemptsLimit;
      set
      {
        int? attemptsLimit = this.attemptsLimit;
        int? nullable = value;
        if ((attemptsLimit.GetValueOrDefault() == nullable.GetValueOrDefault() ? (attemptsLimit.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (AttemptsLimit));
        this.attemptsLimit = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? ExpirationUtc
    {
      get => this.expirationUtc;
      set
      {
        DateTime? expirationUtc = this.expirationUtc;
        DateTime? nullable = value;
        if ((expirationUtc.HasValue == nullable.HasValue ? (expirationUtc.HasValue ? (expirationUtc.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (ExpirationUtc));
        this.expirationUtc = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int Restriction
    {
      get => this.restriction;
      set
      {
        if (this.restriction == value)
          return;
        this.NotifyPropertyChanging(nameof (Restriction));
        this.restriction = value;
      }
    }

    public bool IsDeleted { get; set; }

    public bool IsExpired
    {
      get => this.ExpirationUtc.HasValue && DateTime.UtcNow > this.ExpirationUtc.Value;
    }

    public bool IsAttemptsLimitReached
    {
      get => this.AttemptsLimit.HasValue && this.Attempts > this.AttemptsLimit.Value;
    }

    public WaScheduledTask()
    {
    }

    public WaScheduledTask(
      WaScheduledTask.Types type,
      string lookupKey,
      byte[] data,
      WaScheduledTask.Restrictions restriction,
      TimeSpan? expiration)
    {
      this.TaskType = (int) type;
      this.LookupKey = lookupKey;
      this.BinaryData = data;
      this.Restriction = (int) restriction;
      this.ExpirationUtc = expiration.HasValue ? new DateTime?(DateTime.UtcNow.Add(expiration.Value)) : new DateTime?();
    }

    public static void ProcessPendingTasks(bool isFg)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        WaScheduledTask[] waScheduledTasks1 = db.GetWaScheduledTasks(excludeTypes: new WaScheduledTask.Types[2]
        {
          WaScheduledTask.Types.RateCall,
          WaScheduledTask.Types.PendingRevoke
        }, limit: new int?(5), restriction: (WaScheduledTask.Restrictions) (isFg ? 1 : 2));
        if (((IEnumerable<WaScheduledTask>) waScheduledTasks1).Any<WaScheduledTask>())
        {
          int num = 0;
          foreach (WaScheduledTask task in waScheduledTasks1)
            db.AttemptScheduledTaskOnThreadPool(task, num++ * 500);
          Log.l("watask", "attempted {0} pending MessagesContext tasks", (object) waScheduledTasks1.Length);
        }
        if (isFg)
        {
          WaScheduledTask[] waScheduledTasks2 = db.GetWaScheduledTasks(new WaScheduledTask.Types[1]
          {
            WaScheduledTask.Types.RateCall
          });
          if (!((IEnumerable<WaScheduledTask>) waScheduledTasks2).Any<WaScheduledTask>())
            return;
          Log.l("watask", "found {0} pending call rating tasks", (object) waScheduledTasks2.Length);
          int length = waScheduledTasks2.Length;
          for (int index = 0; index < length; ++index)
          {
            WaScheduledTask task = waScheduledTasks2[index];
            bool tag;
            int delayInMS;
            if (index == length - 1)
            {
              tag = false;
              delayInMS = 300;
            }
            else
            {
              tag = true;
              delayInMS = 1000;
            }
            db.AttemptScheduledTaskOnThreadPool(task, delayInMS, tag: (object) tag);
          }
        }
        else
          db.CleanupWaScheduledTasks();
      }));
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        WaScheduledTask[] scheduledTasks = db.GetScheduledTasks(limit: new int?(5), restriction: isFg ? WaScheduledTask.Restrictions.FgOnly : WaScheduledTask.Restrictions.BgOnly);
        if (((IEnumerable<WaScheduledTask>) scheduledTasks).Any<WaScheduledTask>())
        {
          int num = 0;
          foreach (WaScheduledTask task in scheduledTasks)
            db.AttemptScheduledTaskOnThreadPool(task, num++ * 500);
          Log.l("watask", "attempted {0} pending ContactsContext tasks", (object) scheduledTasks.Length);
        }
        if (isFg)
          return;
        db.CleanupScheduledTasks();
      }));
    }

    private string GetPendingKey()
    {
      return string.Format("{0}-{1}", (object) this.TaskType, (object) this.TaskID);
    }

    public static void AttemptOnThreadPool(
      SqliteDataContext db,
      WaScheduledTask task,
      int delayInMS,
      Action<WaScheduledTask> deleteTaskOnDbSubmit,
      Action<WaScheduledTask> attemptTask,
      bool ignorePendingCheck = false)
    {
      if (task == null)
        return;
      object[] objArray1 = new object[5]
      {
        (object) (WaScheduledTask.Types) task.TaskType,
        (object) task.TaskID,
        (object) task.LookupKey,
        (object) task.Attempts,
        null
      };
      DateTime? expirationUtc = task.ExpirationUtc;
      string str1;
      if (!expirationUtc.HasValue)
      {
        str1 = "n/a";
      }
      else
      {
        expirationUtc = task.ExpirationUtc;
        str1 = expirationUtc.ToString();
      }
      objArray1[4] = (object) str1;
      Log.l("watask", "attempt | type:{0},id:{1},key:{2},attempts:{3},expiration:{4}", objArray1);
      bool flag = true;
      if (task.AttemptsLimit.HasValue && task.Attempts > task.AttemptsLimit.Value)
        Log.l("watask", "skip attempt | reached limit | id:{0}", (object) task.TaskID);
      else if (task.IsExpired)
        Log.l("watask", "skip attempt | expired | id:{0}", (object) task.TaskID);
      else if (task.IsDeleted)
        Log.l("watask", "skip attempt | deleted | id:{0}", (object) task.TaskID);
      else
        flag = false;
      if (flag)
      {
        deleteTaskOnDbSubmit(task);
        db.SubmitChanges();
      }
      else
      {
        if (AppState.IsBackgroundAgent)
        {
          if (task.Restriction == 1)
          {
            Log.l("watask", "skipped fg only task from bg | id:{0}", (object) task.TaskID);
            return;
          }
        }
        else if (task.Restriction == 2)
        {
          Log.l("watask", "skipped bg only task from fg | id:{0}", (object) task.TaskID);
          return;
        }
        if (!ignorePendingCheck && WaScheduledTask.pendingAttempts.Contains(task.GetPendingKey()))
        {
          Log.l("watask", "skip attempt | pending | id:{0}", (object) task.TaskID);
        }
        else
        {
          WaScheduledTask.pendingAttempts.Add(task.GetPendingKey());
          ++task.Attempts;
          db.SubmitChanges();
          WAThreadPool.RunAfterDelay(TimeSpan.FromMilliseconds((double) Math.Max(100, delayInMS)), (Action) (() => attemptTask(task)));
          object[] objArray2 = new object[5]
          {
            (object) (WaScheduledTask.Types) task.TaskType,
            (object) task.TaskID,
            (object) task.LookupKey,
            (object) task.Attempts,
            null
          };
          expirationUtc = task.ExpirationUtc;
          string str2;
          if (!expirationUtc.HasValue)
          {
            str2 = "n/a";
          }
          else
          {
            expirationUtc = task.ExpirationUtc;
            str2 = expirationUtc.ToString();
          }
          objArray2[4] = (object) str2;
          Log.l("watask", "attempt scheduled on thread pool | type:{0},id:{1},key:{2},attempts:{3},expiration:{4}", objArray2);
        }
      }
    }

    public static void OnAttemptError(
      SqliteDataContext db,
      WaScheduledTask task,
      Exception ex,
      Action<WaScheduledTask> deleteTaskOnDbSubmit)
    {
      WaScheduledTask.pendingAttempts.Remove(task.GetPendingKey());
      try
      {
        object[] objArray = new object[6]
        {
          (object) task.TaskID,
          (object) (WaScheduledTask.Types) task.TaskType,
          (object) task.LookupKey,
          (object) task.Attempts,
          (object) task.AttemptsLimit,
          null
        };
        DateTime? expirationUtc = task.ExpirationUtc;
        string str;
        if (!expirationUtc.HasValue)
        {
          str = "n/a";
        }
        else
        {
          expirationUtc = task.ExpirationUtc;
          str = expirationUtc.ToString();
        }
        objArray[5] = (object) str;
        Log.l("watask", "failed | id:{0},type:{1},key:{2},attempts:{3},limit:{4},expiration:{5}", objArray);
        if (!task.IsAttemptsLimitReached)
          return;
        Log.l("watask", "deleted after too many attempts | id:{0}", (object) task.TaskID);
        deleteTaskOnDbSubmit(task);
        db.SubmitChanges();
      }
      catch (DatabaseInvalidatedException ex1)
      {
      }
    }

    public static void OnAttemptDone(WaScheduledTask task)
    {
      WaScheduledTask.pendingAttempts.Remove(task.GetPendingKey());
      Log.l("watask", "attempted | id:{0},type:{1},key:{2}", (object) task.TaskID, (object) (WaScheduledTask.Types) task.TaskType, (object) task.LookupKey);
    }

    public static void OnTaskDone(
      SqliteDataContext db,
      WaScheduledTask task,
      Action<WaScheduledTask> deleteTaskOnDbSubmit)
    {
      try
      {
        Log.l("watask", "finished | id:{0},type:{1},attempts:{2}", (object) task.TaskID, (object) (WaScheduledTask.Types) task.TaskType, (object) task.Attempts);
        deleteTaskOnDbSubmit(task);
        db.SubmitChanges();
      }
      catch (DatabaseInvalidatedException ex)
      {
      }
    }

    public static IObservable<Unit> GetAttemptObservable(WaScheduledTask task, object tag = null)
    {
      IObservable<Unit> observable = (IObservable<Unit>) null;
      switch ((WaScheduledTask.Types) task.TaskType)
      {
        case WaScheduledTask.Types.ClearMessages:
          observable = ClearChat.PerformClearChatHistory(task.LookupKey);
          break;
        case WaScheduledTask.Types.DeleteChatPic:
          observable = ChatPictureStore.PerformDeleteSavedChatPictureTask(task);
          break;
        case WaScheduledTask.Types.SaveChatPic:
          observable = ChatPictureStore.PerformSaveChatPictureTask(task);
          break;
        case WaScheduledTask.Types.RateCall:
          bool skipRatingPrompt = (bool) tag;
          observable = VoipHandler.PerformScheduledCallRating(task, skipRatingPrompt);
          break;
        case WaScheduledTask.Types.IndexMessages:
          observable = IndexMessages.BatchMessageIndex();
          break;
        case WaScheduledTask.Types.PurgeStatuses:
          observable = WaStatusHelper.PerformPurgeStatuses();
          break;
        case WaScheduledTask.Types.PendingRevoke:
          observable = AsyncRevoke.PerformPendingRevoke(task);
          break;
        case WaScheduledTask.Types.ClearAllMessages:
          observable = ClearChat.PerformClearAllChatHistory();
          break;
        case WaScheduledTask.Types.GenerateTransitBizSystemMessage:
          observable = SystemMessageUtils.PerformGenerateTransitBizSystemMessage(task);
          break;
        case WaScheduledTask.Types.PostContactRemoved:
          observable = ContactStore.PerformPostContactRemoved(task);
          break;
      }
      return observable ?? Observable.Return<Unit>(new Unit());
    }

    public static Pair<string, List<object>> GetWaScheduledTasksWhereClauses(
      WaScheduledTask.Types[] includeTypes = null,
      WaScheduledTask.Types[] excludeTypes = null,
      bool excludeExpired = true,
      string lookupKey = null,
      WaScheduledTask.Restrictions restriction = WaScheduledTask.Restrictions.None)
    {
      List<string> stringList = new List<string>();
      List<object> objectList = new List<object>();
      if (includeTypes != null)
      {
        if (includeTypes.Length == 1)
        {
          stringList.Add("TaskType = ?");
          objectList.Add((object) (int) includeTypes[0]);
        }
        else if (includeTypes.Length > 1)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append("TaskType IN (");
          bool flag = true;
          foreach (WaScheduledTask.Types includeType in includeTypes)
          {
            if (flag)
              flag = false;
            else
              stringBuilder.Append(", ");
            stringBuilder.Append("?");
            objectList.Add((object) (int) includeType);
          }
          stringBuilder.Append(")");
          stringList.Add(stringBuilder.ToString());
        }
      }
      if (excludeTypes != null)
      {
        if (excludeTypes.Length == 1)
        {
          stringList.Add("TaskType <> ?");
          objectList.Add((object) (int) excludeTypes[0]);
        }
        else if (excludeTypes.Length > 1)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append("TaskType NOT IN (");
          bool flag = true;
          foreach (WaScheduledTask.Types excludeType in excludeTypes)
          {
            if (flag)
              flag = false;
            else
              stringBuilder.Append(", ");
            stringBuilder.Append("?");
            objectList.Add((object) (int) excludeType);
          }
          stringBuilder.Append(")");
          stringList.Add(stringBuilder.ToString());
        }
      }
      if (excludeExpired)
      {
        stringList.Add("(ExpirationUtc IS NULL OR ExpirationUtc > ?)");
        objectList.Add((object) FunRunner.CurrentServerTimeUtc.ToFileTimeUtc());
      }
      if (lookupKey != null)
      {
        stringList.Add("LookupKey = ?");
        objectList.Add((object) lookupKey);
      }
      if (restriction != WaScheduledTask.Restrictions.None)
      {
        stringList.Add("(Restriction = 0 OR Restriction IS NULL OR Restriction = ?)");
        objectList.Add((object) (long) restriction);
      }
      Pair<string, List<object>> tasksWhereClauses = new Pair<string, List<object>>();
      if (stringList.Any<string>())
      {
        tasksWhereClauses.First = string.Format("WHERE {0}", (object) string.Join(" AND ", (IEnumerable<string>) stringList));
        tasksWhereClauses.Second = objectList;
      }
      else
      {
        tasksWhereClauses.First = "";
        tasksWhereClauses.Second = new List<object>();
      }
      return tasksWhereClauses;
    }

    public enum Types
    {
      Undefined = 0,
      ClearMessages = 1,
      DeleteChatPic = 2,
      SaveChatPic = 3,
      RateCall = 4,
      IndexMessages = 5,
      PurgeStatuses = 6,
      PendingRevoke = 7,
      ClearAllMessages = 8,
      GenerateTransitBizSystemMessage = 1001, // 0x000003E9
      PostContactRemoved = 1002, // 0x000003EA
    }

    public enum Restrictions
    {
      None,
      FgOnly,
      BgOnly,
    }
  }
}
