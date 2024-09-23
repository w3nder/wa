// Decompiled with JetBrains decompiler
// Type: WhatsApp.Constants
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using WhatsApp.ProtoBuf;
using Windows.Storage;

#nullable disable
namespace WhatsApp
{
  public static class Constants
  {
    public const string PushChannel = "message";
    public const string PushService = "mpns.whatsapp.net";
    public const string BackupTaskName = "WhatsApp.PeriodicBackupAgent";
    public const int PeriodicTimeSecs = 25;
    public const string ResourceIntensiveTaskName = "WhatsApp.ResourceIntensiveAgent";
    public const int ResourceIntensiveTimeSecs = 600;
    public static string SwitchPhonesUrl = WaWebUrls.GetFaqUrlGeneral("26000006");
    public static string UpdateUrl = "https://www.whatsapp.com/windowsbeta";
    public const string DownloadUrl = "https://whatsapp.com/dl/";
    public const string DownloadUrlLong = "https://www.whatsapp.com/download/";
    public static char JidAgentSeparator = '.';
    public static char JidDeviceSeparator = ':';
    public static char JidDomainSeparator = '@';
    public static char JidGroupTimestampSeparator = '-';
    public static char JidMultiAgentUserSeparator = '-';
    public const string UserDomain = "s.whatsapp.net";
    public const string UserSuffix = "@s.whatsapp.net";
    public const string ChatIDToJidFormat = "{0}@s.whatsapp.net";
    public const string GroupDomain = "g.us";
    public const string GroupJidSuffix = "@g.us";
    public const string JidBroadcastDomain = "broadcast";
    public const string BroadcastJidSuffix = "@broadcast";
    public const string JidCallDomain = "call";
    public const string JidBusinessDomain = "business";
    public const string JidMultiAgentDomain = "b.us";
    public const string StatusV3Jid = "status@broadcast";
    public const string PsaJid = "0@s.whatsapp.net";
    public const string StatusDomain = "s.us";
    public const string StatusSuffix = "@s.us";
    public const uint FieldStatsSendingCap = 150000;
    public const int GroupedListThreshold = 30;
    public const int ImageSearchMinDimension = 300;
    public const int ImageSearchMaxSize = 512000;
    public const int MaxChatPicSize = -1;
    public const int MaxThumbnailSize = 48128;
    public const int MaxVCardPhotoBytes = 40960;
    public const int MaxVCardPhotoSize = 64;
    public const int MaxStatusLength = 139;
    public const int MaxTextLength = 65536;
    public const int MaxCaptionLength = 1024;
    public const int PttAutoDownloadSizeLimit = 524288;
    public const int VersionExpirationDays = 45;
    public const int EverstoreMaxBackoffDelay = 604800;
    public const int KeydMaxBackoffDelay = 610;
    public static TimeSpan TenHours = TimeSpan.FromHours(10.0);
    public static TimeSpan OneDay = TimeSpan.FromHours(24.0);
    public static TimeSpan TwoDays = TimeSpan.FromHours(48.0);
    public static TimeSpan SixDays = TimeSpan.FromDays(6.0);
    public static TimeSpan OneWeek = TimeSpan.FromDays(7.0);
    public static TimeSpan FourWeeks = TimeSpan.FromDays(28.0);
    public static TimeSpan ThirtyDays = TimeSpan.FromDays(30.0);
    public static TimeSpan ThirtyOneDays = TimeSpan.FromDays(31.0);
    public static TimeSpan HalfYear = TimeSpan.FromDays(180.0);
    public static TimeSpan OneYear = TimeSpan.FromDays(365.0);
    public const char Ellipsis = '…';
    public const double FontSizeMedium = 22.0;
    public const double FontSizeLarge = 28.0;
    public const int LocationThumbnailWidth = 680;
    public const int LocationThumbnailHeight = 320;
    public const int SmallLocationThumbnailSize = 300;
    public const int LocationThumbnailOverlayWidth = 239;
    public const int LocationThumbnailOverlayHeight = 111;
    public const double LargeImageThumbnailMaxWidthHeightRatio = 2.4;
    public const int VideoThumbnailSize = 120;
    public const int MaxVideoDuration = 60;
    public const int ProfilePhotoMinSize = 192;
    public const int ProfilePhotoTargetSize = 640;
    public const int ImageMinEdge = 100;
    public const int ItemThumbnailEdge = 98;
    public static Size ItemThumbnailSize = new Size(98.0, 98.0);
    public const int MinimumPttDuration = 1;
    public const int PttVibrateDuration = 75;
    public const int MaxPremptiveWebMessage = 1000;
    public const int LOG_DISCONNECT = 0;
    public const int LOG_WIFI = 1;
    public const int LOG_2G = 2;
    public const int LOG_3G = 3;
    public const int LOG_CONNECTION_UNKNOWN = 4;
    public const int CriticalStorageMinimum = 15;
    public const int StorageMinimum = 100;
    public const int GOOGLE = 1;
    public const int FOURSQUARE = 2;
    public const int FACEBOOK = 3;
    public const int APPLE = 4;
    public const int EmojiUsageIncrement = 10000;
    public const int StickerUsageIncrement = 10000;
    public const double EmojiUsageDecay = 0.9;
    public const double StickerUsageDecay = 0.9;
    public const int AppBarShortSide = 72;
    public static Dictionary<uint, double> TextSizes = new Dictionary<uint, double>()
    {
      {
        100U,
        23.0
      },
      {
        112U,
        24.0
      },
      {
        125U,
        25.0
      },
      {
        137U,
        29.0
      },
      {
        150U,
        33.0
      },
      {
        162U,
        35.0
      },
      {
        175U,
        38.0
      },
      {
        187U,
        40.0
      },
      {
        200U,
        42.0
      }
    };
    public const char NonBreakSpace = ' ';
    public const char ZeroWidthSpace = '\u200B';
    public const bool ForceAudioAgent = false;
    public const long Lifetime = 4444444444;
    public const FileShare DefaultFileShare = FileShare.ReadWrite | FileShare.Delete;
    public const string SecurityInfoUrl = "https://www.whatsapp.com/security/";
    public const string MSFTUpgradePhoneUrl = "https://support.microsoft.com/en-us/help/12662/windows-phone-update-your-windows-phone";
    public const string MicrosoftAccountProviderId = "https://login.microsoft.com";
    public const string MicrosoftAccountAuthority = "consumers";
    public const int MmsTruncationLastChunk = 65536;
    public static readonly Color SysTrayOffWhite = new Color()
    {
      A = byte.MaxValue,
      B = byte.MaxValue,
      G = byte.MaxValue,
      R = 254
    };
    public const int PushNameMaxLength = 25;
    public const long ReadReceiptTimestamp = 1415214000;
    public static string[] LocationUrlWhitelist = new string[3]
    {
      "www.facebook.com",
      "maps.google.com",
      "foursquare.com"
    };
    public static string FBCrashlogGateUrl = "https://crashlogs.whatsapp.net/wa_fls_upload_check";
    public static string FBCrashlogUrl = "https://crashlogs.whatsapp.net/wa_clb_data";
    public static string FBCrashlogAccessToken = NativeInterfaces.Misc.GetString(8);
    public static readonly TimeSpan RevokeExpiryTimeout = TimeSpan.FromSeconds(4096.0);
    public static readonly TimeSpan RevokeIncomingExpiryTimeout = TimeSpan.FromHours(12.0);
    public static readonly int LiveLocationDefaultSubscriptionDurationSeconds = 30;
    public static ClientPayload.UserAgent.ReleaseChannel ReleaseChannel = ClientPayload.UserAgent.ReleaseChannel.BETA;

    public static string OffcialName => AppResources.OfficialName;

    public static string OffcialNameUpper => AppResources.OfficialNameUPPER;

    public static string KillEventName => "WhatsApp.BgKill" + AppState.AppUniqueSuffix;

    public static string InCallEventName => "WhatsApp.InCall" + AppState.AppUniqueSuffix;

    public static string BgSockRequestEventName => "WhatsApp.BgSock" + AppState.AppUniqueSuffix;

    public static string ObscuredEventName => "WhatsApp.Obscured" + AppState.AppUniqueSuffix;

    public static string VoipEventName => "WhatsApp.VoipPush" + AppState.AppUniqueSuffix;

    public static TimeSpan LoginTimeout => TimeSpan.FromSeconds(30.0);

    public static TimeSpan ForegroundPingTimeout => TimeSpan.FromSeconds(100.0);

    public static TimeSpan BackgroundPingTimeout => TimeSpan.FromSeconds(320.0);

    public static string IsoStorePath => ApplicationData.Current.LocalFolder.Path;

    public static TimeSpan ChatPicturePullInterval => TimeSpan.FromDays(7.0);

    public static TimeSpan ServerPropPullInterval => TimeSpan.FromDays(1.0);

    public static TimeSpan TaskCompletionTimeout => TimeSpan.FromMinutes(10.0);

    public static TimeSpan PopTimeout => TimeSpan.FromMinutes(10.0);

    public static TimeSpan WifiPopTimeout => TimeSpan.FromMinutes(1.0);

    public static TimeSpan GetTimeBetweenPops(bool activeUser)
    {
      return TimeSpan.FromMinutes(activeUser ? 30.0 : 60.0);
    }

    public static TimeSpan VoipAgentTimeout => TimeSpan.FromSeconds(40.0);

    public static TimeSpan WatchdogTimeout
    {
      get => TimeSpan.FromSeconds(AppState.IsBackgroundAgent ? 1.0 : 5.0);
    }

    public static int MaxMessageRecipients { get; } = 5;

    public static class Bidi
    {
      public const char PushLtrMarker = '\u200E';
      public const char PushRtlMarker = '\u200F';
      public const char PushLtrEmbed = '\u202A';
      public const char PushRtlEmbed = '\u202B';
      public const char Pop = '\u202C';
    }

    public static class MediaOrigin
    {
      public const string Live = "live";
    }

    public static class Unicode
    {
      public const char VariationSelectorFirst = '︀';
      public const char VariationSelectorLast = '️';
      public const char VariationSelectorText = '︎';
      public const char VariationSelectorEmoji = '️';
      public const uint RegionalIndicatorFirst = 127462;
      public const uint RegionalIndicatorLast = 127487;
      public const char ZeroWidthJoiner = '\u200D';
    }

    public static class PrivacySettings
    {
      public const string LastSeen = "last";
      public const string ProfilePhoto = "profile";
      public const string Status = "status";
      public const string ReadReceipt = "readreceipts";
    }

    public static class MimeTypes
    {
      public const string Pdf = "application/pdf";
      public const string Docx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
      public const string Pptx = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
      public const string Xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
      public const string Doc = "application/msword";
      public const string Ppt = "application/vnd.ms-powerpoint";
      public const string Xls = "application/vnd.ms-excel";
      public const string Txt = "text/plain";
      public const string Webp = "image/webp";
      public const string Png = "image/png";
    }
  }
}
