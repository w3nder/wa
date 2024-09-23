// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Primitives.RelativeAnimatingContentControl
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls.Primitives
{
  public class RelativeAnimatingContentControl : ContentControl
  {
    private const double SimpleDoubleComparisonEpsilon = 9E-06;
    private double _knownWidth;
    private double _knownHeight;
    private List<RelativeAnimatingContentControl.AnimationValueAdapter> _specialAnimations;

    public RelativeAnimatingContentControl()
    {
      this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (e == null || e.NewSize.Height <= 0.0 || e.NewSize.Width <= 0.0)
        return;
      this._knownWidth = e.NewSize.Width;
      this._knownHeight = e.NewSize.Height;
      this.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(0.0, 0.0, this._knownWidth, this._knownHeight)
      };
      this.UpdateAnyAnimationValues();
    }

    private void UpdateAnyAnimationValues()
    {
      if (this._knownHeight <= 0.0 || this._knownWidth <= 0.0)
        return;
      if (this._specialAnimations == null)
      {
        this._specialAnimations = new List<RelativeAnimatingContentControl.AnimationValueAdapter>();
        foreach (VisualStateGroup visualStateGroup in (IEnumerable) VisualStateManager.GetVisualStateGroups((FrameworkElement) this))
        {
          if (visualStateGroup != null)
          {
            foreach (VisualState state in (IEnumerable) visualStateGroup.States)
            {
              if (state != null)
              {
                Storyboard storyboard = state.Storyboard;
                if (storyboard != null)
                {
                  foreach (Timeline child in (PresentationFrameworkCollection<Timeline>) storyboard.Children)
                  {
                    DoubleAnimation da1 = child as DoubleAnimation;
                    DoubleAnimationUsingKeyFrames da2 = child as DoubleAnimationUsingKeyFrames;
                    if (da1 != null)
                      this.ProcessDoubleAnimation(da1);
                    else if (da2 != null)
                      this.ProcessDoubleAnimationWithKeys(da2);
                  }
                }
              }
            }
          }
        }
      }
      this.UpdateKnownAnimations();
    }

    private void UpdateKnownAnimations()
    {
      foreach (RelativeAnimatingContentControl.AnimationValueAdapter specialAnimation in this._specialAnimations)
        specialAnimation.UpdateWithNewDimension(this._knownWidth, this._knownHeight);
    }

    private void ProcessDoubleAnimationWithKeys(DoubleAnimationUsingKeyFrames da)
    {
      foreach (DoubleKeyFrame keyFrame in (PresentationFrameworkCollection<DoubleKeyFrame>) da.KeyFrames)
      {
        RelativeAnimatingContentControl.DoubleAnimationDimension? dimensionFromMagicNumber = RelativeAnimatingContentControl.GeneralAnimationValueAdapter<DoubleKeyFrame>.GetDimensionFromMagicNumber(keyFrame.Value);
        if (dimensionFromMagicNumber.HasValue)
          this._specialAnimations.Add((RelativeAnimatingContentControl.AnimationValueAdapter) new RelativeAnimatingContentControl.DoubleAnimationFrameAdapter(dimensionFromMagicNumber.Value, keyFrame));
      }
    }

    private void ProcessDoubleAnimation(DoubleAnimation da)
    {
      if (da.To.HasValue)
      {
        RelativeAnimatingContentControl.DoubleAnimationDimension? dimensionFromMagicNumber = RelativeAnimatingContentControl.GeneralAnimationValueAdapter<DoubleAnimation>.GetDimensionFromMagicNumber(da.To.Value);
        if (dimensionFromMagicNumber.HasValue)
          this._specialAnimations.Add((RelativeAnimatingContentControl.AnimationValueAdapter) new RelativeAnimatingContentControl.DoubleAnimationToAdapter(dimensionFromMagicNumber.Value, da));
      }
      if (!da.From.HasValue)
        return;
      RelativeAnimatingContentControl.DoubleAnimationDimension? dimensionFromMagicNumber1 = RelativeAnimatingContentControl.GeneralAnimationValueAdapter<DoubleAnimation>.GetDimensionFromMagicNumber(da.To.Value);
      if (!dimensionFromMagicNumber1.HasValue)
        return;
      this._specialAnimations.Add((RelativeAnimatingContentControl.AnimationValueAdapter) new RelativeAnimatingContentControl.DoubleAnimationFromAdapter(dimensionFromMagicNumber1.Value, da));
    }

    private enum DoubleAnimationDimension
    {
      Width,
      Height,
    }

    private abstract class AnimationValueAdapter
    {
      public AnimationValueAdapter(
        RelativeAnimatingContentControl.DoubleAnimationDimension dimension)
      {
        this.Dimension = dimension;
      }

      public RelativeAnimatingContentControl.DoubleAnimationDimension Dimension { get; private set; }

      public abstract void UpdateWithNewDimension(double width, double height);
    }

    private abstract class GeneralAnimationValueAdapter<T> : 
      RelativeAnimatingContentControl.AnimationValueAdapter
    {
      private double _ratio;

      protected T Instance { get; set; }

      protected abstract double GetValue();

      protected abstract void SetValue(double newValue);

      protected double InitialValue { get; private set; }

      public GeneralAnimationValueAdapter(
        RelativeAnimatingContentControl.DoubleAnimationDimension d,
        T instance)
        : base(d)
      {
        this.Instance = instance;
        this.InitialValue = this.StripMagicNumberOff(this.GetValue());
        this._ratio = this.InitialValue / 100.0;
      }

      public double StripMagicNumberOff(double number)
      {
        return this.Dimension != RelativeAnimatingContentControl.DoubleAnimationDimension.Width ? number - 0.2 : number - 0.1;
      }

      public static RelativeAnimatingContentControl.DoubleAnimationDimension? GetDimensionFromMagicNumber(
        double number)
      {
        double num1 = Math.Round(number);
        double num2 = Math.Abs(number - num1);
        if (num2 >= 0.09999100000000001 && num2 <= 0.100009)
          return new RelativeAnimatingContentControl.DoubleAnimationDimension?(RelativeAnimatingContentControl.DoubleAnimationDimension.Width);
        return num2 >= 0.199991 && num2 <= 0.20000900000000002 ? new RelativeAnimatingContentControl.DoubleAnimationDimension?(RelativeAnimatingContentControl.DoubleAnimationDimension.Height) : new RelativeAnimatingContentControl.DoubleAnimationDimension?();
      }

      public override void UpdateWithNewDimension(double width, double height)
      {
        this.UpdateValue(this.Dimension == RelativeAnimatingContentControl.DoubleAnimationDimension.Width ? width : height);
      }

      private void UpdateValue(double sizeToUse) => this.SetValue(sizeToUse * this._ratio);
    }

    private class DoubleAnimationToAdapter : 
      RelativeAnimatingContentControl.GeneralAnimationValueAdapter<DoubleAnimation>
    {
      protected override double GetValue() => this.Instance.To.Value;

      protected override void SetValue(double newValue) => this.Instance.To = new double?(newValue);

      public DoubleAnimationToAdapter(
        RelativeAnimatingContentControl.DoubleAnimationDimension dimension,
        DoubleAnimation instance)
        : base(dimension, instance)
      {
      }
    }

    private class DoubleAnimationFromAdapter : 
      RelativeAnimatingContentControl.GeneralAnimationValueAdapter<DoubleAnimation>
    {
      protected override double GetValue() => this.Instance.From.Value;

      protected override void SetValue(double newValue)
      {
        this.Instance.From = new double?(newValue);
      }

      public DoubleAnimationFromAdapter(
        RelativeAnimatingContentControl.DoubleAnimationDimension dimension,
        DoubleAnimation instance)
        : base(dimension, instance)
      {
      }
    }

    private class DoubleAnimationFrameAdapter : 
      RelativeAnimatingContentControl.GeneralAnimationValueAdapter<DoubleKeyFrame>
    {
      protected override double GetValue() => this.Instance.Value;

      protected override void SetValue(double newValue) => this.Instance.Value = newValue;

      public DoubleAnimationFrameAdapter(
        RelativeAnimatingContentControl.DoubleAnimationDimension dimension,
        DoubleKeyFrame frame)
        : base(dimension, frame)
      {
      }
    }
  }
}
