// Decompiled with JetBrains decompiler
// Type: WhatsApp.InAppFloatingBanner
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;


namespace WhatsApp
{
  public class InAppFloatingBanner
  {
    private static LinkedList<InAppFloatingBanner> visibleInstances;
    private Popup popup;
    protected IInAppFloatingBannerView view;
    private IDisposable sysTrayHideSub;
    protected Action onComplete;
    protected Action onClick;

    private static LinkedList<InAppFloatingBanner> VisibleInstances
    {
      get
      {
        return InAppFloatingBanner.visibleInstances ?? (InAppFloatingBanner.visibleInstances = new LinkedList<InAppFloatingBanner>());
      }
    }

    public static bool IsShowing => InAppFloatingBanner.VisibleInstances.Count > 0;

    protected InAppFloatingBanner()
    {
    }

    public void Show()
    {
      if (this.view == null)
        return;
      if (this.popup == null)
      {
        this.view.DragStarted += (EventHandler) ((sender, e) => this.OnContentDraggingStarted());
        this.view.DragEnded += (EventHandler) ((sender, e) => this.OnContentDraggingEnded());
        this.view.Dismissed += (EventHandler) ((sender, e) => this.Close());
        this.view.Tapped += (EventHandler) ((sender, e) => this.View_Tap());
        Popup popup = new Popup();
        popup.Child = this.view as UIElement;
        popup.Height = this.view.GetTargetHeight();
        popup.MaxHeight = this.view.GetMaxHeight();
        this.popup = popup;
        this.popup.Opened += (EventHandler) ((sender, e) => this.OnOpened());
        this.popup.Closed += (EventHandler) ((sender, e) => this.OnClosed());
      }
      if (this.popup.IsOpen)
        return;
      this.RequesetSysTrayHide();
      new PopupManager(this.popup, false).Show();
    }

    public virtual void Close() => this.ClosePopup();

    private void ClosePopup()
    {
      if (this.popup == null || !this.popup.IsOpen)
        return;
      this.popup.IsOpen = false;
    }

    private void DisposeSysTrayHideSub()
    {
      this.sysTrayHideSub.SafeDispose();
      this.sysTrayHideSub = (IDisposable) null;
    }

    private void RequesetSysTrayHide()
    {
      SysTrayHelper.SysTrayKeeper.Instance.ExtraVertialPageMarginAdjustment = this.ShouldShiftPageContentDownInPortrait() ? this.view.GetTargetHeight() - UIUtils.SystemTraySizePortrait : 0.0;
      IDisposable sysTrayHideSub = this.sysTrayHideSub;
      this.sysTrayHideSub = SysTrayHelper.SysTrayKeeper.Instance.RequestHide();
      sysTrayHideSub.SafeDispose();
    }

    protected virtual bool ShouldPreserveBehindNewBanner() => false;

    protected virtual bool ShouldShiftPageContentDownInPortrait() => false;

    protected virtual void OnContentDraggingStarted()
    {
    }

    protected virtual void OnContentDraggingEnded()
    {
    }

    protected virtual void OnOpened()
    {
      InAppFloatingBanner[] array = InAppFloatingBanner.VisibleInstances.ToArray<InAppFloatingBanner>();
      InAppFloatingBanner.VisibleInstances.AddLast(this);
      foreach (InAppFloatingBanner appFloatingBanner in array)
      {
        if (!appFloatingBanner.ShouldPreserveBehindNewBanner())
          appFloatingBanner.Close();
      }
    }

    protected virtual void OnClosed()
    {
      InAppFloatingBanner.VisibleInstances.Remove(this);
      this.DisposeSysTrayHideSub();
      if (this.onComplete == null)
        return;
      this.onComplete();
    }

    protected virtual void View_Tap()
    {
      foreach (InAppFloatingBanner appFloatingBanner in InAppFloatingBanner.VisibleInstances.ToArray<InAppFloatingBanner>())
        appFloatingBanner.Close();
      Action onClick = this.onClick;
      this.onClick = (Action) null;
      if (onClick == null)
        return;
      onClick();
    }
  }
}
