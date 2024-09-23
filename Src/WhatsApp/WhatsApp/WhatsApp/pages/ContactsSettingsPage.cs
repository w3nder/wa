// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactsSettingsPage
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


namespace WhatsApp
{
  public class ContactsSettingsPage : PhoneApplicationPage
  {
    private IDisposable blockListSubscription;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal TextBlock BlockedListLink;
    internal TextBlock BlockedListCount;
    private bool _contentLoaded;

    public ContactsSettingsPage() => this.InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        this.BlockedListCount.Text = Plurals.Instance.GetString(AppResources.BlockListCountPlural, db.BlockListSet.Count);
        if (this.blockListSubscription != null)
          this.blockListSubscription.Dispose();
        this.blockListSubscription = db.BlockListUpdateSubject.ObserveOnDispatcher<IEnumerable<string>>().Subscribe<IEnumerable<string>>((Action<IEnumerable<string>>) (list => this.BlockedListCount.Text = Plurals.Instance.GetString(AppResources.BlockListCountPlural, db.BlockListSet.Count)));
      }));
      base.OnNavigatedTo(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      this.blockListSubscription.Dispose();
      this.blockListSubscription = (IDisposable) null;
    }

    private void OnInviteFriendsTap(object sender, EventArgs e)
    {
      this.NavigationService.Navigate(UriUtils.CreatePageUri("ShareOptions"));
    }

    private void OnBlockedTap(object sender, EventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "BlockListPage");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/ContactsSettingsPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.BlockedListLink = (TextBlock) this.FindName("BlockedListLink");
      this.BlockedListCount = (TextBlock) this.FindName("BlockedListCount");
    }
  }
}
