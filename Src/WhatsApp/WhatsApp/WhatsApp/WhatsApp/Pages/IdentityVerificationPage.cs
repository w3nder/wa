// Decompiled with JetBrains decompiler
// Type: WhatsApp.Pages.IdentityVerificationPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using ZXing;
using ZXing.QrCode;


namespace WhatsApp.Pages
{
  public class IdentityVerificationPage : PhoneApplicationPage
  {
    private List<IDisposable> subscriptions = new List<IDisposable>();
    private bool backKeyToIdentity;
    private bool qrRead;
    private string RecipientJid;
    private string GeneratedQr;
    private string SecurityNumber;
    private ApplicationBarIconButton ScanButton;
    private ApplicationBarIconButton ShareButton;
    private DispatcherTimer TooltipTimer;
    private DataTransferManager DataTransferManager;
    internal Storyboard CompleteAnimation;
    internal Storyboard ShowScannerStoryboard;
    internal Storyboard ShowIdentityStoryboard;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal Grid ContentPanel;
    internal Grid IdentityPanel;
    internal CompositeTransform IdentityXForm;
    internal Image QrCodeImage;
    internal Grid ScanResult;
    internal CompositeTransform ResultXForm;
    internal Ellipse ScanBackground;
    internal Image ScanIcon;
    internal TextBlock T0;
    internal TextBlock T1;
    internal TextBlock T2;
    internal RichTextBox VerificationScanExplanation;
    internal Grid ScannerPanel;
    internal CompositeTransform ScannerXForm;
    internal QrScanner Scanner;
    internal ZoomBox TooltipZoomBox;
    internal Grid ToolTipBackground;
    internal TextBlock TooltipBlock;
    private bool _contentLoaded;

    public IdentityVerificationPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.PageTitle.SmallTitle = AppResources.VerifySecurityNumberTitle;
      this.BackKeyPress += new EventHandler<CancelEventArgs>(this.Scanner_BackKeyPress);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      string recipientJid = (string) null;
      this.NavigationContext.QueryString.TryGetValue("jid", out recipientJid);
      this.PageTitle.KeepOriginalSubtitleCase = true;
      this.PageTitle.Subtitle = AppResources.You + ", " + JidHelper.GetDisplayNameForContactJid(recipientJid);
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (storeForApplication.DirectoryExists("tmp"))
          {
            if (storeForApplication.FileExists("tmp\\qrcode.jpg"))
              storeForApplication.DeleteFile("tmp\\qrcode.jpg");
          }
        }
        this.LoadFingerprint(recipientJid);
      }));
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      this.subscriptions.ForEach((Action<IDisposable>) (a => a.SafeDispose()));
      this.subscriptions.Clear();
      base.OnNavigatingFrom(e);
    }

    private void LoadFingerprint(string recipient)
    {
      this.RecipientJid = recipient;
      string displayable = (string) null;
      string scannable = (string) null;
      AppState.GetConnection().Encryption.IdentityGetFingerprint(recipient, out displayable, out scannable);
      this.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        if (displayable == null || scannable == null)
          this.ShowNoIdentity(recipient);
        else
          this.ShowIdentity(recipient, displayable, scannable);
      }));
    }

    private void ShowNoIdentity(string jid)
    {
      this.TooltipZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.TooltipZoomBox.Visibility = Visibility.Visible;
      this.TooltipBlock.Text = string.Format(AppResources.VerificationNoSession, (object) JidHelper.GetDisplayNameForContactJid(jid));
      this.IdentityPanel.Visibility = Visibility.Collapsed;
    }

    private void ShowIdentity(string recipient, string displayable, string scannable)
    {
      this.IdentityPanel.Visibility = Visibility.Visible;
      this.TooltipZoomBox.Visibility = Visibility.Collapsed;
      Paragraph paragraph = new Paragraph();
      paragraph.Inlines.Add(AppResources.VerificationScanExplanation + " ");
      Hyperlink hyperlink1 = new Hyperlink();
      hyperlink1.Foreground = (Brush) UIUtils.AccentBrush;
      hyperlink1.TextDecorations = (TextDecorationCollection) null;
      hyperlink1.Command = (ICommand) new ActionCommand((Action) (() => new WebBrowserTask()
      {
        Uri = new Uri(WaWebUrls.FaqUrlGroupE2e)
      }.Show()));
      Hyperlink hyperlink2 = hyperlink1;
      hyperlink2.Inlines.Add(AppResources.LearnMoreSecurity);
      paragraph.Inlines.Add((Inline) hyperlink2);
      this.VerificationScanExplanation.Blocks.Clear();
      this.VerificationScanExplanation.Blocks.Add((Block) paragraph);
      this.SecurityNumber = displayable;
      this.GeneratedQr = scannable;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        if (this.IdentityPanel.FindName("T" + index1.ToString()) is TextBlock name)
        {
          string str = displayable.Substring(index1 * 20, 20);
          StringBuilder stringBuilder = new StringBuilder();
          for (int index2 = 0; index2 < str.Length; ++index2)
          {
            stringBuilder.Append(str[index2]);
            if (index2 != str.Length - 1)
            {
              if (index2 % 5 == 4)
                stringBuilder.Append("    ");
              else
                stringBuilder.Append(" ");
            }
          }
          name.Text = stringBuilder.ToString();
        }
      }
      QRCodeWriter qrCodeWriter = new QRCodeWriter();
      Dictionary<EncodeHintType, object> dictionary = new Dictionary<EncodeHintType, object>();
      dictionary[EncodeHintType.CHARACTER_SET] = (object) "ISO-8859-1";
      dictionary[EncodeHintType.MARGIN] = (object) 0;
      string contents = scannable;
      Dictionary<EncodeHintType, object> hints = dictionary;
      this.QrCodeImage.Source = (System.Windows.Media.ImageSource) new BarcodeWriter().Write(qrCodeWriter.encode(contents, BarcodeFormat.QR_CODE, 190, 190, (IDictionary<EncodeHintType, object>) hints));
      this.EnsureScanButton();
    }

    public void EnsureScanButton()
    {
      if (this.ScanButton == null)
      {
        this.ScanButton = new ApplicationBarIconButton()
        {
          IconUri = new Uri("/Images/camera-icon.png", UriKind.RelativeOrAbsolute),
          Text = AppResources.WebScanCode
        };
        this.ScanButton.Click += new EventHandler(this.Scanner_Click);
        this.ApplicationBar.Buttons.Add((object) this.ScanButton);
      }
      if (this.ShareButton != null)
        return;
      this.ShareButton = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/tell-a-friend-icon.png", UriKind.RelativeOrAbsolute),
        Text = AppResources.ShareTitle
      };
      this.ShareButton.Click += new EventHandler(this.ShareButton_Click);
      this.ApplicationBar.Buttons.Add((object) this.ShareButton);
    }

    private void ShareButton_Click(object sender, EventArgs e)
    {
      this.DataTransferManager = DataTransferManager.GetForCurrentView();
      DataTransferManager dataTransferManager = this.DataTransferManager;
      // ISSUE: method pointer
      WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>>(new Func<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>, EventRegistrationToken>(dataTransferManager.add_DataRequested), new Action<EventRegistrationToken>(dataTransferManager.remove_DataRequested), new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>((object) this, __methodptr(OnDataRequested)));
      DataTransferManager.ShowShareUI();
    }

    private string SecurityNumberSpaced
    {
      get
      {
        if (this.SecurityNumber == null)
          return (string) null;
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < this.SecurityNumber.Length; ++index)
        {
          stringBuilder.Append(this.SecurityNumber[index]);
          if (index % 5 == 4 && index < this.SecurityNumber.Length - 1)
            stringBuilder.Append(" ");
        }
        return stringBuilder.ToString();
      }
    }

    private void GetSharingText(out string subject, out string content)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this.SecurityNumber.Length; ++index)
      {
        stringBuilder.Append(this.SecurityNumber[index]);
        if (index % 5 == 4 && index < this.SecurityNumber.Length - 1)
          stringBuilder.Append(" ");
      }
      string str = JidHelper.GetDisplayNameForContactJidNoNumber(Settings.MyJid) ?? Settings.PushName;
      subject = string.Format(AppResources.VerificationShareSubject, (object) str, (object) PhoneNumberFormatter.FormatInternationalNumber(Settings.ChatID));
      content = string.Format(AppResources.VerificationShareContent, (object) this.SecurityNumberSpaced);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      if (this.DataTransferManager != null)
      {
        // ISSUE: method pointer
        WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>>(new Action<EventRegistrationToken>(this.DataTransferManager.remove_DataRequested), new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>((object) this, __methodptr(OnDataRequested)));
      }
      if (this.TooltipTimer == null)
        return;
      this.TooltipTimer.Stop();
    }

    protected async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
    {
      DataRequestDeferral deferral = e.Request.GetDeferral();
      string subject;
      string content;
      this.GetSharingText(out subject, out content);
      e.Request.Data.Properties.put_Title(subject);
      e.Request.Data.Properties.put_Description(content);
      e.Request.Data.SetText(content);
      StorageFolder localFolder = ApplicationData.Current.LocalFolder;
      if (localFolder != null)
      {
        StorageFolder folderAsync = await localFolder.CreateFolderAsync("tmp", (CreationCollisionOption) 3);
        if (folderAsync != null)
        {
          StorageFile file = await folderAsync.CreateFileAsync("qrcode.jpg", (CreationCollisionOption) 1);
          if (file != null)
          {
            using (Stream targetStream = await ((IStorageFile) file).OpenStreamForWriteAsync())
            {
              QRCodeWriter qrCodeWriter = new QRCodeWriter();
              Dictionary<EncodeHintType, object> dictionary = new Dictionary<EncodeHintType, object>();
              dictionary[EncodeHintType.CHARACTER_SET] = (object) "ISO-8859-1";
              string generatedQr = this.GeneratedQr;
              Dictionary<EncodeHintType, object> hints = dictionary;
              new BarcodeWriter().Write(qrCodeWriter.encode(generatedQr, BarcodeFormat.QR_CODE, 500, 500, (IDictionary<EncodeHintType, object>) hints)).SaveJpeg(targetStream, 500, 500, 0, 90);
              targetStream.Flush();
              targetStream.Close();
            }
            e.Request.Data.SetStorageItems((IEnumerable<IStorageItem>) new StorageFile[1]
            {
              file
            });
          }
          file = (StorageFile) null;
        }
      }
      deferral.Complete();
    }

    public void Scanner_Click(object sender, EventArgs args) => this.ShowScanner();

    private void Scanner_QrScanned(object sender, QrScanner.QrScannerEventArgs e)
    {
      if (this.GeneratedQr == null || this.qrRead)
        return;
      this.qrRead = true;
      VibrateController.Default.Start(TimeSpan.FromMilliseconds(200.0));
      this.Scanner.Waiting = true;
      Axolotl.IdentityVerificationResult verificationResult = AppState.GetConnection().Encryption.IdentityVerifyFingerprint(this.GeneratedQr, e.Bytes);
      this.HideScanner();
      switch (verificationResult)
      {
        case Axolotl.IdentityVerificationResult.IdentityMismatchContact:
        case Axolotl.IdentityVerificationResult.IdentityMismatchYou:
          this.TooltipZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
          this.TooltipZoomBox.Visibility = Visibility.Visible;
          this.ToolTipBackground.Background = (Brush) UIUtils.AccentBrush;
          this.TooltipBlock.Text = string.Format(verificationResult == Axolotl.IdentityVerificationResult.IdentityMismatchYou ? AppResources.SecurityCodeIdentityMismatchThem : AppResources.SecurityCodeIdentityMismatch, (object) JidHelper.GetDisplayNameForContactJid(this.RecipientJid));
          if (this.TooltipTimer != null)
            this.TooltipTimer.Stop();
          this.TooltipTimer = new DispatcherTimer();
          this.TooltipTimer.Interval = TimeSpan.FromSeconds(5.0);
          this.TooltipTimer.Tick += new EventHandler(this.TooltipTimer_Tick);
          this.TooltipTimer.Start();
          break;
        case Axolotl.IdentityVerificationResult.VersionMismatch:
          this.ScanIcon.Source = (System.Windows.Media.ImageSource) AssetStore.ScanFailureBang;
          this.ScanBackground.Fill = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, (byte) 67, (byte) 67));
          this.CompleteAnimation.Begin();
          break;
        case Axolotl.IdentityVerificationResult.NoMatch:
          this.ScanIcon.Source = (System.Windows.Media.ImageSource) AssetStore.ScanFailureBang;
          this.ScanBackground.Fill = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, (byte) 67, (byte) 67));
          this.CompleteAnimation.Begin();
          break;
        case Axolotl.IdentityVerificationResult.Match:
          this.ScanIcon.Source = (System.Windows.Media.ImageSource) AssetStore.ScanSuccessCheck;
          this.ScanBackground.Fill = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 204, (byte) 106));
          this.CompleteAnimation.Begin();
          break;
      }
      this.qrRead = false;
    }

    private void TooltipTimer_Tick(object sender, EventArgs e)
    {
      if (this.TooltipTimer != null)
        this.TooltipTimer.Stop();
      this.TooltipZoomBox.Visibility = Visibility.Collapsed;
    }

    private void Scanner_BackKeyPress(object sender, CancelEventArgs e)
    {
      if (!this.backKeyToIdentity)
        return;
      this.HideScanner();
      e.Cancel = true;
    }

    public void ShowScanner()
    {
      this.backKeyToIdentity = true;
      this.SupportedOrientations = SupportedPageOrientation.Portrait;
      this.ApplicationBar.IsVisible = false;
      try
      {
        this.Scanner.ScanAsync();
      }
      catch (Exception ex)
      {
        this.HideScanner();
      }
      this.ScannerPanel.Visibility = Visibility.Visible;
      this.ScannerXForm.TranslateY = 800.0;
      this.IdentityXForm.TranslateY = 0.0;
      this.ShowIdentityStoryboard.Stop();
      this.Dispatcher.BeginInvoke((Action) (() => Storyboarder.Perform(this.ShowScannerStoryboard, onComplete: (Action) (() =>
      {
        this.IdentityXForm.TranslateY = 0.0;
        this.ScannerXForm.TranslateY = 0.0;
        this.IdentityPanel.Visibility = Visibility.Collapsed;
      }))));
    }

    public void HideScanner()
    {
      this.backKeyToIdentity = false;
      this.Scanner.Waiting = false;
      this.Scanner.StopScan();
      this.IdentityPanel.Visibility = Visibility.Visible;
      this.ScannerXForm.TranslateY = 0.0;
      this.IdentityXForm.TranslateY = 800.0;
      this.ShowScannerStoryboard.Stop();
      this.Dispatcher.BeginInvoke((Action) (() => Storyboarder.Perform(this.ShowIdentityStoryboard, onComplete: (Action) (() =>
      {
        this.ApplicationBar.IsVisible = true;
        this.IdentityXForm.TranslateY = 0.0;
        this.ScannerXForm.TranslateY = 0.0;
        this.ScannerPanel.Visibility = Visibility.Collapsed;
      }))));
    }

    private void CopyCode_Click(object sender, RoutedEventArgs e)
    {
      Clipboard.SetText(this.SecurityNumberSpaced);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/IdentityVerificationPage.xaml", UriKind.Relative));
      this.CompleteAnimation = (Storyboard) this.FindName("CompleteAnimation");
      this.ShowScannerStoryboard = (Storyboard) this.FindName("ShowScannerStoryboard");
      this.ShowIdentityStoryboard = (Storyboard) this.FindName("ShowIdentityStoryboard");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ContentPanel = (Grid) this.FindName("ContentPanel");
      this.IdentityPanel = (Grid) this.FindName("IdentityPanel");
      this.IdentityXForm = (CompositeTransform) this.FindName("IdentityXForm");
      this.QrCodeImage = (Image) this.FindName("QrCodeImage");
      this.ScanResult = (Grid) this.FindName("ScanResult");
      this.ResultXForm = (CompositeTransform) this.FindName("ResultXForm");
      this.ScanBackground = (Ellipse) this.FindName("ScanBackground");
      this.ScanIcon = (Image) this.FindName("ScanIcon");
      this.T0 = (TextBlock) this.FindName("T0");
      this.T1 = (TextBlock) this.FindName("T1");
      this.T2 = (TextBlock) this.FindName("T2");
      this.VerificationScanExplanation = (RichTextBox) this.FindName("VerificationScanExplanation");
      this.ScannerPanel = (Grid) this.FindName("ScannerPanel");
      this.ScannerXForm = (CompositeTransform) this.FindName("ScannerXForm");
      this.Scanner = (QrScanner) this.FindName("Scanner");
      this.TooltipZoomBox = (ZoomBox) this.FindName("TooltipZoomBox");
      this.ToolTipBackground = (Grid) this.FindName("ToolTipBackground");
      this.TooltipBlock = (TextBlock) this.FindName("TooltipBlock");
    }
  }
}
