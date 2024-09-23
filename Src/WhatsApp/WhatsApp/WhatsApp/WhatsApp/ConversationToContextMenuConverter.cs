// Decompiled with JetBrains decompiler
// Type: WhatsApp.ConversationToContextMenuConverter
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
using WhatsApp.CommonOps;


namespace WhatsApp
{
  public class ConversationToContextMenuConverter : IValueConverter
  {
    public Subject<Unit> RefreshItemSubject { get; set; }

    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is Conversation conversation))
        return (object) null;
      ContextMenu contextMenu = new ContextMenu();
      contextMenu.Tag = (object) conversation;
      contextMenu.IsZoomEnabled = false;
      contextMenu.Opened += new RoutedEventHandler(this.Menu_Opened);
      return (object) contextMenu;
    }

    public object ConvertBack(
      object value,
      System.Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<MenuItem> GetMenuItems(Conversation convo)
    {
      IEnumerable<MenuItem> menuItems = (IEnumerable<MenuItem>) null;
      if (convo.IsUserChat())
      {
        UserStatus user = UserCache.Get(convo.Jid, true);
        if (user.IsInDeviceContactList || user.IsVerified() && (user.VerifiedLevel == VerifiedLevel.high || user.VerifiedLevel == VerifiedLevel.low))
          menuItems = this.GetIndividualChatItemMenuItems(convo);
        if (!user.IsInDeviceContactList)
          menuItems = this.GetUnknownIndividualChatItemMenuItems(convo);
      }
      else if (convo.IsGroup())
      {
        menuItems = this.GetGroupChatItemMenuItems(convo);
      }
      else
      {
        if (convo.IsBroadcast())
          return this.GetBroadcastListItemMenuItems(convo);
        Log.SendCrashLog(new Exception(), "unexpected convo type");
      }
      return this.GetCommonMenuItemsTop(convo).Concat<MenuItem>((IEnumerable<MenuItem>) ((object) menuItems ?? (object) new MenuItem[0])).Concat<MenuItem>(this.GetCommonMenuItemsBottom(convo));
    }

    private IEnumerable<MenuItem> GetCommonMenuItemsTop(Conversation convo)
    {
      List<MenuItem> commonMenuItemsTop = new List<MenuItem>();
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Header = convo.IsRead() ? (object) AppResources.MarkAsUnread : (object) AppResources.MarkAsRead;
      menuItem1.Tag = (object) convo;
      MenuItem menuItem2 = menuItem1;
      menuItem2.Click += new RoutedEventHandler(this.ToggleReadState_Click);
      commonMenuItemsTop.Add(menuItem2);
      bool isMuted = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        JidInfo jidInfo = db.GetJidInfo(convo.Jid, CreateOptions.None);
        isMuted = jidInfo != null && jidInfo.IsMuted();
      }));
      string str = isMuted ? AppResources.Unmute : AppResources.MuteTitle;
      MenuItem menuItem3 = new MenuItem();
      menuItem3.Header = (object) str;
      menuItem3.Tag = (object) convo;
      MenuItem menuItem4 = menuItem3;
      menuItem4.Click += new RoutedEventHandler(this.ToggleMute_Click);
      commonMenuItemsTop.Add(menuItem4);
      if (!convo.IsArchived)
      {
        MenuItem menuItem5;
        if (convo.IsPinned())
        {
          MenuItem menuItem6 = new MenuItem();
          menuItem6.Header = (object) AppResources.UnpinTitle;
          menuItem6.Tag = (object) convo;
          menuItem5 = menuItem6;
          menuItem5.Click += new RoutedEventHandler(this.UnpinChat_Click);
        }
        else
        {
          MenuItem menuItem7 = new MenuItem();
          menuItem7.Header = (object) AppResources.PinTitle;
          menuItem7.Tag = (object) convo;
          menuItem5 = menuItem7;
          menuItem5.Click += new RoutedEventHandler(this.PinChat_Click);
        }
        commonMenuItemsTop.Add(menuItem5);
      }
      return (IEnumerable<MenuItem>) commonMenuItemsTop;
    }

    private IEnumerable<MenuItem> GetCommonMenuItemsBottom(Conversation convo)
    {
      LinkedList<MenuItem> commonMenuItemsBottom = new LinkedList<MenuItem>();
      MenuItem menuItem1;
      if (convo.IsArchived)
      {
        MenuItem menuItem2 = new MenuItem();
        menuItem2.Header = (object) AppResources.Unarchive;
        menuItem2.Tag = (object) convo;
        menuItem1 = menuItem2;
        menuItem1.Click += new RoutedEventHandler(this.UnarchiveChat_Click);
      }
      else
      {
        MenuItem menuItem3 = new MenuItem();
        menuItem3.Header = (object) AppResources.Archive;
        menuItem3.Tag = (object) convo;
        menuItem1 = menuItem3;
        menuItem1.Click += new RoutedEventHandler(this.ArchiveChat_Click);
      }
      commonMenuItemsBottom.AddLast(menuItem1);
      MenuItem menuItem4 = new MenuItem();
      menuItem4.Header = !convo.IsGroup() || !convo.IsGroupParticipant() ? (object) AppResources.Delete : (object) AppResources.LeaveGroup;
      menuItem4.Tag = (object) convo;
      MenuItem menuItem5 = menuItem4;
      menuItem5.Click += new RoutedEventHandler(this.DeleteChat_Click);
      commonMenuItemsBottom.AddLast(menuItem5);
      return (IEnumerable<MenuItem>) commonMenuItemsBottom;
    }

    private IEnumerable<MenuItem> GetGroupChatItemMenuItems(Conversation convo)
    {
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Header = (object) AppResources.GroupInfo;
      menuItem1.Tag = (object) convo;
      MenuItem menuItem2 = menuItem1;
      menuItem2.Click += new RoutedEventHandler(this.ChatInfo_Click);
      return (IEnumerable<MenuItem>) new MenuItem[1]
      {
        menuItem2
      };
    }

    private IEnumerable<MenuItem> GetBroadcastListItemMenuItems(Conversation convo)
    {
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Header = !convo.IsGroup() || !convo.IsGroupParticipant() ? (object) AppResources.Delete : (object) AppResources.LeaveGroup;
      menuItem1.Tag = (object) convo;
      MenuItem menuItem2 = menuItem1;
      menuItem2.Click += new RoutedEventHandler(this.DeleteChat_Click);
      return (IEnumerable<MenuItem>) new MenuItem[1]
      {
        menuItem2
      };
    }

    private IEnumerable<MenuItem> GetIndividualChatItemMenuItems(Conversation convo)
    {
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Header = (object) AppResources.ViewContactDetails;
      menuItem1.Tag = (object) convo;
      MenuItem menuItem2 = menuItem1;
      menuItem2.Click += new RoutedEventHandler(this.ChatInfo_Click);
      return (IEnumerable<MenuItem>) new MenuItem[1]
      {
        menuItem2
      };
    }

    private IEnumerable<MenuItem> GetUnknownIndividualChatItemMenuItems(Conversation convo)
    {
      List<MenuItem> chatItemMenuItems = new List<MenuItem>(2);
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Header = (object) AppResources.CreateNewContact;
      menuItem1.Tag = (object) convo;
      MenuItem menuItem2 = menuItem1;
      menuItem2.Click += new RoutedEventHandler(this.CreateNewContact_Click);
      chatItemMenuItems.Add(menuItem2);
      MenuItem menuItem3 = new MenuItem();
      menuItem3.Header = (object) AppResources.AddToExistingContact;
      menuItem3.Tag = (object) convo;
      MenuItem menuItem4 = menuItem3;
      menuItem4.Click += new RoutedEventHandler(this.AddToExistingContact_Click);
      chatItemMenuItems.Add(menuItem4);
      return (IEnumerable<MenuItem>) chatItemMenuItems;
    }

    private void Menu_Opened(object sender, EventArgs e)
    {
      if (!(sender is ContextMenu contextMenu) || !(contextMenu.Tag is Conversation tag))
        return;
      contextMenu.ItemsSource = (IEnumerable) this.GetMenuItems(tag);
    }

    private void Pin_Click(object sender, RoutedEventArgs e)
    {
      Conversation tag = !(sender is MenuItem menuItem) ? (Conversation) null : menuItem.Tag as Conversation;
      if (tag == null)
        return;
      PinToStart.Pin(tag);
    }

    private void ToggleReadState_Click(object sender, RoutedEventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;
      Conversation convo = menuItem == null ? (Conversation) null : menuItem.Tag as Conversation;
      if (convo == null)
        return;
      AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db => MarkChatRead.ToggleReadState(db, convo.Jid, true)))));
    }

    private void ChatInfo_Click(object sender, EventArgs e)
    {
      Conversation tag = !(sender is MenuItem menuItem) ? (Conversation) null : menuItem.Tag as Conversation;
      if (tag == null)
        return;
      if (tag.IsGroup())
      {
        GroupInfoPage.Start((NavigationService) null, tag);
      }
      else
      {
        if (!tag.IsUserChat())
          return;
        ContactInfoPage.Start(UserCache.Get(tag.Jid, true));
      }
    }

    private void CreateNewContact_Click(object sender, EventArgs e)
    {
      Conversation tag = !(sender is MenuItem menuItem) ? (Conversation) null : menuItem.Tag as Conversation;
      if (tag == null)
        return;
      AddContact.Launch(tag.Jid, false);
    }

    private void AddToExistingContact_Click(object sender, EventArgs e)
    {
      Conversation tag = !(sender is MenuItem menuItem) ? (Conversation) null : menuItem.Tag as Conversation;
      if (tag == null)
        return;
      AddContact.Launch(tag.Jid, true);
    }

    private void ToggleMute_Click(object sender, EventArgs e)
    {
      Conversation tag = !(sender is MenuItem menuItem) ? (Conversation) null : menuItem.Tag as Conversation;
      if (tag == null)
        return;
      MuteChatPicker.Launch(tag.Jid).Subscribe<Unit>();
    }

    private void PinChat_Click(object sender, EventArgs e)
    {
      Conversation tag = !(sender is MenuItem menuItem) ? (Conversation) null : menuItem.Tag as Conversation;
      if (tag == null)
        return;
      PinChat.Pin(tag);
    }

    private void UnpinChat_Click(object sender, EventArgs e)
    {
      Conversation tag = !(sender is MenuItem menuItem) ? (Conversation) null : menuItem.Tag as Conversation;
      if (tag == null)
        return;
      PinChat.Unpin(tag);
    }

    private void ArchiveChat_Click(object sender, EventArgs e)
    {
      Conversation tag = !(sender is MenuItem menuItem) ? (Conversation) null : menuItem.Tag as Conversation;
      if (tag == null)
        return;
      ArchiveChat.Archive(tag.Jid);
      FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHAT_ARCHIVE);
      if (this.RefreshItemSubject == null)
        return;
      this.RefreshItemSubject.OnNext(new Unit());
    }

    private void UnarchiveChat_Click(object sender, EventArgs e)
    {
      Conversation tag = !(sender is MenuItem menuItem) ? (Conversation) null : menuItem.Tag as Conversation;
      if (tag == null)
        return;
      ArchiveChat.Unarchive(tag.Jid);
      if (this.RefreshItemSubject == null)
        return;
      this.RefreshItemSubject.OnNext(new Unit());
    }

    private void DeleteChat_Click(object sender, EventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;
      Conversation convo = menuItem == null ? (Conversation) null : menuItem.Tag as Conversation;
      if (convo == null)
        return;
      DeleteChat.PromptForDelete(Observable.Return<bool>(true), convo).Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (accept =>
      {
        if (!accept)
          return;
        DeleteChat.Delete(convo.Jid);
        FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHAT_DELETE);
      }));
    }
  }
}
