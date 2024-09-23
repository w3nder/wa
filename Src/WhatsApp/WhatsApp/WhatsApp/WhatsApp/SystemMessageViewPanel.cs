// Decompiled with JetBrains decompiler
// Type: WhatsApp.SystemMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WhatsApp.CommonOps;


namespace WhatsApp
{
  internal class SystemMessageViewPanel : MessageViewPanel
  {
    private RichTextBlock textBlock;
    private Image icon;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.System;

    public SystemMessageViewPanel()
    {
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.TextWrapping = TextWrapping.Wrap;
      richTextBlock.TextAlignment = TextAlignment.Center;
      richTextBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
      richTextBlock.FontSize = MessageViewModel.SmallTextFontSizeStatic;
      richTextBlock.LargeEmojiSize = true;
      this.textBlock = richTextBlock;
      this.Children.Add((UIElement) this.textBlock);
      this.Tap += new EventHandler<GestureEventArgs>(this.OnTap);
    }

    public override void Render(MessageViewModel vm)
    {
      base.Render(vm);
      this.textBlock.Foreground = vm.ForegroundBrush;
      this.textBlock.Text = new RichTextBlock.TextSet()
      {
        Text = vm.TextStr
      };
      if (vm is SystemMessageViewModel messageViewModel && messageViewModel.ShowSystemMessageIcon)
      {
        if (this.icon == null)
        {
          Image image = new Image();
          image.Width = vm.TextFontSize;
          image.Height = vm.TextFontSize;
          this.icon = image;
          this.Children.Add((UIElement) this.icon);
          Grid.SetColumn((FrameworkElement) this.textBlock, 1);
          this.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = GridLength.Auto
          });
          this.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.0, GridUnitType.Star)
          });
        }
        this.icon.Source = messageViewModel.SystemMessageIcon;
      }
      else
      {
        if (this.icon == null)
          return;
        this.Children.Remove((UIElement) this.icon);
        Grid.SetColumn((FrameworkElement) this.textBlock, 0);
        this.ColumnDefinitions.Clear();
        this.icon = (Image) null;
      }
    }

    private void OnTap(object sender, EventArgs e)
    {
      if (!(this.viewModel is SystemMessageViewModel viewModel) || !viewModel.IsTapAllowed)
        return;
      ViewMessage.View(this.viewModel.Message);
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
        if (!(args.Key == "TextChanged"))
          return;
        this.textBlock.Text = new RichTextBlock.TextSet()
        {
          Text = this.viewModel.TextStr
        };
      }
    }
  }
}
