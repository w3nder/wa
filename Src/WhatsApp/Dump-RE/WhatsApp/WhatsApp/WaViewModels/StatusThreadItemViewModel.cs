// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.StatusThreadItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class StatusThreadItemViewModel : StatusItemViewModel
  {
    private WaStatusThread statusThread;
    private UserStatus user;
    private string key;

    public WaStatusThread StatusThread
    {
      get => this.statusThread;
      set
      {
        if (this.statusThread == value)
          return;
        this.statusThread = value;
        this.Refresh();
      }
    }

    public override WaStatus Status
    {
      get => this.StatusThread?.LatestStatus;
      set
      {
      }
    }

    public UserStatus Sender => this.user;

    public override object Model => (object) this.StatusThread;

    public override string Key => this.key ?? (this.key = this.StatusThread?.Jid);

    public override string Jid => this.StatusThread?.Jid;

    public int Count
    {
      get
      {
        WaStatusThread statusThread = this.StatusThread;
        return statusThread == null ? 0 : statusThread.Count;
      }
    }

    public int ViewedCount
    {
      get
      {
        WaStatusThread statusThread = this.StatusThread;
        return statusThread == null ? 0 : statusThread.ViewedCount;
      }
    }

    public override bool ShowViewProgress => !this.IsMuted;

    public override bool IsDimmed => this.IsMuted;

    public bool IsMuted { get; set; }

    public override Brush PictureBackgroundBrush
    {
      get
      {
        int? msgId = this.Status?.MessageId;
        if (msgId.HasValue)
        {
          Message msg = (Message) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => msg = db.GetMessageById(msgId.Value)));
          uint? backgroundArgb = (uint?) msg?.InternalProperties?.ExtendedTextPropertiesField?.BackgroundArgb;
          if (backgroundArgb.HasValue)
            return (Brush) new SolidColorBrush(Color.FromArgb((byte) (backgroundArgb.Value >> 24 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value >> 16 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value >> 8 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value & (uint) byte.MaxValue)));
        }
        return base.PictureBackgroundBrush;
      }
    }

    public StatusThreadItemViewModel(WaStatusThread statusThread, UserStatus sender)
      : base((WaStatus) null)
    {
      this.statusThread = statusThread;
      this.user = sender;
      this.EnableContextMenu = true;
    }

    public override string GetTitle() => this.user?.GetDisplayName(skipAddressBookCheck: true);

    protected override IEnumerable<MenuItem> GetMenuItemsImpl()
    {
      bool isMuted = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        JidInfo jidInfo = db.GetJidInfo(this.Jid, CreateOptions.None);
        isMuted = jidInfo != null && jidInfo.IsStatusMuted;
      }));
      List<MenuItem> menuItemsImpl = new List<MenuItem>();
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Header = isMuted ? (object) AppResources.StatusUnmute : (object) AppResources.StatusMute;
      MenuItem menuItem2 = menuItem1;
      menuItem2.Click += new RoutedEventHandler(this.ToggleMute_Click);
      menuItemsImpl.Add(menuItem2);
      return (IEnumerable<MenuItem>) menuItemsImpl;
    }

    private void ToggleMute_Click(object sender, EventArgs e)
    {
      bool isCurrentlyMuted = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        JidInfo jidInfo = db.GetJidInfo(this.Jid, CreateOptions.None);
        isCurrentlyMuted = jidInfo != null && jidInfo.IsStatusMuted;
      }));
      UserStatus user = UserCache.Get(this.Jid, true);
      string contactName = user.GetDisplayName(true);
      string title = string.Format(isCurrentlyMuted ? AppResources.StatusUnmuteConfirmTitle : AppResources.StatusMuteConfirmTitle, (object) contactName);
      UIUtils.Decision(string.Format(isCurrentlyMuted ? AppResources.StatusUnmuteConfirmBody : AppResources.StatusMuteConfirmBody, (object) contactName), isCurrentlyMuted ? AppResources.StatusUnmute : AppResources.StatusMute, AppResources.Cancel, title).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (!confirmed)
          return;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          JidInfo jidInfo = db.GetJidInfo(this.Jid, CreateOptions.CreateToDbIfNotFound);
          jidInfo.IsStatusMuted = !isCurrentlyMuted;
          db.SubmitChanges();
          Settings.UpdateContactsChecksum();
          AppState.QrPersistentAction.NotifyContactChange(new FunXMPP.ContactResponse()
          {
            Jid = user.Jid,
            DisplayName = contactName,
            ShortName = user.FirstName,
            VName = user.GetVerifiedNameForDisplay(),
            Checksum = Settings.ContactsChecksum,
            StatusMute = new bool?(jidInfo.IsStatusMuted),
            Notify = user.PushName,
            Verify = user.VerificationLevelForWeb(),
            IsEnterprise = new bool?(user.IsEnterprise()),
            Checkmark = new bool?(this.ShowVerifiedIcon)
          });
          JidInfo.JidInfoUpdatedSubject.OnNext(jidInfo);
        }));
      }));
    }
  }
}
