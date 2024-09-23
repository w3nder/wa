// Decompiled with JetBrains decompiler
// Type: WhatsApp.PersistentAction
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WhatsApp.CommonOps;
using WhatsApp.ProtoBuf;
using WhatsApp.WaCollections;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "ActionType,ActionDataString", Name = "TypeAndData")]
  public class PersistentAction : PropChangingBase
  {
    public const int DefaultMaxAttempts = 100;
    private static TimeSpan DefaultExpiration = TimeSpan.FromDays(14.0);
    private int actionType;
    private string actionDataString;
    private int attempts;
    private int? attemptsLimit;
    private DateTime? expirationTime;
    private string jid;
    private bool fromMe;
    private string id;
    private int failureCount;
    private int? maxFailureCount;
    public bool PhotoAttempted;
    private bool hashUpdateInProgress;
    private const int PaHsmVersion = 3;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int ActionID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int ActionType
    {
      get => this.actionType;
      set
      {
        if (this.actionType == value)
          return;
        this.NotifyPropertyChanging(nameof (ActionType));
        this.actionType = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string ActionDataString
    {
      get => this.actionDataString;
      set
      {
        if (!(this.actionDataString != value))
          return;
        this.NotifyPropertyChanging(nameof (ActionDataString));
        this.actionDataString = value;
      }
    }

    public byte[] ActionData
    {
      get
      {
        string actionDataString = this.actionDataString;
        if (actionDataString != null)
        {
          try
          {
            return Convert.FromBase64String(actionDataString);
          }
          catch (Exception ex)
          {
          }
        }
        return (byte[]) null;
      }
      set
      {
        string str = (string) null;
        if (value != null)
          str = Convert.ToBase64String(value);
        this.ActionDataString = str;
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
    public DateTime? ExpirationTime
    {
      get => this.expirationTime;
      set
      {
        DateTime? expirationTime = this.expirationTime;
        DateTime? nullable = value;
        if ((expirationTime.HasValue == nullable.HasValue ? (expirationTime.HasValue ? (expirationTime.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (ExpirationTime));
        this.expirationTime = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Jid
    {
      get => this.jid;
      set
      {
        if (!(this.jid != value))
          return;
        this.NotifyPropertyChanging(nameof (Jid));
        this.jid = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool FromMe
    {
      get => this.fromMe;
      set
      {
        if (this.fromMe == value)
          return;
        this.NotifyPropertyChanging(nameof (FromMe));
        this.fromMe = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Id
    {
      get => this.id;
      set
      {
        if (!(this.id != value))
          return;
        this.NotifyPropertyChanging(nameof (Id));
        this.id = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [Deprecated]
    public int FailureCount
    {
      get => this.failureCount;
      set
      {
        if (this.failureCount == value)
          return;
        this.NotifyPropertyChanging(nameof (FailureCount));
        this.failureCount = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [Deprecated]
    public int? MaxFailureCount
    {
      get => this.maxFailureCount;
      set
      {
        int? maxFailureCount = this.maxFailureCount;
        int? nullable = value;
        if ((maxFailureCount.GetValueOrDefault() == nullable.GetValueOrDefault() ? (maxFailureCount.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (MaxFailureCount));
        this.maxFailureCount = value;
      }
    }

    public PersistentAction()
    {
      this.ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc + PersistentAction.DefaultExpiration);
    }

    public string ActionDataStringForLog
    {
      get
      {
        switch ((PersistentAction.Types) this.ActionType)
        {
          case PersistentAction.Types.AckMedia:
            return MediaDownload.RedactUrl(this.ActionDataString);
          case PersistentAction.Types.SetRecoveryToken:
          case PersistentAction.Types.SetPhoto:
          case PersistentAction.Types.SetStatus:
          case PersistentAction.Types.SendReceipts:
          case PersistentAction.Types.Qr:
          case PersistentAction.Types.OptimisticResume:
          case PersistentAction.Types.RehydrateHighlyStructuredMessage:
          case PersistentAction.Types.SendPaymentsRequest:
          case PersistentAction.Types.Mms4RouteSelection:
            return "[data]";
          default:
            return this.ActionDataString;
        }
      }
    }

    public bool Removed { get; set; }

    public bool IsAttemptsLimitReached => this.Attempts >= (this.AttemptsLimit ?? 100);

    public bool IsExpired
    {
      get
      {
        return this.ExpirationTime.HasValue && FunRunner.CurrentServerTimeUtc >= this.ExpirationTime.Value;
      }
    }

    public IObservable<Unit> Perform(
      FunXMPP.Connection connection,
      PersistentActionRetryState state = null)
    {
      switch (this.actionType)
      {
        case 1:
          return connection.SendAckMedia(this.ActionDataString, this.FromMe);
        case 2:
          return connection.SendLeaveGroup(this.ActionDataString);
        case 3:
          return connection.SendSetRecoveryToken(this.ActionData);
        case 4:
          return PersistentAction.PerformAutoDownload(this.Jid, this.FromMe, this.Id, state, this.Attempts);
        case 6:
          return PersistentAction.PerformSendGetImage(this.ActionDataString, connection);
        case 7:
        case 13:
        case 16:
        case 18:
        case 20:
        case 21:
          return Observable.Return<Unit>(new Unit());
        case 8:
          return PersistentAction.PerformSendDeleteBroadcastList(this.ActionDataString, connection);
        case 9:
          return this.PerformSetPhoto(this.DeserializePhotoArgs(), connection);
        case 10:
          return PersistentAction.PerformSetStatus(this.ActionDataString, this.Jid, connection);
        case 11:
          return PersistentAction.PerformSendReadReceipt(this.ActionData, connection);
        case 12:
          return PersistentAction.PerformReuploadMedia(this.Jid, this.FromMe, this.Id, this.ActionDataString, connection, this.Attempts);
        case 14:
          return PersistentAction.PerformQr(this.ActionDataString);
        case 15:
          return PersistentAction.PerformSendPostponedReceipts(this.ActionData);
        case 17:
          return PersistentAction.PerformSendVerifyAxolotlDigest(connection);
        case 19:
          return PersistentAction.PerformSendIndividualRetryForGroup(connection, this.Jid, this.Id, this.ActionData);
        case 22:
          return PersistentAction.PerformIdentityChangedForUser(this.jid);
        case 23:
          return PersistentAction.PerformCapabilitiesRefresh(connection);
        case 24:
          return PersistentAction.PerformProtocolBufferMessageUpgrade();
        case 25:
          return PersistentAction.PerformContactVCardIndex();
        case 26:
          return PersistentAction.PerformDisplayFullEncryptionToAllChats();
        case 27:
          return this.PerformRehashLocalFiles();
        case 28:
          return this.PerformOptimisticUploadResume(this.ActionData);
        case 29:
          return this.PerformRehydrateHighlyStructuredMessage(this.Jid, this.Id, this.ActionData);
        case 30:
          return VerifiedNamesCertifier.PerformCertifyVerifiedUser(connection, this.Jid, this.ActionData);
        case 32:
          return WaStatusHelper.PerformSendStatusV3PrivacyList(connection, this);
        case 33:
          return this.PerformNotifyChangedNumber(connection, this.ActionData);
        case 34:
          return PaymentsPersistentAction.SendPaymentsRequest(connection, this.ActionData);
        case 35:
          return Mms4Helper.PerformMms4RouteSelection(connection);
        case 36:
          return PersistentAction.PerformSendEnableLocationSharing(connection, this.ActionData);
        case 37:
          return PersistentAction.PerformSendDisableLocationSharing(connection, this.Jid, this.Id, this.ActionDataString);
        case 38:
          return PersistentAction.PerformSendSubscribeToLocationUpdates(this.Jid, this.ActionDataString != null);
        case 39:
          return PersistentAction.PerformSendUnsubscribeToLocationUpdates(this.Jid, this.Id);
        case 40:
          return Observable.Return<Unit>(new Unit());
        case 41:
          return connection.SendLeaveGroup(this.ActionDataString, delete: true);
        case 42:
          return PersistentAction.PerformReuploadMediaNotification(this.Jid, this.FromMe, this.Id, this.ActionDataString, connection, this.Attempts);
        case 43:
          return GdprTos.PerformSendGdprStage(this.ActionDataString);
        case 44:
          return GdprTos.PerformSendGdprAccept();
        case 45:
          return GdprTos.PerformAckGdprTosReset();
        case 46:
          return GdprTos.PerformSendGdprTosPage(this.ActionDataString);
        case 47:
          return GdprReport.PerformDeleteGdprReport();
        case 48:
          return PersistentAction.PerformAddOneTime2TierSysMsgToBizChats();
        case 49:
          return Mms4Helper.PerformMms4HostSelection(connection);
        default:
          return Observable.Return<Unit>(new Unit());
      }
    }

    public static PersistentAction AutoDownload(Message m)
    {
      return new PersistentAction()
      {
        ActionType = 4,
        ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(1.0)),
        Jid = m.KeyRemoteJid,
        FromMe = m.KeyFromMe,
        Id = m.KeyId
      };
    }

    private static IObservable<Unit> PerformAutoDownload(
      string jid,
      bool fromMe,
      string id,
      PersistentActionRetryState state,
      int attempts)
    {
      Message msg = (Message) null;
      bool shouldAutoDownload = false;
      if (jid != null && id != null)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msg = db.GetMessage(jid, id, fromMe);
          if (msg == null)
            return;
          shouldAutoDownload = msg.ShouldAutoDownload(db);
        }));
      if (!shouldAutoDownload || msg.LocalFileExists() || msg.Status == FunXMPP.FMessage.Status.Error || state != null && state.MaxAutoDownload.HasValue && state.MaxAutoDownload.Value <= 0)
        return Observable.Return<Unit>(new Unit());
      IObservable<bool> source1 = msg.IsPtt() ? Observable.Return<bool>(true) : PersistentAction.EnforceAutodownloadNetworkRules(msg.GetAutoDownloadSetting()).Select<Pair<PersistentAction.DownloadConnectionTypes, bool>, bool>((Func<Pair<PersistentAction.DownloadConnectionTypes, bool>, bool>) (p =>
      {
        if (!p.Second)
          return p.Second;
        long num1 = msg.ShouldStartPrefetchingVideo() ? (long) Settings.VideoPrefetchBytes : msg.MediaSize;
        if (p.First == PersistentAction.DownloadConnectionTypes.CellData && num1 > (long) Settings.MaxAutodownloadSize)
          return false;
        double num2;
        ulong val2;
        switch (msg.MediaWaType)
        {
          case FunXMPP.FMessage.Type.Image:
          case FunXMPP.FMessage.Type.Document:
          case FunXMPP.FMessage.Type.Gif:
          case FunXMPP.FMessage.Type.Sticker:
            num2 = 0.05;
            val2 = 33554432UL;
            break;
          case FunXMPP.FMessage.Type.Audio:
          case FunXMPP.FMessage.Type.Video:
            num2 = 0.1;
            val2 = 134217728UL;
            break;
          default:
            return true;
        }
        DiskSpace diskSpace = NativeInterfaces.Misc.GetDiskSpace(Constants.IsoStorePath);
        Log.l("auto download", "disk space: {0} mb free, {1} mb capacity ({2}% free)", (object) (diskSpace.FreeBytes / 1024UL / 1024UL), (object) (diskSpace.TotalBytes / 1024UL / 1024UL), (object) (ulong) (diskSpace.TotalBytes == 0UL ? 0L : (long) (diskSpace.FreeBytes * 100UL / diskSpace.TotalBytes)));
        ulong val1 = (ulong) (num2 * (double) diskSpace.TotalBytes);
        if (diskSpace.FreeBytes < (ulong) Settings.MaxMediaSize)
        {
          Log.WriteLineDebug("Available space is less than max media size");
          return false;
        }
        if (diskSpace.FreeBytes >= Math.Min(val1, val2))
          return true;
        Log.l("auto download", "Free space is less than {0}", val1 < val2 ? (object) string.Format("{0}%", (object) (int) (num2 * 100.0)) : (object) string.Format("{0}mb", (object) (val2 / 1024UL / 1024UL)));
        return false;
      }));
      IObservable<Unit> downloader = Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        bool cancel = false;
        if (state != null && state.MaxAutoDownload.HasValue)
        {
          PersistentActionRetryState actionRetryState = state;
          int? maxAutoDownload = actionRetryState.MaxAutoDownload;
          int num = 1;
          actionRetryState.MaxAutoDownload = maxAutoDownload.HasValue ? new int?(maxAutoDownload.GetValueOrDefault() - num) : new int?();
        }
        AppState.Worker.Enqueue((Action) (() =>
        {
          if (cancel)
            return;
          WhatsApp.Events.MediaDownload mediaDownloadEvent = FieldStats.GetFsMediaDownloadEvent(msg);
          mediaDownloadEvent.retryCount = new long?((long) attempts);
          msg.IsAutoDownloading = true;
          IObservable<MediaDownload.MediaProgress> source3 = AppState.IsBackgroundAgent ? MediaDownload.TransferFromBackground(msg, mediaDownloadEvent) : MediaDownload.TransferFromForeground(msg, mediaDownloadEvent, false);
          msg.SetPendingMediaSubscription("Persistent Action", AppState.IsBackgroundAgent ? PendingMediaTransfer.TransferTypes.Download_Background : PendingMediaTransfer.TransferTypes.Download_Foreground_NotInteractive, MediaDownload.TransferForMessageObservable(msg, source3, mediaDownloadEvent), (Action<Unit>) (u => observer.OnNext(u)), (Action<Exception>) (ex =>
          {
            if (ex is CryptographicException)
              observer.OnNext(new Unit());
            else
              observer.OnError(ex);
          }), (Action) (() =>
          {
            if (attempts == 1 && msg.IsPtt())
            {
              Log.p("persist action", "notify ptt download end | msg_id={0}", (object) msg.MessageID);
              MediaDownload.VoiceMessageDownloadEndedSubj.OnNext(msg);
            }
            observer.OnCompleted();
          }));
        }));
        return (Action) (() => cancel = true);
      }));
      return source1.Where<bool>((Func<bool, bool>) (x => x)).SelectMany<bool, Unit, Unit>((Func<bool, IObservable<Unit>>) (x => downloader), (Func<bool, Unit, Unit>) ((x, y) => y));
    }

    private static IObservable<Pair<PersistentAction.DownloadConnectionTypes, bool>> EnforceAutodownloadNetworkRules(
      AutoDownloadSetting downloadType)
    {
      Pair<PersistentAction.DownloadConnectionTypes, bool> pair = new Pair<PersistentAction.DownloadConnectionTypes, bool>(PersistentAction.DownloadConnectionTypes.Unknown, false);
      if ((downloadType & AutoDownloadSetting.EnabledOnWifi) != AutoDownloadSetting.Disabled && NetworkStateMonitor.IsWifiDataConnected())
      {
        pair.First = PersistentAction.DownloadConnectionTypes.Wifi;
        pair.Second = true;
      }
      else if ((downloadType & AutoDownloadSetting.EnabledOnData) != AutoDownloadSetting.Disabled)
      {
        pair.First = PersistentAction.DownloadConnectionTypes.CellData;
        pair.Second = true;
        if ((downloadType & AutoDownloadSetting.EnabledWhileRoaming) == AutoDownloadSetting.Disabled)
        {
          try
          {
            if (NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.NetworkInfo).Roaming)
              pair.Second = false;
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "roaming check");
            pair.Second = false;
          }
        }
      }
      return Observable.Return<Pair<PersistentAction.DownloadConnectionTypes, bool>>(pair);
    }

    public static PersistentAction SendReceipt(Message m, string receiptType)
    {
      ReceiptSpec receiptSpec = new ReceiptSpec()
      {
        Jid = m.KeyRemoteJid,
        Id = m.KeyId,
        Participant = m.RemoteResource
      };
      if (string.IsNullOrEmpty(receiptSpec.Participant))
        receiptSpec.Participant = (string) null;
      else if (m.IsBroadcasted())
      {
        string participant = receiptSpec.Participant;
        receiptSpec.Participant = receiptSpec.Jid;
        receiptSpec.Jid = participant;
      }
      return PersistentAction.SendReceipts((IEnumerable<ReceiptSpec>) new ReceiptSpec[1]
      {
        receiptSpec
      }, receiptType);
    }

    public static PersistentAction SendReceipts(
      IEnumerable<ReceiptSpec> receiptsEnum,
      string receiptType)
    {
      ReceiptSpec[] array = receiptsEnum.ToArray<ReceiptSpec>();
      if (!((IEnumerable<ReceiptSpec>) array).Any<ReceiptSpec>())
        return (PersistentAction) null;
      List<string> source1 = new List<string>();
      source1.Add(receiptType);
      source1.Add(array[0].Jid);
      source1.Add(array[0].Participant ?? "");
      source1.Add(array[0].Id);
      foreach (ReceiptSpec receiptSpec in ((IEnumerable<ReceiptSpec>) array).Skip<ReceiptSpec>(1))
        source1.Add(receiptSpec.Id);
      List<byte> byteList = new List<byte>();
      foreach (byte[] source2 in source1.Select<string, byte[]>((Func<string, byte[]>) (str => Encoding.UTF8.GetBytes(str))))
      {
        uint count = (uint) (source2.Length & (int) ushort.MaxValue);
        byteList.Add((byte) (count & (uint) byte.MaxValue));
        byteList.Add((byte) (count >> 8));
        byteList.AddRange(((IEnumerable<byte>) source2).Take<byte>((int) count));
      }
      return new PersistentAction()
      {
        ActionType = 11,
        ActionData = byteList.ToArray()
      };
    }

    public static IObservable<Unit> PerformSendReadReceipt(byte[] data, FunXMPP.Connection conn)
    {
      if (data == null)
        return Observable.Return<Unit>(new Unit());
      int index = 0;
      List<string> strings = new List<string>();
      int count;
      for (; data.Length - index >= 2; index += count + 2)
      {
        count = (int) data[index] | (int) data[index + 1] << 8;
        if (count == 0)
          strings.Add("");
        else if (count <= data.Length - 2 - index)
        {
          try
          {
            strings.Add(Encoding.UTF8.GetString(data, index + 2, count));
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "decode read receipt");
            return Observable.Return<Unit>(new Unit());
          }
        }
        else
        {
          Log.WriteLineDebug("length went beyond array bounds");
          return Observable.Return<Unit>(new Unit());
        }
      }
      return strings.Count < 4 ? Observable.Return<Unit>(new Unit()) : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        string type = strings[0];
        string to = strings[1];
        string participant = strings[2];
        if (participant.Length == 0)
          participant = (string) null;
        conn.SendReceipt(to, participant, strings[3], type, strings.Count > 4 ? strings.Skip<string>(4).Select<string, Pair<string, string>>((Func<string, Pair<string, string>>) (s => new Pair<string, string>(s, (string) null))).ToArray<Pair<string, string>>() : (Pair<string, string>[]) null, (Action) (() =>
        {
          observer.OnNext(new Unit());
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    public static PersistentAction SendPostponedReceipts(DateTime end, DateTime? begin)
    {
      BinaryData binaryData = new BinaryData();
      long val1 = DateTimeUtils.SanitizeTimestamp(end.ToUnixTime());
      long val2 = DateTimeUtils.SanitizeTimestamp(begin.HasValue ? begin.Value.ToUnixTime() : 0L);
      binaryData.AppendLong64(val2);
      binaryData.AppendLong64(val1);
      return new PersistentAction()
      {
        ActionType = 15,
        ActionData = binaryData.Get(),
        ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(3.0))
      };
    }

    public static IObservable<Unit> PerformSendPostponedReceipts(byte[] data)
    {
      BinaryData bd = new BinaryData(data);
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        long tTargetBegin = bd.ReadLong64(0);
        long tEnd = bd.ReadLong64(8);
        long tBegin = Math.Max(tTargetBegin, tEnd - (long) TimeSpan.FromHours(24.0).TotalSeconds);
        DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          ReceiptSpec[] array = ((IEnumerable<PostponedReceipt>) db.GetPostponedReceipts(tBegin, tEnd)).Select<PostponedReceipt, ReceiptSpec>((Func<PostponedReceipt, ReceiptSpec>) (r => new ReceiptSpec()
          {
            Jid = r.TargetJid,
            Participant = r.ParticipantJid,
            Id = r.KeyId,
            MessageTimestamp = new DateTime?()
          })).ToArray<ReceiptSpec>();
          Log.l("read receipts", "send postponed receipts | between {0}/{1} and {2}, found: {3}", (object) tTargetBegin, (object) tBegin, (object) tEnd, (object) array.Length);
          if (((IEnumerable<ReceiptSpec>) array).Any<ReceiptSpec>())
          {
            LinkedList<PersistentAction> outgoingTasks = (LinkedList<PersistentAction>) null;
            ReadReceipts.ScheduleSend(db, array, out outgoingTasks);
            AppState.Worker.Enqueue((Action) (() =>
            {
              foreach (PersistentAction a in outgoingTasks)
                AppState.AttemptPersistentAction(a);
            }));
          }
          db.DeletePostponedReceipts(tTargetBegin, tEnd);
        }));
        PerformanceTimer.End("fetch and schedule send postponed receipts", start);
        observer.OnNext(new Unit());
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public static PersistentAction SendVerifyAxolotlDigest()
    {
      return new PersistentAction()
      {
        ActionType = 17,
        ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(3.0))
      };
    }

    public static IObservable<Unit> PerformSendVerifyAxolotlDigest(FunXMPP.Connection conn)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        conn.Encryption.VerifyDigest((Action) (() => observer.OnNext(new Unit())));
        return (Action) (() => { });
      }));
    }

    public static PersistentAction SendIndividualRetryForGroup(
      string keyRemoteJid,
      string keyId,
      string participantJid,
      int count)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendInt32(count);
      binaryData.AppendStrWithLengthPrefix(participantJid);
      return new PersistentAction()
      {
        ActionType = 19,
        ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(3.0)),
        Jid = keyRemoteJid,
        Id = keyId,
        ActionData = binaryData.Get()
      };
    }

    public static IObservable<Unit> PerformSendIndividualRetryForGroup(
      FunXMPP.Connection conn,
      string keyRemoteJid,
      string keyId,
      byte[] bytes)
    {
      int newOffset = 0;
      BinaryData binaryData = new BinaryData(bytes);
      int count = binaryData.ReadInt32(0);
      string participant = binaryData.ReadStrWithLengthPrefix(4, out newOffset);
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Action onCompleteSend = (Action) (() => observer.OnNext(new Unit()));
        FunXMPP.Connection.AsyncReceiptThread.Enqueue((Action) (() => conn.Encryption.SendIndividualRetryForGroup(keyRemoteJid, keyId, participant, count, onCompleteSend)));
        return (Action) (() => { });
      }));
    }

    public static PersistentAction IdentityChangedForUser(string jid)
    {
      return new PersistentAction()
      {
        ActionType = 22,
        Jid = jid
      };
    }

    public static IObservable<Unit> PerformIdentityChangedForUser(string keyRemoteJid)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Axolotl.IdentityChangedForUser(keyRemoteJid);
        VerifiedNamesCertifier.IdentityChangedForUser(keyRemoteJid, false);
        observer.OnNext(new Unit());
        return (Action) (() => { });
      }));
    }

    private static IObservable<Unit> PerformSendGetImage(string jid, FunXMPP.Connection conn)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        try
        {
          conn.SendGetPhoto(jid, (string) null, true, (Action) (() =>
          {
            Log.l("PersistAct", "get image success | jid: {0}", (object) jid);
            observer.OnNext(new Unit());
          }), (Action<int>) (errCode =>
          {
            Log.l("PersistAct", "get image failed | jid: {0} err={1}", (object) jid, (object) errCode);
            if (errCode == 404 || errCode == 401)
            {
              observer.OnNext(new Unit());
              Log.l("PersistAct", "done | jid: {0} err={1}", (object) jid, (object) errCode);
            }
            else
              observer.OnError(new Exception("get image"));
          }));
        }
        catch (Exception ex)
        {
          observer.OnError(ex);
        }
        return (Action) (() => { });
      }));
    }

    public static IObservable<Unit> PerformSendDeleteBroadcastList(
      string broadcastListJid,
      FunXMPP.Connection conn)
    {
      return !JidHelper.IsBroadcastJid(broadcastListJid) ? Observable.Return<Unit>(new Unit()) : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        try
        {
          conn.SendDeleteBroadcastList(broadcastListJid, (Action) (() =>
          {
            Log.WriteLineDebug("persist action: deleted bcast list | jid={0}", (object) broadcastListJid);
            observer.OnNext(new Unit());
          }), (Action<int>) (errCode => Log.WriteLineDebug("persist action: delete bcast list failed | jid={0} err={1}", (object) broadcastListJid, (object) errCode)));
        }
        catch (Exception ex)
        {
          observer.OnError(ex);
        }
        return (Action) (() => { });
      }));
    }

    public static PersistentAction SendEnableLocationSharing(
      IEnumerable<FunXMPP.FMessage.Participant> participants)
    {
      return new PersistentAction()
      {
        ActionType = 36,
        ActionData = PersistentAction.SerializeLocationSharingArgs(participants)
      };
    }

    private static byte[] SerializeLocationSharingArgs(
      IEnumerable<FunXMPP.FMessage.Participant> participants)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendInt32(participants.Count<FunXMPP.FMessage.Participant>());
      foreach (FunXMPP.FMessage.Participant participant in participants)
      {
        binaryData.AppendStrWithLengthPrefix(participant.Jid);
        binaryData.AppendStrWithLengthPrefix(participant.Encrypted.cipher_text_type);
        binaryData.AppendBytesWithLengthPrefix(participant.Encrypted.cipher_text_bytes);
        binaryData.AppendInt32(participant.Encrypted.cipher_version);
      }
      return binaryData.Get();
    }

    public static IObservable<Unit> PerformSendEnableLocationSharing(
      FunXMPP.Connection conn,
      byte[] actionData)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        if (conn == null)
        {
          observer.OnCompleted();
        }
        else
        {
          IEnumerable<FunXMPP.FMessage.Participant> participants = (IEnumerable<FunXMPP.FMessage.Participant>) PersistentAction.DeserializeLocationSharingArgs(actionData);
          if (participants.Count<FunXMPP.FMessage.Participant>() > 0)
            conn.SendEnableLocationSharing((Action) (() =>
            {
              observer.OnNext(new Unit());
              observer.OnCompleted();
            }), (Action) (() => observer.OnCompleted()), participants);
        }
        return (Action) (() => { });
      }));
    }

    private static List<FunXMPP.FMessage.Participant> DeserializeLocationSharingArgs(
      byte[] actionData)
    {
      List<FunXMPP.FMessage.Participant> participantList = new List<FunXMPP.FMessage.Participant>();
      if (actionData == null)
        return participantList;
      try
      {
        int offset = 0;
        BinaryData binaryData = new BinaryData(actionData);
        int num = binaryData.ReadInt32(offset);
        int newOffset = offset + 4;
        for (int index = 0; index < num; ++index)
        {
          string jid = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
          participantList.Add(new FunXMPP.FMessage.Participant(jid)
          {
            Encrypted = new FunXMPP.FMessage.Encrypted()
            {
              cipher_text_type = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset),
              cipher_text_bytes = binaryData.ReadBytesWithLengthPrefix(newOffset, out newOffset),
              cipher_version = binaryData.ReadInt32(newOffset)
            }
          });
          newOffset += 4;
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception deserializing location sharing");
      }
      return participantList;
    }

    public static PersistentAction SendDisableLocationSharing(
      string gjid,
      string id,
      string sequenceid)
    {
      return new PersistentAction()
      {
        ActionType = 37,
        Id = id,
        Jid = gjid,
        ActionDataString = sequenceid
      };
    }

    public static IObservable<Unit> PerformSendDisableLocationSharing(
      FunXMPP.Connection conn,
      string gjid,
      string id,
      string sequenceid)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        if (conn == null)
          observer.OnCompleted();
        else
          conn.SendDisableLocationSharing((Action) (() =>
          {
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action) (() => observer.OnCompleted()), gjid, id, sequenceid);
        return (Action) (() => { });
      }));
    }

    public static PersistentAction SendSubscribeToLocationUpdates(string gjid, bool hasParticipants)
    {
      return new PersistentAction()
      {
        ActionType = 38,
        Jid = gjid,
        ActionDataString = hasParticipants ? "true" : (string) null
      };
    }

    public static IObservable<Unit> PerformSendSubscribeToLocationUpdates(
      string jid,
      bool hasParticipants)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        FunXMPP.Connection connection = AppState.GetConnection();
        if (connection == null)
          observer.OnCompleted();
        else
          connection.SendSubscribeToLocationUpdates((Action) (() =>
          {
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action<int>) (error => observer.OnCompleted()), jid, hasParticipants);
        return (Action) (() => { });
      }));
    }

    public static PersistentAction SendUnsubscribeToLocationUpdates(string gjid, string id)
    {
      return new PersistentAction()
      {
        ActionType = 39,
        Jid = gjid,
        Id = id
      };
    }

    public static IObservable<Unit> PerformSendUnsubscribeToLocationUpdates(string jid, string id)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        FunXMPP.Connection connection = AppState.GetConnection();
        if (connection == null)
          observer.OnCompleted();
        else
          connection.SendUnsubscribeToLocationUpdates((Action) (() =>
          {
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action<int>) (error => observer.OnCompleted()), jid, id);
        return (Action) (() => { });
      }));
    }

    public PersistentAction.SetPhotoArgs DeserializePhotoArgs()
    {
      byte[] actionData = this.ActionData;
      if (actionData == null)
        return (PersistentAction.SetPhotoArgs) null;
      int count = BinaryData.ReadInt32(actionData, 0);
      int num1 = BinaryData.ReadInt32(actionData, 4);
      int num2 = BinaryData.ReadInt32(actionData, 8);
      if (count == 0)
        return (PersistentAction.SetPhotoArgs) null;
      PersistentAction.SetPhotoArgs setPhotoArgs = new PersistentAction.SetPhotoArgs();
      setPhotoArgs.Jid = Encoding.UTF8.GetString(actionData, 12, count);
      if (num1 != 0)
        setPhotoArgs.Thumbnail = new PersistentAction.SetPhotoArgs.Buffer()
        {
          Buf = actionData,
          Offset = 12 + count,
          Length = num1
        };
      if (num2 != 0)
        setPhotoArgs.FullSize = new PersistentAction.SetPhotoArgs.Buffer()
        {
          Buf = actionData,
          Offset = 12 + count + num1,
          Length = num2
        };
      int index = 12 + count + num1 + num2;
      setPhotoArgs.ShowSystemMessage = actionData.Length < index + 1 || actionData[index] > (byte) 0;
      return setPhotoArgs;
    }

    private static byte[] SerializePhotoArgs(
      string jid,
      byte[] thumbnail,
      byte[] fullSize,
      bool showSystemMessage)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendInt32(jid.Length);
      binaryData.AppendInt32((thumbnail ?? new byte[0]).Length);
      binaryData.AppendInt32((fullSize ?? new byte[0]).Length);
      binaryData.AppendBytes((IEnumerable<byte>) Encoding.UTF8.GetBytes(jid));
      binaryData.AppendBytes((IEnumerable<byte>) (thumbnail ?? new byte[0]));
      binaryData.AppendBytes((IEnumerable<byte>) (fullSize ?? new byte[0]));
      binaryData.AppendByte(showSystemMessage ? (byte) 1 : (byte) 0);
      return binaryData.Get();
    }

    private IObservable<Unit> PerformSetPhoto(
      PersistentAction.SetPhotoArgs args,
      FunXMPP.Connection conn)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        byte[] thumbnailBytes = (byte[]) null;
        byte[] bytes = (byte[]) null;
        if (args.Thumbnail != null)
          thumbnailBytes = args.Thumbnail.ToByteArray();
        if (args.FullSize != null)
          bytes = args.FullSize.ToByteArray();
        Action<int?> forceUpdate = (Action<int?>) (errCode =>
        {
          this.PhotoAttempted = true;
          ChatPictureStore.ForcePendingPictureUpdate(args.Jid, errCode);
        });
        conn.SendSetPhoto(args.Jid, bytes, thumbnailBytes, (Action) (() =>
        {
          observer.OnNext(new Unit());
          forceUpdate(new int?());
        }), (Action<int>) (err =>
        {
          if (err == 401 || err == 403 || err == 404)
            observer.OnNext(new Unit());
          forceUpdate(new int?(err));
        }), showSystemMessage: args.ShowSystemMessage);
        return (Action) (() => { });
      }));
    }

    public static IObservable<Unit> PerformSetStatus(
      string pendingStatus,
      string incomingId,
      FunXMPP.Connection conn)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        string currStatus = (string) null;
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          UserStatus userStatus = db.GetUserStatus(Settings.MyJid);
          if (userStatus == null)
            return;
          currStatus = userStatus.Status;
        }));
        if (pendingStatus == currStatus)
          conn.SendStatusUpdate(pendingStatus, incomingId, (Action) (() => observer.OnNext(new Unit())), (Action<int>) (errCode =>
          {
            Log.WriteLineDebug("persist action: set status failed | pending={0} err={1}", (object) pendingStatus, (object) errCode);
            observer.OnError(new Exception("set status failed: " + errCode.ToString()));
          }));
        else
          observer.OnNext(new Unit());
        return (Action) (() => { });
      }));
    }

    public static IObservable<Unit> PerformReuploadMedia(
      string jid,
      bool fromMe,
      string id,
      string participant,
      FunXMPP.Connection conn,
      int attempts)
    {
      return Observable.CreateWithDisposable<Unit>((Func<IObserver<Unit>, IDisposable>) (observer =>
      {
        Message msg = (Message) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => msg = db.GetMessage(jid, id, fromMe)));
        IObservable<MediaUploadMms4.Mms4UploadResult> source = (IObservable<MediaUploadMms4.Mms4UploadResult>) null;
        string recipient = JidHelper.IsMultiParticipantsChatJid(jid) ? participant : jid;
        if (msg != null && (source = MediaUploadMms4.GetUploadObservableForResendMaybeMms4(msg, recipient, false, new int?(attempts))) != null)
          return source.Subscribe<MediaUploadMms4.Mms4UploadResult>((Action<MediaUploadMms4.Mms4UploadResult>) (res =>
          {
            try
            {
              Log.l("reupload media", "Completed for {0}, {1}", (object) recipient, (object) res.SkipUpload);
              if (!res.SkipUpload)
                AppState.SchedulePersistentAction(PersistentAction.ReuploadMediaNotification(jid, true, id, participant));
              observer.OnNext(new Unit());
              observer.OnCompleted();
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "Exception when attempting reupload");
            }
            finally
            {
              observer.OnCompleted();
            }
          }));
        Log.l("reupload media", "Completed for {0} - {1} {2}", (object) recipient, (object) (msg == null), (object) (source == null));
        observer.OnNext(new Unit());
        observer.OnCompleted();
        return (IDisposable) new DisposableAction((Action) (() => { }));
      }));
    }

    public static IObservable<Unit> PerformReuploadMediaNotification(
      string jid,
      bool fromMe,
      string id,
      string participant,
      FunXMPP.Connection conn,
      int attempts)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        try
        {
          Log.l("reupload media", "Notifying for {0}, {1}", (object) (JidHelper.IsMultiParticipantsChatJid(jid) ? participant : jid), (object) id);
          AppState.GetConnection().SendEncryptedMediaNotification((Action) (() =>
          {
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action) (() => observer.OnCompleted()), jid, id, participant);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "Exception when attempting reupload");
        }
        return (Action) (() => { });
      }));
    }

    private static IObservable<Unit> PerformQr(string blobString)
    {
      byte[] blob = Encoding.UTF8.GetBytes(blobString);
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        try
        {
          QrPersistentAction.Instance.Process(blob, (Action) (() => observer.OnNext(new Unit())));
        }
        catch (Exception ex)
        {
          observer.OnError(ex);
          observer.OnCompleted();
        }
        return (Action) (() => { });
      }));
    }

    public static IObservable<Unit> PerformCapabilitiesRefresh(FunXMPP.Connection conn)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Set<string> set = new Set<string>();
        UserStatus[] contacts = (UserStatus[]) null;
        string[] jidStrings = (string[]) null;
        ContactsContext.Instance((Action<ContactsContext>) (db => contacts = db.GetWaContacts(false)));
        foreach (string key in ((IEnumerable<UserStatus>) (contacts ?? new UserStatus[0])).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)))
          set.Add(key);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => jidStrings = db.GetAllGroupParticipantsJids()));
        foreach (string key in jidStrings ?? new string[0])
          set.Add(key);
        string myJid = Settings.MyJid;
        if (myJid != null)
          set.Add(myJid);
        if (set.Any<string>())
          UsyncQueryRequest.SendGetRemoteCapabilities((IEnumerable<string>) set, onComplete: (Action) (() => observer.OnNext(new Unit())));
        else
          observer.OnNext(new Unit());
        return (Action) (() => { });
      })).SubscribeOn<Unit>(WAThreadPool.Scheduler);
    }

    public static IObservable<Unit> PerformProtocolBufferMessageUpgrade()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          bool flag = false;
          Message[] messagesByType = db.GetMessagesByType(FunXMPP.FMessage.Type.ProtocolBuffer);
          Log.l("FutureProof", "Messages {0}", (object) messagesByType.Length);
          foreach (Message message3 in messagesByType)
          {
            Message msg = message3;
            if (msg.BinaryData != null)
            {
              try
              {
                WhatsApp.ProtoBuf.Message message4 = WhatsApp.ProtoBuf.Message.Deserialize(msg.BinaryData);
                message4.Serialized = msg.BinaryData;
                if (message4.UnknownSerialized == null)
                {
                  FunXMPP.FMessage fmessage = msg.ToFMessage();
                  message4.PopulateFMessage(fmessage);
                  if (fmessage.media_wa_type == FunXMPP.FMessage.Type.HSM)
                  {
                    Log.l("StoreIncomingHSM", "creating persistent action from futureproofed msg");
                    string senderJid = fmessage.GetSenderJid();
                    AppState.SchedulePersistentAction(PersistentAction.RehydrateHighlyStructuredMessage(fmessage.key.remote_jid, fmessage.key.id, fmessage.remote_resource, fmessage.timestamp ?? DateTime.UtcNow, senderJid, fmessage.verified_name, fmessage.binary_data));
                  }
                  else
                  {
                    Message src = new Message(fmessage);
                    msg.CopyFrom((SqliteMessagesContext) db, src, true, true);
                    Conversation conversation = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None);
                    if (conversation != null)
                    {
                      conversation.UpdateModifyTag();
                      AppState.Worker.Enqueue((Action) (() =>
                      {
                        AppState.QrPersistentAction.NotifyMessage(msg, QrMessageForwardType.Update);
                        AppState.QrPersistentAction.NotifyChatStatus(msg.KeyRemoteJid, FunXMPP.ChatStatusForwardAction.ModifyTag);
                        if (!msg.FunTimestamp.HasValue || FunRunner.CurrentServerTimeUtc.Subtract(TimeSpan.FromDays(2.0)).CompareTo(msg.FunTimestamp.Value) >= 0)
                          return;
                        AppState.SchedulePersistentAction(PersistentAction.AutoDownload(msg));
                      }));
                    }
                    flag = true;
                  }
                  msg.BinaryData = (byte[]) null;
                }
                else
                  Log.l("FutureProof", "Unknown message found");
              }
              catch (Exception ex)
              {
                string context = string.Format("exception processing futureproofed message {0} {1}", (object) msg.KeyRemoteJid, (object) msg.KeyId);
                Log.LogException(ex, context);
              }
            }
          }
          if (!flag)
            return;
          db.SubmitChanges();
        }));
        observer.OnNext(new Unit());
        return (Action) (() => { });
      })).SubscribeOn<Unit>(WAThreadPool.Scheduler);
    }

    public static IObservable<Unit> PerformContactVCardIndex()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          bool flag = false;
          using (Sqlite.PreparedStatement stmt = db.PrepareStatement("SELECT * FROM ContactVCards"))
          {
            HashSet<int> intSet = new HashSet<int>(db.ParseTable<VCard>(stmt, "ContactVCards").Where<VCard>((Func<VCard, bool>) (vcard => vcard.MessageId.HasValue)).Select<VCard, int>((Func<VCard, int>) (vcard => vcard.MessageId.Value)));
            foreach (Message msg in db.GetMessagesByType(FunXMPP.FMessage.Type.Contact))
            {
              if (!intSet.Contains(msg.MessageID))
              {
                foreach (ContactVCard contactCard in msg.GetContactCards())
                  db.IndexContactVCard(contactCard, msg.MessageID);
                flag = true;
              }
            }
          }
          if (!flag)
            return;
          db.SubmitChanges();
        }));
        observer.OnNext(new Unit());
        return (Action) (() => { });
      }));
    }

    public static IObservable<Unit> PerformDisplayFullEncryptionToAllChats()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          foreach (JidInfo jidInfo in db.GetJidsPendingEncryptedAnnouncement())
          {
            jidInfo.SupportsFullEncryption = JidInfo.FullEncryptionState.SupportedAndNotified;
            if (jidInfo.Jid.IsGroupJid())
            {
              Conversation conversation = db.GetConversation(jidInfo.Jid, CreateOptions.None);
              if (conversation == null || conversation.IsReadOnly())
                continue;
            }
            Message conversationEncrypted = SystemMessageUtils.CreateConversationEncrypted(db, jidInfo.Jid);
            db.InsertMessageOnSubmit(conversationEncrypted);
            SystemMessageUtils.TryGenerateInitialBizSystemMessage2Tier((SqliteMessagesContext) db, jidInfo.Jid, false);
          }
          db.SubmitChanges();
        }));
        Settings.E2EVerificationCleanup = true;
        observer.OnNext(new Unit());
        return (Action) (() => { });
      }));
    }

    public static IObservable<Unit> PerformAddOneTime2TierSysMsgToBizChats()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        if (!Settings.BizChat2TierOneTimeSysMsgAdded.HasValue)
        {
          int bizUserCount = 0;
          int updateCount = 0;
          int userCount = 0;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            List<Conversation> conversations = db.GetConversations(new JidHelper.JidTypes[1]
            {
              JidHelper.JidTypes.User
            }, true);
            bool flag = false;
            foreach (Conversation conversation in conversations)
            {
              ++userCount;
              UserStatus user = UserCache.Get(conversation.Jid, false);
              if (user != null && user.IsVerified())
              {
                ++bizUserCount;
                Message systemMessage2Tier = SystemMessageUtils.MaybeCreateOneTimeBizSystemMessage2Tier(db, conversation.Jid, user.GetVerifiedNameForDisplay(), user.VerifiedLevel, user.GetVerifiedTier());
                if (systemMessage2Tier != null)
                {
                  ++updateCount;
                  flag = true;
                  db.InsertMessageOnSubmit(systemMessage2Tier);
                  flag = true;
                }
                else
                  Log.d("biz", "One time message - did not update user {0}, {1}", (object) conversation.Jid, (object) (user?.VerifiedLevel.ToString() ?? "null"));
              }
              else if (user == null)
                Log.d("biz", "One time message - could not find user {0}", (object) conversation.Jid);
            }
            if (!flag)
              return;
            db.SubmitChanges();
          }));
          Settings.BizChat2TierOneTimeSysMsgAdded = new DateTime?(DateTime.UtcNow);
          Log.l("biz", "Onetime update of biz chats - {0} updated from {1} biz, total:{2}", (object) updateCount, (object) bizUserCount, (object) userCount);
        }
        observer.OnNext(new Unit());
        return (Action) (() => { });
      }));
    }

    public static PersistentAction AckMedia(string url, bool webrequest)
    {
      return new PersistentAction()
      {
        ActionType = 1,
        ActionDataString = url,
        FromMe = webrequest
      };
    }

    public static PersistentAction LeaveGroup(string jid)
    {
      return new PersistentAction()
      {
        ActionType = 2,
        ActionDataString = jid
      };
    }

    public static PersistentAction LeaveAndDeleteGroup(string jid)
    {
      return new PersistentAction()
      {
        ActionType = 41,
        ActionDataString = jid
      };
    }

    public static PersistentAction SetRecoveryToken(byte[] token)
    {
      return new PersistentAction()
      {
        ActionType = 3,
        ActionData = token
      };
    }

    public static PersistentAction SendGetImage(string jid)
    {
      return new PersistentAction()
      {
        actionType = 6,
        actionDataString = jid,
        ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(1.0))
      };
    }

    public static PersistentAction SendDeleteBroadcastList(string broadcastListJid)
    {
      return new PersistentAction()
      {
        actionType = 8,
        actionDataString = broadcastListJid,
        attemptsLimit = new int?(3)
      };
    }

    public static PersistentAction SetPhoto(
      string jid,
      byte[] thumbnail,
      byte[] fullSize,
      bool showSystemMessage)
    {
      return new PersistentAction()
      {
        ActionType = 9,
        ActionData = PersistentAction.SerializePhotoArgs(jid, thumbnail, fullSize, showSystemMessage),
        AttemptsLimit = new int?(3)
      };
    }

    public static PersistentAction SetStatus(string status, string incomingId)
    {
      return new PersistentAction()
      {
        actionType = 10,
        actionDataString = status,
        Jid = incomingId
      };
    }

    public static PersistentAction ReuploadMedia(
      string jid,
      bool fromMe,
      string id,
      string participant)
    {
      return new PersistentAction()
      {
        ActionType = 12,
        Jid = jid,
        FromMe = fromMe,
        Id = id,
        ActionDataString = participant
      };
    }

    public static PersistentAction ReuploadMediaNotification(
      string jid,
      bool fromMe,
      string id,
      string participant)
    {
      return new PersistentAction()
      {
        ActionType = 42,
        Jid = jid,
        FromMe = fromMe,
        Id = id,
        ActionDataString = participant,
        ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(1.0)),
        Attempts = 20
      };
    }

    public static PersistentAction Qr(QrPersistentAction.PersistentData data)
    {
      byte[] bytes = data.Serialize();
      return new PersistentAction()
      {
        ActionType = 14,
        ActionDataString = Encoding.UTF8.GetString(bytes, 0, bytes.Length),
        AttemptsLimit = new int?(4)
      };
    }

    public static PersistentAction CapabilitiesRefresh()
    {
      return new PersistentAction() { ActionType = 23 };
    }

    public static PersistentAction ProtocolBufferMessageUpgrade()
    {
      return new PersistentAction() { ActionType = 24 };
    }

    public static PersistentAction ContactVCardsIndex()
    {
      return new PersistentAction() { ActionType = 25 };
    }

    public static PersistentAction DisplayFullEncryptionToAllChats()
    {
      return new PersistentAction() { ActionType = 26 };
    }

    public static PersistentAction AddOneTime2TierSysMsgToBizChats()
    {
      return new PersistentAction() { ActionType = 48 };
    }

    public static PersistentAction ResumeUpload(
      string hashBase64,
      string mediaCiperResumeUrl,
      string mediaCipherRefs,
      string ouId)
    {
      return new PersistentAction()
      {
        ActionType = 28,
        ActionData = PersistentAction.SerializeResumeArgs(hashBase64, mediaCiperResumeUrl, mediaCipherRefs, ouId)
      };
    }

    private static byte[] SerializeResumeArgs(
      string hashBase64,
      string mediaCiperResumeUrl,
      string mediaCipherRefs,
      string ouId)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 1);
      binaryData.AppendInt32(hashBase64.Length);
      binaryData.AppendBytes((IEnumerable<byte>) Encoding.UTF8.GetBytes(hashBase64));
      binaryData.AppendInt32(mediaCiperResumeUrl.Length);
      binaryData.AppendBytes((IEnumerable<byte>) Encoding.UTF8.GetBytes(mediaCiperResumeUrl));
      binaryData.AppendInt32(mediaCipherRefs.Length);
      binaryData.AppendBytes((IEnumerable<byte>) Encoding.UTF8.GetBytes(mediaCipherRefs));
      binaryData.AppendInt32(ouId.Length);
      binaryData.AppendBytes((IEnumerable<byte>) Encoding.UTF8.GetBytes(ouId));
      return binaryData.Get();
    }

    private IObservable<Unit> PerformRehashLocalFiles()
    {
      return AppState.IsBackgroundAgent && BackgroundAgent.agentType != BackgroundAgent.BackgroundAgentType.AudioAgent ? Observable.Never<Unit>() : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Action iterate = (Action) null;
        iterate = (Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          if (this.hashUpdateInProgress && !AppState.IsBackgroundAgent)
            return;
          this.hashUpdateInProgress = true;
          db.FindAndUpdateLocalFileHashes((Action) (() =>
          {
            this.hashUpdateInProgress = false;
            iterate();
          }), (Action) (() =>
          {
            this.hashUpdateInProgress = false;
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }));
        })));
        iterate();
        return (Action) (() => { });
      }));
    }

    private IObservable<Unit> PerformOptimisticUploadResume(byte[] actionData)
    {
      string hashBase64;
      string mediaCiperResumeUrl;
      string mediaCipherRefs;
      string ouId;
      if (!PersistentAction.DeserializeResumeArgs(actionData, out hashBase64, out mediaCiperResumeUrl, out mediaCipherRefs, out ouId))
        return Observable.Never<Unit>();
      Log.l("OPU", "Persistent Action resuming optimistic upload {0} to {1}", (object) ouId, (object) MediaDownload.RedactUrl(mediaCiperResumeUrl));
      return MediaUpload.ResumePersistentAction(hashBase64, mediaCiperResumeUrl, mediaCipherRefs);
    }

    public static bool DeserializeResumeArgs(
      byte[] actionData,
      out string hashBase64,
      out string mediaCiperResumeUrl,
      out string mediaCipherRefs,
      out string ouId)
    {
      hashBase64 = (string) null;
      mediaCiperResumeUrl = (string) null;
      mediaCipherRefs = (string) null;
      ouId = (string) null;
      if (actionData == null)
        return false;
      try
      {
        if (actionData[0] != (byte) 1)
          return false;
        int offset1 = 1;
        int count1 = BinaryData.ReadInt32(actionData, offset1);
        int index1 = offset1 + 4;
        hashBase64 = Encoding.UTF8.GetString(actionData, index1, count1);
        int offset2 = index1 + count1;
        int count2 = BinaryData.ReadInt32(actionData, offset2);
        int index2 = offset2 + 4;
        mediaCiperResumeUrl = Encoding.UTF8.GetString(actionData, index2, count2);
        int offset3 = index2 + count2;
        int count3 = BinaryData.ReadInt32(actionData, offset3);
        int index3 = offset3 + 4;
        mediaCipherRefs = Encoding.UTF8.GetString(actionData, index3, count3);
        int offset4 = index3 + count3;
        int count4 = BinaryData.ReadInt32(actionData, offset4);
        int index4 = offset4 + 4;
        ouId = Encoding.UTF8.GetString(actionData, index4, count4);
        int num = index4 + count4;
        return true;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception deserializing resume");
        return false;
      }
    }

    public static PersistentAction RehydrateHighlyStructuredMessage(
      string jid,
      string keyId,
      string remoteResource,
      DateTime timeout,
      string senderJid,
      ulong? serial,
      byte[] serializedHighlyStructuredMessage)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 3);
      binaryData.AppendStrWithLengthPrefix(remoteResource);
      binaryData.AppendLong64(timeout.ToBinary());
      if (serial.HasValue)
        binaryData.AppendULong64(serial.Value);
      else
        binaryData.AppendULong64(0UL);
      binaryData.AppendStrWithLengthPrefix(senderJid);
      binaryData.AppendInt32(serializedHighlyStructuredMessage.Length);
      binaryData.AppendBytes((IEnumerable<byte>) serializedHighlyStructuredMessage);
      return new PersistentAction()
      {
        ActionType = 29,
        Jid = jid,
        Id = keyId,
        ActionData = binaryData.Get(),
        AttemptsLimit = new int?(int.MaxValue)
      };
    }

    public IObservable<Unit> PerformRehydrateHighlyStructuredMessage(
      string jid,
      string keyId,
      byte[] actionData)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        BinaryData binaryData = new BinaryData(actionData);
        int offset1 = 0;
        int num = (int) binaryData.ReadByte(offset1);
        int newOffset1 = offset1 + 1;
        if (num != 3)
          observer.OnNext(new Unit());
        string remoteResource = binaryData.ReadStrWithLengthPrefix(newOffset1, out newOffset1);
        DateTime timeout = DateTime.FromBinary(binaryData.ReadLong64(newOffset1));
        int offset2 = newOffset1 + 8;
        ulong serial = binaryData.ReadULong64(offset2);
        int newOffset2 = offset2 + 8;
        string senderJid = binaryData.ReadStrWithLengthPrefix(newOffset2, out newOffset2);
        int length = binaryData.ReadInt32(newOffset2);
        newOffset2 += 4;
        byte[] serializedHighlyStructuredMessage = binaryData.ReadBytes(newOffset2, length);
        Log.d("hsm", "running rehydrate");
        ((Action) (async () =>
        {
          try
          {
            Log.d("hsm", "running rehydrate action");
            bool flag = await HighlyStructuredMessage.RehydrateMessageAsync(jid, keyId, remoteResource, timeout, senderJid, serial, serializedHighlyStructuredMessage);
            Log.d("hsm", "ran rehydrate action {0}", (object) flag);
            if (!flag)
              return;
            observer.OnNext(new Unit());
          }
          catch (Exception ex)
          {
            Log.SendCrashLog(ex, "hsm exception rehydrating message", logOnlyForRelease: true);
          }
          finally
          {
            observer.OnCompleted();
          }
        }))();
        return (Action) (() => { });
      }));
    }

    public IObservable<Unit> PerformNotifyChangedNumber(FunXMPP.Connection conn, byte[] actionData)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        if (actionData == null)
        {
          observer.OnNext(new Unit());
          observer.OnCompleted();
          return (Action) (() => { });
        }
        BinaryData binaryData = new BinaryData(actionData);
        int newOffset1 = 0;
        string oldChatId = binaryData.ReadStrWithLengthPrefix(newOffset1, out newOffset1);
        int n = binaryData.ReadInt32(newOffset1);
        int newOffset2 = newOffset1 + 4;
        List<string> notifyJids = new List<string>();
        for (int index = 0; index < n; ++index)
        {
          string str = binaryData.ReadStrWithLengthPrefix(newOffset2, out newOffset2);
          notifyJids.Add(str);
        }
        Log.l("change number", "notify {0} contacts | old:{1}", (object) n, (object) oldChatId);
        conn.SendChangeNumber(oldChatId, (IEnumerable<string>) notifyJids).Subscribe<Unit>((Action<Unit>) (_ =>
        {
          Log.l("change number", "notify {0} contacts success", (object) n);
          observer.OnNext(new Unit());
        }), (Action<Exception>) (ex =>
        {
          Log.l("change number", "notify {0} contacts failed | err:{1}", (object) n, (object) ex.Message);
          observer.OnCompleted();
        }), (Action) (() => observer.OnCompleted()));
        return (Action) (() => { });
      }));
    }

    public enum Types
    {
      Unknown,
      AckMedia,
      LeaveGroup,
      SetRecoveryToken,
      AutoDownload,
      SendPlayedReceipt,
      SendGetImage,
      SendChangeNumber,
      SendDeleteBroadcastList,
      SetPhoto,
      SetStatus,
      SendReceipts,
      ReuploadMedia,
      DeleteChatPic,
      Qr,
      SendPostponedReceipts,
      SendRetryReceipt,
      SendVerifyAxolotlDigest,
      SendGroupEncryptionWelcome,
      SendIndividualRetryForGroup,
      SendTosUpdateStage,
      SendTosUpdateAccept,
      IdentityChangedForUser,
      CapabilitiesRefresh,
      ProtocolBufferMessageUpgrade,
      ContactVCardIndex,
      DisplayFullEncryptionToAllChats,
      ReHashLocalFiles,
      OptimisticResume,
      RehydrateHighlyStructuredMessage,
      CertifyVerifiedUser,
      MayBeReused,
      SendStatusV3PrivacyList,
      NotifyChangedNumber,
      SendPaymentsRequest,
      Mms4RouteSelection,
      SendEnableLocationSharing,
      SendDisableLocationSharing,
      SendSubscribeToLocationUpdates,
      SendUnsubscribeToLocationUpdates,
      SendRetryLocationKey,
      LeaveAndDeleteGroup,
      ReuploadMediaNotification,
      SendGdprStage,
      SendGdprAccept,
      AckGdprTosReset,
      SendGdprTosPage,
      DeleteGdprReport,
      AddOneTime2TierSysMsgToBizChats,
      Mms4HostSelection,
    }

    public enum DownloadConnectionTypes
    {
      Unknown,
      Wifi,
      CellData,
    }

    public class SetPhotoArgs
    {
      public string Jid;
      public PersistentAction.SetPhotoArgs.Buffer FullSize;
      public PersistentAction.SetPhotoArgs.Buffer Thumbnail;
      public bool ShowSystemMessage;

      public class Buffer
      {
        public byte[] Buf;
        public int Offset;
        public int Length;

        public byte[] ToByteArray()
        {
          byte[] destinationArray = new byte[this.Length];
          Array.Copy((Array) this.Buf, this.Offset, (Array) destinationArray, 0, destinationArray.Length);
          return destinationArray;
        }
      }
    }
  }
}
