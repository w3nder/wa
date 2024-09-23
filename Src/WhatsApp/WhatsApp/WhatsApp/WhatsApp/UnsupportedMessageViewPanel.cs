// Decompiled with JetBrains decompiler
// Type: WhatsApp.UnsupportedMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class UnsupportedMessageViewPanel : MessageViewPanel
  {
    private Image image;
    private RichTextBlock textBlock;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Unsupported;

    public UnsupportedMessageViewPanel()
    {
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = GridLength.Auto
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      Image image = new Image();
      image.Height = 36.0 * this.zoomMultiplier;
      image.Width = 36.0 * this.zoomMultiplier;
      image.Margin = new Thickness(0.0, 4.0 * this.zoomMultiplier, 16.0 * this.zoomMultiplier, 0.0);
      image.Stretch = Stretch.UniformToFill;
      image.HorizontalAlignment = HorizontalAlignment.Center;
      image.VerticalAlignment = VerticalAlignment.Top;
      this.image = image;
      Grid.SetColumn((FrameworkElement) this.image, 0);
      this.Children.Add((UIElement) this.image);
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.Foreground = UIUtils.SubtleBrushWhite;
      richTextBlock.TextWrapping = TextWrapping.Wrap;
      richTextBlock.FontSize = 22.0 * this.zoomMultiplier;
      richTextBlock.FontStyle = FontStyles.Italic;
      richTextBlock.Margin = new Thickness(-12.0, 0.0, -12.0, 0.0);
      richTextBlock.AllowLinks = true;
      richTextBlock.EnableScan = false;
      this.textBlock = richTextBlock;
      Grid.SetColumn((FrameworkElement) this.textBlock, 1);
      this.Children.Add((UIElement) this.textBlock);
    }

    public override void Render(MessageViewModel vm)
    {
      base.Render(vm);
      if (!(vm is UnsupportedMessageViewModel messageViewModel))
        return;
      BitmapSource icon = messageViewModel.Icon;
      this.image.Source = (System.Windows.Media.ImageSource) icon;
      this.image.Visibility = (icon != null).ToVisibility();
      this.textBlock.Text = messageViewModel.GetRichText();
      this.textBlock.Margin = new Thickness(-12.0, -2.0 * this.zoomMultiplier, -12.0, 0.0);
    }
  }
}
