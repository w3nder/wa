// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Primitives.LoopingSelector
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls.Primitives
{
  [TemplatePart(Name = "CenteringTransform", Type = typeof (TranslateTransform))]
  [TemplatePart(Name = "PanningTransform", Type = typeof (TranslateTransform))]
  [TemplatePart(Name = "ItemsPanel", Type = typeof (Panel))]
  public class LoopingSelector : Control
  {
    private const string ItemsPanelName = "ItemsPanel";
    private const string CenteringTransformName = "CenteringTransform";
    private const string PanningTransformName = "PanningTransform";
    private const double DragSensitivity = 12.0;
    private static readonly Duration _selectDuration = new Duration(TimeSpan.FromMilliseconds(500.0));
    private readonly IEasingFunction _selectEase;
    private static readonly Duration _panDuration = new Duration(TimeSpan.FromMilliseconds(100.0));
    private readonly IEasingFunction _panEase;
    private DoubleAnimation _panelAnimation;
    private Storyboard _panelStoryboard;
    private Panel _itemsPanel;
    private TranslateTransform _panningTransform;
    private TranslateTransform _centeringTransform;
    private bool _isSelecting;
    private LoopingSelectorItem _selectedItem;
    private Queue<LoopingSelectorItem> _temporaryItemsPool;
    private double _minimumPanelScroll;
    private double _maximumPanelScroll;
    private int _additionalItemsCount;
    private bool _isAnimating;
    private double _dragTarget;
    private bool _isAllowedToDragVertically;
    private bool _isDragging;
    private bool _actualIsExpanded;
    private bool _changeStateAfterAnimation;
    private LoopingSelector.State _state;
    public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(nameof (DataSource), typeof (ILoopingSelectorDataSource), typeof (LoopingSelector), new PropertyMetadata((object) null, new PropertyChangedCallback(LoopingSelector.OnDataSourceChanged)));
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof (ItemTemplate), typeof (DataTemplate), typeof (LoopingSelector), new PropertyMetadata((object) null, new PropertyChangedCallback(LoopingSelector.OnItemTemplateChanged)));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof (IsExpanded), typeof (bool), typeof (LoopingSelector), new PropertyMetadata((object) false, new PropertyChangedCallback(LoopingSelector.OnIsExpandedChanged)));

    public ILoopingSelectorDataSource DataSource
    {
      get => (ILoopingSelectorDataSource) this.GetValue(LoopingSelector.DataSourceProperty);
      set => this.SetValue(LoopingSelector.DataSourceProperty, (object) value);
    }

    private void OnDataSourceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!this.IsReady || this._isSelecting || e.AddedItems.Count != 1)
        return;
      object addedItem = e.AddedItems[0];
      foreach (LoopingSelectorItem child in (PresentationFrameworkCollection<UIElement>) this._itemsPanel.Children)
      {
        if (child.DataContext == addedItem)
        {
          this.SelectAndSnapTo(child);
          return;
        }
      }
      this.UpdateData();
    }

    private static void OnDataSourceChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      LoopingSelector loopingSelector = (LoopingSelector) obj;
      if (e.OldValue != null)
        ((ILoopingSelectorDataSource) e.OldValue).SelectionChanged -= new EventHandler<SelectionChangedEventArgs>(loopingSelector.OnDataSourceSelectionChanged);
      if (e.NewValue != null)
        ((ILoopingSelectorDataSource) e.NewValue).SelectionChanged += new EventHandler<SelectionChangedEventArgs>(loopingSelector.OnDataSourceSelectionChanged);
      loopingSelector.UpdateData();
    }

    public DataTemplate ItemTemplate
    {
      get => (DataTemplate) this.GetValue(LoopingSelector.ItemTemplateProperty);
      set => this.SetValue(LoopingSelector.ItemTemplateProperty, (object) value);
    }

    private static void OnItemTemplateChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ((LoopingSelector) obj).UpdateItemTemplates();
    }

    public Size ItemSize { get; set; }

    public Thickness ItemMargin { get; set; }

    public LoopingSelector()
    {
      ExponentialEase exponentialEase = new ExponentialEase();
      exponentialEase.EasingMode = EasingMode.EaseInOut;
      this._selectEase = (IEasingFunction) exponentialEase;
      this._panEase = (IEasingFunction) new ExponentialEase();
      this._minimumPanelScroll = -3.4028234663852886E+38;
      this._maximumPanelScroll = 3.4028234663852886E+38;
      this._isAllowedToDragVertically = true;
      // ISSUE: explicit constructor call
      base.\u002Ector();
      this.DefaultStyleKey = (object) typeof (LoopingSelector);
      this.CreateEventHandlers();
    }

    public bool IsExpanded
    {
      get => (bool) this.GetValue(LoopingSelector.IsExpandedProperty);
      set => this.SetValue(LoopingSelector.IsExpandedProperty, (object) value);
    }

    public event DependencyPropertyChangedEventHandler IsExpandedChanged;

    private static void OnIsExpandedChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      LoopingSelector sender1 = (LoopingSelector) sender;
      if (sender1.IsExpanded)
        sender1.Balance();
      else
        sender1.SelectAndSnapToClosest();
      sender1.UpdateItemState();
      if (sender1._state == LoopingSelector.State.Normal || sender1._state == LoopingSelector.State.Expanded)
        sender1._state = sender1.IsExpanded ? LoopingSelector.State.Expanded : LoopingSelector.State.Normal;
      sender1._actualIsExpanded = sender1.IsExpanded;
      DependencyPropertyChangedEventHandler isExpandedChanged = sender1.IsExpandedChanged;
      if (isExpandedChanged == null)
        return;
      isExpandedChanged((object) sender1, e);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (!(this.GetTemplateChild("ItemsPanel") is Panel panel))
        panel = (Panel) new Canvas();
      this._itemsPanel = panel;
      if (!(this.GetTemplateChild("CenteringTransform") is TranslateTransform translateTransform1))
        translateTransform1 = new TranslateTransform();
      this._centeringTransform = translateTransform1;
      if (!(this.GetTemplateChild("PanningTransform") is TranslateTransform translateTransform2))
        translateTransform2 = new TranslateTransform();
      this._panningTransform = translateTransform2;
      this.CreateVisuals();
    }

    private void LoopingSelector_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (!this._isAnimating)
        return;
      double y = this._panningTransform.Y;
      this.StopAnimation();
      this._panningTransform.Y = y;
      this._isAnimating = false;
      this._state = LoopingSelector.State.Dragging;
    }

    private void LoopingSelector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this._selectedItem == sender || this._state != LoopingSelector.State.Dragging || this._isAnimating)
        return;
      this.SelectAndSnapToClosest();
      this._state = LoopingSelector.State.Expanded;
    }

    private void OnTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this._panningTransform == null)
        return;
      if (this.IsExpanded)
        this.SelectAndSnapToClosest();
      else
        this.IsExpanded = true;
      e.Handled = true;
    }

    private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this._isAllowedToDragVertically = true;
      this._isDragging = false;
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (this._isDragging)
      {
        Duration panDuration = LoopingSelector._panDuration;
        IEasingFunction panEase = this._panEase;
        LoopingSelector loopingSelector = this;
        double dragTarget = loopingSelector._dragTarget;
        double y = e.DeltaManipulation.Translation.Y;
        double num1;
        double num2 = num1 = dragTarget + y;
        loopingSelector._dragTarget = num1;
        double to = num2;
        this.AnimatePanel(panDuration, panEase, to);
        this._changeStateAfterAnimation = false;
        e.Handled = true;
      }
      else if (Math.Abs(e.CumulativeManipulation.Translation.X) > 12.0)
      {
        this._isAllowedToDragVertically = false;
      }
      else
      {
        if (!this._isAllowedToDragVertically || Math.Abs(e.CumulativeManipulation.Translation.Y) <= 12.0)
          return;
        this._isDragging = true;
        this._state = LoopingSelector.State.Dragging;
        e.Handled = true;
        this._selectedItem = (LoopingSelectorItem) null;
        if (!this.IsExpanded)
          this.IsExpanded = true;
        this._dragTarget = this._panningTransform.Y;
        this.UpdateItemState();
      }
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (!this._isDragging)
        return;
      if (e.IsInertial)
      {
        this._state = LoopingSelector.State.Flicking;
        this._selectedItem = (LoopingSelectorItem) null;
        if (!this.IsExpanded)
          this.IsExpanded = true;
        Point initialVelocity = new Point(0.0, e.FinalVelocities.LinearVelocity.Y);
        double stopTime = PhysicsConstants.GetStopTime(initialVelocity);
        Point stopPoint = PhysicsConstants.GetStopPoint(initialVelocity);
        IEasingFunction easingFunction = PhysicsConstants.GetEasingFunction(stopTime);
        double num = this._panningTransform.Y + stopPoint.Y;
        double to = Math.Round(num / this.ActualItemHeight) * this.ActualItemHeight;
        this.AnimatePanel(new Duration(TimeSpan.FromSeconds(stopTime * (to / num))), easingFunction, to);
        this._changeStateAfterAnimation = false;
        e.Handled = true;
        this._selectedItem = (LoopingSelectorItem) null;
        this.UpdateItemState();
      }
      if (this._state == LoopingSelector.State.Dragging)
        this.SelectAndSnapToClosest();
      this._state = LoopingSelector.State.Expanded;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      this._centeringTransform.Y = Math.Round(e.NewSize.Height / 2.0);
      this.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(0.0, 0.0, e.NewSize.Width, e.NewSize.Height)
      };
      this.UpdateData();
    }

    private void OnWrapperClick(object sender, EventArgs e)
    {
      if (this._state == LoopingSelector.State.Normal)
      {
        this._state = LoopingSelector.State.Expanded;
        this.IsExpanded = true;
      }
      else
      {
        if (this._state != LoopingSelector.State.Expanded)
          return;
        if (!this._isAnimating && sender == this._selectedItem)
        {
          this._state = LoopingSelector.State.Normal;
          this.IsExpanded = false;
        }
        else
        {
          if (sender == this._selectedItem || this._isAnimating)
            return;
          this.SelectAndSnapTo((LoopingSelectorItem) sender);
          this._selectedItem.SetState(LoopingSelectorItem.State.Selected, true);
        }
      }
    }

    private void SelectAndSnapTo(LoopingSelectorItem item)
    {
      if (item == null)
        return;
      if (this._selectedItem != null && this._selectedItem != item)
        this._selectedItem.SetState(this.IsExpanded ? LoopingSelectorItem.State.Expanded : LoopingSelectorItem.State.Normal, true);
      if (this._selectedItem != item)
      {
        this._selectedItem = item;
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this._isSelecting = true;
          this.DataSource.SelectedItem = item.DataContext;
          this._isSelecting = false;
        }));
      }
      TranslateTransform transform = item.Transform;
      if (transform == null)
        return;
      double to = -transform.Y - Math.Round(item.ActualHeight / 2.0);
      if (this._panningTransform.Y != to)
      {
        this.AnimatePanel(LoopingSelector._selectDuration, this._selectEase, to);
        this._changeStateAfterAnimation = true;
      }
      else
        this._selectedItem.SetState(LoopingSelectorItem.State.Selected, true);
    }

    private void UpdateData()
    {
      if (!this.IsReady)
        return;
      this._temporaryItemsPool = new Queue<LoopingSelectorItem>(this._itemsPanel.Children.Count);
      foreach (LoopingSelectorItem child in (PresentationFrameworkCollection<UIElement>) this._itemsPanel.Children)
      {
        if (child.GetState() == LoopingSelectorItem.State.Selected)
          child.SetState(LoopingSelectorItem.State.Normal, false);
        this._temporaryItemsPool.Enqueue(child);
        child.Remove();
      }
      this._itemsPanel.Children.Clear();
      this.StopAnimation();
      this._panningTransform.Y = 0.0;
      this._minimumPanelScroll = -3.4028234663852886E+38;
      this._maximumPanelScroll = 3.4028234663852886E+38;
      this.Balance();
    }

    private void AnimatePanel(Duration duration, IEasingFunction ease, double to)
    {
      double num1 = Math.Max(this._minimumPanelScroll, Math.Min(this._maximumPanelScroll, to));
      if (to != num1)
      {
        double num2 = Math.Abs(this._panningTransform.Y - to);
        double num3 = Math.Abs(this._panningTransform.Y - num1) / num2;
        duration = new Duration(TimeSpan.FromMilliseconds((double) duration.TimeSpan.Milliseconds * num3));
        to = num1;
      }
      double y = this._panningTransform.Y;
      this.StopAnimation();
      CompositionTarget.Rendering += new EventHandler(this.AnimationPerFrameCallback);
      this._panelAnimation.Duration = duration;
      this._panelAnimation.EasingFunction = ease;
      this._panelAnimation.From = new double?(y);
      this._panelAnimation.To = new double?(to);
      this._panelStoryboard.Begin();
      this._panelStoryboard.SeekAlignedToLastTick(TimeSpan.Zero);
      this._isAnimating = true;
    }

    private void StopAnimation()
    {
      this._panelStoryboard.Stop();
      this._isAnimating = false;
      CompositionTarget.Rendering -= new EventHandler(this.AnimationPerFrameCallback);
    }

    private void Brake(double newStoppingPoint)
    {
      double num = this._panelAnimation.To.Value - this._panelAnimation.From.Value;
      this.AnimatePanel(new Duration(TimeSpan.FromMilliseconds((double) this._panelAnimation.Duration.TimeSpan.Milliseconds * Math.Abs((newStoppingPoint - this._panningTransform.Y) / num))), this._panelAnimation.EasingFunction, newStoppingPoint);
      this._changeStateAfterAnimation = false;
    }

    private bool IsReady
    {
      get => this.ActualHeight > 0.0 && this.DataSource != null && this._itemsPanel != null;
    }

    private void Balance()
    {
      if (!this.IsReady)
        return;
      double actualItemWidth = this.ActualItemWidth;
      double actualItemHeight = this.ActualItemHeight;
      this._additionalItemsCount = (int) Math.Round(this.ActualHeight * 1.5 / actualItemHeight);
      int num = -1;
      LoopingSelectorItem loopingSelectorItem1;
      if (this._itemsPanel.Children.Count == 0)
      {
        num = 0;
        this._selectedItem = loopingSelectorItem1 = this.CreateAndAddItem(this._itemsPanel, this.DataSource.SelectedItem);
        loopingSelectorItem1.Transform.Y = -actualItemHeight / 2.0;
        loopingSelectorItem1.Transform.X = (this.ActualWidth - actualItemWidth) / 2.0;
        loopingSelectorItem1.SetState(LoopingSelectorItem.State.Selected, false);
      }
      else
        loopingSelectorItem1 = (LoopingSelectorItem) this._itemsPanel.Children[this.GetClosestItem()];
      if (!this.IsExpanded)
        return;
      int count1;
      LoopingSelectorItem before = LoopingSelector.GetFirstItem(loopingSelectorItem1, out count1);
      int count2;
      LoopingSelectorItem after = LoopingSelector.GetLastItem(loopingSelectorItem1, out count2);
      if (count1 < count2 || count1 < this._additionalItemsCount)
      {
        for (; count1 < this._additionalItemsCount; ++count1)
        {
          object previous = this.DataSource.GetPrevious(before.DataContext);
          if (previous == null)
          {
            this._maximumPanelScroll = -before.Transform.Y - actualItemHeight / 2.0;
            if (this._isAnimating && this._panelAnimation.To.Value > this._maximumPanelScroll)
            {
              this.Brake(this._maximumPanelScroll);
              break;
            }
            break;
          }
          LoopingSelectorItem loopingSelectorItem2;
          if (count2 > this._additionalItemsCount)
          {
            loopingSelectorItem2 = after;
            after = after.Previous;
            loopingSelectorItem2.Remove();
            loopingSelectorItem2.Content = loopingSelectorItem2.DataContext = previous;
          }
          else
          {
            loopingSelectorItem2 = this.CreateAndAddItem(this._itemsPanel, previous);
            loopingSelectorItem2.Transform.X = (this.ActualWidth - actualItemWidth) / 2.0;
          }
          loopingSelectorItem2.Transform.Y = before.Transform.Y - actualItemHeight;
          loopingSelectorItem2.InsertBefore(before);
          before = loopingSelectorItem2;
        }
      }
      if (count2 < count1 || count2 < this._additionalItemsCount)
      {
        for (; count2 < this._additionalItemsCount; ++count2)
        {
          object next = this.DataSource.GetNext(after.DataContext);
          if (next == null)
          {
            this._minimumPanelScroll = -after.Transform.Y - actualItemHeight / 2.0;
            if (this._isAnimating && this._panelAnimation.To.Value < this._minimumPanelScroll)
            {
              this.Brake(this._minimumPanelScroll);
              break;
            }
            break;
          }
          LoopingSelectorItem loopingSelectorItem3;
          if (count1 > this._additionalItemsCount)
          {
            loopingSelectorItem3 = before;
            before = before.Next;
            loopingSelectorItem3.Remove();
            loopingSelectorItem3.Content = loopingSelectorItem3.DataContext = next;
          }
          else
          {
            loopingSelectorItem3 = this.CreateAndAddItem(this._itemsPanel, next);
            loopingSelectorItem3.Transform.X = (this.ActualWidth - actualItemWidth) / 2.0;
          }
          loopingSelectorItem3.Transform.Y = after.Transform.Y + actualItemHeight;
          loopingSelectorItem3.InsertAfter(after);
          after = loopingSelectorItem3;
        }
      }
      this._temporaryItemsPool = (Queue<LoopingSelectorItem>) null;
    }

    private static LoopingSelectorItem GetFirstItem(LoopingSelectorItem item, out int count)
    {
      count = 0;
      for (; item.Previous != null; item = item.Previous)
        ++count;
      return item;
    }

    private static LoopingSelectorItem GetLastItem(LoopingSelectorItem item, out int count)
    {
      count = 0;
      for (; item.Next != null; item = item.Next)
        ++count;
      return item;
    }

    private void AnimationPerFrameCallback(object sender, EventArgs e) => this.Balance();

    private int GetClosestItem()
    {
      if (!this.IsReady)
        return -1;
      double actualItemHeight = this.ActualItemHeight;
      int count = this._itemsPanel.Children.Count;
      double y = this._panningTransform.Y;
      double num1 = actualItemHeight / 2.0;
      int closestItem = -1;
      double num2 = double.MaxValue;
      for (int index = 0; index < count; ++index)
      {
        double num3 = Math.Abs(((LoopingSelectorItem) this._itemsPanel.Children[index]).Transform.Y + num1 + y);
        if (num3 <= num1)
        {
          closestItem = index;
          break;
        }
        if (num2 > num3)
        {
          num2 = num3;
          closestItem = index;
        }
      }
      return closestItem;
    }

    private void PanelStoryboardCompleted(object sender, EventArgs e)
    {
      CompositionTarget.Rendering -= new EventHandler(this.AnimationPerFrameCallback);
      this._isAnimating = false;
      if (this._state == LoopingSelector.State.Dragging)
        return;
      if (this._changeStateAfterAnimation)
        this._selectedItem.SetState(LoopingSelectorItem.State.Selected, true);
      this.SelectAndSnapToClosest();
    }

    private void SelectAndSnapToClosest()
    {
      if (!this.IsReady)
        return;
      int closestItem = this.GetClosestItem();
      if (closestItem == -1)
        return;
      this.SelectAndSnapTo((LoopingSelectorItem) this._itemsPanel.Children[closestItem]);
    }

    private void UpdateItemState()
    {
      if (!this.IsReady)
        return;
      bool isExpanded = this.IsExpanded;
      foreach (LoopingSelectorItem child in (PresentationFrameworkCollection<UIElement>) this._itemsPanel.Children)
      {
        if (child == this._selectedItem)
          child.SetState(LoopingSelectorItem.State.Selected, true);
        else
          child.SetState(isExpanded ? LoopingSelectorItem.State.Expanded : LoopingSelectorItem.State.Normal, true);
      }
    }

    private void UpdateItemTemplates()
    {
      if (!this.IsReady)
        return;
      foreach (ContentControl child in (PresentationFrameworkCollection<UIElement>) this._itemsPanel.Children)
        child.ContentTemplate = this.ItemTemplate;
    }

    private double ActualItemWidth => this.Padding.Left + this.Padding.Right + this.ItemSize.Width;

    private double ActualItemHeight
    {
      get => this.Padding.Top + this.Padding.Bottom + this.ItemSize.Height;
    }

    private void CreateVisuals()
    {
      this._panelAnimation = new DoubleAnimation();
      Storyboard.SetTarget((Timeline) this._panelAnimation, (DependencyObject) this._panningTransform);
      Storyboard.SetTargetProperty((Timeline) this._panelAnimation, new PropertyPath("Y", new object[0]));
      this._panelStoryboard = new Storyboard();
      this._panelStoryboard.Children.Add((Timeline) this._panelAnimation);
      this._panelStoryboard.Completed += new EventHandler(this.PanelStoryboardCompleted);
    }

    private void CreateEventHandlers()
    {
      this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
      this.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.OnManipulationStarted);
      this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.OnManipulationCompleted);
      this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.OnManipulationDelta);
      this.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.OnTap);
      this.AddHandler(UIElement.MouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(this.LoopingSelector_MouseLeftButtonDown), true);
      this.AddHandler(UIElement.MouseLeftButtonUpEvent, (Delegate) new MouseButtonEventHandler(this.LoopingSelector_MouseLeftButtonUp), true);
    }

    private LoopingSelectorItem CreateAndAddItem(Panel parent, object content)
    {
      bool flag = this._temporaryItemsPool != null && this._temporaryItemsPool.Count > 0;
      LoopingSelectorItem andAddItem = flag ? this._temporaryItemsPool.Dequeue() : new LoopingSelectorItem();
      if (!flag)
      {
        andAddItem.ContentTemplate = this.ItemTemplate;
        andAddItem.Width = this.ItemSize.Width;
        andAddItem.Height = this.ItemSize.Height;
        andAddItem.Padding = this.ItemMargin;
        andAddItem.Click += new EventHandler<EventArgs>(this.OnWrapperClick);
      }
      andAddItem.DataContext = andAddItem.Content = content;
      parent.Children.Add((UIElement) andAddItem);
      if (!flag)
      {
        andAddItem.ApplyTemplate();
        andAddItem.SetState(LoopingSelectorItem.State.Normal, false);
      }
      if (this.IsExpanded)
        andAddItem.SetState(LoopingSelectorItem.State.Expanded, !this._actualIsExpanded);
      return andAddItem;
    }

    private enum State
    {
      Normal,
      Expanded,
      Dragging,
      Snapping,
      Flicking,
    }
  }
}
