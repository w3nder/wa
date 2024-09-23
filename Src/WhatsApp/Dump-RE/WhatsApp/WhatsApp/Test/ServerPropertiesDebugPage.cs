// Decompiled with JetBrains decompiler
// Type: WhatsApp.Test.ServerPropertiesDebugPage
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

#nullable disable
namespace WhatsApp.Test
{
  public class ServerPropertiesDebugPage : PhoneApplicationPage
  {
    private IDisposable settingsPropChangedSub;
    private bool isRefreshing;
    internal Grid LayoutRoot;
    internal ProgressBar ProgressIndicator;
    internal WhatsApp.CompatibilityShims.LongListSelector PropList;
    internal TextBlock LastUpdatedBlock;
    internal Button RefreshButton;
    private bool _contentLoaded;

    public ServerPropertiesDebugPage()
    {
      this.InitializeComponent();
      this.PropList.OverlapScrollBar = true;
      this.Show();
    }

    private void Show()
    {
      this.PropList.ItemsSource = (IList) ((IEnumerable<FunEventHandler.ServerPropHandler>) FunEventHandler.ServerPropHandlers).Select(h => new
      {
        PropKey = h.Key,
        PropValue = ""
      }).OrderBy(p => p.PropKey).ToArray();
      this.LastUpdatedBlock.Text = string.Format("last updated: {0}", Settings.LastPropertiesQueryUtc.HasValue ? (object) DateTimeUtils.FunTimeToPhoneTime(Settings.LastPropertiesQueryUtc.Value).ToString() : (object) "n/a");
      this.RefreshButton.Visibility = Visibility.Visible;
      this.ProgressIndicator.Visibility = Visibility.Collapsed;
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.isRefreshing)
        return;
      this.isRefreshing = true;
      this.PropList.ItemsSource = (IList) new object[0];
      this.LastUpdatedBlock.Text = "";
      this.ProgressIndicator.Visibility = Visibility.Visible;
      this.RefreshButton.Visibility = Visibility.Collapsed;
      Settings.ForceServerPropsReload = true;
      AppState.GetConnection().SendGetServerProperties();
      if (this.settingsPropChangedSub != null)
        return;
      this.settingsPropChangedSub = Settings.GetSettingsChangedObservable(new Settings.Key[1]
      {
        Settings.Key.ForceServerPropsReload
      }).Where<Settings.Key>((Func<Settings.Key, bool>) (k => k == Settings.Key.ForceServerPropsReload)).ObserveOnDispatcher<Settings.Key>().Subscribe<Settings.Key>((Action<Settings.Key>) (_ =>
      {
        this.isRefreshing = false;
        AppState.Worker.RunAfterDelay(TimeSpan.FromSeconds(3.0), (Action) (() => this.Dispatcher.BeginInvoke((Action) (() => this.Show()))));
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Test/ServerPropertiesDebugPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ProgressIndicator = (ProgressBar) this.FindName("ProgressIndicator");
      this.PropList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("PropList");
      this.LastUpdatedBlock = (TextBlock) this.FindName("LastUpdatedBlock");
      this.RefreshButton = (Button) this.FindName("RefreshButton");
    }
  }
}
