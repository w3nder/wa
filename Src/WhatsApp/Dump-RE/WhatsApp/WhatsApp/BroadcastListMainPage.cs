// Decompiled with JetBrains decompiler
// Type: WhatsApp.BroadcastListMainPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class BroadcastListMainPage : PhoneApplicationPage
  {
    private ChatCollection broadcastLists_;
    private LinkedList<IDisposable> disposables = new LinkedList<IDisposable>();
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal ChatListControl ChatList;
    internal Grid InfoPanel;
    internal TextBlock WarningBlock;
    private bool _contentLoaded;

    public BroadcastListMainPage()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.Mode = PageTitlePanel.Modes.NotZoomed;
      this.PageTitle.SmallTitle = AppResources.BroadcastLists.ToUpper();
      this.WarningBlock.Text = string.Format(AppResources.BroadcastWarning, (object) PhoneNumberFormatter.FormatInternationalNumber(Settings.ChatID));
      this.ChatList.IsMultiSelectionAllowed = false;
      this.disposables.AddLast(this.ChatList.ChatRequestedObservable().ObserveOnDispatcher<Conversation>().Subscribe<Conversation>(new Action<Conversation>(this.ChatList_ChatRequested)));
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.LoadBroadcasts();
    }

    private void LoadBroadcasts()
    {
      this.ChatList.SetSourceAsync(Observable.Create<List<ConversationItem>>((Func<IObserver<List<ConversationItem>>, Action>) (observer =>
      {
        List<ConversationItem> blists = (List<ConversationItem>) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => blists = db.GetConversationItems(new JidHelper.JidTypes[1]
        {
          JidHelper.JidTypes.Broadcast
        }, false, SqliteMessagesContext.ConversationSortTypes.TimestampOnly)));
        observer.OnNext(blists);
        observer.OnCompleted();
        return (Action) (() => { });
      })), (Func<Conversation, bool>) (c => c.IsBroadcast()), (Action<ChatCollection>) (chats => this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.broadcastLists_ = chats;
        chats.GetAllFromDb = (Func<MessagesContext, IEnumerable<Conversation>>) (db => (IEnumerable<Conversation>) db.GetConversations(new JidHelper.JidTypes[1]
        {
          JidHelper.JidTypes.Broadcast
        }, false));
        chats.CollectionChanged += new EventHandler(this.OnBroadcastListsCollectionChanged);
        this.InfoPanel.Visibility = (chats.Count == 0).ToVisibility();
      }))), "broadcast list");
    }

    private Conversation CreateNewBroadcastList(MessagesContext db)
    {
      DateTime now = DateTime.Now;
      string shortTimestampId = DateTimeUtils.GetShortTimestampId(FunRunner.CurrentServerTimeUtc);
      int num = 0;
      string jid = string.Format("{0}{1}", (object) shortTimestampId, (object) "@broadcast");
      while (db.GetConversation(jid, CreateOptions.None) != null)
        jid = string.Format("{0}{1}{2}", (object) shortTimestampId, (object) num++, (object) "@broadcast");
      return db.GetConversation(jid, CreateOptions.CreateIfNotFound);
    }

    private void OnBroadcastListsCollectionChanged(object sender, EventArgs e)
    {
      this.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.InfoPanel.Visibility = (this.broadcastLists_.Count == 0).ToVisibility()));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e) => base.OnNavigatedTo(e);

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.ChatList.Clear();
      base.OnRemovedFromJournal(e);
    }

    private void NewBroadcastList_Click(object sender, EventArgs e)
    {
      Conversation bl = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => bl = this.CreateNewBroadcastList(db)));
      EditParticipantsPage.Start(bl).Take<PageArgs>(1).Subscribe<PageArgs>((Action<PageArgs>) (args =>
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          if (db.GetConversation(bl.Jid, CreateOptions.None) != null)
            return;
          db.InsertConversationOnSubmit(bl);
          Message broadcastListCreated = SystemMessageUtils.CreateBroadcastListCreated(db, bl.Jid, bl.GetParticipantCount(true));
          db.InsertMessageOnSubmit(broadcastListCreated);
          db.SubmitChanges();
          Settings.UpdateContactsChecksum();
          AppState.QrPersistentAction.NotifyContactChange(new FunXMPP.ContactResponse()
          {
            Jid = bl.Jid,
            DisplayName = AppResources.PageTitleBroadcastList
          });
        }));
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateToChat(args.NavService, bl.Jid, false, nameof (BroadcastListMainPage))));
      }));
      FieldStats.ReportUiUsage(wam_enum_ui_usage_type.NEW_BROADCAST_LIST);
    }

    private void ChatList_ChatRequested(Conversation c)
    {
      if (c == null)
        return;
      NavUtils.NavigateToChat(this.NavigationService, c.Jid, false, nameof (BroadcastListMainPage));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/BroadcastListMainPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ChatList = (ChatListControl) this.FindName("ChatList");
      this.InfoPanel = (Grid) this.FindName("InfoPanel");
      this.WarningBlock = (TextBlock) this.FindName("WarningBlock");
    }
  }
}
