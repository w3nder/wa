// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.ListPicker
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplateVisualState(GroupName = "PickerStates", Name = "Normal")]
  [TemplateVisualState(GroupName = "PickerStates", Name = "Highlighted")]
  [TemplateVisualState(GroupName = "PickerStates", Name = "Disabled")]
  [TemplatePart(Name = "MultipleSelectionModeSummary", Type = typeof (TextBlock))]
  [TemplatePart(Name = "ItemsPresenterHost", Type = typeof (Canvas))]
  [TemplatePart(Name = "ItemsPresenter", Type = typeof (ItemsPresenter))]
  [TemplatePart(Name = "ItemsPresenterTranslateTransform", Type = typeof (TranslateTransform))]
  public class ListPicker : ItemsControl
  {
    private const string ItemsPresenterPartName = "ItemsPresenter";
    private const string ItemsPresenterTranslateTransformPartName = "ItemsPresenterTranslateTransform";
    private const string ItemsPresenterHostPartName = "ItemsPresenterHost";
    private const string MultipleSelectionModeSummaryPartName = "MultipleSelectionModeSummary";
    private const string BorderPartName = "Border";
    private const string PickerStatesGroupName = "PickerStates";
    private const string PickerStatesNormalStateName = "Normal";
    private const string PickerStatesHighlightedStateName = "Highlighted";
    private const string PickerStatesDisabledStateName = "Disabled";
    private const double NormalModeOffset = 4.0;
    private readonly DoubleAnimation _heightAnimation = new DoubleAnimation();
    private readonly DoubleAnimation _translateAnimation = new DoubleAnimation();
    private readonly Storyboard _storyboard = new Storyboard();
    private PhoneApplicationFrame _frame;
    private PhoneApplicationPage _page;
    private FrameworkElement _itemsPresenterHostParent;
    private Canvas _itemsPresenterHostPart;
    private ItemsPresenter _itemsPresenterPart;
    private TranslateTransform _itemsPresenterTranslateTransformPart;
    private bool _updatingSelection;
    private int _deferredSelectedIndex = -1;
    private object _deferredSelectedItem;
    private object _frameContentWhenOpened;
    private NavigationInTransition _savedNavigationInTransition;
    private NavigationOutTransition _savedNavigationOutTransition;
    private ListPickerPage _listPickerPage;
    private TextBlock _multipleSelectionModeSummary;
    private Border _border;
    private bool _hasPickerPageOpen;
    public static readonly DependencyProperty SummaryForSelectedItemsDelegateProperty = DependencyProperty.Register(nameof (SummaryForSelectedItemsDelegate), typeof (Func<IList, string>), typeof (ListPicker), (PropertyMetadata) null);
    public static readonly DependencyProperty ListPickerModeProperty = DependencyProperty.Register(nameof (ListPickerMode), typeof (ListPickerMode), typeof (ListPicker), new PropertyMetadata((object) ListPickerMode.Normal, new PropertyChangedCallback(ListPicker.OnListPickerModeChanged)));
    private static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(nameof (IsHighlighted), typeof (bool), typeof (ListPicker), new PropertyMetadata((object) false, new PropertyChangedCallback(ListPicker.OnIsHighlightedChanged)));
    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof (SelectedIndex), typeof (int), typeof (ListPicker), new PropertyMetadata((object) -1, new PropertyChangedCallback(ListPicker.OnSelectedIndexChanged)));
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof (SelectedItem), typeof (object), typeof (ListPicker), new PropertyMetadata((object) null, new PropertyChangedCallback(ListPicker.OnSelectedItemChanged)));
    private static readonly DependencyProperty ShadowItemTemplateProperty = DependencyProperty.Register("ShadowItemTemplate", typeof (DataTemplate), typeof (ListPicker), new PropertyMetadata((object) null, new PropertyChangedCallback(ListPicker.OnShadowOrFullModeItemTemplateChanged)));
    public static readonly DependencyProperty FullModeItemTemplateProperty = DependencyProperty.Register(nameof (FullModeItemTemplate), typeof (DataTemplate), typeof (ListPicker), new PropertyMetadata((object) null, new PropertyChangedCallback(ListPicker.OnShadowOrFullModeItemTemplateChanged)));
    private static readonly DependencyProperty ActualFullModeItemTemplateProperty = DependencyProperty.Register("ActualFullModeItemTemplate", typeof (DataTemplate), typeof (ListPicker), (PropertyMetadata) null);
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof (Header), typeof (object), typeof (ListPicker), (PropertyMetadata) null);
    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof (HeaderTemplate), typeof (DataTemplate), typeof (ListPicker), (PropertyMetadata) null);
    public static readonly DependencyProperty FullModeHeaderProperty = DependencyProperty.Register(nameof (FullModeHeader), typeof (object), typeof (ListPicker), (PropertyMetadata) null);
    public static readonly DependencyProperty ItemCountThresholdProperty = DependencyProperty.Register(nameof (ItemCountThreshold), typeof (int), typeof (ListPicker), new PropertyMetadata((object) 5, new PropertyChangedCallback(ListPicker.OnItemCountThresholdChanged)));
    public static readonly DependencyProperty PickerPageUriProperty = DependencyProperty.Register(nameof (PickerPageUri), typeof (Uri), typeof (ListPicker), (PropertyMetadata) null);
    public static readonly DependencyProperty ExpansionModeProperty = DependencyProperty.Register(nameof (ExpansionMode), typeof (ExpansionMode), typeof (ListPicker), new PropertyMetadata((object) ExpansionMode.ExpansionAllowed, (PropertyChangedCallback) null));
    public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof (SelectionMode), typeof (SelectionMode), typeof (ListPicker), new PropertyMetadata((object) SelectionMode.Single, new PropertyChangedCallback(ListPicker.OnSelectionModeChanged)));
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof (SelectedItems), typeof (IList), typeof (ListPicker), new PropertyMetadata(new PropertyChangedCallback(ListPicker.OnSelectedItemsChanged)));

    public event SelectionChangedEventHandler SelectionChanged;

    public Func<IList, string> SummaryForSelectedItemsDelegate
    {
      get
      {
        return (Func<IList, string>) this.GetValue(ListPicker.SummaryForSelectedItemsDelegateProperty);
      }
      set => this.SetValue(ListPicker.SummaryForSelectedItemsDelegateProperty, (object) value);
    }

    public ListPickerMode ListPickerMode
    {
      get => (ListPickerMode) this.GetValue(ListPicker.ListPickerModeProperty);
      private set => this.SetValue(ListPicker.ListPickerModeProperty, (object) value);
    }

    private static void OnListPickerModeChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((ListPicker) o).OnListPickerModeChanged((ListPickerMode) e.OldValue, (ListPickerMode) e.NewValue);
    }

    private void OnListPickerModeChanged(ListPickerMode oldValue, ListPickerMode newValue)
    {
      if (ListPickerMode.Expanded == oldValue)
      {
        if (this._page != null)
        {
          this._page.BackKeyPress -= new EventHandler<CancelEventArgs>(this.OnPageBackKeyPress);
          this._page = (PhoneApplicationPage) null;
        }
        if (this._frame != null)
        {
          this._frame.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(this.OnFrameManipulationStarted);
          this._frame = (PhoneApplicationFrame) null;
        }
      }
      if (ListPickerMode.Expanded == newValue)
      {
        if (this._frame == null)
        {
          this._frame = Application.Current.RootVisual as PhoneApplicationFrame;
          if (this._frame != null)
            this._frame.AddHandler(UIElement.ManipulationStartedEvent, (Delegate) new EventHandler<ManipulationStartedEventArgs>(this.OnFrameManipulationStarted), true);
        }
        if (this._frame != null)
        {
          this._page = this._frame.Content as PhoneApplicationPage;
          if (this._page != null)
            this._page.BackKeyPress += new EventHandler<CancelEventArgs>(this.OnPageBackKeyPress);
        }
      }
      if (ListPickerMode.Full == oldValue)
        this.ClosePickerPage();
      if (ListPickerMode.Full == newValue)
        this.OpenPickerPage();
      this.SizeForAppropriateView(ListPickerMode.Full != oldValue);
      this.IsHighlighted = ListPickerMode.Expanded == newValue;
    }

    private bool IsHighlighted
    {
      get => (bool) this.GetValue(ListPicker.IsHighlightedProperty);
      set => this.SetValue(ListPicker.IsHighlightedProperty, (object) value);
    }

    private static void OnIsHighlightedChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      (o as ListPicker).OnIsHighlightedChanged();
    }

    private void OnIsHighlightedChanged() => this.UpdateVisualStates(true);

    private static void OnIsEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      (o as ListPicker).OnIsEnabledChanged();
    }

    private void OnIsEnabledChanged() => this.UpdateVisualStates(true);

    public int SelectedIndex
    {
      get => (int) this.GetValue(ListPicker.SelectedIndexProperty);
      set => this.SetValue(ListPicker.SelectedIndexProperty, (object) value);
    }

    private static void OnSelectedIndexChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((ListPicker) o).OnSelectedIndexChanged((int) e.OldValue, (int) e.NewValue);
    }

    private void OnSelectedIndexChanged(int oldValue, int newValue)
    {
      if (this.Items.Count <= newValue || 0 < this.Items.Count && newValue < 0 || this.Items.Count == 0 && newValue != -1)
      {
        this._deferredSelectedIndex = this.Template == null && 0 <= newValue ? newValue : throw new InvalidOperationException(Microsoft.Phone.Controls.Properties.Resources.InvalidSelectedIndex);
      }
      else
      {
        if (!this._updatingSelection)
        {
          this._updatingSelection = true;
          this.SelectedItem = -1 != newValue ? this.Items[newValue] : (object) null;
          this._updatingSelection = false;
        }
        if (-1 == oldValue)
          return;
        ListPickerItem listPickerItem = (ListPickerItem) this.ItemContainerGenerator.ContainerFromIndex(oldValue);
        if (listPickerItem == null)
          return;
        listPickerItem.IsSelected = false;
      }
    }

    public object SelectedItem
    {
      get => this.GetValue(ListPicker.SelectedItemProperty);
      set => this.SetValue(ListPicker.SelectedItemProperty, value);
    }

    private static void OnSelectedItemChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((ListPicker) o).OnSelectedItemChanged(e.OldValue, e.NewValue);
    }

    private void OnSelectedItemChanged(object oldValue, object newValue)
    {
      if (newValue != null && (this.Items == null || this.Items.Count == 0))
      {
        if (this.Template != null)
          throw new InvalidOperationException(Microsoft.Phone.Controls.Properties.Resources.InvalidSelectedItem);
        this._deferredSelectedItem = newValue;
      }
      else
      {
        int num = newValue != null ? this.Items.IndexOf(newValue) : -1;
        if (-1 == num && 0 < this.Items.Count)
          throw new InvalidOperationException(Microsoft.Phone.Controls.Properties.Resources.InvalidSelectedItem);
        if (!this._updatingSelection)
        {
          this._updatingSelection = true;
          this.SelectedIndex = num;
          this._updatingSelection = false;
        }
        if (this.ListPickerMode != ListPickerMode.Normal)
          this.ListPickerMode = ListPickerMode.Normal;
        else
          this.SizeForAppropriateView(false);
        SelectionChangedEventHandler selectionChanged = this.SelectionChanged;
        if (selectionChanged == null)
          return;
        object[] objArray1;
        if (oldValue != null)
          objArray1 = new object[1]{ oldValue };
        else
          objArray1 = new object[0];
        IList removedItems = (IList) objArray1;
        object[] objArray2;
        if (newValue != null)
          objArray2 = new object[1]{ newValue };
        else
          objArray2 = new object[0];
        IList addedItems = (IList) objArray2;
        selectionChanged((object) this, new SelectionChangedEventArgs(removedItems, addedItems));
      }
    }

    public DataTemplate FullModeItemTemplate
    {
      get => (DataTemplate) this.GetValue(ListPicker.FullModeItemTemplateProperty);
      set => this.SetValue(ListPicker.FullModeItemTemplateProperty, (object) value);
    }

    private static void OnShadowOrFullModeItemTemplateChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((ListPicker) o).OnShadowOrFullModeItemTemplateChanged();
    }

    private void OnShadowOrFullModeItemTemplateChanged()
    {
      this.SetValue(ListPicker.ActualFullModeItemTemplateProperty, (object) (this.FullModeItemTemplate ?? this.ItemTemplate));
    }

    public object Header
    {
      get => this.GetValue(ListPicker.HeaderProperty);
      set => this.SetValue(ListPicker.HeaderProperty, value);
    }

    public DataTemplate HeaderTemplate
    {
      get => (DataTemplate) this.GetValue(ListPicker.HeaderTemplateProperty);
      set => this.SetValue(ListPicker.HeaderTemplateProperty, (object) value);
    }

    public object FullModeHeader
    {
      get => this.GetValue(ListPicker.FullModeHeaderProperty);
      set => this.SetValue(ListPicker.FullModeHeaderProperty, value);
    }

    public int ItemCountThreshold
    {
      get => (int) this.GetValue(ListPicker.ItemCountThresholdProperty);
      private set => this.SetValue(ListPicker.ItemCountThresholdProperty, (object) value);
    }

    private static void OnItemCountThresholdChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((ListPicker) o).OnItemCountThresholdChanged((int) e.NewValue);
    }

    private void OnItemCountThresholdChanged(int newValue)
    {
      if (newValue < 0)
        throw new ArgumentOutOfRangeException("ItemCountThreshold");
    }

    public Uri PickerPageUri
    {
      get => (Uri) this.GetValue(ListPicker.PickerPageUriProperty);
      set => this.SetValue(ListPicker.PickerPageUriProperty, (object) value);
    }

    public ExpansionMode ExpansionMode
    {
      get => (ExpansionMode) this.GetValue(ListPicker.ExpansionModeProperty);
      set => this.SetValue(ListPicker.ExpansionModeProperty, (object) value);
    }

    public SelectionMode SelectionMode
    {
      get => (SelectionMode) this.GetValue(ListPicker.SelectionModeProperty);
      set => this.SetValue(ListPicker.SelectionModeProperty, (object) value);
    }

    private static void OnSelectionModeChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((ListPicker) o).OnSelectionModeChanged((SelectionMode) e.NewValue);
    }

    private void OnSelectionModeChanged(SelectionMode newValue)
    {
      if (newValue == SelectionMode.Multiple || newValue == SelectionMode.Extended)
      {
        if (this._multipleSelectionModeSummary == null || this._itemsPresenterHostPart == null)
          return;
        this._multipleSelectionModeSummary.Visibility = Visibility.Visible;
        this._itemsPresenterHostPart.Visibility = Visibility.Collapsed;
      }
      else
      {
        if (this._multipleSelectionModeSummary == null || this._itemsPresenterHostPart == null)
          return;
        this._multipleSelectionModeSummary.Visibility = Visibility.Collapsed;
        this._itemsPresenterHostPart.Visibility = Visibility.Visible;
      }
    }

    public IList SelectedItems
    {
      get => (IList) this.GetValue(ListPicker.SelectedItemsProperty);
      set => this.SetValue(ListPicker.SelectedItemsProperty, (object) value);
    }

    private static void OnSelectedItemsChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((ListPicker) o).OnSelectedItemsChanged((IList) e.OldValue, (IList) e.NewValue);
    }

    private void OnSelectedItemsChanged(IList oldValue, IList newValue)
    {
      this.UpdateSummary(newValue);
      SelectionChangedEventHandler selectionChanged = this.SelectionChanged;
      if (selectionChanged == null)
        return;
      IList removedItems = (IList) new List<object>();
      if (oldValue != null)
      {
        foreach (object obj in (IEnumerable) oldValue)
        {
          if (newValue == null || !newValue.Contains(obj))
            removedItems.Add(obj);
        }
      }
      IList addedItems = (IList) new List<object>();
      if (newValue != null)
      {
        foreach (object obj in (IEnumerable) newValue)
        {
          if (oldValue == null || !oldValue.Contains(obj))
            addedItems.Add(obj);
        }
      }
      selectionChanged((object) this, new SelectionChangedEventArgs(removedItems, addedItems));
    }

    public ListPicker()
    {
      this.DefaultStyleKey = (object) typeof (ListPicker);
      Storyboard.SetTargetProperty((Timeline) this._heightAnimation, new PropertyPath((object) FrameworkElement.HeightProperty));
      Storyboard.SetTargetProperty((Timeline) this._translateAnimation, new PropertyPath((object) TranslateTransform.YProperty));
      Duration duration = (Duration) TimeSpan.FromSeconds(0.2);
      this._heightAnimation.Duration = duration;
      this._translateAnimation.Duration = duration;
      ExponentialEase exponentialEase = new ExponentialEase();
      exponentialEase.EasingMode = EasingMode.EaseInOut;
      exponentialEase.Exponent = 4.0;
      IEasingFunction easingFunction = (IEasingFunction) exponentialEase;
      this._heightAnimation.EasingFunction = easingFunction;
      this._translateAnimation.EasingFunction = easingFunction;
      this.RegisterNotification("IsEnabled", new PropertyChangedCallback(ListPicker.OnIsEnabledChanged));
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
      if (!(Application.Current.RootVisual is FrameworkElement rootVisual))
        return;
      this.FlowDirection = rootVisual.FlowDirection;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => this.UpdateVisualStates(true);

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      if (this._frame == null)
        return;
      this._frame.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(this.OnFrameManipulationStarted);
      this._frame = (PhoneApplicationFrame) null;
    }

    public override void OnApplyTemplate()
    {
      if (this._itemsPresenterHostParent != null)
        this._itemsPresenterHostParent.SizeChanged -= new SizeChangedEventHandler(this.OnItemsPresenterHostParentSizeChanged);
      this._storyboard.Stop();
      base.OnApplyTemplate();
      this._itemsPresenterPart = this.GetTemplateChild("ItemsPresenter") as ItemsPresenter;
      this._itemsPresenterTranslateTransformPart = this.GetTemplateChild("ItemsPresenterTranslateTransform") as TranslateTransform;
      this._itemsPresenterHostPart = this.GetTemplateChild("ItemsPresenterHost") as Canvas;
      this._itemsPresenterHostParent = this._itemsPresenterHostPart != null ? this._itemsPresenterHostPart.Parent as FrameworkElement : (FrameworkElement) null;
      this._multipleSelectionModeSummary = this.GetTemplateChild("MultipleSelectionModeSummary") as TextBlock;
      this._border = this.GetTemplateChild("Border") as Border;
      if (this._itemsPresenterHostParent != null)
        this._itemsPresenterHostParent.SizeChanged += new SizeChangedEventHandler(this.OnItemsPresenterHostParentSizeChanged);
      if (this._itemsPresenterHostPart != null)
      {
        Storyboard.SetTarget((Timeline) this._heightAnimation, (DependencyObject) this._itemsPresenterHostPart);
        if (!this._storyboard.Children.Contains((Timeline) this._heightAnimation))
          this._storyboard.Children.Add((Timeline) this._heightAnimation);
      }
      else if (this._storyboard.Children.Contains((Timeline) this._heightAnimation))
        this._storyboard.Children.Remove((Timeline) this._heightAnimation);
      if (this._itemsPresenterTranslateTransformPart != null)
      {
        Storyboard.SetTarget((Timeline) this._translateAnimation, (DependencyObject) this._itemsPresenterTranslateTransformPart);
        if (!this._storyboard.Children.Contains((Timeline) this._translateAnimation))
          this._storyboard.Children.Add((Timeline) this._translateAnimation);
      }
      else if (this._storyboard.Children.Contains((Timeline) this._translateAnimation))
        this._storyboard.Children.Remove((Timeline) this._translateAnimation);
      this.SetBinding(ListPicker.ShadowItemTemplateProperty, new Binding("ItemTemplate")
      {
        Source = (object) this
      });
      if (-1 != this._deferredSelectedIndex)
      {
        this.SelectedIndex = this._deferredSelectedIndex;
        this._deferredSelectedIndex = -1;
      }
      if (this._deferredSelectedItem != null)
      {
        this.SelectedItem = this._deferredSelectedItem;
        this._deferredSelectedItem = (object) null;
      }
      this.OnSelectionModeChanged(this.SelectionMode);
      this.OnSelectedItemsChanged(this.SelectedItems, this.SelectedItems);
    }

    protected override bool IsItemItsOwnContainerOverride(object item) => item is ListPickerItem;

    protected override DependencyObject GetContainerForItemOverride()
    {
      return (DependencyObject) new ListPickerItem();
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);
      ContentControl contentControl = (ContentControl) element;
      contentControl.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.OnContainerTap);
      contentControl.SizeChanged += new SizeChangedEventHandler(this.OnListPickerItemSizeChanged);
      if (!object.Equals(item, this.SelectedItem))
        return;
      this.SizeForAppropriateView(false);
    }

    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
      base.ClearContainerForItemOverride(element, item);
      ContentControl contentControl = (ContentControl) element;
      contentControl.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.OnContainerTap);
      contentControl.SizeChanged -= new SizeChangedEventHandler(this.OnListPickerItemSizeChanged);
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);
      if (0 < this.Items.Count && this.SelectedItem == null)
      {
        if (this.GetBindingExpression(ListPicker.SelectedIndexProperty) == null && this.GetBindingExpression(ListPicker.SelectedItemProperty) == null)
          this.SelectedIndex = 0;
      }
      else if (this.Items.Count == 0)
      {
        this.SelectedIndex = -1;
        this.ListPickerMode = ListPickerMode.Normal;
      }
      else if (this.Items.Count <= this.SelectedIndex)
        this.SelectedIndex = this.Items.Count - 1;
      else if (!object.Equals(this.Items[this.SelectedIndex], this.SelectedItem))
      {
        int num = this.Items.IndexOf(this.SelectedItem);
        if (-1 == num)
          this.SelectedItem = this.Items[0];
        else
          this.SelectedIndex = num;
      }
      this.Dispatcher.BeginInvoke((Action) (() => this.SizeForAppropriateView(false)));
    }

    private bool IsValidManipulation(object OriginalSource, Point p)
    {
      for (DependencyObject reference = OriginalSource as DependencyObject; reference != null; reference = VisualTreeHelper.GetParent(reference))
      {
        if (this._itemsPresenterHostPart == reference || this._multipleSelectionModeSummary == reference || this._border == reference)
        {
          double num = 11.0;
          return p.X > 0.0 && p.Y > 0.0 - num && p.X < this._border.RenderSize.Width && p.Y < this._border.RenderSize.Height + num;
        }
      }
      return false;
    }

    protected override void OnTap(System.Windows.Input.GestureEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      if (this.ListPickerMode != ListPickerMode.Normal)
        return;
      if (!this.IsEnabled)
      {
        e.Handled = true;
      }
      else
      {
        Point position = e.GetPosition((UIElement) e.OriginalSource);
        if (!this.IsValidManipulation(e.OriginalSource, position) || !this.Open())
          return;
        e.Handled = true;
      }
    }

    protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      base.OnManipulationStarted(e);
      if (this.ListPickerMode != ListPickerMode.Normal)
        return;
      if (!this.IsEnabled)
      {
        e.Complete();
      }
      else
      {
        Point point = e.ManipulationOrigin;
        if (e.OriginalSource != e.ManipulationContainer)
          point = e.ManipulationContainer.TransformToVisual((UIElement) e.OriginalSource).Transform(point);
        if (!this.IsValidManipulation(e.OriginalSource, point))
          return;
        this.IsHighlighted = true;
      }
    }

    protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      base.OnManipulationDelta(e);
      if (this.ListPickerMode != ListPickerMode.Normal)
        return;
      if (!this.IsEnabled)
      {
        e.Complete();
      }
      else
      {
        Point point = e.ManipulationOrigin;
        if (e.OriginalSource != e.ManipulationContainer)
          point = e.ManipulationContainer.TransformToVisual((UIElement) e.OriginalSource).Transform(point);
        if (this.IsValidManipulation(e.OriginalSource, point))
          return;
        this.IsHighlighted = false;
        e.Complete();
      }
    }

    protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      base.OnManipulationCompleted(e);
      if (!this.IsEnabled || this.ListPickerMode != ListPickerMode.Normal)
        return;
      this.IsHighlighted = false;
    }

    public bool Open()
    {
      if (this.SelectionMode == SelectionMode.Single)
      {
        if (this.ListPickerMode != ListPickerMode.Normal)
          return false;
        this.ListPickerMode = this.ExpansionMode != ExpansionMode.ExpansionAllowed || this.Items.Count > this.ItemCountThreshold ? ListPickerMode.Full : ListPickerMode.Expanded;
        return true;
      }
      this.ListPickerMode = ListPickerMode.Full;
      return true;
    }

    private void OnItemsPresenterHostParentSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this._itemsPresenterPart != null && this._itemsPresenterHostPart != null && (e.NewSize.Width != e.PreviousSize.Width || e.NewSize.Width == 0.0))
        this.UpdateItemsPresenterWidth(e.NewSize.Width);
      this._itemsPresenterHostParent.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(new Point(), e.NewSize)
      };
    }

    private void UpdateItemsPresenterWidth(double availableWidth)
    {
      this._itemsPresenterPart.Width = this._itemsPresenterHostPart.Width = double.NaN;
      this._itemsPresenterPart.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      if (double.IsNaN(this.Width) && this.HorizontalAlignment != HorizontalAlignment.Stretch)
        this._itemsPresenterHostPart.Width = this._itemsPresenterPart.DesiredSize.Width;
      if (availableWidth <= this._itemsPresenterPart.DesiredSize.Width)
        return;
      this._itemsPresenterPart.Width = availableWidth;
    }

    private void OnListPickerItemSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (object.Equals(this.ItemContainerGenerator.ItemFromContainer((DependencyObject) sender), this.SelectedItem))
        this.SizeForAppropriateView(false);
      if (!double.IsNaN(this.Width) || this.HorizontalAlignment == HorizontalAlignment.Stretch)
        return;
      this._itemsPresenterHostPart.Width = this._itemsPresenterPart.DesiredSize.Width;
    }

    private void OnPageBackKeyPress(object sender, CancelEventArgs e)
    {
      this.ListPickerMode = ListPickerMode.Normal;
      e.Cancel = true;
    }

    private void SizeForAppropriateView(bool animate)
    {
      switch (this.ListPickerMode)
      {
        case ListPickerMode.Normal:
          this.SizeForNormalMode(animate);
          break;
        case ListPickerMode.Expanded:
          this.SizeForExpandedMode();
          break;
        case ListPickerMode.Full:
          return;
      }
      this._storyboard.Begin();
      if (animate)
        return;
      this._storyboard.SkipToFill();
    }

    private void SizeForNormalMode(bool animate)
    {
      ContentControl element = (ContentControl) this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem);
      if (element != null)
      {
        if (0.0 < element.ActualHeight)
          this.SetContentHeight(element.ActualHeight + element.Margin.Top + element.Margin.Bottom - 8.0);
        if (this._itemsPresenterTranslateTransformPart != null)
        {
          if (!animate)
            this._itemsPresenterTranslateTransformPart.Y = -4.0;
          this._translateAnimation.To = new double?(element.Margin.Top - LayoutInformation.GetLayoutSlot((FrameworkElement) element).Top - 4.0);
          this._translateAnimation.From = animate ? new double?() : this._translateAnimation.To;
        }
      }
      else
        this.SetContentHeight(0.0);
      ListPickerItem listPickerItem = (ListPickerItem) this.ItemContainerGenerator.ContainerFromIndex(this.SelectedIndex);
      if (listPickerItem == null)
        return;
      listPickerItem.IsSelected = false;
    }

    private void SizeForExpandedMode()
    {
      if (this._itemsPresenterPart != null)
        this.SetContentHeight(this._itemsPresenterPart.ActualHeight);
      if (this._itemsPresenterTranslateTransformPart != null)
        this._translateAnimation.To = new double?(0.0);
      ListPickerItem listPickerItem = (ListPickerItem) this.ItemContainerGenerator.ContainerFromIndex(this.SelectedIndex);
      if (listPickerItem == null)
        return;
      listPickerItem.IsSelected = true;
    }

    private void SetContentHeight(double height)
    {
      if (this._itemsPresenterHostPart == null || double.IsNaN(height))
        return;
      double height1 = this._itemsPresenterHostPart.Height;
      this._heightAnimation.From = new double?(double.IsNaN(height1) ? height : height1);
      this._heightAnimation.To = new double?(height);
    }

    private void OnFrameManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      if (ListPickerMode.Expanded != this.ListPickerMode)
        return;
      DependencyObject reference = e.OriginalSource as DependencyObject;
      DependencyObject dependencyObject = (DependencyObject) ((FrameworkElement) this._itemsPresenterHostPart ?? (FrameworkElement) this);
      for (; reference != null; reference = VisualTreeHelper.GetParent(reference))
      {
        if (dependencyObject == reference)
          return;
      }
      this.ListPickerMode = ListPickerMode.Normal;
    }

    private void OnContainerTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (ListPickerMode.Expanded != this.ListPickerMode)
        return;
      this.SelectedItem = this.ItemContainerGenerator.ItemFromContainer((DependencyObject) sender);
      this.ListPickerMode = ListPickerMode.Normal;
      e.Handled = true;
    }

    private void UpdateVisualStates(bool useTransitions)
    {
      if (!this.IsEnabled)
        VisualStateManager.GoToState((Control) this, "Disabled", useTransitions);
      else if (this.IsHighlighted)
        VisualStateManager.GoToState((Control) this, "Highlighted", useTransitions);
      else
        VisualStateManager.GoToState((Control) this, "Normal", useTransitions);
    }

    private void UpdateSummary(IList newValue)
    {
      string str = (string) null;
      if (this.SummaryForSelectedItemsDelegate != null)
        str = this.SummaryForSelectedItemsDelegate(newValue);
      if (str == null)
        str = newValue == null || newValue.Count == 0 ? " " : newValue[0].ToString();
      if (string.IsNullOrEmpty(str))
        str = " ";
      if (this._multipleSelectionModeSummary == null)
        return;
      this._multipleSelectionModeSummary.Text = str;
    }

    private void OpenPickerPage()
    {
      if ((Uri) null == this.PickerPageUri)
        throw new ArgumentException("PickerPageUri");
      if (this._frame != null)
        return;
      this._frame = Application.Current.RootVisual as PhoneApplicationFrame;
      if (this._frame == null)
        return;
      this._frameContentWhenOpened = this._frame.Content;
      if (this._frameContentWhenOpened is UIElement contentWhenOpened)
      {
        this._savedNavigationInTransition = TransitionService.GetNavigationInTransition(contentWhenOpened);
        TransitionService.SetNavigationInTransition(contentWhenOpened, (NavigationInTransition) null);
        this._savedNavigationOutTransition = TransitionService.GetNavigationOutTransition(contentWhenOpened);
        TransitionService.SetNavigationOutTransition(contentWhenOpened, (NavigationOutTransition) null);
      }
      this._frame.Navigated += new NavigatedEventHandler(this.OnFrameNavigated);
      this._frame.NavigationStopped += new NavigationStoppedEventHandler(this.OnFrameNavigationStoppedOrFailed);
      this._frame.NavigationFailed += new NavigationFailedEventHandler(this.OnFrameNavigationStoppedOrFailed);
      this._hasPickerPageOpen = true;
      this._frame.Navigate(this.PickerPageUri);
    }

    private void ClosePickerPage()
    {
      if (this._frame == null)
      {
        this._frame = Application.Current.RootVisual as PhoneApplicationFrame;
        if (this._frame != null)
        {
          this._frame.Navigated -= new NavigatedEventHandler(this.OnFrameNavigated);
          this._frame.NavigationStopped -= new NavigationStoppedEventHandler(this.OnFrameNavigationStoppedOrFailed);
          this._frame.NavigationFailed -= new NavigationFailedEventHandler(this.OnFrameNavigationStoppedOrFailed);
          if (this._frameContentWhenOpened is UIElement contentWhenOpened)
          {
            TransitionService.SetNavigationInTransition(contentWhenOpened, this._savedNavigationInTransition);
            this._savedNavigationInTransition = (NavigationInTransition) null;
            TransitionService.SetNavigationOutTransition(contentWhenOpened, this._savedNavigationOutTransition);
            this._savedNavigationOutTransition = (NavigationOutTransition) null;
          }
          this._frame = (PhoneApplicationFrame) null;
          this._frameContentWhenOpened = (object) null;
        }
      }
      if (this._listPickerPage == null)
        return;
      if (this.SelectionMode == SelectionMode.Single && this._listPickerPage.SelectedItem != null)
        this.SelectedItem = this._listPickerPage.SelectedItem;
      else if ((this.SelectionMode == SelectionMode.Multiple || this.SelectionMode == SelectionMode.Extended) && this._listPickerPage.SelectedItems != null)
        this.SelectedItems = this._listPickerPage.SelectedItems;
      this._listPickerPage = (ListPickerPage) null;
    }

    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
      if (e.Content == this._frameContentWhenOpened)
      {
        this.ListPickerMode = ListPickerMode.Normal;
      }
      else
      {
        if (this._listPickerPage != null || !this._hasPickerPageOpen)
          return;
        this._hasPickerPageOpen = false;
        this._listPickerPage = e.Content as ListPickerPage;
        if (this._listPickerPage == null)
          return;
        this._listPickerPage.FlowDirection = this.FlowDirection;
        this._listPickerPage.HeaderText = this.FullModeHeader == null ? (string) this.Header : (string) this.FullModeHeader;
        this._listPickerPage.FullModeItemTemplate = this.FullModeItemTemplate;
        this._listPickerPage.Items.Clear();
        if (this.Items != null)
        {
          foreach (object obj in (PresentationFrameworkCollection<object>) this.Items)
            this._listPickerPage.Items.Add(obj);
        }
        this._listPickerPage.SelectionMode = this.SelectionMode;
        if (this.SelectionMode == SelectionMode.Single)
        {
          this._listPickerPage.SelectedItem = this.SelectedItem;
        }
        else
        {
          this._listPickerPage.SelectedItems.Clear();
          if (this.SelectedItems == null)
            return;
          foreach (object selectedItem in (IEnumerable) this.SelectedItems)
            this._listPickerPage.SelectedItems.Add(selectedItem);
        }
      }
    }

    private void OnFrameNavigationStoppedOrFailed(object sender, EventArgs e)
    {
      this.ListPickerMode = ListPickerMode.Normal;
    }
  }
}
