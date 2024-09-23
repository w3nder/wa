// Decompiled with JetBrains decompiler
// Type: WhatsApp.AudioEndpointSwitchPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace WhatsApp
{
  public class AudioEndpointSwitchPanel : UserControl
  {
    private Popup popup_;
    private PageOrientation orientation_ = PageOrientation.Portrait;
    internal Storyboard DropdownAnimation;
    internal Storyboard SlideUpAnimation;
    internal Grid LayoutRoot;
    internal Button SwitchButton;
    internal RoundButton RoundButton;
    private bool _contentLoaded;

    public AudioEndpointSwitchPanelViewModel ViewModel { get; private set; }

    public AudioEndpointSwitchPanel()
    {
      this.InitializeComponent();
      this.ViewModel = new AudioEndpointSwitchPanelViewModel(this);
      this.DataContext = (object) this.ViewModel;
      this.ViewModel.AudioEndpointChanged += (EventHandler) ((sender, e) => this.NotifyAudioEndpointChanged());
    }

    public event EventHandler AudioEndpointChanged;

    protected void NotifyAudioEndpointChanged()
    {
      if (this.AudioEndpointChanged == null)
        return;
      this.AudioEndpointChanged((object) this, new EventArgs());
    }

    public Popup AudioEndpointSwitchPopup
    {
      get
      {
        if (this.popup_ == null)
        {
          Popup popup = new Popup();
          popup.Height = 200.0;
          popup.HorizontalOffset = 0.0;
          popup.VerticalOffset = 0.0;
          this.popup_ = popup;
          this.popup_.Child = (UIElement) this;
          this.popup_.Opened += (EventHandler) ((sender, e) => Storyboarder.Perform(this.Resources, "DropdownAnimation", onComplete: (Action) (() =>
          {
            if (!(this.LayoutRoot.RenderTransform is TranslateTransform renderTransform2))
              return;
            renderTransform2.Y = -82.0;
          })));
        }
        return this.popup_;
      }
    }

    public PageOrientation Orientation
    {
      get => this.orientation_;
      set
      {
        this.orientation_ = value;
        this.OnOrientationChanged();
      }
    }

    public bool IsPopupOpen => this.popup_ != null && this.popup_.IsOpen;

    public IDisposable ClosePopup()
    {
      Storyboard sb = (Storyboard) null;
      bool cancel = false;
      if (this.IsPopupOpen)
        sb = Storyboarder.Perform(this.Resources, "SlideUpAnimation", onComplete: (Action) (() =>
        {
          sb = (Storyboard) null;
          if (this.LayoutRoot.RenderTransform is TranslateTransform renderTransform2)
          {
            double num = cancel ? -82.0 : -200.0;
            renderTransform2.Y = num;
          }
          if (cancel)
            return;
          this.popup_.IsOpen = false;
        }));
      return (IDisposable) new DisposableAction((Action) (() =>
      {
        cancel = true;
        if (sb == null)
          return;
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
        {
          if (sb == null)
            return;
          try
          {
            sb.Stop();
          }
          catch (Exception ex)
          {
          }
        }));
      }));
    }

    private void OnOrientationChanged()
    {
      if (this.Orientation.IsLandscape())
        this.AudioEndpointSwitchPopup.Width = Math.Max(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight);
      else
        this.AudioEndpointSwitchPopup.Width = Math.Min(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight);
      if (!(this.AudioEndpointSwitchPopup.Child is AudioEndpointSwitchPanel child))
        return;
      child.Width = this.AudioEndpointSwitchPopup.Width;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/AudioEndpointSwitchPanel.xaml", UriKind.Relative));
      this.DropdownAnimation = (Storyboard) this.FindName("DropdownAnimation");
      this.SlideUpAnimation = (Storyboard) this.FindName("SlideUpAnimation");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.SwitchButton = (Button) this.FindName("SwitchButton");
      this.RoundButton = (RoundButton) this.FindName("RoundButton");
    }
  }
}
