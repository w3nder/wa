// Decompiled with JetBrains decompiler
// Type: WhatsApp.ParallaxLongListSelector
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WhatsApp.CompatibilityShims;

#nullable disable
namespace WhatsApp
{
  public class ParallaxLongListSelector : LongListSelector
  {
    private ViewportControl viewport_;
    private double parallaxStartOffset_;
    private bool _enableParallaxScrolling = true;
    private CompositeTransform headerTransform_;
    private bool _viewportEventAttached;

    public ViewportControl Viewport => this.viewport_;

    public double ParallaxStartOffset
    {
      get => this.parallaxStartOffset_;
      set
      {
        this.parallaxStartOffset_ = value;
        this.UpdateHeaderTranslate();
      }
    }

    public bool EnableParallaxScrolling
    {
      get => this._enableParallaxScrolling;
      set
      {
        this._enableParallaxScrolling = value;
        if (!this._enableParallaxScrolling)
        {
          this.headerTransform_.TranslateY = 0.0;
          if (this.viewport_ == null || !this._viewportEventAttached)
            return;
          this.viewport_.ViewportChanged -= new EventHandler<ViewportChangedEventArgs>(this.OnViewportChanged);
          this._viewportEventAttached = false;
        }
        else
        {
          if (this.viewport_ == null || this._viewportEventAttached)
            return;
          this.viewport_.ViewportChanged += new EventHandler<ViewportChangedEventArgs>(this.OnViewportChanged);
          this._viewportEventAttached = true;
        }
      }
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.viewport_ = this.GetTemplateChild("ViewportControl") as ViewportControl;
      if (this.EnableParallaxScrolling && !this._viewportEventAttached && this.viewport_ != null)
      {
        this.viewport_.ViewportChanged += new EventHandler<ViewportChangedEventArgs>(this.OnViewportChanged);
        this._viewportEventAttached = true;
      }
      if (!(this.ListHeader is UIElement listHeader))
        return;
      this.headerTransform_ = listHeader.RenderTransform as CompositeTransform;
    }

    private void UpdateHeaderTranslate()
    {
      if (this.headerTransform_ == null || this.viewport_ == null || this.viewport_.Viewport.Top <= this.ParallaxStartOffset)
        return;
      this.headerTransform_.TranslateY = (this.viewport_.Viewport.Top - this.ParallaxStartOffset) / 3.0;
    }

    private void OnViewportChanged(object sender, ViewportChangedEventArgs args)
    {
      this.UpdateHeaderTranslate();
    }
  }
}
