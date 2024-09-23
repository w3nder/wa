// Decompiled with JetBrains decompiler
// Type: WhatsApp.LiveLocationPin
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class LiveLocationPin : UserControl
  {
    private int height = 58;
    private bool _isAccurate;
    private double _accuracyRadius;
    private List<System.Windows.Media.ImageSource> pinIcons;
    internal Grid LayoutRoot;
    internal Grid InnerLayout;
    internal Ellipse AccuracyEllipse;
    internal Grid PicturePanel;
    private bool _contentLoaded;

    public LiveLocationPin()
    {
      this.InitializeComponent();
      double num = (double) this.height * 0.5;
      this.PicturePanel.Clip = (Geometry) new EllipseGeometry()
      {
        Center = new System.Windows.Point(num, num),
        RadiusX = num,
        RadiusY = num
      };
    }

    public bool IsAccurate
    {
      get => this._isAccurate;
      set
      {
        this._isAccurate = value;
        this.AccuracyEllipse.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    public double AccuracyRadius
    {
      get => this._accuracyRadius;
      set
      {
        this._accuracyRadius = value;
        double num = Math.Min(800.0, value * 2.0);
        this.AccuracyEllipse.Width = num;
        this.AccuracyEllipse.Height = num;
      }
    }

    public List<System.Windows.Media.ImageSource> PinIcons
    {
      get => this.pinIcons;
      set
      {
        this.pinIcons = value;
        int count = this.pinIcons.Count;
        if (count <= 0)
          return;
        Image image1 = new Image();
        image1.Source = this.pinIcons[0];
        image1.HorizontalAlignment = HorizontalAlignment.Center;
        image1.VerticalAlignment = VerticalAlignment.Center;
        image1.Stretch = Stretch.UniformToFill;
        Image image2 = image1;
        this.PicturePanel.Children.Add((UIElement) image2);
        if (count <= 1)
          return;
        image2.RenderTransform = (Transform) new TranslateTransform()
        {
          X = (double) (-this.height / 4)
        };
        Image image3 = new Image();
        image3.Source = this.pinIcons[1];
        image3.HorizontalAlignment = HorizontalAlignment.Center;
        image3.VerticalAlignment = VerticalAlignment.Center;
        image3.Stretch = Stretch.UniformToFill;
        image3.RenderTransform = (Transform) new TranslateTransform()
        {
          X = (double) (this.height / 4)
        };
        Image image4 = image3;
        image2.Clip = (Geometry) new RectangleGeometry()
        {
          Rect = new Rect()
          {
            Height = (double) this.height,
            Width = (double) (this.height / 2 - 1 + this.height / 4),
            X = 0.0,
            Y = 0.0
          }
        };
        image4.Clip = (Geometry) new RectangleGeometry()
        {
          Rect = new Rect()
          {
            Height = (double) this.height,
            Width = (double) (this.height / 2),
            X = (double) (this.height / 4 + 1),
            Y = 0.0
          }
        };
        this.PicturePanel.Children.Add((UIElement) image4);
        if (count > 2)
        {
          image4.RenderTransform = (Transform) new TranslateTransform()
          {
            X = (double) (this.height / 4),
            Y = (double) (-this.height / 4)
          };
          Image image5 = new Image();
          image5.Source = this.pinIcons[2];
          image5.HorizontalAlignment = HorizontalAlignment.Center;
          image5.VerticalAlignment = VerticalAlignment.Center;
          image5.Stretch = Stretch.UniformToFill;
          image5.RenderTransform = (Transform) new TranslateTransform()
          {
            X = (double) (this.height / 4),
            Y = (double) (this.height / 4)
          };
          Image image6 = image5;
          Image image7 = image4;
          RectangleGeometry rectangleGeometry1 = new RectangleGeometry();
          Rect rect = new Rect();
          rect.Height = (double) (this.height / 2 - 1 + this.height / 4);
          rect.Width = (double) (this.height / 2);
          rect.X = (double) (this.height / 4 + 1);
          rect.Y = 0.0;
          rectangleGeometry1.Rect = rect;
          image7.Clip = (Geometry) rectangleGeometry1;
          Image image8 = image6;
          RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
          rect = new Rect();
          rect.Height = (double) (this.height / 2);
          rect.Width = (double) (this.height / 2);
          rect.X = (double) (this.height / 4 + 1);
          rect.Y = (double) (this.height / 4 + 1);
          rectangleGeometry2.Rect = rect;
          image8.Clip = (Geometry) rectangleGeometry2;
          this.PicturePanel.Children.Add((UIElement) image6);
          if (count > 3)
          {
            image2.RenderTransform = (Transform) new TranslateTransform()
            {
              X = (double) (-this.height / 4),
              Y = (double) (-this.height / 4)
            };
            Image image9 = new Image();
            image9.Source = this.pinIcons[3];
            image9.HorizontalAlignment = HorizontalAlignment.Center;
            image9.VerticalAlignment = VerticalAlignment.Center;
            image9.Stretch = Stretch.UniformToFill;
            image9.RenderTransform = (Transform) new TranslateTransform()
            {
              X = (double) (-this.height / 4),
              Y = (double) (this.height / 4)
            };
            Image image10 = image9;
            Image image11 = image2;
            RectangleGeometry rectangleGeometry3 = new RectangleGeometry();
            rect = new Rect();
            rect.Height = (double) (this.height / 2 - 1 + this.height / 4);
            rect.Width = (double) (this.height / 2);
            rect.X = (double) (this.height / 4 - 1);
            rect.Y = 0.0;
            rectangleGeometry3.Rect = rect;
            image11.Clip = (Geometry) rectangleGeometry3;
            Image image12 = image10;
            RectangleGeometry rectangleGeometry4 = new RectangleGeometry();
            rect = new Rect();
            rect.Height = (double) (this.height / 2);
            rect.Width = (double) (this.height / 2);
            rect.X = (double) (this.height / 4 - 1);
            rect.Y = (double) (this.height / 4 + 1);
            rectangleGeometry4.Rect = rect;
            image12.Clip = (Geometry) rectangleGeometry4;
            this.PicturePanel.Children.Add((UIElement) image10);
          }
        }
        UIElementCollection children1 = this.PicturePanel.Children;
        Grid grid = new Grid();
        grid.Background = (Brush) UIUtils.BlackBrush;
        grid.Width = (double) this.height;
        grid.Height = (double) this.height;
        grid.HorizontalAlignment = HorizontalAlignment.Center;
        grid.VerticalAlignment = VerticalAlignment.Center;
        grid.Opacity = 0.3;
        children1.Add((UIElement) grid);
        UIElementCollection children2 = this.PicturePanel.Children;
        TextBlock textBlock = new TextBlock();
        textBlock.Text = count.ToString();
        textBlock.Foreground = (Brush) UIUtils.WhiteBrush;
        textBlock.FontSize = 32.0;
        textBlock.Margin = new Thickness(0.0, -4.0, 0.0, 0.0);
        textBlock.HorizontalAlignment = HorizontalAlignment.Center;
        textBlock.VerticalAlignment = VerticalAlignment.Center;
        children2.Add((UIElement) textBlock);
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/LiveLocationPin.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.InnerLayout = (Grid) this.FindName("InnerLayout");
      this.AccuracyEllipse = (Ellipse) this.FindName("AccuracyEllipse");
      this.PicturePanel = (Grid) this.FindName("PicturePanel");
    }
  }
}
