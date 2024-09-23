// Decompiled with JetBrains decompiler
// Type: WhatsApp.PivotWithPreview
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Info;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace WhatsApp
{
  public class PivotWithPreview : UserControl
  {
    private Grid[] _contents = new Grid[3];
    private Storyboard _slideStoryboard;
    private DoubleAnimation _slideAnimation;
    private Storyboard _scaleStoryboard;
    private DoubleAnimation _scaleAnimation;
    private int _count;
    private int _index = -1;
    private double _lastDeltaX;
    private double _startedX;
    private Stopwatch sw = new Stopwatch();
    internal Grid LayoutRoot;
    internal CompositeTransform RootXForm;
    private bool _contentLoaded;

    public PivotWithPreview()
    {
      this.InitializeComponent();
      int index = 0;
      foreach (FrameworkElement child in (PresentationFrameworkCollection<UIElement>) this.LayoutRoot.Children)
      {
        if (child is Grid)
        {
          this._contents[index] = child as Grid;
          ++index;
        }
      }
      this._slideStoryboard = this.LayoutRoot.Resources[(object) "SlideStoryboard"] as Storyboard;
      this._slideAnimation = this._slideStoryboard.Children[0] as DoubleAnimation;
      this._scaleStoryboard = this.LayoutRoot.Resources[(object) "ScaleStoryboard"] as Storyboard;
      this._scaleAnimation = this._scaleStoryboard.Children[0] as DoubleAnimation;
      this.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.OnManipulationStarted);
      this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.OnManipulationDelta);
      this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.OnManipulationCompleted);
      this.LayoutRoot.Visibility = Visibility.Collapsed;
    }

    public int Count
    {
      get => this._count;
      set
      {
        int count = this._count;
        this._count = value;
        this.LayoutRoot.Visibility = this._count == 0 ? Visibility.Collapsed : Visibility.Visible;
      }
    }

    public int Index
    {
      get => this._index;
      set
      {
        if (!this.IsValidIndex(value))
          throw new IndexOutOfRangeException();
        int index = this._index;
        this._index = value;
        this.FireSelectionChanged(index, value);
        this.Reset();
      }
    }

    public event EventHandler<VisualRequestEventArgs> VisualRequest;

    public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

    private void FireVisualRequest(int index)
    {
      if (!this.IsValidIndex(index))
        return;
      EventHandler<VisualRequestEventArgs> visualRequest = this.VisualRequest;
      if (visualRequest == null)
        return;
      VisualRequestEventArgs e = new VisualRequestEventArgs(index);
      visualRequest((object) this, e);
      if (e.Visual == null)
        return;
      UIElementCollection children = this._contents[index % 3].Children;
      if (children.Count == 1 && children[0] == e.Visual)
        return;
      children.Clear();
      children.Add((UIElement) e.Visual);
    }

    private void FireSelectionChanged(int oldIndex, int newIndex)
    {
      EventHandler<SelectionChangedEventArgs> selectionChanged = this.SelectionChanged;
      if (selectionChanged == null)
        return;
      SelectionChangedEventArgs e = new SelectionChangedEventArgs((IList) new List<int>()
      {
        oldIndex
      }, (IList) new List<int>() { newIndex });
      selectionChanged((object) this, e);
    }

    private bool IsValidIndex(int index) => index >= 0 && index < this.Count;

    private void Reset()
    {
      this.SetPositions();
      this.FireVisualRequest(this.Index - 1);
      this.FireVisualRequest(this.Index);
      this.FireVisualRequest(this.Index + 1);
    }

    private void SetPositions()
    {
      int index1 = this.Index % 3;
      int index2 = (this.Index + 2) % 3;
      int index3 = (this.Index + 1) % 3;
      ((CompositeTransform) this._contents[index2].RenderTransform).TranslateX = -(this.ActualWidth + 24.0);
      ((CompositeTransform) this._contents[index1].RenderTransform).TranslateX = 0.0;
      ((CompositeTransform) this._contents[index3].RenderTransform).TranslateX = this.ActualWidth + 24.0;
      this._contents[index2].Opacity = this.IsValidIndex(this.Index - 1) ? 1.0 : 0.0;
      this._contents[index1].Opacity = 1.0;
      this._contents[index3].Opacity = this.IsValidIndex(this.Index + 1) ? 1.0 : 0.0;
    }

    private void AnimateToCurrent(double velocity)
    {
      if (Math.Abs(this.RootXForm.TranslateX) > 0.0)
      {
        this._slideAnimation.Duration = (Duration) TimeSpan.FromMilliseconds(velocity != 0.0 ? Math.Min(300000.0 / Math.Abs(velocity), 2000.0) : 1000.0);
        this._slideAnimation.From = new double?(this.RootXForm.TranslateX);
        this._slideStoryboard.Seek(TimeSpan.Zero);
        this._slideStoryboard.Begin();
        this.sw.Reset();
        this.sw.Start();
      }
      else
      {
        if (this.RootXForm.ScaleX >= 1.0)
          return;
        this._scaleAnimation.From = new double?(this.RootXForm.ScaleX);
        this._scaleStoryboard.Seek(TimeSpan.Zero);
        this._scaleStoryboard.Begin();
      }
    }

    private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this._scaleStoryboard.Pause();
      if (this._slideStoryboard.GetCurrentState() != ClockState.Stopped)
      {
        this._slideStoryboard.Stop();
        double totalMilliseconds = this._slideStoryboard.Children[0].Duration.TimeSpan.TotalMilliseconds;
        if ((double) this.sw.ElapsedMilliseconds > totalMilliseconds)
        {
          this._startedX = this.RootXForm.TranslateX = 0.0;
          this.sw.Stop();
        }
        else
        {
          double valueOrDefault = ((DoubleAnimation) this._slideStoryboard.Children[0]).From.GetValueOrDefault();
          double num1 = 0.0;
          double num2 = ((double) this.sw.ElapsedMilliseconds + (DeviceStatus.DeviceTotalMemory > 750000000L ? 55.0 : 80.0)) / totalMilliseconds;
          double d = 6.0 * totalMilliseconds / 1000.0;
          double num3 = 1.0 - num2;
          double num4 = 1.0 - (Math.Exp(d * num3) - 1.0) / (Math.Exp(d) - 1.0);
          this._startedX = this.RootXForm.TranslateX = valueOrDefault + (num1 - valueOrDefault) * num4;
          this.sw.Stop();
        }
      }
      e.Handled = true;
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (!this.IsValidIndex(this.Index - 1) && e.CumulativeManipulation.Translation.X > 0.0)
      {
        this.RootXForm.CenterX = this.ActualWidth * 2.0 + 100.0;
        this.RootXForm.ScaleX = (this.ActualWidth - Math.Abs(e.CumulativeManipulation.Translation.X) / 6.0) / this.ActualWidth;
        this.RootXForm.TranslateX = 0.0;
      }
      else if (!this.IsValidIndex(this.Index + 1) && e.CumulativeManipulation.Translation.X < 0.0)
      {
        this.RootXForm.CenterX = -this.ActualWidth - 100.0;
        this.RootXForm.ScaleX = (this.ActualWidth - Math.Abs(e.CumulativeManipulation.Translation.X) / 6.0) / this.ActualWidth;
        this.RootXForm.TranslateX = 0.0;
      }
      else
      {
        this.RootXForm.TranslateX = this._startedX + e.CumulativeManipulation.Translation.X;
        this.RootXForm.ScaleX = 1.0;
      }
      this._lastDeltaX = e.DeltaManipulation.Translation.X;
      e.Handled = true;
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (this.RootXForm.TranslateX < 0.0 && this._lastDeltaX < 0.0)
      {
        ++this._index;
        this.RootXForm.TranslateX = this.ActualWidth + this.RootXForm.TranslateX;
        this.SetPositions();
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.FireSelectionChanged(this._index - 1, this._index);
          this.FireVisualRequest(this.Index + 1);
        }));
      }
      else if (this.RootXForm.TranslateX > 0.0 && this._lastDeltaX > 0.0)
      {
        --this._index;
        this.RootXForm.TranslateX = -this.ActualWidth + this.RootXForm.TranslateX;
        this.SetPositions();
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.FireSelectionChanged(this._index + 1, this._index);
          this.FireVisualRequest(this.Index - 1);
        }));
      }
      this.AnimateToCurrent(e.IsInertial ? e.FinalVelocities.LinearVelocity.X : 0.0);
      e.Handled = true;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/PivotWithPreview.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.RootXForm = (CompositeTransform) this.FindName("RootXForm");
    }
  }
}
