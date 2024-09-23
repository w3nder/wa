// Decompiled with JetBrains decompiler
// Type: WhatsApp.PlaybackSlider
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class PlaybackSlider : UserControl
  {
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (double), typeof (PlaybackSlider), new PropertyMetadata((object) 0.0, (PropertyChangedCallback) ((dep, e) => (dep as PlaybackSlider).OnValueChanged((double) e.NewValue))));
    private double valueCopy_;
    private double dragStartPos_;
    private double dragStartValue_;
    private double minimum_;
    private double maximum_ = 100.0;
    private double minimumDelta_ = 1.0;
    private double MinProgressFillLength = 8.0;
    private bool seeking_;
    private bool canSeek_;
    internal Grid LayoutRoot;
    internal Rectangle ProgressTrack;
    internal Rectangle ProgressFill;
    internal Ellipse Scrubber;
    private bool _contentLoaded;

    public double Value
    {
      get => this.valueCopy_;
      set
      {
        this.SetValue(value, false);
        this.SetValue(PlaybackSlider.ValueProperty, (object) value);
      }
    }

    public double Minimum
    {
      get => this.minimum_;
      set => this.minimum_ = Math.Max(0.0, value);
    }

    public double Maximum
    {
      get => this.maximum_;
      set
      {
        this.maximum_ = Math.Max(0.0, value);
        this.minimumDelta_ = this.maximum_ / 100.0;
      }
    }

    public double MinimumDelta => this.minimumDelta_;

    private double MaxProgressFillLength => this.ActualWidth - 8.0;

    public bool CanSeek
    {
      get => this.canSeek_;
      set
      {
        if (this.canSeek_ == value)
          return;
        this.canSeek_ = value;
        this.Scrubber.Visibility = this.canSeek_.ToVisibility();
      }
    }

    public event EventHandler SeekStarted;

    public event EventHandler<PlaybackSlider.PlaybackSliderSeekArgs> SeekCompleted;

    protected void NotifySeekStarted()
    {
      if (this.SeekStarted == null)
        return;
      this.SeekStarted((object) this, new EventArgs());
    }

    protected void NotifySeekCompleted(double? seekedVal)
    {
      if (this.SeekStarted == null)
        return;
      this.SeekCompleted((object) this, new PlaybackSlider.PlaybackSliderSeekArgs(seekedVal));
    }

    public PlaybackSlider()
    {
      this.InitializeComponent();
      this.CanSeek = false;
    }

    private void SetValue(double newVal, bool notify)
    {
      this.valueCopy_ = newVal;
      if (!notify)
        return;
      this.OnValueChanged(newVal);
    }

    private double ValueToProgressLength(double value)
    {
      return Math.Max(this.MinProgressFillLength, Math.Min((value - this.Minimum) / (this.Maximum - this.Minimum) * this.ActualWidth, this.MaxProgressFillLength));
    }

    private double ProgressLengthToValue(double len)
    {
      return len / this.ActualWidth * (this.Maximum - this.Minimum);
    }

    private void Slider_DragStarted(object sender, DragStartedGestureEventArgs e)
    {
      if (!this.CanSeek)
        return;
      this.seeking_ = true;
      this.dragStartPos_ = Math.Min(Math.Max(this.MinProgressFillLength, e.GetPosition((UIElement) this.LayoutRoot).X), this.MaxProgressFillLength);
      this.dragStartValue_ = this.Value;
      this.NotifySeekStarted();
    }

    private void Slider_DragDelta(object sender, DragDeltaGestureEventArgs e)
    {
      if (!this.CanSeek && !this.seeking_)
        return;
      this.SetValue(this.dragStartValue_ + this.ProgressLengthToValue(e.GetPosition((UIElement) this.LayoutRoot).X - this.dragStartPos_), true);
    }

    private void Slider_DragCompleted(object sender, DragCompletedGestureEventArgs e)
    {
      if (!this.CanSeek && !this.seeking_)
        return;
      this.seeking_ = false;
      if (this.Value > this.Minimum + this.MinimumDelta && this.Value < this.Maximum - this.MinimumDelta)
      {
        this.NotifySeekCompleted(new double?(this.Value));
      }
      else
      {
        this.SetValue(this.Minimum, true);
        this.NotifySeekCompleted(new double?());
      }
    }

    private void OnValueChanged(double newVal)
    {
      if (newVal < this.Minimum)
        newVal = this.Minimum;
      else if (newVal > this.Maximum)
        newVal = this.Maximum;
      if (newVal > this.Minimum + this.MinimumDelta || this.seeking_)
      {
        this.CanSeek = true;
        this.ProgressFill.Width = this.ValueToProgressLength(newVal);
      }
      else
      {
        this.CanSeek = false;
        this.ProgressFill.Width = 0.0;
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/PlaybackSlider.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ProgressTrack = (Rectangle) this.FindName("ProgressTrack");
      this.ProgressFill = (Rectangle) this.FindName("ProgressFill");
      this.Scrubber = (Ellipse) this.FindName("Scrubber");
    }

    public class PlaybackSliderSeekArgs : EventArgs
    {
      public double? SeekedValue { get; private set; }

      public PlaybackSliderSeekArgs(double? seekedVal) => this.SeekedValue = seekedVal;
    }
  }
}
