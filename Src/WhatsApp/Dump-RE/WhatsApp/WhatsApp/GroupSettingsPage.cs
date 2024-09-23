// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupSettingsPage
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
using WhatsApp.CommonOps;

#nullable disable
namespace WhatsApp
{
  public class GroupSettingsPage : PhoneApplicationPage
  {
    private static string nextInstanceTargetName;
    private static string nextJid;
    private Conversation convo;
    private GroupSettingsPageViewModel viewModel;
    private List<IDisposable> disposables = new List<IDisposable>();
    private const string LogHeader = "group settings";
    private GlobalProgressIndicator progress;
    internal ListBoxItem RestrictPanel;
    internal TextBlock RestrictTitleBlock;
    internal TextBlock RestrictStateBlock;
    internal ListBoxItem AnnouncePanel;
    internal TextBlock AnnounceTitleBlock;
    internal TextBlock AnnounceStateBlock;
    private bool _contentLoaded;

    public GroupSettingsPage()
    {
      this.InitializeComponent();
      this.DataContext = (object) (this.viewModel = new GroupSettingsPageViewModel(GroupSettingsPage.nextInstanceTargetName, GroupSettingsPage.nextJid, this.Orientation));
      GroupSettingsPage.nextInstanceTargetName = (string) null;
      GroupSettingsPage.nextJid = (string) null;
      this.progress = new GlobalProgressIndicator((DependencyObject) this);
    }

    public static void Start(string targetName, string jid)
    {
      GroupSettingsPage.nextInstanceTargetName = targetName;
      GroupSettingsPage.nextJid = jid;
      NavUtils.NavigateToPage(nameof (GroupSettingsPage));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (this.convo == null && !string.IsNullOrEmpty(this.viewModel?.Jid))
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          this.convo = db.GetConversation(this.viewModel.Jid, CreateOptions.None);
          if (this.convo != null)
          {
            this.UpdateConvo(this.convo);
            this.disposables.Add(db.UpdatedConversationSubject.Select<ConvoAndMessage, Conversation>((Func<ConvoAndMessage, Conversation>) (convoAndMsg => convoAndMsg.Conversation)).Where<Conversation>((Func<Conversation, bool>) (c => c.Jid == this.convo.Jid)).ObserveOnDispatcher<Conversation>().Subscribe<Conversation>(new Action<Conversation>(this.UpdateConvo)));
          }
          else
          {
            Log.l("group settings", "OnNavigatedTo | no conversation found for viewModel.Jid={0}", (object) this.viewModel.Jid);
            this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
          }
        }));
      base.OnNavigatedTo(e);
      if (!string.IsNullOrEmpty(this.viewModel?.Jid))
        return;
      Log.l("group settings", "OnNavigatedTo | view model has no jid: vm={0}", (object) this.viewModel);
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.disposables.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
      this.disposables.Clear();
      base.OnRemovedFromJournal(e);
    }

    private void RestrictPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (string.IsNullOrEmpty(this.viewModel?.Jid))
        Log.l("group settings", "RestictPanel | view model has no jid: vm={0}", (object) this.viewModel);
      else
        GroupRestrictionPicker.Launch(this.viewModel.Jid, this.progress);
    }

    private void AnnouncePanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (string.IsNullOrEmpty(this.viewModel?.Jid))
        Log.l("group settings", "AnnouncePanel | view model has no jid: vm={0}", (object) this.viewModel);
      else
        AnnouncementGroupPicker.Launch(this.viewModel.Jid, this.progress);
    }

    private void UpdateConvo(Conversation conversation)
    {
      this.viewModel.IsLocked = conversation.IsLocked();
      this.viewModel.IsAnnouncementOnly = conversation.IsAnnounceOnly();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/GroupSettingsPage.xaml", UriKind.Relative));
      this.RestrictPanel = (ListBoxItem) this.FindName("RestrictPanel");
      this.RestrictTitleBlock = (TextBlock) this.FindName("RestrictTitleBlock");
      this.RestrictStateBlock = (TextBlock) this.FindName("RestrictStateBlock");
      this.AnnouncePanel = (ListBoxItem) this.FindName("AnnouncePanel");
      this.AnnounceTitleBlock = (TextBlock) this.FindName("AnnounceTitleBlock");
      this.AnnounceStateBlock = (TextBlock) this.FindName("AnnounceStateBlock");
    }
  }
}
