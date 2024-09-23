// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageFooterPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace WhatsApp
{
  public class MessageFooterPanel : Grid
  {
    private StackPanel timestampPanel;
    private Grid leftTimestampGrid;
    private Border timestampPanelBorder;
    private TextBlock timestampBlock;
    private TextBlock infoBlock;
    private TextBlock starBlock;
    private Image statusIcon;
    private Image broadcastIcon;
    private IDisposable starBlockFadeSbDisposable;
    private IDisposable statusIconFadeOutSbDisposable;
    private IDisposable statusIconFadeInSbDisposable;
    private Storyboard statusIconFadeOutSb;
    private Storyboard statusIconFadeInSb;

    public MessageFooterPanel()
    {
      this.HorizontalAlignment = HorizontalAlignment.Stretch;
      this.VerticalAlignment = VerticalAlignment.Bottom;
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = GridLength.Auto
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = GridLength.Auto
      });
      this.timestampPanelBorder = new Border();
      Grid.SetColumn((FrameworkElement) this.timestampPanelBorder, 2);
      this.Children.Add((UIElement) this.timestampPanelBorder);
      StackPanel stackPanel = new StackPanel();
      stackPanel.Orientation = Orientation.Horizontal;
      stackPanel.VerticalAlignment = VerticalAlignment.Bottom;
      stackPanel.HorizontalAlignment = HorizontalAlignment.Right;
      this.timestampPanel = stackPanel;
      this.timestampPanelBorder.Child = (UIElement) this.timestampPanel;
      TextBlock textBlock = new TextBlock();
      textBlock.FontSize = MessageViewModel.SmallTextFontSizeStatic;
      textBlock.VerticalAlignment = VerticalAlignment.Bottom;
      textBlock.IsHitTestVisible = false;
      this.timestampBlock = textBlock;
      this.timestampPanel.Children.Add((UIElement) this.timestampBlock);
    }

    public void Render(MessageViewModel vm)
    {
      if (vm == null)
        return;
      this.StopStatusIconAnimations();
      if (vm.ShouldShowFooter)
      {
        this.Visibility = Visibility.Visible;
        if (vm.ShouldShowFooterOnLeft)
        {
          if (Grid.GetColumn((FrameworkElement) this.timestampPanelBorder) == 2)
          {
            if (this.leftTimestampGrid == null)
            {
              Grid grid = new Grid();
              grid.VerticalAlignment = VerticalAlignment.Bottom;
              grid.HorizontalAlignment = HorizontalAlignment.Right;
              this.leftTimestampGrid = grid;
              this.leftTimestampGrid.ColumnDefinitions.Add(new ColumnDefinition()
              {
                Width = new GridLength(1.0, GridUnitType.Star)
              });
              this.leftTimestampGrid.ColumnDefinitions.Add(new ColumnDefinition()
              {
                Width = GridLength.Auto
              });
              Grid.SetColumn((FrameworkElement) this.leftTimestampGrid, 0);
            }
            this.Children.Remove((UIElement) this.timestampPanelBorder);
            this.Children.Add((UIElement) this.leftTimestampGrid);
            this.leftTimestampGrid.Children.Add((UIElement) this.timestampPanelBorder);
            Grid.SetColumn((FrameworkElement) this.timestampPanelBorder, 1);
            this.ColumnDefinitions[0].Width = vm.BubbleContentWidth;
          }
        }
        else if (this.leftTimestampGrid != null && this.Children.Contains((UIElement) this.leftTimestampGrid))
        {
          this.leftTimestampGrid.Children.Remove((UIElement) this.timestampPanelBorder);
          this.Children.Remove((UIElement) this.leftTimestampGrid);
          this.Children.Add((UIElement) this.timestampPanelBorder);
          Grid.SetColumn((FrameworkElement) this.timestampPanelBorder, 2);
          this.ColumnDefinitions[0].Width = GridLength.Auto;
        }
        this.timestampPanelBorder.Background = vm.FooterBackgroundBrush;
        this.timestampPanelBorder.Padding = vm.FooterPanelPadding;
        this.timestampBlock.Text = vm.TimestampStr;
        this.timestampBlock.Foreground = vm.TimestampBrush;
        this.UpdateStatusIcon(vm, false);
        if (vm.ShouldShowBroadcastIcon)
        {
          if (this.broadcastIcon == null)
          {
            double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
            Image image = new Image();
            image.Width = 25.0 * zoomMultiplier;
            image.Height = 18.0 * zoomMultiplier;
            image.Stretch = Stretch.UniformToFill;
            image.Margin = new Thickness(0.0, 0.0, 6.0 * zoomMultiplier, 3.0 * zoomMultiplier);
            image.VerticalAlignment = VerticalAlignment.Bottom;
            image.IsHitTestVisible = false;
            this.broadcastIcon = image;
            this.timestampPanel.Children.Insert(this.timestampPanel.Children.IndexOf((UIElement) this.timestampBlock), (UIElement) this.broadcastIcon);
          }
          this.broadcastIcon.Source = vm.BroadcastIconSource;
          this.broadcastIcon.Visibility = Visibility.Visible;
        }
        else if (this.broadcastIcon != null)
          this.broadcastIcon.Visibility = Visibility.Collapsed;
        this.UpdateStarred(vm);
        this.UpdateFooterInfo(vm);
      }
      else
        this.Visibility = Visibility.Collapsed;
    }

    public void UpdateFooterInfo(MessageViewModel vm)
    {
      if (vm == null)
        return;
      if (vm.ShouldShowFooterInfo)
      {
        if (this.infoBlock == null)
        {
          TextBlock textBlock = new TextBlock();
          textBlock.FontSize = MessageViewModel.SmallTextFontSizeStatic;
          textBlock.TextWrapping = TextWrapping.NoWrap;
          textBlock.TextTrimming = TextTrimming.WordEllipsis;
          textBlock.IsHitTestVisible = false;
          textBlock.HorizontalAlignment = HorizontalAlignment.Left;
          textBlock.VerticalAlignment = VerticalAlignment.Bottom;
          this.infoBlock = textBlock;
          Grid.SetColumn((FrameworkElement) this.infoBlock, 0);
          this.Children.Add((UIElement) this.infoBlock);
        }
        this.infoBlock.Foreground = vm.TimestampBrush;
        this.infoBlock.Text = vm.FooterInfoStr;
      }
      else
      {
        if (this.infoBlock == null)
          return;
        this.infoBlock.Text = (string) null;
      }
    }

    public void UpdateForeground(MessageViewModel vm, bool skipStatusIcon)
    {
      if (vm == null)
        return;
      Brush timestampBrush = vm.TimestampBrush;
      this.timestampBlock.Foreground = timestampBrush;
      if (this.infoBlock != null)
        this.infoBlock.Foreground = timestampBrush;
      if (!skipStatusIcon)
        this.UpdateStatusIcon(vm, false);
      if (this.broadcastIcon != null)
        this.broadcastIcon.Source = vm.BroadcastIconSource;
      if (this.starBlock == null)
        return;
      this.starBlock.Foreground = timestampBrush;
    }

    public void UpdateStarred(MessageViewModel vm)
    {
      this.starBlockFadeSbDisposable.SafeDispose();
      this.starBlockFadeSbDisposable = (IDisposable) null;
      if (vm.Message.IsStarred)
      {
        if (this.starBlock == null)
        {
          double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
          TextBlock textBlock = new TextBlock();
          textBlock.FontSize = 22.0 * zoomMultiplier;
          textBlock.Text = "★";
          textBlock.VerticalAlignment = VerticalAlignment.Bottom;
          textBlock.Margin = new Thickness(0.0, 0.0, 6.0 * zoomMultiplier, -1.0 * zoomMultiplier);
          textBlock.IsHitTestVisible = false;
          this.starBlock = textBlock;
          this.timestampPanel.Children.Insert(0, (UIElement) this.starBlock);
        }
        this.starBlock.Opacity = 0.0;
        this.starBlock.Visibility = Visibility.Visible;
        this.starBlock.Foreground = vm.TimestampBrush;
        this.starBlockFadeSbDisposable = WaAnimations.PerformFade((DependencyObject) this.starBlock, WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(300.0), (Action) (() =>
        {
          this.starBlock.Opacity = 1.0;
          this.starBlockFadeSbDisposable.SafeDispose();
          this.starBlockFadeSbDisposable = (IDisposable) null;
        }));
      }
      else
      {
        if (this.starBlock == null || this.starBlock.Visibility != Visibility.Visible)
          return;
        this.starBlock.Opacity = 1.0;
        this.starBlockFadeSbDisposable = WaAnimations.PerformFade((DependencyObject) this.starBlock, WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(300.0), (Action) (() =>
        {
          this.starBlock.Opacity = 0.0;
          this.starBlock.Visibility = Visibility.Collapsed;
          this.starBlockFadeSbDisposable.SafeDispose();
          this.starBlockFadeSbDisposable = (IDisposable) null;
        }));
      }
    }

    public void UpdateStatusIcon(MessageViewModel vm, bool animate)
    {
      if (vm == null)
        return;
      if (vm.Message.Status.IsPlayedByTarget())
        animate = false;
      if (vm.ShouldShowStatusIcon)
      {
        if (this.statusIcon == null)
        {
          double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
          Image image = new Image();
          image.Width = 25.0 * zoomMultiplier;
          image.Height = 18.0 * zoomMultiplier;
          image.Stretch = Stretch.UniformToFill;
          image.Margin = new Thickness(6.0 * zoomMultiplier, 0.0, 0.0, 3.0 * zoomMultiplier);
          image.VerticalAlignment = VerticalAlignment.Bottom;
          image.IsHitTestVisible = false;
          this.statusIcon = image;
          this.timestampPanel.Children.Add((UIElement) this.statusIcon);
        }
        this.statusIcon.Visibility = Visibility.Visible;
        if (animate)
        {
          if (this.statusIconFadeOutSbDisposable != null)
            return;
          if (this.statusIconFadeInSbDisposable == null)
            this.PerformAnimatedStatusUpdate(vm);
          else
            this.statusIcon.Source = vm.StatusIconSource;
        }
        else
        {
          this.StopStatusIconAnimations();
          this.statusIcon.Opacity = 1.0;
          this.statusIcon.Source = vm.StatusIconSource;
        }
      }
      else
      {
        if (this.statusIcon == null)
          return;
        this.StopStatusIconAnimations();
        this.statusIcon.Source = (System.Windows.Media.ImageSource) null;
        this.statusIcon.Visibility = Visibility.Collapsed;
      }
    }

    private void StopStatusIconAnimations()
    {
      this.statusIconFadeOutSbDisposable.SafeDispose();
      this.statusIconFadeOutSbDisposable = (IDisposable) null;
      this.statusIconFadeInSbDisposable.SafeDispose();
      this.statusIconFadeInSbDisposable = (IDisposable) null;
    }

    private void PerformAnimatedStatusUpdate(MessageViewModel mvm)
    {
      if (this.statusIconFadeOutSb == null)
        this.statusIconFadeOutSb = WaAnimations.CreateStoryboard(WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(300.0), (DependencyObject) this.statusIcon));
      if (this.statusIconFadeInSb == null)
        this.statusIconFadeInSb = WaAnimations.CreateStoryboard(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(300.0), (DependencyObject) this.statusIcon));
      this.statusIcon.Opacity = 1.0;
      this.statusIconFadeOutSbDisposable = Storyboarder.PerformWithDisposable(this.statusIconFadeOutSb, onComplete: (Action) (() =>
      {
        this.statusIconFadeOutSbDisposable = (IDisposable) null;
        this.statusIcon.Source = mvm.StatusIconSource;
        this.statusIcon.Opacity = 0.0;
        this.statusIconFadeInSbDisposable.SafeDispose();
        this.statusIconFadeInSbDisposable = Storyboarder.PerformWithDisposable(this.statusIconFadeInSb, onComplete: (Action) (() =>
        {
          this.statusIcon.Opacity = 1.0;
          this.statusIconFadeInSbDisposable = (IDisposable) null;
        }), callOnCompleteOnDisposing: true);
      }), onDisposing: (Action) (() =>
      {
        this.statusIconFadeOutSbDisposable = (IDisposable) null;
        this.statusIcon.Source = mvm.StatusIconSource;
        this.statusIcon.Opacity = 1.0;
      }));
    }
  }
}
