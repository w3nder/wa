// Decompiled with JetBrains decompiler
// Type: WhatsApp.TextMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace WhatsApp
{
  public class TextMessageViewPanel : MessageViewPanel
  {
    protected TextBlock plainTextBlock;
    protected RichTextBlock richTextBlock;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Text;

    public TextMessageViewPanel()
    {
      this.plainTextBlock = new TextBlock()
      {
        Foreground = (Brush) UIUtils.WhiteBrush,
        TextWrapping = TextWrapping.Wrap
      };
      this.Children.Add((UIElement) this.plainTextBlock);
    }

    public override void Render(MessageViewModel vm)
    {
      base.Render(vm);
      this.ClearText();
      if (!vm.ShouldShowText)
        return;
      WaRichText.Chunk[] inlineFormattings = vm.InlineFormattings;
      if (vm.TextHasRichContent || inlineFormattings != null)
      {
        if (this.richTextBlock == null)
        {
          RichTextBlock richTextBlock = new RichTextBlock();
          richTextBlock.Foreground = (Brush) UIUtils.WhiteBrush;
          richTextBlock.TextWrapping = TextWrapping.Wrap;
          richTextBlock.LargeEmojiSize = true;
          this.richTextBlock = richTextBlock;
          this.Children.Add((UIElement) this.richTextBlock);
        }
        this.richTextBlock.AllowLinks = vm.AllowLinks;
        this.richTextBlock.FontSize = vm.TextFontSize;
        this.richTextBlock.Text = new RichTextBlock.TextSet()
        {
          Text = vm.TextStr,
          SerializedFormatting = (IEnumerable<LinkDetector.Result>) vm.TextPerformanceHint,
          PartialFormattings = (IEnumerable<WaRichText.Chunk>) inlineFormattings
        };
        int bottom = 0;
        FlowDirection? textFlowDirection = vm.TextFlowDirection;
        if (textFlowDirection.HasValue)
        {
          RichTextBlock richTextBlock = this.richTextBlock;
          textFlowDirection = vm.TextFlowDirection;
          int num = (int) textFlowDirection.Value;
          richTextBlock.FlowDirection = (FlowDirection) num;
          CultureInfo cult = new CultureInfo(AppResources.CultureString);
          if (cult.IsRightToLeft())
          {
            textFlowDirection = vm.TextFlowDirection;
            FlowDirection flowDirection = FlowDirection.LeftToRight;
            if ((textFlowDirection.GetValueOrDefault() == flowDirection ? (textFlowDirection.HasValue ? 1 : 0) : 0) != 0)
              goto label_9;
          }
          if (!cult.IsRightToLeft())
          {
            textFlowDirection = vm.TextFlowDirection;
            FlowDirection flowDirection = FlowDirection.RightToLeft;
            if ((textFlowDirection.GetValueOrDefault() == flowDirection ? (textFlowDirection.HasValue ? 1 : 0) : 0) == 0 || !vm.ShouldShowFooter)
              goto label_10;
          }
          else
            goto label_10;
label_9:
          bottom = 24;
        }
label_10:
        this.richTextBlock.Margin = new Thickness(-12.0, (vm.ShouldShowHeader ? -2.0 : 4.0) * this.zoomMultiplier, -12.0, (double) bottom);
        this.richTextBlock.Visibility = Visibility.Visible;
        this.plainTextBlock.Visibility = Visibility.Collapsed;
      }
      else
      {
        int bottom = 0;
        if (vm.TextFlowDirection.HasValue)
        {
          this.plainTextBlock.FlowDirection = vm.TextFlowDirection.Value;
          CultureInfo cult = new CultureInfo(AppResources.CultureString);
          FlowDirection? textFlowDirection;
          if (cult.IsRightToLeft())
          {
            textFlowDirection = vm.TextFlowDirection;
            FlowDirection flowDirection = FlowDirection.LeftToRight;
            if ((textFlowDirection.GetValueOrDefault() == flowDirection ? (textFlowDirection.HasValue ? 1 : 0) : 0) != 0)
              goto label_16;
          }
          if (!cult.IsRightToLeft())
          {
            textFlowDirection = vm.TextFlowDirection;
            FlowDirection flowDirection = FlowDirection.RightToLeft;
            if ((textFlowDirection.GetValueOrDefault() == flowDirection ? (textFlowDirection.HasValue ? 1 : 0) : 0) == 0 || !vm.ShouldShowFooter)
              goto label_17;
          }
          else
            goto label_17;
label_16:
          bottom = 24;
        }
label_17:
        this.plainTextBlock.Text = vm.TextStr;
        this.plainTextBlock.FontSize = vm.TextFontSize;
        this.plainTextBlock.Margin = new Thickness(0.0, (vm.ShouldShowHeader ? -2.0 : 4.0) * this.zoomMultiplier, 0.0, (double) bottom);
        this.ShowElement((FrameworkElement) this.richTextBlock, false);
        this.plainTextBlock.Visibility = Visibility.Visible;
      }
    }

    public override void ProcessViewModelNotification(KeyValuePair<string, object> args)
    {
      base.ProcessViewModelNotification(args);
      switch (args.Key)
      {
        case "TextFontSizeChanged":
          this.OnTextFontSizeChanged();
          break;
        case "AllowLinksChanged":
          if (this.viewModel == null || this.richTextBlock == null)
            break;
          this.richTextBlock.AllowLinks = this.viewModel.AllowLinks;
          this.richTextBlock.Refresh();
          break;
      }
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.ClearText();
    }

    private void ClearText()
    {
      this.plainTextBlock.Text = (string) null;
      if (this.richTextBlock == null)
        return;
      this.richTextBlock.Text = (RichTextBlock.TextSet) null;
    }

    private void OnTextFontSizeChanged()
    {
      double textFontSize = this.viewModel.TextFontSize;
      this.plainTextBlock.FontSize = textFontSize;
      if (this.richTextBlock == null)
        return;
      this.richTextBlock.FontSize = textFontSize;
    }
  }
}
