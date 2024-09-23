// Decompiled with JetBrains decompiler
// Type: WhatsApp.AutoDownloadPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace WhatsApp
{
  public class AutoDownloadPage : PhoneApplicationPage
  {
    private List<AutoDownloadPage.AutoDownloadModel> items;
    private static Dictionary<string, PropertyInfo> SettingsProperties = ((IEnumerable<PropertyInfo>) typeof (Settings).GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (propInfo => propInfo.PropertyType == typeof (AutoDownloadSetting))).ToDictionary<PropertyInfo, string>((Func<PropertyInfo, string>) (propInfo => propInfo.Name));
    public readonly IEnumerable<AutoDownloadPage.AutoDownloadSettingModel> AvailableSettings = (IEnumerable<AutoDownloadPage.AutoDownloadSettingModel>) new AutoDownloadPage.AutoDownloadSettingModel[4]
    {
      new AutoDownloadPage.AutoDownloadSettingModel()
      {
        Description = AppResources.AutodownloadNone,
        Flag = AutoDownloadSetting.Disabled
      },
      new AutoDownloadPage.AutoDownloadSettingModel()
      {
        Description = AppResources.AutodownloadOnWifi,
        Flag = AutoDownloadSetting.EnabledOnWifi
      },
      new AutoDownloadPage.AutoDownloadSettingModel()
      {
        Description = AppResources.AutodownloadOnData,
        Flag = AutoDownloadSetting.Enabled
      },
      new AutoDownloadPage.AutoDownloadSettingModel()
      {
        Description = AppResources.AutodownloadWhenRoaming,
        Flag = AutoDownloadSetting.Enabled | AutoDownloadSetting.EnabledWhileRoaming
      }
    };
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal WhatsApp.CompatibilityShims.LongListSelector MainList;
    internal TextBlock LowStorageWarningBlock;
    internal TextBlock PttDownloadExplanationBlock;
    private bool _contentLoaded;

    public AutoDownloadPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.TitlePanel.SmallTitle = AppResources.Settings;
      this.TitlePanel.LargeTitle = AppResources.AutodownloadTitle;
      this.PttDownloadExplanationBlock.Text = string.Format(AppResources.AutodownloadPttExplanation, (object) (Settings.MaxAutodownloadSize / 1048576));
      this.items = new List<AutoDownloadPage.AutoDownloadModel>()
      {
        new AutoDownloadPage.AutoDownloadModel(this.AvailableSettings, AppResources.AutodownloadImages, "AutoDownloadImage"),
        new AutoDownloadPage.AutoDownloadModel(this.AvailableSettings, AppResources.AutodownloadVideo, "AutoDownloadVideo", AppResources.AutodownloadVideoRoamingWarning),
        new AutoDownloadPage.AutoDownloadModel(this.AvailableSettings, AppResources.AutodownloadAudio, "AutoDownloadAudio")
      };
      this.items.Add(new AutoDownloadPage.AutoDownloadModel(this.AvailableSettings, AppResources.AutodownloadDocuments, "AutoDownloadDocument"));
      this.MainList.ItemsSource = (IList) this.items;
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (NativeInterfaces.Misc.GetDiskSpace(Constants.IsoStorePath).FreeBytes / 1024UL / 1024UL >= 100UL)
          return;
        this.LowStorageWarningBlock.Text = AppResources.LowStorageAutoDownloadOff;
        this.LowStorageWarningBlock.Visibility = Visibility.Visible;
      }));
    }

    private void ResetToDefaults_Click(object sender, EventArgs e)
    {
      Settings.DeleteMany((IEnumerable<Settings.Key>) new Settings.Key[4]
      {
        Settings.Key.AutoDownloadSettingAudio,
        Settings.Key.AutoDownloadSettingImage,
        Settings.Key.AutoDownloadSettingVideo,
        Settings.Key.AutoDownloadSettingDocument
      });
      foreach (AutoDownloadPage.AutoDownloadModel autoDownloadModel in this.MainList.ItemsSource.Cast<AutoDownloadPage.AutoDownloadModel>())
        autoDownloadModel.Load();
      IList itemsSource = this.MainList.ItemsSource;
      this.MainList.ItemsSource = (IList) null;
      this.MainList.ItemsSource = itemsSource;
    }

    private void ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count == 0)
        return;
      AutoDownloadPage.AutoDownloadSettingModel addedItem = e.AddedItems[0] as AutoDownloadPage.AutoDownloadSettingModel;
      if (!(sender is FrameworkElement frameworkElement) || addedItem == null || !(frameworkElement.Tag is AutoDownloadPage.AutoDownloadModel tag))
        return;
      tag.Flag = addedItem.Flag;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/AutoDownloadPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.MainList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("MainList");
      this.LowStorageWarningBlock = (TextBlock) this.FindName("LowStorageWarningBlock");
      this.PttDownloadExplanationBlock = (TextBlock) this.FindName("PttDownloadExplanationBlock");
    }

    public class AutoDownloadSettingModel
    {
      public AutoDownloadSetting Flag;

      public string Description { get; set; }
    }

    public class AutoDownloadModel
    {
      private PropertyInfo SettingsProperty;
      private AutoDownloadSetting flag_;
      private string roamingWarning;

      public string Title { get; set; }

      public AutoDownloadSetting Flag
      {
        get => this.flag_;
        set
        {
          if (this.flag_ == value)
            return;
          this.flag_ = value;
          this.OnChanged();
        }
      }

      public IEnumerable<AutoDownloadPage.AutoDownloadSettingModel> AvailableSettings { get; private set; }

      public int InitialSelection
      {
        get
        {
          int initialSelection = 0;
          foreach (AutoDownloadPage.AutoDownloadSettingModel availableSetting in this.AvailableSettings)
          {
            if (this.Flag == availableSetting.Flag)
              return initialSelection;
            ++initialSelection;
          }
          if ((this.Flag & AutoDownloadSetting.EnabledOnData) != AutoDownloadSetting.Disabled)
            this.Flag |= AutoDownloadSetting.EnabledOnWifi;
          else
            this.Flag = (this.Flag & AutoDownloadSetting.EnabledOnWifi) == AutoDownloadSetting.Disabled ? AutoDownloadSetting.Disabled : AutoDownloadSetting.EnabledOnWifi;
          return this.InitialSelection;
        }
      }

      public AutoDownloadModel(
        IEnumerable<AutoDownloadPage.AutoDownloadSettingModel> settings,
        string title,
        string propertyName,
        string roamingWarning = null)
      {
        this.AvailableSettings = settings;
        if (!AutoDownloadPage.SettingsProperties.TryGetValue(propertyName, out this.SettingsProperty))
          throw new Exception("Unknown property: " + propertyName);
        this.Title = title;
        this.Load();
        this.roamingWarning = roamingWarning;
      }

      public void Load()
      {
        this.flag_ = (AutoDownloadSetting) this.SettingsProperty.GetValue((object) null, (object[]) null);
      }

      private void OnChanged()
      {
        if (this.roamingWarning != null && (this.Flag & AutoDownloadSetting.EnabledWhileRoaming) != AutoDownloadSetting.Disabled)
        {
          int num;
          Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => num = (int) MessageBox.Show(this.roamingWarning)));
        }
        AppState.Worker.Enqueue((Action) (() => this.SettingsProperty.SetValue((object) null, (object) this.Flag, (object[]) null)));
      }
    }
  }
}
