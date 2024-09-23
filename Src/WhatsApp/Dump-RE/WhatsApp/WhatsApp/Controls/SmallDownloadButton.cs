// Decompiled with JetBrains decompiler
// Type: WhatsApp.Controls.SmallDownloadButton
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp.Controls
{
  public class SmallDownloadButton : UserControl
  {
    private const string LogHeader = "SmallDownloadButton";
    private BitmapSource downloadIcon = (BitmapSource) ImageStore.DownloadIcon;
    private BitmapSource cancelIcon = AssetStore.DismissIconWhite;
    private const double circlePerimeter = 6.2831853071795862;
    private const double startAtTopOfCircleOffset = -1.5707963267948966;
    private const int progressStrokeThickness = 2;
    private const double progressMargin = 4.0;
    private const double verySmallOffset = 0.0005;
    private bool isDownloading;
    private Path filledInPath;
    private Path unfilledPath;
    private ArcSegment filledInProgressSegment;
    internal Canvas LayoutRoot;
    internal Border Border;
    internal Button Button;
    internal Image Icon;
    internal TextBlock TextBlock;
    private bool _contentLoaded;

    public event EventHandler Click;

    public bool IsDownloading
    {
      get => this.isDownloading;
      set
      {
        if (value && !this.isDownloading)
        {
          this.isDownloading = true;
          this.StartDownloading();
        }
        else
        {
          if (value || !this.isDownloading)
            return;
          this.isDownloading = false;
          this.StopDownloading();
        }
      }
    }

    private double radius => this.LayoutRoot.Height / 2.0;

    public SmallDownloadButton()
    {
      this.InitializeComponent();
      this.Border.Background = (Brush) new SolidColorBrush(Colors.Black);
      this.Border.Background.Opacity = 0.5;
      this.Icon.Source = (System.Windows.Media.ImageSource) this.downloadIcon;
      this.TextBlock.Margin = new Thickness(8.0, 0.0, 8.0, 0.0);
    }

    public void UpdateDownloadSize(string size) => this.TextBlock.Text = size;

    private void StartDownloading()
    {
      this.TextBlock.Visibility = Visibility.Collapsed;
      this.Icon.Source = (System.Windows.Media.ImageSource) this.cancelIcon;
      this.isDownloading = true;
      System.Windows.Point point = new System.Windows.Point(this.radius, 4.0);
      Size size = new Size(this.radius - 4.0, this.radius - 4.0);
      if (this.unfilledPath == null)
      {
        this.unfilledPath = new Path();
        PathFigure pathFigure = new PathFigure();
        PathGeometry pathGeometry = new PathGeometry();
        ArcSegment arcSegment = new ArcSegment()
        {
          RotationAngle = 45.0,
          IsLargeArc = true,
          SweepDirection = SweepDirection.Counterclockwise
        };
        pathFigure.Segments.Add((PathSegment) arcSegment);
        pathGeometry.Figures.Add(pathFigure);
        this.unfilledPath.Data = (Geometry) pathGeometry;
        pathFigure.StartPoint = point;
        arcSegment.Point = new System.Windows.Point(point.X + 0.0005, point.Y);
        this.unfilledPath.StrokeThickness = 2.0;
        this.unfilledPath.Stroke = (Brush) new SolidColorBrush(Colors.Gray);
        arcSegment.Size = size;
        this.LayoutRoot.Children.Add((UIElement) this.unfilledPath);
      }
      else
        this.unfilledPath.Visibility = Visibility.Visible;
      if (this.filledInPath == null)
      {
        this.filledInPath = new Path();
        PathFigure pathFigure = new PathFigure();
        PathGeometry pathGeometry = new PathGeometry();
        this.filledInProgressSegment = new ArcSegment()
        {
          RotationAngle = 45.0,
          IsLargeArc = true,
          SweepDirection = SweepDirection.Clockwise
        };
        pathFigure.Segments.Add((PathSegment) this.filledInProgressSegment);
        pathGeometry.Figures.Add(pathFigure);
        this.filledInPath.Data = (Geometry) pathGeometry;
        pathFigure.StartPoint = point;
        this.filledInProgressSegment.Point = this.FilledInProgressSegmentEndPoint(0.0);
        this.filledInPath.StrokeThickness = 2.0;
        this.filledInPath.Stroke = (Brush) UIUtils.AccentBrush;
        this.filledInProgressSegment.Size = size;
        this.LayoutRoot.Children.Add((UIElement) this.filledInPath);
      }
      else
        this.filledInPath.Visibility = Visibility.Visible;
    }

    public void UpdateDownloadProgress(double progress)
    {
      if (this.filledInProgressSegment == null)
        return;
      this.filledInProgressSegment.IsLargeArc = progress >= 0.5;
      this.filledInProgressSegment.Point = this.FilledInProgressSegmentEndPoint(progress);
    }

    private System.Windows.Point FilledInProgressSegmentEndPoint(double progress)
    {
      double num1 = 2.0 * Math.PI * progress - Math.PI / 2.0;
      double num2 = (this.radius - 4.0) * Math.Cos(num1) + this.radius;
      double y = (this.radius - 4.0) * Math.Sin(num1) + this.radius;
      double num3 = progress == 1.0 ? 0.0005 : 0.0;
      return new System.Windows.Point(num2 - num3, y);
    }

    private void StopDownloading()
    {
      this.TextBlock.Visibility = Visibility.Visible;
      this.Icon.Source = (System.Windows.Media.ImageSource) this.downloadIcon;
      this.isDownloading = false;
      if (this.filledInPath != null)
        this.filledInPath.Visibility = Visibility.Collapsed;
      if (this.unfilledPath == null)
        return;
      this.unfilledPath.Visibility = Visibility.Collapsed;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      EventHandler click = this.Click;
      if (click == null)
        return;
      click(sender, (EventArgs) e);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/SmallDownloadButton.xaml", UriKind.Relative));
      this.LayoutRoot = (Canvas) this.FindName("LayoutRoot");
      this.Border = (Border) this.FindName("Border");
      this.Button = (Button) this.FindName("Button");
      this.Icon = (Image) this.FindName("Icon");
      this.TextBlock = (TextBlock) this.FindName("TextBlock");
    }
  }
}
