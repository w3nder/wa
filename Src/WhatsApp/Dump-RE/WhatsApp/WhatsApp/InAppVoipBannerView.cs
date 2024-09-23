// Decompiled with JetBrains decompiler
// Type: WhatsApp.InAppVoipBannerView
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class InAppVoipBannerView : Grid, IInAppFloatingBannerView
  {
    private bool showDebugInfo;
    private double targetHeight = 64.0;
    private double maxHeight = 64.0;
    private TextBlock miscBlock;
    private TextBlock durationBlock;
    private TextBlock nameBlock;
    private Image icon;
    private StackPanel panel;
    private IDisposable pendingSbSub;
    private IDisposable timerSub;
    private int ticks;
    private bool isShowingCallInfo = true;
    private UiCallState callState;
    private int? duration;

    public InAppVoipBannerView()
    {
      double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
      this.VerticalAlignment = VerticalAlignment.Top;
      this.MinHeight = this.MaxHeight = this.targetHeight = this.maxHeight = UIUtils.SystemTraySizePortrait * zoomMultiplier + 32.0;
      this.Background = (Brush) UIUtils.AccentBrush;
      this.Margin = new Thickness(0.0);
      StackPanel stackPanel = new StackPanel();
      stackPanel.Orientation = Orientation.Horizontal;
      stackPanel.Margin = new Thickness(24.0 * zoomMultiplier, 0.0, 0.0, 6.0 * zoomMultiplier);
      stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
      stackPanel.VerticalAlignment = VerticalAlignment.Bottom;
      stackPanel.RenderTransform = (Transform) new CompositeTransform();
      this.panel = stackPanel;
      UIElementCollection children1 = this.panel.Children;
      Image image1 = new Image();
      image1.Source = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("whatsapp-icon.png", AssetStore.ThemeSetting.Dark);
      image1.Width = 22.0 * zoomMultiplier;
      image1.Height = 22.0 * zoomMultiplier;
      image1.VerticalAlignment = VerticalAlignment.Bottom;
      image1.Margin = new Thickness(0.0, 0.0, 0.0, 3.0 * zoomMultiplier);
      Image image2 = image1;
      this.icon = image1;
      Image image3 = image2;
      children1.Add((UIElement) image3);
      UIElementCollection children2 = this.panel.Children;
      TextBlock textBlock1 = new TextBlock();
      textBlock1.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock1.TextWrapping = TextWrapping.NoWrap;
      textBlock1.VerticalAlignment = VerticalAlignment.Bottom;
      textBlock1.FontSize = 22.0 * zoomMultiplier;
      textBlock1.Margin = new Thickness(12.0, 0.0, 0.0, 0.0);
      TextBlock textBlock2 = textBlock1;
      this.miscBlock = textBlock1;
      TextBlock textBlock3 = textBlock2;
      children2.Add((UIElement) textBlock3);
      UIElementCollection children3 = this.panel.Children;
      TextBlock textBlock4 = new TextBlock();
      textBlock4.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock4.TextWrapping = TextWrapping.NoWrap;
      textBlock4.VerticalAlignment = VerticalAlignment.Bottom;
      textBlock4.FontSize = 22.0 * zoomMultiplier;
      textBlock4.Margin = new Thickness(12.0, 0.0, 0.0, 0.0);
      TextBlock textBlock5 = textBlock4;
      this.durationBlock = textBlock4;
      TextBlock textBlock6 = textBlock5;
      children3.Add((UIElement) textBlock6);
      UIElementCollection children4 = this.panel.Children;
      TextBlock textBlock7 = new TextBlock();
      textBlock7.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock7.TextWrapping = TextWrapping.NoWrap;
      textBlock7.VerticalAlignment = VerticalAlignment.Bottom;
      textBlock7.FontSize = 22.0 * zoomMultiplier;
      textBlock7.Margin = new Thickness(12.0, 0.0, 0.0, 0.0);
      TextBlock textBlock8 = textBlock7;
      this.nameBlock = textBlock7;
      TextBlock textBlock9 = textBlock8;
      children4.Add((UIElement) textBlock9);
      Button button = new Button();
      button.Style = Application.Current.Resources[(object) "BorderlessButton"] as Style;
      button.Margin = new Thickness(0.0);
      button.Padding = new Thickness(0.0);
      button.BorderThickness = new Thickness(0.0);
      button.HorizontalContentAlignment = HorizontalAlignment.Left;
      button.VerticalContentAlignment = VerticalAlignment.Bottom;
      button.Content = (object) this.panel;
      this.Children.Add((UIElement) button);
      TiltEffect.SetIsTiltEnabled((DependencyObject) this, true);
      this.Tap += (EventHandler<System.Windows.Input.GestureEventArgs>) ((sender, e) => this.NotifyTapped());
      this.timerSub = Observable.Timer(TimeSpan.FromSeconds(1.0)).Repeat<long>().ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
      {
        ++this.ticks;
        if (this.ticks % 6 == 0)
        {
          this.ToggleViews();
          this.ticks = 0;
        }
        if (!this.duration.HasValue)
          return;
        this.SetDuration(new int?(this.duration.Value + 1));
      }));
    }

    public static InAppVoipBannerView Create(string userName, UiCallState initialState)
    {
      InAppVoipBannerView appVoipBannerView = new InAppVoipBannerView();
      appVoipBannerView.nameBlock.Text = userName;
      appVoipBannerView.SetCallState(initialState);
      return appVoipBannerView;
    }

    public void SetCallState(UiCallState state)
    {
      this.callState = state;
      this.UpdateMiscBlock();
    }

    public void SetDuration(int? durationInSecs)
    {
      if ((this.duration = durationInSecs).HasValue)
        this.durationBlock.Text = DateTimeUtils.FormatDuration(this.duration.Value);
      else
        this.durationBlock.Text = "";
    }

    public void UpdateParticipants(string participantsNames)
    {
      this.nameBlock.Text = participantsNames;
    }

    public void Dispose()
    {
      this.pendingSbSub.SafeDispose();
      this.pendingSbSub = (IDisposable) null;
      this.timerSub.SafeDispose();
      this.timerSub = (IDisposable) null;
    }

    private void UpdateMiscBlock()
    {
      if (this.isShowingCallInfo)
      {
        if (this.callState == UiCallState.Calling)
        {
          this.miscBlock.Text = AppResources.CallScreenLabelDialing;
          this.miscBlock.Visibility = Visibility.Visible;
        }
        else if (this.showDebugInfo)
        {
          this.miscBlock.Text = string.Format("({0})", (object) this.callState);
          this.miscBlock.Visibility = Visibility.Visible;
        }
        else
          this.miscBlock.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.miscBlock.Text = !this.showDebugInfo ? AppResources.TapToExpand : string.Format("{0} ({1})", (object) AppResources.TapToExpand, (object) this.callState);
        this.miscBlock.Visibility = Visibility.Visible;
      }
    }

    public void ToggleViews()
    {
      this.pendingSbSub.SafeDispose();
      this.pendingSbSub = (IDisposable) null;
      DoubleAnimation[] animations1 = new DoubleAnimation[2]
      {
        WaAnimations.Fade(WaAnimations.FadeType.FadeOut, TimeSpan.FromMilliseconds(350.0)),
        null
      };
      TimeSpan duration1 = TimeSpan.FromMilliseconds(450.0);
      PowerEase easeFunc = new PowerEase();
      easeFunc.EasingMode = EasingMode.EaseIn;
      easeFunc.Power = 3.0;
      animations1[1] = WaAnimations.HorizontalSlide(0.0, 400.0, duration1, easeFunc: (EasingFunctionBase) easeFunc);
      this.pendingSbSub = Storyboarder.PerformWithDisposable(WaAnimations.CreateStoryboard((IEnumerable<DoubleAnimation>) animations1), (DependencyObject) this.panel, onComplete: (Action) (() =>
      {
        this.pendingSbSub = (IDisposable) null;
        this.panel.Opacity = 0.0;
        this.isShowingCallInfo = !this.isShowingCallInfo;
        if (this.isShowingCallInfo)
        {
          this.nameBlock.Visibility = Visibility.Visible;
          this.durationBlock.Visibility = (this.callState == UiCallState.Active).ToVisibility();
        }
        else
          this.durationBlock.Visibility = this.nameBlock.Visibility = Visibility.Collapsed;
        this.UpdateMiscBlock();
        DoubleAnimation[] animations2 = new DoubleAnimation[2]
        {
          WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(150.0)),
          null
        };
        TimeSpan duration2 = TimeSpan.FromMilliseconds(200.0);
        animations2[1] = WaAnimations.HorizontalSlide(-200.0, 0.0, duration2, easeFunc: (EasingFunctionBase) new PowerEase()
        {
          EasingMode = EasingMode.EaseOut,
          Power = 3.0
        });
        this.pendingSbSub = Storyboarder.PerformWithDisposable(WaAnimations.CreateStoryboard((IEnumerable<DoubleAnimation>) animations2), (DependencyObject) this.panel, onComplete: (Action) (() =>
        {
          this.panel.Opacity = 1.0;
          this.pendingSbSub = (IDisposable) null;
        }), callOnCompleteOnDisposing: true, context: "slide in voip banner content");
      }), callOnCompleteOnDisposing: true, context: "slide away voip banner content");
    }

    public event EventHandler DragStarted;

    public event EventHandler DragEnded;

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

    public double GetTargetHeight() => this.targetHeight;

    public double GetMaxHeight() => this.maxHeight;

    public double GetActualHeight() => this.ActualHeight;

    public IObservable<Unit> HandleTimeout() => Observable.Empty<Unit>();
  }
}
