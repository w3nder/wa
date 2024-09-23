// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.ChatItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class ChatItemViewModel : JidItemViewModel
  {
    private string searchText;
    private Pair<int, int>[] searchOffsets;
    private IDisposable lastMsgStatusSub;
    private string presenceStr;
    private RichTextBlock.TextSet subtitleCache;
    private IEnumerable<WaRichText.Chunk> titleStrHighlights;
    private string senderStr_;
    private static int readWeight = FunXMPP.FMessage.Status.ReadByTarget.GetOverrideWeight();
    private static int deliveredWeight = FunXMPP.FMessage.Status.ReceivedByTarget.GetOverrideWeight();
    private static int sentWeight = FunXMPP.FMessage.Status.ReceivedByServer.GetOverrideWeight();

    public Conversation Conversation { get; private set; }

    public Message LastMessage { get; private set; }

    public override object Model => (object) this.Conversation;

    public override string Key => this.Jid;

    public override string Jid => this.Conversation.Jid;

    private void UpdateTitleStrHighlights(string title)
    {
      if ((this.searchText == null || this.searchText.Length == 0) && (this.searchOffsets == null || this.searchOffsets.Length == 0) || title == null || title.Length == 0)
        this.titleStrHighlights = (IEnumerable<WaRichText.Chunk>) null;
      else if (this.searchOffsets != null)
        this.titleStrHighlights = (IEnumerable<WaRichText.Chunk>) ((IEnumerable<Pair<int, int>>) this.searchOffsets).Select<Pair<int, int>, WaRichText.Chunk>((Func<Pair<int, int>, WaRichText.Chunk>) (p => new WaRichText.Chunk(p.First, p.Second, WaRichText.Formats.Foreground, UIUtils.AccentColorCode))).ToArray<WaRichText.Chunk>();
      else if (this.searchText != null)
      {
        int offset = title.IndexOf(this.searchText, StringComparison.CurrentCultureIgnoreCase);
        if (offset < 0 || offset + this.searchText.Length - 1 >= title.Length)
          return;
        this.titleStrHighlights = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
        {
          new WaRichText.Chunk(offset, this.searchText.Length, WaRichText.Formats.Foreground, UIUtils.AccentColorCode)
        };
      }
      else
        this.titleStrHighlights = (IEnumerable<WaRichText.Chunk>) null;
    }

    public override Brush SubtitleBrush
    {
      get => !this.ShouldHighlight ? UIUtils.SubtleBrush : (Brush) UIUtils.AccentBrush;
    }

    public override FontWeight SubtitleWeight
    {
      get => this.presenceStr == null ? FontWeights.Normal : FontWeights.SemiBold;
    }

    public virtual string TimestampStr
    {
      get
      {
        if (this.Conversation == null || !this.EnableChatPreview)
          return (string) null;
        DateTime? nullable = this.Conversation.LocalTimestamp ?? (this.LastMessage == null ? new DateTime?() : this.LastMessage.LocalTimestamp);
        return !nullable.HasValue ? (string) null : DateTimeUtils.FormatCompact(nullable.Value, DateTimeUtils.TimeDisplay.SameDayOnly);
      }
    }

    public virtual bool ShowTimestamp => this.EnableChatPreview;

    public virtual bool ShowSender
    {
      get
      {
        return this.Conversation != null && this.EnableChatPreview && this.presenceStr == null && this.LastMessage != null && !this.LastMessage.KeyFromMe && this.LastMessage.MediaWaType != FunXMPP.FMessage.Type.System && this.Conversation.IsGroup();
      }
    }

    public string SenderStr
    {
      get
      {
        if (!this.ShowSender)
          return this.senderStr_ = (string) null;
        if (this.senderStr_ == null && this.LastMessage != null)
        {
          string senderJid = this.LastMessage.GetSenderJid();
          if (JidHelper.IsPsaJid(senderJid))
          {
            this.senderStr_ = Constants.OffcialName;
          }
          else
          {
            UserStatus userStatus = UserCache.Get(senderJid, true);
            if (userStatus != null)
              this.senderStr_ = string.Format("{0}: ", (object) userStatus.GetDisplayName(true));
          }
        }
        return this.senderStr_;
      }
    }

    public virtual bool ShowStatusIcon
    {
      get
      {
        return this.EnableChatPreview && this.presenceStr == null && this.LastMessage != null && (this.LastMessage.Status == FunXMPP.FMessage.Status.Unsent || this.LastMessage.MediaWaType != FunXMPP.FMessage.Type.Revoked) && this.LastMessage.KeyFromMe;
      }
    }

    public System.Windows.Media.ImageSource StatusIconSource => this.GetStatusIconSource();

    public virtual bool ShowMuteIcon => this.Conversation != null && this.Conversation.IsMuted();

    public System.Windows.Media.ImageSource MuteIconSource => this.GetMuteIconSource();

    public bool ShowPinIcon => this.Conversation != null && this.Conversation.IsPinned();

    public System.Windows.Media.ImageSource PinIconSource => this.GetPinIconSource();

    public virtual bool ShowMediaIcon
    {
      get
      {
        return this.EnableChatPreview && this.presenceStr == null && this.LastMessage != null && this.LastMessage.MediaWaType != FunXMPP.FMessage.Type.System && this.LastMessage.MediaWaType != FunXMPP.FMessage.Type.Undefined && this.LastMessage.MediaWaType != FunXMPP.FMessage.Type.ExtendedText && this.LastMessage.IsViewingSupported();
      }
    }

    public System.Windows.Media.ImageSource MediaIconSource
    {
      get
      {
        if (this.LastMessage == null)
          return (System.Windows.Media.ImageSource) null;
        switch (this.LastMessage.MediaWaType)
        {
          case FunXMPP.FMessage.Type.Image:
            return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlinePictureSubtle : (System.Windows.Media.ImageSource) AssetStore.InlinePictureAccent;
          case FunXMPP.FMessage.Type.Audio:
            if (this.LastMessage.IsPtt())
            {
              if (this.LastMessage.IsPlayedByTarget())
                return (System.Windows.Media.ImageSource) AssetStore.InlineMicBlue;
              return !this.LastMessage.KeyFromMe ? (System.Windows.Media.ImageSource) AssetStore.InlineMicGreen : (System.Windows.Media.ImageSource) AssetStore.InlineMicSubtle;
            }
            return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlineAudioSubtle : (System.Windows.Media.ImageSource) AssetStore.InlineAudioAccent;
          case FunXMPP.FMessage.Type.Video:
            return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlineVideoSubtle : (System.Windows.Media.ImageSource) AssetStore.InlineVideoAccent;
          case FunXMPP.FMessage.Type.Contact:
            return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlineContactSubtle : (System.Windows.Media.ImageSource) AssetStore.InlineContactAccent;
          case FunXMPP.FMessage.Type.Location:
            return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlineLocationSubtle : (System.Windows.Media.ImageSource) AssetStore.InlineLocationAccent;
          case FunXMPP.FMessage.Type.Document:
            return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlineDocSubtle : (System.Windows.Media.ImageSource) AssetStore.InlineDocAccent;
          case FunXMPP.FMessage.Type.Gif:
            return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlineGifSubtle : (System.Windows.Media.ImageSource) AssetStore.InlineGifAccent;
          case FunXMPP.FMessage.Type.LiveLocation:
            return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlineLiveLocationSubtle : (System.Windows.Media.ImageSource) AssetStore.InlineLiveLocationAccent;
          case FunXMPP.FMessage.Type.Sticker:
            return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlineStickerSubtle : (System.Windows.Media.ImageSource) AssetStore.InlineStickerAccent;
          case FunXMPP.FMessage.Type.Revoked:
            return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlineRevokeSubtle : (System.Windows.Media.ImageSource) AssetStore.InlineRevokeAccent;
          default:
            return (System.Windows.Media.ImageSource) null;
        }
      }
    }

    public virtual bool ShowUnreadCount
    {
      get
      {
        return this.Conversation != null && this.EnableChatPreview && this.Conversation.GetUnreadMessagesCount() > 0;
      }
    }

    public virtual bool ShouldHighlight
    {
      get
      {
        if (!this.EnableChatPreview)
          return false;
        if (this.presenceStr != null)
          return true;
        return this.Conversation != null && !this.Conversation.IsRead();
      }
    }

    public string UnreadCountStr
    {
      get
      {
        if (this.Conversation == null)
          return (string) null;
        int unreadMessagesCount = this.Conversation.GetUnreadMessagesCount();
        return unreadMessagesCount <= 0 ? (string) null : unreadMessagesCount.ToString();
      }
    }

    public virtual bool ShowLabel
    {
      get
      {
        Conversation conversation = this.Conversation;
        return conversation != null && conversation.IsArchived;
      }
    }

    public string LabelStr => !this.ShowLabel ? (string) null : AppResources.Archived;

    public override double PictureSize
    {
      get
      {
        return this.Conversation == null || !this.Conversation.IsBroadcast() ? base.PictureSize : 48.0;
      }
    }

    public override Brush PictureBackgroundBrush
    {
      get
      {
        return this.Conversation == null || !this.Conversation.IsBroadcast() ? base.PictureBackgroundBrush : (Brush) UIUtils.AccentBrush;
      }
    }

    public override bool IsDimmed => this.ShouldDisable();

    public override bool ShowVerifiedIcon
    {
      get
      {
        if (!this.showVerifiedIcon.HasValue && this.Conversation.IsPsaChat())
          this.showVerifiedIcon = new bool?(true);
        return this.showVerifiedIcon ?? (this.showVerifiedIcon = new bool?(false)).Value;
      }
    }

    public bool EnableChatPreview { get; set; }

    public bool EnableRecipientCheck { get; set; }

    public ChatItemViewModel(Conversation convo) => this.Conversation = convo;

    public ChatItemViewModel(Conversation convo, Message lastMsg)
      : this(convo)
    {
      this.LastMessage = lastMsg;
    }

    public override void Refresh()
    {
      this.subtitleCache = (RichTextBlock.TextSet) null;
      base.Refresh();
    }

    public virtual bool ShouldDisable()
    {
      return this.EnableRecipientCheck && this.Conversation != null && this.Conversation.IsGroup() && this.Conversation.IsReadOnly();
    }

    public void CopySearchItemsFrom(ChatItemViewModel other)
    {
      this.searchOffsets = other.searchOffsets;
      this.searchText = other.searchText;
      this.Notify("Title");
    }

    public void SetSearchText(string value)
    {
      if (this.searchOffsets == null && !(this.searchText != value))
        return;
      this.searchOffsets = (Pair<int, int>[]) null;
      this.searchText = value;
      this.Notify("Title");
    }

    public void SetSearchOffsets(Pair<int, int>[] offsets)
    {
      if (this.searchText == null && this.searchOffsets == null && offsets == null)
        return;
      this.searchText = (string) null;
      this.searchOffsets = offsets;
      this.Notify("Title");
    }

    public void SetMessage(Message msg, bool notify = true)
    {
      this.senderStr_ = (string) null;
      this.subtitleCache = (RichTextBlock.TextSet) null;
      this.LastMessage = msg;
      this.lastMsgStatusSub.SafeDispose();
      this.lastMsgStatusSub = (IDisposable) null;
      if (this.EnableChatPreview && msg != null && !msg.IsMaxStatusReached())
        this.lastMsgStatusSub = msg.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (args => args.PropertyName == "Status" || args.PropertyName == "MediaWaType")).ObserveOnDispatcher<PropertyChangedEventArgs>().Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (args =>
        {
          switch (args.PropertyName)
          {
            case "Status":
              this.Notify("StatusIcon");
              break;
            case "MediaWaType":
              this.Refresh();
              this.Notify("MediaWaType");
              break;
          }
        }));
      if (!(this.EnableChatPreview & notify))
        return;
      this.Refresh();
    }

    private System.Windows.Media.ImageSource GetStatusIconSource()
    {
      if (this.LastMessage == null || !this.LastMessage.KeyFromMe)
        return (System.Windows.Media.ImageSource) null;
      int overrideWeight = this.LastMessage.Status.GetOverrideWeight();
      return overrideWeight < ChatItemViewModel.readWeight ? (overrideWeight < ChatItemViewModel.deliveredWeight ? (overrideWeight < ChatItemViewModel.sentWeight ? (this.LastMessage.Status != FunXMPP.FMessage.Status.Error ? (System.Windows.Media.ImageSource) AssetStore.InlineClock : (System.Windows.Media.ImageSource) AssetStore.InlineError) : (System.Windows.Media.ImageSource) AssetStore.InlineCheck) : (System.Windows.Media.ImageSource) AssetStore.InlineDoubleChecks) : (System.Windows.Media.ImageSource) AssetStore.InlineBlueChecks;
    }

    private System.Windows.Media.ImageSource GetMuteIconSource()
    {
      if (!this.ShowMuteIcon)
        return (System.Windows.Media.ImageSource) null;
      return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlineMuteSubtle : (System.Windows.Media.ImageSource) AssetStore.InlineMuteAccent;
    }

    private System.Windows.Media.ImageSource GetPinIconSource()
    {
      if (!this.ShowPinIcon)
        return (System.Windows.Media.ImageSource) null;
      return !this.ShouldHighlight ? (System.Windows.Media.ImageSource) AssetStore.InlinePinSubtle : (System.Windows.Media.ImageSource) AssetStore.InlinePinAccent;
    }

    public bool ShouldEnableRichTextSubtitle()
    {
      return this.LastMessage != null && this.LastMessage.HasText();
    }

    public override RichTextBlock.TextSet GetSubtitle()
    {
      if (this.Conversation == null)
        return (RichTextBlock.TextSet) null;
      RichTextBlock.TextSet subtitle = new RichTextBlock.TextSet();
      if (this.EnableRecipientCheck && this.Conversation.IsGroup() && !this.Conversation.IsGroupParticipant())
        subtitle.Text = AppResources.NotGroupParticipantChatItemText;
      else if (this.EnableChatPreview)
      {
        if (this.presenceStr != null)
          subtitle.Text = this.presenceStr;
        else if (this.subtitleCache != null)
          subtitle = this.subtitleCache;
        else if (this.LastMessage != null)
        {
          LinkDetector.Result[] formats = (LinkDetector.Result[]) null;
          string previewText = this.LastMessage.GetPreviewText(out formats, true, false);
          subtitle.Text = Emoji.ConvertToUnicode(previewText);
          subtitle.SerializedFormatting = (IEnumerable<LinkDetector.Result>) formats;
        }
        else
          subtitle = (RichTextBlock.TextSet) null;
      }
      else if (this.subtitleCache != null)
      {
        subtitle = this.subtitleCache;
      }
      else
      {
        string str = (string) null;
        if (this.Conversation.IsUserChat())
        {
          UserStatus userStatus = UserCache.Get(this.Conversation.Jid, false);
          if (userStatus != null)
            str = userStatus.Status;
        }
        else
          str = this.Conversation.GetParticipantNames(12);
        subtitle.Text = str;
      }
      return subtitle;
    }

    protected override IEnumerable<MenuItem> GetMenuItemsImpl()
    {
      return this.Conversation != null ? new ConversationToContextMenuConverter().GetMenuItems(this.Conversation) : base.GetMenuItemsImpl();
    }

    public override bool GetCachedPicSource(out System.Windows.Media.ImageSource cached)
    {
      return this.Conversation != null ? ChatPictureStore.GetCache(this.Conversation.Jid, out cached) : base.GetCachedPicSource(out cached);
    }

    protected override IObservable<System.Windows.Media.ImageSource> GetPictureSourceObservableImpl(
      bool getCurrent,
      bool trackChange)
    {
      if (this.Conversation == null)
        return base.GetPictureSourceObservableImpl(getCurrent, trackChange);
      IObservable<System.Windows.Media.ImageSource> sourceObservableImpl;
      switch (JidHelper.GetJidType(this.Conversation.Jid))
      {
        case JidHelper.JidTypes.Broadcast:
          sourceObservableImpl = Observable.Return<System.Windows.Media.ImageSource>((System.Windows.Media.ImageSource) AssetStore.Broadcast);
          break;
        case JidHelper.JidTypes.Psa:
          sourceObservableImpl = Observable.Return<System.Windows.Media.ImageSource>((System.Windows.Media.ImageSource) AssetStore.WhatsAppAvatar);
          break;
        default:
          sourceObservableImpl = base.GetPictureSourceObservableImpl(getCurrent, trackChange);
          break;
      }
      return sourceObservableImpl;
    }

    public override string GetTitle()
    {
      Conversation conversation = this.Conversation;
      return (conversation != null ? conversation.GetName() : (string) null) ?? "";
    }

    public override RichTextBlock.TextSet GetRichTitle()
    {
      string title = this.GetTitle();
      this.UpdateTitleStrHighlights(title);
      RichTextBlock.TextSet richTitle = new RichTextBlock.TextSet();
      richTitle.Text = title;
      Conversation conversation = this.Conversation;
      richTitle.SerializedFormatting = conversation != null ? conversation.GetGroupSubjectPerformanceHint() : (IEnumerable<LinkDetector.Result>) null;
      richTitle.PartialFormattings = this.titleStrHighlights;
      return richTitle;
    }

    public override IObservable<RichTextBlock.TextSet> GetRichTitleObservableImpl()
    {
      return this.Conversation != null && this.Conversation.IsGroup() ? Observable.Return<RichTextBlock.TextSet>(this.GetRichTitle()) : base.GetRichTitleObservableImpl().SubscribeOn<RichTextBlock.TextSet>((IScheduler) AppState.ImageWorker);
    }

    public override IDisposable ActivateLazySubscriptions()
    {
      IDisposable disposable = (IDisposable) null;
      if (this.Conversation != null && this.EnableChatPreview)
      {
        bool isGroup = JidHelper.IsGroupJid(this.Conversation.Jid);
        disposable = PresenceState.Instance.GetPresence(this.Conversation.Jid, false).ObserveOnDispatcher<PresenceEventArgs>().Subscribe<PresenceEventArgs>((Action<PresenceEventArgs>) (args =>
        {
          string str = (string) null;
          if (args != null && args.FormattedString != null)
          {
            if (isGroup)
              str = args.FormattedString.PresenceString;
            else if (args.State == Presence.OnlineAndRecording || args.State == Presence.OnlineAndTyping)
              str = args.FormattedString.PresenceString;
          }
          if (!(this.presenceStr != str))
            return;
          this.Notify("Presence", (object) (this.presenceStr = str));
        }));
      }
      return (IDisposable) new DisposableChain(new IDisposable[3]
      {
        disposable,
        base.ActivateLazySubscriptions(),
        (IDisposable) new DisposableAction((Action) (() => this.presenceStr = (string) null))
      });
    }

    protected override void DisposeManagedResources()
    {
      this.lastMsgStatusSub.SafeDispose();
      this.lastMsgStatusSub = (IDisposable) null;
      base.DisposeManagedResources();
    }
  }
}
