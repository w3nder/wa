// Decompiled with JetBrains decompiler
// Type: WhatsApp.StarredMessagesPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;


namespace WhatsApp
{
  public class StarredMessagesPage : PhoneApplicationPage
  {
    private static string[] nextInstanceJids = new string[0];
    private string[] jids = new string[0];
    private IMessageListControl msgList;
    private FrameworkElement msgListElement;
    private IDisposable msgDeletedSub;
    private IDisposable msgUpdatedSub;
    private IDisposable audioRoutingSub;
    private bool isPageRemoved;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal ZoomBox SearchFieldZoomBox;
    internal EmojiTextBox SearchField;
    internal ZoomBox TooltipZoomBox;
    internal Grid TooltipGrid;
    internal Image NoStarredImg;
    internal TextBlock StarredMsgsBlock;
    internal TextBlock TooltipBlock;
    private bool _contentLoaded;

    public StarredMessagesPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.jids = StarredMessagesPage.nextInstanceJids;
      StarredMessagesPage.nextInstanceJids = new string[0];
      this.PageTitle.SmallTitle = AppResources.StarredMessagesUpper;
      this.TooltipZoomBox.ZoomFactor = this.SearchFieldZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      if (this.jids != null && !((IEnumerable<string>) this.jids).Any<string>())
        return;
      this.InitMessageList();
    }

    public static void Start(string[] jids)
    {
      StarredMessagesPage.nextInstanceJids = jids;
      NavUtils.NavigateToPage(nameof (StarredMessagesPage));
    }

    private void InitMessageList()
    {
      MessageListControl messageListControl1 = new MessageListControl(false, false, false);
      messageListControl1.CacheMode = (CacheMode) new BitmapCache();
      messageListControl1.Margin = new Thickness(0.0, 12.0, 0.0, 0.0);
      messageListControl1.ItemTemplate = this.Resources[(object) "MessageListItemTemplateLLS"] as DataTemplate;
      messageListControl1.EnableScrollButton = false;
      MessageListControl messageListControl2 = messageListControl1;
      this.msgList = (IMessageListControl) messageListControl2;
      this.msgListElement = (FrameworkElement) messageListControl2;
      Grid.SetRow(this.msgListElement, 1);
      this.LayoutRoot.Children.Insert(1, (UIElement) this.msgListElement);
      AppState.Worker.Enqueue((Action) (() =>
      {
        Message[] msgs = (Message[]) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msgs = db.GetStarredMessages(this.jids, new int?(), new int?());
          this.msgDeletedSub = db.DeletedMessagesSubject.Where<Message>((Func<Message, bool>) (msg => this.jids == null || ((IEnumerable<string>) this.jids).Contains<string>(msg.KeyRemoteJid))).ObserveOnDispatcher<Message>().Subscribe<Message>((Action<Message>) (deletedMsg =>
          {
            if (this.isPageRemoved)
              return;
            this.msgList.Remove(deletedMsg);
            if (!this.msgList.IsEmpty())
              return;
            this.ShowNoStarredMessagesUI();
          }));
          this.msgUpdatedSub = db.MessageUpdateSubject.Select<DbDataUpdate, Message>((Func<DbDataUpdate, Message>) (u => u.UpdatedObj as Message)).Where<Message>((Func<Message, bool>) (msg =>
          {
            if (this.jids == null)
              return true;
            return msg != null && ((IEnumerable<string>) this.jids).Contains<string>(msg.KeyRemoteJid);
          })).ObserveOnDispatcher<Message>().Subscribe<Message>((Action<Message>) (updatedMsg =>
          {
            if (updatedMsg.IsStarred)
              return;
            this.msgList.Remove(updatedMsg);
            if (!this.msgList.IsEmpty())
              return;
            this.ShowNoStarredMessagesUI();
          }));
        }));
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.isPageRemoved)
          {
            this.msgDeletedSub.SafeDispose();
            this.msgUpdatedSub.SafeDispose();
            this.msgUpdatedSub = this.msgDeletedSub = (IDisposable) null;
          }
          else
          {
            if (msgs == null)
              msgs = new Message[0];
            if (!((IEnumerable<Message>) msgs).Any<Message>())
              this.ShowNoStarredMessagesUI();
            this.msgList.SetMessages(msgs, false, new int?(), forStarredMessagesView: true);
          }
        }));
      }));
    }

    private void ShowNoStarredMessagesUI()
    {
      this.TooltipBlock.Text = AppResources.NoStarredMessages;
      this.TooltipZoomBox.Visibility = Visibility.Visible;
      this.NoStarredImg.Source = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("illustration-no-starred.png");
      this.StarredMsgsBlock.Text = AppResources.NoStarredMessagesTitle;
      new AppBarWrapper(this.ApplicationBar).EnableMenuItems(false);
    }

    private void InitAudioPlayback()
    {
      if (this.audioRoutingSub != null)
        return;
      this.audioRoutingSub = (IDisposable) new PttPlaybackWrapper();
    }

    private void DisposeAudioPlayback()
    {
      this.audioRoutingSub.SafeDispose();
      this.audioRoutingSub = (IDisposable) null;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      this.InitAudioPlayback();
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      this.DisposeAudioPlayback();
      base.OnNavigatingFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.isPageRemoved = true;
      this.msgDeletedSub.SafeDispose();
      this.msgUpdatedSub.SafeDispose();
      this.msgUpdatedSub = this.msgDeletedSub = (IDisposable) null;
      base.OnRemovedFromJournal(e);
    }

    private void UnstarAllMenuItem_Click(object sender, EventArgs e)
    {
      string[] targetJids = this.jids;
      string unstarConfirmTitle = AppResources.UnstarConfirmTitle;
      string confirmBody = AppResources.UnstarAllMessagesConfirmBody;
      if (targetJids != null && ((IEnumerable<string>) targetJids).Any<string>())
      {
        string firstJid = ((IEnumerable<string>) targetJids).First<string>();
        if (JidHelper.IsGroupJid(firstJid))
        {
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Conversation conversation = db.GetConversation(firstJid, CreateOptions.None);
            if (conversation == null)
              return;
            confirmBody = string.Format(AppResources.UnstarGroupMessagesConfirmBody, (object) conversation.GetName());
          }));
        }
        else
        {
          UserStatus userStatus = ((IEnumerable<string>) targetJids).Select<string, UserStatus>((Func<string, UserStatus>) (jid => UserCache.Get(jid, false))).FirstOrDefault<UserStatus>((Func<UserStatus, bool>) (u => u != null));
          if (userStatus != null)
            confirmBody = string.Format(AppResources.UnstarContactMessagesConfirmBody, (object) userStatus.GetDisplayName(true));
        }
      }
      Observable.Return<bool>(true).Decision(confirmBody, AppResources.UnstarConfirmPositiveButton, AppResources.CancelButton, unstarConfirmTitle).Take<bool>(1).Where<bool>((Func<bool, bool>) (confirmed => confirmed)).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (_ =>
      {
        this.msgList.RemoveAll();
        this.ShowNoStarredMessagesUI();
        AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.UnstarMessages(targetJids)))));
        int modTag = 0;
        if (targetJids != null && ((IEnumerable<string>) targetJids).Any<string>())
        {
          foreach (string str in targetJids)
          {
            string jid = str;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              Conversation conversation = db.GetConversation(jid, CreateOptions.None);
              if (conversation == null)
                return;
              modTag = (int) conversation.ModifyTag;
            }));
            AppState.QrPersistentAction.NotifyUnStarAll(jid, modTag);
          }
        }
        else
          AppState.QrPersistentAction.NotifyUnStarAll("s.whatsapp.net", modTag);
      }));
    }

    private void SearchButton_Click(object sender, EventArgs e)
    {
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/StarredMessagesPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.SearchFieldZoomBox = (ZoomBox) this.FindName("SearchFieldZoomBox");
      this.SearchField = (EmojiTextBox) this.FindName("SearchField");
      this.TooltipZoomBox = (ZoomBox) this.FindName("TooltipZoomBox");
      this.TooltipGrid = (Grid) this.FindName("TooltipGrid");
      this.NoStarredImg = (Image) this.FindName("NoStarredImg");
      this.StarredMsgsBlock = (TextBlock) this.FindName("StarredMsgsBlock");
      this.TooltipBlock = (TextBlock) this.FindName("TooltipBlock");
    }
  }
}
