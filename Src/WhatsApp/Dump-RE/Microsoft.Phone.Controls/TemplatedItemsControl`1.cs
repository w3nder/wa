// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Primitives.TemplatedItemsControl`1
// Assembly: Microsoft.Phone.Controls, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: 4CA3932D-706F-4F20-BA5D-2E9619E9CC1E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls.Primitives
{
  public class TemplatedItemsControl<T> : ItemsControl where T : FrameworkElement, new()
  {
    private readonly Dictionary<object, T> _itemToContainer = new Dictionary<object, T>();
    private readonly Dictionary<T, object> _containerToItem = new Dictionary<T, object>();
    [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is correct use of DependencyProperty.")]
    public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof (ItemContainerStyle), typeof (Style), typeof (TemplatedItemsControl<T>), (PropertyMetadata) null);

    public Style ItemContainerStyle
    {
      get => this.GetValue(TemplatedItemsControl<T>.ItemContainerStyleProperty) as Style;
      set => this.SetValue(TemplatedItemsControl<T>.ItemContainerStyleProperty, (object) value);
    }

    protected override bool IsItemItsOwnContainerOverride(object item) => item is T;

    protected override DependencyObject GetContainerForItemOverride()
    {
      T container = new T();
      this.ApplyItemContainerStyle((DependencyObject) container);
      return (DependencyObject) container;
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      this.ApplyItemContainerStyle(element);
      base.PrepareContainerForItemOverride(element, item);
      this._itemToContainer[item] = (T) element;
      this._containerToItem[(T) element] = item;
    }

    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
      base.ClearContainerForItemOverride(element, item);
      this._itemToContainer.Remove(item);
      this._containerToItem.Remove((T) element);
    }

    protected virtual void ApplyItemContainerStyle(DependencyObject container)
    {
      if (!(container is T obj) || obj.ReadLocalValue(FrameworkElement.StyleProperty) != DependencyProperty.UnsetValue)
        return;
      Style itemContainerStyle = this.ItemContainerStyle;
      if (itemContainerStyle != null)
        obj.Style = itemContainerStyle;
      else
        obj.ClearValue(FrameworkElement.StyleProperty);
    }

    protected object GetItem(T container)
    {
      object obj = (object) null;
      if ((object) container != null)
        this._containerToItem.TryGetValue(container, out obj);
      return obj;
    }

    protected T GetContainer(object item)
    {
      T container = default (T);
      if (item != null)
        this._itemToContainer.TryGetValue(item, out container);
      return container;
    }
  }
}
