// Decompiled with JetBrains decompiler
// Type: WhatsApp.WallpaperPreviewControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace WhatsApp
{
  public class WallpaperPreviewControl : UserControl
  {
    internal Grid LayoutRoot;
    internal CompositeTransform RootXForm;
    internal WallpaperPanel WallpaperPanel;
    internal WallpaperPreviewForeground PreviewForeground;
    private bool _contentLoaded;

    public WallpaperPreviewControl()
    {
      this.InitializeComponent();
      this.PreviewForeground.Margin = new Thickness(0.0, UIUtils.SystemTraySizePortrait, 0.0, 0.0);
      this.PreviewForeground.SetTexts(AppResources.ChooseWallpaper, AppResources.SwipeWallpaperLeft, AppResources.SwipeWallpaperRight);
    }

    public void Set(WallpaperStore.WallpaperState wallpaper, System.Windows.Media.ImageSource imgSrc = null)
    {
      this.WallpaperPanel.Set(wallpaper, imgSrc);
      this.PreviewForeground.Update(wallpaper);
    }

    public void AnimateIn(System.Windows.Point position, Size renderSize, Action onCompleted)
    {
      Storyboard s = this.Resources[(object) "ExpandCollapseStoryboard"] as Storyboard;
      s.Stop();
      DoubleAnimation child1 = s.Children[0] as DoubleAnimation;
      child1.From = new double?(renderSize.Width / this.ActualWidth);
      child1.To = new double?(1.0);
      this.RootXForm.ScaleX = renderSize.Width / this.ActualWidth;
      DoubleAnimation child2 = s.Children[1] as DoubleAnimation;
      child2.From = new double?(renderSize.Height / this.ActualHeight);
      child2.To = new double?(1.0);
      this.RootXForm.ScaleY = renderSize.Height / this.ActualHeight;
      DoubleAnimation child3 = s.Children[2] as DoubleAnimation;
      child3.From = new double?(position.X);
      child3.To = new double?(0.0);
      this.RootXForm.TranslateX = position.X;
      DoubleAnimation child4 = s.Children[3] as DoubleAnimation;
      child4.From = new double?(position.Y);
      child4.To = new double?(0.0);
      this.RootXForm.TranslateY = position.Y;
      Storyboard s2 = this.Resources[(object) "MockOverlayStoryboard"] as Storyboard;
      s2.Stop();
      DoubleAnimation child5 = s2.Children[0] as DoubleAnimation;
      Storyboard.SetTarget((Timeline) child5, (DependencyObject) this.PreviewForeground.TitleBlock.RenderTransform);
      child5.From = new double?(-50.0);
      child5.To = new double?(0.0);
      ((CompositeTransform) this.PreviewForeground.TitleBlock.RenderTransform).TranslateY = -50.0;
      DoubleAnimation child6 = s2.Children[1] as DoubleAnimation;
      Storyboard.SetTarget((Timeline) child6, (DependencyObject) this.PreviewForeground.MessagesPanel.RenderTransform);
      child6.From = new double?(100.0);
      child6.To = new double?(0.0);
      ((CompositeTransform) this.PreviewForeground.MessagesPanel.RenderTransform).TranslateY = 100.0;
      DoubleAnimation child7 = s2.Children[2] as DoubleAnimation;
      child7.From = new double?(0.0);
      child7.To = new double?(1.0);
      child7.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(200.0));
      this.PreviewForeground.Opacity = 0.0;
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        Storyboarder.Perform(s, onComplete: this.GetExpandCollapsedCompleted(onCompleted));
        Storyboarder.Perform(s2, onComplete: this.GetMockOverlayCompleted());
      }));
    }

    public void AnimateOut(System.Windows.Point position, Size renderSize, Action onCompleted)
    {
      Storyboard resource1 = this.Resources[(object) "ExpandCollapseStoryboard"] as Storyboard;
      resource1.Stop();
      DoubleAnimation child1 = resource1.Children[0] as DoubleAnimation;
      child1.To = new double?(renderSize.Width / this.ActualWidth);
      child1.From = new double?(1.0);
      DoubleAnimation child2 = resource1.Children[1] as DoubleAnimation;
      child2.To = new double?(renderSize.Height / this.ActualHeight);
      child2.From = new double?(1.0);
      DoubleAnimation child3 = resource1.Children[2] as DoubleAnimation;
      child3.To = new double?(position.X);
      child3.From = new double?(0.0);
      DoubleAnimation child4 = resource1.Children[3] as DoubleAnimation;
      child4.To = new double?(position.Y);
      child4.From = new double?(0.0);
      Storyboard resource2 = this.Resources[(object) "MockOverlayStoryboard"] as Storyboard;
      resource2.Stop();
      DoubleAnimation child5 = resource2.Children[0] as DoubleAnimation;
      child5.From = new double?(0.0);
      child5.To = new double?(-50.0);
      Storyboard.SetTarget((Timeline) child5, (DependencyObject) this.PreviewForeground.TitleBlock.RenderTransform);
      DoubleAnimation child6 = resource2.Children[1] as DoubleAnimation;
      child6.From = new double?(0.0);
      child6.To = new double?(100.0);
      Storyboard.SetTarget((Timeline) child6, (DependencyObject) this.PreviewForeground.MessagesPanel.RenderTransform);
      DoubleAnimation child7 = resource2.Children[2] as DoubleAnimation;
      child7.From = new double?(1.0);
      child7.To = new double?(0.0);
      child7.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(0.0));
      Storyboarder.Perform(resource1, onComplete: this.GetExpandCollapsedCompleted(onCompleted));
      Storyboarder.Perform(resource2, onComplete: this.GetMockOverlayCompleted());
    }

    private Action GetExpandCollapsedCompleted(Action onCompleted)
    {
      return (Action) (() =>
      {
        this.RootXForm.ScaleX = 1.0;
        this.RootXForm.ScaleY = 1.0;
        this.RootXForm.TranslateX = 0.0;
        this.RootXForm.TranslateY = 0.0;
        ((CompositeTransform) this.PreviewForeground.TitleBlock.RenderTransform).TranslateY = 0.0;
        ((CompositeTransform) this.PreviewForeground.MessagesPanel.RenderTransform).TranslateY = 0.0;
        this.PreviewForeground.Opacity = 1.0;
        if (onCompleted == null)
          return;
        onCompleted();
      });
    }

    private Action GetMockOverlayCompleted()
    {
      return (Action) (() =>
      {
        ((CompositeTransform) this.PreviewForeground.TitleBlock.RenderTransform).TranslateY = 0.0;
        ((CompositeTransform) this.PreviewForeground.MessagesPanel.RenderTransform).TranslateY = 0.0;
        this.PreviewForeground.Opacity = 1.0;
      });
    }

    private static void LogHash(string imagePath)
    {
      try
      {
        using (Stream inputStream = App.OpenFromXAP(imagePath))
        {
          using (SHA1Managed shA1Managed = new SHA1Managed())
            Log.WriteLineDebug("File name = [{0}], hash = [{1}]", (object) imagePath, (object) shA1Managed.ComputeHash(inputStream).ToHexString());
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "failed to hash file");
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/WallpaperPreviewControl.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.RootXForm = (CompositeTransform) this.FindName("RootXForm");
      this.WallpaperPanel = (WallpaperPanel) this.FindName("WallpaperPanel");
      this.PreviewForeground = (WallpaperPreviewForeground) this.FindName("PreviewForeground");
    }
  }
}
