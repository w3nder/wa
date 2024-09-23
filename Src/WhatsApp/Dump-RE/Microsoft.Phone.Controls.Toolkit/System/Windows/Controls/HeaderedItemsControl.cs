// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.HeaderedItemsControl
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows.Data;

#nullable disable
namespace System.Windows.Controls
{
  [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof (ContentPresenter))]
  public class HeaderedItemsControl : ItemsControl
  {
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof (Header), typeof (object), typeof (HeaderedItemsControl), new PropertyMetadata(new PropertyChangedCallback(HeaderedItemsControl.OnHeaderPropertyChanged)));
    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof (HeaderTemplate), typeof (DataTemplate), typeof (HeaderedItemsControl), new PropertyMetadata(new PropertyChangedCallback(HeaderedItemsControl.OnHeaderTemplatePropertyChanged)));
    public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof (ItemContainerStyle), typeof (Style), typeof (HeaderedItemsControl), new PropertyMetadata((object) null, new PropertyChangedCallback(HeaderedItemsControl.OnItemContainerStylePropertyChanged)));

    internal bool HeaderIsItem { get; set; }

    public object Header
    {
      get => this.GetValue(HeaderedItemsControl.HeaderProperty);
      set => this.SetValue(HeaderedItemsControl.HeaderProperty, value);
    }

    private static void OnHeaderPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as HeaderedItemsControl).OnHeaderChanged(e.OldValue, e.NewValue);
    }

    public DataTemplate HeaderTemplate
    {
      get => this.GetValue(HeaderedItemsControl.HeaderTemplateProperty) as DataTemplate;
      set => this.SetValue(HeaderedItemsControl.HeaderTemplateProperty, (object) value);
    }

    private static void OnHeaderTemplatePropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as HeaderedItemsControl).OnHeaderTemplateChanged(e.OldValue as DataTemplate, e.NewValue as DataTemplate);
    }

    public Style ItemContainerStyle
    {
      get => this.GetValue(HeaderedItemsControl.ItemContainerStyleProperty) as Style;
      set => this.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, (object) value);
    }

    private static void OnItemContainerStylePropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as HeaderedItemsControl).ItemsControlHelper.UpdateItemContainerStyle(e.NewValue as Style);
    }

    internal ItemsControlHelper ItemsControlHelper { get; private set; }

    public HeaderedItemsControl()
    {
      this.DefaultStyleKey = (object) typeof (HeaderedItemsControl);
      this.ItemsControlHelper = new ItemsControlHelper((ItemsControl) this);
    }

    protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
    {
    }

    protected virtual void OnHeaderTemplateChanged(
      DataTemplate oldHeaderTemplate,
      DataTemplate newHeaderTemplate)
    {
    }

    public override void OnApplyTemplate()
    {
      this.ItemsControlHelper.OnApplyTemplate();
      base.OnApplyTemplate();
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      ItemsControlHelper.PrepareContainerForItemOverride(element, this.ItemContainerStyle);
      HeaderedItemsControl.PreparePrepareHeaderedItemsControlContainerForItemOverride(element, item, (ItemsControl) this, this.ItemContainerStyle);
      base.PrepareContainerForItemOverride(element, item);
    }

    internal static void PreparePrepareHeaderedItemsControlContainerForItemOverride(
      DependencyObject element,
      object item,
      ItemsControl parent,
      Style parentItemContainerStyle)
    {
      if (!(element is HeaderedItemsControl control))
        return;
      HeaderedItemsControl.PrepareHeaderedItemsControlContainer(control, item, parent, parentItemContainerStyle);
    }

    private static void PrepareHeaderedItemsControlContainer(
      HeaderedItemsControl control,
      object item,
      ItemsControl parentItemsControl,
      Style parentItemContainerStyle)
    {
      if (control == item)
        return;
      DataTemplate itemTemplate = parentItemsControl.ItemTemplate;
      if (itemTemplate != null)
        control.SetValue(ItemsControl.ItemTemplateProperty, (object) itemTemplate);
      if (parentItemContainerStyle != null && HeaderedItemsControl.HasDefaultValue((Control) control, HeaderedItemsControl.ItemContainerStyleProperty))
        control.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, (object) parentItemContainerStyle);
      if (control.HeaderIsItem || HeaderedItemsControl.HasDefaultValue((Control) control, HeaderedItemsControl.HeaderProperty))
      {
        control.Header = item;
        control.HeaderIsItem = true;
      }
      if (itemTemplate != null)
        control.SetValue(HeaderedItemsControl.HeaderTemplateProperty, (object) itemTemplate);
      if (parentItemContainerStyle != null && control.Style == null)
        control.SetValue(FrameworkElement.StyleProperty, (object) parentItemContainerStyle);
      if (!(itemTemplate is HierarchicalDataTemplate hierarchicalDataTemplate))
        return;
      if (hierarchicalDataTemplate.ItemsSource != null && HeaderedItemsControl.HasDefaultValue((Control) control, ItemsControl.ItemsSourceProperty))
        control.SetBinding(ItemsControl.ItemsSourceProperty, new Binding()
        {
          Converter = hierarchicalDataTemplate.ItemsSource.Converter,
          ConverterCulture = hierarchicalDataTemplate.ItemsSource.ConverterCulture,
          ConverterParameter = hierarchicalDataTemplate.ItemsSource.ConverterParameter,
          Mode = hierarchicalDataTemplate.ItemsSource.Mode,
          NotifyOnValidationError = hierarchicalDataTemplate.ItemsSource.NotifyOnValidationError,
          Path = hierarchicalDataTemplate.ItemsSource.Path,
          Source = control.Header,
          ValidatesOnExceptions = hierarchicalDataTemplate.ItemsSource.ValidatesOnExceptions
        });
      if (hierarchicalDataTemplate.IsItemTemplateSet && control.ItemTemplate == itemTemplate)
      {
        control.ClearValue(ItemsControl.ItemTemplateProperty);
        if (hierarchicalDataTemplate.ItemTemplate != null)
          control.ItemTemplate = hierarchicalDataTemplate.ItemTemplate;
      }
      if (!hierarchicalDataTemplate.IsItemContainerStyleSet || control.ItemContainerStyle != parentItemContainerStyle)
        return;
      control.ClearValue(HeaderedItemsControl.ItemContainerStyleProperty);
      if (hierarchicalDataTemplate.ItemContainerStyle == null)
        return;
      control.ItemContainerStyle = hierarchicalDataTemplate.ItemContainerStyle;
    }

    private static bool HasDefaultValue(Control control, DependencyProperty property)
    {
      return control.ReadLocalValue(property) == DependencyProperty.UnsetValue;
    }
  }
}
