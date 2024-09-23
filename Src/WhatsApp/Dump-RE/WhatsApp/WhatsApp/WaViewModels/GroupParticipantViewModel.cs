// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.GroupParticipantViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Windows;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class GroupParticipantViewModel : UserViewModel
  {
    private bool? isGroupAdmin;
    private bool? isSuperAdmin;
    private Conversation groupChat;
    private GlobalProgressIndicator progressIndicator;

    public int Error { get; set; }

    public override bool ShowSubtitle
    {
      get
      {
        return this.Error > 0 || !string.IsNullOrEmpty(this.User?.Status) || this.ShowAccentRightText || this.ShowSubtleRightText;
      }
    }

    public override RichTextBlock.TextSet GetSubtitle()
    {
      RichTextBlock.TextSet subtitle = base.GetSubtitle();
      if (this.Error > 0)
      {
        if (subtitle == null)
          subtitle = new RichTextBlock.TextSet();
        subtitle.Text = AppResources.GroupAddParticipantFailSubtitle;
        subtitle.PartialFormattings = (IEnumerable<WaRichText.Chunk>) null;
      }
      return subtitle;
    }

    public override bool ShowAccentRightText => this.IsGroupAdmin;

    public override string AccentRightText => !this.IsGroupAdmin ? "" : AppResources.GroupAdmin;

    public override bool ShowSubtleRightText
    {
      get
      {
        UserStatus user = this.User;
        return (user != null ? (user.IsInDeviceContactList ? 1 : 0) : 0) == 0;
      }
    }

    public override string SubtleRightText
    {
      get
      {
        return this.User != null && !string.IsNullOrEmpty(this.User.PushName) ? string.Format("~{0}", (object) Emoji.ConvertToTextOnly(this.User.PushName, (byte[]) null)) : "";
      }
    }

    public bool IsGroupAdmin
    {
      get
      {
        return this.User != null && this.groupChat != null && (this.isGroupAdmin ?? (this.isGroupAdmin = new bool?(this.groupChat.UserIsAdmin(this.User.Jid))).Value);
      }
      set
      {
        if (!this.isGroupAdmin.HasValue)
          return;
        bool? isGroupAdmin = this.isGroupAdmin;
        bool flag = value;
        if ((isGroupAdmin.GetValueOrDefault() == flag ? (!isGroupAdmin.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.isGroupAdmin = new bool?();
        this.Notify("RightText");
      }
    }

    public bool IsSuperAdmin
    {
      get => (this.isSuperAdmin = new bool?(this.groupChat.UserIsSuperAdmin(this.User.Jid))).Value;
      set
      {
        if (!this.isSuperAdmin.HasValue)
          return;
        bool? isSuperAdmin = this.isSuperAdmin;
        bool flag = value;
        if ((isSuperAdmin.GetValueOrDefault() == flag ? (!isSuperAdmin.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.isSuperAdmin = new bool?();
        this.Notify("RightText");
      }
    }

    public override bool EnableContextMenu
    {
      get => this.Error <= 0 && this.User != null && this.User.Jid != Settings.MyJid;
    }

    public GroupParticipantViewModel(
      UserStatus user,
      Conversation group,
      GlobalProgressIndicator globalProgressIndicator)
      : base(user)
    {
      this.groupChat = group;
      this.progressIndicator = globalProgressIndicator;
    }

    public override string GetTitle()
    {
      return !JidHelper.IsSelfJid(this.User?.Jid) ? base.GetTitle() : AppResources.You;
    }

    protected override IEnumerable<MenuItem> GetMenuItemsImpl()
    {
      UserStatus user = this.User;
      if (user == null || this.groupChat == null || !this.EnableContextMenu)
        return (IEnumerable<MenuItem>) null;
      List<MenuItem> menuItemsImpl = new List<MenuItem>();
      if (user.VerifiedLevel != VerifiedLevel.NotApplicable)
      {
        MenuItem menuItem1 = new MenuItem();
        menuItem1.Header = (object) AppResources.ViewBusinessInfo;
        MenuItem menuItem2 = menuItem1;
        menuItem2.Click += (RoutedEventHandler) ((sender, e) => ContactInfoPage.Start(user));
        menuItemsImpl.Add(menuItem2);
      }
      string displayName = user.GetDisplayName(true);
      if (!string.IsNullOrEmpty(displayName))
      {
        MenuItem menuItem3 = new MenuItem();
        menuItem3.Header = (object) string.Format(AppResources.OpenSender, (object) displayName);
        MenuItem menuItem4 = menuItem3;
        menuItem4.Click += (RoutedEventHandler) ((sender, e) => NavUtils.NavigateToChat(user.Jid, false));
        menuItemsImpl.Add(menuItem4);
        MenuItem menuItem5 = new MenuItem();
        menuItem5.Header = (object) string.Format(AppResources.CallAContact, (object) displayName);
        MenuItem menuItem6 = menuItem5;
        menuItem6.Click += (RoutedEventHandler) ((sender, e) => CallContact.Call(user.Jid, context: "from participant menu"));
        menuItemsImpl.Add(menuItem6);
        MenuItem menuItem7 = new MenuItem();
        menuItem7.Header = (object) string.Format(AppResources.VideoCallAContact, (object) displayName);
        MenuItem menuItem8 = menuItem7;
        menuItem8.Click += (RoutedEventHandler) ((sender, e) => CallContact.VideoCall(user.Jid));
        menuItemsImpl.Add(menuItem8);
      }
      if (user.IsInDeviceContactList)
      {
        MenuItem menuItem9 = new MenuItem();
        menuItem9.Header = (object) AppResources.ViewContactDetails;
        MenuItem menuItem10 = menuItem9;
        menuItem10.Click += (RoutedEventHandler) ((sender, e) => ContactInfoPage.Start(user));
        menuItemsImpl.Add(menuItem10);
      }
      else
      {
        MenuItem menuItem11 = new MenuItem();
        menuItem11.Header = (object) AppResources.CreateNewContact;
        MenuItem menuItem12 = menuItem11;
        menuItem12.Click += (RoutedEventHandler) ((sender, e) => AddContact.Launch(user.Jid, false));
        menuItemsImpl.Add(menuItem12);
        MenuItem menuItem13 = new MenuItem();
        menuItem13.Header = (object) AppResources.AddToExistingContact;
        MenuItem menuItem14 = menuItem13;
        menuItem14.Click += (RoutedEventHandler) ((sender, e) => AddContact.Launch(user.Jid, true));
        menuItemsImpl.Add(menuItem14);
      }
      MenuItem menuItem15 = new MenuItem();
      menuItem15.Header = (object) AppResources.VerifySecurityNumberMenuItem;
      MenuItem menuItem16 = menuItem15;
      menuItem16.Click += (RoutedEventHandler) ((sender, e) => NavUtils.VerifyIdentityForJid(user.Jid));
      menuItemsImpl.Add(menuItem16);
      if (this.groupChat.UserIsAdmin(Settings.MyJid) && this.groupChat.IsGroupParticipant())
      {
        MenuItem menuItem17 = new MenuItem();
        menuItem17.Header = (object) string.Format(AppResources.RemoveFromGroup, (object) displayName);
        MenuItem menuItem18 = menuItem17;
        menuItem18.Click += (RoutedEventHandler) ((sender, e) =>
        {
          if (this.IsSuperAdmin)
            GroupAminOps.OnRemoveParticipantsErrors(new List<Pair<string, int>>()
            {
              new Pair<string, int>(this.Jid, 406)
            });
          else
            GroupAminOps.RemoveParticipant(this.groupChat.Jid, user, this.progressIndicator);
        });
        menuItemsImpl.Add(menuItem18);
        if (!this.groupChat.UserIsAdmin(user.Jid))
        {
          MenuItem menuItem19 = new MenuItem();
          menuItem19.Header = (object) AppResources.MakeGroupAdmin;
          MenuItem menuItem20 = menuItem19;
          menuItem20.Click += (RoutedEventHandler) ((sender, e) => GroupAminOps.MakePaticipantAdmin(this.groupChat.Jid, user.Jid, this.progressIndicator));
          menuItemsImpl.Add(menuItem20);
        }
        else
        {
          MenuItem menuItem21 = new MenuItem();
          menuItem21.Header = (object) AppResources.DismissGroupAdmin;
          MenuItem menuItem22 = menuItem21;
          menuItem22.Click += (RoutedEventHandler) ((sender, e) =>
          {
            if (this.IsSuperAdmin)
              GroupAminOps.OnDemoteParticipantsErrors(new List<Pair<string, int>>()
              {
                new Pair<string, int>(this.Jid, 406)
              });
            else
              GroupAminOps.MakePaticipantNotAdmin(this.groupChat.Jid, user.Jid, this.progressIndicator);
          });
          if (Settings.GroupsV3)
            menuItemsImpl.Add(menuItem22);
        }
      }
      return (IEnumerable<MenuItem>) menuItemsImpl;
    }
  }
}
