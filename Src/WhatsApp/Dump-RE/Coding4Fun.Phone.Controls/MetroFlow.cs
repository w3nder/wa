// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.MetroFlow
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class MetroFlow : ItemsControl
  {
    private const string LayoutRootName = "LayoutRoot";
    private GridLength _minimizedGridLength = new GridLength(48.0);
    private readonly GridLength _maximizedGridLength = new GridLength(1.0, (GridUnitType) 2);
    private Storyboard _animationBoard;
    private Grid _layoutGrid;
    private int _minimizingColumnIndex;
    public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register(nameof (AnimationDuration), typeof (TimeSpan), typeof (MetroFlow), new PropertyMetadata((object) TimeSpan.FromMilliseconds(100.0)));
    public static readonly DependencyProperty SelectedColumnIndexProperty = DependencyProperty.Register(nameof (SelectedColumnIndex), typeof (int), typeof (MetroFlow), new PropertyMetadata((object) 0, new PropertyChangedCallback(MetroFlow.SelectedColumnIndexChanged)));
    public static readonly DependencyProperty ExpandingWidthProperty = DependencyProperty.Register(nameof (ExpandingWidth), typeof (double), typeof (MetroFlow), new PropertyMetadata(new PropertyChangedCallback(MetroFlow.ColumnGrowWidthChanged)));
    public static readonly DependencyProperty CollapsingWidthProperty = DependencyProperty.Register(nameof (CollapsingWidth), typeof (double), typeof (MetroFlow), new PropertyMetadata(new PropertyChangedCallback(MetroFlow.ColumnShrinkWidthChanged)));

    public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

    public event EventHandler<SelectionTapEventArgs> SelectionTap;

    public MetroFlow() => ((Control) this).DefaultStyleKey = (object) typeof (MetroFlow);

    protected virtual void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);
      if (this._layoutGrid == null)
        return;
      if (this.SelectedColumnIndex >= ((PresentationFrameworkCollection<object>) this.Items).Count)
        this.SelectedColumnIndex = ((PresentationFrameworkCollection<object>) this.Items).Count - 1;
      else if (((PresentationFrameworkCollection<object>) this.Items).Count > 0 && this.SelectedColumnIndex < 0)
        this.SelectedColumnIndex = 0;
      this.ControlLoaded();
    }

    protected virtual bool IsItemItsOwnContainerOverride(object item) => item is MetroFlowData;

    public virtual void OnApplyTemplate()
    {
      ((FrameworkElement) this).OnApplyTemplate();
      this._layoutGrid = ((Control) this).GetTemplateChild("LayoutRoot") as Grid;
      if (this._layoutGrid == null || DesignerProperties.IsInDesignTool && ((PresentationFrameworkCollection<object>) this.Items).Count <= 0)
        return;
      this.ControlLoaded();
    }

    public TimeSpan AnimationDuration
    {
      get => (TimeSpan) ((DependencyObject) this).GetValue(MetroFlow.AnimationDurationProperty);
      set
      {
        ((DependencyObject) this).SetValue(MetroFlow.AnimationDurationProperty, (object) value);
      }
    }

    public int SelectedColumnIndex
    {
      get => (int) ((DependencyObject) this).GetValue(MetroFlow.SelectedColumnIndexProperty);
      set
      {
        ((DependencyObject) this).SetValue(MetroFlow.SelectedColumnIndexProperty, (object) value);
      }
    }

    private static void SelectedColumnIndexChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is MetroFlow metroFlow) || e.NewValue == e.OldValue)
        return;
      metroFlow.SelectionIndexChanged((int) e.OldValue);
    }

    private void SelectionIndexChanged(int oldIndex)
    {
      this._minimizingColumnIndex = oldIndex;
      this.VerifyMinimizingColumnIndex();
      if (this.SelectionChanged != null)
      {
        MetroFlowData metroFlowData = ((PresentationFrameworkCollection<object>) this.Items).Count <= 0 || this.SelectedColumnIndex < 0 ? (MetroFlowData) null : (MetroFlowData) ((PresentationFrameworkCollection<object>) this.Items)[this.SelectedColumnIndex];
        this.SelectionChanged((object) this, new SelectionChangedEventArgs((IList) new List<MetroFlowData>()
        {
          ((PresentationFrameworkCollection<object>) this.Items).Count <= 0 || this._minimizingColumnIndex < 0 ? (MetroFlowData) null : (MetroFlowData) ((PresentationFrameworkCollection<object>) this.Items)[this._minimizingColumnIndex]
        }, (IList) new List<MetroFlowData>()
        {
          metroFlowData
        }));
      }
      this.CreateSb(this._layoutGrid, oldIndex);
    }

    public double ExpandingWidth
    {
      get => (double) ((DependencyObject) this).GetValue(MetroFlow.ExpandingWidthProperty);
      set => ((DependencyObject) this).SetValue(MetroFlow.ExpandingWidthProperty, (object) value);
    }

    public double CollapsingWidth
    {
      get => (double) ((DependencyObject) this).GetValue(MetroFlow.CollapsingWidthProperty);
      set => ((DependencyObject) this).SetValue(MetroFlow.CollapsingWidthProperty, (object) value);
    }

    private static void ColumnGrowWidthChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is MetroFlow metroFlow))
        return;
      Grid layoutGrid = metroFlow._layoutGrid;
      if (((PresentationFrameworkCollection<ColumnDefinition>) layoutGrid.ColumnDefinitions).Count <= 1)
        return;
      MetroFlow.ChangeColumnWidth(((PresentationFrameworkCollection<ColumnDefinition>) layoutGrid.ColumnDefinitions)[metroFlow.SelectedColumnIndex], (double) e.NewValue);
    }

    private static void ColumnShrinkWidthChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is MetroFlow metroFlow))
        return;
      metroFlow.VerifyMinimizingColumnIndex();
      Grid layoutGrid = metroFlow._layoutGrid;
      if (((PresentationFrameworkCollection<ColumnDefinition>) layoutGrid.ColumnDefinitions).Count <= 1)
        return;
      MetroFlow.ChangeColumnWidth(((PresentationFrameworkCollection<ColumnDefinition>) layoutGrid.ColumnDefinitions)[metroFlow._minimizingColumnIndex], (double) e.NewValue);
    }

    private void VerifyMinimizingColumnIndex()
    {
      if (this._minimizingColumnIndex < ((PresentationFrameworkCollection<object>) this.Items).Count)
        return;
      this._minimizingColumnIndex = ((PresentationFrameworkCollection<object>) this.Items).Count - 1;
      if (this.SelectedColumnIndex == this._minimizingColumnIndex)
        --this._minimizingColumnIndex;
      if (this._minimizingColumnIndex >= 0)
        return;
      this._minimizingColumnIndex = 0;
    }

    private static void ChangeColumnWidth(ColumnDefinition target, double value)
    {
      if (target == null)
        return;
      target.Width = new GridLength(value);
    }

    private void ControlLoaded()
    {
      Grid layoutGrid = this._layoutGrid;
      if (this._layoutGrid == null || this.Items == null)
        return;
      ((PresentationFrameworkCollection<ColumnDefinition>) layoutGrid.ColumnDefinitions).Clear();
      ((PresentationFrameworkCollection<UIElement>) ((Panel) layoutGrid).Children).Clear();
      int num = 0;
      foreach (MetroFlowData metroFlowData in (PresentationFrameworkCollection<object>) this.Items)
      {
        bool flag = num == this.SelectedColumnIndex;
        ColumnDefinition columnDefinition = new ColumnDefinition()
        {
          Width = !flag ? this._minimizedGridLength : new GridLength(1.0, (GridUnitType) 2)
        };
        ((PresentationFrameworkCollection<ColumnDefinition>) layoutGrid.ColumnDefinitions).Add(columnDefinition);
        MetroFlowItem metroFlowItem = new MetroFlowItem()
        {
          ItemIndex = num + 1,
          ItemIndexOpacity = !flag ? 1.0 : 0.0,
          ItemIndexVisibility = !flag ? (Visibility) 0 : (Visibility) 1,
          ImageSource = (ImageSource) new BitmapImage(metroFlowData.ImageUri),
          ImageOpacity = flag ? 1.0 : 0.0,
          ImageVisibility = flag ? (Visibility) 0 : (Visibility) 1,
          Title = metroFlowData.Title,
          TitleOpacity = flag ? 1.0 : 0.0,
          TitleVisibility = flag ? (Visibility) 0 : (Visibility) 1
        };
        ((DependencyObject) metroFlowItem).SetValue(Grid.ColumnProperty, (object) num);
        ((UIElement) metroFlowItem).Tap += new EventHandler<GestureEventArgs>(this.ItemTap);
        ((PresentationFrameworkCollection<UIElement>) ((Panel) layoutGrid).Children).Add((UIElement) metroFlowItem);
        ++num;
      }
    }

    private void ItemTap(object sender, GestureEventArgs e)
    {
      if (!(sender is MetroFlowItem element))
        return;
      int selectedColumnIndex = this.SelectedColumnIndex;
      this.SelectedColumnIndex = MetroFlow.GetColumnIndex((DependencyObject) element);
      if (selectedColumnIndex != this.SelectedColumnIndex || this.SelectionTap == null)
        return;
      this.SelectionTap((object) this, new SelectionTapEventArgs()
      {
        Index = this.SelectedColumnIndex,
        Data = (MetroFlowData) ((PresentationFrameworkCollection<object>) this.Items)[this.SelectedColumnIndex]
      });
    }

    private void HandleStoppingAnimation(int targetIndex)
    {
      if (this._animationBoard == null || this._animationBoard.GetCurrentState() != null)
        return;
      this._animationBoard.Stop();
      this.AnimationCompleted(targetIndex);
    }

    private void CreateSb(Grid target, int oldIndex)
    {
      if (target == null || ((PresentationFrameworkCollection<ColumnDefinition>) target.ColumnDefinitions).Count < this.SelectedColumnIndex)
        return;
      this.HandleStoppingAnimation(oldIndex);
      Storyboard sb = new Storyboard();
      MetroFlowItem metroFlowItem1 = MetroFlow.GetMetroFlowItem((Panel) target, this.SelectedColumnIndex);
      MetroFlowItem metroFlowItem2 = MetroFlow.GetMetroFlowItem((Panel) target, oldIndex);
      if (metroFlowItem1 != null)
      {
        metroFlowItem1.ImageVisibility = (Visibility) 0;
        metroFlowItem1.TitleVisibility = (Visibility) 0;
        this.CreateDoubleAnimations(sb, (DependencyObject) metroFlowItem1, "ImageOpacity", 1.0, metroFlowItem1.ImageOpacity);
        this.CreateDoubleAnimations(sb, (DependencyObject) metroFlowItem1, "TitleOpacity", 1.0, metroFlowItem1.TitleOpacity);
        this.CreateDoubleAnimations(sb, (DependencyObject) metroFlowItem1, "ItemIndexOpacity", fromValue: metroFlowItem1.ItemIndexOpacity);
      }
      if (metroFlowItem2 != null)
      {
        metroFlowItem2.ItemIndexVisibility = (Visibility) 0;
        this.CreateDoubleAnimations(sb, (DependencyObject) metroFlowItem2, "ImageOpacity", fromValue: metroFlowItem2.ImageOpacity);
        this.CreateDoubleAnimations(sb, (DependencyObject) metroFlowItem2, "TitleOpacity", fromValue: metroFlowItem2.TitleOpacity);
        this.CreateDoubleAnimations(sb, (DependencyObject) metroFlowItem2, "ItemIndexOpacity", 1.0, metroFlowItem2.ItemIndexOpacity);
      }
      double toValue = this._minimizedGridLength.Value;
      string propertyPath1 = "CollapsingWidth";
      DoubleAnimation doubleAnimations1 = this.CreateDoubleAnimations(sb, (DependencyObject) this, propertyPath1, toValue);
      double fromValue = this._minimizedGridLength.Value;
      string propertyPath2 = "ExpandingWidth";
      DoubleAnimation doubleAnimations2 = this.CreateDoubleAnimations(sb, (DependencyObject) this, propertyPath2, fromValue: fromValue);
      ((Timeline) sb).Completed += (EventHandler) ((sbSender, sbEventArgs) => this.AnimationCompleted());
      if (metroFlowItem2 != null)
      {
        double actualWidth = ((FrameworkElement) metroFlowItem2).ActualWidth;
        doubleAnimations2.To = new double?(actualWidth);
        doubleAnimations1.From = new double?(actualWidth);
      }
      ((UIElement) this).UpdateLayout();
      this._animationBoard = sb;
      this._animationBoard.Begin();
    }

    private DoubleAnimation CreateDoubleAnimations(
      Storyboard sb,
      DependencyObject target,
      string propertyPath,
      double toValue = 0.0,
      double fromValue = 0.0)
    {
      DoubleAnimation doubleAnimation = new DoubleAnimation();
      doubleAnimation.To = new double?(toValue);
      doubleAnimation.From = new double?(fromValue);
      ((Timeline) doubleAnimation).Duration = Duration.op_Implicit(this.AnimationDuration);
      DoubleAnimation doubleAnimations = doubleAnimation;
      Storyboard.SetTarget((Timeline) doubleAnimations, target);
      Storyboard.SetTargetProperty((Timeline) doubleAnimations, new PropertyPath(propertyPath, new object[0]));
      ((PresentationFrameworkCollection<Timeline>) sb.Children).Add((Timeline) doubleAnimations);
      return doubleAnimations;
    }

    private static MetroFlowItem GetMetroFlowItem(Panel target, int index)
    {
      return ((IEnumerable<UIElement>) target.Children).Where<UIElement>((Func<UIElement, bool>) (item => MetroFlow.GetColumnIndex((DependencyObject) item) == index)).SingleOrDefault<UIElement>() as MetroFlowItem;
    }

    private static int GetColumnIndex(DependencyObject element)
    {
      return (int) element.GetValue(Grid.ColumnProperty);
    }

    private void AnimationCompleted() => this.AnimationCompleted(this.SelectedColumnIndex);

    private void AnimationCompleted(int column)
    {
      for (int index = 0; index < ((PresentationFrameworkCollection<ColumnDefinition>) this._layoutGrid.ColumnDefinitions).Count; ++index)
        ((PresentationFrameworkCollection<ColumnDefinition>) this._layoutGrid.ColumnDefinitions)[index].Width = index != column ? this._minimizedGridLength : this._maximizedGridLength;
      foreach (MetroFlowItem element in ((IEnumerable<UIElement>) ((Panel) this._layoutGrid).Children).Select<UIElement, MetroFlowItem>((Func<UIElement, MetroFlowItem>) (t => t as MetroFlowItem)))
        MetroFlow.SetMetroFlowControlItemProperties(element, MetroFlow.GetColumnIndex((DependencyObject) element) == column);
      ((UIElement) this).UpdateLayout();
    }

    private static void SetMetroFlowControlItemProperties(MetroFlowItem item, bool isLarge)
    {
      if (item == null)
        return;
      item.ImageVisibility = item.TitleVisibility = isLarge ? (Visibility) 0 : (Visibility) 1;
      MetroFlowItem metroFlowItem1 = item;
      MetroFlowItem metroFlowItem2 = item;
      int num1 = isLarge ? 1 : 0;
      double num2;
      double num3 = num2 = (double) num1;
      metroFlowItem2.ImageOpacity = num2;
      double num4 = num3;
      metroFlowItem1.TitleOpacity = num4;
      item.ItemIndexVisibility = isLarge ? (Visibility) 1 : (Visibility) 0;
      item.ItemIndexOpacity = isLarge ? 0.0 : 1.0;
    }
  }
}
