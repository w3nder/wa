// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Rating
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplatePart(Name = "UnfilledGridElement", Type = typeof (Grid))]
  [TemplatePart(Name = "FilledClipElement", Type = typeof (UIElement))]
  [TemplatePart(Name = "DragTextBlockElement", Type = typeof (TextBlock))]
  [TemplateVisualState(Name = "Visible", GroupName = "DragHelperStates")]
  [TemplatePart(Name = "DragBorderElement", Type = typeof (Border))]
  [TemplateVisualState(Name = "Collapsed", GroupName = "DragHelperStates")]
  [TemplatePart(Name = "FilledGridElement", Type = typeof (Grid))]
  public class Rating : Control
  {
    private const string FilledClipElement = "FilledClipElement";
    private const string FilledGridElement = "FilledGridElement";
    private const string UnfilledGridElement = "UnfilledGridElement";
    private const string DragBorderElement = "DragBorderElement";
    private const string DragTextBlockElement = "DragTextBlockElement";
    private const string DragHelperStates = "DragHelperStates";
    private const string DragHelperCollapsed = "Collapsed";
    private const string DragHelperVisible = "Visible";
    private UIElement _filledClipElement;
    private Grid _filledGridElement;
    private Grid _unfilledGridElement;
    private Border _dragBorderElement;
    private TextBlock _dragTextBlockElement;
    private Geometry _clippingMask;
    private List<RatingItem> _filledItemCollection = new List<RatingItem>();
    private List<RatingItem> _unfilledItemCollection = new List<RatingItem>();
    public static readonly DependencyProperty FilledItemStyleProperty = DependencyProperty.Register(nameof (FilledItemStyle), typeof (Style), typeof (Rating), new PropertyMetadata(new PropertyChangedCallback(Rating.OnFilledItemStyleChanged)));
    public static readonly DependencyProperty UnfilledItemStyleProperty = DependencyProperty.Register(nameof (UnfilledItemStyle), typeof (Style), typeof (Rating), new PropertyMetadata(new PropertyChangedCallback(Rating.OnUnfilledItemStyleChanged)));
    public static readonly DependencyProperty RatingItemCountProperty = DependencyProperty.Register(nameof (RatingItemCount), typeof (int), typeof (Rating), new PropertyMetadata((object) 5, new PropertyChangedCallback(Rating.OnRatingItemCountChanged)));
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (double), typeof (Rating), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(Rating.OnValueChanged)));
    public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register(nameof (ReadOnly), typeof (bool), typeof (Rating), new PropertyMetadata((object) false));
    public static readonly DependencyProperty AllowHalfItemIncrementProperty = DependencyProperty.Register(nameof (AllowHalfItemIncrement), typeof (bool), typeof (Rating), new PropertyMetadata((object) false));
    public static readonly DependencyProperty AllowSelectingZeroProperty = DependencyProperty.Register(nameof (AllowSelectingZero), typeof (bool), typeof (Rating), new PropertyMetadata((object) false));
    public static readonly DependencyProperty ShowSelectionHelperProperty = DependencyProperty.Register("ShowSelectionHelperItems", typeof (bool), typeof (Rating), new PropertyMetadata((object) false));
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), typeof (Orientation), typeof (Rating), new PropertyMetadata((object) Orientation.Horizontal, new PropertyChangedCallback(Rating.OnOrientationChanged)));

    public event EventHandler ValueChanged;

    public Rating()
    {
      this.DefaultStyleKey = (object) typeof (Rating);
      this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
      this.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.OnManipulationStarted);
      this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.OnManipulationDelta);
      this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.OnManipulationCompleted);
      this.AdjustNumberOfRatingItems();
      this.SynchronizeGrids();
      this.UpdateClippingMask();
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (this.ReadOnly)
        return;
      this.PerformValueCalculation(e.ManipulationOrigin, e.ManipulationContainer);
      this.UpdateDragHelper();
      if (!this.ShowSelectionHelper)
        return;
      this.ChangeDragHelperVisibility(true);
    }

    private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      if (this.ReadOnly)
        return;
      this.PerformValueCalculation(e.ManipulationOrigin, e.ManipulationContainer);
      this.UpdateDragHelper();
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (!this.ReadOnly)
        this.PerformValueCalculation(e.ManipulationOrigin, e.ManipulationContainer);
      this.ChangeDragHelperVisibility(false);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => this.UpdateClippingMask();

    public override void OnApplyTemplate()
    {
      this._filledClipElement = this.GetTemplateChild("FilledClipElement") as UIElement;
      this._filledGridElement = this.GetTemplateChild("FilledGridElement") as Grid;
      this._unfilledGridElement = this.GetTemplateChild("UnfilledGridElement") as Grid;
      this._dragBorderElement = this.GetTemplateChild("DragBorderElement") as Border;
      this._dragTextBlockElement = this.GetTemplateChild("DragTextBlockElement") as TextBlock;
      if (this._filledClipElement != null)
        this._filledClipElement.Clip = this._clippingMask;
      if (this._dragBorderElement != null)
        this._dragBorderElement.RenderTransform = (Transform) new TranslateTransform();
      VisualStateManager.GoToState((Control) this, "Collapsed", false);
      this.SynchronizeGrids();
    }

    private void ChangeDragHelperVisibility(bool isVisible)
    {
      if (this._dragBorderElement == null)
        return;
      if (isVisible)
        VisualStateManager.GoToState((Control) this, "Visible", true);
      else
        VisualStateManager.GoToState((Control) this, "Collapsed", true);
    }

    private void UpdateDragHelper()
    {
      if (this.RatingItemCount == 0)
        return;
      string format = !this.AllowHalfItemIncrement ? "F0" : "F1";
      if (this._dragTextBlockElement != null)
        this._dragTextBlockElement.Text = this.Value.ToString(format, (IFormatProvider) CultureInfo.CurrentCulture);
      if (this.Orientation == Orientation.Horizontal)
      {
        if (this._dragBorderElement == null)
          return;
        double num1 = this._dragBorderElement.ActualWidth / 2.0;
        double num2 = this._filledItemCollection[0].ActualWidth / 2.0;
        TranslateTransform renderTransform = (TranslateTransform) this._dragBorderElement.RenderTransform;
        renderTransform.X = this.AllowHalfItemIncrement || this.AllowSelectingZero ? this.Value / (double) this.RatingItemCount * this.ActualWidth - num1 : this.Value / (double) this.RatingItemCount * this.ActualWidth - num1 - num2;
        renderTransform.Y = -(this.ActualHeight / 2.0 + 15.0);
      }
      else
      {
        if (this._dragBorderElement == null)
          return;
        double num3 = this._dragBorderElement.ActualHeight / 2.0;
        double num4 = this._filledItemCollection[0].ActualHeight / 2.0;
        TranslateTransform renderTransform = (TranslateTransform) this._dragBorderElement.RenderTransform;
        renderTransform.Y = this.AllowHalfItemIncrement || this.AllowSelectingZero ? this.Value / (double) this.RatingItemCount * this.ActualHeight - num3 : this.Value / (double) this.RatingItemCount * this.ActualHeight - num3 - num4;
        renderTransform.X = -(this.ActualWidth / 2.0 + 15.0);
      }
    }

    private void PerformValueCalculation(Point location, UIElement locationRelativeSource)
    {
      location = locationRelativeSource.TransformToVisual((UIElement) this).Transform(location);
      int count = this._filledItemCollection.Count;
      if (this.AllowHalfItemIncrement)
        count *= 2;
      double num = this.Orientation != Orientation.Horizontal ? Math.Ceiling(location.Y / this.ActualHeight * (double) count) : Math.Ceiling(location.X / this.ActualWidth * (double) count);
      if (!this.AllowSelectingZero && num <= 0.0)
        num = 1.0;
      this.Value = num;
    }

    private void UpdateClippingMask()
    {
      Rect rect = this.Orientation != Orientation.Horizontal ? new Rect(0.0, 0.0, this.ActualWidth, (this.ActualHeight - this.BorderThickness.Top - this.BorderThickness.Bottom) * (this.Value / (double) this.RatingItemCount)) : new Rect(0.0, 0.0, (this.ActualWidth - this.BorderThickness.Right - this.BorderThickness.Left) * (this.Value / (double) this.RatingItemCount), this.ActualHeight);
      if (this._clippingMask is RectangleGeometry clippingMask)
        clippingMask.Rect = rect;
      else
        this._clippingMask = (Geometry) new RectangleGeometry()
        {
          Rect = rect
        };
    }

    private static RatingItem BuildNewRatingItem(Style s)
    {
      RatingItem ratingItem = new RatingItem();
      if (s != null)
        ratingItem.Style = s;
      return ratingItem;
    }

    private void AdjustNumberOfRatingItems()
    {
      while (this._filledItemCollection.Count > this.RatingItemCount)
        this._filledItemCollection.RemoveAt(0);
      while (this._unfilledItemCollection.Count > this.RatingItemCount)
        this._unfilledItemCollection.RemoveAt(0);
      while (this._filledItemCollection.Count < this.RatingItemCount)
        this._filledItemCollection.Add(Rating.BuildNewRatingItem(this.FilledItemStyle));
      while (this._unfilledItemCollection.Count < this.RatingItemCount)
        this._unfilledItemCollection.Add(Rating.BuildNewRatingItem(this.UnfilledItemStyle));
    }

    private void SynchronizeGrid(Grid grid, IList<RatingItem> ratingItemList)
    {
      if (grid == null)
        return;
      grid.RowDefinitions.Clear();
      grid.ColumnDefinitions.Clear();
      if (this.Orientation == Orientation.Horizontal)
      {
        while (grid.ColumnDefinitions.Count < ratingItemList.Count)
          grid.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.0, GridUnitType.Star)
          });
        grid.Children.Clear();
        for (int index = 0; index < ratingItemList.Count; ++index)
        {
          grid.Children.Add((UIElement) ratingItemList[index]);
          Grid.SetColumn((FrameworkElement) ratingItemList[index], index);
          Grid.SetRow((FrameworkElement) ratingItemList[index], 0);
        }
      }
      else
      {
        while (grid.RowDefinitions.Count < ratingItemList.Count)
          grid.RowDefinitions.Add(new RowDefinition()
          {
            Height = new GridLength(1.0, GridUnitType.Star)
          });
        grid.Children.Clear();
        for (int index = 0; index < ratingItemList.Count; ++index)
        {
          grid.Children.Add((UIElement) ratingItemList[index]);
          Grid.SetRow((FrameworkElement) ratingItemList[index], index);
          Grid.SetColumn((FrameworkElement) ratingItemList[index], 0);
        }
      }
    }

    private void SynchronizeGrids()
    {
      this.SynchronizeGrid(this._unfilledGridElement, (IList<RatingItem>) this._unfilledItemCollection);
      this.SynchronizeGrid(this._filledGridElement, (IList<RatingItem>) this._filledItemCollection);
    }

    public Style FilledItemStyle
    {
      get => (Style) this.GetValue(Rating.FilledItemStyleProperty);
      set => this.SetValue(Rating.FilledItemStyleProperty, (object) value);
    }

    private static void OnFilledItemStyleChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs e)
    {
      ((Rating) dependencyObject).OnFilledItemStyleChanged();
    }

    private void OnFilledItemStyleChanged()
    {
      foreach (FrameworkElement filledItem in this._filledItemCollection)
        filledItem.Style = this.FilledItemStyle;
    }

    public Style UnfilledItemStyle
    {
      get => (Style) this.GetValue(Rating.UnfilledItemStyleProperty);
      set => this.SetValue(Rating.UnfilledItemStyleProperty, (object) value);
    }

    private static void OnUnfilledItemStyleChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs e)
    {
      ((Rating) dependencyObject).OnUnfilledItemStyleChanged();
    }

    private void OnUnfilledItemStyleChanged()
    {
      foreach (FrameworkElement unfilledItem in this._unfilledItemCollection)
        unfilledItem.Style = this.UnfilledItemStyle;
    }

    public int RatingItemCount
    {
      get => (int) this.GetValue(Rating.RatingItemCountProperty);
      set => this.SetValue(Rating.RatingItemCountProperty, (object) value);
    }

    private static void OnRatingItemCountChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs e)
    {
      ((Rating) dependencyObject).OnRatingItemCountChanged();
    }

    private void OnRatingItemCountChanged()
    {
      if (this.RatingItemCount <= 0)
        this.RatingItemCount = 0;
      this.AdjustNumberOfRatingItems();
      this.SynchronizeGrids();
    }

    public double Value
    {
      get => (double) this.GetValue(Rating.ValueProperty);
      set
      {
        this.SetValue(Rating.ValueProperty, (object) value);
        if (this.ValueChanged == null)
          return;
        this.ValueChanged((object) this, EventArgs.Empty);
      }
    }

    private static void OnValueChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs e)
    {
      ((Rating) dependencyObject).OnValueChanged();
    }

    private void OnValueChanged()
    {
      if (this.Value > (double) this.RatingItemCount || this.Value < 0.0)
        this.Value = Math.Max(0.0, Math.Min((double) this.RatingItemCount, this.Value));
      this.UpdateClippingMask();
    }

    public bool ReadOnly
    {
      get => (bool) this.GetValue(Rating.ReadOnlyProperty);
      set => this.SetValue(Rating.ReadOnlyProperty, (object) value);
    }

    public bool AllowHalfItemIncrement
    {
      get => (bool) this.GetValue(Rating.AllowHalfItemIncrementProperty);
      set => this.SetValue(Rating.AllowHalfItemIncrementProperty, (object) value);
    }

    public bool AllowSelectingZero
    {
      get => (bool) this.GetValue(Rating.AllowSelectingZeroProperty);
      set => this.SetValue(Rating.AllowSelectingZeroProperty, (object) value);
    }

    public bool ShowSelectionHelper
    {
      get => (bool) this.GetValue(Rating.ShowSelectionHelperProperty);
      set => this.SetValue(Rating.ShowSelectionHelperProperty, (object) value);
    }

    public Orientation Orientation
    {
      get => (Orientation) this.GetValue(Rating.OrientationProperty);
      set => this.SetValue(Rating.OrientationProperty, (object) value);
    }

    private static void OnOrientationChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs e)
    {
      ((Rating) dependencyObject).OnOrientationChanged();
    }

    private void OnOrientationChanged()
    {
      this.SynchronizeGrids();
      this.UpdateClippingMask();
    }
  }
}
