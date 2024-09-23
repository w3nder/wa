// Decompiled with JetBrains decompiler
// Type: WhatsApp.StatusViewProgressBar
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


namespace WhatsApp
{
  public class StatusViewProgressBar : Grid
  {
    private const int MaxSegmentsPerRow = 25;
    private const int GapPixels = 4;
    private int segmentCount = -1;
    private int currIndex = -1;
    private Storyboard progressSb;
    private DoubleAnimation progressAnimation;

    public StatusViewProgressBar(int segments)
    {
      this.segmentCount = segments;
      this.progressAnimation = new DoubleAnimation()
      {
        To = new double?(0.0)
      };
      Storyboard.SetTargetProperty((Timeline) this.progressAnimation, new PropertyPath("X", new object[0]));
      this.progressSb = new Storyboard();
      this.progressSb.Completed += new EventHandler(this.OnProgressAnimationCompleted);
      this.progressSb.Children.Add((Timeline) this.progressAnimation);
      double height = 4.0 * ResolutionHelper.ZoomMultiplier;
      if (segments > 25)
      {
        int num = segments / 25 + 1;
        for (int index = 1; index < num; ++index)
        {
          this.RowDefinitions.Add(new RowDefinition()
          {
            Height = new GridLength(height, GridUnitType.Pixel)
          });
          this.RowDefinitions.Add(new RowDefinition()
          {
            Height = new GridLength(4.0 * ResolutionHelper.ZoomMultiplier, GridUnitType.Pixel)
          });
        }
        this.RowDefinitions.Add(new RowDefinition()
        {
          Height = new GridLength(height, GridUnitType.Pixel)
        });
      }
      int num1 = Math.Min(segments, 25);
      int num2 = (int) (4.0 * ResolutionHelper.ZoomMultiplier + 0.5);
      for (int index = 1; index < num1; ++index)
      {
        this.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Star)
        });
        this.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength((double) num2, GridUnitType.Pixel)
        });
      }
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      for (int index = 0; index < segments; ++index)
      {
        SegmentBar element = new SegmentBar(height)
        {
          Background = UIUtils.InactiveBrush
        };
        Grid.SetRow((FrameworkElement) element, index / num1 * 2);
        Grid.SetColumn((FrameworkElement) element, index % num1 * 2);
        this.Children.Add((UIElement) element);
      }
    }

    public void FillTill(int i)
    {
      this.progressSb.Stop();
      if (i < 0 || i >= this.Children.Count)
        return;
      for (int index = 0; index < i; ++index)
      {
        if (this.Children[index] is SegmentBar child)
          child.Fill();
      }
      int count = this.Children.Count;
      for (int index = i; index < count; ++index)
      {
        if (this.Children[index] is SegmentBar child)
          child.Clear();
      }
    }

    public void Begin(int i, int durationInSecs)
    {
      this.FillTill(i);
      if (this.Children[i] is SegmentBar child)
        child.Animate(true, (Duration) TimeSpan.FromSeconds((double) durationInSecs), this.progressSb, this.progressAnimation);
      this.currIndex = i;
    }

    public void Pause() => this.progressSb.Pause();

    public void Resume() => this.progressSb.Resume();

    public void Stop() => this.progressSb.Stop();

    private void OnProgressAnimationCompleted(object sender, EventArgs e)
    {
      if (!(this.Children.ElementAtOrDefault<UIElement>(this.currIndex) is SegmentBar segmentBar))
        return;
      segmentBar.Fill();
      this.progressSb.Stop();
    }
  }
}
