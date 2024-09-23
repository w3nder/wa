// Decompiled with JetBrains decompiler
// Type: WhatsApp.ImageEditPage
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class ImageEditPage : PhoneApplicationPage
  {
    private static ImageEditPage.ImageEditPageConfigs nextInstanceConfigs_;
    private static IObserver<ImageEditPage.ImageEditPageResults> nextInstanceObserver_;
    private ImageEditPage.ImageEditPageConfigs configs_;
    private IObserver<ImageEditPage.ImageEditPageResults> observer_;
    private bool actionTaken_;
    internal Grid LayoutRoot;
    internal ImageEditControl EditControl;
    internal RichTextBlock ImageLink;
    internal RichTextBlock MaybeCopyright;
    private bool _contentLoaded;

    public ImageEditPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.configs_ = ImageEditPage.nextInstanceConfigs_;
      this.observer_ = ImageEditPage.nextInstanceObserver_;
      ImageEditPage.nextInstanceConfigs_ = (ImageEditPage.ImageEditPageConfigs) null;
      ImageEditPage.nextInstanceObserver_ = (IObserver<ImageEditPage.ImageEditPageResults>) null;
      if (this.configs_ != null)
      {
        if (this.configs_.MinRelativeCropSize.HasValue)
          this.EditControl.MinRelativeCropSize = this.configs_.MinRelativeCropSize;
        this.EditControl.Setup(this.configs_.OriginalBitmap, this.configs_.CropMode, this.configs_.InitialCropRatio);
        this.Loaded += new RoutedEventHandler(this.OnLoaded);
      }
      this.MaybeCopyright.Visibility = Visibility.Collapsed;
      this.ImageLink.Visibility = Visibility.Collapsed;
      if (this.configs_ == null || this.configs_.ImageFrom != ImageEditPage.ImageEditPageConfigs.ImageSource.Bing)
        return;
      this.MaybeCopyright.Visibility = Visibility.Visible;
      string imageMaybeCopyright = AppResources.ImageMaybeCopyright;
      WaRichText.Chunk chunk1 = ((IEnumerable<WaRichText.Chunk>) WaRichText.GetHtmlLinkChunks(imageMaybeCopyright)).SingleOrDefault<WaRichText.Chunk>();
      if (chunk1 == null)
      {
        this.MaybeCopyright.Text = new RichTextBlock.TextSet()
        {
          Text = imageMaybeCopyright
        };
      }
      else
      {
        string info = "https://www.whatsapp.com/legal/";
        string tosString = chunk1.Value;
        Func<string> valueFunc = (Func<string>) (() => tosString.Replace(' ', ' '));
        WaRichText.Chunk chunk2 = new WaRichText.Chunk(chunk1.Offset, chunk1.Length, WaRichText.Formats.Link, info, valueFunc);
        this.MaybeCopyright.Text = new RichTextBlock.TextSet()
        {
          Text = imageMaybeCopyright,
          PartialFormattings = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
          {
            chunk2
          }
        };
      }
      string str = (string) null;
      if (this.configs_.ImagePath != null)
      {
        try
        {
          str = new Uri(this.configs_.ImagePath).Host;
        }
        catch (Exception ex)
        {
          string context = string.Format("Exception extracting hostname from {0}", (object) this.configs_.ImagePath);
          Log.LogException(ex, context);
        }
      }
      if (str == null)
        return;
      this.ImageLink.Visibility = Visibility.Visible;
      WaRichText.Chunk chunk3 = new WaRichText.Chunk(0, str.Length, WaRichText.Formats.Link, this.configs_.ImagePath);
      this.ImageLink.Text = new RichTextBlock.TextSet()
      {
        Text = str,
        PartialFormattings = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
        {
          chunk3
        }
      };
    }

    public static IObservable<ImageEditPage.ImageEditPageResults> Start(
      ImageEditPage.ImageEditPageConfigs configs,
      NavigationService callerNav = null)
    {
      return Observable.Create<ImageEditPage.ImageEditPageResults>((Func<IObserver<ImageEditPage.ImageEditPageResults>, Action>) (observer =>
      {
        ImageEditPage.nextInstanceConfigs_ = configs;
        ImageEditPage.nextInstanceObserver_ = observer;
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateToPage(callerNav, nameof (ImageEditPage))));
        return (Action) (() => { });
      }));
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.SlideUpFadeIn), (DependencyObject) this.EditControl);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      bool flag = false;
      if (this.observer_ == null || this.configs_ == null)
        flag = true;
      base.OnNavigatedTo(e);
      if (flag)
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      else
        SysTrayHelper.SetVisible(false);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      if (this.observer_ != null)
        this.observer_.OnCompleted();
      base.OnRemovedFromJournal(e);
    }

    private void Accept_Click(object sender, EventArgs e)
    {
      if (this.actionTaken_)
        return;
      if (this.configs_ == null || this.observer_ == null)
      {
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else
      {
        this.actionTaken_ = true;
        this.ApplicationBar.IsVisible = false;
        this.IsEnabled = false;
        try
        {
          Pair<System.Windows.Point, Size> croppingState = this.EditControl.GetCroppingState(true);
          this.observer_.OnNext(new ImageEditPage.ImageEditPageResults(this.NavigationService)
          {
            RelativeCropPos = croppingState == null ? new System.Windows.Point?() : new System.Windows.Point?(croppingState.First),
            RelativeCropSize = croppingState == null ? new Size?() : new Size?(croppingState.Second)
          });
          this.observer_.OnCompleted();
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "get cropped image");
          this.observer_.OnError(ex);
        }
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ImageEditPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.EditControl = (ImageEditControl) this.FindName("EditControl");
      this.ImageLink = (RichTextBlock) this.FindName("ImageLink");
      this.MaybeCopyright = (RichTextBlock) this.FindName("MaybeCopyright");
    }

    public class ImageEditPageConfigs
    {
      public ImageEditControl.CroppingMode CropMode;
      public double InitialCropRatio = 1.0;
      public ImageEditControl.RotationMode InitialRotateMode;
      public Size? MinRelativeCropSize;
      public ImageEditPage.ImageEditPageConfigs.ImageSource ImageFrom;
      public string ImagePath;

      public BitmapSource OriginalBitmap { get; private set; }

      public ImageEditPageConfigs(BitmapSource bitmap) => this.OriginalBitmap = bitmap;

      public enum ImageSource
      {
        Unknown,
        Bing,
      }
    }

    public class ImageEditPageResults : PageArgs
    {
      public System.Windows.Point? RelativeCropPos { get; set; }

      public Size? RelativeCropSize { get; set; }

      public ImageEditPageResults(NavigationService navService)
        : base(navService)
      {
      }
    }
  }
}
