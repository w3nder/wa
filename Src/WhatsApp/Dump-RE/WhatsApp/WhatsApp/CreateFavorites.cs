// Decompiled with JetBrains decompiler
// Type: WhatsApp.CreateFavorites
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class CreateFavorites : PhoneApplicationPage
  {
    public static bool allowSkipThisPage;
    private IDisposable statusUpdateSub;
    private IDisposable supressUISub;
    internal Grid LayoutRoot;
    internal StackPanel ContentPanel;
    private bool _contentLoaded;

    public CreateFavorites() => this.InitializeComponent();

    private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
    {
      this.supressUISub = AppState.MuteUIUpdates.Subscribe();
      this.AttemptSync();
    }

    private void AttemptSync()
    {
      Settings.LastFullSyncUtc = new DateTime?();
      ContactStore.EnsureSyncRegComplete((Action<ContactSync.SyncProcessResult>) (ex =>
      {
        if (ex == ContactSync.SyncProcessResult.NoContactsFound)
          ContactSync.SetFullSyncComplete();
        this.Dispatcher.BeginInvoke((Action) (() => this.FailedFirstColdSync()));
      }), (Action) (() => this.Dispatcher.BeginInvoke((Action) (() => this.NavigateOnFavoritesSynced()))));
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.supressUISub.SafeDispose();
      this.supressUISub = (IDisposable) null;
      this.statusUpdateSub.SafeDispose();
      this.statusUpdateSub = (IDisposable) null;
      base.OnRemovedFromJournal(e);
    }

    private void NavigateOnFavoritesSynced()
    {
      this.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.NavigationService.Navigate(new Uri("/PageSelect?ClearStack=true", UriKind.Relative))));
    }

    private void FailedFirstColdSync()
    {
      CreateFavorites.allowSkipThisPage = true;
      this.NavigateOnFavoritesSynced();
      ContactStore.EnsureSyncRegCompleteWithRetry();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/CreateFavorites.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
    }
  }
}
