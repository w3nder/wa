// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class ChatItemControl : ItemControlBase
  {
    protected TextBlock timestampBlock;
    protected TextBlock unreadCountBlock;
    protected TextBlock senderBlock;
    protected Image statusIcon;
    protected Image mediaIcon;
    protected Image muteIcon;
    protected Image pinIcon;
    protected Border labelBox;
    protected TextBlock labelBlock;
    protected Rectangle outerHintPanel;
    private string presenceStr;
    private IDisposable subtitleRowFadeOutSbDisposable;
    private IDisposable subtitleRowFadeInSbDisposable;
    private Storyboard subtitleRowFadeOutSb;
    private Storyboard subtitleRowFadeInSb;

    private Storyboard SubtitleRowFadeOutSb
    {
      get
      {
        return this.subtitleRowFadeOutSb ?? (this.subtitleRowFadeOutSb = WaAnimations.CreateStoryboard(WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(200.0), (DependencyObject) this.subtitleRow)));
      }
    }

    private Storyboard SubtitleRowFadeInSb
    {
      get
      {
        return this.subtitleRowFadeInSb ?? (this.subtitleRowFadeInSb = WaAnimations.CreateStoryboard(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(200.0), (DependencyObject) this.subtitleRow)));
      }
    }

    protected override void InitComponents()
    {
      base.InitComponents();
      this.titleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      TextBlock textBlock1 = new TextBlock();
      textBlock1.HorizontalAlignment = HorizontalAlignment.Right;
      textBlock1.VerticalAlignment = VerticalAlignment.Bottom;
      textBlock1.FontSize = UIUtils.FontSizeSmall;
      textBlock1.Margin = new Thickness(18.0, 0.0, 0.0, 4.0);
      textBlock1.Style = UIUtils.TextStyleNormal;
      this.timestampBlock = textBlock1;
      this.titleRow.Children.Add((UIElement) this.timestampBlock);
      Grid.SetColumn((FrameworkElement) this.timestampBlock, 1);
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(0.0)
      });
      TextBlock textBlock2 = new TextBlock();
      textBlock2.VerticalAlignment = VerticalAlignment.Top;
      textBlock2.Margin = new Thickness(0.0);
      textBlock2.TextWrapping = TextWrapping.NoWrap;
      textBlock2.FontSize = 20.0;
      textBlock2.Visibility = Visibility.Collapsed;
      this.senderBlock = textBlock2;
      this.subtitleRow.Children.Add((UIElement) this.senderBlock);
      Grid.SetColumn((FrameworkElement) this.senderBlock, 0);
      Image image1 = new Image();
      image1.Margin = new Thickness(0.0, -2.0, 6.0, 0.0);
      image1.Width = 25.0;
      image1.Height = 18.0;
      image1.Stretch = Stretch.UniformToFill;
      image1.Visibility = Visibility.Collapsed;
      this.statusIcon = image1;
      this.subtitleRow.Children.Add((UIElement) this.statusIcon);
      Grid.SetColumn((FrameworkElement) this.statusIcon, 0);
      Image image2 = new Image();
      image2.Margin = new Thickness(0.0, 2.0, 6.0, 0.0);
      image2.Height = 24.0;
      image2.Stretch = Stretch.UniformToFill;
      image2.Visibility = Visibility.Collapsed;
      this.mediaIcon = image2;
      this.subtitleRow.Children.Add((UIElement) this.mediaIcon);
      Grid.SetColumn((FrameworkElement) this.mediaIcon, 1);
      Grid.SetColumn((FrameworkElement) this.subtitleBlock, 2);
      TextBlock textBlock3 = new TextBlock();
      textBlock3.HorizontalAlignment = HorizontalAlignment.Right;
      textBlock3.VerticalAlignment = VerticalAlignment.Top;
      textBlock3.Margin = new Thickness(18.0, 0.0, 0.0, 0.0);
      textBlock3.FontSize = 20.0;
      textBlock3.Visibility = Visibility.Collapsed;
      textBlock3.Foreground = (Brush) UIUtils.AccentBrush;
      this.unreadCountBlock = textBlock3;
      this.unreadCountBlock.SetValue(TextOptions.DisplayColorEmojiProperty, (object) false);
      this.subtitleRow.Children.Add((UIElement) this.unreadCountBlock);
      Grid.SetColumn((FrameworkElement) this.unreadCountBlock, 5);
    }

    protected override void UpdateComponents(JidItemViewModel vm)
    {
      base.UpdateComponents(vm);
      if (!(vm is ChatItemViewModel chatItemViewModel))
        return;
      this.Opacity = chatItemViewModel.ShouldDisable() ? 0.5 : 1.0;
      if (chatItemViewModel.ShowTimestamp)
      {
        this.timestampBlock.Text = chatItemViewModel.TimestampStr;
        this.timestampBlock.Visibility = Visibility.Visible;
      }
      else
        this.timestampBlock.Visibility = Visibility.Collapsed;
      if (this.presenceStr == null)
      {
        this.UpdateSubtitleRowWithoutAnimations();
      }
      else
      {
        this.presenceStr = (string) null;
        this.UpdateSubtitleRowWithFadeAnimations();
      }
    }

    private void TryCreateMuteIcon()
    {
      if (this.muteIcon != null)
        return;
      Image image = new Image();
      image.Margin = new Thickness(12.0, 2.0, 0.0, 0.0);
      image.Height = 24.0;
      image.Stretch = Stretch.UniformToFill;
      image.Visibility = Visibility.Collapsed;
      this.muteIcon = image;
      this.subtitleRow.Children.Add((UIElement) this.muteIcon);
      Grid.SetColumn((FrameworkElement) this.muteIcon, 3);
    }

    private void TryCreatePinIcon()
    {
      if (this.pinIcon != null)
        return;
      Image image = new Image();
      image.Margin = new Thickness(12.0, 2.0, 0.0, 0.0);
      image.Height = 24.0;
      image.Stretch = Stretch.UniformToFill;
      image.Visibility = Visibility.Collapsed;
      this.pinIcon = image;
      this.subtitleRow.Children.Add((UIElement) this.pinIcon);
      Grid.SetColumn((FrameworkElement) this.pinIcon, 4);
    }

    private void TryCreateLabelBox()
    {
      if (this.labelBox != null)
        return;
      Border border = new Border();
      border.Margin = new Thickness(12.0, 0.0, 0.0, 0.0);
      border.HorizontalAlignment = HorizontalAlignment.Right;
      border.VerticalAlignment = VerticalAlignment.Bottom;
      border.BorderThickness = new Thickness(2.0);
      border.Padding = new Thickness(0.0);
      border.BorderBrush = UIUtils.SubtleBrush;
      border.Visibility = Visibility.Collapsed;
      this.labelBox = border;
      this.subtitleRow.Children.Add((UIElement) this.labelBox);
      Grid.SetColumn((FrameworkElement) this.labelBox, 6);
      TextBlock textBlock = new TextBlock();
      textBlock.Margin = new Thickness(3.0, 0.0, 3.0, 2.0);
      textBlock.VerticalAlignment = VerticalAlignment.Bottom;
      textBlock.FontWeight = FontWeights.SemiBold;
      textBlock.FontSize = 14.0;
      this.labelBlock = textBlock;
      this.labelBox.Child = (UIElement) this.labelBlock;
    }

    private void UpdateSubtitleRowWithoutAnimations()
    {
      this.StopSubtitleRowAnimations();
      this.subtitleRow.Opacity = 1.0;
      this.UpdateSubtitleRowImpl(this.ViewModel as ChatItemViewModel);
    }

    private void UpdateSubtitleRowWithFadeAnimations()
    {
      this.StopSubtitleRowAnimations();
      this.subtitleRow.Opacity = 1.0;
      this.subtitleRowFadeOutSbDisposable = Storyboarder.PerformWithDisposable(this.SubtitleRowFadeOutSb, (DependencyObject) null, true, (Action) (() =>
      {
        this.subtitleRow.Opacity = 0.0;
        this.subtitleRowFadeOutSbDisposable = (IDisposable) null;
        this.UpdateSubtitleRowImpl(this.ViewModel as ChatItemViewModel);
        this.subtitleRowFadeInSbDisposable = Storyboarder.PerformWithDisposable(this.SubtitleRowFadeInSb, onComplete: (Action) (() =>
        {
          this.subtitleRow.Opacity = 1.0;
          this.subtitleRowFadeInSbDisposable = (IDisposable) null;
        }), callOnCompleteOnDisposing: true);
      }), false, (string) null);
    }

    private void UpdateSubtitleRowImpl(ChatItemViewModel vm)
    {
      if (vm == null)
        return;
      Brush subtitleBrush = vm.SubtitleBrush;
      FontWeight subtitleWeight = vm.SubtitleWeight;
      this.subtitleBlock.Foreground = subtitleBrush;
      this.subtitleBlock.FontWeight = subtitleWeight;
      if (vm.ShowSender)
      {
        this.senderBlock.Foreground = subtitleBrush;
        this.senderBlock.Text = vm.SenderStr;
        this.senderBlock.Visibility = Visibility.Visible;
        Grid.SetColumn((FrameworkElement) this.senderBlock, 0);
      }
      else
      {
        this.senderBlock.Visibility = Visibility.Collapsed;
        Grid.SetColumn((FrameworkElement) this.senderBlock, 6);
      }
      if (vm.ShowStatusIcon)
      {
        this.statusIcon.Source = vm.StatusIconSource;
        this.statusIcon.Visibility = Visibility.Visible;
      }
      else
      {
        this.statusIcon.Source = (System.Windows.Media.ImageSource) null;
        this.statusIcon.Visibility = Visibility.Collapsed;
      }
      if (vm.ShowMediaIcon)
      {
        this.mediaIcon.Visibility = Visibility.Visible;
        this.mediaIcon.Source = vm.MediaIconSource;
      }
      else
        this.mediaIcon.Visibility = Visibility.Collapsed;
      if (vm.ShowPinIcon)
      {
        this.TryCreatePinIcon();
        this.pinIcon.Visibility = Visibility.Visible;
        this.pinIcon.Source = vm.PinIconSource;
      }
      else if (this.pinIcon != null)
        this.pinIcon.Visibility = Visibility.Collapsed;
      if (vm.ShowMuteIcon)
      {
        this.TryCreateMuteIcon();
        this.muteIcon.Visibility = Visibility.Visible;
        this.muteIcon.Source = vm.MuteIconSource;
      }
      else if (this.muteIcon != null)
        this.muteIcon.Visibility = Visibility.Collapsed;
      if (vm.ShowUnreadCount)
      {
        this.unreadCountBlock.Foreground = subtitleBrush;
        this.unreadCountBlock.Text = vm.UnreadCountStr;
        this.unreadCountBlock.Visibility = Visibility.Visible;
      }
      else
        this.unreadCountBlock.Visibility = Visibility.Collapsed;
      if (vm.ShowLabel)
      {
        this.TryCreateLabelBox();
        this.labelBlock.Text = vm.LabelStr;
        this.labelBox.Visibility = Visibility.Visible;
      }
      else if (this.labelBox != null)
        this.labelBox.Visibility = Visibility.Collapsed;
      RichTextBlock.TextSet subtitle = vm.GetSubtitle();
      if (subtitle != null)
      {
        if (vm.ShouldEnableRichTextSubtitle())
        {
          this.subtitleBlock.EnableScan = true;
        }
        else
        {
          subtitle.SerializedFormatting = (IEnumerable<LinkDetector.Result>) null;
          subtitle.PartialFormattings = (IEnumerable<WaRichText.Chunk>) null;
          this.subtitleBlock.EnableScan = false;
        }
      }
      this.subtitleBlock.Text = subtitle;
    }

    private void StopSubtitleRowAnimations()
    {
      this.subtitleRowFadeInSbDisposable.SafeDispose();
      this.subtitleRowFadeInSbDisposable = (IDisposable) null;
      this.subtitleRowFadeOutSbDisposable.SafeDispose();
      this.subtitleRowFadeOutSbDisposable = (IDisposable) null;
    }

    protected override void OnVmNotified(string k, object v)
    {
      base.OnVmNotified(k, v);
      switch (k)
      {
        case "Presence":
          string str = v as string;
          if (this.presenceStr == str)
            break;
          this.presenceStr = str;
          this.UpdateSubtitleRowWithFadeAnimations();
          break;
        case "StatusIcon":
          if (this.ViewModel is ChatItemViewModel viewModel && viewModel.ShowStatusIcon)
          {
            this.statusIcon.Source = viewModel.StatusIconSource;
            this.statusIcon.Visibility = Visibility.Visible;
            break;
          }
          this.statusIcon.Visibility = Visibility.Collapsed;
          this.statusIcon.Source = (System.Windows.Media.ImageSource) null;
          break;
        case "MediaWaType":
          this.UpdateSubtitleRowImpl(this.ViewModel as ChatItemViewModel);
          break;
      }
    }
  }
}
