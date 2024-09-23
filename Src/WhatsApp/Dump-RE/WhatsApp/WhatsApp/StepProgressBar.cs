// Decompiled with JetBrains decompiler
// Type: WhatsApp.StepProgressBar
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace WhatsApp
{
  public class StepProgressBar : Grid
  {
    private int gapWidth = 4;
    private int totalSteps = 1;
    private int animatingIndex;
    private bool animatingForward = true;
    private Storyboard progressSb;
    private DoubleAnimation progressAnimation;
    private Subject<int> progressAnimationEndedSubj;

    public int GapWidth
    {
      get => this.gapWidth;
      set
      {
        if (this.gapWidth == value)
          return;
        this.gapWidth = value;
        this.CreateSegments(this.totalSteps, this.gapWidth);
      }
    }

    public int TotalSteps
    {
      get => this.totalSteps;
      set
      {
        if (this.totalSteps == value)
          return;
        this.totalSteps = value;
        this.CreateSegments(this.totalSteps, this.gapWidth);
      }
    }

    public StepProgressBar()
    {
      this.progressAnimation = new DoubleAnimation()
      {
        To = new double?(0.0)
      };
      Storyboard.SetTargetProperty((Timeline) this.progressAnimation, new PropertyPath("X", new object[0]));
      this.progressSb = new Storyboard();
      this.progressSb.Children.Add((Timeline) this.progressAnimation);
      this.progressSb.Completed += new EventHandler(this.OnProgressAnimationCompleted);
    }

    public IObservable<int> GetProgressAnimationEndedObservable()
    {
      return (IObservable<int>) this.progressAnimationEndedSubj ?? (IObservable<int>) (this.progressAnimationEndedSubj = new Subject<int>());
    }

    public void Setup(int totalSteps, int gapWidth)
    {
      this.totalSteps = totalSteps;
      this.gapWidth = gapWidth;
      this.CreateSegments(totalSteps, gapWidth);
    }

    private void CreateSegments(int n, int gap)
    {
      this.ColumnDefinitions.Clear();
      if (n <= 0)
        return;
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      gap = Math.Max(gap, 0);
      for (int index = 1; index < n; ++index)
      {
        if (gap > 0)
          this.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength((double) gap, GridUnitType.Pixel)
          });
        this.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Star)
        });
      }
      for (int index = 0; index < n; ++index)
      {
        SegmentBar segmentBar = new SegmentBar(4.0);
        segmentBar.Height = 4.0;
        segmentBar.Foreground = (Brush) UIUtils.AccentBrush;
        SegmentBar element = segmentBar;
        Grid.SetColumn((FrameworkElement) element, index * 2);
        this.Children.Add((UIElement) element);
      }
    }

    public void FillTill(int i)
    {
      this.progressSb.Stop();
      int count = this.Children.Count;
      if (i < 0 || i >= count)
        return;
      for (int index = 0; index < i; ++index)
      {
        if (this.Children[index] is SegmentBar child)
          child.Fill();
      }
      for (int index = i; index < count; ++index)
      {
        if (this.Children[index] is SegmentBar child)
          child.Clear();
      }
    }

    public void Backward(int i, int durationInMs)
    {
      this.FillTill(i + 1);
      if (this.Children[i] is SegmentBar child)
        child.Animate(false, (Duration) TimeSpan.FromMilliseconds((double) durationInMs), this.progressSb, this.progressAnimation);
      this.animatingIndex = i;
      this.animatingForward = false;
    }

    public void Forward(int i, int durationInMs)
    {
      this.FillTill(i);
      if (this.Children[i] is SegmentBar child)
        child.Animate(true, (Duration) TimeSpan.FromMilliseconds((double) durationInMs), this.progressSb, this.progressAnimation);
      this.animatingIndex = i;
      this.animatingForward = true;
    }

    private void OnProgressAnimationCompleted(object sender, EventArgs e)
    {
      int animatingIndex = this.animatingIndex;
      this.animatingIndex = -1;
      this.progressSb.Stop();
      SegmentBar segmentBar = this.Children.ElementAtOrDefault<UIElement>(animatingIndex) as SegmentBar;
      if (this.animatingForward)
        segmentBar?.Fill();
      else
        segmentBar?.Clear();
      this.progressAnimationEndedSubj?.OnNext(animatingIndex);
    }
  }
}
