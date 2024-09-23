// Decompiled with JetBrains decompiler
// Type: WhatsApp.NotificationSettingsPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class NotificationSettingsPageViewModel : PageViewModelBase
  {
    private CustomTones.Tone[] tones;

    public Visibility DebugOnlyVisibility => UIUtils.DebugOnlyVisibility;

    public string SmallTitleStr => AppResources.SettingsTitle;

    public string LargeTitleStr => AppResources.CustomAlertsTitle;

    public string MessageSectionTitleStr => AppResources.MessageNotificationSettingsTitle;

    public bool EnableIndividualAlerts
    {
      get => Settings.EnableIndividualAlerts;
      set
      {
        Settings.EnableIndividualAlerts = value;
        this.NotifyPropertyChanged("EnableMessageToneSelectButton");
      }
    }

    public string MessageToneSelectionStr => AppResources.CustomAlertsIndividual;

    public string MessageToneName => this.GetToneName(Settings.IndividualTone);

    public bool EnableMessageToneSelectButton => Settings.EnableIndividualAlerts;

    public string GroupSectionTitleStr => AppResources.GroupNotificationSettingsTitle;

    public string GroupToneSelectionStr => AppResources.CustomAlertsGroup;

    public string GroupToneName => this.GetToneName(Settings.GroupTone);

    public bool EnableGroupToneSelectButton => Settings.EnableGroupAlerts;

    public bool EnableGroupAlerts
    {
      get => Settings.EnableGroupAlerts;
      set
      {
        Settings.EnableGroupAlerts = value;
        this.NotifyPropertyChanged("EnableGroupToneSelectButton");
      }
    }

    public string CallSectionTitleStr => AppResources.CallTonesSettingsTitle;

    public string CallToneSelectionStr => AppResources.CustomToneForCalls;

    public string VoipRingtoneName => Ringtones.GetRingtoneName();

    public string InAppSectionTitleStr => AppResources.InAppNotificationSettingsTitle;

    public string EnableInAppNotificationToastStr
    {
      get => AppResources.InAppNotificationToastsToggleHeader;
    }

    public bool EnableInAppNotificationToast
    {
      get => Settings.EnableInAppNotificationToast;
      set => Settings.EnableInAppNotificationToast = value;
    }

    public string EnableInAppNotificationSoundStr
    {
      get => AppResources.InAppNotificationSoundsToggleHeader;
    }

    public bool EnableInAppNotificationSound
    {
      get => Settings.EnableInAppNotificationSound;
      set => Settings.EnableInAppNotificationSound = value;
    }

    public string EnableInAppNotificationVibrateStr
    {
      get => AppResources.InAppNotificationVibrateToggleHeader;
    }

    public bool EnableInAppNotificationVibrate
    {
      get => Settings.EnableInAppNotificationVibrate;
      set => Settings.EnableInAppNotificationVibrate = value;
    }

    public string PreviewSectionTitleStr => AppResources.MessagePreviewNotificationSettingsTitle;

    public string ShowPreviewInNotificationsStr => AppResources.PreviewSetting;

    public bool ShowPreviewInNotifications
    {
      get => Settings.PreviewEnabled;
      set => Settings.PreviewEnabled = value;
    }

    public string ResetButtonLabelStr => AppResources.ResetNotificationSettingsButton;

    public string ResetButtonTooltipStr
    {
      get => AppResources.ResetNotificationSettingsButtonTooltipIncludingPerChat;
    }

    public NotificationSettingsPageViewModel(PageOrientation initialOrientation)
      : base(initialOrientation)
    {
      this.tones = CustomTones.ListAlerts();
    }

    public string GetToneName(string toneFile)
    {
      string str = (string) null;
      if (toneFile != null)
        str = ((IEnumerable<CustomTones.Tone>) this.tones).Where<CustomTones.Tone>((Func<CustomTones.Tone, bool>) (t => t.Path == toneFile)).Select<CustomTones.Tone, string>((Func<CustomTones.Tone, string>) (t => t.Title)).FirstOrDefault<string>();
      return str ?? CustomTones.DefaultAlertName;
    }

    public void RefreshNotificationSoundSelections()
    {
      this.NotifyPropertyChanged("MessageToneName");
      this.NotifyPropertyChanged("GroupToneName");
    }

    public void RefreshRingtoneSelection() => this.NotifyPropertyChanged("VoipRingtoneName");

    public void RefreshAll()
    {
      this.NotifyPropertyChanged("EnableIndividualAlerts");
      this.NotifyPropertyChanged("EnableMessageToneSelectButton");
      this.NotifyPropertyChanged("EnableGroupAlerts");
      this.NotifyPropertyChanged("EnableGroupToneSelectButton");
      this.RefreshNotificationSoundSelections();
      this.RefreshRingtoneSelection();
      this.NotifyPropertyChanged("EnableInAppNotificationToast");
      this.NotifyPropertyChanged("EnableInAppNotificationSound");
      this.NotifyPropertyChanged("EnableInAppNotificationVibrate");
      this.NotifyPropertyChanged("ShowPreviewInNotifications");
    }
  }
}
