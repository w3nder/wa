// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.PopUp`2
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using Clarity.Phone.Extensions;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public abstract class PopUp<T, TPopUpResult> : Control
  {
    private DialogService _popUp;
    private PhoneApplicationPage _startingPage;
    private bool _alreadyFired;
    private bool _isCalculateFrameVerticalOffset;
    private static readonly DependencyProperty FrameTransformProperty = DependencyProperty.Register(nameof (FrameTransform), typeof (double), typeof (PopUp<T, TPopUpResult>), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(PopUp<T, TPopUpResult>.OnFrameTransformPropertyChanged)));
    public static readonly DependencyProperty OverlayProperty = DependencyProperty.Register(nameof (Overlay), typeof (Brush), typeof (PopUp<T, TPopUpResult>), new PropertyMetadata(!DesignerProperties.IsInDesignTool ? Application.Current.Resources[(object) "PhoneSemitransparentBrush"] : (object) null));

    public bool IsOpen => this._popUp != null && this._popUp.IsOpen;

    public bool IsAppBarVisible { get; set; }

    protected bool IsCalculateFrameVerticalOffset
    {
      get => this._isCalculateFrameVerticalOffset;
      set
      {
        this._isCalculateFrameVerticalOffset = value;
        if (!this._isCalculateFrameVerticalOffset)
          return;
        Binding binding = new Binding("Y");
        if (!(Application.Current.RootVisual is Frame rootVisual) || !(rootVisual.RenderTransform is TransformGroup renderTransform))
          return;
        binding.Source = (object) ((IEnumerable<Transform>) renderTransform.Children).FirstOrDefault<Transform>((Func<Transform, bool>) (t => t is TranslateTransform));
        ((FrameworkElement) this).SetBinding(PopUp<T, TPopUpResult>.FrameTransformProperty, binding);
      }
    }

    internal IApplicationBar AppBar { get; set; }

    protected internal bool IsBackKeyOverride { get; set; }

    protected DialogService.AnimationTypes AnimationType { get; set; }

    public event EventHandler<PopUpEventArgs<T, TPopUpResult>> Completed;

    public event EventHandler Opened;

    public virtual void OnApplyTemplate()
    {
      ((FrameworkElement) this).OnApplyTemplate();
      if (this._popUp == null)
        return;
      this._popUp.SetAlignmentsOnOverlay(((FrameworkElement) this).HorizontalAlignment, ((FrameworkElement) this).VerticalAlignment);
    }

    public virtual void OnCompleted(PopUpEventArgs<T, TPopUpResult> result)
    {
      this._alreadyFired = true;
      if (this.Completed != null)
        this.Completed((object) this, result);
      if (this._popUp != null)
        this._popUp.Hide();
      this.ResetWorldAndDestroyPopUp();
    }

    private static void OnFrameTransformPropertyChanged(
      DependencyObject source,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(source is PopUp<T, TPopUpResult> popUp) || popUp._popUp == null || !popUp.IsCalculateFrameVerticalOffset)
        return;
      popUp._popUp.ControlVerticalOffset = -popUp.FrameTransform;
      popUp._popUp.CalculateVerticalOffset();
    }

    public virtual void Show()
    {
      this._popUp = new DialogService()
      {
        AnimationType = this.AnimationType,
        Child = (FrameworkElement) this,
        BackgroundBrush = this.Overlay,
        IsBackKeyOverride = this.IsBackKeyOverride
      };
      if (this.IsCalculateFrameVerticalOffset)
        this._popUp.ControlVerticalOffset = -this.FrameTransform;
      this._popUp.Closed += new EventHandler(this.PopUpClosed);
      this._popUp.Opened += new EventHandler(this.PopUpOpened);
      ((DependencyObject) this).Dispatcher.BeginInvoke((Action) (() =>
      {
        if (!this.IsAppBarVisible)
        {
          this.AppBar = this._popUp.Page.ApplicationBar;
          this._popUp.Page.ApplicationBar = (IApplicationBar) null;
        }
        this._startingPage = this._popUp.Page;
        this._popUp.Show();
      }));
    }

    private void PopUpOpened(object sender, EventArgs e)
    {
      if (this.Opened == null)
        return;
      this.Opened(sender, e);
    }

    protected virtual TPopUpResult GetOnClosedValue() => default (TPopUpResult);

    public void Hide() => this.PopUpClosed((object) this, (EventArgs) null);

    private void PopUpClosed(object sender, EventArgs e)
    {
      if (!this._alreadyFired)
        this.OnCompleted(new PopUpEventArgs<T, TPopUpResult>()
        {
          PopUpResult = this.GetOnClosedValue()
        });
      else
        this.ResetWorldAndDestroyPopUp();
    }

    private void ResetWorldAndDestroyPopUp()
    {
      if (this._popUp == null)
        return;
      if (!this.IsAppBarVisible && this.AppBar != null && this._startingPage != null)
        this._startingPage.ApplicationBar = this.AppBar;
      this._startingPage = (PhoneApplicationPage) null;
      this._popUp.Child = (FrameworkElement) null;
      this._popUp = (DialogService) null;
    }

    private double FrameTransform
    {
      get
      {
        return (double) ((DependencyObject) this).GetValue(PopUp<T, TPopUpResult>.FrameTransformProperty);
      }
      set
      {
        ((DependencyObject) this).SetValue(PopUp<T, TPopUpResult>.FrameTransformProperty, (object) value);
      }
    }

    public Brush Overlay
    {
      get => (Brush) ((DependencyObject) this).GetValue(PopUp<T, TPopUpResult>.OverlayProperty);
      set
      {
        ((DependencyObject) this).SetValue(PopUp<T, TPopUpResult>.OverlayProperty, (object) value);
      }
    }
  }
}
