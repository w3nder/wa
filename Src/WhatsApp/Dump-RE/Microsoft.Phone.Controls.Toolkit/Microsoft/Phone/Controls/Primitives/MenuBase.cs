// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Primitives.MenuBase
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls.Primitives
{
  [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof (MenuItem))]
  public abstract class MenuBase : ItemsControl
  {
    public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof (ItemContainerStyle), typeof (Style), typeof (MenuBase), (PropertyMetadata) null);

    public Style ItemContainerStyle
    {
      get => (Style) this.GetValue(MenuBase.ItemContainerStyleProperty);
      set => this.SetValue(MenuBase.ItemContainerStyleProperty, (object) value);
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is MenuItem || item is Separator;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      return (DependencyObject) new MenuItem();
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);
      if (!(element is MenuItem menuItem))
        return;
      menuItem.ParentMenuBase = this;
      if (menuItem == item)
        return;
      DataTemplate itemTemplate = this.ItemTemplate;
      Style itemContainerStyle = this.ItemContainerStyle;
      if (itemTemplate != null)
        menuItem.SetValue(ItemsControl.ItemTemplateProperty, (object) itemTemplate);
      if (itemContainerStyle != null && MenuBase.HasDefaultValue((Control) menuItem, HeaderedItemsControl.ItemContainerStyleProperty))
        menuItem.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, (object) itemContainerStyle);
      if (MenuBase.HasDefaultValue((Control) menuItem, HeaderedItemsControl.HeaderProperty))
        menuItem.Header = item;
      if (itemTemplate != null)
        menuItem.SetValue(HeaderedItemsControl.HeaderTemplateProperty, (object) itemTemplate);
      if (itemContainerStyle == null)
        return;
      menuItem.SetValue(FrameworkElement.StyleProperty, (object) itemContainerStyle);
    }

    private static bool HasDefaultValue(Control control, DependencyProperty property)
    {
      return control.ReadLocalValue(property) == DependencyProperty.UnsetValue;
    }
  }
}
