// Decompiled with JetBrains decompiler
// Type: WhatsApp.UnreadDividerViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace WhatsApp
{
  internal class UnreadDividerViewPanel : MessageViewPanel
  {
    private Image arrowIcon;
    private TextBlock textBlock;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.UnreadDivider;

    public UnreadDividerViewPanel()
    {
      Image image = new Image();
      image.Stretch = Stretch.UniformToFill;
      image.IsHitTestVisible = false;
      image.HorizontalAlignment = HorizontalAlignment.Left;
      image.VerticalAlignment = VerticalAlignment.Center;
      image.Margin = new Thickness(0.0);
      image.Width = 26.4 * this.zoomMultiplier;
      image.Height = 16.0 * this.zoomMultiplier;
      this.arrowIcon = image;
      this.Children.Add((UIElement) this.arrowIcon);
      TextBlock textBlock = new TextBlock();
      textBlock.TextWrapping = TextWrapping.NoWrap;
      textBlock.HorizontalAlignment = HorizontalAlignment.Center;
      textBlock.FontSize = 24.0 * this.zoomMultiplier;
      this.textBlock = textBlock;
      this.Children.Add((UIElement) this.textBlock);
    }

    public override void Render(MessageViewModel vm)
    {
      if (!(vm is UnreadDividerViewModel dividerViewModel))
        return;
      base.Render(vm);
      this.arrowIcon.Source = dividerViewModel.UnreadDividerChevronIcon;
      this.textBlock.Foreground = dividerViewModel.ForegroundBrush;
      this.textBlock.FontFamily = dividerViewModel.UnreadDividerForegroundFontFamily;
      this.textBlock.Text = vm.TextStr;
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.arrowIcon.Source = (System.Windows.Media.ImageSource) null;
    }

    public override void ProcessViewModelNotification(KeyValuePair<string, object> args)
    {
      base.ProcessViewModelNotification(args);
      if (args.Key == "ChatBackgroundChanged")
      {
        this.textBlock.Foreground = this.viewModel.ForegroundBrush;
      }
      else
      {
        if (!(args.Key == "TextStr"))
          return;
        TextBlock textBlock = this.textBlock;
        if (!(args.Value is string str))
          str = this.viewModel?.TextStr ?? "";
        textBlock.Text = str;
      }
    }
  }
}
