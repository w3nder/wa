// Decompiled with JetBrains decompiler
// Type: WhatsApp.SysTrayHelper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public static class SysTrayHelper
  {
    public static bool SetVisible(bool visible)
    {
      bool flag = true;
      try
      {
        SystemTray.IsVisible = visible;
      }
      catch (Exception ex)
      {
        flag = false;
        string context = string.Format("setting system tray visible = {0}", (object) visible);
        Log.LogException(ex, context);
      }
      return flag;
    }

    public static bool SetForegroundColor(DependencyObject obj, Color color)
    {
      bool flag = true;
      try
      {
        SystemTray.SetForegroundColor(obj, color);
      }
      catch (Exception ex)
      {
        flag = false;
        Log.LogException(ex, "setting system tray foreground");
      }
      return flag;
    }

    public static bool SetOpacity(DependencyObject obj, double opacity)
    {
      bool flag = true;
      try
      {
        SystemTray.SetOpacity(obj, opacity);
      }
      catch (Exception ex)
      {
        flag = false;
        Log.LogException(ex, "setting system tray opacity");
      }
      return flag;
    }

    public static bool GetOpacity(DependencyObject obj, out double opacity)
    {
      bool opacity1 = true;
      opacity = 1.0;
      if (obj != null)
      {
        try
        {
          opacity = SystemTray.GetOpacity(obj);
        }
        catch (Exception ex)
        {
          opacity1 = false;
          Log.LogException(ex, "getting system tray opacity");
        }
      }
      return opacity1;
    }

    public static bool IsSysTrayTransparentOnPage(PhoneApplicationPage page)
    {
      double opacity = 1.0;
      return SysTrayHelper.GetOpacity((DependencyObject) page, out opacity) && opacity < 1.0;
    }

    public class SysTrayKeeper
    {
      private static object instanceLock = new object();
      private static SysTrayHelper.SysTrayKeeper instance = (SysTrayHelper.SysTrayKeeper) null;
      private RefCountAction hideSysTraySub;
      private bool wasSysTrayVisiblePreHide = true;
      private bool adjustPageMarginOnHide = true;
      private double extraVertialPageMarginAdjustment;

      public static SysTrayHelper.SysTrayKeeper Instance
      {
        get
        {
          return Utils.LazyInit<SysTrayHelper.SysTrayKeeper>(ref SysTrayHelper.SysTrayKeeper.instance, (Func<SysTrayHelper.SysTrayKeeper>) (() => new SysTrayHelper.SysTrayKeeper()), SysTrayHelper.SysTrayKeeper.instanceLock);
        }
      }

      public double ExtraVertialPageMarginAdjustment
      {
        get => this.extraVertialPageMarginAdjustment;
        set => this.extraVertialPageMarginAdjustment = Math.Max(value, 0.0);
      }

      private SysTrayKeeper()
      {
        this.hideSysTraySub = new RefCountAction(new Action(this.OnSystemTrayHideRequested), new Action(this.OnSystemTrayRestoreRequested));
      }

      public IDisposable RequestHide(bool adjustPageMargin = true)
      {
        this.adjustPageMarginOnHide = adjustPageMargin;
        return this.hideSysTraySub.Subscribe();
      }

      private void ClearRootFrameEventsSubscriptions()
      {
        TransitionFrame rootFrame = App.CurrentApp.RootFrame;
        rootFrame.OrientationChanged -= new EventHandler<OrientationChangedEventArgs>(this.RootFrame_OrientationChanged);
        rootFrame.Navigating -= new NavigatingCancelEventHandler(this.RootFrame_Navigating);
        rootFrame.Navigated -= new NavigatedEventHandler(this.RootFrame_Navigated);
      }

      private void UpdatePageMargin(PageOrientation orientation = PageOrientation.None)
      {
        PhoneApplicationPage currentPage = App.CurrentApp.CurrentPage;
        if (currentPage == null)
          return;
        Thickness thickness = new Thickness(0.0);
        bool flag1 = this.hideSysTraySub.InEffect && this.wasSysTrayVisiblePreHide && !SysTrayHelper.IsSysTrayTransparentOnPage(currentPage);
        bool flag2 = orientation.IsPortrait() && this.ExtraVertialPageMarginAdjustment > 0.0;
        if (flag1 | flag2)
        {
          if (orientation.IsPortrait())
          {
            double num = (flag1 ? UIUtils.SystemTraySizePortrait : 0.0) + this.ExtraVertialPageMarginAdjustment;
            thickness = orientation != PageOrientation.PortraitDown ? new Thickness(0.0, num, 0.0, 0.0) : new Thickness(0.0, 0.0, 0.0, num);
          }
          else
          {
            switch (orientation)
            {
              case PageOrientation.LandscapeLeft:
                thickness = new Thickness(UIUtils.SystemTraySizeLandscape, 0.0, 0.0, 0.0);
                break;
              case PageOrientation.LandscapeRight:
                thickness = new Thickness(0.0, 0.0, UIUtils.SystemTraySizeLandscape, 0.0);
                break;
            }
          }
        }
        currentPage.Margin = thickness;
      }

      private void HideSystemTray()
      {
        if (!this.wasSysTrayVisiblePreHide)
          return;
        SysTrayHelper.SetVisible(false);
      }

      private void RestoreState()
      {
        SysTrayHelper.SetVisible(this.wasSysTrayVisiblePreHide);
        PhoneApplicationPage currentPage = App.CurrentApp.CurrentPage;
        if (currentPage == null)
          return;
        currentPage.Margin = new Thickness(0.0);
      }

      private void SaveState()
      {
        try
        {
          this.wasSysTrayVisiblePreHide = SystemTray.IsVisible;
        }
        catch (Exception ex)
        {
          this.wasSysTrayVisiblePreHide = true;
        }
      }

      private void OnSystemTrayHideRequested()
      {
        this.SaveState();
        TransitionFrame rootFrame = App.CurrentApp.RootFrame;
        this.ClearRootFrameEventsSubscriptions();
        rootFrame.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(this.RootFrame_OrientationChanged);
        rootFrame.Navigating += new NavigatingCancelEventHandler(this.RootFrame_Navigating);
        rootFrame.Navigated += new NavigatedEventHandler(this.RootFrame_Navigated);
        this.HideSystemTray();
        if (!this.adjustPageMarginOnHide)
          return;
        this.UpdatePageMargin(rootFrame.Orientation);
      }

      private void OnSystemTrayRestoreRequested()
      {
        this.ClearRootFrameEventsSubscriptions();
        this.RestoreState();
      }

      private void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
      {
        this.RestoreState();
      }

      private void RootFrame_Navigated(object sender, NavigationEventArgs e)
      {
        this.SaveState();
        if (this.hideSysTraySub.InEffect)
          this.HideSystemTray();
        this.UpdatePageMargin(App.CurrentApp.RootFrame.Orientation);
      }

      private void RootFrame_OrientationChanged(object sender, OrientationChangedEventArgs e)
      {
        this.UpdatePageMargin(e.Orientation);
      }
    }
  }
}
