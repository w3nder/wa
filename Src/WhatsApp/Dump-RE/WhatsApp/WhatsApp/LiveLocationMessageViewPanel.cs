// Decompiled with JetBrains decompiler
// Type: WhatsApp.LiveLocationMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WhatsApp.CommonOps;

#nullable disable
namespace WhatsApp
{
  internal class LiveLocationMessageViewPanel : LocationMessageViewPanel
  {
    private TextBlock liveLocationTextBlock;
    private TextBlock buttonTextBlock;
    private Image liveIcon;
    private RichTextBlock caption;
    private Grid bottomPanel;
    private Grid profilePic;
    private Grid doneSharingOverlay;
    private Image profile;
    private ProgressBar progressIndicator;
    private IDisposable profileSub;

    public LiveLocationMessageViewPanel()
    {
      this.Background = (Brush) UIUtils.TransparentBrush;
      Grid grid1 = new Grid();
      grid1.Background = (Brush) UIUtils.WhiteBrush;
      grid1.Opacity = 0.6;
      grid1.HorizontalAlignment = HorizontalAlignment.Stretch;
      grid1.VerticalAlignment = VerticalAlignment.Stretch;
      this.doneSharingOverlay = grid1;
      this.profilePic = new Grid();
      Ellipse ellipse1 = new Ellipse();
      ellipse1.Width = 64.0;
      ellipse1.Height = 64.0;
      ellipse1.Fill = (Brush) UIUtils.WhiteBrush;
      Ellipse ellipse2 = ellipse1;
      int num1 = 58;
      Image image1 = new Image();
      image1.Width = (double) num1;
      image1.Height = (double) num1;
      image1.Stretch = Stretch.UniformToFill;
      image1.HorizontalAlignment = HorizontalAlignment.Center;
      image1.VerticalAlignment = VerticalAlignment.Center;
      this.profile = image1;
      double num2 = (double) num1 * 0.5;
      this.profile.Clip = (Geometry) new EllipseGeometry()
      {
        Center = new System.Windows.Point(num2, num2),
        RadiusX = num2,
        RadiusY = num2
      };
      this.profilePic.Children.Add((UIElement) ellipse2);
      this.profilePic.Children.Add((UIElement) this.profile);
      Grid.SetRow((FrameworkElement) this.profilePic, 0);
      this.Children.Add((UIElement) this.profilePic);
      Grid.SetRow((FrameworkElement) this.doneSharingOverlay, 0);
      this.Children.Add((UIElement) this.doneSharingOverlay);
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      Grid grid2 = new Grid();
      grid2.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
      Grid element = grid2;
      element.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = GridLength.Auto
      });
      element.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      UIElementCollection children1 = element.Children;
      Image image2 = new Image();
      image2.Source = (System.Windows.Media.ImageSource) AssetStore.InlineLiveLocationWhite;
      image2.Height = 22.0 * this.zoomMultiplier;
      image2.Margin = new Thickness(6.0 * this.zoomMultiplier, 0.0, 0.0, 0.0);
      Image image3 = image2;
      this.liveIcon = image2;
      Image image4 = image3;
      children1.Add((UIElement) image4);
      UIElementCollection children2 = element.Children;
      TextBlock textBlock1 = new TextBlock();
      textBlock1.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock1.TextWrapping = TextWrapping.Wrap;
      textBlock1.FontSize = 22.0 * this.zoomMultiplier;
      textBlock1.IsHitTestVisible = false;
      textBlock1.Margin = new Thickness(6.0 * this.zoomMultiplier, 0.0, 6.0 * this.zoomMultiplier, 2.0);
      TextBlock textBlock2 = textBlock1;
      this.liveLocationTextBlock = textBlock1;
      TextBlock textBlock3 = textBlock2;
      children2.Add((UIElement) textBlock3);
      ProgressBar progressBar = new ProgressBar();
      progressBar.IsIndeterminate = true;
      progressBar.Foreground = (Brush) UIUtils.WhiteBrush;
      progressBar.Padding = new Thickness(0.0);
      progressBar.VerticalAlignment = VerticalAlignment.Top;
      progressBar.HorizontalAlignment = HorizontalAlignment.Stretch;
      progressBar.IsHitTestVisible = false;
      progressBar.Visibility = Visibility.Collapsed;
      this.progressIndicator = progressBar;
      Grid.SetColumnSpan((FrameworkElement) this.progressIndicator, 2);
      element.Children.Add((UIElement) this.progressIndicator);
      Grid.SetRow((FrameworkElement) element, 1);
      Grid.SetColumn((FrameworkElement) this.liveLocationTextBlock, 1);
      this.Children.Add((UIElement) element);
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.Foreground = (Brush) UIUtils.WhiteBrush;
      richTextBlock.TextWrapping = TextWrapping.Wrap;
      richTextBlock.FontSize = Settings.SystemFontSize * this.zoomMultiplier;
      richTextBlock.Margin = new Thickness(-12.0, 0.0, -12.0, 0.0);
      richTextBlock.IsHitTestVisible = false;
      this.caption = richTextBlock;
      Grid.SetRow((FrameworkElement) this.caption, 2);
      this.Children.Add((UIElement) this.caption);
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      Grid grid3 = new Grid();
      grid3.Margin = new Thickness(0.0, 6.0 * this.zoomMultiplier, 0.0, -5.0);
      grid3.Height = 48.0 * this.zoomMultiplier;
      this.bottomPanel = grid3;
      Grid.SetRow((FrameworkElement) this.bottomPanel, 3);
      this.Children.Add((UIElement) this.bottomPanel);
      Grid grid4 = new Grid();
      grid4.Margin = new Thickness(0.0);
      grid4.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
      Grid grid5 = grid4;
      UIElementCollection children3 = grid5.Children;
      TextBlock textBlock4 = new TextBlock();
      textBlock4.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock4.VerticalAlignment = VerticalAlignment.Center;
      textBlock4.HorizontalAlignment = HorizontalAlignment.Center;
      textBlock4.Margin = new Thickness(0.0, 0.0, 0.0, 4.0 * this.zoomMultiplier);
      TextBlock textBlock5 = textBlock4;
      this.buttonTextBlock = textBlock4;
      TextBlock textBlock6 = textBlock5;
      children3.Add((UIElement) textBlock6);
      grid5.Tap += new EventHandler<GestureEventArgs>(this.ViewBtn_Tap);
      this.bottomPanel.Children.Add((UIElement) grid5);
    }

    protected override void DisposeSubscriptions()
    {
      base.DisposeSubscriptions();
      this.profileSub.SafeDispose();
      this.profileSub = (IDisposable) null;
    }

    private void ViewBtn_Tap(object sender, GestureEventArgs e)
    {
      if (Settings.LiveLocationEnabled && this.viewModel.Message.KeyFromMe)
        UIUtils.MessageBox(" ", AppResources.LiveLocationStopSharing, (IEnumerable<string>) new string[2]
        {
          AppResources.Cancel,
          AppResources.LiveLocationStop
        }, (Action<int>) (idx =>
        {
          if (idx != 1)
            return;
          LiveLocationManager.Instance.DisableLocationSharing(this.viewModel.Message.KeyRemoteJid, wam_enum_live_location_sharing_session_ended_reason.USER_CANCELED);
        }));
      else
        ViewMessage.View(this.viewModel.Message);
    }

    public override void Render(MessageViewModel vm)
    {
      if (!(vm is LiveLocationMessageViewModel vm1))
        return;
      base.Render((MessageViewModel) vm1);
      this.UpdateLiveLocation(vm1);
      this.UpdateThumbnail((MessageViewModel) vm1);
      this.UpdateProgressIndicator(vm1);
    }

    public void UpdateLiveLocation(LiveLocationMessageViewModel vm)
    {
      DateTime locationExpirationTime = vm.GetLocationExpirationTime();
      if (vm.IsCurrentlyLive())
      {
        string str = DateTimeUtils.FormatCompactTime(locationExpirationTime).Replace(' ', ' ');
        this.ShowElement((FrameworkElement) this.liveIcon, true);
        this.ShowElement((FrameworkElement) this.bottomPanel, true);
        this.ShowElement((FrameworkElement) this.doneSharingOverlay, false);
        this.IsHitTestVisible = true;
        this.liveLocationTextBlock.Foreground = (Brush) UIUtils.WhiteBrush;
        this.liveLocationTextBlock.Text = Plurals.Instance.GetStringWithIndex(AppResources.LiveLocationLiveUntilPlural, 1, (object) str, (object) (AppState.IsMilitaryTimeDisplayed() || locationExpirationTime.Hour <= 12 ? locationExpirationTime.Hour : locationExpirationTime.Hour - 12));
        this.buttonTextBlock.Text = !Settings.LiveLocationEnabled || !this.viewModel.Message.KeyFromMe ? AppResources.LiveLocationView : AppResources.LiveLocationStopSharingLabel;
      }
      else
      {
        this.ShowElement((FrameworkElement) this.liveIcon, false);
        this.ShowElement((FrameworkElement) this.bottomPanel, false);
        this.ShowElement((FrameworkElement) this.doneSharingOverlay, true);
        this.IsHitTestVisible = false;
        this.liveLocationTextBlock.Foreground = UIUtils.SubtleBrushWhite;
        this.liveLocationTextBlock.Text = AppResources.LiveLocationEnded;
      }
      this.ShowElement((FrameworkElement) this.caption, true);
      this.caption.Text = new RichTextBlock.TextSet()
      {
        Text = vm.GetCaption()
      };
      System.Windows.Media.ImageSource cachedImgSrc = (System.Windows.Media.ImageSource) null;
      if (!ChatPictureStore.GetCache(vm.SenderJid, out cachedImgSrc))
      {
        this.profileSub.SafeDispose();
        this.profileSub = ChatPictureStore.Get(vm.SenderJid, false, false, true).Select<ChatPictureStore.PicState, System.Windows.Media.ImageSource>((Func<ChatPictureStore.PicState, System.Windows.Media.ImageSource>) (picState => (System.Windows.Media.ImageSource) picState.Image)).SubscribeOn<System.Windows.Media.ImageSource>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<System.Windows.Media.ImageSource>().Subscribe<System.Windows.Media.ImageSource>((Action<System.Windows.Media.ImageSource>) (imgSrc => this.profile.Source = imgSrc != null ? imgSrc : (System.Windows.Media.ImageSource) AssetStore.DefaultContactIconBlack));
      }
      else
        this.profile.Source = cachedImgSrc ?? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIconBlack;
    }

    private void UpdateProgressIndicator(LiveLocationMessageViewModel vm)
    {
      this.progressIndicator.Visibility = this.IsPendingLiveLocationMessage(vm) ? Visibility.Visible : Visibility.Collapsed;
    }

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.LiveLocation;

    public override void OnTap(object sender, EventArgs e)
    {
      if (this.IsPendingLiveLocationMessage(this.viewModel as LiveLocationMessageViewModel))
        return;
      base.OnTap(sender, e);
    }

    private bool IsPendingLiveLocationMessage(LiveLocationMessageViewModel vm)
    {
      return vm != null && vm.IsCurrentlyLive() && vm.Message.KeyFromMe && vm.Message.Status == FunXMPP.FMessage.Status.Pending;
    }

    public override void ProcessViewModelNotification(KeyValuePair<string, object> args)
    {
      base.ProcessViewModelNotification(args);
      if (!(args.Key == "StatusChanged"))
        return;
      this.UpdateProgressIndicator(this.viewModel as LiveLocationMessageViewModel);
    }
  }
}
