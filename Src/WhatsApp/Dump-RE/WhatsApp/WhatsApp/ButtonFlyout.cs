// Decompiled with JetBrains decompiler
// Type: WhatsApp.ButtonFlyout
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace WhatsApp
{
  public class ButtonFlyout : UserControl
  {
    public static string BUTTON_FLYOUT_NAME_AUTOMATION = "AidFlyoutCanvas";
    private Storyboard openStoryboard;
    public Dictionary<FlyoutCommand, WeakReference<Button>> buttons = new Dictionary<FlyoutCommand, WeakReference<Button>>();
    private bool flyoutOpen;
    private Canvas flyoutCanvas;
    internal Grid LayoutRoot;
    internal Grid ContentPanel;
    private bool _contentLoaded;

    public event EventHandler FlyoutOpening;

    public ButtonFlyout() => this.InitializeComponent();

    public IEnumerable<FlyoutCommand> Actions { get; set; }

    public void UpdateAction(FlyoutCommand action)
    {
      if (!this.buttons.ContainsKey(action))
        return;
      Button target = (Button) null;
      this.buttons[action].TryGetTarget(out target);
      if (target == null)
        return;
      ((TextBlock) target.Content).Foreground = action.IsEnabled ? (Brush) UIUtils.ForegroundBrush : UIUtils.SubtleBrush;
    }

    public IEnumerable<FrameworkElement> Content
    {
      set
      {
        foreach (UIElement uiElement in value)
          this.ContentPanel.Children.Add(uiElement);
      }
    }

    public void OpenFlyout(object sender, EventArgs e)
    {
      if (this.Actions == null)
        return;
      if (!this.FlyoutIsOpen)
      {
        EventHandler flyoutOpening = this.FlyoutOpening;
        if (flyoutOpening != null)
          flyoutOpening((object) this, (EventArgs) null);
      }
      this.FlyoutIsOpen = !this.FlyoutIsOpen;
    }

    private void CreateFlyout()
    {
      if (this.flyoutCanvas != null || this.Actions == null)
        return;
      StackPanel stackPanel1 = new StackPanel();
      stackPanel1.HorizontalAlignment = HorizontalAlignment.Stretch;
      StackPanel stackPanel2 = stackPanel1;
      foreach (FlyoutCommand action in this.Actions)
      {
        Button button1 = new Button();
        button1.HorizontalAlignment = HorizontalAlignment.Stretch;
        button1.Margin = new Thickness();
        button1.Padding = new Thickness();
        button1.BorderThickness = new Thickness();
        button1.HorizontalContentAlignment = HorizontalAlignment.Left;
        TextBlock textBlock = new TextBlock();
        textBlock.Style = this.Resources[(object) "PhoneTextNormalStyle"] as Style;
        textBlock.Text = action.Title;
        textBlock.Margin = new Thickness(10.0, 16.0, 10.0, 16.0);
        textBlock.Foreground = action.IsEnabled ? (Brush) UIUtils.ForegroundBrush : UIUtils.SubtleBrush;
        button1.Content = (object) textBlock;
        button1.Tag = (object) action;
        Button button2 = button1;
        TiltEffect.SetIsTiltEnabled((DependencyObject) button2, true);
        button2.Click += new RoutedEventHandler(this.ItemButton_Click);
        stackPanel2.Children.Add((UIElement) button2);
        this.buttons[action] = new WeakReference<Button>(button2);
      }
      Border border1 = new Border();
      border1.BorderBrush = this.Resources[(object) "PhoneForegroundBrush"] as Brush;
      border1.BorderThickness = new Thickness(2.0);
      border1.Background = this.Resources[(object) "PhoneChromeBrush"] as Brush;
      border1.Width = 320.0;
      Border border2 = border1;
      border2.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.Border_ManipulationStarted);
      border2.Child = (UIElement) stackPanel2;
      double length = 800.0 + this.ActualHeight + 12.0;
      Canvas.SetTop((UIElement) border2, length);
      Canvas.SetLeft((UIElement) border2, 800.0 + this.ActualWidth - 320.0);
      Canvas canvas1 = new Canvas();
      canvas1.Name = ButtonFlyout.BUTTON_FLYOUT_NAME_AUTOMATION;
      canvas1.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(0.0, 800.0 + this.ActualHeight, 2000.0, 2000.0)
      };
      Canvas canvas2 = canvas1;
      canvas2.Children.Add((UIElement) border2);
      Canvas canvas3 = new Canvas();
      canvas3.Background = (Brush) UIUtils.TransparentBrush;
      canvas3.Width = 2000.0;
      canvas3.Height = 2000.0;
      this.flyoutCanvas = canvas3;
      this.flyoutCanvas.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.Canvas_ManipulationStarted);
      this.flyoutCanvas.RenderTransform = (Transform) new CompositeTransform()
      {
        TranslateX = -800.0,
        TranslateY = -800.0
      };
      this.flyoutCanvas.Children.Add((UIElement) canvas2);
      this.LayoutRoot.Children.Add((UIElement) this.flyoutCanvas);
      DoubleAnimation doubleAnimation = new DoubleAnimation();
      doubleAnimation.Duration = (Duration) TimeSpan.FromMilliseconds(150.0);
      doubleAnimation.From = new double?(length - 200.0);
      doubleAnimation.To = new double?(length);
      ExponentialEase exponentialEase = new ExponentialEase();
      exponentialEase.EasingMode = EasingMode.EaseOut;
      exponentialEase.Exponent = 5.0;
      doubleAnimation.EasingFunction = (IEasingFunction) exponentialEase;
      DoubleAnimation element = doubleAnimation;
      Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("(Canvas.Top)", new object[0]));
      Storyboard.SetTarget((Timeline) element, (DependencyObject) border2);
      this.openStoryboard = new Storyboard();
      this.openStoryboard.Children.Add((Timeline) element);
    }

    private void ItemButton_Click(object sender, RoutedEventArgs e)
    {
      FlyoutCommand tag = ((FrameworkElement) sender).Tag as FlyoutCommand;
      if (!tag.IsEnabled)
        return;
      tag.ClickAction();
      this.FlyoutIsOpen = false;
    }

    public bool FlyoutIsOpen
    {
      get => this.flyoutOpen;
      set
      {
        if (value)
        {
          this.CreateFlyout();
          this.openStoryboard.Stop();
          this.openStoryboard.Begin();
          this.flyoutCanvas.Visibility = Visibility.Visible;
        }
        else if (this.flyoutCanvas != null)
          this.flyoutCanvas.Visibility = Visibility.Collapsed;
        this.flyoutOpen = value;
      }
    }

    private void Border_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      e.Handled = true;
    }

    private void Canvas_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.FlyoutIsOpen = false;
      e.Handled = true;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new ButtonFlyout.FlyoutButtonAutomationPeer((FrameworkElement) this);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ButtonFlyout.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContentPanel = (Grid) this.FindName("ContentPanel");
    }

    public class FlyoutButtonAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
      private ButtonFlyout bf;

      public FlyoutButtonAutomationPeer(FrameworkElement peer)
        : base(peer)
      {
        this.bf = peer as ButtonFlyout;
      }

      protected override List<AutomationPeer> GetChildrenCore()
      {
        List<AutomationPeer> childrenCore = new List<AutomationPeer>();
        foreach (FlyoutCommand action in this.bf.Actions)
          childrenCore.Add((AutomationPeer) new ButtonFlyout.FlyoutButtonItemAutomationPeer((FrameworkElement) this.bf, action));
        return childrenCore;
      }

      public override object GetPattern(PatternInterface patternInterface)
      {
        return patternInterface == PatternInterface.Invoke ? (object) this : base.GetPattern(patternInterface);
      }

      public void Invoke() => this.bf.OpenFlyout((object) null, (EventArgs) null);
    }

    public class FlyoutButtonItemAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
      private ButtonFlyout bf;
      private FlyoutCommand command;

      public FlyoutButtonItemAutomationPeer(FrameworkElement peer, FlyoutCommand command)
        : base(peer)
      {
        this.bf = peer as ButtonFlyout;
        this.command = command;
      }

      protected override string GetNameCore() => this.command.Title;

      protected override string GetAutomationIdCore()
      {
        return this.command.AutomationId != null ? this.command.AutomationId : this.command.Title;
      }

      public override object GetPattern(PatternInterface patternInterface)
      {
        return patternInterface == PatternInterface.Invoke ? (object) this : base.GetPattern(patternInterface);
      }

      public void Invoke() => this.command.ClickAction();
    }
  }
}
