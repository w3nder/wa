// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.ContextMenu
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplateVisualState(GroupName = "VisibilityStates", Name = "Closed")]
  [TemplateVisualState(GroupName = "VisibilityStates", Name = "OpenLandscape")]
  [TemplateVisualState(GroupName = "VisibilityStates", Name = "Open")]
  [TemplateVisualState(GroupName = "VisibilityStates", Name = "OpenReversed")]
  [TemplateVisualState(GroupName = "VisibilityStates", Name = "OpenLandscapeReversed")]
  public class ContextMenu : MenuBase
  {
    private const double LandscapeWidth = 480.0;
    private const double SystemTrayLandscapeWidth = 72.0;
    private const double ApplicationBarLandscapeWidth = 72.0;
    private const double TotalBorderWidth = 8.0;
    private const string VisibilityGroupName = "VisibilityStates";
    private const string OpenVisibilityStateName = "Open";
    private const string OpenReversedVisibilityStateName = "OpenReversed";
    private const string ClosedVisibilityStateName = "Closed";
    private const string OpenLandscapeVisibilityStateName = "OpenLandscape";
    private const string OpenLandscapeReversedVisibilityStateName = "OpenLandscapeReversed";
    private StackPanel _outerPanel;
    private Grid _innerGrid;
    private PhoneApplicationPage _page;
    private readonly List<ApplicationBarIconButton> _applicationBarIconButtons = new List<ApplicationBarIconButton>();
    private Storyboard _backgroundResizeStoryboard;
    private List<Storyboard> _openingStoryboard;
    private bool _openingStoryboardPlaying;
    private DateTime _openingStoryboardReleaseThreshold;
    private PhoneApplicationFrame _rootVisual;
    private DependencyObject _owner;
    private Popup _popup;
    private Panel _overlay;
    private Point _popupAlignmentPoint;
    private bool _settingIsOpen;
    private bool _reversed;
    private Brush _backgroundBrush = (Brush) Application.Current.Resources[(object) "PhoneBackgroundBrush"];
    public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(nameof (IsZoomEnabled), typeof (bool), typeof (ContextMenu), new PropertyMetadata((object) true));
    public static readonly DependencyProperty IsFadeEnabledProperty = DependencyProperty.Register(nameof (IsFadeEnabled), typeof (bool), typeof (ContextMenu), new PropertyMetadata((object) true));
    public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(nameof (VerticalOffset), typeof (double), typeof (ContextMenu), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(ContextMenu.OnVerticalOffsetChanged)));
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof (IsOpen), typeof (bool), typeof (ContextMenu), new PropertyMetadata((object) false, new PropertyChangedCallback(ContextMenu.OnIsOpenChanged)));
    public static readonly DependencyProperty RegionOfInterestProperty = DependencyProperty.Register(nameof (RegionOfInterest), typeof (Rect?), typeof (ContextMenu), (PropertyMetadata) null);
    private static readonly DependencyProperty ApplicationBarMirrorProperty = DependencyProperty.Register("ApplicationBarMirror", typeof (IApplicationBar), typeof (ContextMenu), new PropertyMetadata(new PropertyChangedCallback(ContextMenu.OnApplicationBarMirrorChanged)));

    public DependencyObject Owner
    {
      get => this._owner;
      internal set
      {
        if (this._owner != null && this._owner is FrameworkElement owner1)
        {
          owner1.Hold -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.OnOwnerHold);
          owner1.Unloaded -= new RoutedEventHandler(this.OnOwnerUnloaded);
          this.OnOwnerUnloaded((object) null, (RoutedEventArgs) null);
        }
        this._owner = value;
        if (this._owner == null || !(this._owner is FrameworkElement owner2))
          return;
        owner2.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(this.OnOwnerHold);
        owner2.Unloaded += new RoutedEventHandler(this.OnOwnerUnloaded);
      }
    }

    public bool IsZoomEnabled
    {
      get => (bool) this.GetValue(ContextMenu.IsZoomEnabledProperty);
      set => this.SetValue(ContextMenu.IsZoomEnabledProperty, (object) value);
    }

    public bool IsFadeEnabled
    {
      get => (bool) this.GetValue(ContextMenu.IsFadeEnabledProperty);
      set => this.SetValue(ContextMenu.IsFadeEnabledProperty, (object) value);
    }

    [TypeConverter(typeof (LengthConverter))]
    public double VerticalOffset
    {
      get => (double) this.GetValue(ContextMenu.VerticalOffsetProperty);
      set => this.SetValue(ContextMenu.VerticalOffsetProperty, (object) value);
    }

    private static void OnVerticalOffsetChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((ContextMenu) o).UpdateContextMenuPlacement();
    }

    public bool IsOpen
    {
      get => (bool) this.GetValue(ContextMenu.IsOpenProperty);
      set => this.SetValue(ContextMenu.IsOpenProperty, (object) value);
    }

    private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      ((ContextMenu) o).OnIsOpenChanged((bool) e.NewValue);
    }

    private void OnIsOpenChanged(bool newValue)
    {
      if (this._settingIsOpen)
        return;
      if (newValue)
        this.OpenPopup(new Point(-1.0, -1.0));
      else
        this.ClosePopup();
    }

    public Rect? RegionOfInterest
    {
      get => (Rect?) this.GetValue(ContextMenu.RegionOfInterestProperty);
      set => this.SetValue(ContextMenu.RegionOfInterestProperty, (object) value);
    }

    public event RoutedEventHandler Opened;

    protected virtual void OnOpened(RoutedEventArgs e)
    {
      this.SetRenderTransform();
      this.UpdateVisualStates(true);
      RoutedEventHandler opened = this.Opened;
      if (opened == null)
        return;
      opened((object) this, e);
    }

    private void SetRenderTransform()
    {
      if (DesignerProperties.IsInDesignTool || this._rootVisual.Orientation.IsPortrait())
      {
        double x = this._popupAlignmentPoint.X / this.Width;
        if (this._outerPanel != null)
          this._outerPanel.RenderTransformOrigin = new Point(x, 0.0);
        if (this._innerGrid == null)
          return;
        this._innerGrid.RenderTransformOrigin = new Point(0.0, this._reversed ? 1.0 : 0.0);
      }
      else
      {
        if (this._outerPanel != null)
          this._outerPanel.RenderTransformOrigin = new Point(0.0, 0.5);
        if (this._innerGrid == null)
          return;
        this._innerGrid.RenderTransformOrigin = new Point(this._reversed ? 1.0 : 0.0, 0.0);
      }
    }

    public event RoutedEventHandler Closed;

    protected virtual void OnClosed(RoutedEventArgs e)
    {
      this.UpdateVisualStates(true);
      RoutedEventHandler closed = this.Closed;
      if (closed == null)
        return;
      closed((object) this, e);
    }

    public ContextMenu()
    {
      this.IsZoomEnabled = false;
      this.FlowDirection = FlowDirection.LeftToRight;
      this.DefaultStyleKey = (object) typeof (ContextMenu);
      this._openingStoryboard = new List<Storyboard>();
      if (Application.Current.RootVisual == null)
        this.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
      else
        this.InitializeRootVisual();
    }

    public override void OnApplyTemplate()
    {
      if (this._openingStoryboard != null)
      {
        foreach (Timeline timeline in this._openingStoryboard)
          timeline.Completed -= new EventHandler(this.OnStoryboardCompleted);
        this._openingStoryboard.Clear();
      }
      this._openingStoryboardPlaying = false;
      base.OnApplyTemplate();
      this.SetDefaultStyle();
      if (VisualTreeHelper.GetChild((DependencyObject) this, 0) is FrameworkElement child)
      {
        foreach (VisualStateGroup visualStateGroup in (IEnumerable) VisualStateManager.GetVisualStateGroups(child))
        {
          if ("VisibilityStates" == visualStateGroup.Name)
          {
            foreach (VisualState state in (IEnumerable) visualStateGroup.States)
            {
              if (("Open" == state.Name || "OpenLandscape" == state.Name || "OpenReversed" == state.Name || "OpenLandscapeReversed" == state.Name) && state.Storyboard != null)
              {
                this._openingStoryboard.Add(state.Storyboard);
                state.Storyboard.Completed += new EventHandler(this.OnStoryboardCompleted);
              }
            }
          }
        }
      }
      this._outerPanel = this.GetTemplateChild("OuterPanel") as StackPanel;
      this._innerGrid = this.GetTemplateChild("InnerGrid") as Grid;
      bool flag = DesignerProperties.IsInDesignTool || this._rootVisual.Orientation.IsPortrait();
      this.SetRenderTransform();
      if (!this.IsOpen)
        return;
      if (this._innerGrid != null)
        this._innerGrid.MinHeight = flag ? 0.0 : this._rootVisual.ActualWidth;
      this.UpdateVisualStates(true);
    }

    private void SetDefaultStyle()
    {
      SolidColorBrush solidColorBrush1;
      SolidColorBrush solidColorBrush2;
      if (DesignerProperties.IsInDesignTool || Application.Current.Resources.IsDarkThemeActive())
      {
        solidColorBrush1 = new SolidColorBrush(Colors.White);
        solidColorBrush2 = new SolidColorBrush(Colors.Black);
      }
      else
      {
        solidColorBrush1 = new SolidColorBrush(Colors.Black);
        solidColorBrush2 = new SolidColorBrush(Colors.White);
      }
      Style style = new Style(typeof (ContextMenu));
      Setter setter1 = new Setter(Control.BackgroundProperty, (object) solidColorBrush1);
      Setter setter2 = new Setter(Control.BorderBrushProperty, (object) solidColorBrush2);
      if (this.Style == null)
      {
        style.Setters.Add((SetterBase) setter1);
        style.Setters.Add((SetterBase) setter2);
      }
      else
      {
        bool flag1 = false;
        bool flag2 = false;
        foreach (Setter setter3 in (PresentationFrameworkCollection<SetterBase>) this.Style.Setters)
        {
          if (setter3.Property == Control.BackgroundProperty)
            flag1 = true;
          else if (setter3.Property == Control.BorderBrushProperty)
            flag2 = true;
          style.Setters.Add((SetterBase) new Setter(setter3.Property, setter3.Value));
        }
        if (!flag1)
          style.Setters.Add((SetterBase) setter1);
        if (!flag2)
          style.Setters.Add((SetterBase) setter2);
      }
      this.Style = style;
    }

    private void OnStoryboardCompleted(object sender, EventArgs e)
    {
      this._openingStoryboardPlaying = false;
    }

    private void UpdateVisualStates(bool useTransitions)
    {
      string stateName;
      if (this.IsOpen)
      {
        if (this._openingStoryboard != null)
        {
          this._openingStoryboardPlaying = true;
          this._openingStoryboardReleaseThreshold = DateTime.UtcNow.AddSeconds(0.3);
        }
        if (this._rootVisual != null && this._rootVisual.Orientation.IsPortrait())
        {
          if (this._outerPanel != null)
            this._outerPanel.Orientation = Orientation.Vertical;
          stateName = this._reversed ? "OpenReversed" : "Open";
        }
        else
        {
          if (this._outerPanel != null)
            this._outerPanel.Orientation = Orientation.Horizontal;
          stateName = this._reversed ? "OpenLandscapeReversed" : "OpenLandscape";
        }
        if (this._backgroundResizeStoryboard != null)
          this._backgroundResizeStoryboard.Begin();
      }
      else
        stateName = "Closed";
      VisualStateManager.GoToState((Control) this, stateName, useTransitions);
    }

    private bool PositionIsOnScreenRight(double position)
    {
      return PageOrientation.LandscapeLeft != this._rootVisual.Orientation ? position < this._rootVisual.ActualHeight / 2.0 : position > this._rootVisual.ActualHeight / 2.0;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      e.Handled = true;
      base.OnMouseLeftButtonDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      switch (e.Key)
      {
        case Key.Escape:
          this.ClosePopup();
          e.Handled = true;
          break;
        case Key.Up:
          this.FocusNextItem(false);
          e.Handled = true;
          break;
        case Key.Down:
          this.FocusNextItem(true);
          e.Handled = true;
          break;
      }
      base.OnKeyDown(e);
    }

    private void OnLayoutUpdated(object sender, EventArgs e)
    {
      if (Application.Current.RootVisual == null)
        return;
      this.InitializeRootVisual();
      this.LayoutUpdated -= new EventHandler(this.OnLayoutUpdated);
    }

    private void OnRootVisualManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (!this._openingStoryboardPlaying || !(DateTime.UtcNow <= this._openingStoryboardReleaseThreshold))
        return;
      this.IsOpen = false;
    }

    private void OnOwnerHold(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.IsOpen)
        return;
      this.OpenPopup(e.GetPosition((UIElement) null));
      e.Handled = true;
    }

    private static void OnApplicationBarMirrorChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((ContextMenu) o).OnApplicationBarMirrorChanged((IApplicationBar) e.OldValue, (IApplicationBar) e.NewValue);
    }

    private void OnApplicationBarMirrorChanged(IApplicationBar oldValue, IApplicationBar newValue)
    {
      if (oldValue != null)
        oldValue.StateChanged -= new EventHandler<ApplicationBarStateChangedEventArgs>(this.OnEventThatClosesContextMenu);
      if (newValue == null)
        return;
      newValue.StateChanged += new EventHandler<ApplicationBarStateChangedEventArgs>(this.OnEventThatClosesContextMenu);
    }

    private void OnEventThatClosesContextMenu(object sender, EventArgs e) => this.IsOpen = false;

    private void OnOwnerUnloaded(object sender, RoutedEventArgs e)
    {
      if (this._rootVisual == null)
        return;
      this._rootVisual.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.OnRootVisualManipulationCompleted);
      this._rootVisual.OrientationChanged -= new EventHandler<OrientationChangedEventArgs>(this.OnEventThatClosesContextMenu);
    }

    private void OnPageBackKeyPress(object sender, CancelEventArgs e)
    {
      if (!this.IsOpen)
        return;
      this.IsOpen = false;
      e.Cancel = true;
    }

    private static GeneralTransform SafeTransformToVisual(UIElement element, UIElement visual)
    {
      try
      {
        return element.TransformToVisual(visual);
      }
      catch (ArgumentException ex)
      {
        return (GeneralTransform) new TranslateTransform();
      }
    }

    private void InitializeRootVisual()
    {
      if (this._rootVisual != null)
        return;
      this._rootVisual = Application.Current.RootVisual as PhoneApplicationFrame;
      if (this._rootVisual == null)
        return;
      this._rootVisual.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.OnRootVisualManipulationCompleted);
      this._rootVisual.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.OnRootVisualManipulationCompleted);
      this._rootVisual.OrientationChanged -= new EventHandler<OrientationChangedEventArgs>(this.OnEventThatClosesContextMenu);
      this._rootVisual.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(this.OnEventThatClosesContextMenu);
    }

    private void InitializePage()
    {
      if (this._rootVisual == null)
        return;
      this._page = this._rootVisual.Content as PhoneApplicationPage;
      if (this._page == null)
        return;
      this._page.BackKeyPress += new EventHandler<CancelEventArgs>(this.OnPageBackKeyPress);
      this.SetBinding(ContextMenu.ApplicationBarMirrorProperty, new Binding()
      {
        Source = (object) this._page,
        Path = new PropertyPath("ApplicationBar", new object[0])
      });
    }

    private void FocusNextItem(bool down)
    {
      int count = this.Items.Count;
      int num = down ? -1 : count;
      if (FocusManager.GetFocusedElement() is MenuItem focusedElement && this == focusedElement.ParentMenuBase)
        num = this.ItemContainerGenerator.IndexFromContainer((DependencyObject) focusedElement);
      int index = num;
      do
      {
        index = (index + count + (down ? 1 : -1)) % count;
      }
      while ((!(this.ItemContainerGenerator.ContainerFromIndex(index) is MenuItem menuItem) || !menuItem.IsEnabled || !menuItem.Focus()) && index != num);
    }

    internal void ChildMenuItemClicked() => this.ClosePopup();

    private void OnContextMenuOrRootVisualSizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.UpdateContextMenuPlacement();
    }

    private void OnOverlayMouseButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!(VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition((UIElement) null), (UIElement) this._rootVisual) as List<UIElement>).Contains((UIElement) this))
        this.ClosePopup();
      e.Handled = true;
    }

    private double AdjustContextMenuPositionForPortraitMode(
      Rect bounds,
      double roiY,
      double roiHeight,
      ref bool reversed)
    {
      double num1 = 0.0;
      bool flag = false;
      double num2 = bounds.Bottom - this.ActualHeight;
      double num3 = bounds.Top + this.ActualHeight;
      if (bounds.Height <= this.ActualHeight)
        flag = true;
      else if (roiY + roiHeight <= num2)
      {
        num1 = roiY + roiHeight;
        reversed = false;
      }
      else if (roiY >= num3)
      {
        num1 = roiY - this.ActualHeight;
        reversed = true;
      }
      else if (this._popupAlignmentPoint.Y >= 0.0)
      {
        num1 = this._popupAlignmentPoint.Y;
        if (num1 <= num2)
          reversed = false;
        else if (num1 >= num3)
        {
          num1 -= this.ActualHeight;
          reversed = true;
        }
        else
          flag = true;
      }
      else
        flag = true;
      if (flag)
      {
        num1 = num2;
        reversed = true;
        if (num1 <= bounds.Top)
        {
          num1 = bounds.Top;
          reversed = false;
        }
      }
      return num1;
    }

    private void UpdateContextMenuPlacement()
    {
      if (this._rootVisual == null || this._overlay == null)
        return;
      Point point = new Point(this._popupAlignmentPoint.X, this._popupAlignmentPoint.Y);
      bool flag = this._rootVisual.Orientation.IsPortrait();
      double width = flag ? this._rootVisual.ActualWidth : this._rootVisual.ActualHeight;
      double height = flag ? this._rootVisual.ActualHeight : this._rootVisual.ActualWidth;
      Rect bounds = new Rect(0.0, 0.0, width, height);
      if (this._page != null)
        bounds = ContextMenu.SafeTransformToVisual((UIElement) this._page, (UIElement) this._rootVisual).TransformBounds(new Rect(0.0, 0.0, this._page.ActualWidth, this._page.ActualHeight));
      if (flag && this._rootVisual != null)
      {
        double y;
        double roiHeight;
        if (this.RegionOfInterest.HasValue)
        {
          y = this.RegionOfInterest.Value.Y;
          roiHeight = this.RegionOfInterest.Value.Height;
        }
        else if (this.Owner is FrameworkElement)
        {
          FrameworkElement owner = (FrameworkElement) this.Owner;
          y = owner.TransformToVisual((UIElement) this._rootVisual).Transform(new Point(0.0, 0.0)).Y;
          roiHeight = owner.ActualHeight;
        }
        else
        {
          y = this._popupAlignmentPoint.Y;
          roiHeight = 0.0;
        }
        point.Y = this.AdjustContextMenuPositionForPortraitMode(bounds, y, roiHeight, ref this._reversed);
      }
      double x = point.X;
      double num = point.Y + this.VerticalOffset;
      double val1;
      if (flag)
      {
        val1 = bounds.Left;
        this.Width = bounds.Width;
        if (this._innerGrid != null)
          this._innerGrid.Width = this.Width;
      }
      else
      {
        if (this.PositionIsOnScreenRight(num))
        {
          this.Width = SystemTray.IsVisible ? 408.0 : 480.0;
          val1 = SystemTray.IsVisible ? 72.0 : 0.0;
          this._reversed = true;
        }
        else
        {
          this.Width = this._page.ApplicationBar == null || !this._page.ApplicationBar.IsVisible ? 480.0 : 408.0;
          val1 = bounds.Width - this.Width + (SystemTray.IsVisible ? 72.0 : 0.0);
          this._reversed = false;
        }
        if (this._innerGrid != null)
          this._innerGrid.Width = this.Width - 8.0;
        num = 0.0;
      }
      Canvas.SetLeft((UIElement) this, Math.Max(val1, 0.0));
      Canvas.SetTop((UIElement) this, num);
      this._overlay.Width = width;
      this._overlay.Height = height;
    }

    private void OpenPopup(Point position)
    {
      this._popupAlignmentPoint = position;
      this.InitializeRootVisual();
      this.InitializePage();
      bool flag = this._rootVisual.Orientation.IsPortrait();
      if (flag)
      {
        if (this._innerGrid != null)
          this._innerGrid.MinHeight = 0.0;
      }
      else if (this._innerGrid != null)
        this._innerGrid.MinHeight = this._rootVisual.ActualWidth;
      Canvas canvas = new Canvas();
      canvas.Background = (Brush) new SolidColorBrush(Colors.Transparent);
      this._overlay = (Panel) canvas;
      this._overlay.MouseLeftButtonUp += new MouseButtonEventHandler(this.OnOverlayMouseButtonUp);
      if (this.IsZoomEnabled && this._rootVisual != null)
      {
        double pixelWidth = flag ? this._rootVisual.ActualWidth : this._rootVisual.ActualHeight;
        double pixelHeight = flag ? this._rootVisual.ActualHeight : this._rootVisual.ActualWidth;
        Rectangle rectangle1 = new Rectangle();
        rectangle1.Width = pixelWidth;
        rectangle1.Height = pixelHeight;
        rectangle1.Fill = this._backgroundBrush;
        rectangle1.CacheMode = (CacheMode) new BitmapCache();
        this._overlay.Children.Insert(0, (UIElement) rectangle1);
        if (this._owner is FrameworkElement owner)
          owner.Opacity = 0.0;
        WriteableBitmap writeableBitmap = new WriteableBitmap((int) pixelWidth, (int) pixelHeight);
        writeableBitmap.Render((UIElement) this._rootVisual, (Transform) null);
        writeableBitmap.Invalidate();
        Transform target1 = (Transform) new ScaleTransform()
        {
          CenterX = (pixelWidth / 2.0),
          CenterY = (pixelHeight / 2.0)
        };
        Image image = new Image();
        image.Source = (ImageSource) writeableBitmap;
        image.RenderTransform = target1;
        image.CacheMode = (CacheMode) new BitmapCache();
        this._overlay.Children.Insert(1, (UIElement) image);
        Rectangle rectangle2 = new Rectangle();
        rectangle2.Width = pixelWidth;
        rectangle2.Height = pixelHeight;
        rectangle2.Fill = this._backgroundBrush;
        rectangle2.Opacity = 0.0;
        rectangle2.CacheMode = (CacheMode) new BitmapCache();
        UIElement target2 = (UIElement) rectangle2;
        this._overlay.Children.Insert(2, target2);
        if (owner != null)
        {
          ((UIElement) this.Owner).Opacity = 1.0;
          Point point = ContextMenu.SafeTransformToVisual((UIElement) owner, (UIElement) this._rootVisual).Transform(new Point(owner.FlowDirection == FlowDirection.RightToLeft ? owner.ActualWidth : 0.0, 0.0));
          Rectangle rectangle3 = new Rectangle();
          rectangle3.Width = owner.ActualWidth;
          rectangle3.Height = owner.ActualHeight;
          rectangle3.Fill = (Brush) new SolidColorBrush(Colors.Transparent);
          rectangle3.CacheMode = (CacheMode) new BitmapCache();
          UIElement element1 = (UIElement) rectangle3;
          Canvas.SetLeft(element1, point.X);
          Canvas.SetTop(element1, point.Y);
          this._overlay.Children.Insert(3, element1);
          UIElement element2 = (UIElement) new Image()
          {
            Source = (ImageSource) new WriteableBitmap((UIElement) owner, (Transform) null)
          };
          Canvas.SetLeft(element2, point.X);
          Canvas.SetTop(element2, point.Y);
          this._overlay.Children.Insert(4, element2);
        }
        double num1 = 1.0;
        double num2 = 0.94;
        TimeSpan timeSpan = TimeSpan.FromSeconds(0.42);
        ExponentialEase exponentialEase = new ExponentialEase();
        exponentialEase.EasingMode = EasingMode.EaseInOut;
        IEasingFunction easingFunction = (IEasingFunction) exponentialEase;
        this._backgroundResizeStoryboard = new Storyboard();
        DoubleAnimation doubleAnimation1 = new DoubleAnimation();
        doubleAnimation1.From = new double?(num1);
        doubleAnimation1.To = new double?(num2);
        doubleAnimation1.Duration = (Duration) timeSpan;
        doubleAnimation1.EasingFunction = easingFunction;
        DoubleAnimation element3 = doubleAnimation1;
        Storyboard.SetTarget((Timeline) element3, (DependencyObject) target1);
        Storyboard.SetTargetProperty((Timeline) element3, new PropertyPath((object) ScaleTransform.ScaleXProperty));
        this._backgroundResizeStoryboard.Children.Add((Timeline) element3);
        DoubleAnimation doubleAnimation2 = new DoubleAnimation();
        doubleAnimation2.From = new double?(num1);
        doubleAnimation2.To = new double?(num2);
        doubleAnimation2.Duration = (Duration) timeSpan;
        doubleAnimation2.EasingFunction = easingFunction;
        DoubleAnimation element4 = doubleAnimation2;
        Storyboard.SetTarget((Timeline) element4, (DependencyObject) target1);
        Storyboard.SetTargetProperty((Timeline) element4, new PropertyPath((object) ScaleTransform.ScaleYProperty));
        this._backgroundResizeStoryboard.Children.Add((Timeline) element4);
        if (this.IsFadeEnabled)
        {
          DoubleAnimation doubleAnimation3 = new DoubleAnimation();
          doubleAnimation3.From = new double?(0.0);
          doubleAnimation3.To = new double?(0.3);
          doubleAnimation3.Duration = (Duration) timeSpan;
          doubleAnimation3.EasingFunction = easingFunction;
          DoubleAnimation element5 = doubleAnimation3;
          Storyboard.SetTarget((Timeline) element5, (DependencyObject) target2);
          Storyboard.SetTargetProperty((Timeline) element5, new PropertyPath((object) UIElement.OpacityProperty));
          this._backgroundResizeStoryboard.Children.Add((Timeline) element5);
        }
      }
      TransformGroup transformGroup = new TransformGroup();
      if (this._rootVisual != null)
      {
        switch (this._rootVisual.Orientation)
        {
          case PageOrientation.LandscapeLeft:
            transformGroup.Children.Add((Transform) new RotateTransform()
            {
              Angle = 90.0
            });
            transformGroup.Children.Add((Transform) new TranslateTransform()
            {
              X = this._rootVisual.ActualWidth
            });
            break;
          case PageOrientation.LandscapeRight:
            transformGroup.Children.Add((Transform) new RotateTransform()
            {
              Angle = -90.0
            });
            transformGroup.Children.Add((Transform) new TranslateTransform()
            {
              Y = this._rootVisual.ActualHeight
            });
            break;
        }
      }
      this._overlay.RenderTransform = (Transform) transformGroup;
      if (this._page != null && this._page.ApplicationBar != null && this._page.ApplicationBar.Buttons != null)
      {
        foreach (object button in (IEnumerable) this._page.ApplicationBar.Buttons)
        {
          if (button is ApplicationBarIconButton applicationBarIconButton)
          {
            applicationBarIconButton.Click += new EventHandler(this.OnEventThatClosesContextMenu);
            this._applicationBarIconButtons.Add(applicationBarIconButton);
          }
        }
      }
      this._overlay.Children.Add((UIElement) this);
      this._popup = new Popup()
      {
        Child = (UIElement) this._overlay
      };
      if (this._rootVisual != null)
        this.FlowDirection = this._rootVisual.FlowDirection;
      this._popup.Opened += (EventHandler) ((s, e) => this.OnOpened(new RoutedEventArgs()));
      this.SizeChanged += new SizeChangedEventHandler(this.OnContextMenuOrRootVisualSizeChanged);
      if (this._rootVisual != null)
        this._rootVisual.SizeChanged += new SizeChangedEventHandler(this.OnContextMenuOrRootVisualSizeChanged);
      if (this.ReadLocalValue(FrameworkElement.DataContextProperty) == DependencyProperty.UnsetValue)
      {
        DependencyObject dependencyObject = this.Owner ?? (DependencyObject) this._rootVisual;
        this.SetBinding(FrameworkElement.DataContextProperty, new Binding("DataContext")
        {
          Source = (object) dependencyObject
        });
      }
      this._popup.IsOpen = true;
      this.Focus();
      this._settingIsOpen = true;
      this.IsOpen = true;
      this._settingIsOpen = false;
    }

    private void ClosePopup()
    {
      if (this._backgroundResizeStoryboard != null)
      {
        foreach (DoubleAnimation child in (PresentationFrameworkCollection<Timeline>) this._backgroundResizeStoryboard.Children)
        {
          double num = child.From.Value;
          child.From = child.To;
          child.To = new double?(num);
        }
        Popup popup = this._popup;
        Panel overlay = this._overlay;
        this._backgroundResizeStoryboard.Completed += (EventHandler) ((param0, param1) =>
        {
          if (popup != null)
          {
            popup.IsOpen = false;
            popup.Child = (UIElement) null;
          }
          overlay?.Children.Clear();
        });
        this._backgroundResizeStoryboard.Begin();
        this._backgroundResizeStoryboard = (Storyboard) null;
        this._popup = (Popup) null;
        this._overlay = (Panel) null;
      }
      else
      {
        if (this._popup != null)
        {
          this._popup.IsOpen = false;
          this._popup.Child = (UIElement) null;
          this._popup = (Popup) null;
        }
        if (this._overlay != null)
        {
          this._overlay.Children.Clear();
          this._overlay = (Panel) null;
        }
      }
      this.SizeChanged -= new SizeChangedEventHandler(this.OnContextMenuOrRootVisualSizeChanged);
      if (this._rootVisual != null)
        this._rootVisual.SizeChanged -= new SizeChangedEventHandler(this.OnContextMenuOrRootVisualSizeChanged);
      foreach (ApplicationBarIconButton applicationBarIconButton in this._applicationBarIconButtons)
        applicationBarIconButton.Click -= new EventHandler(this.OnEventThatClosesContextMenu);
      this._applicationBarIconButtons.Clear();
      if (this._page != null)
      {
        this._page.BackKeyPress -= new EventHandler<CancelEventArgs>(this.OnPageBackKeyPress);
        this.ClearValue(ContextMenu.ApplicationBarMirrorProperty);
        this._page = (PhoneApplicationPage) null;
      }
      this._settingIsOpen = true;
      this.IsOpen = false;
      this._settingIsOpen = false;
      this.OnClosed(new RoutedEventArgs());
    }
  }
}
