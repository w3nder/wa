// Decompiled with JetBrains decompiler
// Type: WhatsApp.LocationParticipantControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class LocationParticipantControl : UserItemControl
  {
    protected RichTextBlock richSubtleRightBlock;
    protected TextBlock rightButtonBlock;

    protected override void InitComponents()
    {
      base.InitComponents();
      this.nameBlock.FontSize = UIUtils.FontSizeMediumLarge;
      this.nameBlock.VerticalAlignment = VerticalAlignment.Top;
      this.subtitleBlock.FontSize = UIUtils.FontSizeSmall;
    }

    protected override void UpdateRightText(UserViewModel vm)
    {
      int num1 = vm.ShowSubtleRightText ? 1 : 0;
      if (num1 != 0 && this.richSubtleRightBlock == null)
      {
        this.titleRow.ColumnDefinitions.Clear();
        this.titleRow.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Auto)
        });
        this.titleRow.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Star)
        });
        RichTextBlock richTextBlock = new RichTextBlock();
        richTextBlock.Margin = new Thickness(12.0, 0.0, 0.0, 0.0);
        richTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
        richTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
        richTextBlock.FontSize = UIUtils.FontSizeSmall;
        richTextBlock.Foreground = UIUtils.SubtleBrush;
        richTextBlock.TextWrapping = TextWrapping.NoWrap;
        richTextBlock.Visibility = Visibility.Collapsed;
        this.richSubtleRightBlock = richTextBlock;
        this.titleRow.Children.Add((UIElement) this.richSubtleRightBlock);
        Grid.SetColumn((FrameworkElement) this.richSubtleRightBlock, 1);
      }
      if (num1 != 0)
      {
        this.richSubtleRightBlock.Text = new RichTextBlock.TextSet()
        {
          Text = vm.SubtleRightText
        };
        this.richSubtleRightBlock.Visibility = Visibility.Visible;
      }
      else if (this.richSubtleRightBlock != null)
        this.richSubtleRightBlock.Visibility = Visibility.Collapsed;
      LiveLocationParticipantViewModel participantViewModel = vm as LiveLocationParticipantViewModel;
      int num2 = participantViewModel.ShowRightButton ? 1 : 0;
      if (num2 != 0 && this.rightButtonBlock == null)
      {
        TextBlock textBlock = new TextBlock();
        textBlock.HorizontalAlignment = HorizontalAlignment.Right;
        textBlock.VerticalAlignment = VerticalAlignment.Center;
        textBlock.FontSize = UIUtils.FontSizeSmall;
        textBlock.Foreground = (Brush) UIUtils.RedBrush;
        textBlock.Padding = new Thickness(12.0, 24.0, 0.0, 24.0);
        textBlock.Style = UIUtils.TextStyleNormal;
        textBlock.Tag = (object) participantViewModel.ChatJid;
        textBlock.Visibility = Visibility.Collapsed;
        this.rightButtonBlock = textBlock;
        this.Children.Add((UIElement) this.rightButtonBlock);
        Grid.SetColumn((FrameworkElement) this.rightButtonBlock, 1);
      }
      if (num2 != 0)
      {
        this.rightButtonBlock.Text = participantViewModel.RightButtonText;
        this.rightButtonBlock.Tap += new EventHandler<GestureEventArgs>(this.RightButtonBlock_Tap);
        this.rightButtonBlock.Visibility = Visibility.Visible;
      }
      else
      {
        if (this.rightButtonBlock == null)
          return;
        this.rightButtonBlock.Tap -= new EventHandler<GestureEventArgs>(this.RightButtonBlock_Tap);
        this.rightButtonBlock.Visibility = Visibility.Collapsed;
      }
    }

    private void RightButtonBlock_Tap(object sender, GestureEventArgs e)
    {
      UIUtils.MessageBox(" ", AppResources.LiveLocationStopSharing, (IEnumerable<string>) new string[2]
      {
        AppResources.Cancel,
        AppResources.LiveLocationStop
      }, (Action<int>) (idx =>
      {
        if (idx != 1)
          return;
        LiveLocationManager.Instance.DisableLocationSharing((string) ((FrameworkElement) sender).Tag, wam_enum_live_location_sharing_session_ended_reason.USER_CANCELED);
      }));
    }
  }
}
