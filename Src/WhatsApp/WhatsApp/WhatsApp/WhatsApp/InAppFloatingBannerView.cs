// Decompiled with JetBrains decompiler
// Type: WhatsApp.InAppFloatingBannerView
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace WhatsApp
{
  public class InAppFloatingBannerView : UserControl, IInAppFloatingBannerView
  {
    private const double DimissZoneStart = 0.33;
    private const double SwipingSpeedThreshold = 300.0;
    private bool isClickHandled;
    private double targetHeight = 64.0;
    private double maxHeight = 64.0;
    private DispatcherTimer draggingTimeoutTimer = new DispatcherTimer()
    {
      Interval = TimeSpan.FromMilliseconds(2500.0)
    };
    private UIUtils.FrameworkElementWrapper layoutRootWrapper;
    private Storyboard appearSB;
    private Storyboard timeoutDismissSB;
    private Storyboard userDismissSB;
    private Storyboard userRestoreSB;
    internal Storyboard AppearAnimation;
    internal Storyboard TimeoutDismissAnimation;
    internal Storyboard UserDismissAnimation;
    internal Storyboard UserRestoreAnimation;
    internal Grid LayoutRoot;
    private bool _contentLoaded;

    public event EventHandler DragStarted;

    protected void NotifyDragStarted()
    {
      if (this.DragStarted == null)
        return;
      this.DragStarted((object) this, new EventArgs());
    }

    public event EventHandler DragEnded;

    protected void NotifyDragEnded()
    {
      if (this.DragEnded == null)
        return;
      this.DragEnded((object) this, new EventArgs());
    }

    public event EventHandler Dismissed;

    protected void NotifyDismissed()
    {
      if (this.Dismissed == null)
        return;
      this.Dismissed((object) this, new EventArgs());
    }

    public event EventHandler Tapped;

    protected void NotifyTapped()
    {
      if (this.Tapped == null)
        return;
      this.Tapped((object) this, new EventArgs());
    }

    private Storyboard AppearSB
    {
      get
      {
        return this.appearSB ?? (this.appearSB = this.Resources[(object) "AppearAnimation"] as Storyboard);
      }
    }

    private Storyboard TimeoutDismissSB
    {
      get
      {
        return this.timeoutDismissSB ?? (this.timeoutDismissSB = this.Resources[(object) "TimeoutDismissAnimation"] as Storyboard);
      }
    }

    private Storyboard UserClickSB => this.TimeoutDismissSB;

    private Storyboard UserDismissSB
    {
      get
      {
        return this.userDismissSB ?? (this.userDismissSB = this.Resources[(object) "UserDismissAnimation"] as Storyboard);
      }
    }

    private Storyboard UserRestoreSB
    {
      get
      {
        return this.userRestoreSB ?? (this.userRestoreSB = this.Resources[(object) "UserRestoreAnimation"] as Storyboard);
      }
    }

    private InAppFloatingBannerView()
    {
      this.InitializeComponent();
      this.layoutRootWrapper = new UIUtils.FrameworkElementWrapper((FrameworkElement) this.LayoutRoot);
      this.draggingTimeoutTimer.Tick += (EventHandler) ((sender, e) => this.HandleDragCompleted(0.0));
    }

    public static InAppFloatingBannerView CreateForMessage(Message msg)
    {
      InAppFloatingBannerView forMessage = new InAppFloatingBannerView();
      forMessage.DataContext = (object) new InAppFloatingBannerViewModel(msg);
      return forMessage;
    }

    public static InAppFloatingBannerView CreateForError(string content)
    {
      InAppFloatingBannerView forError = new InAppFloatingBannerView();
      forError.DataContext = (object) new InAppFloatingBannerViewModel(content, true);
      return forError;
    }

    public static InAppFloatingBannerView CreateForString(string msg)
    {
      InAppFloatingBannerView forString = new InAppFloatingBannerView();
      forString.DataContext = (object) new InAppFloatingBannerViewModel(msg, false);
      return forString;
    }

    public double GetTargetHeight() => this.targetHeight;

    public double GetMaxHeight() => this.maxHeight;

    public double GetActualHeight() => this.ActualHeight;

    private void StopAllAnimations()
    {
      if (this.appearSB != null)
        this.AppearSB.Stop();
      if (this.timeoutDismissSB != null)
        this.TimeoutDismissSB.Stop();
      if (this.userRestoreSB != null)
        this.UserRestoreSB.Stop();
      if (this.userDismissSB == null)
        return;
      this.UserDismissSB.Stop();
    }

    private bool PlayRestoreAnimation(Action onComplete)
    {
      Storyboard userRestoreSb = this.UserRestoreSB;
      if (userRestoreSb == null || !(userRestoreSb.Children[0] is DoubleAnimation child1))
        return false;
      child1.From = new double?(this.layoutRootWrapper.Left);
      if (!(userRestoreSb.Children[1] is DoubleAnimation child2))
        return false;
      child2.From = new double?(this.LayoutRoot.Opacity);
      this.StopAllAnimations();
      try
      {
        Storyboarder.Perform(userRestoreSb, onComplete: onComplete);
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    private void RestoreNotification()
    {
      this.PlayRestoreAnimation((Action) (() =>
      {
        this.layoutRootWrapper.Left = 0.0;
        this.LayoutRoot.Opacity = 1.0;
      }));
    }

    private bool PlayDismissAnimation(Action onComplete)
    {
      Storyboard userDismissSb = this.UserDismissSB;
      if (userDismissSb == null || !(userDismissSb.Children[0] is DoubleAnimation child1))
        return false;
      child1.From = new double?(this.layoutRootWrapper.Left);
      child1.To = new double?(this.layoutRootWrapper.Left + this.LayoutRoot.ActualWidth);
      if (!(userDismissSb.Children[1] is DoubleAnimation child2))
        return false;
      child2.From = new double?(this.LayoutRoot.Opacity);
      this.StopAllAnimations();
      try
      {
        Storyboarder.Perform(userDismissSb, onComplete: onComplete);
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    private void DismissNotification()
    {
      this.PlayDismissAnimation((Action) (() =>
      {
        this.LayoutRoot.Opacity = 0.0;
        this.NotifyDismissed();
      }));
    }

    public IObservable<Unit> HandleTimeout()
    {
      return Observable.CreateWithDisposable<Unit>((Func<IObserver<Unit>, IDisposable>) (observer =>
      {
        this.StopAllAnimations();
        return Storyboarder.PerformWithDisposable(this.TimeoutDismissSB, onComplete: (Action) (() =>
        {
          this.LayoutRoot.Opacity = 0.0;
          observer.OnCompleted();
        }), callOnCompleteOnDisposing: true, context: "");
      }));
    }

    private void HandleDragCompleted(double velocity)
    {
      this.draggingTimeoutTimer.Stop();
      this.NotifyDragEnded();
      if (velocity > 300.0)
        this.DismissNotification();
      else if (velocity > -300.0)
      {
        if (this.layoutRootWrapper.Left / this.LayoutRoot.ActualWidth > 0.33)
          this.DismissNotification();
        else
          this.RestoreNotification();
      }
      else
        this.RestoreNotification();
    }

    private void OnTap(object sender, GestureEventArgs e)
    {
      if (this.isClickHandled)
        return;
      this.isClickHandled = true;
      this.StopAllAnimations();
      Storyboarder.Perform(this.UserClickSB, onComplete: (Action) (() => this.NotifyTapped()));
    }

    private void OnDragStarted(object sender, DragStartedGestureEventArgs e)
    {
      this.draggingTimeoutTimer.Start();
      this.NotifyDragStarted();
    }

    private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
    {
      double num1 = this.layoutRootWrapper.Left + e.HorizontalChange;
      if (num1 < 0.0)
        num1 = 0.0;
      this.layoutRootWrapper.Left = num1;
      double num2 = num1 / this.ActualWidth;
      this.LayoutRoot.Opacity = num2 < 0.33 ? 1.0 : 1.0 - num2;
      this.draggingTimeoutTimer.Stop();
      this.draggingTimeoutTimer.Start();
    }

    private void OnDragCompleted(object sender, DragCompletedGestureEventArgs e)
    {
      this.HandleDragCompleted(e.HorizontalVelocity);
    }

    private void OnFlicked(object sender, FlickGestureEventArgs e)
    {
      this.HandleDragCompleted(e.HorizontalVelocity);
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => Storyboarder.Perform(this.AppearSB);

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/InAppFloatingBannerView.xaml", UriKind.Relative));
      this.AppearAnimation = (Storyboard) this.FindName("AppearAnimation");
      this.TimeoutDismissAnimation = (Storyboard) this.FindName("TimeoutDismissAnimation");
      this.UserDismissAnimation = (Storyboard) this.FindName("UserDismissAnimation");
      this.UserRestoreAnimation = (Storyboard) this.FindName("UserRestoreAnimation");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
    }
  }
}
