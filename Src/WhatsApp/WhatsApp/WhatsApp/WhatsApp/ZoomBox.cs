// Decompiled with JetBrains decompiler
// Type: WhatsApp.ZoomBox
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using ScreenSizeSupport.Misc;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace WhatsApp
{
  [TemplatePart(Name = "contentHolder", Type = typeof (UIElement))]
  public class ZoomBox : ContentControl
  {
    public const string ContentHolderPartName = "contentHolder";
    private readonly ScaleTransform transform = new ScaleTransform();
    private UIElement contentHolder;
    public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(nameof (ZoomFactor), typeof (double), typeof (ZoomBox), new PropertyMetadata((object) 1.0, new PropertyChangedCallback(ZoomBox.OnZoomFactorPropertyChanged)));

    public double ZoomFactor
    {
      get => (double) this.GetValue(ZoomBox.ZoomFactorProperty);
      set => this.SetValue(ZoomBox.ZoomFactorProperty, (object) value);
    }

    private static void OnZoomFactorPropertyChanged(
      DependencyObject source,
      DependencyPropertyChangedEventArgs e)
    {
      ZoomBox zoomBox = (ZoomBox) source;
      if (double.IsNaN((double) e.NewValue) || (double) e.NewValue <= 0.0)
      {
        zoomBox.ZoomFactor = (double) e.OldValue;
        throw new ArgumentOutOfRangeException("ZoomFactor", "must be a positive number");
      }
      zoomBox.InvalidateMeasure();
    }

    public static ZoomBox GetForElement(UIElement element)
    {
      for (UIElement reference = element; reference != null; reference = VisualTreeHelper.GetParent((DependencyObject) reference) as UIElement)
      {
        if (reference is ZoomBox)
          return reference as ZoomBox;
      }
      return (ZoomBox) null;
    }

    public ZoomBox()
    {
      this.transform = new ScaleTransform()
      {
        ScaleX = 1.0,
        ScaleY = 1.0
      };
      this.DefaultStyleKey = (object) typeof (ZoomBox);
    }

    public override void OnApplyTemplate()
    {
      if (this.contentHolder != null)
        this.contentHolder.RenderTransform = (Transform) null;
      this.contentHolder = (UIElement) null;
      base.OnApplyTemplate();
      if (!(this.GetTemplateChild("contentHolder") is UIElement templateChild))
        return;
      this.contentHolder = templateChild;
      if (this.ZoomFactor == 1.0)
        return;
      this.contentHolder.RenderTransform = (Transform) this.transform;
    }

    protected override Size ArrangeOverride(Size finalSizeInHostCoordinates)
    {
      double zoomFactor = this.ZoomFactor;
      Size size = base.ArrangeOverride(finalSizeInHostCoordinates.Scale(zoomFactor)).Scale(1.0 / zoomFactor);
      if (zoomFactor != 1.0)
      {
        this.transform.ScaleX = this.transform.ScaleY = 1.0 / zoomFactor;
        this.contentHolder.RenderTransform = (Transform) this.transform;
        return size;
      }
      this.contentHolder.RenderTransform = (Transform) null;
      return size;
    }

    protected override Size MeasureOverride(Size availableSizeInHostCoordinates)
    {
      double zoomFactor = this.ZoomFactor;
      return base.MeasureOverride(availableSizeInHostCoordinates.Scale(zoomFactor)).Scale(1.0 / zoomFactor);
    }
  }
}
