// Decompiled with JetBrains decompiler
// Type: WhatsApp.LocationMessageViewPanel
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


namespace WhatsApp
{
  internal class LocationMessageViewPanel : MessageViewPanel
  {
    protected Image thumbnailImage;
    protected TextBlock nameBlock;
    protected TextTrimmingControl addressBlock;
    protected Rectangle foregroundProtection;
    private IDisposable thumbSub;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Location;

    public LocationMessageViewPanel()
    {
      Image image = new Image();
      image.Stretch = Stretch.UniformToFill;
      image.HorizontalAlignment = HorizontalAlignment.Center;
      this.thumbnailImage = image;
      this.thumbnailImage.Tap += new EventHandler<GestureEventArgs>(this.OnTap);
      this.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      Grid.SetRow((FrameworkElement) this.thumbnailImage, 0);
      this.Children.Add((UIElement) this.thumbnailImage);
    }

    public override void Render(MessageViewModel vm)
    {
      if (!(vm is LocationMessageViewModel vm1))
        return;
      base.Render((MessageViewModel) vm1);
      if (vm is LiveLocationMessageViewModel)
        return;
      this.UpdateThumbnail((MessageViewModel) vm1);
      this.UpdateFooterProtection((MessageViewModel) vm1);
      this.UpdatePlaceDetails(vm1);
    }

    protected override void DisposeSubscriptions()
    {
      base.DisposeSubscriptions();
      this.thumbSub.SafeDispose();
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.thumbnailImage.Source = (System.Windows.Media.ImageSource) null;
    }

    public void UpdateThumbnail(MessageViewModel vm)
    {
      if (vm == null)
        return;
      this.thumbnailImage.Width = vm.ThumbnailWidth;
      this.thumbnailImage.Height = vm.ThumbnailHeight;
      this.thumbSub.SafeDispose();
      this.thumbSub = vm.GetThumbnailObservable().SubscribeOn<MessageViewModel.ThumbnailState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<MessageViewModel.ThumbnailState>().Subscribe<MessageViewModel.ThumbnailState>((Action<MessageViewModel.ThumbnailState>) (thumbState =>
      {
        this.thumbnailImage.Source = thumbState == null || thumbState.Image == null || this.viewModel == null || this.viewModel.Message == null || !(thumbState.KeyId == this.viewModel.Message.KeyId) ? (System.Windows.Media.ImageSource) ImageStore.GetStockIcon("/Images/map_loading.png") : thumbState.Image;
        this.UpdateFooterProtection(vm);
      }));
    }

    private void UpdatePlaceDetails(LocationMessageViewModel vm)
    {
      if (vm == null || vm.Message.IsRevoked())
        return;
      if (vm.Message.IsCoordinateLocation())
      {
        if (this.nameBlock != null)
          this.nameBlock.Visibility = Visibility.Collapsed;
        if (this.addressBlock == null)
          return;
        this.addressBlock.Visibility = Visibility.Collapsed;
      }
      else
      {
        if (this.RowDefinitions.Count == 1)
        {
          this.RowDefinitions.Add(new RowDefinition()
          {
            Height = GridLength.Auto
          });
          this.RowDefinitions.Add(new RowDefinition()
          {
            Height = GridLength.Auto
          });
        }
        if (this.nameBlock == null)
        {
          TextBlock textBlock = new TextBlock();
          textBlock.Foreground = (Brush) UIUtils.WhiteBrush;
          textBlock.TextWrapping = TextWrapping.NoWrap;
          textBlock.TextTrimming = TextTrimming.WordEllipsis;
          textBlock.TextDecorations = TextDecorations.Underline;
          textBlock.FontSize = 26.0 * this.zoomMultiplier;
          textBlock.Margin = new Thickness(6.0 * this.zoomMultiplier, 0.0, 6.0 * this.zoomMultiplier, 0.0);
          textBlock.IsHitTestVisible = false;
          this.nameBlock = textBlock;
          Grid.SetRow((FrameworkElement) this.nameBlock, 1);
          this.Children.Add((UIElement) this.nameBlock);
        }
        this.nameBlock.Visibility = Visibility.Visible;
        this.nameBlock.Text = vm.PlaceTitleStr;
        if (this.addressBlock == null)
        {
          TextTrimmingControl textTrimmingControl = new TextTrimmingControl();
          textTrimmingControl.MaxLines = 3;
          textTrimmingControl.FontSize = 22.0 * this.zoomMultiplier;
          textTrimmingControl.Foreground = (Brush) UIUtils.WhiteBrush;
          textTrimmingControl.VerticalAlignment = VerticalAlignment.Top;
          textTrimmingControl.HorizontalAlignment = HorizontalAlignment.Left;
          textTrimmingControl.Margin = new Thickness(6.0 * this.zoomMultiplier, 0.0, 6.0 * this.zoomMultiplier, 3.0 * this.zoomMultiplier);
          textTrimmingControl.IsHitTestVisible = false;
          textTrimmingControl.ReservedEndSpace = 100.0 * this.zoomMultiplier;
          this.addressBlock = textTrimmingControl;
          Grid.SetRow((FrameworkElement) this.addressBlock, 2);
          this.Children.Add((UIElement) this.addressBlock);
        }
        this.addressBlock.Visibility = Visibility.Visible;
        this.addressBlock.Text = vm.PlaceAddressStr;
      }
    }

    private void UpdateFooterProtection(MessageViewModel vm)
    {
      if (vm.ShouldUseFooterProtection)
      {
        if (this.foregroundProtection == null)
        {
          Rectangle rectangle = new Rectangle();
          rectangle.HorizontalAlignment = HorizontalAlignment.Right;
          rectangle.VerticalAlignment = VerticalAlignment.Bottom;
          rectangle.Stretch = Stretch.Fill;
          rectangle.IsHitTestVisible = false;
          rectangle.Width = 216.0 * this.zoomMultiplier;
          rectangle.Height = 72.0 * this.zoomMultiplier;
          LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
          GradientStopCollection gradientStopCollection = new GradientStopCollection();
          gradientStopCollection.Add(new GradientStop()
          {
            Color = Color.FromArgb((byte) 0, (byte) 240, (byte) 239, (byte) 237),
            Offset = 0.0
          });
          gradientStopCollection.Add(new GradientStop()
          {
            Color = Color.FromArgb((byte) 221, (byte) 240, (byte) 239, (byte) 237),
            Offset = 1.0
          });
          linearGradientBrush.GradientStops = gradientStopCollection;
          linearGradientBrush.StartPoint = new System.Windows.Point(0.9, 0.1);
          linearGradientBrush.EndPoint = new System.Windows.Point(1.0, 1.0);
          rectangle.Fill = (Brush) linearGradientBrush;
          this.foregroundProtection = rectangle;
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
      switch (args.Key)
      {
        case "ThumbnailChanged":
          this.UpdateThumbnail(this.viewModel);
          break;
        case "PlaceDetailsChanged":
          this.OnPlaceDetailsChanged();
          break;
      }
    }

    private void OnPlaceDetailsChanged()
    {
      this.UpdatePlaceDetails(this.viewModel as LocationMessageViewModel);
    }

    public virtual void OnTap(object sender, EventArgs e)
    {
      ViewMessage.View(this.viewModel.Message);
    }
  }
}
