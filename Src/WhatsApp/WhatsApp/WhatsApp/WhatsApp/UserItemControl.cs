// Decompiled with JetBrains decompiler
// Type: WhatsApp.UserItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class UserItemControl : ItemControlBase
  {
    protected TextBlock accentRightBlock;
    protected TextBlock subtleRightBlock;

    public bool ShowAccentRightText { get; set; } = true;

    protected override void InitComponents()
    {
      base.InitComponents();
      this.detailsPanel.RowDefinitions.Clear();
    }

    protected override void UpdateComponents(JidItemViewModel vm)
    {
      base.UpdateComponents(vm);
      if (!(vm is UserViewModel vm1))
        return;
      this.UpdateSubtitleRow((JidItemViewModel) vm1);
      this.UpdateRightText(vm1);
    }

    protected override void OnVmNotified(string k, object v)
    {
      base.OnVmNotified(k, v);
      if (!(k == "RightText"))
        return;
      this.UpdateRightText(this.ViewModel as UserViewModel);
    }

    protected virtual void UpdateRightText(UserViewModel vm)
    {
      int num = vm.ShowSubtleRightText ? 1 : 0;
      bool flag = vm.ShowAccentRightText && this.ShowAccentRightText;
      if ((num | (flag ? 1 : 0)) != 0 && (this.accentRightBlock == null || this.subtleRightBlock == null))
      {
        this.subtitleRow.ColumnDefinitions.Clear();
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
        Grid.SetColumn((FrameworkElement) this.subtitleBlock, 0);
        TextBlock textBlock1 = new TextBlock();
        textBlock1.Margin = new Thickness(12.0, 0.0, 0.0, 6.0);
        textBlock1.HorizontalAlignment = HorizontalAlignment.Right;
        textBlock1.VerticalAlignment = VerticalAlignment.Bottom;
        textBlock1.Style = UIUtils.TextStyleSubtle;
        textBlock1.Visibility = Visibility.Collapsed;
        this.subtleRightBlock = textBlock1;
        this.subtitleRow.Children.Add((UIElement) this.subtleRightBlock);
        Grid.SetColumn((FrameworkElement) this.subtleRightBlock, 1);
        TextBlock textBlock2 = new TextBlock();
        textBlock2.Margin = new Thickness(12.0, 0.0, 0.0, 6.0);
        textBlock2.HorizontalAlignment = HorizontalAlignment.Right;
        textBlock2.VerticalAlignment = VerticalAlignment.Bottom;
        textBlock2.Style = UIUtils.TextStyleNormal;
        textBlock2.Foreground = (Brush) UIUtils.AccentBrush;
        textBlock2.Visibility = Visibility.Collapsed;
        this.accentRightBlock = textBlock2;
        this.subtitleRow.Children.Add((UIElement) this.accentRightBlock);
        Grid.SetColumn((FrameworkElement) this.accentRightBlock, 2);
      }
      if (num != 0)
      {
        this.subtleRightBlock.Text = vm.SubtleRightText;
        this.subtleRightBlock.Visibility = Visibility.Visible;
      }
      else if (this.subtleRightBlock != null)
        this.subtleRightBlock.Visibility = Visibility.Collapsed;
      if (flag)
      {
        this.accentRightBlock.Text = vm.AccentRightText;
        this.accentRightBlock.Visibility = Visibility.Visible;
      }
      else
      {
        if (this.accentRightBlock == null)
          return;
        this.accentRightBlock.Visibility = Visibility.Collapsed;
      }
    }

    protected override void UpdateSubtitleRow(JidItemViewModel vm)
    {
      if (vm is UserViewModel userViewModel && userViewModel.ShowSubtitle)
      {
        if (this.detailsPanel.RowDefinitions.Count < 2)
        {
          this.detailsPanel.RowDefinitions.Clear();
          this.detailsPanel.RowDefinitions.Add(new RowDefinition()
          {
            Height = new GridLength(1.0, GridUnitType.Star)
          });
          this.detailsPanel.RowDefinitions.Add(new RowDefinition()
          {
            Height = GridLength.Auto
          });
        }
        userViewModel.GetSubtitleObservable().Take<RichTextBlock.TextSet>(1).ObserveOnDispatcher<RichTextBlock.TextSet>().Subscribe<RichTextBlock.TextSet>((Action<RichTextBlock.TextSet>) (s => this.subtitleBlock.Text = s));
        this.subtitleBlock.Foreground = userViewModel.SubtitleBrush;
        this.subtitleBlock.FontWeight = userViewModel.SubtitleWeight;
      }
      else
        this.subtitleRow.Visibility = Visibility.Collapsed;
      base.UpdateSubtitleRow(vm);
    }
  }
}
