// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.LongListMultiSelector
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplatePart(Name = "InnerSelector", Type = typeof (LongListSelector))]
  [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof (LongListMultiSelectorItem))]
  public class LongListMultiSelector : Control
  {
    private const string InnerSelectorName = "InnerSelector";
    private LongListSelector _innerSelector;
    private HashSet<WeakReference<LongListMultiSelectorItem>> _realizedItems = new HashSet<WeakReference<LongListMultiSelectorItem>>();
    private LongListMultiSelector.SelectedItemsList _selectedItems = new LongListMultiSelector.SelectedItemsList();
    private LongListSelectorLayoutMode _layoutMode;
    public static readonly DependencyProperty GridCellSizeProperty = DependencyProperty.Register(nameof (GridCellSize), typeof (Size), typeof (LongListMultiSelector), new PropertyMetadata((object) Size.Empty));
    public static readonly DependencyProperty GroupFooterTemplateProperty = DependencyProperty.Register(nameof (GroupFooterTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty GroupHeaderTemplateProperty = DependencyProperty.Register(nameof (GroupHeaderTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty HideEmptyGroupsProperty = DependencyProperty.Register(nameof (HideEmptyGroups), typeof (bool), typeof (LongListMultiSelector), new PropertyMetadata((object) false));
    public static readonly DependencyProperty IsGroupingEnabledProperty = DependencyProperty.Register(nameof (IsGroupingEnabled), typeof (bool), typeof (LongListMultiSelector), new PropertyMetadata((object) false));
    public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof (ItemContainerStyle), typeof (Style), typeof (LongListMultiSelector), new PropertyMetadata((object) null, new PropertyChangedCallback(LongListMultiSelector.OnItemContainerStylePropertyChanged)));
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (IList), typeof (LongListMultiSelector), new PropertyMetadata((object) null, new PropertyChangedCallback(LongListMultiSelector.OnItemsSourcePropertyChanged)));
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof (ItemTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ItemInfoTemplateProperty = DependencyProperty.Register(nameof (ItemInfoTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((object) null, new PropertyChangedCallback(LongListMultiSelector.OnItemInfoTemplatePropertyChanged)));
    public static readonly DependencyProperty JumpListStyleProperty = DependencyProperty.Register(nameof (JumpListStyle), typeof (Style), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ListFooterProperty = DependencyProperty.Register(nameof (ListFooter), typeof (object), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ListFooterTemplateProperty = DependencyProperty.Register(nameof (ListFooterTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ListHeaderProperty = DependencyProperty.Register(nameof (ListHeader), typeof (object), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ListHeaderTemplateProperty = DependencyProperty.Register(nameof (ListHeaderTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof (SelectedItems), typeof (IList), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsSelectionEnabledProperty = DependencyProperty.Register(nameof (IsSelectionEnabled), typeof (bool), typeof (LongListMultiSelector), new PropertyMetadata((object) false, new PropertyChangedCallback(LongListMultiSelector.OnIsSelectionEnabledPropertyChanged)));
    public static readonly DependencyProperty EnforceIsSelectionEnabledProperty = DependencyProperty.Register(nameof (EnforceIsSelectionEnabled), typeof (bool), typeof (LongListMultiSelector), new PropertyMetadata((object) false, new PropertyChangedCallback(LongListMultiSelector.OnEnforceIsSelectionEnabledPropertyChanged)));
    internal static readonly DependencyProperty DefaultListItemContainerStyleProperty = DependencyProperty.Register(nameof (DefaultListItemContainerStyle), typeof (Style), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    internal static readonly DependencyProperty DefaultGridItemContainerStyleProperty = DependencyProperty.Register(nameof (DefaultGridItemContainerStyle), typeof (Style), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));

    public ManipulationState ManipulationState
    {
      get
      {
        return this._innerSelector != null ? this._innerSelector.ManipulationState : ManipulationState.Idle;
      }
    }

    public Size GridCellSize
    {
      get => (Size) this.GetValue(LongListMultiSelector.GridCellSizeProperty);
      set => this.SetValue(LongListMultiSelector.GridCellSizeProperty, (object) value);
    }

    public DataTemplate GroupFooterTemplate
    {
      get => (DataTemplate) this.GetValue(LongListMultiSelector.GroupFooterTemplateProperty);
      set => this.SetValue(LongListMultiSelector.GroupFooterTemplateProperty, (object) value);
    }

    public DataTemplate GroupHeaderTemplate
    {
      get => (DataTemplate) this.GetValue(LongListMultiSelector.GroupHeaderTemplateProperty);
      set => this.SetValue(LongListMultiSelector.GroupHeaderTemplateProperty, (object) value);
    }

    public bool HideEmptyGroups
    {
      get => (bool) this.GetValue(LongListMultiSelector.HideEmptyGroupsProperty);
      set => this.SetValue(LongListMultiSelector.HideEmptyGroupsProperty, (object) value);
    }

    public bool IsGroupingEnabled
    {
      get => (bool) this.GetValue(LongListMultiSelector.IsGroupingEnabledProperty);
      set => this.SetValue(LongListMultiSelector.IsGroupingEnabledProperty, (object) value);
    }

    public Style ItemContainerStyle
    {
      get => (Style) this.GetValue(LongListMultiSelector.ItemContainerStyleProperty);
      set => this.SetValue(LongListMultiSelector.ItemContainerStyleProperty, (object) value);
    }

    private static void OnItemContainerStylePropertyChanged(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is LongListMultiSelector listMultiSelector))
        return;
      listMultiSelector.OnItemContainerStyleChanged();
    }

    public IList ItemsSource
    {
      get => (IList) this.GetValue(LongListMultiSelector.ItemsSourceProperty);
      set => this.SetValue(LongListMultiSelector.ItemsSourceProperty, (object) value);
    }

    private static void OnItemsSourcePropertyChanged(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is LongListMultiSelector listMultiSelector))
        return;
      listMultiSelector.OnItemsSourceChanged(e.OldValue, e.NewValue);
    }

    public DataTemplate ItemTemplate
    {
      get => (DataTemplate) this.GetValue(LongListMultiSelector.ItemTemplateProperty);
      set => this.SetValue(LongListMultiSelector.ItemTemplateProperty, (object) value);
    }

    public DataTemplate ItemInfoTemplate
    {
      get => (DataTemplate) this.GetValue(LongListMultiSelector.ItemInfoTemplateProperty);
      set => this.SetValue(LongListMultiSelector.ItemInfoTemplateProperty, (object) value);
    }

    private static void OnItemInfoTemplatePropertyChanged(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is LongListMultiSelector listMultiSelector))
        return;
      listMultiSelector.OnItemInfoTemplateChanged();
    }

    public Style JumpListStyle
    {
      get => (Style) this.GetValue(LongListMultiSelector.JumpListStyleProperty);
      set => this.SetValue(LongListMultiSelector.JumpListStyleProperty, (object) value);
    }

    public LongListSelectorLayoutMode LayoutMode
    {
      get => this._layoutMode;
      set
      {
        this._layoutMode = value;
        if (this._innerSelector == null)
          return;
        this._innerSelector.LayoutMode = value;
      }
    }

    public object ListFooter
    {
      get => this.GetValue(LongListMultiSelector.ListFooterProperty);
      set => this.SetValue(LongListMultiSelector.ListFooterProperty, value);
    }

    public DataTemplate ListFooterTemplate
    {
      get => (DataTemplate) this.GetValue(LongListMultiSelector.ListFooterTemplateProperty);
      set => this.SetValue(LongListMultiSelector.ListFooterTemplateProperty, (object) value);
    }

    public object ListHeader
    {
      get => this.GetValue(LongListMultiSelector.ListHeaderProperty);
      set => this.SetValue(LongListMultiSelector.ListHeaderProperty, value);
    }

    public DataTemplate ListHeaderTemplate
    {
      get => (DataTemplate) this.GetValue(LongListMultiSelector.ListHeaderTemplateProperty);
      set => this.SetValue(LongListMultiSelector.ListHeaderTemplateProperty, (object) value);
    }

    public IList SelectedItems
    {
      get => (IList) this.GetValue(LongListMultiSelector.SelectedItemsProperty);
    }

    public bool IsSelectionEnabled
    {
      get => (bool) this.GetValue(LongListMultiSelector.IsSelectionEnabledProperty);
      set => this.SetValue(LongListMultiSelector.IsSelectionEnabledProperty, (object) value);
    }

    public bool EnforceIsSelectionEnabled
    {
      get => (bool) this.GetValue(LongListMultiSelector.EnforceIsSelectionEnabledProperty);
      set => this.SetValue(LongListMultiSelector.EnforceIsSelectionEnabledProperty, (object) value);
    }

    internal Style DefaultListItemContainerStyle
    {
      get => (Style) this.GetValue(LongListMultiSelector.DefaultListItemContainerStyleProperty);
      set
      {
        this.SetValue(LongListMultiSelector.DefaultListItemContainerStyleProperty, (object) value);
      }
    }

    internal Style DefaultGridItemContainerStyle
    {
      get => (Style) this.GetValue(LongListMultiSelector.DefaultGridItemContainerStyleProperty);
      set
      {
        this.SetValue(LongListMultiSelector.DefaultGridItemContainerStyleProperty, (object) value);
      }
    }

    public event EventHandler<ItemRealizationEventArgs> ItemRealized;

    public event EventHandler<ItemRealizationEventArgs> ItemUnrealized;

    public event EventHandler JumpListClosed;

    public event EventHandler JumpListOpening;

    public event EventHandler ManipulationStateChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public event SelectionChangedEventHandler SelectionChanged;

    public event DependencyPropertyChangedEventHandler IsSelectionEnabledChanged;

    public LongListMultiSelector()
    {
      this.DefaultStyleKey = (object) typeof (LongListMultiSelector);
      this.SetValue(LongListMultiSelector.SelectedItemsProperty, (object) this._selectedItems);
      this._selectedItems.CollectionCleared += new EventHandler<LongListMultiSelector.ClearedChangedArgs>(this.OnSelectedItemsCollectionCleared);
      this._selectedItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnSelectedItemsCollectionChanged);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this._realizedItems.Clear();
      if (this._innerSelector != null)
      {
        this._innerSelector.ItemRealized -= new EventHandler<ItemRealizationEventArgs>(this.OnInnerSelectorItemRealized);
        this._innerSelector.ItemUnrealized -= new EventHandler<ItemRealizationEventArgs>(this.OnInnerSelectorItemUnrealized);
        this._innerSelector.JumpListClosed -= new EventHandler(this.OnInnerSelectorJumpListClosed);
        this._innerSelector.JumpListOpening -= new EventHandler(this.OnInnerSelectorJumpListOpening);
        this._innerSelector.ManipulationStateChanged -= new EventHandler(this.OnInnerSelectorManipulationStateChanged);
        this._innerSelector.PropertyChanged -= new PropertyChangedEventHandler(this.OnInnerSelectorPropertyChanged);
      }
      this._innerSelector = this.GetTemplateChild("InnerSelector") as LongListSelector;
      if (this._innerSelector == null)
        return;
      this._innerSelector.LayoutMode = this.LayoutMode;
      this._innerSelector.ItemRealized += new EventHandler<ItemRealizationEventArgs>(this.OnInnerSelectorItemRealized);
      this._innerSelector.ItemUnrealized += new EventHandler<ItemRealizationEventArgs>(this.OnInnerSelectorItemUnrealized);
      this._innerSelector.JumpListClosed += new EventHandler(this.OnInnerSelectorJumpListClosed);
      this._innerSelector.JumpListOpening += new EventHandler(this.OnInnerSelectorJumpListOpening);
      this._innerSelector.ManipulationStateChanged += new EventHandler(this.OnInnerSelectorManipulationStateChanged);
      this._innerSelector.PropertyChanged += new PropertyChangedEventHandler(this.OnInnerSelectorPropertyChanged);
    }

    private void OnItemContainerStyleChanged()
    {
      this.ApplyLiveItems((Action<LongListMultiSelectorItem>) (item => item.Style = this.ItemContainerStyle));
    }

    protected virtual void OnItemsSourceChanged(object oldValue, object newValue)
    {
      if (oldValue is INotifyCollectionChanged collectionChanged1)
        collectionChanged1.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnItemsSourceCollectionChanged);
      if (newValue is INotifyCollectionChanged collectionChanged2)
        collectionChanged2.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnItemsSourceCollectionChanged);
      this.SelectedItems.Clear();
    }

    protected virtual void OnItemsSourceCollectionChanged(
      object sender,
      NotifyCollectionChangedEventArgs e)
    {
      if (e == null || e.OldItems == null)
        return;
      this.UnselectItems(e.OldItems);
    }

    protected virtual void OnItemInfoTemplateChanged()
    {
      this.ApplyLiveItems((Action<LongListMultiSelectorItem>) (item => item.ContentInfoTemplate = this.ItemInfoTemplate));
    }

    private void OnInnerSelectorPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged(sender, e);
    }

    private void OnInnerSelectorManipulationStateChanged(object sender, EventArgs e)
    {
      if (this.ManipulationStateChanged == null)
        return;
      this.ManipulationStateChanged(sender, e);
    }

    private void OnInnerSelectorJumpListOpening(object sender, EventArgs e)
    {
      if (this.JumpListOpening == null)
        return;
      this.JumpListOpening(sender, e);
    }

    private void OnInnerSelectorJumpListClosed(object sender, EventArgs e)
    {
      if (this.JumpListClosed == null)
        return;
      this.JumpListClosed(sender, e);
    }

    private void OnInnerSelectorItemUnrealized(object sender, ItemRealizationEventArgs e)
    {
      if (e.ItemKind == LongListSelectorItemKind.Item && VisualTreeHelper.GetChildrenCount((DependencyObject) e.Container) > 0 && VisualTreeHelper.GetChild((DependencyObject) e.Container, 0) is LongListMultiSelectorItem child)
      {
        child.IsSelectedChanged -= new EventHandler(this.OnLongListMultiSelectorItemIsSelectedChanged);
        this._realizedItems.Remove(child.WR);
      }
      if (this.ItemUnrealized == null)
        return;
      this.ItemUnrealized(sender, e);
    }

    internal void ConfigureItem(LongListMultiSelectorItem item)
    {
      if (item == null)
        return;
      item.ContentTemplate = this.ItemTemplate;
      if (this.ItemContainerStyle != null)
      {
        if (item.Style != this.ItemContainerStyle)
          item.Style = this.ItemContainerStyle;
      }
      else if (this.LayoutMode == LongListSelectorLayoutMode.Grid)
      {
        if (item.Style != this.DefaultGridItemContainerStyle)
          item.Style = this.DefaultGridItemContainerStyle;
      }
      else if (item.Style != this.DefaultListItemContainerStyle)
        item.Style = this.DefaultListItemContainerStyle;
      if (this.ItemInfoTemplate == null || item.ContentInfoTemplate == this.ItemInfoTemplate)
        return;
      item.SetBinding(LongListMultiSelectorItem.ContentInfoProperty, new Binding());
      item.ContentInfoTemplate = this.ItemInfoTemplate;
    }

    private void OnInnerSelectorItemRealized(object sender, ItemRealizationEventArgs e)
    {
      if (e.ItemKind == LongListSelectorItemKind.Item && VisualTreeHelper.GetChildrenCount((DependencyObject) e.Container) > 0 && VisualTreeHelper.GetChild((DependencyObject) e.Container, 0) is LongListMultiSelectorItem child)
      {
        this.ConfigureItem(child);
        child.IsSelected = this._selectedItems.Contains(child.Content);
        child.IsSelectedChanged += new EventHandler(this.OnLongListMultiSelectorItemIsSelectedChanged);
        child.GotoState(this.IsSelectionEnabled ? LongListMultiSelectorItem.State.Opened : LongListMultiSelectorItem.State.Closed);
        this._realizedItems.Add(child.WR);
      }
      if (this.ItemRealized == null)
        return;
      this.ItemRealized(sender, e);
    }

    private void OnLongListMultiSelectorItemIsSelectedChanged(object sender, EventArgs e)
    {
      if (!(sender is LongListMultiSelectorItem multiSelectorItem))
        return;
      object content = multiSelectorItem.Content;
      if (content == null)
        return;
      if (multiSelectorItem.IsSelected)
      {
        if (this.SelectedItems.Contains(content))
          return;
        this.SelectedItems.Add(content);
      }
      else
        this.SelectedItems.Remove(content);
    }

    private static void OnIsSelectionEnabledPropertyChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is LongListMultiSelector listMultiSelector))
        return;
      listMultiSelector.OnIsSelectionEnabledChanged(e);
    }

    protected virtual void OnIsSelectionEnabledChanged(DependencyPropertyChangedEventArgs e)
    {
      bool newValue = (bool) e.NewValue;
      if (!newValue)
        this.SelectedItems.Clear();
      this.ApplyItemsState(newValue ? LongListMultiSelectorItem.State.Opened : LongListMultiSelectorItem.State.Closed, true);
      if (this.IsSelectionEnabledChanged == null)
        return;
      this.IsSelectionEnabledChanged((object) this, e);
    }

    private static void OnEnforceIsSelectionEnabledPropertyChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is LongListMultiSelector listMultiSelector))
        return;
      listMultiSelector.OnEnforceIsSelectionEnabledChanged();
    }

    protected virtual void OnEnforceIsSelectionEnabledChanged()
    {
      if (!this.EnforceIsSelectionEnabled)
        this.SelectedItems.Clear();
      this.UpdateIsSelectionEnabled();
    }

    protected virtual void UpdateIsSelectionEnabled()
    {
      this.IsSelectionEnabled = this.EnforceIsSelectionEnabled || this.SelectedItems.Count > 0;
    }

    private void OnSelectionChanged(IList removedItems, IList addedItems)
    {
      this.UpdateIsSelectionEnabled();
      if (this.SelectionChanged == null)
        return;
      this.SelectionChanged((object) this, new SelectionChangedEventArgs(removedItems ?? (IList) new List<object>(), addedItems ?? (IList) new List<object>()));
    }

    protected void ApplyLiveItems(Action<LongListMultiSelectorItem> action)
    {
      if (action == null)
        return;
      HashSet<WeakReference<LongListMultiSelectorItem>> weakReferenceSet = new HashSet<WeakReference<LongListMultiSelectorItem>>();
      foreach (WeakReference<LongListMultiSelectorItem> realizedItem in this._realizedItems)
      {
        LongListMultiSelectorItem target;
        if (realizedItem.TryGetTarget(out target))
        {
          action(target);
          weakReferenceSet.Add(realizedItem);
        }
      }
      this._realizedItems = weakReferenceSet;
    }

    private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          this.SelectItems(e.NewItems);
          this.OnSelectionChanged((IList) null, e.NewItems);
          break;
        case NotifyCollectionChangedAction.Remove:
          this.UnselectItems(e.OldItems);
          this.OnSelectionChanged(e.OldItems, (IList) null);
          break;
        case NotifyCollectionChangedAction.Replace:
          this.UnselectItems(e.OldItems);
          this.SelectItems(e.NewItems);
          this.OnSelectionChanged(e.OldItems, e.NewItems);
          break;
      }
    }

    private void OnSelectedItemsCollectionCleared(
      object sender,
      LongListMultiSelector.ClearedChangedArgs e)
    {
      this.ApplyLiveItems((Action<LongListMultiSelectorItem>) (item => item.IsSelected = false));
      this.OnSelectionChanged(e.OldItems, (IList) null);
    }

    private void SelectItems(IList items)
    {
      this.ApplyLiveItems((Action<LongListMultiSelectorItem>) (item =>
      {
        if (!items.Contains(item.Content))
          return;
        item.IsSelected = true;
      }));
    }

    private void UnselectItems(IList items)
    {
      this.ApplyLiveItems((Action<LongListMultiSelectorItem>) (item =>
      {
        if (!items.Contains(item.Content))
          return;
        item.IsSelected = false;
      }));
    }

    public object ContainerFromItem(object item)
    {
      object ret = (object) null;
      this.ApplyLiveItems((Action<LongListMultiSelectorItem>) (llmsItem =>
      {
        if (llmsItem.Content != item)
          return;
        ret = (object) llmsItem;
      }));
      return ret;
    }

    public void ScrollTo(object item)
    {
      if (this._innerSelector == null)
        return;
      this._innerSelector.ScrollTo(item);
    }

    private void ApplyItemsState(LongListMultiSelectorItem.State state, bool useTransitions)
    {
      if (this._innerSelector == null)
        return;
      if (useTransitions)
      {
        List<LongListMultiSelectorItem> invisibleItems = new List<LongListMultiSelectorItem>();
        double actualHeight = this._innerSelector.ActualHeight;
        foreach (WeakReference<LongListMultiSelectorItem> realizedItem in this._realizedItems)
        {
          LongListMultiSelectorItem target;
          if (realizedItem.TryGetTarget(out target))
          {
            GeneralTransform visual = target.TransformToVisual((UIElement) this._innerSelector);
            Point point = visual.Transform(new Point(0.0, 0.0));
            bool flag;
            if (point.Y > actualHeight)
              flag = false;
            else if (point.Y >= 0.0)
            {
              flag = true;
            }
            else
            {
              point = visual.Transform(new Point(target.ActualHeight, 0.0));
              flag = point.Y >= 0.0;
            }
            if (flag)
              target.GotoState(state, true);
            else
              invisibleItems.Add(target);
          }
        }
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          foreach (LongListMultiSelectorItem multiSelectorItem in invisibleItems)
            multiSelectorItem.GotoState(state);
        }));
      }
      else
      {
        foreach (WeakReference<LongListMultiSelectorItem> realizedItem in this._realizedItems)
        {
          LongListMultiSelectorItem target;
          if (realizedItem.TryGetTarget(out target))
            target.GotoState(state);
        }
      }
    }

    private class ClearedChangedArgs : EventArgs
    {
      public IList OldItems { get; private set; }

      public ClearedChangedArgs(IList items) => this.OldItems = items;
    }

    private class SelectedItemsList : ObservableCollection<object>
    {
      public event EventHandler<LongListMultiSelector.ClearedChangedArgs> CollectionCleared;

      protected override void ClearItems()
      {
        if (this.Count <= 0)
          return;
        LongListMultiSelector.ClearedChangedArgs e = this.CollectionCleared != null ? new LongListMultiSelector.ClearedChangedArgs((IList) new List<object>((IEnumerable<object>) this)) : (LongListMultiSelector.ClearedChangedArgs) null;
        base.ClearItems();
        if (this.CollectionCleared == null)
          return;
        this.CollectionCleared((object) this, e);
      }
    }
  }
}
