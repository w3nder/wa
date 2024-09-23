// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.ListPickerPage
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.LocalizedResources;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class ListPickerPage : PhoneApplicationPage
  {
    private const string StateKey_Value = "ListPickerPage_State_Value";
    private PageOrientation _lastOrientation;
    private IList<WeakReference> _itemsToAnimate;
    private static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof (IsOpen), typeof (bool), typeof (ListPickerPage), new PropertyMetadata((object) false, new PropertyChangedCallback(ListPickerPage.OnIsOpenChanged)));
    internal Grid MainGrid;
    internal TextBlock HeaderTitle;
    internal ListBox Picker;
    private bool _contentLoaded;

    public string HeaderText { get; set; }

    public IList Items { get; private set; }

    public SelectionMode SelectionMode { get; set; }

    public object SelectedItem { get; set; }

    public IList SelectedItems { get; private set; }

    public DataTemplate FullModeItemTemplate { get; set; }

    private bool IsOpen
    {
      get => (bool) this.GetValue(ListPickerPage.IsOpenProperty);
      set => this.SetValue(ListPickerPage.IsOpenProperty, (object) value);
    }

    private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      (o as ListPickerPage).OnIsOpenChanged();
    }

    private void OnIsOpenChanged() => this.UpdateVisualState(true);

    public ListPickerPage()
    {
      this.InitializeComponent();
      this.Items = (IList) new List<object>();
      this.SelectedItems = (IList) new List<object>();
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      this.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(this.OnOrientationChanged);
      this._lastOrientation = this.Orientation;
      if (this.ApplicationBar != null)
      {
        foreach (object button in (IEnumerable) this.ApplicationBar.Buttons)
        {
          if (button is IApplicationBarIconButton applicationBarIconButton)
          {
            if ("DONE" == applicationBarIconButton.Text)
            {
              applicationBarIconButton.Text = ControlResources.DateTimePickerDoneText;
              applicationBarIconButton.Click += new EventHandler(this.OnDoneButtonClick);
            }
            else if ("CANCEL" == applicationBarIconButton.Text)
            {
              applicationBarIconButton.Text = ControlResources.DateTimePickerCancelText;
              applicationBarIconButton.Click += new EventHandler(this.OnCancelButtonClick);
            }
          }
        }
      }
      if (this.SelectionMode == SelectionMode.Single)
        this.Picker.ScrollIntoView(this.SelectedItem);
      else if (this.SelectedItems.Count > 0)
        this.Picker.ScrollIntoView(this.SelectedItems[0]);
      this.SetupListItems(-90.0);
      PlaneProjection planeProjection = (PlaneProjection) this.HeaderTitle.Projection;
      if (planeProjection == null)
      {
        planeProjection = new PlaneProjection();
        this.HeaderTitle.Projection = (Projection) planeProjection;
      }
      planeProjection.RotationX = -90.0;
      this.Picker.Opacity = 1.0;
      this.Dispatcher.BeginInvoke((Action) (() => this.IsOpen = true));
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      this.OrientationChanged -= new EventHandler<OrientationChangedEventArgs>(this.OnOrientationChanged);
    }

    private void SetupListItems(double degree)
    {
      this._itemsToAnimate = this.Picker.GetItemsInViewPort();
      for (int index = 0; index < this._itemsToAnimate.Count; ++index)
      {
        FrameworkElement target = (FrameworkElement) this._itemsToAnimate[index].Target;
        if (target != null)
        {
          PlaneProjection planeProjection = (PlaneProjection) target.Projection;
          if (planeProjection == null)
          {
            planeProjection = new PlaneProjection();
            target.Projection = (Projection) planeProjection;
          }
          planeProjection.RotationX = degree;
        }
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      base.OnNavigatedTo(e);
      if (this.State.ContainsKey("ListPickerPage_State_Value"))
      {
        this.State.Remove("ListPickerPage_State_Value");
        if (this.NavigationService.CanGoBack)
        {
          this.NavigationService.GoBack();
          return;
        }
      }
      if (this.HeaderText != null)
        this.HeaderTitle.Text = this.HeaderText.ToUpper(CultureInfo.CurrentCulture);
      this.Picker.DataContext = (object) this.Items;
      this.Picker.SelectionMode = this.SelectionMode;
      if (this.FullModeItemTemplate != null)
        this.Picker.ItemTemplate = this.FullModeItemTemplate;
      if (this.SelectionMode == SelectionMode.Single)
      {
        this.ApplicationBar.IsVisible = false;
        this.Picker.SelectedItem = this.SelectedItem;
      }
      else
      {
        this.ApplicationBar.IsVisible = true;
        this.Picker.ItemContainerStyle = (Style) this.Resources[(object) "ListBoxItemCheckedStyle"];
        foreach (object obj in (IEnumerable) this.Items)
        {
          if (this.SelectedItems != null && this.SelectedItems.Contains(obj))
            this.Picker.SelectedItems.Add(obj);
        }
      }
    }

    private void OnDoneButtonClick(object sender, EventArgs e)
    {
      this.SelectedItem = this.Picker.SelectedItem;
      this.SelectedItems = this.Picker.SelectedItems;
      this.ClosePickerPage();
    }

    private void OnCancelButtonClick(object sender, EventArgs e)
    {
      this.SelectedItem = (object) null;
      this.SelectedItems = (IList) null;
      this.ClosePickerPage();
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      e.Cancel = true;
      this.SelectedItem = (object) null;
      this.SelectedItems = (IList) null;
      this.ClosePickerPage();
    }

    private void ClosePickerPage()
    {
      this.Picker.IsHitTestVisible = false;
      this.IsOpen = false;
    }

    private void OnClosedStoryboardCompleted(object sender, EventArgs e)
    {
      if (!this.NavigationService.CanGoBack)
        return;
      this.NavigationService.GoBack();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      base.OnNavigatedFrom(e);
      if (!e.Uri.IsExternalNavigation())
        return;
      this.State["ListPickerPage_State_Value"] = (object) "ListPickerPage_State_Value";
    }

    private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
    {
      PageOrientation orientation = e.Orientation;
      RotateTransition rotateTransition = new RotateTransition();
      if (this.MainGrid != null)
      {
        switch (orientation)
        {
          case PageOrientation.Portrait:
          case PageOrientation.PortraitUp:
            this.HeaderTitle.Margin = new Thickness(24.0, 12.0, 12.0, 12.0);
            this.Picker.Margin = new Thickness(24.0, 12.0, 0.0, 0.0);
            rotateTransition.Mode = this._lastOrientation == PageOrientation.LandscapeLeft ? RotateTransitionMode.In90Counterclockwise : RotateTransitionMode.In90Clockwise;
            break;
          case PageOrientation.Landscape:
          case PageOrientation.LandscapeLeft:
            this.HeaderTitle.Margin = new Thickness(24.0, 24.0, 0.0, 0.0);
            this.Picker.Margin = new Thickness(24.0, 24.0, 0.0, 0.0);
            rotateTransition.Mode = this._lastOrientation == PageOrientation.LandscapeRight ? RotateTransitionMode.In180Counterclockwise : RotateTransitionMode.In90Clockwise;
            break;
          case PageOrientation.LandscapeRight:
            this.HeaderTitle.Margin = new Thickness(24.0, 24.0, 0.0, 0.0);
            this.Picker.Margin = new Thickness(24.0, 24.0, 0.0, 0.0);
            rotateTransition.Mode = this._lastOrientation == PageOrientation.PortraitUp ? RotateTransitionMode.In90Counterclockwise : RotateTransitionMode.In180Clockwise;
            break;
        }
      }
      PhoneApplicationPage content = (PhoneApplicationPage) ((ContentControl) Application.Current.RootVisual).Content;
      ITransition transition = rotateTransition.GetTransition((UIElement) content);
      transition.Completed += (EventHandler) ((param0, param1) => transition.Stop());
      transition.Begin();
      this._lastOrientation = orientation;
    }

    private void UpdateVisualState(bool useTransitions)
    {
      if (useTransitions)
      {
        ScrollViewer scrollViewer = this.Picker.GetVisualChildren().OfType<ScrollViewer>().FirstOrDefault<ScrollViewer>();
        scrollViewer?.ScrollToVerticalOffset(scrollViewer.VerticalOffset);
        if (!this.IsOpen)
          this.SetupListItems(0.0);
        Storyboard storyboard1 = new Storyboard();
        Storyboard storyboard2 = this.AnimationForElement((FrameworkElement) this.HeaderTitle, 0);
        storyboard1.Children.Add((Timeline) storyboard2);
        for (int index = 0; index < this._itemsToAnimate.Count; ++index)
        {
          Storyboard storyboard3 = this.AnimationForElement((FrameworkElement) this._itemsToAnimate[index].Target, index + 1);
          storyboard1.Children.Add((Timeline) storyboard3);
        }
        if (!this.IsOpen)
          storyboard1.Completed += new EventHandler(this.OnClosedStoryboardCompleted);
        storyboard1.Begin();
      }
      else
      {
        if (this.IsOpen)
          return;
        this.OnClosedStoryboardCompleted((object) null, (EventArgs) null);
      }
    }

    private Storyboard AnimationForElement(FrameworkElement element, int index)
    {
      double num1 = 30.0;
      double num2 = this.IsOpen ? 350.0 : 250.0;
      double num3 = this.IsOpen ? -45.0 : 0.0;
      double num4 = this.IsOpen ? 0.0 : 90.0;
      ExponentialEase exponentialEase1 = new ExponentialEase();
      exponentialEase1.EasingMode = this.IsOpen ? EasingMode.EaseOut : EasingMode.EaseIn;
      exponentialEase1.Exponent = 5.0;
      ExponentialEase exponentialEase2 = exponentialEase1;
      DoubleAnimation doubleAnimation = new DoubleAnimation();
      doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(num2));
      doubleAnimation.From = new double?(num3);
      doubleAnimation.To = new double?(num4);
      doubleAnimation.EasingFunction = (IEasingFunction) exponentialEase2;
      DoubleAnimation element1 = doubleAnimation;
      Storyboard.SetTarget((Timeline) element1, (DependencyObject) element);
      Storyboard.SetTargetProperty((Timeline) element1, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationX)", new object[0]));
      Storyboard storyboard = new Storyboard();
      storyboard.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(num1 * (double) index));
      storyboard.Children.Add((Timeline) element1);
      return storyboard;
    }

    private void OnPickerTapped(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.SelectionMode != SelectionMode.Single)
        return;
      this.SelectedItem = this.Picker.SelectedItem;
      this.ClosePickerPage();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Phone.Controls.Toolkit;component/ListPicker/ListPickerPage.xaml", UriKind.Relative));
      this.MainGrid = (Grid) this.FindName("MainGrid");
      this.HeaderTitle = (TextBlock) this.FindName("HeaderTitle");
      this.Picker = (ListBox) this.FindName("Picker");
    }
  }
}
