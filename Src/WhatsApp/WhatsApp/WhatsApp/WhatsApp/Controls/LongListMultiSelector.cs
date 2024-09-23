// Decompiled with JetBrains decompiler
// Type: WhatsApp.Controls.LongListMultiSelector
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WhatsApp.WaCollections;


namespace WhatsApp.Controls
{
  [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof (LongListMultiSelectorItem))]
  [TemplatePart(Name = "InnerSelector", Type = typeof (WhatsApp.CompatibilityShims.LongListSelector))]
  public class LongListMultiSelector : Control
  {
    private const string InnerSelectorName = "InnerSelector";
    private WhatsApp.CompatibilityShims.LongListSelector innerSelector;
    private Set<WeakReference<LongListMultiSelectorItem>> realizedItems = new Set<WeakReference<LongListMultiSelectorItem>>();
    private LongListMultiSelector.SelectedItemsList selectedItems = new LongListMultiSelector.SelectedItemsList();
    private bool overlapScrollBar;
    private bool isSelectionAllowed = true;
    public static readonly DependencyProperty GridCellSizeProperty = DependencyProperty.Register(nameof (GridCellSize), typeof (Size), typeof (LongListMultiSelector), new PropertyMetadata((object) Size.Empty));
    public static readonly DependencyProperty GroupFooterTemplateProperty = DependencyProperty.Register(nameof (GroupFooterTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty GroupHeaderTemplateProperty = DependencyProperty.Register(nameof (GroupHeaderTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsGroupingEnabledProperty = DependencyProperty.Register(nameof (IsGroupingEnabled), typeof (bool), typeof (LongListMultiSelector), new PropertyMetadata((object) false));
    public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof (ItemContainerStyle), typeof (Style), typeof (LongListMultiSelector), new PropertyMetadata((object) null, new PropertyChangedCallback(LongListMultiSelector.OnItemContainerStylePropertyChanged)));
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (IList), typeof (LongListMultiSelector), new PropertyMetadata((object) null, new PropertyChangedCallback(LongListMultiSelector.OnItemsSourcePropertyChanged)));
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof (ItemTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ItemInfoTemplateProperty = DependencyProperty.Register(nameof (ItemInfoTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((object) null, new PropertyChangedCallback(LongListMultiSelector.OnItemInfoTemplatePropertyChanged)));
    public static readonly DependencyProperty ListFooterProperty = DependencyProperty.Register(nameof (ListFooter), typeof (object), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ListFooterTemplateProperty = DependencyProperty.Register(nameof (ListFooterTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ListHeaderProperty = DependencyProperty.Register(nameof (ListHeader), typeof (object), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ListHeaderTemplateProperty = DependencyProperty.Register(nameof (ListHeaderTemplate), typeof (DataTemplate), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof (SelectedItems), typeof (IList), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsSelectionEnabledProperty = DependencyProperty.Register(nameof (IsSelectionEnabled), typeof (bool), typeof (LongListMultiSelector), new PropertyMetadata((object) false, new PropertyChangedCallback(LongListMultiSelector.OnIsSelectionEnabledPropertyChanged)));
    public static readonly DependencyProperty EnforceIsSelectionEnabledProperty = DependencyProperty.Register(nameof (EnforceIsSelectionEnabled), typeof (bool), typeof (LongListMultiSelector), new PropertyMetadata((object) false, new PropertyChangedCallback(LongListMultiSelector.OnEnforceIsSelectionEnabledPropertyChanged)));
    internal static readonly DependencyProperty DefaultListItemContainerStyleProperty = DependencyProperty.Register(nameof (DefaultListItemContainerStyle), typeof (Style), typeof (LongListMultiSelector), new PropertyMetadata((PropertyChangedCallback) null));

    public bool OverlapScrollBar
    {
      private get => this.overlapScrollBar;
      set
      {
        this.overlapScrollBar = value;
        if (this.innerSelector == null)
          return;
        this.innerSelector.OverlapScrollBar = this.overlapScrollBar;
      }
    }

    public bool IsSelectionAllowed
    {
      get => this.isSelectionAllowed;
      set
      {
        this.isSelectionAllowed = value;
        if (this.isSelectionAllowed)
          return;
        this.IsSelectionEnabled = false;
      }
    }

    public double ViewportFinalY
    {
      get => this.innerSelector != null ? this.innerSelector.ViewportFinalY : 0.0;
    }

    public System.Windows.Controls.Primitives.ManipulationState ManipulationState
    {
      get
      {
        return this.innerSelector != null ? this.innerSelector.ManipulationState : System.Windows.Controls.Primitives.ManipulationState.Idle;
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
      set
      {
        this.SetValue(LongListMultiSelector.IsSelectionEnabledProperty, (object) (this.IsSelectionAllowed & value));
      }
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

    public event EventHandler<ItemRealizationEventArgs> ItemRealized;

    protected void NotifyItemRealized(object sender, ItemRealizationEventArgs e)
    {
      if (this.ItemRealized == null)
        return;
      this.ItemRealized(sender, e);
    }

    public event EventHandler<ItemRealizationEventArgs> ItemUnrealized;

    public event EventHandler JumpListClosed;

    public event EventHandler JumpListOpening;

    public event EventHandler ManipulationStateChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public event SelectionChangedEventHandler MultiSelectionsChanged;

    protected void NotifyMultiSelectionsChanged(IList removedItems, IList addedItems)
    {
      if (this.MultiSelectionsChanged == null)
        return;
      this.MultiSelectionsChanged((object) this, new SelectionChangedEventArgs(removedItems ?? (IList) new List<object>(), addedItems ?? (IList) new List<object>()));
    }

    public event EventHandler<SingleSelectionChangedArgs> SingleSelectionChanged;

    protected void NotifySingleSelectionChanged(object selected)
    {
      if (this.SingleSelectionChanged == null)
        return;
      this.SingleSelectionChanged((object) this, new SingleSelectionChangedArgs(selected));
    }

    public event DependencyPropertyChangedEventHandler IsSelectionEnabledChanged;

    public LongListMultiSelector()
    {
      this.DefaultStyleKey = (object) typeof (LongListMultiSelector);
      this.SetValue(LongListMultiSelector.SelectedItemsProperty, (object) this.selectedItems);
      this.selectedItems.CollectionCleared += new EventHandler<LongListMultiSelector.ClearedChangedArgs>(this.OnSelectedItemsCollectionCleared);
      this.selectedItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnSelectedItemsCollectionChanged);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.realizedItems.Clear();
      if (this.innerSelector != null)
      {
        this.innerSelector.ItemRealized -= new EventHandler<ItemRealizationEventArgs>(this.OnInnerSelectorItemRealized);
        this.innerSelector.ItemUnrealized -= new EventHandler<ItemRealizationEventArgs>(this.OnInnerSelectorItemUnrealized);
      }
      this.innerSelector = this.GetTemplateChild("InnerSelector") as WhatsApp.CompatibilityShims.LongListSelector;
      if (this.innerSelector == null)
        return;
      this.innerSelector.ItemRealized += new EventHandler<ItemRealizationEventArgs>(this.OnInnerSelectorItemRealized);
      this.innerSelector.ItemUnrealized += new EventHandler<ItemRealizationEventArgs>(this.OnInnerSelectorItemUnrealized);
      this.innerSelector.OverlapScrollBar = this.OverlapScrollBar;
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

    private void OnInnerSelectorItemUnrealized(object sender, ItemRealizationEventArgs e)
    {
      if (e.ItemKind == LongListSelectorItemKind.Item && VisualTreeHelper.GetChildrenCount((DependencyObject) e.Container) > 0 && VisualTreeHelper.GetChild((DependencyObject) e.Container, 0) is LongListMultiSelectorItem child)
      {
        child.IsSelectedChanged -= new EventHandler(this.OnLongListMultiSelectorItemIsSelectedChanged);
        child.ContentTapped -= new EventHandler(this.OnLongListMultiSelectorItemContentTapped);
        this.realizedItems.Remove(child.WeakRef);
      }
      if (this.ItemUnrealized == null)
        return;
      this.ItemUnrealized(sender, e);
    }

    internal void ConfigureItem(LongListMultiSelectorItem item)
    {
      if (item == null)
        return;
      item.IsSelectionAllowed = this.IsSelectionAllowed;
      item.ContentTemplate = this.ItemTemplate;
      if (this.ItemContainerStyle != null)
      {
        if (item.Style != this.ItemContainerStyle)
          item.Style = this.ItemContainerStyle;
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
      if (e.ItemKind == LongListSelectorItemKind.Item)
      {
        if (VisualTreeHelper.GetChildrenCount((DependencyObject) e.Container) > 0)
          this.OnInnerSelectorItemRealizedImpl(VisualTreeHelper.GetChild((DependencyObject) e.Container, 0) as LongListMultiSelectorItem);
        else
          e.Container.SizeChanged += new SizeChangedEventHandler(this.OnInnerSelectorItemSizeChanged);
      }
      this.NotifyItemRealized(sender, e);
    }

    private void OnInnerSelectorItemSizeChanged(object sender, EventArgs e)
    {
      if (!(sender is FrameworkElement reference))
        return;
      this.OnInnerSelectorItemRealizedImpl(VisualTreeHelper.GetChild((DependencyObject) reference, 0) as LongListMultiSelectorItem);
      reference.SizeChanged -= new SizeChangedEventHandler(this.OnInnerSelectorItemSizeChanged);
    }

    private void OnInnerSelectorItemRealizedImpl(LongListMultiSelectorItem llItem)
    {
      if (llItem == null)
        return;
      this.ConfigureItem(llItem);
      llItem.IsSelected = this.selectedItems.Contains(llItem.Content);
      llItem.IsSelectedChanged += new EventHandler(this.OnLongListMultiSelectorItemIsSelectedChanged);
      llItem.ContentTapped += new EventHandler(this.OnLongListMultiSelectorItemContentTapped);
      llItem.SelectionMode = this.IsSelectionAllowed ? LongListMultiSelectorItem.SelectionModes.Normal : LongListMultiSelectorItem.SelectionModes.Disabled;
      llItem.GotoState(!this.IsSelectionAllowed || !this.IsSelectionEnabled ? LongListMultiSelectorItem.State.Closed : LongListMultiSelectorItem.State.Opened);
      this.realizedItems.Add(llItem.WeakRef);
    }

    private void OnLongListMultiSelectorItemContentTapped(object sender, EventArgs e)
    {
      if (!(sender is LongListMultiSelectorItem multiSelectorItem))
        return;
      object content = multiSelectorItem.Content;
      if (content == null)
        return;
      this.NotifySingleSelectionChanged(content);
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
      this.NotifyMultiSelectionsChanged(removedItems, addedItems);
    }

    protected void ApplyLiveItems(Action<LongListMultiSelectorItem> action)
    {
      if (action == null)
        return;
      Set<WeakReference<LongListMultiSelectorItem>> set = new Set<WeakReference<LongListMultiSelectorItem>>();
      foreach (WeakReference<LongListMultiSelectorItem> realizedItem in this.realizedItems)
      {
        LongListMultiSelectorItem target;
        if (realizedItem.TryGetTarget(out target))
        {
          action(target);
          set.Add(realizedItem);
        }
      }
      this.realizedItems = set;
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
      if (this.innerSelector == null)
        return;
      this.innerSelector.ScrollTo(item);
    }

    public void ScrollToPretty(double yOffset)
    {
      if (this.innerSelector == null)
        return;
      this.innerSelector.ScrollToPretty(yOffset);
    }

    private void ApplyItemsState(LongListMultiSelectorItem.State state, bool useTransitions)
    {
      if (this.innerSelector == null)
        return;
      if (useTransitions)
      {
        List<LongListMultiSelectorItem> invisibleItems = new List<LongListMultiSelectorItem>();
        double actualHeight = this.innerSelector.ActualHeight;
        foreach (WeakReference<LongListMultiSelectorItem> realizedItem in this.realizedItems)
        {
          LongListMultiSelectorItem target;
          if (realizedItem.TryGetTarget(out target) && target != null)
          {
            bool flag = false;
            try
            {
              double y = target.TransformToVisual((UIElement) this.innerSelector).Transform(new System.Windows.Point(0.0, 0.0)).Y;
              if (y <= actualHeight)
              {
                if (y < 0.0)
                {
                  if (y >= -target.ActualHeight)
                    goto label_11;
                }
                else
                  goto label_11;
              }
              flag = true;
            }
            catch (Exception ex)
            {
              flag = true;
            }
label_11:
            if (flag)
              invisibleItems.Add(target);
            else
              target.GotoState(state, true);
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
        foreach (WeakReference<LongListMultiSelectorItem> realizedItem in this.realizedItems)
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
