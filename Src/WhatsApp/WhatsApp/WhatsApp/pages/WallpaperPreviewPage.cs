// Decompiled with JetBrains decompiler
// Type: WhatsApp.WallpaperPreviewPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class WallpaperPreviewPage : PhoneApplicationPage
  {
    private static Dictionary<int, WallpaperPreviewPage.ControlState> observers = new Dictionary<int, WallpaperPreviewPage.ControlState>();
    private static TicketCounter imageCount = new TicketCounter();
    private static object observerLock = new object();
    private GlobalProgressIndicator globalProgress;
    private WallpaperPreviewPage.ControlState state_;
    private WriteableBitmap bitmap_;
    internal Grid LayoutRoot;
    internal ZoomableImageFrame ImageZoom;
    internal Rectangle TopGradient;
    internal WallpaperPreviewForeground PreviewForeground;
    private bool _contentLoaded;

    public WallpaperPreviewPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.globalProgress = new GlobalProgressIndicator((DependencyObject) this);
      this.ImageZoom.FrameFitMode = ZoomableImageFrame.FrameFitModeValue.FRAME_WITHIN_IMAGE;
      this.PreviewForeground.Margin = new Thickness(0.0, UIUtils.SystemTraySizePortrait, 0.0, 0.0);
      this.PreviewForeground.SetTexts(AppResources.SetWallpaper, AppResources.ZoomWallpaper, AppResources.CropWallpaper);
    }

    public static IObservable<WallpaperPreviewPage.WallpaperPreviewArgs> Start(
      NavigationService nav,
      WriteableBitmap bitmap,
      bool isGlobalWallpaper = false)
    {
      return Observable.Create<WallpaperPreviewPage.WallpaperPreviewArgs>((Func<IObserver<WallpaperPreviewPage.WallpaperPreviewArgs>, Action>) (observer =>
      {
        int? nullable = new int?();
        try
        {
          WallpaperPreviewPage.ControlState controlState = new WallpaperPreviewPage.ControlState()
          {
            Bitmap = bitmap,
            Observer = observer,
            IsGlobal = isGlobalWallpaper
          };
          nullable = new int?(WallpaperPreviewPage.imageCount.NextTicket());
          lock (WallpaperPreviewPage.observerLock)
            WallpaperPreviewPage.observers[nullable.Value] = controlState;
          Uri uri = UriUtils.CreatePageUri(nameof (WallpaperPreviewPage), string.Format("id={0}", (object) nullable.Value));
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() => nav.Navigate(uri)));
        }
        catch (Exception ex)
        {
          if (nullable.HasValue)
          {
            lock (WallpaperPreviewPage.observerLock)
              WallpaperPreviewPage.observers.Remove(nullable.Value);
          }
          observer.OnError(ex);
        }
        return (Action) (() => { });
      }));
    }

    private void ResetCanvas()
    {
      this.ImageZoom.Width = Application.Current.Host.Content.ActualWidth;
      this.ImageZoom.Height = Application.Current.Host.Content.ActualHeight;
      this.ImageZoom.SetScaleLimit();
    }

    private WriteableBitmap GetCroppedBitmap()
    {
      System.Windows.Point pos;
      Size size;
      this.ImageZoom.GetInFrameImageArea(out pos, out size);
      double scale = ResolutionHelper.GetPixelSize().Width / size.Width;
      return this.bitmap_.Crop(pos, size).Scale(scale);
    }

    private void Accept_Click(object sender, EventArgs e)
    {
      if (this.bitmap_ == null || !this.IsEnabled)
        return;
      Action release = (Action) (() => { });
      WallpaperPreviewPage.WallpaperPreviewArgs wallpaperPreviewArgs = new WallpaperPreviewPage.WallpaperPreviewArgs()
      {
        OnComplete = (Action) (() => this.Dispatcher.BeginInvokeIfNeeded(release))
      };
      this.globalProgress.Acquire();
      this.ApplicationBar.IsVisible = false;
      this.IsEnabled = false;
      release = (Action) (() =>
      {
        release = (Action) (() => { });
        this.globalProgress.Release();
        this.ApplicationBar.IsVisible = true;
        this.IsEnabled = true;
        if (!this.state_.IsGlobal)
          this.NavigationService.RemoveBackEntry();
        if (!this.NavigationService.CanGoBack)
          return;
        this.NavigationService.GoBack();
      });
      try
      {
        wallpaperPreviewArgs.Bitmap = this.GetCroppedBitmap();
        this.state_.Observer.OnNext(wallpaperPreviewArgs);
      }
      catch (Exception ex)
      {
        try
        {
          this.state_.Observer.OnError(ex);
        }
        finally
        {
          release();
        }
      }
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this.state_ != null && this.state_.Observer != null)
        this.state_.Observer.OnCompleted();
      base.OnBackKeyPress(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (this.state_ == null)
      {
        string s = (string) null;
        int result;
        if (!this.NavigationContext.QueryString.TryGetValue("id", out s) || !int.TryParse(s, out result))
          throw new InvalidOperationException("Could not determine ID");
        lock (WallpaperPreviewPage.observerLock)
        {
          WallpaperPreviewPage.observers.TryGetValue(result, out this.state_);
          WallpaperPreviewPage.observers.Remove(result);
        }
        if (this.state_ == null)
        {
          this.NavigationService.GoBack();
          return;
        }
        this.bitmap_ = this.state_.Bitmap;
        this.ImageZoom.SetImageSource(this.bitmap_);
        this.ResetCanvas();
      }
      SysTrayHelper.SetVisible(false);
      base.OnNavigatedTo(e);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/WallpaperPreviewPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ImageZoom = (ZoomableImageFrame) this.FindName("ImageZoom");
      this.TopGradient = (Rectangle) this.FindName("TopGradient");
      this.PreviewForeground = (WallpaperPreviewForeground) this.FindName("PreviewForeground");
    }

    public class ControlState
    {
      public WriteableBitmap Bitmap;
      public IObserver<WallpaperPreviewPage.WallpaperPreviewArgs> Observer;
      public bool IsGlobal;
    }

    public struct WallpaperPreviewArgs
    {
      public WriteableBitmap Bitmap;
      public Action OnComplete;
    }
  }
}
