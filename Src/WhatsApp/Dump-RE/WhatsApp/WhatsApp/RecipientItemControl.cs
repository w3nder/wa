// Decompiled with JetBrains decompiler
// Type: WhatsApp.RecipientItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class RecipientItemControl : ItemControlBase
  {
    protected TextBlock senderBlock;
    protected Image statusIcon;
    protected Image mediaIcon;
    protected Image muteIcon;

    protected override void InitComponents()
    {
      base.InitComponents();
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
      Grid.SetColumn((FrameworkElement) this.subtitleBlock, 2);
    }

    protected override void UpdateComponents(JidItemViewModel vm)
    {
      base.UpdateComponents(vm);
      if (!(vm is RecipientItemViewModel vm1))
        return;
      this.Opacity = vm1.IsDimmed ? 0.5 : 1.0;
      this.UpdateSubtitleRow(vm1);
    }

    private void UpdateSubtitleRow(RecipientItemViewModel vm)
    {
      if (vm == null)
        return;
      if (vm.ShowSubtitleRow)
      {
        this.subtitleRow.Visibility = Visibility.Visible;
        Brush subtitleBrush = vm.SubtitleBrush;
        FontWeight subtitleWeight = vm.SubtitleWeight;
        this.subtitleBlock.Foreground = subtitleBrush;
        this.subtitleBlock.FontWeight = subtitleWeight;
        if (vm.ShowSender)
        {
          if (this.senderBlock == null)
          {
            TextBlock textBlock = new TextBlock();
            textBlock.VerticalAlignment = VerticalAlignment.Top;
            textBlock.Margin = new Thickness(0.0);
            textBlock.TextWrapping = TextWrapping.NoWrap;
            textBlock.FontSize = 20.0;
            textBlock.Visibility = Visibility.Collapsed;
            this.senderBlock = textBlock;
            this.subtitleRow.Children.Add((UIElement) this.senderBlock);
            Grid.SetColumn((FrameworkElement) this.senderBlock, 0);
          }
          this.senderBlock.Foreground = subtitleBrush;
          this.senderBlock.Text = vm.SenderStr;
          this.senderBlock.Visibility = Visibility.Visible;
        }
        else if (this.senderBlock != null)
          this.senderBlock.Visibility = Visibility.Collapsed;
        if (vm.ShowStatusIcon)
        {
          if (this.statusIcon == null)
          {
            Image image = new Image();
            image.Margin = new Thickness(0.0, -2.0, 6.0, 0.0);
            image.Width = 25.0;
            image.Height = 18.0;
            image.Stretch = Stretch.UniformToFill;
            image.Visibility = Visibility.Collapsed;
            this.statusIcon = image;
            this.subtitleRow.Children.Add((UIElement) this.statusIcon);
            Grid.SetColumn((FrameworkElement) this.statusIcon, 0);
          }
          this.statusIcon.Source = vm.StatusIconSource;
          this.statusIcon.Visibility = Visibility.Visible;
        }
        else if (this.statusIcon != null)
        {
          this.statusIcon.Source = (System.Windows.Media.ImageSource) null;
          this.statusIcon.Visibility = Visibility.Collapsed;
        }
        if (vm.ShowMediaIcon)
        {
          if (this.mediaIcon == null)
          {
            Image image = new Image();
            image.Margin = new Thickness(0.0, 2.0, 6.0, 0.0);
            image.Height = 24.0;
            image.Stretch = Stretch.UniformToFill;
            image.Visibility = Visibility.Collapsed;
            this.mediaIcon = image;
            this.subtitleRow.Children.Add((UIElement) this.mediaIcon);
            Grid.SetColumn((FrameworkElement) this.mediaIcon, 1);
          }
          this.mediaIcon.Source = vm.MediaIconSource;
          this.mediaIcon.Visibility = Visibility.Visible;
        }
        else if (this.mediaIcon != null)
          this.mediaIcon.Visibility = Visibility.Collapsed;
        if (vm.ShowMuteIcon)
        {
          if (this.muteIcon == null)
          {
            Image image = new Image();
            image.Margin = new Thickness(12.0, 2.0, 0.0, 0.0);
            image.Height = 24.0;
            image.Stretch = Stretch.UniformToFill;
            image.Visibility = Visibility.Collapsed;
            this.muteIcon = image;
            this.subtitleRow.Children.Add((UIElement) this.muteIcon);
            Grid.SetColumn((FrameworkElement) this.muteIcon, 3);
          }
          this.muteIcon.Visibility = Visibility.Visible;
          this.muteIcon.Source = vm.MuteIconSource;
        }
        else if (this.muteIcon != null)
          this.muteIcon.Visibility = Visibility.Collapsed;
        this.subtitleBlock.Text = vm.GetSubtitle();
      }
      else
        this.subtitleRow.Visibility = Visibility.Collapsed;
    }
  }
}
