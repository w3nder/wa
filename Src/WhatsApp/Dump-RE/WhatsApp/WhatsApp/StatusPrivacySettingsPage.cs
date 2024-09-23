// Decompiled with JetBrains decompiler
// Type: WhatsApp.StatusPrivacySettingsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace WhatsApp
{
  public class StatusPrivacySettingsPage : PhoneApplicationPage
  {
    private List<string> statusPickerItems;
    private StatusPrivacySettingPickerWrapper pickerWrapper;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal ListPicker StatusPicker;
    internal TextBlock StatusTooltip;
    private bool _contentLoaded;

    public StatusPrivacySettingsPage()
    {
      this.InitializeComponent();
      this.TitlePanel.SmallTitle = AppResources.PrivacySettingsTitle;
      this.TitlePanel.LargeTitle = AppResources.StatusV3Lower;
      this.StatusTooltip.Text = AppResources.StatusPrivacyTooltip;
      this.pickerWrapper = new StatusPrivacySettingPickerWrapper(this.StatusPicker);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/StatusPrivacySettingsPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.StatusPicker = (ListPicker) this.FindName("StatusPicker");
      this.StatusTooltip = (TextBlock) this.FindName("StatusTooltip");
    }
  }
}
