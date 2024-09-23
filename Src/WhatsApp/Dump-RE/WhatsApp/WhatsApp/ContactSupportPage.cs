// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactSupportPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WhatsApp.Events;
using WhatsApp.WaCollections;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

#nullable disable
namespace WhatsApp
{
  public class ContactSupportPage : PhoneApplicationPage
  {
    private string uniqueLogId;
    private string context;
    private bool feedbackSent;
    private ApplicationBarIconButton nextButton;
    private IDisposable faqSearchSub;
    private int faqResultsReturned = -1;
    private Set<SupportSearchResult> faqResultsReadSet = new Set<SupportSearchResult>();
    private GlobalProgressIndicator globalProgress;
    private ContactUsSession supportFSEvent = new ContactUsSession();
    private SupportSearchResult[] faqResultsReturnedSet;
    private TimeSpan totalArticleReadTime;
    private SupportSearchResult currentArticleBeingRead;
    private bool isQuickMode;
    private Button clickedScreenshot;
    private FileOpenPicker screenshotFilePicker;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal Grid ContentPanel;
    internal TextBlock FeedbackTooltip;
    internal PhoneTextBox DescriptionBlock;
    internal Grid ScreenshotPanel;
    internal HyperlinkButton FaqTooltip;
    internal Button QuickEmailButton;
    internal Button QuickSendButton;
    internal TextBlock BetaDescriptionBlock;
    internal WhiteBlackImage SupportImage;
    internal Grid SearchResultsPanel;
    internal TextBlock SearchResultsTitle;
    internal ListBox SearchResultsListBox;
    internal Grid SendButtonPanel;
    internal Button SendButton;
    internal ApplicationBarIconButton AppBarSendButton;
    private bool _contentLoaded;

    public ContactSupportPage()
    {
      this.InitializeComponent();
      this.uniqueLogId = ContactSupportHelper.GenerateUniqueLogId();
      this.globalProgress = new GlobalProgressIndicator((DependencyObject) this);
      this.supportFSEvent = new ContactUsSession();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.nextButton = this.ApplicationBar.Buttons[0] as ApplicationBarIconButton;
      this.SupportImage.Image = ImageStore.SendEmailIcon;
      this.TitlePanel.SmallTitle = AppResources.ContactSupportPageTitle;
      this.FeedbackTooltip.Text = AppResources.SupportIssueHeader;
      this.FaqTooltip.Content = (object) AppResources.ReadManual;
      this.SearchResultsListBox.GetSelectionChangedAsync().Subscribe<SelectionChangedEventArgs>((Action<SelectionChangedEventArgs>) (_ => this.SearchResultsListBox.SelectedItem = (object) null));
      this.BetaDescriptionBlock.Visibility = Visibility.Visible;
      this.ScreenshotPanel.Visibility = Visibility.Visible;
    }

    private void RefreshPageMargin()
    {
      switch (this.Orientation)
      {
        case PageOrientation.Portrait:
        case PageOrientation.PortraitUp:
          this.TitlePanel.Margin = new Thickness(0.0, UIUtils.SystemTraySizePortrait, 0.0, 0.0);
          this.ContentPanel.Margin = new Thickness(12.0, 20.0, 12.0, 0.0);
          this.SearchResultsPanel.Margin = new Thickness(0.0);
          this.SendButton.Margin = new Thickness(12.0, 0.0, 0.0, 0.0);
          break;
        case PageOrientation.LandscapeLeft:
          this.TitlePanel.Margin = new Thickness(UIUtils.SystemTraySizeLandscape, 0.0, 0.0, 0.0);
          this.ContentPanel.Margin = new Thickness(12.0 + UIUtils.SystemTraySizeLandscape, 20.0, 12.0, 0.0);
          this.SearchResultsPanel.Margin = new Thickness(UIUtils.SystemTraySizeLandscape, 0.0, 0.0, 0.0);
          this.SendButton.Margin = new Thickness(12.0 + UIUtils.SystemTraySizeLandscape, 0.0, 0.0, 0.0);
          break;
        case PageOrientation.LandscapeRight:
          this.TitlePanel.Margin = new Thickness(0.0);
          this.ContentPanel.Margin = new Thickness(12.0, 20.0, 12.0, 0.0);
          this.SearchResultsPanel.Margin = new Thickness(0.0);
          this.SendButton.Margin = new Thickness(12.0, 0.0, 0.0, 0.0);
          break;
      }
    }

    private void CancelSearch()
    {
      this.faqSearchSub.SafeDispose();
      this.faqSearchSub = (IDisposable) null;
      this.globalProgress.Release();
      this.IsEnabled = true;
      this.faqResultsReturned = -1;
      this.faqResultsReadSet.Clear();
    }

    private void SendSupportEmail()
    {
      if (this.feedbackSent)
        return;
      this.feedbackSent = true;
      string str = (this.DescriptionBlock.Text ?? "").Trim();
      Log.l("feedback", str);
      List<StorageFile> storageFileList = new List<StorageFile>();
      foreach (UIElement child in (PresentationFrameworkCollection<UIElement>) this.ScreenshotPanel.Children)
      {
        if (child is Button button && button.Tag != null && button.Tag is StorageFile)
          storageFileList.Add(button.Tag as StorageFile);
      }
      ContactSupportHelper.SendSupportEmail(str, this.uniqueLogId, this.context, this.faqResultsReturned, this.faqResultsReadSet.Count, (object) storageFileList);
      this.supportFSEvent.contactUsScreenshotC = new double?((double) storageFileList.Count<StorageFile>());
      this.supportFSEvent.contactUsExitState = new wam_enum_contact_us_exit_state?(wam_enum_contact_us_exit_state.EMAIL_SEND);
      this.supportFSEvent.contactUsProblemDescription = str;
      this.supportFSEvent.searchFaqResultsGeneratedC = new double?((double) this.faqResultsReturned);
      this.supportFSEvent.searchFaqResultsReadC = new double?((double) this.faqResultsReadSet.Count);
      this.supportFSEvent.searchFaqResultsReadT = new long?((long) this.totalArticleReadTime.TotalMilliseconds);
      this.supportFSEvent.SaveEvent();
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      base.OnBackKeyPress(e);
      if (this.faqSearchSub != null)
      {
        this.CancelSearch();
        e.Cancel = true;
        this.ApplicationBar.IsVisible = true;
      }
      else if (this.SearchResultsPanel.Visibility == Visibility.Visible)
      {
        e.Cancel = true;
        if (ImageStore.IsDarkTheme())
        {
          this.LayoutRoot.Background = (Brush) new SolidColorBrush(Colors.Transparent);
          SysTrayHelper.SetForegroundColor((DependencyObject) this, Colors.White);
          this.TitlePanel.Foreground = (Brush) new SolidColorBrush(Colors.White);
        }
        this.SearchResultsPanel.Visibility = Visibility.Collapsed;
        this.SendButtonPanel.Visibility = Visibility.Collapsed;
        this.ContentPanel.Visibility = Visibility.Visible;
        this.ApplicationBar.IsVisible = true;
        this.RefreshPageMargin();
      }
      else
      {
        this.supportFSEvent.searchFaqResultsGeneratedC = new double?((double) this.faqResultsReturned);
        this.supportFSEvent.searchFaqResultsReadC = new double?((double) this.faqResultsReadSet.Count);
        this.supportFSEvent.searchFaqResultsReadT = new long?((long) this.totalArticleReadTime.TotalMilliseconds);
        this.supportFSEvent.contactUsExitState = new wam_enum_contact_us_exit_state?(wam_enum_contact_us_exit_state.PROBLEM_DESCRIPTION);
        this.supportFSEvent.contactUsProblemDescription = (this.DescriptionBlock.Text ?? "").Trim();
        if (this.faqResultsReturned > 0)
          this.supportFSEvent.contactUsExitState = new wam_enum_contact_us_exit_state?(wam_enum_contact_us_exit_state.SUGGESTED_FAQ);
        this.supportFSEvent.SaveEvent();
      }
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      this.RefreshPageMargin();
      base.OnOrientationChanged(e);
    }

    private void OnSupportSearchResults(SupportSearchResult[] results)
    {
      if (this.faqSearchSub == null)
        return;
      this.CancelSearch();
      if (results != null)
      {
        this.faqResultsReturned = results.Length;
        this.faqResultsReadSet.Clear();
      }
      if (results == null || results.Length == 0)
      {
        this.ApplicationBar.IsVisible = true;
        this.SendSupportEmail();
      }
      else
      {
        this.SearchResultsListBox.ItemsSource = (IEnumerable) results;
        this.faqResultsReturnedSet = results;
        this.supportFSEvent.searchFaqResultsBestId = new long?();
        this.supportFSEvent.searchFaqResultsBestId2 = new long?();
        this.supportFSEvent.searchFaqResultsBestId3 = new long?();
        for (int index = 0; index < 3 && index < results.Length; ++index)
        {
          int num = results[index].URLtoID();
          switch (index)
          {
            case 0:
              this.supportFSEvent.searchFaqResultsBestId = new long?((long) num);
              break;
            case 1:
              this.supportFSEvent.searchFaqResultsBestId2 = new long?((long) num);
              break;
            case 2:
              this.supportFSEvent.searchFaqResultsBestId3 = new long?((long) num);
              break;
          }
        }
        if (ImageStore.IsDarkTheme())
        {
          this.LayoutRoot.Background = (Brush) new SolidColorBrush(Colors.White);
          SysTrayHelper.SetForegroundColor((DependencyObject) this, Colors.Black);
          this.TitlePanel.Foreground = (Brush) new SolidColorBrush(Colors.Black);
        }
        this.ContentPanel.Visibility = Visibility.Collapsed;
        this.SearchResultsPanel.Visibility = Visibility.Visible;
        this.SendButtonPanel.Visibility = Visibility.Visible;
        this.RefreshPageMargin();
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.feedbackSent)
      {
        WAThreadPool.QueueUserWorkItem((Action) (() => Log.SendSupportLog(this.uniqueLogId)));
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else
      {
        this.RefreshPageMargin();
        if (this.context == null)
          this.NavigationContext.QueryString.TryGetValue("context", out this.context);
        if (e.NavigationMode == NavigationMode.New)
        {
          if (Settings.IsWaAdmin)
          {
            WaUriParams waUriParams = new WaUriParams(this.NavigationContext.QueryString);
            this.isQuickMode = false;
            ref bool local = ref this.isQuickMode;
            if (waUriParams.TryGetBoolValue("quicky", out local) && this.isQuickMode)
            {
              this.TitlePanel.SmallTitle = "LOG QUICKY (lucky you ;)";
              this.FeedbackTooltip.Text = "Please rant below:";
              this.nextButton.IsEnabled = true;
              this.BetaDescriptionBlock.Visibility = Visibility.Collapsed;
              this.FaqTooltip.Visibility = Visibility.Collapsed;
              this.QuickEmailButton.Visibility = Visibility.Visible;
              this.QuickEmailButton.Content = (object) "send";
              this.QuickSendButton.Visibility = Visibility.Visible;
              this.QuickSendButton.Content = (object) "send without email";
              this.ApplicationBar.IsVisible = false;
              this.context = "Quicky Mode";
            }
          }
          FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CONTACT_US);
        }
        if (this.currentArticleBeingRead == null)
          return;
        long totalMilliseconds = (long) (DateTime.Now - this.currentArticleBeingRead.startReadTime).TotalMilliseconds;
        switch (((IEnumerable<SupportSearchResult>) this.faqResultsReturnedSet).ToList<SupportSearchResult>().IndexOf(this.currentArticleBeingRead))
        {
          case 0:
            this.supportFSEvent.searchFaqResultsBestReadT = new long?(totalMilliseconds);
            break;
          case 1:
            this.supportFSEvent.searchFaqResultsBestReadT2 = new long?(totalMilliseconds);
            break;
          case 2:
            this.supportFSEvent.searchFaqResultsBestReadT3 = new long?(totalMilliseconds);
            break;
        }
        TimeSpan totalArticleReadTime = this.totalArticleReadTime;
        this.totalArticleReadTime = this.totalArticleReadTime.Add(new TimeSpan(0, 0, (int) totalMilliseconds));
        this.currentArticleBeingRead = (SupportSearchResult) null;
      }
    }

    private void DescriptionBlock_GotFocus(object sender, RoutedEventArgs e)
    {
      if (this.isQuickMode)
        return;
      this.ApplicationBar.IsVisible = true;
      this.TitlePanel.Visibility = Visibility.Collapsed;
      this.FaqTooltip.Visibility = Visibility.Collapsed;
      ((ApplicationBarIconButton) this.ApplicationBar.Buttons[0]).Text = AppResources.NextStep;
      ((ApplicationBarIconButton) this.ApplicationBar.Buttons[0]).IconUri = new Uri("/Images/Next.png", UriKind.Relative);
    }

    private void DescriptionBlock_LostFocus(object sender, RoutedEventArgs e)
    {
      if (this.isQuickMode)
        return;
      this.TitlePanel.Visibility = Visibility.Visible;
      this.FaqTooltip.Visibility = Visibility.Visible;
      ((ApplicationBarIconButton) this.ApplicationBar.Buttons[0]).Text = AppResources.Send;
      ((ApplicationBarIconButton) this.ApplicationBar.Buttons[0]).IconUri = new Uri("/Images/contact-support-send-icon.png", UriKind.Relative);
    }

    private void DescriptionBlock_TextChanged(object sender, TextChangedEventArgs e)
    {
      bool flag = ContactSupportHelper.IsFeedbackAcceptable(this.DescriptionBlock.Text);
      if (flag == this.nextButton.IsEnabled)
        return;
      this.nextButton.IsEnabled = flag;
    }

    private void Faq_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      new WebBrowserTask()
      {
        Uri = new Uri("https://faq.whatsapp.com")
      }.Show();
    }

    private void Next_Click(object sender, EventArgs e)
    {
      if (this.isQuickMode)
        this.SendSupportEmail();
      else if (FocusManager.GetFocusedElement() == this.DescriptionBlock)
      {
        this.Focus();
      }
      else
      {
        if (this.faqSearchSub != null)
          return;
        string str = (this.DescriptionBlock.Text ?? "").Trim();
        if (ContactSupportHelper.IsFeedbackAcceptable(str))
        {
          IObservable<SupportSearchResult[]> observable = Observable.Return<SupportSearchResult[]>(new SupportSearchResult[0]);
          this.IsEnabled = false;
          this.globalProgress.Acquire();
          this.ApplicationBar.IsVisible = false;
          this.faqSearchSub = SupportSearchResult.Fetch(str, false).Timeout<SupportSearchResult[]>(TimeSpan.FromSeconds(15.0), observable).Catch<SupportSearchResult[]>(observable).ObserveOnDispatcher<SupportSearchResult[]>().Subscribe<SupportSearchResult[]>(new Action<SupportSearchResult[]>(this.OnSupportSearchResults));
        }
        else
        {
          int num = (int) MessageBox.Show(AppResources.InsufficientDetail);
        }
      }
    }

    private void SearchResult_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is SupportSearchResult tag))
        return;
      this.faqResultsReadSet.Add(tag);
      this.currentArticleBeingRead = tag;
      tag.Select();
    }

    private void Send_Click(object sender, RoutedEventArgs e) => this.SendSupportEmail();

    private void QuickEmail_Click(object sender, RoutedEventArgs e)
    {
      if (!this.isQuickMode)
        return;
      this.SendSupportEmail();
    }

    private void QuickSend_Click(object sender, RoutedEventArgs e)
    {
      if (!this.isQuickMode)
        return;
      Log.l("feedback", (this.DescriptionBlock.Text ?? "").Trim());
      WAThreadPool.QueueUserWorkItem((Action) (() => Log.SendSupportLog(this.uniqueLogId)));
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void ChooseScreenshot_Click(object sender, RoutedEventArgs e)
    {
      this.clickedScreenshot = sender as Button;
      this.screenshotFilePicker = new FileOpenPicker();
      this.screenshotFilePicker.put_ViewMode((PickerViewMode) 1);
      this.screenshotFilePicker.put_SuggestedStartLocation((PickerLocationId) 6);
      this.screenshotFilePicker.FileTypeFilter.Add(".png");
      this.screenshotFilePicker.FileTypeFilter.Add(".jpg");
      this.screenshotFilePicker.FileTypeFilter.Add(".jpeg");
      this.screenshotFilePicker.PickSingleFileAndContinue();
      CoreApplicationView currentView = CoreApplication.GetCurrentView();
      // ISSUE: method pointer
      WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>>(new Func<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>, EventRegistrationToken>(currentView.add_Activated), new Action<EventRegistrationToken>(currentView.remove_Activated), new TypedEventHandler<CoreApplicationView, IActivatedEventArgs>((object) this, __methodptr(FileOpenPickerReturnActivated)));
    }

    private async void FileOpenPickerReturnActivated(
      CoreApplicationView sender,
      IActivatedEventArgs activatedArgs)
    {
      // ISSUE: method pointer
      WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>>(new Action<EventRegistrationToken>(CoreApplication.GetCurrentView().remove_Activated), new TypedEventHandler<CoreApplicationView, IActivatedEventArgs>((object) this, __methodptr(FileOpenPickerReturnActivated)));
      this.screenshotFilePicker = (FileOpenPicker) null;
      if (activatedArgs.Kind != 1002 || !(activatedArgs is FileOpenPickerContinuationEventArgs continuationEventArgs) || continuationEventArgs.Files == null || !continuationEventArgs.Files.Any<StorageFile>())
        return;
      StorageFile storageFile = continuationEventArgs.Files.First<StorageFile>();
      if (this.clickedScreenshot == null)
        return;
      this.clickedScreenshot.Tag = (object) storageFile;
      IRandomAccessStream windowsRuntimeStream = await storageFile.OpenAsync((FileAccessMode) 0);
      BitmapImage bitmapImage = new BitmapImage();
      bitmapImage.SetSource(((IInputStream) windowsRuntimeStream).AsStreamForRead());
      ((Image) this.clickedScreenshot.Content).Source = (System.Windows.Media.ImageSource) bitmapImage;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ContactSupportPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ContentPanel = (Grid) this.FindName("ContentPanel");
      this.FeedbackTooltip = (TextBlock) this.FindName("FeedbackTooltip");
      this.DescriptionBlock = (PhoneTextBox) this.FindName("DescriptionBlock");
      this.ScreenshotPanel = (Grid) this.FindName("ScreenshotPanel");
      this.FaqTooltip = (HyperlinkButton) this.FindName("FaqTooltip");
      this.QuickEmailButton = (Button) this.FindName("QuickEmailButton");
      this.QuickSendButton = (Button) this.FindName("QuickSendButton");
      this.BetaDescriptionBlock = (TextBlock) this.FindName("BetaDescriptionBlock");
      this.SupportImage = (WhiteBlackImage) this.FindName("SupportImage");
      this.SearchResultsPanel = (Grid) this.FindName("SearchResultsPanel");
      this.SearchResultsTitle = (TextBlock) this.FindName("SearchResultsTitle");
      this.SearchResultsListBox = (ListBox) this.FindName("SearchResultsListBox");
      this.SendButtonPanel = (Grid) this.FindName("SendButtonPanel");
      this.SendButton = (Button) this.FindName("SendButton");
      this.AppBarSendButton = (ApplicationBarIconButton) this.FindName("AppBarSendButton");
    }
  }
}
