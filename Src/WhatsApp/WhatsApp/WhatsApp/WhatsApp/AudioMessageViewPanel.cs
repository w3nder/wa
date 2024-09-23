// Decompiled with JetBrains decompiler
// Type: WhatsApp.AudioMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WhatsApp.CommonOps;


namespace WhatsApp
{
  public class AudioMessageViewPanel : MessageViewPanel
  {
    private Image thumbnailImage;
    private Image pttMicIcon;
    private RoundButton actionButton;
    private ProgressBar transferIndicator;
    private PlaybackSlider playbackBar;
    private TextBlock durationBlock;
    private TextBlock infoBlock;
    private Rectangle thumbBackground;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Audio;

    public AudioMessageViewPanel()
    {
      this.Width = MessageViewModel.DefaultContentWidth + 12.0 * this.zoomMultiplier;
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = GridLength.Auto
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = GridLength.Auto
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      Rectangle rectangle = new Rectangle();
      rectangle.Fill = UIUtils.InactiveBrush;
      this.thumbBackground = rectangle;
      this.Children.Add((UIElement) this.thumbBackground);
      this.thumbnailImage = new Image()
      {
        Stretch = Stretch.UniformToFill
      };
      this.Children.Add((UIElement) this.thumbnailImage);
      RoundButton roundButton = new RoundButton();
      roundButton.ButtonBrush = UIUtils.WhiteBrush;
      roundButton.HorizontalAlignment = HorizontalAlignment.Center;
      roundButton.VerticalAlignment = VerticalAlignment.Center;
      roundButton.ButtonSize = 46.0 * this.zoomMultiplier;
      this.actionButton = roundButton;
      this.actionButton.Click += new EventHandler(this.ActionButton_Click);
      this.Children.Add((UIElement) this.actionButton);
      TextBlock textBlock1 = new TextBlock();
      textBlock1.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock1.IsHitTestVisible = false;
      textBlock1.VerticalAlignment = VerticalAlignment.Center;
      textBlock1.Visibility = Visibility.Collapsed;
      textBlock1.FontSize = 24.0 * this.zoomMultiplier;
      this.infoBlock = textBlock1;
      this.Children.Add((UIElement) this.infoBlock);
      PlaybackSlider playbackSlider = new PlaybackSlider();
      playbackSlider.Maximum = 1.0;
      playbackSlider.VerticalAlignment = VerticalAlignment.Center;
      this.playbackBar = playbackSlider;
      this.playbackBar.SeekStarted += new EventHandler(this.PlaybackBar_SeekStarted);
      this.playbackBar.SeekCompleted += new EventHandler<PlaybackSlider.PlaybackSliderSeekArgs>(this.PlaybackBar_SeekCompleted);
      this.Children.Add((UIElement) this.playbackBar);
      TextBlock textBlock2 = new TextBlock();
      textBlock2.FontSize = MessageViewModel.SmallTextFontSizeStatic;
      textBlock2.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
      textBlock2.VerticalAlignment = VerticalAlignment.Center;
      this.durationBlock = textBlock2;
      this.Children.Add((UIElement) this.durationBlock);
    }

    public override void Render(MessageViewModel vm)
    {
      if (!(vm is AudioMessageViewModel amvm))
        return;
      base.Render(vm);
      bool show = amvm.Message.IsPtt();
      if (show)
      {
        if (!(amvm is PttMessageViewModel messageViewModel))
        {
          this.Cleanup();
          return;
        }
        if (this.pttMicIcon == null)
        {
          Image image = new Image();
          image.IsHitTestVisible = false;
          image.VerticalAlignment = VerticalAlignment.Bottom;
          image.Width = 18.0 * this.zoomMultiplier;
          image.Margin = new Thickness(-9.0 * this.zoomMultiplier, 0.0, -9.0 * this.zoomMultiplier, 0.0);
          this.pttMicIcon = image;
          this.Children.Add((UIElement) this.pttMicIcon);
        }
        this.pttMicIcon.Source = messageViewModel.PttMicIconSource;
        this.thumbnailImage.Width = this.thumbBackground.Width = vm.ThumbnailWidth;
        this.thumbnailImage.Height = this.thumbBackground.Height = vm.ThumbnailHeight;
        this.thumbnailImage.Source = messageViewModel.PttProfilePicSource;
        this.Background = (Brush) UIUtils.TransparentBrush;
      }
      else
      {
        double thumbnailWidth = vm.ThumbnailWidth;
        double thumbnailHeight = vm.ThumbnailHeight;
        this.thumbBackground.Width = thumbnailWidth;
        this.thumbBackground.Height = thumbnailHeight;
        this.thumbBackground.Fill = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
        this.thumbnailImage.Width = thumbnailWidth * 0.8;
        this.thumbnailImage.Height = thumbnailHeight * 0.8;
        this.thumbnailImage.Stretch = Stretch.UniformToFill;
        this.thumbnailImage.Source = (System.Windows.Media.ImageSource) AssetStore.AudioButtonIcon;
        this.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
      }
      this.playbackBar.Margin = amvm.PlaybackBarMargin;
      this.durationBlock.Margin = amvm.PttDurationMargin;
      this.durationBlock.Text = amvm.PttDurationStr;
      bool shouldShowPlaybackBar = amvm.ShouldShowPlaybackBar;
      this.ShowElement((FrameworkElement) this.durationBlock, shouldShowPlaybackBar);
      this.ShowElement((FrameworkElement) this.playbackBar, shouldShowPlaybackBar);
      this.UpdateTransferIndicator(amvm);
      this.UpdateInfoBlock(amvm);
      this.UpdateActionButton(amvm);
      this.ShowElement((FrameworkElement) this.pttMicIcon, show);
      if (amvm.ShouldShowThumbnailOnRight)
      {
        this.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Star);
        this.ColumnDefinitions[2].Width = GridLength.Auto;
        Grid.SetColumn((FrameworkElement) this.actionButton, 0);
        Grid.SetColumn((FrameworkElement) this.thumbnailImage, 2);
        Grid.SetColumn((FrameworkElement) this.thumbBackground, 2);
        Grid.SetColumn((FrameworkElement) this.infoBlock, 1);
        Grid.SetColumn((FrameworkElement) this.playbackBar, 1);
        Grid.SetColumn((FrameworkElement) this.durationBlock, 1);
        if (!show)
          return;
        Grid.SetColumn((FrameworkElement) this.pttMicIcon, 2);
        this.pttMicIcon.HorizontalAlignment = HorizontalAlignment.Left;
      }
      else
      {
        this.ColumnDefinitions[1].Width = GridLength.Auto;
        this.ColumnDefinitions[2].Width = new GridLength(1.0, GridUnitType.Star);
        Grid.SetColumn((FrameworkElement) this.actionButton, 1);
        Grid.SetColumn((FrameworkElement) this.thumbnailImage, 0);
        Grid.SetColumn((FrameworkElement) this.thumbBackground, 0);
        Grid.SetColumn((FrameworkElement) this.infoBlock, 2);
        Grid.SetColumn((FrameworkElement) this.playbackBar, 2);
        Grid.SetColumn((FrameworkElement) this.durationBlock, 2);
        if (!show)
          return;
        Grid.SetColumn((FrameworkElement) this.pttMicIcon, 0);
        this.pttMicIcon.HorizontalAlignment = HorizontalAlignment.Right;
      }
    }

    private void UpdateTransferIndicator(AudioMessageViewModel amvm)
    {
      if (amvm.ShouldShowTransferStatusBar)
      {
        if (this.transferIndicator == null)
        {
          ProgressBar progressBar = new ProgressBar();
          progressBar.IsIndeterminate = true;
          progressBar.Foreground = (Brush) UIUtils.WhiteBrush;
          progressBar.Padding = new Thickness(0.0);
          progressBar.VerticalAlignment = VerticalAlignment.Center;
          progressBar.HorizontalAlignment = HorizontalAlignment.Stretch;
          progressBar.IsHitTestVisible = false;
          this.transferIndicator = progressBar;
          this.Children.Add((UIElement) this.transferIndicator);
        }
        this.transferIndicator.Margin = amvm.TransferIndicatorMargin;
        Grid.SetColumn((FrameworkElement) this.transferIndicator, amvm.ShouldShowThumbnailOnRight ? 1 : 2);
        this.ShowElement((FrameworkElement) this.transferIndicator, true);
      }
      else
        this.ShowElement((FrameworkElement) this.transferIndicator, false);
    }

    private void UpdateInfoBlock(AudioMessageViewModel amvm)
    {
      if (amvm.ShouldShowMediaInfo)
      {
        this.infoBlock.Text = amvm.MediaInfoStr;
        this.ShowElement((FrameworkElement) this.infoBlock, true);
      }
      else
        this.ShowElement((FrameworkElement) this.infoBlock, false);
    }

    private void UpdateActionButton(AudioMessageViewModel amvm)
    {
      if (amvm.ShouldShowActionButton)
      {
        this.actionButton.ButtonIcon = amvm.ActionButtonIcon;
        this.actionButton.ButtonIconReversed = amvm.ActionButtonIconReversed;
        this.actionButton.ButtonActiveBrush = (Brush) amvm.BackgroundBrush;
        this.actionButton.Margin = amvm.ActionButtonMargin;
        this.actionButton.Opacity = 1.0;
      }
      else
        this.actionButton.Opacity = 0.0;
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.thumbnailImage.Source = (System.Windows.Media.ImageSource) null;
      if (this.pttMicIcon == null)
        return;
      this.pttMicIcon.Source = (System.Windows.Media.ImageSource) null;
    }

    public override void ProcessViewModelNotification(KeyValuePair<string, object> args)
    {
      base.ProcessViewModelNotification(args);
      switch (args.Key)
      {
        case "PlaybackProgressChanged":
          this.OnPlaybackProgressChanged();
          break;
        case "PlaybackStateChanged":
          this.OnPlaybackStateChanged();
          break;
        case "StatusChanged":
          this.OnStatusChanged();
          break;
        case "LocalFileUriChanged":
          this.OnLocalFileUriChanged();
          break;
        case "ThumbnailChanged":
          this.OnThumbnailUpdated(args.Value as string);
          break;
        case "TransferStateChanged":
          this.OnTransferStateChanged();
          break;
      }
    }

    private void OnThumbnailUpdated(string context)
    {
      if (!(this.viewModel is PttMessageViewModel viewModel))
        return;
      Log.d("msgbubble", "thumbnail updated | {0} | keyid:{1},type:{2}", (object) (context ?? "n/a"), (object) viewModel.Message.KeyId, (object) viewModel.Message.MediaWaType);
      PttMessageViewModel messageViewModel = viewModel;
      if (messageViewModel == null)
        return;
      this.thumbnailImage.Source = messageViewModel.PttProfilePicSource;
    }

    private void OnLocalFileUriChanged()
    {
    }

    private void OnTransferStateChanged()
    {
      if (!(this.viewModel is AudioMessageViewModel viewModel))
        return;
      this.ShowElements((IEnumerable<FrameworkElement>) new FrameworkElement[2]
      {
        (FrameworkElement) this.playbackBar,
        (FrameworkElement) this.durationBlock
      }, viewModel.ShouldShowPlaybackBar);
      this.UpdateTransferIndicator(viewModel);
      this.UpdateInfoBlock(viewModel);
      this.UpdateActionButton(viewModel);
    }

    private void OnPlaybackProgressChanged()
    {
      if (!(this.viewModel is AudioMessageViewModel viewModel))
        return;
      this.playbackBar.Value = viewModel.PttPlaybackValue;
      this.durationBlock.Text = viewModel.PttDurationStr;
    }

    private void OnPlaybackStateChanged()
    {
      if (!(this.viewModel is AudioMessageViewModel viewModel))
        return;
      this.UpdateActionButton(viewModel);
      this.playbackBar.Value = viewModel.PttPlaybackValue;
      this.durationBlock.Text = viewModel.PttDurationStr;
    }

    private void OnStatusChanged()
    {
      if (this.viewModel.Message.KeyFromMe)
      {
        if (this.viewModel.Message.IsRevoked())
          return;
        if (this.viewModel is AudioMessageViewModel viewModel)
        {
          this.UpdateActionButton(viewModel);
          this.ShowElements((IEnumerable<FrameworkElement>) new FrameworkElement[2]
          {
            (FrameworkElement) this.playbackBar,
            (FrameworkElement) this.durationBlock
          }, viewModel.ShouldShowPlaybackBar);
          this.ShowElement((FrameworkElement) this.transferIndicator, viewModel.ShouldShowTransferStatusBar);
        }
      }
      if (!this.viewModel.Message.IsPtt() || !(this.viewModel is PttMessageViewModel viewModel1))
        return;
      this.UpdateInfoBlock((AudioMessageViewModel) viewModel1);
      this.pttMicIcon.Source = viewModel1.PttMicIconSource;
    }

    private void ActionButton_Click(object sender, EventArgs e)
    {
      if (!(this.viewModel is AudioMessageViewModel viewModel))
        return;
      ViewMessage.View(viewModel.Message);
    }

    private void PlaybackBar_SeekStarted(object sender, EventArgs e)
    {
      PlayAudioMessage.GetInstance(false)?.Player.Stop(true);
    }

    private void PlaybackBar_SeekCompleted(object sender, PlaybackSlider.PlaybackSliderSeekArgs e)
    {
      Message message = this.viewModel.Message;
      if (message == null)
        return;
      double num = 0.0;
      double? seekedValue = e.SeekedValue;
      if (seekedValue.HasValue && message.MediaDurationSeconds > 0)
      {
        seekedValue = e.SeekedValue;
        num = seekedValue.Value * (double) message.MediaDurationSeconds * 1000.0;
      }
      PlayAudioMessage.GetInstance(false)?.Player.Seek(message, new double?(num));
    }
  }
}
