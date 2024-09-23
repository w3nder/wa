// Decompiled with JetBrains decompiler
// Type: WhatsApp.SegmentedCircle
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
  public class SegmentedCircle : UserControl
  {
    private List<Path> segments = new List<Path>();
    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(nameof (Radius), typeof (double), typeof (SegmentedCircle), new PropertyMetadata((object) 40.0, new PropertyChangedCallback(SegmentedCircle.OnRenderPropertyChanged)));
    public static readonly DependencyProperty SegmentCountProperty = DependencyProperty.Register(nameof (SegmentCount), typeof (int), typeof (SegmentedCircle), new PropertyMetadata((object) 1, new PropertyChangedCallback(SegmentedCircle.OnRenderPropertyChanged)));
    public static readonly DependencyProperty FillCountProperty = DependencyProperty.Register(nameof (FillCount), typeof (int), typeof (SegmentedCircle), new PropertyMetadata((object) 0, new PropertyChangedCallback(SegmentedCircle.OnRenderPropertyChanged)));
    public new static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(nameof (Padding), typeof (double), typeof (SegmentedCircle), new PropertyMetadata((object) 2.5, new PropertyChangedCallback(SegmentedCircle.OnRenderPropertyChanged)));
    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(nameof (StrokeThickness), typeof (double), typeof (SegmentedCircle), new PropertyMetadata((object) 2.5, new PropertyChangedCallback(SegmentedCircle.OnRenderPropertyChanged)));
    internal Canvas LayoutRoot;
    private bool _contentLoaded;

    public SegmentedCircle() => this.InitializeComponent();

    public double Radius
    {
      get => (double) this.GetValue(SegmentedCircle.RadiusProperty);
      set => this.SetValue(SegmentedCircle.RadiusProperty, (object) value);
    }

    public int SegmentCount
    {
      get => (int) this.GetValue(SegmentedCircle.SegmentCountProperty);
      set => this.SetValue(SegmentedCircle.SegmentCountProperty, (object) value);
    }

    public int FillCount
    {
      get => (int) this.GetValue(SegmentedCircle.FillCountProperty);
      set => this.SetValue(SegmentedCircle.FillCountProperty, (object) value);
    }

    public double Padding
    {
      get => (double) this.GetValue(SegmentedCircle.PaddingProperty);
      set => this.SetValue(SegmentedCircle.PaddingProperty, (object) value);
    }

    public double StrokeThickness
    {
      get => (double) this.GetValue(SegmentedCircle.StrokeThicknessProperty);
      set => this.SetValue(SegmentedCircle.StrokeThicknessProperty, (object) value);
    }

    public Brush Fill { get; set; }

    public void SetSegments(int total, int filled)
    {
      this.SegmentCount = total;
      this.FillCount = filled;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      int segmentCount = this.SegmentCount;
      double num1 = (segmentCount > 1 ? this.Padding : 0.005) / 360.0 * 2.0 * Math.PI;
      double num2 = 2.0 * Math.PI / (double) segmentCount;
      double radius = this.Radius;
      double strokeThickness = this.StrokeThickness;
      for (int index = 0; index < segmentCount; ++index)
      {
        double x1 = radius * Math.Cos((double) index * num2 - Math.PI / 2.0 + num1) + radius;
        double y1 = radius * Math.Sin((double) index * num2 - Math.PI / 2.0 + num1) + radius;
        double x2 = radius * Math.Cos((double) (index + 1) * num2 - Math.PI / 2.0 - num1) + radius;
        double y2 = radius * Math.Sin((double) (index + 1) * num2 - Math.PI / 2.0 - num1) + radius;
        ArcSegment arcSegment;
        PathFigure pathFigure;
        Path path;
        if (index >= this.segments.Count)
        {
          arcSegment = new ArcSegment()
          {
            RotationAngle = 45.0,
            IsLargeArc = false,
            SweepDirection = SweepDirection.Clockwise
          };
          pathFigure = new PathFigure();
          pathFigure.Segments.Add((PathSegment) arcSegment);
          PathGeometry pathGeometry = new PathGeometry();
          pathGeometry.Figures.Add(pathFigure);
          path = new Path();
          path.Data = (Geometry) pathGeometry;
          this.segments.Add(path);
          this.LayoutRoot.Children.Add((UIElement) path);
        }
        else
        {
          path = this.segments[index];
          path.Visibility = Visibility.Visible;
          pathFigure = ((PathGeometry) path.Data).Figures[0];
          arcSegment = pathFigure.Segments[0] as ArcSegment;
        }
        arcSegment.IsLargeArc = segmentCount == 1;
        pathFigure.StartPoint = new System.Windows.Point(x1, y1);
        arcSegment.Point = new System.Windows.Point(x2, y2);
        path.StrokeThickness = strokeThickness;
        path.Stroke = index < this.FillCount ? this.Fill : this.Background;
        arcSegment.Size = new Size(radius, radius);
      }
      if (this.segments.Count > segmentCount)
      {
        for (int index = segmentCount; index < this.segments.Count; ++index)
          this.segments[index].Visibility = Visibility.Collapsed;
      }
      return base.MeasureOverride(availableSize);
    }

    private static void OnRenderPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is SegmentedCircle segmentedCircle))
        return;
      segmentedCircle.InvalidateMeasure();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/SegmentedCircle.xaml", UriKind.Relative));
      this.LayoutRoot = (Canvas) this.FindName("LayoutRoot");
    }
  }
}
