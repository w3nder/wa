// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Overlays.Scale
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Shapes;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Overlays
{
  [TemplatePart(Name = "ScaleString", Type = typeof (ShadowText))]
  [TemplatePart(Name = "ScaleRectangle", Type = typeof (Rectangle))]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class Scale : Overlay
  {
    internal const string ScaleStringElementName = "ScaleString";
    internal const string ScaleRectangleElementName = "ScaleRectangle";
    private const int metersPerKm = 1000;
    private const double yardsPerMeter = 1.0936133;
    private const int yardsPerMile = 1760;
    private const int feetPerYard = 3;
    private const double feetPerMeter = 3.2808398999999997;
    private const int feetPerMile = 5280;
    private double currentMetersPerPixel;
    private double previousMaxWidth;
    private RegionInfo regionInfo;
    private double scaleInMetersPerPixel;
    private Rectangle scaleRectangle;
    private ShadowText scaleString;
    private ModeBackground setForBackground;
    public static readonly DependencyProperty DistanceUnitProperty = DependencyProperty.Register(nameof (DistanceUnit), typeof (DistanceUnit), typeof (Scale), new PropertyMetadata(new PropertyChangedCallback(Scale.OnUnitChanged)));
    public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(nameof (Culture), typeof (CultureInfo), typeof (Scale), new PropertyMetadata(new PropertyChangedCallback(Scale.OnCultureChanged)));
    private static readonly int[] singleDigitValues = new int[2]
    {
      5,
      2
    };
    private static readonly double[] multiDigitValues = new double[3]
    {
      5.0,
      2.5,
      2.0
    };

    public Scale()
    {
      this.DefaultStyleKey = (object) typeof (Scale);
      this.MaxWidth = 150.0;
      this.scaleString = new ShadowText();
      this.scaleRectangle = new Rectangle();
      this.LayoutUpdated += new EventHandler(this.Scale_LayoutUpdated);
    }

    public double MetersPerPixel
    {
      get => this.scaleInMetersPerPixel;
      internal set
      {
        this.scaleInMetersPerPixel = value;
        this.OnPerPixelChanged();
      }
    }

    public DistanceUnit DistanceUnit
    {
      get => (DistanceUnit) this.GetValue(Scale.DistanceUnitProperty);
      set => this.SetValue(Scale.DistanceUnitProperty, (object) value);
    }

    public CultureInfo Culture
    {
      get => (CultureInfo) this.GetValue(Scale.CultureProperty);
      set => this.SetValue(Scale.CultureProperty, (object) value);
    }

    internal ModeBackground SetForBackground
    {
      get => this.setForBackground;
      set
      {
        this.setForBackground = value;
        if (this.setForBackground == ModeBackground.Light)
          this.scaleString.SetForegroundColorsForLightBackground();
        else
          this.scaleString.SetForegroundColorsForDarkBackground();
      }
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.scaleString = this.GetTemplateChild("ScaleString") as ShadowText;
      this.scaleRectangle = this.GetTemplateChild("ScaleRectangle") as Rectangle;
      this.Refresh();
      this.FireTemplateApplied();
    }

    private void SetScaling(double metersPerPixel)
    {
      if (this.Visibility != Visibility.Visible || metersPerPixel <= 0.0)
        return;
      CultureInfo provider = this.Culture != null ? this.Culture : CultureInfo.CurrentUICulture;
      DistanceUnit distanceUnit = this.DistanceUnit;
      if (distanceUnit == DistanceUnit.Default)
        distanceUnit = (this.regionInfo != null ? this.regionInfo : RegionInfo.CurrentRegion).IsMetric ? DistanceUnit.KilometersMeters : DistanceUnit.MilesFeet;
      double maxWidth = this.MaxWidth;
      this.previousMaxWidth = maxWidth;
      if (DistanceUnit.KilometersMeters == distanceUnit)
      {
        double dIn = metersPerPixel * maxWidth;
        if (dIn > 1000.0)
        {
          int num = Scale.LargestNiceNumber(dIn / 1000.0);
          this.SetScaling((int) ((double) (num * 1000) / metersPerPixel), string.Format((IFormatProvider) provider, OverlayResources.Kilometers, (object) num));
        }
        else
        {
          int num = Scale.LargestNiceNumber(dIn);
          this.SetScaling((int) ((double) num / metersPerPixel), string.Format((IFormatProvider) provider, OverlayResources.Meters, (object) num));
        }
      }
      else
      {
        double num1 = metersPerPixel * 3.2808398999999997;
        double dIn = num1 * maxWidth;
        if (dIn > 5280.0)
        {
          int num2 = Scale.LargestNiceNumber(dIn / 5280.0);
          this.SetScaling((int) ((double) (num2 * 5280) / num1), string.Format((IFormatProvider) provider, OverlayResources.Miles, (object) num2));
        }
        else if (DistanceUnit.MilesFeet == distanceUnit)
        {
          int num3 = Scale.LargestNiceNumber(dIn);
          this.SetScaling((int) ((double) num3 / num1), string.Format((IFormatProvider) provider, OverlayResources.Feet, (object) num3));
        }
        else
        {
          int num4 = Scale.LargestNiceNumber(dIn / 3.0);
          this.SetScaling((int) ((double) (num4 * 3) / num1), string.Format((IFormatProvider) provider, OverlayResources.Yards, (object) num4));
        }
      }
      this.currentMetersPerPixel = metersPerPixel;
    }

    private void SetScaling(int pixels, string text)
    {
      this.Width = (double) pixels + this.scaleRectangle.Margin.Left + this.scaleRectangle.Margin.Right;
      this.scaleString.Text = text;
      this.scaleRectangle.Width = (double) pixels;
    }

    private void Refresh()
    {
      if (this.currentMetersPerPixel <= 0.0)
        return;
      this.SetScaling(this.currentMetersPerPixel);
    }

    private void Scale_LayoutUpdated(object sender, EventArgs e)
    {
      if (this.previousMaxWidth == this.MaxWidth)
        return;
      this.Refresh();
    }

    protected virtual void OnPerPixelChanged() => this.SetScaling(this.MetersPerPixel);

    private static void OnUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scale) d).OnUnitChanged();
    }

    protected virtual void OnUnitChanged() => this.Refresh();

    private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Scale) d).OnCultureChanged();
    }

    protected virtual void OnCultureChanged()
    {
      if (this.Culture != null)
      {
        this.regionInfo = (RegionInfo) null;
      }
      else
      {
        this.regionInfo = ResourceUtility.GetRegionInfo(this.Culture.Name);
        OverlayResources.Culture = this.Culture;
      }
      this.Refresh();
    }

    private static int GetSingleDigitValue(double value)
    {
      int num = (int) Math.Floor(value);
      foreach (int singleDigitValue in Scale.singleDigitValues)
      {
        if (num > singleDigitValue)
          return singleDigitValue;
      }
      return 1;
    }

    private static int GetMultiDigitValue(double value, double exponentOf10)
    {
      foreach (double multiDigitValue in Scale.multiDigitValues)
      {
        if (value > multiDigitValue)
          return (int) (multiDigitValue * exponentOf10);
      }
      return (int) exponentOf10;
    }

    private static int LargestNiceNumber(double dIn)
    {
      double exponentOf10 = Math.Pow(10.0, Math.Floor(Math.Log(dIn) / Math.Log(10.0)));
      double num = dIn / exponentOf10;
      return 1.0 == exponentOf10 ? Scale.GetSingleDigitValue(num) : Scale.GetMultiDigitValue(num, exponentOf10);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, nameof (Scale));
    }
  }
}
