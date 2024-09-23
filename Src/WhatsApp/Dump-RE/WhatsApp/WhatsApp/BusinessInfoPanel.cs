// Decompiled with JetBrains decompiler
// Type: WhatsApp.BusinessInfoPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class BusinessInfoPanel : UserControl
  {
    private BusinessInfoViewModel viewModel;
    private MapPoint locationMapPoint;
    internal Grid VerifiedStatePanel;
    internal JidNameControl ExtraVerifiedName;
    internal TextBlock VerifiedStateBlock;
    internal Grid HoursPanel;
    internal Image HoursIcon;
    internal Grid TodayHoursPanel;
    internal TextBlock TodayDayBlock;
    internal TextBlock TodayHoursBlock;
    internal ScaleTransform ToggleHoursIconTranform;
    internal ItemsControl OtherDaysHoursList;
    internal Grid AddressPanel;
    internal Image AddressIcon;
    internal TextBlock AddressBlock;
    internal MapControl MapView;
    internal Grid CategoryPanel;
    internal Image CategoryIcon;
    internal TextBlock CategoryBlock;
    internal Grid DescriptionPanel;
    internal Image DescriptionIcon;
    internal RichTextBlock DescriptionBlock;
    internal Grid EmailPanel;
    internal Image EmailIcon;
    internal TextBlock EmailBlock;
    internal Grid InstagramPanel;
    internal Image InstagramIcon;
    internal TextBlock InstagramBlock;
    internal ListBoxItem WebsitesPanel;
    internal Image WebsitesIcon;
    internal StackPanel WebsiteLinksPanel;
    private bool _contentLoaded;

    public BusinessInfoPanel()
    {
      Log.d(nameof (BusinessInfoPanel), "Initalizing");
      this.InitializeComponent();
      Log.d(nameof (BusinessInfoPanel), "Initalized");
    }

    public static bool ShouldDisplayPanelForJid(string jid)
    {
      UserStatus user = UserCache.Get(jid, true);
      return user != null && user.IsVerified();
    }

    public void Render(string jid)
    {
      UserStatus bizUser = UserCache.Get(jid, true);
      if (!BusinessInfoPanel.ShouldDisplayPanelForJid(jid))
        return;
      this.viewModel.SafeDispose();
      this.DataContext = (object) (this.viewModel = new BusinessInfoViewModel(bizUser));
      Log.l("biz", "v level: {0}, v name: {1}, name: {2}", (object) bizUser.VerifiedLevel, (object) (bizUser.VerifiedNameCertificateDetails?.VerifiedName ?? ""), (object) bizUser.ContactName);
      this.Render();
      this.Visibility = Visibility.Visible;
    }

    private void Render()
    {
      if (this.viewModel == null)
        return;
      bool checkMark = false;
      string secondInfoName = this.viewModel.BizUser != null ? VerifiedNameRules.GetSecondInfoName(this.viewModel.BizUser, out checkMark) : (string) null;
      if (secondInfoName != null)
      {
        this.ExtraVerifiedName.Visibility = Visibility.Visible;
        this.ExtraVerifiedName.Set(RichTextBlock.TextSet.Create(secondInfoName), checkMark);
      }
      else
        this.ExtraVerifiedName.Visibility = Visibility.Collapsed;
      this.VerifiedStateBlock.Foreground = this.viewModel.VerifiedStateForeground;
      this.VerifiedStateBlock.FontWeight = this.viewModel.VerifiedStateFontWeight;
      this.VerifiedStateBlock.Text = this.viewModel.VerifiedStateText;
      this.HoursPanel.Visibility = this.viewModel.HoursVisibility;
      List<KeyValuePair<string, string>> schedule = this.viewModel.GetSchedule(true);
      if (schedule.Any<KeyValuePair<string, string>>())
      {
        KeyValuePair<string, string> keyValuePair = schedule[0];
        this.TodayDayBlock.Text = keyValuePair.Key;
        this.TodayHoursBlock.Text = keyValuePair.Value;
        this.OtherDaysHoursList.ItemsSource = (IEnumerable) schedule.Skip<KeyValuePair<string, string>>(1).ToList<KeyValuePair<string, string>>();
      }
      this.WebsitesPanel.Visibility = this.viewModel.WebsitesVisibility;
      this.WebsiteLinksPanel.Children.Clear();
      if (this.WebsitesPanel.Visibility == Visibility.Visible)
      {
        foreach (string link in this.viewModel.Links)
        {
          if (link != null && link.Trim().Length > 0)
          {
            string auxiliaryInfo = "https://l.wl.co/l?u=" + HttpUtility.UrlEncode(LinkDetector.InferUriScheme(link.Trim()));
            UIElementCollection children = this.WebsiteLinksPanel.Children;
            RichTextBlock richTextBlock1 = new RichTextBlock();
            richTextBlock1.AllowLinks = true;
            richTextBlock1.AllowTextFormatting = false;
            richTextBlock1.FontSize = 24.0;
            richTextBlock1.Foreground = (Brush) UIUtils.ForegroundBrush;
            richTextBlock1.Margin = new Thickness(-12.0, 0.0, -12.0, 0.0);
            richTextBlock1.Text = new RichTextBlock.TextSet()
            {
              Text = link,
              SerializedFormatting = (IEnumerable<LinkDetector.Result>) new LinkDetector.Result[1]
              {
                new LinkDetector.Result(0, link.Length, 1, link, auxiliaryInfo)
              }
            };
            richTextBlock1.TextWrapping = TextWrapping.Wrap;
            RichTextBlock richTextBlock2 = richTextBlock1;
            children.Add((UIElement) richTextBlock2);
          }
        }
      }
      this.DescriptionPanel.Visibility = this.viewModel.DescriptionVisibility;
      this.DescriptionBlock.Text = this.viewModel.DescriptionText;
      this.AddressPanel.Visibility = this.viewModel.AddressVisibility;
      this.AddressBlock.Text = this.viewModel.AddressText;
      GeoCoordinate locationCoordinate = this.viewModel.LocationCoordinate;
      if (locationCoordinate != (GeoCoordinate) null)
      {
        if (this.locationMapPoint == null)
          this.locationMapPoint = this.MapView.AddPoint(MapPointStyle.Place);
        this.locationMapPoint.SetCoordinate(locationCoordinate);
        (this.locationMapPoint.Element() as MapPlacePin).Expand();
        this.MapView.Center = locationCoordinate;
        this.MapView.Visibility = Visibility.Visible;
      }
      this.CategoryPanel.Visibility = this.viewModel.CategoryVisibility;
      this.CategoryBlock.Text = this.viewModel.CategoryText;
      this.EmailPanel.Visibility = this.viewModel.EmailVisibility;
      this.EmailBlock.Text = this.viewModel.EmailText;
      this.InstagramPanel.Visibility = this.viewModel.InstagramVisibility;
      this.InstagramBlock.Text = this.viewModel.InstagramAccount;
    }

    private void VerifiedStatePanel_Tap(object sender, GestureEventArgs e)
    {
      UserStatus bizUser = this.viewModel?.BizUser;
      if (bizUser == null)
        return;
      Pair<string, string> verifiedState2Tier = SystemMessageUtils.GetPopupMessageForVerifiedState2Tier(bizUser.Jid, bizUser.VerifiedLevel, bizUser.GetVerifiedNameForDisplay(), VerifiedNamesCertifier.ConvertVerifiedLevel(bizUser.VerifiedLevel), bizUser.IsInDevicePhonebook, bizUser.VerifiedNameMatchesContactName());
      UIUtils.ShowMessageBoxWithGeneralLearnMore(verifiedState2Tier.First, verifiedState2Tier.Second);
    }

    private void InstagramPanel_Tap(object sender, GestureEventArgs e)
    {
      string instagramLink = this.viewModel.InstagramLink;
      if (string.IsNullOrEmpty(instagramLink))
        return;
      new WebBrowserTask() { Uri = new Uri(instagramLink) }.Show();
      FieldStats.ReportBizProfileAction(this.viewModel?.BizUser?.Jid, wam_enum_view_business_profile_action.ACTION_CLICK_WEBSITE, new wam_enum_website_source_type?(wam_enum_website_source_type.SOURCE_INSTAGRAM));
    }

    private void AddressPanel_Tap(object sender, GestureEventArgs e)
    {
      string name = this.viewModel.BizUser.GetDisplayName();
      Message.PlaceDetails placeDetails = new Message.PlaceDetails()
      {
        Name = name,
        Address = this.viewModel.AddressText
      };
      this.Dispatcher.BeginInvoke((Action) (() => LocationView.Start(name, this.viewModel.LocationCoordinate, placeDetails, true)));
      FieldStats.ReportBizProfileAction(this.viewModel?.BizUser?.Jid, wam_enum_view_business_profile_action.ACTION_CLICK_LOCATION);
    }

    private void HoursPanel_Tap(object sender, GestureEventArgs e)
    {
      bool flag = this.OtherDaysHoursList.Visibility == Visibility.Collapsed;
      this.OtherDaysHoursList.Visibility = flag.ToVisibility();
      this.ToggleHoursIconTranform.ScaleY = flag ? -1.0 : 1.0;
      FieldStats.ReportBizProfileAction(this.viewModel?.BizUser?.Jid, wam_enum_view_business_profile_action.ACTION_CLICK_HOURS);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/BusinessInfoPanel.xaml", UriKind.Relative));
      this.VerifiedStatePanel = (Grid) this.FindName("VerifiedStatePanel");
      this.ExtraVerifiedName = (JidNameControl) this.FindName("ExtraVerifiedName");
      this.VerifiedStateBlock = (TextBlock) this.FindName("VerifiedStateBlock");
      this.HoursPanel = (Grid) this.FindName("HoursPanel");
      this.HoursIcon = (Image) this.FindName("HoursIcon");
      this.TodayHoursPanel = (Grid) this.FindName("TodayHoursPanel");
      this.TodayDayBlock = (TextBlock) this.FindName("TodayDayBlock");
      this.TodayHoursBlock = (TextBlock) this.FindName("TodayHoursBlock");
      this.ToggleHoursIconTranform = (ScaleTransform) this.FindName("ToggleHoursIconTranform");
      this.OtherDaysHoursList = (ItemsControl) this.FindName("OtherDaysHoursList");
      this.AddressPanel = (Grid) this.FindName("AddressPanel");
      this.AddressIcon = (Image) this.FindName("AddressIcon");
      this.AddressBlock = (TextBlock) this.FindName("AddressBlock");
      this.MapView = (MapControl) this.FindName("MapView");
      this.CategoryPanel = (Grid) this.FindName("CategoryPanel");
      this.CategoryIcon = (Image) this.FindName("CategoryIcon");
      this.CategoryBlock = (TextBlock) this.FindName("CategoryBlock");
      this.DescriptionPanel = (Grid) this.FindName("DescriptionPanel");
      this.DescriptionIcon = (Image) this.FindName("DescriptionIcon");
      this.DescriptionBlock = (RichTextBlock) this.FindName("DescriptionBlock");
      this.EmailPanel = (Grid) this.FindName("EmailPanel");
      this.EmailIcon = (Image) this.FindName("EmailIcon");
      this.EmailBlock = (TextBlock) this.FindName("EmailBlock");
      this.InstagramPanel = (Grid) this.FindName("InstagramPanel");
      this.InstagramIcon = (Image) this.FindName("InstagramIcon");
      this.InstagramBlock = (TextBlock) this.FindName("InstagramBlock");
      this.WebsitesPanel = (ListBoxItem) this.FindName("WebsitesPanel");
      this.WebsitesIcon = (Image) this.FindName("WebsitesIcon");
      this.WebsiteLinksPanel = (StackPanel) this.FindName("WebsiteLinksPanel");
    }
  }
}
