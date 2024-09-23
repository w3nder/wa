// Decompiled with JetBrains decompiler
// Type: WhatsApp.ImageMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WhatsApp.Controls;


namespace WhatsApp
{
  public class ImageMessageViewPanel : MessageViewPanel
  {
    private Image thumbnailImage;
    private RichTextBlock captionBlock;
    private Rectangle foregroundProtection;
    private Rectangle thumbBackground;
    private ButtonWP10 actionButton;
    private SmallDownloadButton smallDownloadButton;
    private ProgressBar transferIndicator;
    private ProgressBar transferProgressBar;
    private TextBlock mediaInfoBlock;
    private IDisposable thumbSub;
    private string currThumbKey;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.ImageAndVideo;

    public ImageMessageViewPanel()
    {
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      Rectangle rectangle = new Rectangle();
      rectangle.Fill = UIUtils.InactiveBrush;
      this.thumbBackground = rectangle;
      Grid.SetRow((FrameworkElement) this.thumbBackground, 0);
      this.Children.Add((UIElement) this.thumbBackground);
      Image image = new Image();
      image.IsHitTestVisible = false;
      this.thumbnailImage = image;
      Grid.SetRow((FrameworkElement) this.thumbnailImage, 0);
      this.Children.Add((UIElement) this.thumbnailImage);
      this.thumbBackground.Tap += new EventHandler<GestureEventArgs>(this.ThumbnailImage_Tap);
    }

    public override void Render(MessageViewModel vm)
    {
      if (!(vm is FileMessageViewModelBase vm1))
        return;
      base.Render(vm);
      this.ClearText();
      this.UpdateThumbnail(vm);
      this.UpdateActionButton(vm1);
      this.UpdateAccessoryButton(vm1);
      this.UpdateMediaInfo(vm1);
      this.UpdateTransferProgressBar(vm1);
      this.UpdateTransferIndicator(vm1);
      this.UpdateCaption(vm1);
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.thumbnailImage.Source = (System.Windows.Media.ImageSource) null;
      this.currThumbKey = (string) null;
      this.ClearText();
    }

    protected override void DisposeSubscriptions()
    {
      base.DisposeSubscriptions();
      this.thumbSub.SafeDispose();
      this.thumbSub = (IDisposable) null;
    }

    private void ClearText()
    {
      if (this.captionBlock == null)
        return;
      this.ShowElement((FrameworkElement) this.captionBlock, false);
      this.captionBlock.Text = (RichTextBlock.TextSet) null;
    }

    private void UpdateThumbnail(MessageViewModel vm)
    {
      if (vm == null)
        return;
      this.thumbnailImage.Width = this.thumbBackground.Width = vm.ThumbnailWidth;
      this.thumbnailImage.Height = this.thumbBackground.Height = vm.ThumbnailHeight;
      if (this.currThumbKey == null && vm.Message.BinaryData != null)
        vm.GetThumbnailObservable(MessageViewModel.ThumbnailOptions.GetSmall).Subscribe<MessageViewModel.ThumbnailState>(new Action<MessageViewModel.ThumbnailState>(this.OnThumbnailState));
      this.thumbSub.SafeDispose();
      this.thumbSub = vm.GetThumbnailObservable().SubscribeOn<MessageViewModel.ThumbnailState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<MessageViewModel.ThumbnailState>().Subscribe<MessageViewModel.ThumbnailState>(new Action<MessageViewModel.ThumbnailState>(this.OnThumbnailState));
    }

    private void UpdateMediaInfo(FileMessageViewModelBase vm)
    {
      if (vm == null)
        return;
      if (vm.ShouldShowMediaInfo)
      {
        if (this.mediaInfoBlock == null)
        {
          TextBlock textBlock = new TextBlock();
          textBlock.Opacity = 0.75;
          textBlock.Foreground = (Brush) UIUtils.WhiteBrush;
          textBlock.FontFamily = (FontFamily) this.Resources[(object) "PhoneFontFamilyLight"];
          textBlock.IsHitTestVisible = false;
          textBlock.HorizontalAlignment = HorizontalAlignment.Left;
          textBlock.VerticalAlignment = VerticalAlignment.Bottom;
          textBlock.Margin = new Thickness(12.0 * this.zoomMultiplier, 0.0, 0.0, 0.0);
          textBlock.FontSize = 56.0 * this.zoomMultiplier;
          this.mediaInfoBlock = textBlock;
          Grid.SetRow((FrameworkElement) this.mediaInfoBlock, 0);
          this.Children.Add((UIElement) this.mediaInfoBlock);
        }
        this.mediaInfoBlock.Text = vm.MediaInfoStr;
        this.ShowElement((FrameworkElement) this.mediaInfoBlock, true);
      }
      else
        this.ShowElement((FrameworkElement) this.mediaInfoBlock, false);
    }

    private void UpdateActionButton(FileMessageViewModelBase vm)
    {
      if (vm == null)
        return;
      if (vm.ShouldShowActionButton)
      {
        if (this.actionButton == null)
        {
          ButtonWP10 buttonWp10 = new ButtonWP10();
          buttonWp10.ButtonBrush = (Brush) UIUtils.WhiteBrush;
          buttonWp10.ButtonSize = 96.0 * this.zoomMultiplier;
          buttonWp10.HorizontalAlignment = HorizontalAlignment.Center;
          buttonWp10.VerticalAlignment = VerticalAlignment.Center;
          this.actionButton = buttonWp10;
          this.actionButton.Click += new EventHandler(this.ActionButton_Click);
          Grid.SetRow((FrameworkElement) this.actionButton, 0);
          this.Children.Add((UIElement) this.actionButton);
        }
        this.actionButton.ButtonIconImage = (System.Windows.Media.ImageSource) vm.ActionButtonIcon;
        this.ShowElement((FrameworkElement) this.actionButton, true);
      }
      else
      {
        if (this.actionButton == null)
          return;
        this.ShowElement((FrameworkElement) this.actionButton, false);
        this.actionButton.ButtonIconImage = (System.Windows.Media.ImageSource) null;
      }
    }

    private void UpdateAccessoryButton(FileMessageViewModelBase vm)
    {
      if (vm == null)
        return;
      if (vm.ShouldShowAccessoryButton)
      {
        if (this.smallDownloadButton == null)
        {
          SmallDownloadButton smallDownloadButton = new SmallDownloadButton();
          smallDownloadButton.HorizontalAlignment = HorizontalAlignment.Left;
          smallDownloadButton.VerticalAlignment = VerticalAlignment.Bottom;
          this.smallDownloadButton = smallDownloadButton;
          this.smallDownloadButton.Click += new EventHandler(this.AccessoryButton_Click);
          Grid.SetRow((FrameworkElement) this.smallDownloadButton, 0);
          this.Children.Add((UIElement) this.smallDownloadButton);
        }
        this.smallDownloadButton.IsDownloading = vm.Message.TransferInProgress && !vm.Message.IsPrefetchingVideo;
        this.smallDownloadButton.UpdateDownloadSize(vm.MediaInfoStr);
        this.smallDownloadButton.UpdateDownloadProgress(vm.TransferProgressBarValue);
        this.ShowElement((FrameworkElement) this.smallDownloadButton, true);
      }
      else
      {
        if (this.smallDownloadButton == null)
          return;
        this.ShowElement((FrameworkElement) this.smallDownloadButton, false);
      }
    }

    private void UpdateTransferIndicator(FileMessageViewModelBase vm)
    {
      if (vm == null)
        return;
      if (vm.ShouldShowTransferStatusBar)
      {
        if (this.transferIndicator == null)
        {
          ProgressBar progressBar = new ProgressBar();
          progressBar.IsIndeterminate = true;
          progressBar.Foreground = (Brush) UIUtils.WhiteBrush;
          progressBar.Padding = new Thickness(0.0);
          progressBar.VerticalAlignment = VerticalAlignment.Bottom;
          progressBar.HorizontalAlignment = HorizontalAlignment.Stretch;
          progressBar.IsHitTestVisible = false;
          this.transferIndicator = progressBar;
          Grid.SetRow((FrameworkElement) this.transferIndicator, 0);
          this.Children.Add((UIElement) this.transferIndicator);
        }
        this.ShowElement((FrameworkElement) this.transferIndicator, true);
      }
      else
        this.ShowElement((FrameworkElement) this.transferIndicator, false);
    }

    private void UpdateTransferProgressBar(FileMessageViewModelBase vm)
    {
      if (vm == null)
        return;
      double progressBarValue = vm.TransferProgressBarValue;
      if (vm.ShouldShowTransferProgressBar)
      {
        if (this.transferProgressBar == null)
        {
          ProgressBar progressBar = new ProgressBar();
          progressBar.Foreground = (Brush) UIUtils.WhiteBrush;
          progressBar.Padding = new Thickness(0.0);
          progressBar.VerticalAlignment = VerticalAlignment.Bottom;
          progressBar.HorizontalAlignment = HorizontalAlignment.Stretch;
          progressBar.Maximum = 1.0;
          progressBar.IsHitTestVisible = false;
          this.transferProgressBar = progressBar;
          Grid.SetRow((FrameworkElement) this.transferProgressBar, 0);
          this.Children.Add((UIElement) this.transferProgressBar);
        }
        this.transferProgressBar.Value = vm.TransferProgressBarValue;
        this.ShowElement((FrameworkElement) this.transferProgressBar, true);
      }
      else
        this.ShowElement((FrameworkElement) this.transferProgressBar, false);
    }

    private void UpdateCaption(FileMessageViewModelBase vm)
    {
      if (vm.ShouldShowText)
      {
        if (this.captionBlock == null)
        {
          RichTextBlock richTextBlock = new RichTextBlock();
          richTextBlock.Foreground = (Brush) UIUtils.WhiteBrush;
          richTextBlock.TextWrapping = TextWrapping.Wrap;
          richTextBlock.Margin = new Thickness(-12.0, 6.0 * this.zoomMultiplier, -12.0, 0.0);
          richTextBlock.LargeEmojiSize = true;
          this.captionBlock = richTextBlock;
          Grid.SetRow((FrameworkElement) this.captionBlock, 1);
          this.Children.Add((UIElement) this.captionBlock);
        }
        this.captionBlock.AllowLinks = vm.AllowLinks;
        this.captionBlock.FontSize = vm.TextFontSize;
        this.captionBlock.Text = new RichTextBlock.TextSet()
        {
          Text = vm.TextStr,
          SerializedFormatting = (IEnumerable<LinkDetector.Result>) vm.TextPerformanceHint
        };
        if (vm.TextFlowDirection.HasValue)
        {
          RichTextBlock captionBlock = this.captionBlock;
          FlowDirection? textFlowDirection = vm.TextFlowDirection;
          int num = (int) textFlowDirection.Value;
          captionBlock.FlowDirection = (FlowDirection) num;
          CultureInfo cult = new CultureInfo(AppResources.CultureString);
          if (cult.IsRightToLeft())
          {
            textFlowDirection = vm.TextFlowDirection;
            FlowDirection flowDirection = FlowDirection.LeftToRight;
            if ((textFlowDirection.GetValueOrDefault() == flowDirection ? (textFlowDirection.HasValue ? 1 : 0) : 0) != 0)
              goto label_8;
          }
          if (!cult.IsRightToLeft())
          {
            textFlowDirection = vm.TextFlowDirection;
            FlowDirection flowDirection = FlowDirection.RightToLeft;
            if ((textFlowDirection.GetValueOrDefault() == flowDirection ? (textFlowDirection.HasValue ? 1 : 0) : 0) == 0 || !vm.ShouldShowFooter)
              goto label_9;
          }
          else
            goto label_9;
label_8:
          this.captionBlock.Margin = new Thickness(0.0, 0.0, 0.0, 24.0);
        }
label_9:
        this.captionBlock.Visibility = Visibility.Visible;
      }
      else
        this.ShowElement((FrameworkElement) this.captionBlock, false);
    }

    private void ShowFooterProtection(bool show)
    {
      if (show)
      {
        if (this.foregroundProtection == null)
        {
          Rectangle rectangle = new Rectangle();
          rectangle.HorizontalAlignment = HorizontalAlignment.Right;
          rectangle.VerticalAlignment = VerticalAlignment.Bottom;
          rectangle.Stretch = Stretch.Fill;
          rectangle.IsHitTestVisible = false;
          rectangle.Width = MessageViewModel.DefaultContentWidth;
          rectangle.Height = 72.0 * this.zoomMultiplier;
          this.foregroundProtection = rectangle;
          Rectangle foregroundProtection = this.foregroundProtection;
          LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
          GradientStopCollection gradientStopCollection = new GradientStopCollection();
          gradientStopCollection.Add(new GradientStop()
          {
            Color = Color.FromArgb((byte) 143, (byte) 0, (byte) 0, (byte) 0),
            Offset = 0.0
          });
          gradientStopCollection.Add(new GradientStop()
          {
            Color = Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0),
            Offset = 1.0
          });
          linearGradientBrush.GradientStops = gradientStopCollection;
          linearGradientBrush.StartPoint = new System.Windows.Point(0.0, 1.0);
          linearGradientBrush.EndPoint = new System.Windows.Point(0.0, 0.0);
          foregroundProtection.Fill = (Brush) linearGradientBrush;
          Grid.SetRow((FrameworkElement) this.foregroundProtection, 0);
          this.Children.Add((UIElement) this.foregroundProtection);
        }
        this.ShowElement((FrameworkElement) this.foregroundProtection, true);
      }
      else
        this.ShowElement((FrameworkElement) this.foregroundProtection, false);
    }

    public override void ProcessViewModelNotification(KeyValuePair<string, object> args)
    {
      base.ProcessViewModelNotification(args);
      string key = args.Key;
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(key))
      {
        case 482861153:
          if (!(key == "AllowLinksChanged"))
            break;
          this.OnAllowLinksChanged();
          break;
        case 1111477559:
          if (!(key == "TransferProgressChanged"))
            break;
          this.OnTransferProgressChanged();
          break;
        case 1823672673:
          if (!(key == "TransferStateChanged"))
            break;
          this.OnTransferStateChanged();
          break;
        case 1889495869:
          if (!(key == "StatusChanged"))
            break;
          this.OnStatusChanged();
          break;
        case 2945049045:
          if (!(key == "MediaDurationChanged"))
            break;
          this.OnMediaDurationChanged();
          break;
        case 2946420676:
          if (!(key == "LocalFileUriChanged"))
            break;
          this.OnLocalFileUriChanged();
          break;
        case 3273603205:
          if (!(key == "ThumbnailChanged"))
            break;
          this.UpdateThumbnail(this.viewModel);
          break;
        case 4034760466:
          if (!(key == "TextFontSizeChanged"))
            break;
          this.OnTextFontSizeChanged();
          break;
      }
    }

    protected virtual Stretch ThumbnailStretch => Stretch.UniformToFill;

    private void OnThumbnailState(MessageViewModel.ThumbnailState thumbState)
    {
      if (thumbState.Image != null && this.viewModel != null && thumbState.KeyId == this.viewModel.Message.KeyId)
      {
        this.thumbnailImage.Stretch = this.ThumbnailStretch;
        this.thumbnailImage.Source = thumbState.Image;
        this.ShowFooterProtection(!this.viewModel.ShouldShowText);
      }
      else
      {
        this.thumbnailImage.Stretch = Stretch.None;
        this.thumbnailImage.Source = (System.Windows.Media.ImageSource) AssetStore.GetMessageDefaultIcon(FunXMPP.FMessage.Type.Image);
        this.ShowFooterProtection(false);
      }
      if (this.viewModel == null)
        return;
      this.currThumbKey = this.viewModel.Message.KeyId;
    }

    private void OnLocalFileUriChanged()
    {
      if (!(this.viewModel is FileMessageViewModelBase viewModel))
        return;
      this.UpdateTransferIndicator(viewModel);
      this.UpdateTransferProgressBar(viewModel);
      this.UpdateMediaInfo(viewModel);
      this.UpdateActionButton(viewModel);
      this.UpdateAccessoryButton(viewModel);
    }

    private void OnStatusChanged()
    {
      if (!this.viewModel.Message.KeyFromMe || this.viewModel.Message.IsRevoked() || !(this.viewModel is FileMessageViewModelBase viewModel))
        return;
      this.UpdateTransferProgressBar(viewModel);
      this.UpdateTransferIndicator(viewModel);
      this.UpdateMediaInfo(viewModel);
      this.UpdateActionButton(viewModel);
      this.UpdateAccessoryButton(viewModel);
    }

    private void OnMediaDurationChanged()
    {
      FileMessageViewModelBase viewModel = this.viewModel as FileMessageViewModelBase;
      this.UpdateMediaInfo(viewModel);
      this.UpdateAccessoryButton(viewModel);
    }

    private void OnTransferProgressChanged()
    {
      FileMessageViewModelBase viewModel = this.viewModel as FileMessageViewModelBase;
      this.UpdateTransferProgressBar(viewModel);
      this.UpdateAccessoryButton(viewModel);
    }

    private void OnTransferStateChanged()
    {
      if (!(this.viewModel is FileMessageViewModelBase viewModel))
        return;
      if (this.viewModel.Message.ShouldPrefetchVideo() && this.viewModel.IsThumbnailAvailable())
        this.UpdateThumbnail(this.viewModel);
      this.UpdateTransferIndicator(viewModel);
      this.UpdateTransferProgressBar(viewModel);
      this.UpdateMediaInfo(viewModel);
      this.UpdateActionButton(viewModel);
      this.UpdateAccessoryButton(viewModel);
    }

    private void OnTextFontSizeChanged()
    {
      if (this.captionBlock == null)
        return;
      this.captionBlock.FontSize = this.viewModel.TextFontSize;
    }

    private void OnAllowLinksChanged()
    {
      if (this.viewModel == null || this.captionBlock == null)
        return;
      this.captionBlock.AllowLinks = this.viewModel.AllowLinks;
      this.captionBlock.Refresh();
    }

    protected virtual void ThumbnailImage_Tap(object sender, EventArgs e)
    {
      if (this.viewModel.Message.Status == FunXMPP.FMessage.Status.Pending)
        return;
      this.ViewMessage();
    }

    protected virtual void ActionButton_Click(object sender, EventArgs e) => this.ViewMessage();

    protected virtual void AccessoryButton_Click(object sender, EventArgs e)
    {
      Log.d("msgbubble", "Download button clicked");
      WhatsApp.CommonOps.ViewMessage.ToggleDownload(this.viewModel.Message);
    }

    protected virtual void ViewMessage() => WhatsApp.CommonOps.ViewMessage.View(this.viewModel.Message);

    protected virtual void ForceDownload()
    {
      WhatsApp.CommonOps.ViewMessage.View(this.viewModel.Message, forceDownload: true);
    }
  }
}
