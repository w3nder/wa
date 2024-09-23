// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonGroupsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class CommonGroupsPage : PhoneApplicationPage
  {
    private static string[] nextInstanceUserJids;
    private string[] userJids;
    private bool isLoaded;
    private bool isShown;
    private List<ChatItemViewModel> viewModels;
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal WhatsApp.CompatibilityShims.LongListSelector ListBox;
    private bool _contentLoaded;

    public CommonGroupsPage()
    {
      this.InitializeComponent();
      this.userJids = CommonGroupsPage.nextInstanceUserJids;
      CommonGroupsPage.nextInstanceUserJids = (string[]) null;
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.SmallTitle = AppResources.GroupsInCommon;
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.ListBox.SelectionChanged += new SelectionChangedEventHandler(this.ListBox_SelectionChanged);
      this.LoadData();
    }

    public static void Start(string[] jids)
    {
      string[] array = ((IEnumerable<string>) (jids ?? new string[0])).Where<string>((Func<string, bool>) (jid => jid != null && JidHelper.IsUserJid(jid))).ToArray<string>();
      if (!((IEnumerable<string>) array).Any<string>())
        return;
      CommonGroupsPage.nextInstanceUserJids = array;
      NavUtils.NavigateToPage(nameof (CommonGroupsPage));
    }

    private void LoadData()
    {
      AppState.Worker.Enqueue((Action) (() =>
      {
        Conversation[] commonGroups = (Conversation[]) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => commonGroups = db.GetGroupsInCommon(this.userJids)));
        if (commonGroups == null)
          return;
        List<ChatItemViewModel> itemVms = ((IEnumerable<Conversation>) commonGroups).OrderBy<Conversation, string>((Func<Conversation, string>) (g => g.GroupSubject)).Select<Conversation, ChatItemViewModel>((Func<Conversation, ChatItemViewModel>) (g =>
        {
          return new ChatItemViewModel(g)
          {
            EnableChatPreview = false,
            EnableContextMenu = false,
            EnableRecipientCheck = false
          };
        })).ToList<ChatItemViewModel>();
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.viewModels = itemVms;
          this.TryShow();
        }));
      }));
    }

    private void TryShow()
    {
      if (!this.isLoaded || this.isShown || this.viewModels == null)
        return;
      this.isShown = true;
      this.ListBox.ItemsSource = (IList) this.viewModels;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      this.isLoaded = true;
      this.TryShow();
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ChatItemViewModel selectedItem = this.ListBox.SelectedItem as ChatItemViewModel;
      this.ListBox.SelectedItem = (object) null;
      if (selectedItem == null)
        return;
      NavUtils.NavigateToChat(selectedItem.Jid, false);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/CommonGroupsPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ListBox = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ListBox");
    }
  }
}
