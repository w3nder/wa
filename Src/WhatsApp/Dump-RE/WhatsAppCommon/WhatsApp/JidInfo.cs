// Decompiled with JetBrains decompiler
// Type: WhatsApp.JidInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using Microsoft.Phone.Reactive;
using System;
using System.Data.Linq.Mapping;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "Jid", IsUnique = true)]
  public class JidInfo : PropChangingChangedBase
  {
    public static Subject<JidInfo> JidInfoUpdatedSubject = new Subject<JidInfo>();
    private string jid;
    private DateTime? muteExpUtc;
    private string notificationSound;
    private string ringTone;
    private bool? isSuspicious;
    private bool? promptedVCards;
    private JidInfo.FullEncryptionState supportsFullEncryption = JidInfo.FullEncryptionState.SupportedAndNotified;
    private bool? saveMediaToPhone;
    private string wallpaper;
    private bool isStatusMuted;
    private int statusAutoDownloadQuota;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int ID { get; set; }

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
    public DateTime? MuteExpirationUtc
    {
      get => this.muteExpUtc;
      set
      {
        DateTime? muteExpUtc = this.muteExpUtc;
        DateTime? nullable = value;
        if ((muteExpUtc.HasValue == nullable.HasValue ? (muteExpUtc.HasValue ? (muteExpUtc.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (MuteExpirationUtc));
        this.muteExpUtc = value;
        this.NotifyPropertyChanged(nameof (MuteExpirationUtc));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string NotificationSound
    {
      get => this.notificationSound;
      set
      {
        if (!(this.notificationSound != value))
          return;
        this.NotifyPropertyChanging(nameof (NotificationSound));
        this.notificationSound = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string RingTone
    {
      get => this.ringTone;
      set
      {
        if (!(this.ringTone != value))
          return;
        this.NotifyPropertyChanging(nameof (RingTone));
        this.ringTone = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool? IsSuspicious
    {
      get => this.isSuspicious;
      set
      {
        bool? isSuspicious = this.isSuspicious;
        bool? nullable = value;
        if ((isSuspicious.GetValueOrDefault() == nullable.GetValueOrDefault() ? (isSuspicious.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (IsSuspicious));
        this.isSuspicious = value;
        this.NotifyPropertyChanged(nameof (IsSuspicious));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool? PromptedVCards
    {
      get => this.promptedVCards;
      set
      {
        bool? promptedVcards = this.promptedVCards;
        bool? nullable = value;
        if ((promptedVcards.GetValueOrDefault() == nullable.GetValueOrDefault() ? (promptedVcards.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (PromptedVCards));
        this.promptedVCards = new bool?(true);
        this.NotifyPropertyChanged(nameof (PromptedVCards));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public JidInfo.FullEncryptionState SupportsFullEncryption
    {
      get => this.supportsFullEncryption;
      set
      {
        if (this.supportsFullEncryption == value)
          return;
        this.NotifyPropertyChanging(nameof (SupportsFullEncryption));
        this.supportsFullEncryption = value;
        this.NotifyPropertyChanged(nameof (SupportsFullEncryption));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool? SaveMediaToPhone
    {
      get => this.saveMediaToPhone;
      set
      {
        bool? saveMediaToPhone = this.saveMediaToPhone;
        bool? nullable = value;
        if ((saveMediaToPhone.GetValueOrDefault() == nullable.GetValueOrDefault() ? (saveMediaToPhone.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (SaveMediaToPhone));
        this.saveMediaToPhone = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Wallpaper
    {
      get => this.wallpaper;
      set
      {
        if (!(this.wallpaper != value))
          return;
        this.NotifyPropertyChanging(nameof (Wallpaper));
        this.wallpaper = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool IsStatusMuted
    {
      get => this.isStatusMuted;
      set
      {
        if (this.isStatusMuted == value)
          return;
        this.NotifyPropertyChanging(nameof (IsStatusMuted));
        this.isStatusMuted = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int StatusAutoDownloadQuota
    {
      get => this.statusAutoDownloadQuota;
      set
      {
        if (this.statusAutoDownloadQuota == value)
          return;
        this.NotifyPropertyChanging(nameof (StatusAutoDownloadQuota));
        this.statusAutoDownloadQuota = value;
      }
    }

    public JidInfo()
    {
    }

    public JidInfo(string jid)
    {
      this.jid = jid;
      this.SupportsFullEncryption = JidInfo.FullEncryptionState.Supported;
    }

    public bool IsMuted()
    {
      return this.MuteExpirationUtc.HasValue && this.MuteExpirationUtc.Value > FunRunner.CurrentServerTimeUtc;
    }

    public bool ShouldPromptVCard() => !this.PromptedVCards.HasValue || !this.PromptedVCards.Value;

    public void OnChatDeleted()
    {
    }

    public enum FullEncryptionState
    {
      Unknown,
      NotSupported,
      Supported,
      SupportedAndNotified,
      SupportedAndNeverNotify,
    }
  }
}
