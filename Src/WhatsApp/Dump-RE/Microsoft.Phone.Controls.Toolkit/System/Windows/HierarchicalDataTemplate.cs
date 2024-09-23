// Decompiled with JetBrains decompiler
// Type: System.Windows.HierarchicalDataTemplate
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows.Data;

#nullable disable
namespace System.Windows
{
  public class HierarchicalDataTemplate : DataTemplate
  {
    private DataTemplate _itemTemplate;
    private Style _itemContainerStyle;

    public Binding ItemsSource { get; set; }

    internal bool IsItemTemplateSet { get; private set; }

    public DataTemplate ItemTemplate
    {
      get => this._itemTemplate;
      set
      {
        this.IsItemTemplateSet = true;
        this._itemTemplate = value;
      }
    }

    internal bool IsItemContainerStyleSet { get; private set; }

    public Style ItemContainerStyle
    {
      get => this._itemContainerStyle;
      set
      {
        this.IsItemContainerStyleSet = true;
        this._itemContainerStyle = value;
      }
    }
  }
}
