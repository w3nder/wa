// Decompiled with JetBrains decompiler
// Type: WhatsApp.AddToGroupsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class AddToGroupsPage : PhoneApplicationPage
  {
    private static string nextInstanceTargetJid;
    private string targetJid;
    private IDisposable groupUpdatedSub;
    private bool addingMember;
    private HashSet<string> allGroupJids;
    private List<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>> viewModels;
    private HashSet<AddToGroupsPage.GroupItemKey> groupKeys = new HashSet<AddToGroupsPage.GroupItemKey>();
    private bool isVmsDisposed;
    private object vmsDisposedLock = new object();
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal WhatsApp.CompatibilityShims.LongListSelector GroupsList;
    internal TextBlock ListHeader;
    private bool _contentLoaded;

    public AddToGroupsPage()
    {
      this.InitializeComponent();
      this.targetJid = AddToGroupsPage.nextInstanceTargetJid;
      AddToGroupsPage.nextInstanceTargetJid = (string) null;
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.SmallTitle = UserCache.Get(this.targetJid, false)?.GetDisplayName();
      this.PageTitle.LargeTitle = AppResources.AddToGroup;
      this.ListHeader.Text = AppResources.CannotAddToGroupsTooltip;
      this.GroupsList.SelectionChanged += new SelectionChangedEventHandler(this.ListSelector_SelectionChanged);
      this.GroupsList.JumpListStyle = (Style) null;
      this.LoadData();
      this.CreateUpdatedMembershipSub();
    }

    public static void Start(string jid)
    {
      AddToGroupsPage.nextInstanceTargetJid = jid;
      NavUtils.NavigateToPage(nameof (AddToGroupsPage));
    }

    private void CreateUpdatedMembershipSub()
    {
      if (this.groupUpdatedSub != null)
        return;
      this.groupUpdatedSub = FunEventHandler.Events.GroupMembershipUpdatedSubject.Where<FunEventHandler.Events.ConversationWithFlags>((Func<FunEventHandler.Events.ConversationWithFlags, bool>) (args =>
      {
        if (args.Conversation == null || !this.allGroupJids.Contains(args.Conversation.Jid))
          return false;
        return args.MembersChanged || args.AdminsChanged;
      })).ObserveOnDispatcher<FunEventHandler.Events.ConversationWithFlags>().Subscribe<FunEventHandler.Events.ConversationWithFlags>((Action<FunEventHandler.Events.ConversationWithFlags>) (args =>
      {
        lock (this.vmsDisposedLock)
        {
          if (this.isVmsDisposed)
            return;
          this.UpdateGroup(args);
        }
      }));
    }

    private void LoadData()
    {
      AppState.Worker.Enqueue((Action) (() =>
      {
        List<Conversation> conversations = (List<Conversation>) null;
        HashSet<string> adminGroupJids = (HashSet<string>) null;
        HashSet<string> targetParticipantGroupJids = (HashSet<string>) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          lock (this.vmsDisposedLock)
          {
            if (!this.isVmsDisposed)
            {
              conversations = db.GetConversations(new JidHelper.JidTypes[1]
              {
                JidHelper.JidTypes.Group
              }, true);
              this.allGroupJids = new HashSet<string>(conversations.Select<Conversation, string>((Func<Conversation, string>) (convo => convo.Jid)));
              adminGroupJids = new HashSet<string>((IEnumerable<string>) db.GetAdminGroups());
              targetParticipantGroupJids = new HashSet<string>((IEnumerable<string>) db.GetGroupsInCommonJids(new string[1]
              {
                this.targetJid
              }));
            }
            if (conversations == null || adminGroupJids == null || targetParticipantGroupJids == null)
              return;
            List<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>> groupings = conversations.OrderBy<Conversation, string>((Func<Conversation, string>) (c => c.GroupSubject)).Select<Conversation, AddToGroupItemViewModel>((Func<Conversation, AddToGroupItemViewModel>) (group =>
            {
              return new AddToGroupItemViewModel(group)
              {
                EnableChatPreview = false,
                EnableContextMenu = false,
                EnableRecipientCheck = false,
                SelfIsAdmin = adminGroupJids.Contains(group.Jid),
                TargetIsParticipant = targetParticipantGroupJids.Contains(group.Jid)
              };
            })).GroupBy<AddToGroupItemViewModel, AddToGroupsPage.GroupItemKey>((Func<AddToGroupItemViewModel, AddToGroupsPage.GroupItemKey>) (gvm => this.GetGroup(gvm.ShouldDisable()))).Select<IGrouping<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>, KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>((Func<IGrouping<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>, KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>) (grp => new KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>(grp))).OrderBy<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>, AddToGroupsPage.GroupItemKey>((Func<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>, AddToGroupsPage.GroupItemKey>) (keyedCollection => keyedCollection.Key)).ToList<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>();
            AddToGroupsPage.GroupItemKey falseGroupKey = this.groupKeys.FirstOrDefault<AddToGroupsPage.GroupItemKey>((Func<AddToGroupsPage.GroupItemKey, bool>) (k => !k.KeyValue));
            AddToGroupsPage.GroupItemKey groupItemKey = this.groupKeys.FirstOrDefault<AddToGroupsPage.GroupItemKey>((Func<AddToGroupsPage.GroupItemKey, bool>) (k => k.KeyValue));
            if (falseGroupKey == null)
              this.CreateEmptyGrouping(groupings, false);
            if (groupItemKey == null)
              this.CreateEmptyGrouping(groupings, true);
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
              if (falseGroupKey == null)
                this.ListHeader.Visibility = Visibility.Visible;
              this.viewModels = groupings;
              this.GroupsList.ItemsSource = (IList) this.viewModels;
            }));
          }
        }));
      }));
    }

    private void CreateEmptyGrouping(
      List<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>> groupings,
      bool groupingKey)
    {
      IEnumerable<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>> source = new List<AddToGroupItemViewModel>((IEnumerable<AddToGroupItemViewModel>) new AddToGroupItemViewModel[1]
      {
        new AddToGroupItemViewModel(new Conversation())
      }).GroupBy<AddToGroupItemViewModel, AddToGroupsPage.GroupItemKey>((Func<AddToGroupItemViewModel, AddToGroupsPage.GroupItemKey>) (gvm => new AddToGroupsPage.GroupItemKey(groupingKey))).Select<IGrouping<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>, KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>((Func<IGrouping<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>, KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>) (grp => new KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>(grp)));
      if (source.FirstOrDefault<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>() == null)
        return;
      if (!groupingKey)
        groupings.Insert(0, source.FirstOrDefault<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>());
      else
        groupings.Add(source.FirstOrDefault<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>());
      groupings[0].Clear();
    }

    private AddToGroupsPage.GroupItemKey GetGroup(bool grouping)
    {
      AddToGroupsPage.GroupItemKey group1 = this.groupKeys.FirstOrDefault<AddToGroupsPage.GroupItemKey>((Func<AddToGroupsPage.GroupItemKey, bool>) (k => k.KeyValue == grouping));
      if (group1 != null)
        return group1;
      AddToGroupsPage.GroupItemKey group2 = new AddToGroupsPage.GroupItemKey(grouping);
      this.groupKeys.Add(group2);
      return group2;
    }

    private void UpdateGroup(FunEventHandler.Events.ConversationWithFlags args)
    {
      if (this.addingMember || this.viewModels == null)
        return;
      KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel> source = this.viewModels.FirstOrDefault<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>((Func<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>, bool>) (list => list.Select<AddToGroupItemViewModel, string>((Func<AddToGroupItemViewModel, string>) (gvm => gvm.Conversation.Jid)).Contains<string>(args.Conversation.Jid)));
      if (source == null)
        return;
      AddToGroupItemViewModel groupItemViewModel1 = source.FirstOrDefault<AddToGroupItemViewModel>((Func<AddToGroupItemViewModel, bool>) (gvm => gvm.Conversation.Jid == args.Conversation.Jid));
      if (groupItemViewModel1 == null)
        return;
      AddToGroupItemViewModel groupItemViewModel2 = new AddToGroupItemViewModel(args.Conversation);
      groupItemViewModel2.EnableChatPreview = false;
      groupItemViewModel2.EnableContextMenu = false;
      groupItemViewModel2.EnableRecipientCheck = false;
      groupItemViewModel2.SelfIsAdmin = args.AdminsChanged ? args.Conversation.UserIsAdmin(Settings.MyJid) : groupItemViewModel1.SelfIsAdmin;
      groupItemViewModel2.TargetIsParticipant = args.MembersChanged ? args.Conversation.ContainsParticipant(this.targetJid) : groupItemViewModel1.TargetIsParticipant;
      AddToGroupItemViewModel updated = groupItemViewModel2;
      source.Remove(groupItemViewModel1);
      if (source.Count == 0)
        this.ListHeader.Visibility = Visibility.Visible;
      KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel> that = this.viewModels.FirstOrDefault<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>((Func<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>, bool>) (c => c.Key.KeyValue == updated.ShouldDisable()));
      if (that == null)
        return;
      that.InsertInOrder<AddToGroupItemViewModel>(updated, (Func<AddToGroupItemViewModel, AddToGroupItemViewModel, bool>) ((gvm1, gvm2) => StringComparer.CurrentCulture.Compare(gvm1.TitleStr, gvm2.TitleStr) < 0));
      if (source.Count <= 0)
        return;
      this.ListHeader.Visibility = Visibility.Collapsed;
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.groupUpdatedSub.SafeDispose();
      this.groupUpdatedSub = (IDisposable) null;
      lock (this.vmsDisposedLock)
        this.isVmsDisposed = true;
      this.viewModels = (List<KeyedObservableCollection<AddToGroupsPage.GroupItemKey, AddToGroupItemViewModel>>) null;
      base.OnRemovedFromJournal(e);
    }

    private void ListSelector_SelectionChanged(object sender, EventArgs e)
    {
      AddToGroupItemViewModel selItem = this.GroupsList.SelectedItem as AddToGroupItemViewModel;
      this.GroupsList.SelectedItem = (object) null;
      if (selItem == null || selItem.ShouldDisable())
        return;
      UserStatus userStatus = UserCache.Get(this.targetJid, false);
      if (userStatus == null)
        return;
      string displayName = userStatus.GetDisplayName();
      if (displayName == null)
        return;
      UIUtils.MessageBox(string.Format(AppResources.ConfirmAddParticipantTitle, (object) displayName), string.Format(AppResources.ConfirmAddParticipantToGroup, (object) displayName, (object) selItem.TitleStr), (IEnumerable<string>) new string[2]
      {
        AppResources.AddToGroup,
        AppResources.Cancel
      }, (Action<int>) (idx =>
      {
        this.addingMember = true;
        if (idx != 0)
          return;
        GroupCreationPage.AddMemberAsync(selItem.Jid, this.targetJid).ObserveOnDispatcher<Dictionary<string, int>>().Subscribe<Dictionary<string, int>>((Action<Dictionary<string, int>>) (_ => NavUtils.NavigateToChat(selItem.Jid, false)));
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/AddToGroupsPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.GroupsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("GroupsList");
      this.ListHeader = (TextBlock) this.FindName("ListHeader");
    }

    private class GroupItemKey : IComparable
    {
      private bool key;

      public GroupItemKey(bool shouldDisable) => this.key = shouldDisable;

      public bool KeyValue => this.key;

      public Visibility DividerVisibility => this.key.ToVisibility();

      public string HeaderText => AppResources.UnavailableGroups;

      public int CompareTo(object obj)
      {
        return this.key.CompareTo((obj as AddToGroupsPage.GroupItemKey).key);
      }
    }
  }
}
