// Decompiled with JetBrains decompiler
// Type: WhatsApp.ArchivedChatsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.CommonOps;

#nullable disable
namespace WhatsApp
{
  public class ArchivedChatsPage : PhoneApplicationPage
  {
    private bool? showingArchiveAll;
    private ChatCollection chatCollection;
    private LinkedList<IDisposable> disposables = new LinkedList<IDisposable>();
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal ChatListControl ChatList;
    internal TextBlock InfoBlock;
    private bool _contentLoaded;

    public ArchivedChatsPage()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.SmallTitle = AppResources.ArchivedChats.ToUpper();
      this.InfoBlock.Text = AppResources.NoArchivedChats;
      this.ChatList.IsMultiSelectionAllowed = false;
      this.disposables.AddLast(this.ChatList.ChatRequestedObservable().ObserveOnDispatcher<Conversation>().Subscribe<Conversation>(new Action<Conversation>(this.ChatList_ChatRequested)));
      this.LoadArchivedChats();
    }

    private void LoadArchivedChats()
    {
      this.ChatList.SetSourceAsync(Observable.Create<List<ConversationItem>>((Func<IObserver<List<ConversationItem>>, Action>) (observer =>
      {
        List<ConversationItem> archivedChats = (List<ConversationItem>) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          archivedChats = db.GetArchivedConversationItems();
          archivedChats.Sort(new Comparison<ConversationItem>(ConversationItem.CompareByTimestamp));
        }));
        observer.OnNext(archivedChats);
        observer.OnCompleted();
        return (Action) (() => { });
      })), (Func<Conversation, bool>) (c => !c.IsBroadcast()), (Action<ChatCollection>) (chats =>
      {
        chats.GetAllFromDb = (Func<MessagesContext, IEnumerable<Conversation>>) (db => (IEnumerable<Conversation>) db.GetConversations(new JidHelper.JidTypes[3]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group,
          JidHelper.JidTypes.Psa
        }, true));
        chats.SkipAddingFilter = (Func<Conversation, bool>) (c => !c.IsArchived);
        chats.CollectionChanged += new EventHandler(this.OnChatCollectionChanged);
        this.chatCollection = chats;
        this.Dispatcher.BeginInvoke((Action) (() => this.InfoBlock.Visibility = (chats.Count == 0).ToVisibility()));
      }), "archived chats");
    }

    private void ResetAppBar(bool? hasUnarchivedChats = null)
    {
      this.ApplicationBar = (IApplicationBar) null;
    }

    private void ChatList_ChatRequested(Conversation c)
    {
      if (c == null)
        return;
      NavUtils.NavigateToChat(this.NavigationService, c.Jid, false, nameof (ArchivedChatsPage));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      this.ResetAppBar();
      base.OnNavigatedTo(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.ChatList.Clear();
      List<IDisposable> list = this.disposables.ToList<IDisposable>();
      this.disposables.Clear();
      list.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
      base.OnRemovedFromJournal(e);
    }

    private void OnChatCollectionChanged(object sender, EventArgs e)
    {
      this.ResetAppBar();
      this.InfoBlock.Visibility = (this.chatCollection.Count == 0).ToVisibility();
    }

    private void ArchiveAll_Click(object sender, EventArgs e)
    {
      Observable.Return<bool>(true).Decision(AppResources.ArchiveAllConfirm).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (accept =>
      {
        if (!accept)
          return;
        this.ChatList.Clear();
        ArchiveChat.ArchiveAll();
        FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHATS_ALL_ARCHIVE);
        this.LoadArchivedChats();
        this.ResetAppBar(new bool?(false));
      }));
    }

    private void UnarchiveAll_Click(object sender, EventArgs e)
    {
      this.ChatList.Clear();
      ArchiveChat.UnarchiveAll();
      this.LoadArchivedChats();
      this.ResetAppBar(new bool?(true));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ArchivedChatsPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ChatList = (ChatListControl) this.FindName("ChatList");
      this.InfoBlock = (TextBlock) this.FindName("InfoBlock");
    }
  }
}
