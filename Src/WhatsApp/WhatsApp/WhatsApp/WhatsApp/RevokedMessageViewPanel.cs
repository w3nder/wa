// Decompiled with JetBrains decompiler
// Type: WhatsApp.RevokedMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;


namespace WhatsApp
{
  public class RevokedMessageViewPanel : MessageViewPanel
  {
    private Image image;
    protected RichTextBox textBlock;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Revoked;

    public RevokedMessageViewPanel()
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
      image.Height = 20.0 * this.zoomMultiplier;
      image.Width = 20.0 * this.zoomMultiplier;
      image.Margin = new Thickness(0.0, 4.0 * this.zoomMultiplier, 0.0, 0.0);
      image.Stretch = Stretch.UniformToFill;
      image.HorizontalAlignment = HorizontalAlignment.Center;
      image.VerticalAlignment = VerticalAlignment.Center;
      image.Opacity = 0.65;
      this.image = image;
      Grid.SetColumn((FrameworkElement) this.image, 0);
      this.Children.Add((UIElement) this.image);
      RichTextBox richTextBox = new RichTextBox();
      richTextBox.Foreground = (Brush) UIUtils.WhiteBrush;
      richTextBox.TextWrapping = TextWrapping.Wrap;
      richTextBox.FontSize = 24.0 * this.zoomMultiplier;
      richTextBox.Opacity = 0.65;
      richTextBox.FontStyle = FontStyles.Italic;
      this.textBlock = richTextBox;
      Grid.SetColumn((FrameworkElement) this.textBlock, 1);
      this.Children.Add((UIElement) this.textBlock);
    }

    public override void Render(MessageViewModel vm)
    {
      base.Render(vm);
      if (!(vm is RevokedMessageViewModel messageViewModel))
        return;
      this.image.Source = (System.Windows.Media.ImageSource) messageViewModel.Icon;
      Paragraph paragraph = new Paragraph();
      paragraph.Inlines.Add(vm.TextStr);
      this.textBlock.Blocks.Clear();
      this.textBlock.Blocks.Add((Block) paragraph);
      this.textBlock.Margin = new Thickness(0.0, (vm.ShouldShowHeader ? -2.0 : 4.0) * this.zoomMultiplier, 0.0, 0.0);
    }
  }
}
