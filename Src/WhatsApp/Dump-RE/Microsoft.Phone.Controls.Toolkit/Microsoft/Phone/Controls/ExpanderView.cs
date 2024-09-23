// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.ExpanderView
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplatePart(Name = "ExpandedStateAnimation", Type = typeof (DoubleAnimation))]
  [TemplatePart(Name = "ExpanderPanel", Type = typeof (Grid))]
  [TemplatePart(Name = "ExpandedToCollapsedKeyFrame", Type = typeof (EasingDoubleKeyFrame))]
  [TemplateVisualState(Name = "Collapsed", GroupName = "ExpansionStates")]
  [TemplatePart(Name = "CollapsedToExpandedKeyFrame", Type = typeof (EasingDoubleKeyFrame))]
  [TemplateVisualState(Name = "Expanded", GroupName = "ExpansionStates")]
  [TemplateVisualState(Name = "Expandable", GroupName = "ExpandabilityStates")]
  [TemplateVisualState(Name = "NonExpandable", GroupName = "ExpandabilityStates")]
  [TemplatePart(Name = "Presenter", Type = typeof (ItemsPresenter))]
  public class ExpanderView : HeaderedItemsControl
  {
    public const string ExpansionStates = "ExpansionStates";
    public const string ExpandabilityStates = "ExpandabilityStates";
    public const string CollapsedState = "Collapsed";
    public const string ExpandedState = "Expanded";
    public const string ExpandableState = "Expandable";
    public const string NonExpandableState = "NonExpandable";
    private const string Presenter = "Presenter";
    private const string ExpanderPanel = "ExpanderPanel";
    private const string ExpandedStateAnimation = "ExpandedStateAnimation";
    private const string CollapsedToExpandedKeyFrame = "CollapsedToExpandedKeyFrame";
    private const string ExpandedToCollapsedKeyFrame = "ExpandedToCollapsedKeyFrame";
    private const int KeyTimeStep = 35;
    private const int InitialKeyTime = 225;
    private const int FinalKeyTime = 250;
    private ItemsPresenter _presenter;
    private Canvas _itemsCanvas;
    private Grid _expanderPanel;
    private DoubleAnimation _expandedStateAnimation;
    private EasingDoubleKeyFrame _collapsedToExpandedFrame;
    private EasingDoubleKeyFrame _expandedToCollapsedFrame;
    public static readonly DependencyProperty ExpanderProperty = DependencyProperty.Register(nameof (Expander), typeof (object), typeof (ExpanderView), new PropertyMetadata((object) null, new PropertyChangedCallback(ExpanderView.OnExpanderPropertyChanged)));
    public static readonly DependencyProperty ExpanderTemplateProperty = DependencyProperty.Register(nameof (ExpanderTemplate), typeof (DataTemplate), typeof (ExpanderView), new PropertyMetadata((object) null, new PropertyChangedCallback(ExpanderView.OnExpanderTemplatePropertyChanged)));
    public static readonly DependencyProperty NonExpandableHeaderProperty = DependencyProperty.Register(nameof (NonExpandableHeader), typeof (object), typeof (ExpanderView), new PropertyMetadata((object) null, new PropertyChangedCallback(ExpanderView.OnNonExpandableHeaderPropertyChanged)));
    public static readonly DependencyProperty NonExpandableHeaderTemplateProperty = DependencyProperty.Register(nameof (NonExpandableHeaderTemplate), typeof (DataTemplate), typeof (ExpanderView), new PropertyMetadata((object) null, new PropertyChangedCallback(ExpanderView.OnNonExpandableHeaderTemplatePropertyChanged)));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof (IsExpanded), typeof (bool), typeof (ExpanderView), new PropertyMetadata((object) false, new PropertyChangedCallback(ExpanderView.OnIsExpandedPropertyChanged)));
    public static readonly DependencyProperty HasItemsProperty = DependencyProperty.Register(nameof (HasItems), typeof (bool), typeof (ExpanderView), new PropertyMetadata((object) false, (PropertyChangedCallback) null));
    public static readonly DependencyProperty IsNonExpandableProperty = DependencyProperty.Register(nameof (IsNonExpandable), typeof (bool), typeof (ExpanderView), new PropertyMetadata((object) false, new PropertyChangedCallback(ExpanderView.OnIsNonExpandablePropertyChanged)));

    public event RoutedEventHandler Expanded;

    public event RoutedEventHandler Collapsed;

    public object Expander
    {
      get => this.GetValue(ExpanderView.ExpanderProperty);
      set => this.SetValue(ExpanderView.ExpanderProperty, value);
    }

    private static void OnExpanderPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ((ExpanderView) obj).OnExpanderChanged(e.OldValue, e.NewValue);
    }

    public DataTemplate ExpanderTemplate
    {
      get => (DataTemplate) this.GetValue(ExpanderView.ExpanderTemplateProperty);
      set => this.SetValue(ExpanderView.ExpanderTemplateProperty, (object) value);
    }

    private static void OnExpanderTemplatePropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ((ExpanderView) obj).OnExpanderTemplateChanged((DataTemplate) e.OldValue, (DataTemplate) e.NewValue);
    }

    public object NonExpandableHeader
    {
      get => this.GetValue(ExpanderView.NonExpandableHeaderProperty);
      set => this.SetValue(ExpanderView.NonExpandableHeaderProperty, value);
    }

    private static void OnNonExpandableHeaderPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ((ExpanderView) obj).OnNonExpandableHeaderChanged(e.OldValue, e.NewValue);
    }

    public DataTemplate NonExpandableHeaderTemplate
    {
      get => (DataTemplate) this.GetValue(ExpanderView.NonExpandableHeaderTemplateProperty);
      set => this.SetValue(ExpanderView.NonExpandableHeaderTemplateProperty, (object) value);
    }

    private static void OnNonExpandableHeaderTemplatePropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ((ExpanderView) obj).OnNonExpandableHeaderTemplateChanged((DataTemplate) e.OldValue, (DataTemplate) e.NewValue);
    }

    public bool IsExpanded
    {
      get => (bool) this.GetValue(ExpanderView.IsExpandedProperty);
      set
      {
        if (this.IsNonExpandable)
          throw new InvalidOperationException(Microsoft.Phone.Controls.Properties.Resources.InvalidExpanderViewOperation);
        this.SetValue(ExpanderView.IsExpandedProperty, (object) value);
      }
    }

    private static void OnIsExpandedPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ExpanderView expanderView = (ExpanderView) obj;
      RoutedEventArgs e1 = new RoutedEventArgs();
      if ((bool) e.NewValue)
        expanderView.OnExpanded(e1);
      else
        expanderView.OnCollapsed(e1);
      expanderView.UpdateVisualState(true);
    }

    public bool HasItems
    {
      get => (bool) this.GetValue(ExpanderView.HasItemsProperty);
      set => this.SetValue(ExpanderView.HasItemsProperty, (object) value);
    }

    public bool IsNonExpandable
    {
      get => (bool) this.GetValue(ExpanderView.IsNonExpandableProperty);
      set => this.SetValue(ExpanderView.IsNonExpandableProperty, (object) value);
    }

    private static void OnIsNonExpandablePropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ExpanderView expanderView = (ExpanderView) obj;
      if ((bool) e.NewValue && expanderView.IsExpanded)
        expanderView.IsExpanded = false;
      expanderView.UpdateVisualState(true);
    }

    public override void OnApplyTemplate()
    {
      if (this._expanderPanel != null)
        this._expanderPanel.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.OnExpanderPanelTap);
      base.OnApplyTemplate();
      this._expanderPanel = this.GetTemplateChild("ExpanderPanel") as Grid;
      this._expandedToCollapsedFrame = this.GetTemplateChild("ExpandedToCollapsedKeyFrame") as EasingDoubleKeyFrame;
      this._collapsedToExpandedFrame = this.GetTemplateChild("CollapsedToExpandedKeyFrame") as EasingDoubleKeyFrame;
      this._itemsCanvas = this.GetTemplateChild("ItemsCanvas") as Canvas;
      if (this.GetTemplateChild("Expanded") is VisualState templateChild)
        this._expandedStateAnimation = templateChild.Storyboard.Children[0] as DoubleAnimation;
      this._presenter = this.GetTemplateChild("Presenter") as ItemsPresenter;
      if (this._presenter != null)
        this._presenter.SizeChanged += new SizeChangedEventHandler(this.OnPresenterSizeChanged);
      if (this._expanderPanel != null)
        this._expanderPanel.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.OnExpanderPanelTap);
      this.UpdateVisualState(false);
    }

    public ExpanderView()
    {
      this.DefaultStyleKey = (object) typeof (ExpanderView);
      this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this._presenter == null)
        return;
      ExpanderView parentByType = this._presenter.GetParentByType<ExpanderView>();
      Point point = parentByType.TransformToVisual((UIElement) this._presenter).Transform(new Point(0.0, 0.0));
      this._presenter.Width = parentByType.RenderSize.Width + point.X;
    }

    private void OnPresenterSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this._itemsCanvas == null || this._presenter == null || !this.IsExpanded)
        return;
      this._itemsCanvas.Height = this._presenter.DesiredSize.Height;
    }

    internal virtual void UpdateVisualState(bool useTransitions)
    {
      if (this._presenter != null)
      {
        if (this._expandedStateAnimation != null)
          this._expandedStateAnimation.To = new double?(this._presenter.DesiredSize.Height);
        if (this._collapsedToExpandedFrame != null)
          this._collapsedToExpandedFrame.Value = this._presenter.DesiredSize.Height;
        if (this._expandedToCollapsedFrame != null)
          this._expandedToCollapsedFrame.Value = this._presenter.DesiredSize.Height;
      }
      string stateName;
      if (this.IsExpanded)
      {
        stateName = "Expanded";
        if (useTransitions)
          this.AnimateContainerDropDown();
      }
      else
        stateName = "Collapsed";
      VisualStateManager.GoToState((Control) this, stateName, useTransitions);
      VisualStateManager.GoToState((Control) this, !this.IsNonExpandable ? "Expandable" : "NonExpandable", useTransitions);
    }

    private void RaiseEvent(RoutedEventHandler handler, RoutedEventArgs args)
    {
      if (handler == null)
        return;
      handler((object) this, args);
    }

    internal void AnimateContainerDropDown()
    {
      for (int index = 0; index < this.Items.Count && this.ItemContainerGenerator.ContainerFromIndex(index) is FrameworkElement target1; ++index)
      {
        Storyboard storyboard = new Storyboard();
        QuadraticEase quadraticEase = new QuadraticEase();
        quadraticEase.EasingMode = EasingMode.EaseOut;
        IEasingFunction easingFunction = (IEasingFunction) quadraticEase;
        int num1 = 225 + 35 * index;
        int num2 = 250 + 35 * index;
        TranslateTransform target = new TranslateTransform();
        target1.RenderTransform = (Transform) target;
        DoubleAnimationUsingKeyFrames element1 = new DoubleAnimationUsingKeyFrames();
        EasingDoubleKeyFrame easingDoubleKeyFrame1 = new EasingDoubleKeyFrame();
        easingDoubleKeyFrame1.EasingFunction = easingFunction;
        easingDoubleKeyFrame1.KeyTime = (KeyTime) TimeSpan.FromMilliseconds(0.0);
        easingDoubleKeyFrame1.Value = -150.0;
        EasingDoubleKeyFrame easingDoubleKeyFrame2 = new EasingDoubleKeyFrame();
        easingDoubleKeyFrame2.EasingFunction = easingFunction;
        easingDoubleKeyFrame2.KeyTime = (KeyTime) TimeSpan.FromMilliseconds((double) num1);
        easingDoubleKeyFrame2.Value = 0.0;
        EasingDoubleKeyFrame easingDoubleKeyFrame3 = new EasingDoubleKeyFrame();
        easingDoubleKeyFrame3.EasingFunction = easingFunction;
        easingDoubleKeyFrame3.KeyTime = (KeyTime) TimeSpan.FromMilliseconds((double) num2);
        easingDoubleKeyFrame3.Value = 0.0;
        element1.KeyFrames.Add((DoubleKeyFrame) easingDoubleKeyFrame1);
        element1.KeyFrames.Add((DoubleKeyFrame) easingDoubleKeyFrame2);
        element1.KeyFrames.Add((DoubleKeyFrame) easingDoubleKeyFrame3);
        Storyboard.SetTarget((Timeline) element1, (DependencyObject) target);
        Storyboard.SetTargetProperty((Timeline) element1, new PropertyPath((object) TranslateTransform.YProperty));
        storyboard.Children.Add((Timeline) element1);
        DoubleAnimationUsingKeyFrames element2 = new DoubleAnimationUsingKeyFrames();
        EasingDoubleKeyFrame easingDoubleKeyFrame4 = new EasingDoubleKeyFrame();
        easingDoubleKeyFrame4.EasingFunction = easingFunction;
        easingDoubleKeyFrame4.KeyTime = (KeyTime) TimeSpan.FromMilliseconds(0.0);
        easingDoubleKeyFrame4.Value = 0.0;
        EasingDoubleKeyFrame easingDoubleKeyFrame5 = new EasingDoubleKeyFrame();
        easingDoubleKeyFrame5.EasingFunction = easingFunction;
        easingDoubleKeyFrame5.KeyTime = (KeyTime) TimeSpan.FromMilliseconds((double) (num1 - 150));
        easingDoubleKeyFrame5.Value = 0.0;
        EasingDoubleKeyFrame easingDoubleKeyFrame6 = new EasingDoubleKeyFrame();
        easingDoubleKeyFrame6.EasingFunction = easingFunction;
        easingDoubleKeyFrame6.KeyTime = (KeyTime) TimeSpan.FromMilliseconds((double) num2);
        easingDoubleKeyFrame6.Value = 1.0;
        element2.KeyFrames.Add((DoubleKeyFrame) easingDoubleKeyFrame4);
        element2.KeyFrames.Add((DoubleKeyFrame) easingDoubleKeyFrame5);
        element2.KeyFrames.Add((DoubleKeyFrame) easingDoubleKeyFrame6);
        Storyboard.SetTarget((Timeline) element2, (DependencyObject) target1);
        Storyboard.SetTargetProperty((Timeline) element2, new PropertyPath((object) UIElement.OpacityProperty));
        storyboard.Children.Add((Timeline) element2);
        storyboard.Begin();
      }
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);
      this.HasItems = this.Items.Count > 0;
    }

    private void OnExpanderPanelTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.IsNonExpandable)
        return;
      this.IsExpanded = !this.IsExpanded;
    }

    protected virtual void OnExpanderChanged(object oldExpander, object newExpander)
    {
    }

    protected virtual void OnExpanderTemplateChanged(
      DataTemplate oldTemplate,
      DataTemplate newTemplate)
    {
    }

    protected virtual void OnNonExpandableHeaderChanged(object oldHeader, object newHeader)
    {
    }

    protected virtual void OnNonExpandableHeaderTemplateChanged(
      DataTemplate oldTemplate,
      DataTemplate newTemplate)
    {
    }

    protected virtual void OnExpanded(RoutedEventArgs e) => this.RaiseEvent(this.Expanded, e);

    protected virtual void OnCollapsed(RoutedEventArgs e) => this.RaiseEvent(this.Collapsed, e);
  }
}
