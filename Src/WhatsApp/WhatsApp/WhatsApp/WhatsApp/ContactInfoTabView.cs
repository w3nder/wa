// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactInfoTabView
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WhatsApp.CommonOps;
using WhatsApp.CompatibilityShims;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ContactInfoTabView : UserControl, IDisposable
  {
    private ContactInfoTabViewModel viewModel;
    private bool isShown;
    private IDisposable vmSub;
    private IDisposable vmPropSub;
    private BusinessInfoPanel businessInfoPanel;
    internal ZoomBox RootZoomBox;
    internal LongListSelector MainList;
    internal StackPanel HeaderPanel;
    internal Image ProfileImage;
    internal TextBlock WarningBlock;
    internal ProgressBar LoadingProgressBar;
    internal ItemsControl WaNumberList;
    internal ChatInfoTabHeader TabHeader;
    internal StackPanel FooterPanel;
    internal ItemsControl ActionList;
    internal ItemsControl InfoList;
    private bool _contentLoaded;

    public ContactInfoTabView()
    {
      this.InitializeComponent();
      this.MainList.OverlapScrollBar = true;
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
    }

    public void PresetPicture(System.Windows.Media.ImageSource startingPic, ContactInfoPageData data)
    {
      if (this.viewModel == null && data != null)
        this.viewModel = new ContactInfoTabViewModel(data, startingPic);
      if (this.viewModel == null)
        return;
      this.viewModel.InitContactPictureSubscription(data);
    }

    public void Show(ContactInfoPageData data)
    {
      if (this.isShown || data == null)
        return;
      this.isShown = true;
      this.DataContext = (object) (this.viewModel ?? (this.viewModel = new ContactInfoTabViewModel(data)));
      ContactInfoTabViewModel.IsVoipEnabled = true;
      this.MainList.ItemsSource = (IList) this.viewModel.CommonGroupsSource;
      this.vmSub = this.viewModel.GetObservable().SubscribeOn<KeyValuePair<string, object>>((IScheduler) AppState.Worker).ObserveOnDispatcher<KeyValuePair<string, object>>().Subscribe<KeyValuePair<string, object>>((Action<KeyValuePair<string, object>>) (p =>
      {
        if (this.viewModel == null)
          return;
        if (p.Key == "ChatDataLoaded")
          this.OnChatDataLoaded();
        else if (p.Key == "JidInfoUpdated")
        {
          this.UpdateChatInfo();
        }
        else
        {
          if (!(p.Key == "BizDataUpdated"))
            return;
          this.UpdateBusinessInfo();
        }
      }));
      this.vmPropSub = this.viewModel.GetPropertyChangedAsync().ObserveOnDispatcher<PropertyChangedEventArgs>().Select<PropertyChangedEventArgs, string>((Func<PropertyChangedEventArgs, string>) (args => args.PropertyName)).Subscribe<string>((Action<string>) (p =>
      {
        if (this.viewModel == null)
          return;
        switch (p)
        {
          case "ProfilePicSource":
            this.ProfileImage.Source = this.viewModel.ProfilePicSource;
            break;
          case "WarningVisibility":
            this.WarningBlock.Visibility = this.viewModel.WarningVisibility;
            break;
          case "WarningStr":
            this.WarningBlock.Text = this.viewModel.WarningStr;
            break;
          case "WaNumberListSource":
            this.WaNumberList.ItemsSource = (IEnumerable) this.viewModel.WaNumberListSource;
            break;
          case "ActionListSource":
            this.ActionList.ItemsSource = (IEnumerable) this.viewModel.ActionListSource;
            break;
          case "InfoListSource":
            this.InfoList.ItemsSource = (IEnumerable) this.viewModel.InfoListSource;
            break;
        }
      }));
      this.ProfileImage.Source = this.viewModel.ProfilePicSource;
      this.WaNumberList.ItemsSource = (IEnumerable) this.viewModel.WaNumberListSource;
      this.ActionList.ItemsSource = (IEnumerable) this.viewModel.ActionListSource;
      this.InfoList.ItemsSource = (IEnumerable) this.viewModel.InfoListSource;
      if (data.IsChatDataLoaded)
        this.OnChatDataLoaded();
      this.UpdateBusinessInfo();
    }

    public void Dispose()
    {
      this.vmSub.SafeDispose();
      this.vmPropSub.SafeDispose();
      this.vmSub = this.vmPropSub = (IDisposable) null;
      this.viewModel.SafeDispose();
      this.viewModel = (ContactInfoTabViewModel) null;
    }

    private void UpdateBusinessInfo()
    {
      if (this.viewModel == null)
        return;
      string jid = this.viewModel.Data.TargetWaAccount == null ? (string) null : this.viewModel.Data.TargetWaAccount.Jid;
      if (jid != null && BusinessInfoPanel.ShouldDisplayPanelForJid(jid))
      {
        if (this.businessInfoPanel == null)
        {
          BusinessInfoPanel businessInfoPanel = new BusinessInfoPanel();
          businessInfoPanel.Margin = new Thickness(0.0, 24.0, 0.0, 0.0);
          this.businessInfoPanel = businessInfoPanel;
          this.HeaderPanel.Children.Insert(this.HeaderPanel.Children.IndexOf((UIElement) this.LoadingProgressBar) + 1, (UIElement) this.businessInfoPanel);
        }
        this.businessInfoPanel.Render(jid);
      }
      else
      {
        if (this.businessInfoPanel == null)
          return;
        this.businessInfoPanel.Visibility = Visibility.Collapsed;
      }
    }

    private void UpdateChatInfo()
    {
      if (this.viewModel == null)
        return;
      string jid = this.viewModel.Data.TargetWaAccount == null ? (string) null : this.viewModel.Data.TargetWaAccount.Jid;
      this.TabHeader.Set(this.viewModel.Data.JidInfos, this.viewModel.Data.GetDisplayName(), jid);
    }

    private void OnChatDataLoaded()
    {
      this.LoadingProgressBar.Visibility = Visibility.Collapsed;
      this.FooterPanel.Visibility = Visibility.Visible;
      this.UpdateChatInfo();
    }

    private void WaNumber_Tap(object sender, GestureEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is ContactInfoTabViewModel.WaNumberViewModel tag) || tag.Jid == null)
        return;
      NavUtils.NavigateToChat(tag.Jid, true);
    }

    private void ChatButton_Click(object sender, GestureEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is ContactInfoTabViewModel.WaNumberViewModel tag) || tag.Jid == null)
        return;
      NavUtils.NavigateToChat(tag.Jid, true);
    }

    private void CallButton_Click(object sender, GestureEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is ContactInfoTabViewModel.WaNumberViewModel tag) || tag.Jid == null)
        return;
      CallContact.Call(tag.Jid, context: "from contact page info tab");
    }

    private void VideoCall_Click(object sender, GestureEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is ContactInfoTabViewModel.WaNumberViewModel tag) || tag.Jid == null)
        return;
      CallContact.VideoCall(tag.Jid);
    }

    private void ActionItem_Tap(object sender, GestureEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is ContactInfoPageData.ContactInfoItem tag))
        return;
      tag.Act();
    }

    private void MainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ChatItemViewModel selectedItem = this.MainList.SelectedItem as ChatItemViewModel;
      this.MainList.SelectedItem = (object) null;
      if (selectedItem == null || selectedItem.Conversation == null)
        return;
      NavUtils.NavigateToChat(selectedItem.Conversation.Jid, false);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ContactInfoTabView.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.MainList = (LongListSelector) this.FindName("MainList");
      this.HeaderPanel = (StackPanel) this.FindName("HeaderPanel");
      this.ProfileImage = (Image) this.FindName("ProfileImage");
      this.WarningBlock = (TextBlock) this.FindName("WarningBlock");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
      this.WaNumberList = (ItemsControl) this.FindName("WaNumberList");
      this.TabHeader = (ChatInfoTabHeader) this.FindName("TabHeader");
      this.FooterPanel = (StackPanel) this.FindName("FooterPanel");
      this.ActionList = (ItemsControl) this.FindName("ActionList");
      this.InfoList = (ItemsControl) this.FindName("InfoList");
    }
  }
}
