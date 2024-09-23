// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ProgressOverlay
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  [System.Windows.Markup.ContentProperty("Content")]
  public class ProgressOverlay : Control
  {
    private const string FadeInName = "fadeIn";
    private const string FadeOutName = "fadeOut";
    private const string LayoutGridName = "LayoutGrid";
    private Storyboard _fadeIn;
    private Storyboard _fadeOut;
    private Grid _layoutGrid;
    public static readonly DependencyProperty ProgressControlProperty = DependencyProperty.Register(nameof (ProgressControl), typeof (object), typeof (ProgressOverlay), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof (Content), typeof (object), typeof (ProgressOverlay), new PropertyMetadata((PropertyChangedCallback) null));

    public ProgressOverlay() => this.DefaultStyleKey = (object) typeof (ProgressOverlay);

    public object ProgressControl
    {
      get => ((DependencyObject) this).GetValue(ProgressOverlay.ProgressControlProperty);
      set => ((DependencyObject) this).SetValue(ProgressOverlay.ProgressControlProperty, value);
    }

    public object Content
    {
      get => ((DependencyObject) this).GetValue(ProgressOverlay.ContentProperty);
      set => ((DependencyObject) this).SetValue(ProgressOverlay.ContentProperty, value);
    }

    public virtual void OnApplyTemplate()
    {
      ((FrameworkElement) this).OnApplyTemplate();
      this._fadeIn = this.GetTemplateChild("fadeIn") as Storyboard;
      this._fadeOut = this.GetTemplateChild("fadeOut") as Storyboard;
      this._layoutGrid = this.GetTemplateChild("LayoutGrid") as Grid;
      if (this._fadeOut == null)
        return;
      ((Timeline) this._fadeOut).Completed += new EventHandler(this.fadeOut_Completed);
    }

    private void fadeOut_Completed(object sender, EventArgs e)
    {
      ((UIElement) this._layoutGrid).Opacity = 1.0;
      ((UIElement) this).Visibility = (Visibility) 1;
    }

    public void Show()
    {
      if (this._fadeIn == null)
        this.ApplyTemplate();
      ((UIElement) this).Visibility = (Visibility) 0;
      if (this._fadeOut != null)
        this._fadeOut.Stop();
      if (this._fadeIn == null)
        return;
      this._fadeIn.Begin();
    }

    public void Hide()
    {
      if (this._fadeOut == null)
        this.ApplyTemplate();
      if (this._fadeIn != null)
        this._fadeIn.Stop();
      if (this._fadeOut == null)
        return;
      this._fadeOut.Begin();
    }
  }
}
