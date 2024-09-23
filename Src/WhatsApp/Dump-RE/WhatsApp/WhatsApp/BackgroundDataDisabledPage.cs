// Decompiled with JetBrains decompiler
// Type: WhatsApp.BackgroundDataDisabledPage
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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WhatsAppNative;
using Windows.Networking.Connectivity;

#nullable disable
namespace WhatsApp
{
  public class BackgroundDataDisabledPage : PhoneApplicationPage
  {
    private Action deepLink;
    private IDisposable notificationBlockSub;
    private static IDisposable invalidateCacheSub;
    private static BackgroundDataDisabledPage.Status? cachedForBlue;
    private static bool? hasDataSense;
    internal Grid LayoutRoot;
    internal Image Image;
    internal StackPanel ContentPanel;
    internal TextBlock DescriptionTextBlock;
    internal Button SettingsButton;
    private bool _contentLoaded;

    public BackgroundDataDisabledPage()
    {
      this.InitializeComponent();
      this.Image.Source = (System.Windows.Media.ImageSource) new BitmapImage()
      {
        UriSource = new Uri(string.Format("/Images/icon-bgdata-restricted-{0}.png", ImageStore.IsDarkTheme() ? (object) "black" : (object) "white"), UriKind.Relative)
      };
      this.notificationBlockSub = AppState.MuteUIUpdates.Subscribe();
    }

    private static BackgroundDataDisabledPage.Status CurrentStatus
    {
      get
      {
        if (!AppState.IsWP81Gdr1OrLater)
          return BackgroundDataDisabledPage.StatusForBlue;
        BackgroundDataDisabledPage.Status currentStatus = NativeInterfaces.Misc.IsBackgroundDataDisabled() ? BackgroundDataDisabledPage.Status.AlwaysDisabled : BackgroundDataDisabledPage.Status.NotApplicable;
        if (currentStatus == BackgroundDataDisabledPage.Status.NotApplicable && BackgroundDataDisabledPage.OSBuildHasDataSense && NativeInterfaces.Misc.IsBackgroundDataDisabledNearLimit() && !NetworkStateMonitor.IsWifiDataConnected())
        {
          currentStatus = BackgroundDataDisabledPage.StatusForBlue;
          if (currentStatus != BackgroundDataDisabledPage.Status.NotApplicable)
            currentStatus = BackgroundDataDisabledPage.Status.NearLimit;
        }
        return currentStatus;
      }
    }

    public static bool Applicable => BackgroundDataDisabledPage.CurrentStatus != 0;

    private static BackgroundDataDisabledPage.Status StatusForBlue
    {
      get
      {
        BackgroundDataDisabledPage.Status statusForBlue1 = BackgroundDataDisabledPage.Status.NotApplicable;
        BackgroundDataDisabledPage.Status? cachedForBlue = BackgroundDataDisabledPage.cachedForBlue;
        if (cachedForBlue.HasValue)
          return cachedForBlue.Value;
        if (!BackgroundDataDisabledPage.OSBuildHasDataSense)
        {
          BackgroundDataDisabledPage.cachedForBlue = new BackgroundDataDisabledPage.Status?(statusForBlue1);
          return statusForBlue1;
        }
        DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
        IEnumerable<ConnectionProfile> connectionProfiles = (IEnumerable<ConnectionProfile>) null;
        if (NetworkStateMonitor.IsWifiDataConnected())
        {
          try
          {
            connectionProfiles = (IEnumerable<ConnectionProfile>) NetworkInformation.GetConnectionProfiles();
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "get connection profiles");
          }
        }
        if (connectionProfiles == null)
        {
          try
          {
            connectionProfiles = (IEnumerable<ConnectionProfile>) new ConnectionProfile[1]
            {
              NetworkInformation.GetInternetConnectionProfile()
            };
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "get main connection profile");
          }
        }
        BackgroundDataDisabledPage.Status statusForBlue2 = ((IEnumerable<ConnectionProfile>) ((object) connectionProfiles ?? (object) new ConnectionProfile[0])).Where<ConnectionProfile>((Func<ConnectionProfile, bool>) (prof =>
        {
          try
          {
            if (prof != null)
            {
              ConnectionCost connectionCost;
              if ((connectionCost = prof.GetConnectionCost()) != null)
              {
                if (connectionCost.NetworkCostType != null)
                {
                  if (connectionCost.NetworkCostType != 1)
                    return (connectionCost.ApproachingDataLimit || connectionCost.OverDataLimit) && !connectionCost.Roaming;
                }
              }
            }
          }
          catch (Exception ex)
          {
            string context = string.Format("check profile{0}", prof == null || prof.ProfileName == null ? (object) "" : (object) (" " + prof.ProfileName));
            Log.LogException(ex, context);
          }
          return false;
        })).Any<ConnectionProfile>() ? BackgroundDataDisabledPage.Status.NotWorkingButUnsureWhy : BackgroundDataDisabledPage.Status.NotApplicable;
        PerformanceTimer.End("check for bg data", start);
        BackgroundDataDisabledPage.cachedForBlue = new BackgroundDataDisabledPage.Status?(statusForBlue2);
        if (BackgroundDataDisabledPage.invalidateCacheSub == null)
          BackgroundDataDisabledPage.invalidateCacheSub = App.ApplicationActivatedSubject.Subscribe<Unit>((Action<Unit>) (_ => BackgroundDataDisabledPage.cachedForBlue = new BackgroundDataDisabledPage.Status?()));
        return statusForBlue2;
      }
    }

    private static bool OSBuildHasDataSense
    {
      get
      {
        if (!BackgroundDataDisabledPage.hasDataSense.HasValue)
        {
          BackgroundDataDisabledPage.hasDataSense = new bool?(false);
          IRegHelper regHelper = AppState.RegHelper;
          try
          {
            BackgroundDataDisabledPage.hasDataSense = new bool?(regHelper.ReadDWord(2147483650U, "SOFTWARE\\Microsoft\\Data Sense", "DSEnabled") > 0U);
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "query data sense");
          }
        }
        return BackgroundDataDisabledPage.hasDataSense ?? false;
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      BackgroundDataDisabledPage.Status currentStatus = BackgroundDataDisabledPage.CurrentStatus;
      if (currentStatus == BackgroundDataDisabledPage.Status.NotApplicable)
      {
        this.NavigationService.Navigate(new Uri("/PageSelect?PageReplace=true", UriKind.Relative));
      }
      else
      {
        if (this.deepLink != null)
          return;
        List<string> stringList = new List<string>();
        string deepLinkUrl = (string) null;
        string str1;
        string str2;
        if (BackgroundDataDisabledPage.OSBuildHasDataSense)
        {
          switch (currentStatus)
          {
            case BackgroundDataDisabledPage.Status.AlwaysDisabled:
            case BackgroundDataDisabledPage.Status.NearLimit:
              str1 = AppResources.BGDataDisabledWithDataSense;
              stringList.Add(AppResources.BGDataDisabledWithDataSenseArg);
              break;
            default:
              str1 = AppResources.BGDataDisabledRemoveLimit;
              stringList.Add(AppResources.BGDataDisabledNearLimitArg);
              stringList.Add(AppResources.BGDataDisabledRemoveLimitArg1);
              break;
          }
          deepLinkUrl = "app://5B04B775-356B-4AA0-AAF8-6491FFEA5646/Settings";
          str2 = AppResources.DataSenseSettings;
        }
        else
        {
          str1 = AppResources.BGDataDisabled;
          stringList.Add(AppResources.BGDataDisabledArg);
          deepLinkUrl = "ms-settings-cellular:";
          str2 = AppResources.SettingsTitle;
        }
        this.deepLink = (Action) (() => NavUtils.NavigateExternal(deepLinkUrl));
        foreach (Utils.FormatResult formatResult in Utils.Format(str1, stringList.ToArray()))
        {
          Run run = new Run() { Text = formatResult.Value };
          if (formatResult.Index.HasValue)
            run.Foreground = this.Resources[(object) "PhoneAccentBrush"] as Brush;
          this.DescriptionTextBlock.Inlines.Add((Inline) run);
        }
        this.SettingsButton.Content = (object) str2;
      }
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      this.notificationBlockSub.SafeDispose();
      this.notificationBlockSub = (IDisposable) null;
      BackgroundDataDisabledPage.invalidateCacheSub.SafeDispose();
      BackgroundDataDisabledPage.invalidateCacheSub = (IDisposable) null;
    }

    private void Settings_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.deepLink == null)
        return;
      this.deepLink();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/BackgroundDataDisabledPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Image = (Image) this.FindName("Image");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.DescriptionTextBlock = (TextBlock) this.FindName("DescriptionTextBlock");
      this.SettingsButton = (Button) this.FindName("SettingsButton");
    }

    private enum Status
    {
      NotApplicable,
      AlwaysDisabled,
      NearLimit,
      NotWorkingButUnsureWhy,
    }
  }
}
