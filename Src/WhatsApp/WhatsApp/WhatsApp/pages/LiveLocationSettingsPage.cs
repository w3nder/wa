// Decompiled with JetBrains decompiler
// Type: WhatsApp.LiveLocationSettingsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace WhatsApp
{
  public class LiveLocationSettingsPage : PhoneApplicationPage
  {
    internal ZoomBox RootZoomBox;
    internal StackPanel LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal TextBlock SharingStatus;
    internal WhatsApp.CompatibilityShims.LongListSelector MainList;
    internal TextBlock ExplanationBlock;
    internal TextBlock StopSharing;
    private bool _contentLoaded;

    public IEnumerable<LiveLocationSettingsModel> SharingJids { get; private set; }

    public LiveLocationSettingsPage()
    {
      this.InitializeComponent();
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.TitlePanel.SmallTitle = AppResources.Settings;
      this.TitlePanel.LargeTitle = AppResources.LiveLocationLowerCase;
      this.ExplanationBlock.Text = AppResources.LiveLocationPrivacyExplanation;
      this.UpdateComponents();
    }

    private void UpdateComponents()
    {
      Dictionary<string, Tuple<double, List<string>>> jidsSendingTo = LiveLocationManager.Instance.GetJidsSendingTo();
      if (jidsSendingTo.Count > 0)
      {
        this.SharingStatus.Text = Plurals.Instance.GetString(AppResources.LiveLocationCurrentlySharingPlural, jidsSendingTo.Count);
        List<LiveLocationSettingsModel> locationSettingsModelList = new List<LiveLocationSettingsModel>();
        foreach (string key in jidsSendingTo.Keys)
          locationSettingsModelList.Add(new LiveLocationSettingsModel()
          {
            Title = this.GetTitleForJid(key),
            Expiration = DateTimeUtils.FormatLiveLocationTimeLeft((long) jidsSendingTo[key].Item1)
          });
        this.MainList.ItemsSource = (IList) locationSettingsModelList;
        this.MainList.Visibility = Visibility.Visible;
        this.StopSharing.Text = Plurals.Instance.GetString(AppResources.LiveLocationStopSharingLabelPlural, jidsSendingTo.Count);
        this.StopSharing.Visibility = Visibility.Visible;
      }
      else
      {
        this.SharingStatus.Text = AppResources.LiveLocationNotSharing;
        this.MainList.Visibility = Visibility.Collapsed;
        this.StopSharing.Visibility = Visibility.Collapsed;
      }
    }

    private RichTextBlock.TextSet GetTitleForJid(string jid)
    {
      Conversation convo = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db2 => convo = db2.GetConversation(jid, CreateOptions.None)));
      return new RichTextBlock.TextSet()
      {
        Text = convo.GetName(true)
      };
    }

    private void StopSharing_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      UIUtils.MessageBox(" ", AppResources.LiveLocationStopSharing, (IEnumerable<string>) new string[2]
      {
        AppResources.Cancel,
        AppResources.LiveLocationStop
      }, (Action<int>) (idx =>
      {
        if (idx != 1)
          return;
        Settings.LiveLocationIsNewUser = true;
        LiveLocationManager.Instance.DisableAllLocationSharing();
        this.UpdateComponents();
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/LiveLocationSettingsPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (StackPanel) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.SharingStatus = (TextBlock) this.FindName("SharingStatus");
      this.MainList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("MainList");
      this.ExplanationBlock = (TextBlock) this.FindName("ExplanationBlock");
      this.StopSharing = (TextBlock) this.FindName("StopSharing");
    }
  }
}
