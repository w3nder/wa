// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace WhatsApp
{
  public class PaymentMessageViewPanel : MessageViewPanel
  {
    private RichTextBlock transactionSummaryBlock;
    private Image bgImage;
    private TextBlock currencyBlock;
    private TextBlock integersBlock;
    private TextBlock decimalsBlock;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Payment;

    public PaymentMessageViewPanel()
    {
      this.Width = MessageViewModel.DefaultContentWidth;
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.Foreground = UIUtils.SubtleBrushWhite;
      richTextBlock.TextWrapping = TextWrapping.NoWrap;
      richTextBlock.FontSize = 20.0 * this.zoomMultiplier;
      richTextBlock.Margin = new Thickness(-12.0, 0.0, -12.0, 0.0);
      this.transactionSummaryBlock = richTextBlock;
      Grid.SetRow((FrameworkElement) this.transactionSummaryBlock, 0);
      this.Children.Add((UIElement) this.transactionSummaryBlock);
      Grid grid1 = new Grid();
      grid1.Height = 132.0 * this.zoomMultiplier;
      grid1.Margin = new Thickness(0.0, 6.0 * this.zoomMultiplier, 0.0, 0.0);
      Grid element = grid1;
      Grid.SetRow((FrameworkElement) element, 1);
      this.Children.Add((UIElement) element);
      Image image = new Image();
      image.Stretch = Stretch.UniformToFill;
      image.Source = (System.Windows.Media.ImageSource) AssetStore.PaymentBackground;
      image.Opacity = 0.3;
      this.bgImage = image;
      element.Children.Add((UIElement) this.bgImage);
      StackPanel stackPanel1 = new StackPanel();
      stackPanel1.HorizontalAlignment = HorizontalAlignment.Center;
      stackPanel1.VerticalAlignment = VerticalAlignment.Center;
      stackPanel1.Orientation = Orientation.Horizontal;
      stackPanel1.Margin = new Thickness(0.0, 4.0 * this.zoomMultiplier, 0.0, 0.0);
      StackPanel stackPanel2 = stackPanel1;
      element.Children.Add((UIElement) stackPanel2);
      double num1 = 28.0 * this.zoomMultiplier;
      double num2 = num1 * 0.5;
      Grid grid2 = new Grid();
      grid2.Background = (Brush) UIUtils.WhiteBrush;
      grid2.Width = num1;
      grid2.Height = num1;
      grid2.Clip = (Geometry) new EllipseGeometry()
      {
        Center = new System.Windows.Point(num2, num2),
        RadiusX = num2,
        RadiusY = num2
      };
      grid2.VerticalAlignment = VerticalAlignment.Top;
      Grid grid3 = grid2;
      stackPanel2.Children.Add((UIElement) grid3);
      TextBlock textBlock1 = new TextBlock();
      textBlock1.FontSize = 22.0 * this.zoomMultiplier;
      textBlock1.FontWeight = FontWeights.SemiBold;
      textBlock1.VerticalAlignment = VerticalAlignment.Center;
      textBlock1.HorizontalAlignment = HorizontalAlignment.Center;
      this.currencyBlock = textBlock1;
      grid3.Children.Add((UIElement) this.currencyBlock);
      TextBlock textBlock2 = new TextBlock();
      textBlock2.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock2.FontSize = 56.0 * this.zoomMultiplier;
      textBlock2.VerticalAlignment = VerticalAlignment.Top;
      textBlock2.Margin = new Thickness(6.0 * this.zoomMultiplier, -18.0 * this.zoomMultiplier, 0.0, 0.0);
      this.integersBlock = textBlock2;
      stackPanel2.Children.Add((UIElement) this.integersBlock);
      TextBlock textBlock3 = new TextBlock();
      textBlock3.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock3.FontSize = 28.0 * this.zoomMultiplier;
      textBlock3.VerticalAlignment = VerticalAlignment.Top;
      textBlock3.Margin = new Thickness(0.0, -8.0 * this.zoomMultiplier, 0.0, 0.0);
      this.decimalsBlock = textBlock3;
      stackPanel2.Children.Add((UIElement) this.decimalsBlock);
    }

    public override void Render(MessageViewModel vm)
    {
      base.Render(vm);
      this.transactionSummaryBlock.Text = vm.PaymentInfoStr;
      this.currencyBlock.Foreground = (Brush) vm.BackgroundBrush;
      this.currencyBlock.Text = vm.PaymentCurrencyStr;
      this.integersBlock.Text = vm.PaymentAmountIntegerStr;
      this.decimalsBlock.Text = vm.PaymentAmountDecimalStr;
    }
  }
}
