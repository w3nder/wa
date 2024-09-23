// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.LongListMultiSelectorItem
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplatePart(Name = "OuterHintPanel", Type = typeof (Rectangle))]
  [TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
  [TemplatePart(Name = "InnerHintPanel", Type = typeof (Rectangle))]
  [TemplatePart(Name = "OuterCover", Type = typeof (Grid))]
  [TemplatePart(Name = "InfoPresenter", Type = typeof (ContentControl))]
  [TemplateVisualState(Name = "Opened", GroupName = "HasSelectionStates")]
  [TemplateVisualState(Name = "Exposed", GroupName = "ManipulationStates")]
  [TemplatePart(Name = "ContentContainer", Type = typeof (ContentControl))]
  [TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
  [TemplateVisualState(Name = "Closed", GroupName = "HasSelectionStates")]
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
    private const double _translationYLimit = 0.4;
    private Rectangle _outerHintPanel;
    private Rectangle _innerHintPanel;
    private Grid _outerCover;
    private bool _insideAndDown;
    private bool _isOpened;
    private WeakReference<LongListMultiSelectorItem> _wr;
    public static readonly DependencyProperty ContentInfoProperty = DependencyProperty.Register(nameof (ContentInfo), typeof (object), typeof (LongListMultiSelectorItem), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ContentInfoTemplateProperty = DependencyProperty.Register(nameof (ContentInfoTemplate), typeof (DataTemplate), typeof (LongListMultiSelectorItem), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof (IsSelected), typeof (bool), typeof (LongListMultiSelectorItem), new PropertyMetadata((object) false, new PropertyChangedCallback(LongListMultiSelectorItem.OnIsSelectedPropertyChanged)));
    public static readonly DependencyProperty HintPanelHeightProperty = DependencyProperty.Register(nameof (HintPanelHeight), typeof (double), typeof (LongListMultiSelectorItem), new PropertyMetadata((object) double.NaN, new PropertyChangedCallback(LongListMultiSelectorItem.OnHintPanelHeightPropertyChanged)));

    internal WeakReference<LongListMultiSelectorItem> WR
    {
      get
      {
        if (this._wr == null)
          this._wr = new WeakReference<LongListMultiSelectorItem>(this);
        return this._wr;
      }
    }

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
      set => this.SetValue(LongListMultiSelectorItem.IsSelectedProperty, (object) value);
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

    public LongListMultiSelectorItem()
    {
      this.DefaultStyleKey = (object) typeof (LongListMultiSelectorItem);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (this._outerHintPanel != null)
      {
        this._outerHintPanel.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(this.OnSelectPanelManipulationStarted);
        this._outerHintPanel.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(this.OnSelectPanelManipulationDelta);
        this._outerHintPanel.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.OnSelectPanelManipulationCompleted);
      }
      if (this._innerHintPanel != null)
      {
        this._innerHintPanel.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(this.OnSelectPanelManipulationStarted);
        this._innerHintPanel.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(this.OnSelectPanelManipulationDelta);
        this._innerHintPanel.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.OnSelectPanelManipulationCompleted);
      }
      if (this._outerCover != null)
        this._outerCover.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.OnCoverTap);
      this._outerHintPanel = this.GetTemplateChild("OuterHintPanel") as Rectangle;
      if (this._outerHintPanel != null)
      {
        this._outerHintPanel.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.OnSelectPanelManipulationStarted);
        this._outerHintPanel.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.OnSelectPanelManipulationDelta);
        this._outerHintPanel.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.OnSelectPanelManipulationCompleted);
      }
      this._innerHintPanel = this.GetTemplateChild("InnerHintPanel") as Rectangle;
      if (this._innerHintPanel != null)
      {
        this._innerHintPanel.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.OnSelectPanelManipulationStarted);
        this._innerHintPanel.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.OnSelectPanelManipulationDelta);
        this._innerHintPanel.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.OnSelectPanelManipulationCompleted);
      }
      this._outerCover = this.GetTemplateChild("OuterCover") as Grid;
      if (this._outerCover != null)
        this._outerCover.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.OnCoverTap);
      this.OnHintPanelHeightChanged();
      this.GotoState(this._isOpened ? LongListMultiSelectorItem.State.Opened : LongListMultiSelectorItem.State.Closed);
      this.GotoState(this.IsSelected ? LongListMultiSelectorItem.State.Selected : LongListMultiSelectorItem.State.Unselected);
    }

    protected virtual void OnIsSelectedChanged()
    {
      this.GotoState(this.IsSelected ? LongListMultiSelectorItem.State.Selected : LongListMultiSelectorItem.State.Unselected, true);
      if (this.IsSelectedChanged == null)
        return;
      this.IsSelectedChanged((object) this, (EventArgs) null);
    }

    private void OnCoverTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.IsSelected = !this.IsSelected;
    }

    private void OnSelectPanelManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this._insideAndDown = true;
      this.GotoState(LongListMultiSelectorItem.State.Exposed, true);
    }

    private void OnSelectPanelManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (e.DeltaManipulation.Translation.X == 0.0 && e.DeltaManipulation.Translation.Y > -0.4 && e.DeltaManipulation.Translation.Y < 0.4)
        return;
      this._insideAndDown = false;
      this.GotoState(LongListMultiSelectorItem.State.Closed, true);
    }

    private void OnSelectPanelManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (!this._insideAndDown)
        return;
      this._insideAndDown = false;
      this.IsSelected = true;
    }

    internal void GotoState(LongListMultiSelectorItem.State state, bool useTransitions = false)
    {
      string stateName;
      switch (state)
      {
        case LongListMultiSelectorItem.State.Opened:
          this._isOpened = true;
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
          this._isOpened = false;
          stateName = "Closed";
          break;
      }
      VisualStateManager.GoToState((Control) this, stateName, useTransitions);
    }

    protected virtual void OnHintPanelHeightChanged()
    {
      if (this._outerHintPanel != null)
        this._outerHintPanel.VerticalAlignment = double.IsNaN(this.HintPanelHeight) ? VerticalAlignment.Stretch : VerticalAlignment.Top;
      if (this._innerHintPanel == null)
        return;
      this._innerHintPanel.VerticalAlignment = double.IsNaN(this.HintPanelHeight) ? VerticalAlignment.Stretch : VerticalAlignment.Top;
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
      this.GetParentByType<LongListMultiSelector>()?.ConfigureItem(this);
      base.OnContentChanged(oldContent, newContent);
    }

    internal enum State
    {
      Opened,
      Exposed,
      Closed,
      Selected,
      Unselected,
    }
  }
}
