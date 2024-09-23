// Decompiled with JetBrains decompiler
// Type: WhatsApp.JidNameControl
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
  public class JidNameControl : ContentControl
  {
    private StackPanel rootPanel;
    private RichTextBlock nameBlock;
    private Image verifiedIcon;
    private IDisposable nameSub;

    public JidNameControl()
    {
      StackPanel stackPanel1 = new StackPanel();
      stackPanel1.Orientation = Orientation.Horizontal;
      stackPanel1.VerticalAlignment = VerticalAlignment.Stretch;
      StackPanel stackPanel2 = stackPanel1;
      this.rootPanel = stackPanel1;
      this.Content = (object) stackPanel2;
    }

    public void Set(RichTextBlock.TextSet t, bool showBadge)
    {
      this.EnsureNameBlock();
      this.nameBlock.MaxWidth = double.PositiveInfinity;
      this.nameSub.SafeDispose();
      this.nameSub = (IDisposable) null;
      this.nameBlock.Text = t;
      this.UpdateVerifiedBadge(showBadge);
    }

    public void Set(JidItemViewModel vm, bool useCache)
    {
      if (vm == null)
        return;
      this.EnsureNameBlock();
      this.nameBlock.MaxWidth = double.PositiveInfinity;
      this.nameSub.SafeDispose();
      this.nameSub = vm.GetRichTitleObservable(!useCache).ObserveOnDispatcherIfNeeded<RichTextBlock.TextSet>().Subscribe<RichTextBlock.TextSet>((Action<RichTextBlock.TextSet>) (t => this.nameBlock.Text = t), (Action) (() => this.nameSub = (IDisposable) null));
      this.UpdateVerifiedBadge(vm.ShowVerifiedIcon);
    }

    private void UpdateVerifiedBadge(bool showBadge)
    {
      if (showBadge)
      {
        this.nameBlock.FontWeight = FontWeights.SemiBold;
        this.EnsureVerifiedBadge();
        this.verifiedIcon.Visibility = Visibility.Visible;
      }
      else
      {
        this.nameBlock.FontWeight = this.FontWeight;
        if (this.verifiedIcon == null)
          return;
        this.verifiedIcon.Visibility = Visibility.Collapsed;
      }
    }

    private void EnsureNameBlock()
    {
      if (this.nameBlock != null)
        return;
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
      richTextBlock.VerticalAlignment = VerticalAlignment.Center;
      richTextBlock.Margin = new Thickness(-12.0, 0.0, 0.0, 0.0);
      richTextBlock.FontWeight = this.FontWeight;
      richTextBlock.FontFamily = UIUtils.FontFamilySemiLight;
      richTextBlock.Foreground = (Brush) UIUtils.ForegroundBrush;
      richTextBlock.FontSize = this.FontSize;
      richTextBlock.TextWrapping = TextWrapping.NoWrap;
      richTextBlock.AllowTextFormatting = false;
      richTextBlock.AllowLinks = false;
      this.nameBlock = richTextBlock;
      this.rootPanel.Children.Add((UIElement) this.nameBlock);
    }

    private void EnsureVerifiedBadge()
    {
      if (this.verifiedIcon != null)
        return;
      double num = this.FontSize * 1.1;
      Image image = new Image();
      image.Source = (System.Windows.Media.ImageSource) AssetStore.InlineVerified;
      image.Margin = new Thickness(-6.0, 0.0, 0.0, 0.0);
      image.HorizontalAlignment = HorizontalAlignment.Left;
      image.VerticalAlignment = VerticalAlignment.Center;
      image.Width = num;
      image.Height = num;
      this.verifiedIcon = image;
      this.rootPanel.Children.Add((UIElement) this.verifiedIcon);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.nameBlock != null && this.verifiedIcon != null && this.verifiedIcon.Visibility == Visibility.Visible)
      {
        double width1 = this.rootPanel.DesiredSize.Width;
        double width2 = availableSize.Width;
        if (width1 > width2)
          this.nameBlock.MaxWidth = width2 - this.FontSize * 1.1;
        else if (width1 < width2 * 0.7)
        {
          this.nameBlock.MaxWidth = width2 - this.FontSize * 1.1;
          this.nameBlock.Refresh();
        }
      }
      return base.MeasureOverride(availableSize);
    }
  }
}
