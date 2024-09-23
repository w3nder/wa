// Decompiled with JetBrains decompiler
// Type: WhatsApp.WallpaperPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class WallpaperPanel : Grid
  {
    private Image wallpaperImage;
    private List<Rectangle> fgProtections = new List<Rectangle>();

    public WallpaperPanel() => this.Init(false);

    public WallpaperPanel(bool createBottomOverlay) => this.Init(createBottomOverlay);

    private void Init(bool createBottomOverlay)
    {
      this.wallpaperImage = new Image()
      {
        Stretch = Stretch.UniformToFill
      };
      this.Children.Add((UIElement) this.wallpaperImage);
      Rectangle rectangle1 = new Rectangle();
      rectangle1.Height = 160.0 * ResolutionHelper.ZoomMultiplier;
      rectangle1.VerticalAlignment = VerticalAlignment.Top;
      LinearGradientBrush linearGradientBrush1 = new LinearGradientBrush();
      GradientStopCollection gradientStopCollection1 = new GradientStopCollection();
      gradientStopCollection1.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 153, (byte) 0, (byte) 0, (byte) 0),
        Offset = 0.0
      });
      gradientStopCollection1.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0),
        Offset = 1.0
      });
      linearGradientBrush1.GradientStops = gradientStopCollection1;
      linearGradientBrush1.StartPoint = new System.Windows.Point(0.0, 0.0);
      linearGradientBrush1.EndPoint = new System.Windows.Point(0.0, 1.0);
      rectangle1.Fill = (Brush) linearGradientBrush1;
      Rectangle rectangle2 = rectangle1;
      this.fgProtections.Add(rectangle2);
      this.Children.Add((UIElement) rectangle2);
      if (!createBottomOverlay)
        return;
      Rectangle rectangle3 = new Rectangle();
      rectangle3.Height = 200.0 * ResolutionHelper.ZoomMultiplier;
      rectangle3.VerticalAlignment = VerticalAlignment.Bottom;
      LinearGradientBrush linearGradientBrush2 = new LinearGradientBrush();
      GradientStopCollection gradientStopCollection2 = new GradientStopCollection();
      gradientStopCollection2.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0),
        Offset = 0.0
      });
      gradientStopCollection2.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 51, (byte) 0, (byte) 0, (byte) 0),
        Offset = 0.4
      });
      gradientStopCollection2.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 102, (byte) 0, (byte) 0, (byte) 0),
        Offset = 1.0
      });
      linearGradientBrush2.GradientStops = gradientStopCollection2;
      linearGradientBrush2.StartPoint = new System.Windows.Point(0.0, 0.0);
      linearGradientBrush2.EndPoint = new System.Windows.Point(0.0, 1.0);
      rectangle3.Fill = (Brush) linearGradientBrush2;
      Rectangle rectangle4 = rectangle3;
      this.fgProtections.Add(rectangle4);
      this.Children.Add((UIElement) rectangle4);
    }

    public void Clear() => this.Set((WallpaperStore.WallpaperState) null);

    public bool Set(WallpaperStore.WallpaperState wallpaper, System.Windows.Media.ImageSource preloadedImgSrc = null)
    {
      System.Windows.Media.ImageSource imageSource = (System.Windows.Media.ImageSource) null;
      SolidColorBrush solidColorBrush = UIUtils.TransparentBrush;
      bool flag = false;
      if (wallpaper != null)
      {
        Color? solidColor = wallpaper.SolidColor;
        if (solidColor.HasValue)
        {
          solidColor = wallpaper.SolidColor;
          solidColorBrush = new SolidColorBrush(solidColor.Value);
          flag = true;
        }
        else
        {
          imageSource = preloadedImgSrc ?? wallpaper.Image;
          flag = imageSource != null;
        }
      }
      this.wallpaperImage.Source = imageSource;
      this.Background = (Brush) solidColorBrush;
      this.fgProtections.ForEach((Action<Rectangle>) (elem =>
      {
        if (elem == null)
          return;
        elem.Visibility = (wallpaper != null && wallpaper.NeedsForegroundProtection).ToVisibility();
      }));
      this.Opacity = flag ? 1.0 : 0.0;
      return flag;
    }
  }
}
