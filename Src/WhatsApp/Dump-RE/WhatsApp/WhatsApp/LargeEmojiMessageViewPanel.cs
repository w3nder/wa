// Decompiled with JetBrains decompiler
// Type: WhatsApp.LargeEmojiMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace WhatsApp
{
  internal class LargeEmojiMessageViewPanel : MessageViewPanel
  {
    private RichTextBlock emojiBlock;
    private IDisposable activeAnimationSub;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.LargeEmoji;

    public LargeEmojiMessageViewPanel()
    {
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
      richTextBlock.Margin = new Thickness(0.0, 0.0, 0.0, 28.0 * this.zoomMultiplier);
      richTextBlock.LargeEmojiSize = true;
      richTextBlock.HorizontalContentAlignment = HorizontalAlignment.Center;
      richTextBlock.VerticalAlignment = VerticalAlignment.Center;
      richTextBlock.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
      richTextBlock.RenderTransform = (Transform) new CompositeTransform();
      this.emojiBlock = richTextBlock;
      Grid.SetRow((FrameworkElement) this.emojiBlock, 0);
      this.Children.Add((UIElement) this.emojiBlock);
    }

    public override void Render(MessageViewModel vm)
    {
      if (!(vm is LargeEmojiMessageViewModel messageViewModel))
        return;
      base.Render(vm);
      if (messageViewModel.NumEmojiChars == 1)
        this.emojiBlock.FontSize = 48.0 * this.zoomMultiplier;
      else if (messageViewModel.NumEmojiChars == 2)
        this.emojiBlock.FontSize = 40.0 * this.zoomMultiplier;
      else if (messageViewModel.NumEmojiChars == 3)
        this.emojiBlock.FontSize = 32.0 * this.zoomMultiplier;
      this.emojiBlock.Text = new RichTextBlock.TextSet()
      {
        Text = vm.Message.GetTextForDisplay()
      };
      Storyboard sb = messageViewModel.Animation;
      if (sb == null)
        return;
      this.activeAnimationSub = Storyboarder.PerformWithDisposable(sb, (DependencyObject) this.emojiBlock.RenderTransform, false, (Action) null, (Action) (() => sb.Stop()), "large emoji animation");
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.emojiBlock.Text = (RichTextBlock.TextSet) null;
    }

    protected override void DisposeSubscriptions()
    {
      base.DisposeSubscriptions();
      IDisposable sub = this.activeAnimationSub;
      if (sub == null)
        return;
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
      {
        sub.SafeDispose();
        sub = (IDisposable) null;
      }));
    }
  }
}
