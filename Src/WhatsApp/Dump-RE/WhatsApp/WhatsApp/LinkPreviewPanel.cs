// Decompiled with JetBrains decompiler
// Type: WhatsApp.LinkPreviewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class LinkPreviewPanel : Grid, IDisposable
  {
    private Image thumbnailImage;
    private Rectangle thumbnailBackground;
    private Grid detailsPanel;
    private TextBlock titleBlock;
    private TextBlock descriptionBlock;
    private TextBlock hostnameBlock;
    private StackPanel bottomPanel;
    private TextBlock linkBlock;
    private Button openLinkButton;
    private Brush foregroundBrush;
    private double zoomMultiplier = 1.0;
    private IDisposable thumbSub;
    private string openLinkTarget;
    public static int ThumbsizeBase = 100;

    public Button DismissButton { get; private set; }

    public LinkPreviewPanel(
      double zoomMultiplier,
      SolidColorBrush fgBrush = null,
      SolidColorBrush bgBrush = null)
    {
      if (fgBrush == null)
        fgBrush = UIUtils.ForegroundBrush;
      this.foregroundBrush = (Brush) fgBrush;
      if (bgBrush == null)
        bgBrush = UIUtils.BackgroundBrush;
      this.zoomMultiplier = zoomMultiplier;
      int num1 = (int) ((double) LinkPreviewPanel.ThumbsizeBase * zoomMultiplier + 0.5);
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      Rectangle rectangle = new Rectangle();
      rectangle.Fill = UIUtils.PhoneChromeBrush;
      rectangle.Width = (double) num1;
      rectangle.Height = (double) num1;
      this.thumbnailBackground = rectangle;
      Grid.SetColumn((FrameworkElement) this.thumbnailBackground, 0);
      this.Children.Add((UIElement) this.thumbnailBackground);
      Image image1 = new Image();
      image1.HorizontalAlignment = HorizontalAlignment.Center;
      image1.VerticalAlignment = VerticalAlignment.Center;
      image1.Width = (double) num1;
      image1.Height = (double) num1;
      image1.Stretch = Stretch.UniformToFill;
      this.thumbnailImage = image1;
      Grid.SetColumn((FrameworkElement) this.thumbnailImage, 0);
      this.Children.Add((UIElement) this.thumbnailImage);
      Grid grid = new Grid();
      grid.Margin = new Thickness(6.0, 6.0, 6.0, 0.0);
      grid.Height = (double) num1;
      this.detailsPanel = grid;
      Grid.SetColumn((FrameworkElement) this.detailsPanel, 1);
      this.Children.Add((UIElement) this.detailsPanel);
      StackPanel stackPanel1 = new StackPanel();
      stackPanel1.VerticalAlignment = VerticalAlignment.Top;
      StackPanel stackPanel2 = stackPanel1;
      this.detailsPanel.Children.Add((UIElement) stackPanel2);
      TextBlock textBlock1 = new TextBlock();
      textBlock1.Foreground = (Brush) fgBrush;
      textBlock1.TextWrapping = TextWrapping.NoWrap;
      textBlock1.FontSize = 22.0 * zoomMultiplier;
      textBlock1.VerticalAlignment = VerticalAlignment.Top;
      this.titleBlock = textBlock1;
      stackPanel2.Children.Add((UIElement) this.titleBlock);
      TextBlock textBlock2 = new TextBlock();
      textBlock2.Foreground = (Brush) fgBrush;
      textBlock2.TextWrapping = TextWrapping.NoWrap;
      textBlock2.FontSize = 22.0 * zoomMultiplier;
      textBlock2.VerticalAlignment = VerticalAlignment.Top;
      textBlock2.Opacity = 0.65;
      this.descriptionBlock = textBlock2;
      stackPanel2.Children.Add((UIElement) this.descriptionBlock);
      TextBlock textBlock3 = new TextBlock();
      textBlock3.Foreground = (Brush) fgBrush;
      textBlock3.TextWrapping = TextWrapping.NoWrap;
      textBlock3.TextTrimming = TextTrimming.WordEllipsis;
      textBlock3.FontSize = 16.0 * zoomMultiplier;
      textBlock3.VerticalAlignment = VerticalAlignment.Bottom;
      textBlock3.Margin = new Thickness(0.0, 0.0, 0.0, 2.0);
      this.hostnameBlock = textBlock3;
      this.detailsPanel.Children.Add((UIElement) this.hostnameBlock);
      Rectangle fadingGradient = UIUtils.CreateFadingGradient(bgBrush.Color, new System.Windows.Point(1.0, 0.0), new System.Windows.Point(0.0, 0.0));
      fadingGradient.HorizontalAlignment = HorizontalAlignment.Right;
      fadingGradient.VerticalAlignment = VerticalAlignment.Stretch;
      fadingGradient.Width = 48.0 * zoomMultiplier;
      Grid.SetColumn((FrameworkElement) fadingGradient, 1);
      this.Children.Add((UIElement) fadingGradient);
      double num2 = 36.0 * zoomMultiplier;
      Image image2 = new Image();
      image2.Stretch = Stretch.UniformToFill;
      image2.VerticalAlignment = VerticalAlignment.Center;
      image2.HorizontalAlignment = HorizontalAlignment.Center;
      image2.Source = (System.Windows.Media.ImageSource) AssetStore.DismissIcon;
      image2.Width = num2;
      image2.Height = num2;
      Image image3 = image2;
      Button button = new Button();
      button.Margin = new Thickness(12.0 * zoomMultiplier, 0.0, 12.0 * zoomMultiplier, 0.0);
      button.Padding = new Thickness(0.0);
      button.Style = Application.Current.Resources[(object) "BorderlessButton"] as Style;
      button.VerticalAlignment = VerticalAlignment.Center;
      button.HorizontalAlignment = HorizontalAlignment.Center;
      button.VerticalContentAlignment = VerticalAlignment.Center;
      button.HorizontalContentAlignment = HorizontalAlignment.Center;
      button.Content = (object) image3;
      button.Width = num2;
      button.Height = num2;
      Button element = button;
      Grid.SetColumn((FrameworkElement) element, 2);
      this.Children.Add((UIElement) element);
      this.DismissButton = element;
    }

    public void ShowLinkPanel(bool show, Brush bgBrush = null)
    {
      if (show && this.bottomPanel == null)
      {
        this.RowDefinitions.Clear();
        this.RowDefinitions.Add(new RowDefinition()
        {
          Height = new GridLength(1.0, GridUnitType.Auto)
        });
        this.RowDefinitions.Add(new RowDefinition()
        {
          Height = new GridLength(1.0, GridUnitType.Auto)
        });
        StackPanel stackPanel = new StackPanel();
        stackPanel.Background = bgBrush;
        this.bottomPanel = stackPanel;
        Grid.SetColumnSpan((FrameworkElement) this.bottomPanel, 3);
        Grid.SetRow((FrameworkElement) this.bottomPanel, 1);
        this.Children.Add((UIElement) this.bottomPanel);
        TextBlock textBlock = new TextBlock();
        textBlock.Foreground = this.foregroundBrush;
        textBlock.TextWrapping = TextWrapping.Wrap;
        textBlock.FontSize = 22.0 * this.zoomMultiplier;
        textBlock.VerticalAlignment = VerticalAlignment.Top;
        textBlock.Margin = new Thickness(6.0, 12.0, 6.0, 12.0);
        this.linkBlock = textBlock;
        this.linkBlock.Tap += (EventHandler<GestureEventArgs>) ((sender, e) => this.OpenLink(this.openLinkTarget));
        this.bottomPanel.Children.Add((UIElement) this.linkBlock);
        Button button = new Button();
        button.Content = (object) AppResources.OpenLink;
        button.HorizontalAlignment = HorizontalAlignment.Left;
        button.Foreground = this.foregroundBrush;
        button.BorderBrush = this.foregroundBrush;
        this.openLinkButton = button;
        this.openLinkButton.Click += (RoutedEventHandler) ((sender, e) => this.OpenLink(this.openLinkTarget));
        this.bottomPanel.Children.Add((UIElement) this.openLinkButton);
      }
      if (this.bottomPanel == null)
        return;
      this.bottomPanel.Visibility = show.ToVisibility();
    }

    private void OpenLink(string link)
    {
      if (string.IsNullOrEmpty(link))
        return;
      Uri result = (Uri) null;
      if (!Uri.TryCreate(link, UriKind.Absolute, out result) || !(result != (Uri) null))
        return;
      new WebBrowserTask() { Uri = result }.Show();
    }

    public void Update(WebPageMetadata data)
    {
      this.thumbSub.SafeDispose();
      this.thumbSub = (IDisposable) null;
      if (data.ThumbnailUrl == null && data.Thumbnail == null)
        this.thumbnailImage.Visibility = this.thumbnailBackground.Visibility = Visibility.Collapsed;
      else
        this.thumbSub = data.LoadThumbnail().ObserveOnDispatcher<BitmapSource>().Subscribe<BitmapSource>((Action<BitmapSource>) (bitmap =>
        {
          if (bitmap == null)
          {
            this.thumbnailImage.Visibility = this.thumbnailBackground.Visibility = Visibility.Collapsed;
          }
          else
          {
            this.thumbnailImage.Source = (System.Windows.Media.ImageSource) bitmap;
            this.thumbnailImage.Visibility = this.thumbnailBackground.Visibility = Visibility.Visible;
          }
          this.thumbSub.SafeDispose();
          this.thumbSub = (IDisposable) null;
        }));
      bool flag = false;
      if (string.IsNullOrEmpty(data.Title))
      {
        this.titleBlock.Text = "";
      }
      else
      {
        this.titleBlock.Text = data.Title;
        flag = true;
      }
      if (string.IsNullOrEmpty(data.Description))
      {
        this.descriptionBlock.Text = "";
      }
      else
      {
        this.descriptionBlock.Text = data.Description;
        flag = true;
      }
      string uriString = data.CanonicalUrl ?? data.OriginalUrl;
      string str = (string) null;
      if (uriString != null)
      {
        try
        {
          str = new Uri(uriString).Host;
        }
        catch (Exception ex)
        {
        }
      }
      this.hostnameBlock.Text = str ?? "";
      this.detailsPanel.Visibility = flag.ToVisibility();
      if (this.linkBlock != null)
        this.linkBlock.Text = uriString;
      this.openLinkTarget = uriString;
    }

    public void Dispose()
    {
      this.thumbSub.SafeDispose();
      this.thumbSub = (IDisposable) null;
    }
  }
}
