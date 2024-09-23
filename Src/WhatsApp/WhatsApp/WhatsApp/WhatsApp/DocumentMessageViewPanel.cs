// Decompiled with JetBrains decompiler
// Type: WhatsApp.DocumentMessageViewPanel
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WhatsApp.CommonOps;


namespace WhatsApp
{
  public class DocumentMessageViewPanel : MessageViewPanel
  {
    private Image thumbnailImage;
    private Image typeIcon;
    private TextBlock titleBlock;
    private Rectangle backgroundRect;
    private ProgressBar transferIndicator;
    private ProgressBar transferProgressBar;
    private RoundButton actionButton;
    private Grid secondRow;
    private Rectangle gradientBlock;
    private IDisposable thumbSub;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Document;

    public DocumentMessageViewPanel()
    {
      this.Width = MessageViewModel.DefaultContentWidth;
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(56.0 * this.zoomMultiplier)
      });
      this.backgroundRect = new Rectangle();
      Grid.SetRow((FrameworkElement) this.backgroundRect, 0);
      Grid.SetRowSpan((FrameworkElement) this.backgroundRect, 2);
      this.Children.Add((UIElement) this.backgroundRect);
      Image image1 = new Image();
      image1.IsHitTestVisible = false;
      image1.Stretch = Stretch.UniformToFill;
      this.thumbnailImage = image1;
      Grid.SetRow((FrameworkElement) this.thumbnailImage, 0);
      this.Children.Add((UIElement) this.thumbnailImage);
      Grid grid = new Grid();
      grid.IsHitTestVisible = false;
      this.secondRow = grid;
      this.secondRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = GridLength.Auto
      });
      this.secondRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.secondRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = GridLength.Auto
      });
      Grid.SetRow((FrameworkElement) this.secondRow, 1);
      this.Children.Add((UIElement) this.secondRow);
      Image image2 = new Image();
      image2.IsHitTestVisible = false;
      image2.Stretch = Stretch.UniformToFill;
      image2.Width = 27.0 * this.zoomMultiplier;
      image2.Height = 32.0 * this.zoomMultiplier;
      image2.VerticalAlignment = VerticalAlignment.Center;
      image2.Margin = new Thickness(12.0 * this.zoomMultiplier, 0.0, 0.0, 0.0);
      this.typeIcon = image2;
      Grid.SetColumn((FrameworkElement) this.typeIcon, 0);
      this.secondRow.Children.Add((UIElement) this.typeIcon);
      TextBlock textBlock = new TextBlock();
      textBlock.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock.IsHitTestVisible = false;
      textBlock.Margin = new Thickness(12.0 * this.zoomMultiplier, 0.0, 12.0 * this.zoomMultiplier, 0.0);
      textBlock.TextWrapping = TextWrapping.NoWrap;
      textBlock.FontSize = 24.0 * this.zoomMultiplier;
      textBlock.VerticalAlignment = VerticalAlignment.Center;
      this.titleBlock = textBlock;
      Grid.SetColumn((FrameworkElement) this.titleBlock, 1);
      this.secondRow.Children.Add((UIElement) this.titleBlock);
      Rectangle rectangle = new Rectangle();
      rectangle.Stretch = Stretch.Fill;
      rectangle.HorizontalAlignment = HorizontalAlignment.Right;
      rectangle.VerticalAlignment = VerticalAlignment.Stretch;
      rectangle.Width = 64.0 * this.zoomMultiplier;
      this.gradientBlock = rectangle;
      Grid.SetColumn((FrameworkElement) this.gradientBlock, 1);
      this.secondRow.Children.Add((UIElement) this.gradientBlock);
      this.Tap += new EventHandler<GestureEventArgs>(this.OnTap);
    }

    public override void Render(MessageViewModel vm)
    {
      if (!(vm is DocumentMessageViewModel vm1))
        return;
      base.Render(vm);
      SolidColorBrush maskedBackgroundBrush = vm.MaskedBackgroundBrush;
      this.backgroundRect.Fill = (Brush) maskedBackgroundBrush;
      this.gradientBlock.Fill = (Brush) UIUtils.CreateFadingGradientBrush(maskedBackgroundBrush.Color, new System.Windows.Point(1.0, 0.0), new System.Windows.Point(0.0, 0.0));
      this.UpdateThumbnail(vm);
      this.UpdateTransferProgressBar((FileMessageViewModelBase) vm1);
      this.UpdateTransferIndicator((FileMessageViewModelBase) vm1);
      this.UpdateActionButton((FileMessageViewModelBase) vm1);
      this.titleBlock.Text = vm1.TextStr;
      this.typeIcon.Source = vm1.GetDocumentTypeIcon();
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.thumbnailImage.Source = (System.Windows.Media.ImageSource) null;
    }

    protected override void DisposeSubscriptions()
    {
      base.DisposeSubscriptions();
      this.thumbSub.SafeDispose();
      this.thumbSub = (IDisposable) null;
    }

    public override void ProcessViewModelNotification(KeyValuePair<string, object> args)
    {
      base.ProcessViewModelNotification(args);
      switch (args.Key)
      {
        case "StatusChanged":
          this.OnStatusChanged();
          break;
        case "ThumbnailChanged":
          this.UpdateThumbnail(this.viewModel);
          break;
        case "TransferProgressChanged":
          this.OnTransferProgressChanged();
          break;
        case "TransferStateChanged":
          this.OnTransferStateChanged();
          break;
        case "LocalFileUriChanged":
          this.OnLocalFileUriChanged();
          break;
      }
    }

    private void UpdateActionButton(FileMessageViewModelBase vm)
    {
      if (vm.ShouldShowActionButton)
      {
        RoundButton roundButton = new RoundButton();
        roundButton.ButtonBrush = UIUtils.WhiteBrush;
        roundButton.HorizontalAlignment = HorizontalAlignment.Center;
        roundButton.VerticalAlignment = VerticalAlignment.Center;
        roundButton.ButtonSize = 38.0 * this.zoomMultiplier;
        roundButton.ButtonIcon = (BitmapSource) ImageStore.DownloadIcon;
        roundButton.ButtonIconReversed = (BitmapSource) ImageStore.DownloadIconActive;
        roundButton.Margin = new Thickness(0.0, 0.0, 12.0 * this.zoomMultiplier, 0.0);
        this.actionButton = roundButton;
        this.actionButton.Click += new EventHandler(this.ActionButton_Click);
        Grid.SetColumn((FrameworkElement) this.actionButton, 2);
        this.secondRow.Children.Add((UIElement) this.actionButton);
        this.ShowElement((FrameworkElement) this.actionButton, true);
      }
      else
        this.ShowElement((FrameworkElement) this.actionButton, false);
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
          Grid.SetRow((FrameworkElement) this.transferIndicator, 1);
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
      if (vm.ShouldShowTransferProgressBar)
      {
        if (this.transferProgressBar == null)
        {
          ProgressBar progressBar = new ProgressBar();
          progressBar.Foreground = (Brush) UIUtils.WhiteBrush;
          progressBar.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
          progressBar.Padding = new Thickness(0.0);
          progressBar.VerticalAlignment = VerticalAlignment.Bottom;
          progressBar.HorizontalAlignment = HorizontalAlignment.Stretch;
          progressBar.Maximum = 1.0;
          progressBar.IsHitTestVisible = false;
          this.transferProgressBar = progressBar;
          Grid.SetRow((FrameworkElement) this.transferProgressBar, 1);
          this.Children.Add((UIElement) this.transferProgressBar);
        }
        this.transferProgressBar.Value = vm.TransferProgressBarValue;
        this.ShowElement((FrameworkElement) this.transferProgressBar, true);
      }
      else
        this.ShowElement((FrameworkElement) this.transferProgressBar, false);
    }

    private void UpdateThumbnail(MessageViewModel vm)
    {
      if (vm == null)
        return;
      if (vm.Message.BinaryData == null && vm.Message.DataFileName == null)
      {
        this.ShowElement((FrameworkElement) this.thumbnailImage, false);
      }
      else
      {
        this.ShowElement((FrameworkElement) this.thumbnailImage, true);
        this.thumbnailImage.Width = vm.ThumbnailWidth;
        this.thumbnailImage.Height = vm.ThumbnailHeight;
        this.thumbSub.SafeDispose();
        this.thumbSub = vm.GetThumbnailObservable().SubscribeOn<MessageViewModel.ThumbnailState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<MessageViewModel.ThumbnailState>().Subscribe<MessageViewModel.ThumbnailState>(new Action<MessageViewModel.ThumbnailState>(this.OnThumbnailState));
      }
    }

    private void OnTap(object sender, EventArgs e) => ViewMessage.View(this.viewModel.Message);

    private void OnThumbnailState(MessageViewModel.ThumbnailState thumbState)
    {
      if (thumbState.Image != null && this.viewModel != null && thumbState.KeyId == this.viewModel.Message.KeyId)
      {
        this.thumbnailImage.Stretch = Stretch.UniformToFill;
        this.thumbnailImage.Source = thumbState.Image;
        this.ShowElement((FrameworkElement) this.thumbnailImage, true);
      }
      else
      {
        this.thumbnailImage.Source = (System.Windows.Media.ImageSource) null;
        this.ShowElement((FrameworkElement) this.thumbnailImage, false);
      }
    }

    private void OnLocalFileUriChanged()
    {
      if (!(this.viewModel is FileMessageViewModelBase viewModel))
        return;
      this.UpdateTransferIndicator(viewModel);
      this.UpdateTransferProgressBar(viewModel);
      this.UpdateActionButton(viewModel);
    }

    private void OnStatusChanged()
    {
      if (!this.viewModel.Message.KeyFromMe || this.viewModel.Message.IsRevoked() || !(this.viewModel is FileMessageViewModelBase viewModel))
        return;
      this.UpdateTransferProgressBar(viewModel);
      this.UpdateTransferIndicator(viewModel);
    }

    private void OnTransferProgressChanged()
    {
      this.UpdateTransferProgressBar(this.viewModel as FileMessageViewModelBase);
    }

    private void OnTransferStateChanged()
    {
      if (!(this.viewModel is FileMessageViewModelBase viewModel))
        return;
      this.UpdateTransferIndicator(viewModel);
      this.UpdateTransferProgressBar(viewModel);
      this.UpdateActionButton(viewModel);
    }

    private void ActionButton_Click(object sender, EventArgs e)
    {
      if (this.viewModel == null)
        return;
      ViewMessage.View(this.viewModel.Message);
    }
  }
}
