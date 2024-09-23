// Decompiled with JetBrains decompiler
// Type: WhatsApp.Controls.LongListMultiSelectorItem
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace WhatsApp.Controls
{
  [TemplateVisualState(Name = "Opened", GroupName = "HasSelectionStates")]
  [TemplateVisualState(Name = "Closed", GroupName = "HasSelectionStates")]
  [TemplateVisualState(Name = "Exposed", GroupName = "ManipulationStates")]
  [TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
  [TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
  [TemplatePart(Name = "ContentContainer", Type = typeof (ContentControl))]
  [TemplatePart(Name = "OuterHintPanel", Type = typeof (Rectangle))]
  [TemplatePart(Name = "InnerHintPanel", Type = typeof (Rectangle))]
  [TemplatePart(Name = "OuterCover", Type = typeof (Grid))]
  [TemplatePart(Name = "InfoPresenter", Type = typeof (ContentControl))]
  public class LongListMultiSelectorItem : ContentControl
  {
    private const string HasSelectionStatesesName = "HasSelectionStates";
    private const string OpenedStateName = "Opened";
    private const string ClosedStateName = "Closed";
    private const string ExposedStateName = "Exposed";
    private const string ManipulationStatesName = "ManipulationStates";
    private const string SelectionStatesName = "SelectionStates";
    private const string SelectedStateName = "Selected";
    private const string UnselectedStateName = "Unselected";
    private const string ContentContainerName = "ContentContainer";
    private const string OuterHintPanelName = "OuterHintPanel";
    private const string InnerHintPanelName = "InnerHintPanel";
    private const string OuterCoverName = "OuterCover";
    private const string InfoPresenterName = "InfoPresenter";
    private const string PresenterName = "Presenter";
    private const string SelectBoxName = "SelectBox";
    private const double TranslationYLimit = 0.4;
    protected Rectangle outerHintPanel;
    protected Rectangle innerHintPanel;
    protected Grid outerCover;
    protected ContentPresenter presenter;
    protected CheckBox selectBox;
    private bool insideAndDown;
    private bool isOpened;
    private WeakReference<LongListMultiSelectorItem> weakRef;
    private LongListMultiSelectorItem.SelectionModes selMode;
    public static readonly DependencyProperty ContentInfoProperty = DependencyProperty.Register(nameof (ContentInfo), typeof (object), typeof (LongListMultiSelectorItem), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ContentInfoTemplateProperty = DependencyProperty.Register(nameof (ContentInfoTemplate), typeof (DataTemplate), typeof (LongListMultiSelectorItem), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof (IsSelected), typeof (bool), typeof (LongListMultiSelectorItem), new PropertyMetadata((object) false, new PropertyChangedCallback(LongListMultiSelectorItem.OnIsSelectedPropertyChanged)));
    public static readonly DependencyProperty HintPanelHeightProperty = DependencyProperty.Register(nameof (HintPanelHeight), typeof (double), typeof (LongListMultiSelectorItem), new PropertyMetadata((object) double.NaN, new PropertyChangedCallback(LongListMultiSelectorItem.OnHintPanelHeightPropertyChanged)));

    internal WeakReference<LongListMultiSelectorItem> WeakRef
    {
      get => this.weakRef ?? (this.weakRef = new WeakReference<LongListMultiSelectorItem>(this));
    }

    public LongListMultiSelectorItem.SelectionModes SelectionMode
    {
      get => this.selMode;
      set
      {
        if (this.selMode == value)
          return;
        LongListMultiSelectorItem.SelectionModes selMode = this.selMode;
        this.selMode = value;
        this.OnSelectionModeChanged(selMode, value);
      }
    }

    protected virtual double SideMargin => 24.0;

    public bool IsSelectionAllowed { get; set; }

    public object ContentInfo
    {
      get => this.GetValue(LongListMultiSelectorItem.ContentInfoProperty);
      set => this.SetValue(LongListMultiSelectorItem.ContentInfoProperty, value);
    }

    public DataTemplate ContentInfoTemplate
    {
      get => (DataTemplate) this.GetValue(LongListMultiSelectorItem.ContentInfoTemplateProperty);
      set => this.SetValue(LongListMultiSelectorItem.ContentInfoTemplateProperty, (object) value);
    }

    public bool IsSelected
    {
      get => (bool) this.GetValue(LongListMultiSelectorItem.IsSelectedProperty);
      set
      {
        this.SetValue(LongListMultiSelectorItem.IsSelectedProperty, (object) (this.SelectionMode == LongListMultiSelectorItem.SelectionModes.Normal & value));
      }
    }

    private static void OnIsSelectedPropertyChanged(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is LongListMultiSelectorItem multiSelectorItem))
        return;
      multiSelectorItem.OnIsSelectedChanged();
    }

    public double HintPanelHeight
    {
      get => (double) this.GetValue(LongListMultiSelectorItem.HintPanelHeightProperty);
      set => this.SetValue(LongListMultiSelectorItem.HintPanelHeightProperty, (object) value);
    }

    private static void OnHintPanelHeightPropertyChanged(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is LongListMultiSelectorItem multiSelectorItem))
        return;
      multiSelectorItem.OnHintPanelHeightChanged();
    }

    public event EventHandler IsSelectedChanged;

    protected void NotifyIsSelectedChanged()
    {
      if (this.IsSelectedChanged == null)
        return;
      this.IsSelectedChanged((object) this, (EventArgs) null);
    }

    public event EventHandler ContentTapped;

    protected void NotifyContentTapped()
    {
      if (this.ContentTapped == null)
        return;
      this.ContentTapped((object) this, (EventArgs) null);
    }

    public LongListMultiSelectorItem()
    {
      this.DefaultStyleKey = (object) typeof (LongListMultiSelectorItem);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (this.outerHintPanel != null)
      {
        this.outerHintPanel.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(this.OnSelectPanelManipulationStarted);
        this.outerHintPanel.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(this.OnSelectPanelManipulationDelta);
        this.outerHintPanel.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.OnSelectPanelManipulationCompleted);
      }
      if (this.innerHintPanel != null)
      {
        this.innerHintPanel.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(this.OnSelectPanelManipulationStarted);
        this.innerHintPanel.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(this.OnSelectPanelManipulationDelta);
        this.innerHintPanel.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.OnSelectPanelManipulationCompleted);
      }
      if (this.outerCover != null)
        this.outerCover.Tap -= new EventHandler<GestureEventArgs>(this.OnCoverTap);
      if (this.presenter != null)
        this.presenter.Tap -= new EventHandler<GestureEventArgs>(this.OnPresenterTap);
      this.outerHintPanel = this.GetTemplateChild("OuterHintPanel") as Rectangle;
      if (this.outerHintPanel != null && this.IsSelectionAllowed)
      {
        this.outerHintPanel.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.OnSelectPanelManipulationStarted);
        this.outerHintPanel.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.OnSelectPanelManipulationDelta);
        this.outerHintPanel.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.OnSelectPanelManipulationCompleted);
      }
      this.innerHintPanel = this.GetTemplateChild("InnerHintPanel") as Rectangle;
      if (this.innerHintPanel != null && this.IsSelectionAllowed)
      {
        this.innerHintPanel.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.OnSelectPanelManipulationStarted);
        this.innerHintPanel.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.OnSelectPanelManipulationDelta);
        this.innerHintPanel.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.OnSelectPanelManipulationCompleted);
      }
      this.outerCover = this.GetTemplateChild("OuterCover") as Grid;
      if (this.outerCover != null)
        this.outerCover.Tap += new EventHandler<GestureEventArgs>(this.OnCoverTap);
      this.presenter = this.GetTemplateChild("Presenter") as ContentPresenter;
      if (this.presenter != null)
        this.presenter.Tap += new EventHandler<GestureEventArgs>(this.OnPresenterTap);
      this.selectBox = this.GetTemplateChild("SelectBox") as CheckBox;
      this.OnHintPanelHeightChanged();
      this.GotoState(this.isOpened ? LongListMultiSelectorItem.State.Opened : LongListMultiSelectorItem.State.Closed);
      this.GotoState(this.IsSelected ? LongListMultiSelectorItem.State.Selected : LongListMultiSelectorItem.State.Unselected);
    }

    protected virtual void OnIsSelectedChanged()
    {
      this.GotoState(this.IsSelected ? LongListMultiSelectorItem.State.Selected : LongListMultiSelectorItem.State.Unselected, true);
      this.NotifyIsSelectedChanged();
    }

    private void OnCoverTap(object sender, GestureEventArgs e)
    {
      this.IsSelected = !this.IsSelected;
    }

    private void OnPresenterTap(object sender, GestureEventArgs e) => this.NotifyContentTapped();

    private void OnSelectPanelManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      if (this.SelectionMode != LongListMultiSelectorItem.SelectionModes.Normal)
        return;
      this.insideAndDown = true;
      this.GotoState(LongListMultiSelectorItem.State.Exposed, true);
    }

    private void OnSelectPanelManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (this.SelectionMode != LongListMultiSelectorItem.SelectionModes.Normal || e.DeltaManipulation.Translation.X == 0.0 && e.DeltaManipulation.Translation.Y >= -0.4 && e.DeltaManipulation.Translation.Y <= 0.4)
        return;
      this.insideAndDown = false;
      this.GotoState(LongListMultiSelectorItem.State.Closed, true);
    }

    private void OnSelectPanelManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (this.SelectionMode != LongListMultiSelectorItem.SelectionModes.Normal)
      {
        this.NotifyContentTapped();
      }
      else
      {
        if (!this.insideAndDown)
          return;
        this.insideAndDown = false;
        this.IsSelected = true;
      }
    }

    internal void GotoState(LongListMultiSelectorItem.State state, bool useTransitions = false)
    {
      switch (this.SelectionMode)
      {
        case LongListMultiSelectorItem.SelectionModes.Disabled:
        case LongListMultiSelectorItem.SelectionModes.Hidden:
          state = LongListMultiSelectorItem.State.Closed;
          useTransitions = false;
          break;
      }
      string stateName;
      switch (state)
      {
        case LongListMultiSelectorItem.State.Opened:
          this.isOpened = true;
          stateName = "Opened";
          break;
        case LongListMultiSelectorItem.State.Exposed:
          stateName = "Exposed";
          break;
        case LongListMultiSelectorItem.State.Selected:
          stateName = "Selected";
          break;
        case LongListMultiSelectorItem.State.Unselected:
          stateName = "Unselected";
          break;
        default:
          this.isOpened = false;
          stateName = "Closed";
          break;
      }
      VisualStateManager.GoToState((Control) this, stateName, useTransitions);
    }

    protected virtual void OnHintPanelHeightChanged()
    {
      if (this.outerHintPanel != null)
        this.outerHintPanel.VerticalAlignment = double.IsNaN(this.HintPanelHeight) ? VerticalAlignment.Stretch : VerticalAlignment.Top;
      if (this.innerHintPanel == null)
        return;
      this.innerHintPanel.VerticalAlignment = double.IsNaN(this.HintPanelHeight) ? VerticalAlignment.Stretch : VerticalAlignment.Top;
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
      this.GetParentByType<LongListMultiSelector>()?.ConfigureItem(this);
      base.OnContentChanged(oldContent, newContent);
    }

    private void OnSelectionModeChanged(
      LongListMultiSelectorItem.SelectionModes oldMode,
      LongListMultiSelectorItem.SelectionModes newMode)
    {
      if ((oldMode == LongListMultiSelectorItem.SelectionModes.Hidden ? 1 : (newMode == LongListMultiSelectorItem.SelectionModes.Hidden ? 1 : 0)) == 0)
        return;
      if (this.presenter != null)
      {
        double right = newMode == LongListMultiSelectorItem.SelectionModes.Hidden ? 0.0 : this.SideMargin;
        this.presenter.Margin = new Thickness(0.0, 0.0, right, 0.0);
        if (this.presenter.RenderTransform is CompositeTransform renderTransform)
          renderTransform.TranslateX = right;
      }
      this.innerHintPanel.Visibility = this.outerHintPanel.Visibility = (newMode != LongListMultiSelectorItem.SelectionModes.Hidden).ToVisibility();
      if (newMode == LongListMultiSelectorItem.SelectionModes.Normal)
      {
        this.GotoState(this.isOpened ? LongListMultiSelectorItem.State.Opened : LongListMultiSelectorItem.State.Closed);
        this.GotoState(this.IsSelected ? LongListMultiSelectorItem.State.Selected : LongListMultiSelectorItem.State.Unselected);
      }
      else
        this.GotoState(LongListMultiSelectorItem.State.Closed);
    }

    internal enum State
    {
      Opened,
      Exposed,
      Closed,
      Selected,
      Unselected,
    }

    public enum SelectionModes
    {
      Normal,
      Disabled,
      Hidden,
    }
  }
}
