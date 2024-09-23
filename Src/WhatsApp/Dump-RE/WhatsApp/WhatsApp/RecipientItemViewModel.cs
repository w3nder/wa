// Decompiled with JetBrains decompiler
// Type: WhatsApp.RecipientItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class RecipientItemViewModel : ChatItemViewModel
  {
    protected RecipientListPage.RecipientItemType itemType;
    private bool isReadOnly;
    private RichTextBlock.TextSet subtitleText;

    public UserStatus User { get; private set; }

    public string SubtitleOverride { get; set; }

    public override string Jid
    {
      get
      {
        string jid = this.Conversation?.Jid;
        if (jid != null)
          return jid;
        return this.User?.Jid;
      }
    }

    public RecipientListPage.RecipientItemType Type => this.itemType;

    public override bool ShowTimestamp => false;

    public override bool ShouldHighlight => false;

    public string TitleStrLower { get; private set; }

    public override bool ShowSender
    {
      get => this.itemType == RecipientListPage.RecipientItemType.Recent && base.ShowSender;
    }

    public override bool ShowStatusIcon
    {
      get => this.itemType == RecipientListPage.RecipientItemType.Recent && base.ShowStatusIcon;
    }

    public override bool ShowMediaIcon
    {
      get => this.itemType == RecipientListPage.RecipientItemType.Recent && base.ShowMediaIcon;
    }

    public override bool ShowMuteIcon
    {
      get => this.itemType == RecipientListPage.RecipientItemType.Recent && base.ShowMuteIcon;
    }

    public virtual bool ShowSubtitleRow
    {
      get
      {
        return !JidHelper.IsUserJid(this.Jid) || this.itemType == RecipientListPage.RecipientItemType.Recent || !string.IsNullOrEmpty(this.SubtitleOverride);
      }
    }

    public override bool IsSelected
    {
      get => !this.ShouldDisable() && base.IsSelected;
      set
      {
        if (this.ShouldDisable())
          return;
        base.IsSelected = value;
      }
    }

    public bool IsReadOnly
    {
      get => this.isReadOnly;
      set
      {
        if (this.isReadOnly == value)
          return;
        this.isReadOnly = value;
      }
    }

    public RecipientItemViewModel(Conversation convo, RecipientListPage.RecipientItemType type)
      : base(convo)
    {
      this.itemType = type;
    }

    public RecipientItemViewModel(
      Conversation convo,
      Message lastMessage,
      RecipientListPage.RecipientItemType type)
      : base(convo, lastMessage)
    {
      this.itemType = type;
    }

    public RecipientItemViewModel(UserStatus user)
      : base((Conversation) null)
    {
      this.User = user;
      this.itemType = RecipientListPage.RecipientItemType.WaContact;
    }

    public void SetTitleStr(string s, bool notify)
    {
      this.richTitleCache = (RichTextBlock.TextSet) null;
      this.TitleStrLower = s.ToLower();
      if (!notify)
        return;
      this.Notify("Title");
    }

    public override string GetTitle() => this.User?.GetDisplayName() ?? base.GetTitle();

    public override IObservable<RichTextBlock.TextSet> GetRichTitleObservableImpl()
    {
      return this.itemType == RecipientListPage.RecipientItemType.Frequent ? Observable.Return<RichTextBlock.TextSet>(this.GetRichTitle()) : base.GetRichTitleObservableImpl();
    }

    public override RichTextBlock.TextSet GetSubtitle()
    {
      if (this.subtitleText == null)
      {
        RichTextBlock.TextSet textSet = new RichTextBlock.TextSet();
        switch (this.itemType)
        {
          case RecipientListPage.RecipientItemType.Group:
            textSet.Text = this.Conversation.IsGroupParticipant() ? (!this.Conversation.IsAnnounceOnlyForUser() ? this.Conversation.GetParticipantNames(10) : AppResources.AnnouncementOnlyGroupSendMessageNotAdmin) : AppResources.NotGroupParticipantChatItemText;
            break;
          case RecipientListPage.RecipientItemType.WaContact:
            textSet.Text = string.IsNullOrEmpty(this.SubtitleOverride) ? this.User?.Status : this.SubtitleOverride;
            break;
          default:
            textSet = base.GetSubtitle();
            break;
        }
        this.subtitleText = textSet;
      }
      return this.subtitleText;
    }

    protected override IObservable<System.Windows.Media.ImageSource> GetPictureSourceObservableImpl(
      bool getCurrent,
      bool trackChange)
    {
      return base.GetPictureSourceObservableImpl(true, false);
    }

    public override IDisposable ActivateLazySubscriptions() => (IDisposable) null;
  }
}
