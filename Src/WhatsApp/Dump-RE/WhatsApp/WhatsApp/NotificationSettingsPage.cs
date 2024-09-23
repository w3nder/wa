// Decompiled with JetBrains decompiler
// Type: WhatsApp.NotificationSettingsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class NotificationSettingsPage : PhoneApplicationPage
  {
    private NotificationSettingsPageViewModel viewModel;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal ZoomBox LayoutRootZoomBox;
    private bool _contentLoaded;

    public NotificationSettingsPage()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.DataContext = (object) (this.viewModel = new NotificationSettingsPageViewModel(this.Orientation));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.viewModel == null)
        return;
      this.viewModel.RefreshNotificationSoundSelections();
    }

    private void MessageToneSelectButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      SelectAlertPage.StartForIndividual();
    }

    private void GroupToneSelectButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      SelectAlertPage.StartForGroup();
    }

    private void CallToneSelectButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      TonePickerPage.Start(Ringtones.LoadRingtones(), Settings.VoipRingtone).ObserveOnDispatcher<Ringtones.Tone>().Subscribe<Ringtones.Tone>((Action<Ringtones.Tone>) (tone =>
      {
        Settings.VoipRingtone = tone.Filepath;
        if (this.viewModel == null)
          return;
        this.viewModel.RefreshRingtoneSelection();
      }));
    }

    private void ResetAll_Click(object sender, RoutedEventArgs e)
    {
      Observable.Return<bool>(true).Decision(AppResources.ResetNotificationSettingsConfirm, AppResources.Reset, AppResources.Cancel).Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (!confirmed)
          return;
        Settings.EnableIndividualAlerts = true;
        Settings.IndividualTone = (string) null;
        Settings.EnableGroupAlerts = true;
        Settings.GroupTone = (string) null;
        Settings.VoipRingtone = (string) null;
        Settings.EnableInAppNotificationToast = true;
        Settings.EnableInAppNotificationSound = true;
        Settings.EnableInAppNotificationVibrate = true;
        Settings.PreviewEnabled = true;
        if (this.viewModel != null)
          this.viewModel.RefreshAll();
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.ResetJidInfoCustomTones()));
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/NotificationSettingsPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
    }
  }
}
