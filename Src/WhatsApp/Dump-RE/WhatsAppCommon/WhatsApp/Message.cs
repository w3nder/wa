// Decompiled with JetBrains decompiler
// Type: WhatsApp.Message
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "KeyRemoteJid,KeyFromMe,KeyId", IsUnique = true)]
  [Index(Columns = "KeyRemoteJid,MessageID", Name = "LoadPerformanceIndex")]
  [Index(Columns = "MediaUrl")]
  [Index(Columns = "BackgroundID", Name = "BackgroundID")]
  [Index(Columns = "Status", Name = "Status")]
  [Index(Columns = "FtsStatus,MessageID", Name = "FtsStatusIndex")]
  [Index(Columns = "IsStarred", Name = "IsStarredIndex")]
  [Index(Columns = "Flags", Name = "FlagsIndex")]
  [Index(Columns = "MediaHash", Name = "MediaHashIndex")]
  public class Message : PropChangingChangedBase
  {
    public const int DummyIDForBroadcastListMessage = 0;
    public const int DummyIDForUnreadDivider = -101;
    public const int DummyIDForDateDivider = -102;
    private int messageId;
    private string keyRemoteJid;
    private bool keyFromMe;
    private string keyId;
    private FunXMPP.FMessage.Status status;
    private string remoteResource;
    private bool wantsReceipt;
    private string data;
    private byte[] binaryData;
    private string dataFileName;
    private long timestampLong;
    private string pushName;
    private string mediaUrl;
    private string mediaIp;
    private string mediaMimeType;
    private FunXMPP.FMessage.Type mediaWaType;
    private long mediaSize;
    private int mediaDurationSeconds;
    private string mediaOrigin;
    private string mediaName;
    private byte[] mediaHash;
    private string mediaCaption;
    private byte[] mediaKey;
    private double latitude;
    private double longitude;
    private string localFileUri;
    private string backgroundId;
    private string locationDetails;
    private string locationUrl;
    private int? broadcastMsgId_;
    private byte[] textPerformanceHint;
    private byte[] textSplittingHint;
    private int ftsStatus;
    private string participantsHash;
    private bool isStarred;
    private Message.MessageFlags flags;
    private byte[] internalPropertiesProtoBuf;
    private byte[] protoBuf;
    private string quotedMediaFileUri;
    private bool transferInProgress;
    private double? transferValue;
    private IDisposable pendingMediaSubscription;
    private object pendingMediaSubLock = new object();
    private bool playbackInProgress;
    private double playbackValue;
    public UploadContext UploadContext;
    private MessageMiscInfo miscInfo;
    private string cachedDisplayText;
    private LinkDetector.Result[] cachedFormattings;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int MessageID
    {
      get => this.messageId;
      set
      {
        if (this.messageId == value)
          return;
        this.NotifyPropertyChanging(nameof (MessageID));
        this.messageId = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string KeyRemoteJid
    {
      get => this.keyRemoteJid;
      set
      {
        if (!(this.keyRemoteJid != value))
          return;
        this.NotifyPropertyChanging(nameof (KeyRemoteJid));
        this.keyRemoteJid = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool KeyFromMe
    {
      get => this.keyFromMe;
      set
      {
        if (this.keyFromMe == value)
          return;
        this.NotifyPropertyChanging(nameof (KeyFromMe));
        this.keyFromMe = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string KeyId
    {
      get => this.keyId;
      set
      {
        if (!(this.keyId != value))
          return;
        this.NotifyPropertyChanging(nameof (KeyId));
        this.keyId = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public FunXMPP.FMessage.Status Status
    {
      get => this.status;
      set
      {
        if (this.status == value)
          return;
        this.NotifyPropertyChanging(nameof (Status));
        this.status = value;
        this.NotifyPropertyChanged(nameof (Status));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string RemoteResource
    {
      get => this.remoteResource;
      set
      {
        if (!(this.remoteResource != value))
          return;
        this.NotifyPropertyChanging(nameof (RemoteResource));
        this.remoteResource = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool WantsReceipt
    {
      get => this.wantsReceipt;
      set
      {
        if (this.wantsReceipt == value)
          return;
        this.NotifyPropertyChanging(nameof (WantsReceipt));
        this.wantsReceipt = value;
      }
    }

    [Column(DbType = "ntext", UpdateCheck = UpdateCheck.Never)]
    [Sensitive]
    public string Data
    {
      get => this.data;
      set
      {
        if (!(this.data != value))
          return;
        this.NotifyPropertyChanging(nameof (Data));
        this.data = value;
      }
    }

    [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
    [Sensitive]
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
    public string DataFileName
    {
      get => this.dataFileName;
      set
      {
        if (!(this.dataFileName != value))
          return;
        this.NotifyPropertyChanging(nameof (DataFileName));
        this.dataFileName = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public long TimestampLong
    {
      get => this.timestampLong;
      set
      {
        if (this.timestampLong == value)
          return;
        this.NotifyPropertyChanging(nameof (TimestampLong));
        this.timestampLong = DateTimeUtils.SanitizeTimestamp(value);
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public long? CreationTimeLong { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [Sensitive]
    public string PushName
    {
      get => this.pushName;
      set
      {
        if (!(this.pushName != value))
          return;
        this.NotifyPropertyChanging(nameof (PushName));
        this.pushName = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string MediaUrl
    {
      get => this.mediaUrl;
      set
      {
        if (!(this.mediaUrl != value))
          return;
        this.NotifyPropertyChanging(nameof (MediaUrl));
        this.mediaUrl = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string MediaIp
    {
      get => this.mediaIp;
      set
      {
        if (!(this.mediaIp != value))
          return;
        this.NotifyPropertyChanging(nameof (MediaIp));
        this.mediaIp = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string MediaMimeType
    {
      get => this.mediaMimeType;
      set
      {
        if (!(this.mediaMimeType != value))
          return;
        this.NotifyPropertyChanging(nameof (MediaMimeType));
        this.mediaMimeType = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public FunXMPP.FMessage.Type MediaWaType
    {
      get => this.mediaWaType;
      set
      {
        if (this.mediaWaType == value)
          return;
        int num = this.mediaWaType == FunXMPP.FMessage.Type.CipherText ? 1 : 0;
        this.NotifyPropertyChanging(nameof (MediaWaType));
        this.mediaWaType = value;
        if (num == 0)
          return;
        this.NotifyPropertyChanged(nameof (MediaWaType));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public long MediaSize
    {
      get => this.mediaSize;
      set
      {
        if (this.mediaSize == value)
          return;
        this.NotifyPropertyChanging(nameof (MediaSize));
        this.mediaSize = value;
        this.NotifyPropertyChanged(nameof (MediaSize));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int MediaDurationSeconds
    {
      get => this.mediaDurationSeconds;
      set
      {
        if (this.mediaDurationSeconds == value)
          return;
        this.NotifyPropertyChanging(nameof (MediaDurationSeconds));
        this.mediaDurationSeconds = value;
        this.NotifyPropertyChanged(nameof (MediaDurationSeconds));
      }
    }

    public long? EndLiveLocation()
    {
      if (!FunXMPP.FMessage.Type.LiveLocation.Equals((object) this.MediaWaType))
        return new long?();
      int num = 0;
      if (this.FunTimestamp.HasValue)
        num = (int) (FunRunner.CurrentServerTimeUtc - this.FunTimestamp.Value).TotalSeconds;
      if (num < this.MediaDurationSeconds)
        this.MediaDurationSeconds = num;
      return new long?((long) this.MediaDurationSeconds);
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string MediaOrigin
    {
      get => this.mediaOrigin;
      set
      {
        if (!(this.mediaOrigin != value))
          return;
        this.NotifyPropertyChanging(nameof (MediaOrigin));
        this.mediaOrigin = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string MediaName
    {
      get => this.mediaName;
      set
      {
        if (!(this.mediaName != value))
          return;
        this.NotifyPropertyChanging(nameof (MediaName));
        this.mediaName = value;
      }
    }

    [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
    public byte[] MediaHash
    {
      get => this.mediaHash;
      set
      {
        if (this.mediaHash == value)
          return;
        this.NotifyPropertyChanging(nameof (MediaHash));
        this.mediaHash = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string MediaCaption
    {
      get => this.mediaCaption;
      set
      {
        if (!(this.mediaCaption != value))
          return;
        this.NotifyPropertyChanging(nameof (MediaCaption));
        this.mediaCaption = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] MediaKey
    {
      get => this.mediaKey;
      set
      {
        if (this.mediaKey == value)
          return;
        this.NotifyPropertyChanging(nameof (MediaKey));
        this.mediaKey = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public double Latitude
    {
      get => this.latitude;
      set
      {
        if (this.latitude == value)
          return;
        this.NotifyPropertyChanging(nameof (Latitude));
        this.latitude = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public double Longitude
    {
      get => this.longitude;
      set
      {
        if (this.longitude == value)
          return;
        this.NotifyPropertyChanging(nameof (Longitude));
        this.longitude = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string LocalFileUri
    {
      get => this.localFileUri;
      set
      {
        if (!(this.localFileUri != value))
          return;
        this.NotifyPropertyChanging(nameof (LocalFileUri));
        this.localFileUri = value;
        this.NotifyPropertyChanged(nameof (LocalFileUri));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string BackgroundID
    {
      get => this.backgroundId;
      set
      {
        if (!(this.backgroundId != value))
          return;
        this.NotifyPropertyChanging(nameof (BackgroundID));
        this.backgroundId = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string LocationDetails
    {
      get => this.locationDetails;
      set
      {
        if (!(this.locationDetails != value))
          return;
        this.NotifyPropertyChanging(nameof (LocationDetails));
        this.locationDetails = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string LocationUrl
    {
      get => this.locationUrl;
      set
      {
        if (!(this.locationUrl != value))
          return;
        this.NotifyPropertyChanging(nameof (LocationUrl));
        this.locationUrl = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? BroadcastMessageID
    {
      get => this.broadcastMsgId_;
      set
      {
        int? broadcastMsgId = this.broadcastMsgId_;
        int? nullable = value;
        if ((broadcastMsgId.GetValueOrDefault() == nullable.GetValueOrDefault() ? (broadcastMsgId.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (BroadcastMessageID));
        this.broadcastMsgId_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] TextPerformanceHint
    {
      get => this.textPerformanceHint;
      set
      {
        if (this.textPerformanceHint == value)
          return;
        this.NotifyPropertyChanging(nameof (TextPerformanceHint));
        this.textPerformanceHint = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] TextSplittingHint
    {
      get => this.textSplittingHint;
      set
      {
        if (this.textSplittingHint == value)
          return;
        this.NotifyPropertyChanging(nameof (TextSplittingHint));
        this.textSplittingHint = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int FtsStatus
    {
      get => this.ftsStatus;
      set
      {
        this.NotifyPropertyChanging(nameof (FtsStatus));
        this.ftsStatus = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string ParticipantsHash
    {
      get => this.participantsHash;
      set
      {
        if (!(this.participantsHash != value))
          return;
        this.NotifyPropertyChanging(nameof (ParticipantsHash));
        this.participantsHash = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool IsStarred
    {
      get => this.isStarred;
      set
      {
        if (this.isStarred == value)
          return;
        this.NotifyPropertyChanging(nameof (IsStarred));
        this.isStarred = value;
        this.NotifyPropertyChanged(nameof (IsStarred));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public Message.MessageFlags Flags
    {
      get => this.flags;
      set
      {
        if (this.flags == value)
          return;
        this.NotifyPropertyChanging(nameof (Flags));
        this.flags = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] InternalPropertiesProtobuf
    {
      get => this.internalPropertiesProtoBuf;
      set
      {
        if (this.internalPropertiesProtoBuf == value)
          return;
        this.NotifyPropertyChanging(nameof (InternalPropertiesProtobuf));
        this.internalPropertiesProtoBuf = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] ProtoBuf
    {
      get => this.protoBuf;
      set
      {
        if (this.protoBuf == value)
          return;
        this.NotifyPropertyChanging(nameof (ProtoBuf));
        this.protoBuf = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string QuotedMediaFileUri
    {
      get => this.quotedMediaFileUri;
      set
      {
        if (!(this.quotedMediaFileUri != value))
          return;
        this.NotifyPropertyChanging(nameof (QuotedMediaFileUri));
        this.quotedMediaFileUri = value;
        this.NotifyPropertyChanged(nameof (QuotedMediaFileUri));
      }
    }

    public MessageProperties InternalProperties
    {
      get
      {
        return this.InternalPropertiesProtobuf != null ? MessageProperties.Deserialize(this.InternalPropertiesProtobuf) : (MessageProperties) null;
      }
      set
      {
        this.InternalPropertiesProtobuf = value == null ? (byte[]) null : MessageProperties.SerializeToBytes(value);
      }
    }

    public DateTime? LocalTimestamp
    {
      get
      {
        DateTime? funTimestamp = this.FunTimestamp;
        return !funTimestamp.HasValue ? new DateTime?() : new DateTime?(DateTimeUtils.FunTimeToPhoneTime(funTimestamp.Value));
      }
      private set
      {
        this.FunTimestamp = value.HasValue ? new DateTime?(DateTimeUtils.PhoneTimeToFunTime(value.Value)) : new DateTime?();
      }
    }

    public DateTime? FunTimestamp
    {
      get
      {
        DateTime? funTimestamp = DateTimeUtils.NullableFromUnixTime(this.TimestampLong);
        if (funTimestamp.HasValue)
          return funTimestamp;
        long? creationTimeLong = this.CreationTimeLong;
        if (!creationTimeLong.HasValue)
          return new DateTime?();
        creationTimeLong = this.CreationTimeLong;
        return DateTimeUtils.NullableFromUnixTime(creationTimeLong.Value);
      }
      set => this.TimestampLong = value.HasValue ? value.Value.ToUnixTime() : 0L;
    }

    public bool NotifyTransfer { get; set; } = true;

    public bool TransferInProgress
    {
      get => this.transferInProgress;
      private set
      {
        this.transferInProgress = value;
        if (!this.NotifyTransfer)
          return;
        this.NotifyPropertyChanged(nameof (TransferInProgress));
      }
    }

    public double TransferValue
    {
      get => this.transferValue.HasValue ? Math.Min(this.transferValue.Value, 1.0) : 0.0;
      set
      {
        if (this.transferValue.HasValue && this.transferValue.Value == value)
          return;
        this.transferValue = new double?(value);
        if (!this.NotifyTransfer)
          return;
        this.NotifyPropertyChanged(nameof (TransferValue));
      }
    }

    public bool DownloadInProgress
    {
      get
      {
        if (!string.IsNullOrEmpty(this.LocalFileUri))
          return false;
        if (this.TransferInProgress)
          return true;
        MessageMiscInfo misc = this.GetMiscInfo();
        if (misc == null)
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => misc = this.GetMiscInfo((SqliteMessagesContext) db, CreateOptions.CreateIfNotFound)));
        return misc.BackgroundId != null;
      }
    }

    public bool IsAutoDownloading { get; set; }

    public bool IsPrefetchingVideo => this.IsAutoDownloading && this.ShouldPrefetchVideo();

    public bool HasSidecar()
    {
      return (this.InternalProperties?.MediaPropertiesField?.Sidecar?.Length ?? 0) != 0;
    }

    public void SetPendingMediaSubscription(
      string loggingInfo,
      PendingMediaTransfer.TransferTypes tType,
      IObservable<Unit> observer,
      Action<Unit> onNext = null,
      Action<Exception> onError = null,
      Action onCompleted = null,
      bool disposeExisting = false)
    {
      lock (this.pendingMediaSubLock)
      {
        if (this.pendingMediaSubscription != null)
        {
          if (disposeExisting)
          {
            this.pendingMediaSubscription.SafeDispose();
            this.pendingMediaSubscription = (IDisposable) null;
          }
          else
          {
            int int32 = GCHandle.ToIntPtr(GCHandle.Alloc((object) this, GCHandleType.WeakTrackResurrection)).ToInt32();
            Log.l("pendingMedia", string.Format("{0} Media subscribe ignored - already active {1} @ {2}", (object) loggingInfo, (object) this.messageId, (object) int32));
            return;
          }
        }
        int int32_1 = GCHandle.ToIntPtr(GCHandle.Alloc((object) this, GCHandleType.WeakTrackResurrection)).ToInt32();
        Log.l("pendingMedia", string.Format("{0} Media subscribe accepted {1} @ {2}", (object) loggingInfo, (object) this.messageId, (object) int32_1));
        if (!MediaTransferUtils.AddPendingTransfer(this, tType, onNext, onError, onCompleted))
          return;
        this.pendingMediaSubscription = observer.Subscribe<Unit>((Action<Unit>) (unit =>
        {
          Action<Unit> action = onNext;
          if (action == null)
            return;
          action(unit);
        }), (Action<Exception>) (ex =>
        {
          Log.LogException(ex, string.Format("{0} exception", (object) loggingInfo));
          Action<Exception> action = onError;
          if (action != null)
            action(ex);
          this.ClearPendingMedia();
        }), (Action) (() =>
        {
          Action action = onCompleted;
          if (action != null)
            action();
          this.ClearPendingMedia();
        }));
        this.TransferInProgress = true;
      }
    }

    public void ClearPendingMedia()
    {
      lock (this.pendingMediaSubLock)
      {
        Log.l("pendingMedia", string.Format("Media subscribe cleared {0} @ {1} ({2},{3})", (object) this.messageId, (object) GCHandle.ToIntPtr(GCHandle.Alloc((object) this, GCHandleType.WeakTrackResurrection)).ToInt32(), (object) (this.pendingMediaSubscription != null), (object) this.TransferInProgress));
        this.pendingMediaSubscription = (IDisposable) null;
        this.TransferInProgress = false;
      }
      MediaTransferUtils.RemovePendingTransfer(this);
    }

    public bool CancelPendingMedia()
    {
      bool flag = false;
      lock (this.pendingMediaSubLock)
      {
        Log.l("pendingMedia", string.Format("Media subscribe cancelled {0} @ {1} ({2},{3})", (object) this.messageId, (object) GCHandle.ToIntPtr(GCHandle.Alloc((object) this, GCHandleType.WeakTrackResurrection)).ToInt32(), (object) (this.pendingMediaSubscription != null), (object) this.TransferInProgress));
        flag = this.pendingMediaSubscription != null;
        this.pendingMediaSubscription.SafeDispose();
        this.pendingMediaSubscription = (IDisposable) null;
        this.TransferInProgress = false;
      }
      MediaTransferUtils.RemovePendingTransfer(this);
      return flag;
    }

    public bool PlaybackInProgress
    {
      get => this.playbackInProgress;
      set
      {
        if (this.playbackInProgress == value)
          return;
        this.playbackInProgress = value;
        this.NotifyPropertyChanged(nameof (PlaybackInProgress));
      }
    }

    public double PlaybackValue
    {
      get => this.playbackValue;
      set
      {
        this.playbackValue = value;
        this.NotifyPropertyChanged(nameof (PlaybackValue));
      }
    }

    public bool IsAlerted { get; set; }

    public bool IsAutomuted { get; set; }

    public MessageMiscInfo GetMiscInfo(SqliteMessagesContext db = null, CreateOptions options = CreateOptions.None)
    {
      if (db == null || this.MessageID == 0 || this.miscInfo != null && this.IsMiscInfoUpToDate(db, this.miscInfo))
        return this.miscInfo;
      MessageMiscInfo messageMiscInfo = db.GetMessageMiscInfo(this.MessageID, options);
      this.miscInfo = messageMiscInfo;
      return messageMiscInfo;
    }

    private bool IsMiscInfoUpToDate(SqliteMessagesContext db, MessageMiscInfo misc)
    {
      int cacheVersion1 = db.GetCacheVersion();
      int? cacheVersion2 = db.GetCacheVersion("MessageMiscInfos", (object) misc.ID);
      int valueOrDefault = cacheVersion2.GetValueOrDefault();
      return cacheVersion1 == valueOrDefault && cacheVersion2.HasValue;
    }

    public bool SetMiscInfo(MessageMiscInfo misc, MessagesContext db = null)
    {
      if (misc.MessageId.HasValue)
      {
        if (misc.MessageId.Value == this.MessageID)
        {
          this.miscInfo = misc;
          return true;
        }
      }
      else if (this.GetMiscInfo((SqliteMessagesContext) db) == null)
      {
        misc.MessageId = new int?(this.MessageID);
        this.miscInfo = misc;
        return true;
      }
      return false;
    }

    public static bool LocationDetailsUsable(string locationDetails)
    {
      return !string.IsNullOrEmpty(locationDetails) && !(locationDetails == "\n");
    }

    public static Message.PlaceDetails SplitPlaceDetails(string locationDetails)
    {
      if (!Message.LocationDetailsUsable(locationDetails))
        return (Message.PlaceDetails) null;
      string str = locationDetails.ConvertLineEndings();
      int length = str.IndexOf('\n');
      return new Message.PlaceDetails()
      {
        Name = length < 0 ? str : str.Substring(0, length),
        Address = length < 0 ? "" : str.Substring(length + 1)
      };
    }

    public Message.PlaceDetails ParsePlaceDetails()
    {
      Message.PlaceDetails placeDetails = Message.SplitPlaceDetails(this.LocationDetails);
      if (placeDetails == null)
        return placeDetails;
      try
      {
        string uriString = this.LocationUrl;
        if (uriString != null)
        {
          int length = uriString.IndexOf(':');
          if (length < 0 || !this.ValidScheme(uriString.Substring(0, length)))
            uriString = "http://" + uriString;
          Uri uri = new Uri(uriString);
          placeDetails.Url = uriString;
        }
      }
      catch (Exception ex)
      {
      }
      if (placeDetails.Url == null)
        placeDetails.Url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://bing.com/maps/?q={0}&sll={1},{2}", (object) Uri.EscapeUriString(placeDetails.Name), (object) this.Latitude, (object) this.Longitude);
      return placeDetails;
    }

    private bool ValidScheme(string scheme)
    {
      if (scheme.Length == 0 || !char.IsLetter(scheme[0]))
        return false;
      for (int index = 1; index < scheme.Length; ++index)
      {
        if (!char.IsLetterOrDigit(scheme[index]) && !"+-.".Contains<char>(scheme[index]))
          return false;
      }
      return true;
    }

    public string GetTextForDisplay(bool replaceNewLines = false)
    {
      string str = this.cachedDisplayText;
      if (this.cachedDisplayText == null | replaceNewLines)
      {
        str = Emoji.ConvertToUnicode(this.Data ?? this.MediaCaption ?? "");
        if (replaceNewLines)
          str = NotificationString.ReplaceNewlines(str);
        else
          this.cachedDisplayText = str;
      }
      return str;
    }

    public LinkDetector.Result[] GetRichTextFormattings(bool skipCache = false, bool replaceNewLines = false)
    {
      if (this.cachedFormattings != null && !skipCache)
        return this.cachedFormattings;
      if (!this.HasText())
        return (LinkDetector.Result[]) null;
      LinkDetector.Result[] richTextFormattings = (LinkDetector.Result[]) null;
      string textForDisplay = this.GetTextForDisplay(replaceNewLines);
      if (this.TextPerformanceHint != null)
        richTextFormattings = LinkDetector.Result.Deserialize(textForDisplay, this.TextPerformanceHint);
      if (richTextFormattings == null)
      {
        Log.d((Func<string>) (() => this.LogInfo()), "get rich text chunks | re-match and enqueue save");
        richTextFormattings = LinkDetector.GetMatches(textForDisplay).ToArray<LinkDetector.Result>();
        if (!replaceNewLines)
        {
          byte[] snap = LinkDetector.Result.Serialize((IEnumerable<LinkDetector.Result>) richTextFormattings);
          AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Log.d((Func<string>) (() => this.LogInfo()), "saving performance hint");
            this.TextPerformanceHint = snap;
            db.SubmitChanges();
          }))));
        }
      }
      WaRichText.Chunk[] mentionChunks = this.GetMentionChunks();
      if (((IEnumerable<WaRichText.Chunk>) mentionChunks).Any<WaRichText.Chunk>())
      {
        if (richTextFormattings == null || !((IEnumerable<LinkDetector.Result>) richTextFormattings).Any<LinkDetector.Result>())
          richTextFormattings = new LinkDetector.Result[1]
          {
            new LinkDetector.Result(0, textForDisplay.Length, 0, textForDisplay)
          };
        List<LinkDetector.Result> resultList = new List<LinkDetector.Result>();
        LinkedList<LinkDetector.Result> source1 = new LinkedList<LinkDetector.Result>((IEnumerable<LinkDetector.Result>) richTextFormattings);
        LinkedList<WaRichText.Chunk> source2 = new LinkedList<WaRichText.Chunk>((IEnumerable<WaRichText.Chunk>) mentionChunks);
        while (source2.Any<WaRichText.Chunk>())
        {
          if (!source1.Any<LinkDetector.Result>())
          {
            Log.d((Func<string>) (() => this.LogInfo()), "remaining mentions fall beyond last hint");
            using (LinkedList<WaRichText.Chunk>.Enumerator enumerator = source2.GetEnumerator())
            {
              while (enumerator.MoveNext())
                Log.d((Func<string>) (() => this.LogInfo()), "mention dropped | {0}", (object) enumerator.Current.AuxiliaryInfo);
              break;
            }
          }
          else
          {
            LinkDetector.Result other = source1.First.Value;
            WaRichText.Chunk chunk = source2.First.Value;
            if (other.Index > chunk.Offset)
            {
              source2.RemoveFirst();
              Log.d((Func<string>) (() => this.LogInfo()), "mention dropped | {0}", (object) chunk.AuxiliaryInfo);
            }
            else
            {
              int length = chunk.Length;
              int num1 = chunk.Offset + length - 1;
              int num2 = other.Index + other.Length - 1;
              if (num1 > num2)
              {
                resultList.Add(other);
                source1.RemoveFirst();
              }
              else
              {
                source1.RemoveFirst();
                if (chunk.Offset > other.Index)
                  resultList.Add(new LinkDetector.Result(other)
                  {
                    Length = chunk.Offset - other.Index
                  });
                LinkDetector.Result result = new LinkDetector.Result(chunk.Offset, length, other.type | 256, (string) null)
                {
                  AuxiliaryInfo = chunk.AuxiliaryInfo
                };
                resultList.Add(result);
                source2.RemoveFirst();
                if (num1 < num2)
                  source1.AddFirst(new LinkDetector.Result(other)
                  {
                    Index = num1 + 1,
                    Length = num2 - num1
                  });
              }
            }
          }
        }
        if (source1.Any<LinkDetector.Result>())
        {
          foreach (LinkDetector.Result result in source1)
            resultList.Add(result);
        }
        richTextFormattings = resultList.ToArray();
      }
      if (!replaceNewLines)
        this.cachedFormattings = richTextFormattings;
      return richTextFormattings;
    }

    public string MediaUploadUrl { get; set; }

    public bool ForceNotNoteworthy { get; set; }

    public string ModifyTag { get; set; }

    public Message()
    {
    }

    public Message(bool setTimestamp)
    {
      if (!setTimestamp)
        return;
      this.FunTimestamp = new DateTime?(FunRunner.CurrentServerTimeUtc);
    }

    public Message(FunXMPP.FMessage fMessage)
    {
      this.PushName = fMessage.push_name;
      this.KeyRemoteJid = fMessage.key.remote_jid;
      this.KeyFromMe = fMessage.key.from_me;
      this.KeyId = fMessage.key.id;
      this.Status = fMessage.status;
      this.RemoteResource = fMessage.remote_resource;
      this.WantsReceipt = fMessage.wants_receipt;
      this.Data = fMessage.data;
      this.FunTimestamp = fMessage.timestamp;
      this.MediaUrl = fMessage.media_url;
      this.MediaMimeType = fMessage.media_mime_type;
      this.MediaWaType = fMessage.media_wa_type;
      this.MediaSize = fMessage.media_size;
      this.MediaDurationSeconds = fMessage.media_duration_seconds;
      this.MediaName = fMessage.media_name;
      this.MediaOrigin = fMessage.media_origin;
      this.MediaCaption = fMessage.media_caption;
      this.MediaHash = fMessage.media_hash;
      this.MediaKey = fMessage.media_key;
      this.MediaIp = fMessage.media_ip;
      this.Latitude = fMessage.latitude;
      this.Longitude = fMessage.longitude;
      this.LocationDetails = fMessage.details;
      this.LocationUrl = fMessage.location_url;
      this.ProtoBuf = fMessage.proto_buf;
      if (fMessage.message_properties != null)
        fMessage.message_properties.SetForMessage(this);
      if (!string.IsNullOrEmpty(fMessage.notify_mute))
      {
        switch (fMessage.notify_mute)
        {
          case "none":
            MessageProperties forMessage1 = MessageProperties.GetForMessage(this);
            forMessage1.EnsureCommonProperties.NotifyLevel = new MessageProperties.CommonProperties.NotifyLevels?(MessageProperties.CommonProperties.NotifyLevels.ExcludeAlertAndBadge);
            forMessage1.Save();
            break;
          case "ambient":
            MessageProperties forMessage2 = MessageProperties.GetForMessage(this);
            forMessage2.EnsureCommonProperties.NotifyLevel = new MessageProperties.CommonProperties.NotifyLevels?(MessageProperties.CommonProperties.NotifyLevels.ExcludeAlert);
            this.IsAutomuted = true;
            forMessage2.Save();
            break;
        }
      }
      if (fMessage.binary_data != null && fMessage.binary_data.Length != 0)
        this.BinaryData = fMessage.binary_data;
      if (fMessage.multicast)
      {
        MessageProperties forMessage3 = MessageProperties.GetForMessage(this);
        forMessage3.EnsureCommonProperties.Multicast = new bool?(true);
        forMessage3.Save();
      }
      if (fMessage.urlPhoneNumber)
      {
        MessageProperties forMessage4 = MessageProperties.GetForMessage(this);
        forMessage4.EnsureCommonProperties.UrlNumber = new bool?(true);
        forMessage4.Save();
      }
      if (fMessage.urlText)
      {
        MessageProperties forMessage5 = MessageProperties.GetForMessage(this);
        forMessage5.EnsureCommonProperties.UrlText = new bool?(true);
        forMessage5.Save();
      }
      if (!fMessage.verified_name.HasValue && fMessage.verified_level == null && fMessage.verified_name_certificate == null)
        return;
      MessageProperties forMessage6 = MessageProperties.GetForMessage(this);
      MessageProperties.BizProperties ensureBizProperties = forMessage6.EnsureBizProperties;
      ensureBizProperties.Cert = fMessage.verified_name_certificate;
      ensureBizProperties.Level = fMessage.verified_level;
      ensureBizProperties.Serial = fMessage.verified_name;
      forMessage6.Save();
    }

    public static Message CreateSystemMessage(
      SqliteMessagesContext db,
      string remoteJid,
      string participantJid,
      byte[] binaryData,
      DateTime? funTimeUtc = null)
    {
      return new Message(false)
      {
        KeyRemoteJid = remoteJid,
        keyFromMe = false,
        KeyId = FunXMPP.GenerateMessageId(),
        RemoteResource = participantJid,
        MediaWaType = FunXMPP.FMessage.Type.System,
        Status = FunXMPP.FMessage.Status.NeverSend,
        BinaryData = binaryData,
        FunTimestamp = new DateTime?(funTimeUtc ?? FunRunner.CurrentServerTimeUtc)
      };
    }

    public enum MessageFlags
    {
      None,
      Deleted,
    }

    public class PlaceDetails
    {
      public string Name;
      public string Address;
      public string Url;
    }
  }
}
