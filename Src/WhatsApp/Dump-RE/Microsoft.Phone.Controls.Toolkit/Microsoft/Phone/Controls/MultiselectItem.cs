// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.MultiselectItem
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplatePart(Name = "InfoPresenter", Type = typeof (ContentControl))]
  [TemplateVisualState(Name = "Closed", GroupName = "SelectionEnabledStates")]
  [TemplateVisualState(Name = "Exposed", GroupName = "SelectionEnabledStates")]
  [TemplateVisualState(Name = "Opened", GroupName = "SelectionEnabledStates")]
  [TemplatePart(Name = "OutterHintPanel", Type = typeof (Rectangle))]
  [TemplatePart(Name = "OutterCover", Type = typeof (Grid))]
  [TemplatePart(Name = "InnerHintPanel", Type = typeof (Rectangle))]
  public class MultiselectItem : ContentControl
  {
    private const string SelectionEnabledStates = "SelectionEnabledStates";
    private const string Closed = "Closed";
    private const string Exposed = "Exposed";
    private const string Opened = "Opened";
    private const string SelectBox = "SelectBox";
    private const string OutterHintPanel = "OutterHintPanel";
    private const string InnerHintPanel = "InnerHintPanel";
    private const string OutterCover = "OutterCover";
    private const string InfoPresenter = "InfoPresenter";
    private const double _deltaLimitX = 0.0;
    private const double _deltaLimitY = 0.4;
    private Rectangle _outterHintPanel;
    private Rectangle _innerHintPanel;
    private Grid _outterCover;
    private ContentControl _infoPresenter;
    private MultiselectList _parent;
    private double _manipulationDeltaX;
    private double _manipulationDeltaY;
    internal bool _isBeingVirtualized;
    internal bool _canTriggerSelectionChanged = true;
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof (IsSelected), typeof (bool), typeof (MultiselectItem), new PropertyMetadata((object) false, new PropertyChangedCallback(MultiselectItem.OnIsSelectedPropertyChanged)));
    internal static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof (State), typeof (SelectionEnabledState), typeof (MultiselectItem), new PropertyMetadata((object) SelectionEnabledState.Closed, (PropertyChangedCallback) null));
    public static readonly DependencyProperty HintPanelHeightProperty = DependencyProperty.Register(nameof (HintPanelHeight), typeof (double), typeof (MultiselectItem), new PropertyMetadata((object) double.NaN, (PropertyChangedCallback) null));
    public static readonly DependencyProperty ContentInfoProperty = DependencyProperty.Register(nameof (ContentInfo), typeof (object), typeof (MultiselectItem), new PropertyMetadata((object) null, new PropertyChangedCallback(MultiselectItem.OnContentInfoPropertyChanged)));
    public static readonly DependencyProperty ContentInfoTemplateProperty = DependencyProperty.Register(nameof (ContentInfoTemplate), typeof (DataTemplate), typeof (MultiselectItem), new PropertyMetadata((object) null, new PropertyChangedCallback(MultiselectItem.OnContentInfoTemplatePropertyChanged)));

    public event RoutedEventHandler Selected;

    public event RoutedEventHandler Unselected;

    public bool IsSelected
    {
      get => (bool) this.GetValue(MultiselectItem.IsSelectedProperty);
      set => this.SetValue(MultiselectItem.IsSelectedProperty, (object) value);
    }

    private static void OnIsSelectedPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      MultiselectItem multiselectItem = (MultiselectItem) obj;
      RoutedEventArgs e1 = new RoutedEventArgs();
      bool newValue = (bool) e.NewValue;
      if (newValue)
        multiselectItem.OnSelected(e1);
      else
        multiselectItem.OnUnselected(e1);
      if (multiselectItem._parent == null || multiselectItem._isBeingVirtualized)
        return;
      if (newValue)
      {
        multiselectItem._parent.SelectedItems.Add(multiselectItem.Content);
        if (!multiselectItem._canTriggerSelectionChanged)
          return;
        multiselectItem._parent.OnSelectionChanged((IList) new object[0], (IList) new object[1]
        {
          multiselectItem.Content
        });
      }
      else
      {
        multiselectItem._parent.SelectedItems.Remove(multiselectItem.Content);
        if (!multiselectItem._canTriggerSelectionChanged)
          return;
        multiselectItem._parent.OnSelectionChanged((IList) new object[1]
        {
          multiselectItem.Content
        }, (IList) new object[0]);
      }
    }

    internal SelectionEnabledState State
    {
      get => (SelectionEnabledState) this.GetValue(MultiselectItem.StateProperty);
      set => this.SetValue(MultiselectItem.StateProperty, (object) value);
    }

    public double HintPanelHeight
    {
      get => (double) this.GetValue(MultiselectItem.HintPanelHeightProperty);
      set => this.SetValue(MultiselectItem.HintPanelHeightProperty, (object) value);
    }

    private static void OnHintPanelHeightPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      MultiselectItem multiselectItem = (MultiselectItem) obj;
      if (multiselectItem._outterHintPanel != null)
      {
        if (double.IsNaN((double) e.NewValue))
          multiselectItem._outterHintPanel.VerticalAlignment = VerticalAlignment.Stretch;
        else
          multiselectItem._outterHintPanel.VerticalAlignment = VerticalAlignment.Top;
      }
      if (multiselectItem._innerHintPanel == null)
        return;
      if (double.IsNaN(multiselectItem.HintPanelHeight))
        multiselectItem._innerHintPanel.VerticalAlignment = VerticalAlignment.Stretch;
      else
        multiselectItem._innerHintPanel.VerticalAlignment = VerticalAlignment.Top;
    }

    public object ContentInfo
    {
      get => this.GetValue(MultiselectItem.ContentInfoProperty);
      set => this.SetValue(MultiselectItem.ContentInfoProperty, value);
    }

    private static void OnContentInfoPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ((MultiselectItem) obj).OnContentInfoChanged(e.OldValue, e.NewValue);
    }

    public DataTemplate ContentInfoTemplate
    {
      get => (DataTemplate) this.GetValue(MultiselectItem.ContentInfoTemplateProperty);
      set => this.SetValue(MultiselectItem.ContentInfoTemplateProperty, (object) value);
    }

    private static void OnContentInfoTemplatePropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ((MultiselectItem) obj).OnContentInfoTemplateChanged(e.OldValue as DataTemplate, e.NewValue as DataTemplate);
    }

    public override void OnApplyTemplate()
    {
      this._parent = this.GetParentByType<MultiselectList>();
      if (this._innerHintPanel != null)
      {
        this._innerHintPanel.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(this.HintPanel_ManipulationStarted);
        this._innerHintPanel.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(this.HintPanel_ManipulationDelta);
        this._innerHintPanel.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.HintPanel_ManipulationCompleted);
      }
      if (this._outterHintPanel != null)
      {
        this._outterHintPanel.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(this.HintPanel_ManipulationStarted);
        this._outterHintPanel.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(this.HintPanel_ManipulationDelta);
        this._outterHintPanel.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.HintPanel_ManipulationCompleted);
      }
      if (this._outterCover != null)
        this._outterCover.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.Cover_Tap);
      this._innerHintPanel = this.GetTemplateChild("InnerHintPanel") as Rectangle;
      this._outterHintPanel = this.GetTemplateChild("OutterHintPanel") as Rectangle;
      this._outterCover = this.GetTemplateChild("OutterCover") as Grid;
      this._infoPresenter = this.GetTemplateChild("InfoPresenter") as ContentControl;
      base.OnApplyTemplate();
      if (this._innerHintPanel != null)
      {
        this._innerHintPanel.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.HintPanel_ManipulationStarted);
        this._innerHintPanel.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.HintPanel_ManipulationDelta);
        this._innerHintPanel.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.HintPanel_ManipulationCompleted);
      }
      if (this._outterHintPanel != null)
      {
        this._outterHintPanel.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.HintPanel_ManipulationStarted);
        this._outterHintPanel.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.HintPanel_ManipulationDelta);
        this._outterHintPanel.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.HintPanel_ManipulationCompleted);
      }
      if (this._outterCover != null)
        this._outterCover.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Cover_Tap);
      if (this.ContentInfo == null && this._parent != null && this._parent.ItemInfoTemplate != null)
      {
        this._infoPresenter.ContentTemplate = this._parent.ItemInfoTemplate;
        Binding binding = new Binding();
        this.SetBinding(MultiselectItem.ContentInfoProperty, binding);
      }
      if (this._outterHintPanel != null)
      {
        if (double.IsNaN(this.HintPanelHeight))
          this._outterHintPanel.VerticalAlignment = VerticalAlignment.Stretch;
        else
          this._outterHintPanel.VerticalAlignment = VerticalAlignment.Top;
      }
      if (this._innerHintPanel != null)
      {
        if (double.IsNaN(this.HintPanelHeight))
          this._innerHintPanel.VerticalAlignment = VerticalAlignment.Stretch;
        else
          this._innerHintPanel.VerticalAlignment = VerticalAlignment.Top;
      }
      this.UpdateVisualState(false);
    }

    public MultiselectItem() => this.DefaultStyleKey = (object) typeof (MultiselectItem);

    internal void UpdateVisualState(bool useTransitions)
    {
      string stateName;
      switch (this.State)
      {
        case SelectionEnabledState.Closed:
          stateName = "Closed";
          break;
        case SelectionEnabledState.Exposed:
          stateName = "Exposed";
          break;
        case SelectionEnabledState.Opened:
          stateName = "Opened";
          break;
        default:
          stateName = "Closed";
          break;
      }
      VisualStateManager.GoToState((Control) this, stateName, useTransitions);
    }

    private void RaiseEvent(RoutedEventHandler handler, RoutedEventArgs args)
    {
      if (handler == null)
        return;
      handler((object) this, args);
    }

    protected virtual void OnSelected(RoutedEventArgs e)
    {
      if (this._parent == null)
      {
        this.State = SelectionEnabledState.Opened;
        this.UpdateVisualState(true);
      }
      this.RaiseEvent(this.Selected, e);
    }

    protected virtual void OnUnselected(RoutedEventArgs e)
    {
      if (this._parent == null)
      {
        this.State = SelectionEnabledState.Closed;
        this.UpdateVisualState(true);
      }
      this.RaiseEvent(this.Unselected, e);
    }

    protected virtual void OnContentInfoChanged(object oldContentInfo, object newContentInfo)
    {
    }

    protected virtual void OnContentInfoTemplateChanged(
      DataTemplate oldContentInfoTemplate,
      DataTemplate newContentInfoTemplate)
    {
    }

    private void HintPanel_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.State = SelectionEnabledState.Exposed;
      this.UpdateVisualState(true);
    }

    private void HintPanel_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      this._manipulationDeltaX = e.DeltaManipulation.Translation.X;
      this._manipulationDeltaY = e.DeltaManipulation.Translation.Y;
      if (this._manipulationDeltaX < 0.0)
        this._manipulationDeltaX *= -1.0;
      if (this._manipulationDeltaY < 0.0)
        this._manipulationDeltaY *= -1.0;
      if (this._manipulationDeltaX <= 0.0 && this._manipulationDeltaY < 0.4)
        return;
      this.State = SelectionEnabledState.Closed;
      this.UpdateVisualState(true);
    }

    private void HintPanel_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (this._manipulationDeltaX == 0.0 && this._manipulationDeltaY < 0.4)
        this.IsSelected = true;
      this._manipulationDeltaX = 0.0;
      this._manipulationDeltaY = 0.0;
    }

    private void Cover_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.IsSelected = !this.IsSelected;
    }
  }
}
