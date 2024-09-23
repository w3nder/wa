// Decompiled with JetBrains decompiler
// Type: WhatsApp.StickerMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WhatsApp.CommonOps;

#nullable disable
namespace WhatsApp
{
  public class StickerMessageViewPanel : MessageViewPanel
  {
    private Image thumbnailImage;
    private IDisposable thumbSub;
    private IDisposable activeAnimationSub;
    private ProgressBar transferProgressBar;
    private ButtonWP10 actionButton;
    private ProgressBar transferIndicator;
    private new const string LogHeader = "sticker msgbubble";

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Sticker;

    public StickerMessageViewPanel()
    {
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      Image image = new Image();
      image.Margin = new Thickness(0.0, 16.0 * this.zoomMultiplier, 0.0, 32.0 * this.zoomMultiplier);
      image.Source = AssetStore.StickerThumbnailPlaceholder;
      image.Opacity = 0.0;
      this.thumbnailImage = image;
      Grid.SetRow((FrameworkElement) this.thumbnailImage, 0);
      this.Children.Add((UIElement) this.thumbnailImage);
    }

    public override void Render(MessageViewModel vm)
    {
      if (!(vm is StickerMessageViewModel vm1))
        return;
      base.Render(vm);
      Log.d("sticker msgbubble", nameof (Render), (object) vm1.Message.LogInfo());
      this.UpdateThumbnail((MessageViewModel) vm1);
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.thumbnailImage.Source = (System.Windows.Media.ImageSource) null;
    }

    protected override void DisposeSubscriptions()
    {
      base.DisposeSubscriptions();
      IDisposable sub = this.activeAnimationSub;
      if (sub != null)
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
        {
          sub.SafeDispose();
          sub = (IDisposable) null;
        }));
      this.thumbSub.SafeDispose();
      this.thumbSub = (IDisposable) null;
    }

    private void UpdateThumbnail(MessageViewModel vm)
    {
      if (!(vm is StickerMessageViewModel messageViewModel))
        return;
      this.thumbnailImage.Width = this.thumbnailImage.Height = messageViewModel.StickerWidth;
      this.thumbnailImage.HorizontalAlignment = messageViewModel.StickerAlignment;
      this.thumbnailImage.Source = AssetStore.StickerThumbnailPlaceholder;
      this.thumbnailImage.Opacity = 0.0;
      this.thumbSub.SafeDispose();
      this.thumbSub = vm.GetThumbnailObservable().SubscribeOn<MessageViewModel.ThumbnailState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<MessageViewModel.ThumbnailState>().Subscribe<MessageViewModel.ThumbnailState>(new Action<MessageViewModel.ThumbnailState>(this.OnThumbnailState));
    }

    private void UpdateActionButton(FileMessageViewModelBase vm)
    {
      if (vm == null || !(vm is StickerMessageViewModel messageViewModel))
        return;
      if (vm.ShouldShowActionButton)
      {
        if (this.actionButton == null)
        {
          double left = (messageViewModel.StickerWidth - 96.0 * this.zoomMultiplier) / 2.0;
          ButtonWP10 buttonWp10 = new ButtonWP10();
          buttonWp10.ButtonBrush = (Brush) UIUtils.WhiteBrush;
          buttonWp10.ButtonSize = 96.0 * this.zoomMultiplier;
          buttonWp10.Margin = new Thickness(left, 16.0 * this.zoomMultiplier, 0.0, 32.0 * this.zoomMultiplier);
          buttonWp10.HorizontalAlignment = HorizontalAlignment.Left;
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

    private void UpdateTransferIndicator(FileMessageViewModelBase vm)
    {
      StickerMessageViewModel messageViewModel = (StickerMessageViewModel) vm;
      if (messageViewModel == null)
        return;
      if (vm.ShouldShowTransferStatusBar)
      {
        if (this.transferIndicator == null)
        {
          ProgressBar progressBar = new ProgressBar();
          progressBar.IsIndeterminate = true;
          progressBar.Foreground = (Brush) UIUtils.WhiteBrush;
          progressBar.Padding = new Thickness(0.0, 16.0 * this.zoomMultiplier, 0.0, 32.0 * this.zoomMultiplier);
          progressBar.VerticalAlignment = VerticalAlignment.Bottom;
          progressBar.HorizontalAlignment = HorizontalAlignment.Left;
          progressBar.IsHitTestVisible = false;
          progressBar.Width = messageViewModel.StickerWidth;
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
      StickerMessageViewModel messageViewModel = (StickerMessageViewModel) vm;
      if (messageViewModel == null)
        return;
      if (vm.ShouldShowTransferProgressBar)
      {
        if (this.transferProgressBar == null)
        {
          ProgressBar progressBar = new ProgressBar();
          progressBar.Foreground = (Brush) UIUtils.WhiteBrush;
          progressBar.Padding = new Thickness(0.0, 16.0 * this.zoomMultiplier, 0.0, 32.0 * this.zoomMultiplier);
          progressBar.VerticalAlignment = VerticalAlignment.Bottom;
          progressBar.HorizontalAlignment = HorizontalAlignment.Left;
          progressBar.Maximum = 1.0;
          progressBar.IsHitTestVisible = false;
          progressBar.Width = messageViewModel.StickerWidth;
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

    public override void ProcessViewModelNotification(KeyValuePair<string, object> args)
    {
      base.ProcessViewModelNotification(args);
      if (!(args.Key == "TransferStateChanged"))
        return;
      this.OnTransferStateChanged();
    }

    private void OnThumbnailState(MessageViewModel.ThumbnailState thumbState)
    {
      Log.d("sticker msgbubble", nameof (OnThumbnailState), (object) this.viewModel.Message.LogInfo());
      if (!(this.viewModel is StickerMessageViewModel viewModel))
        return;
      Storyboard sb = viewModel.Animation;
      if (viewModel.ShouldShowTransferStatusBar)
        this.thumbnailImage.Opacity = 0.0;
      else if (sb != null)
      {
        this.thumbnailImage.Source = thumbState.Image;
        this.thumbnailImage.Opacity = 1.0;
        this.activeAnimationSub = Storyboarder.PerformWithDisposable(sb, (DependencyObject) this.thumbnailImage, false, (Action) null, (Action) (() => sb.Stop()), "large sticker animation");
      }
      else if (thumbState.Image != null && this.viewModel != null && thumbState.KeyId == this.viewModel.Message.KeyId)
      {
        this.thumbnailImage.Source = thumbState.Image;
        this.thumbnailImage.Opacity = 1.0;
      }
      else if (!this.viewModel.Message.TransferInProgress)
        this.thumbnailImage.Opacity = 1.0;
      this.UpdateActionButton((FileMessageViewModelBase) viewModel);
      this.UpdateTransferProgressBar((FileMessageViewModelBase) viewModel);
      this.UpdateTransferIndicator((FileMessageViewModelBase) viewModel);
    }

    private void OnTransferStateChanged()
    {
      if (!(this.viewModel is FileMessageViewModelBase viewModel))
        return;
      Log.d("sticker msgbubble", nameof (OnTransferStateChanged), (object) viewModel.Message.LogInfo());
      this.UpdateThumbnail(this.viewModel);
      this.UpdateTransferProgressBar(viewModel);
      this.UpdateTransferIndicator(viewModel);
      this.UpdateActionButton(viewModel);
    }

    protected virtual void ActionButton_Click(object sender, EventArgs e)
    {
      Log.d("sticker msgbubble", "Download button clicked");
      ViewMessage.ToggleDownload(this.viewModel.Message);
    }
  }
}
