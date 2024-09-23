// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.ListPickerItem
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
  [TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
  public class ListPickerItem : ContentControl
  {
    private const string SelectionStatesGroupName = "SelectionStates";
    private const string SelectionStatesUnselectedStateName = "Unselected";
    private const string SelectionStatesSelectedStateName = "Selected";
    private bool _isSelected;

    public ListPickerItem() => this.DefaultStyleKey = (object) typeof (ListPickerItem);

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      VisualStateManager.GoToState((Control) this, this.IsSelected ? "Selected" : "Unselected", false);
    }

    internal bool IsSelected
    {
      get => this._isSelected;
      set
      {
        this._isSelected = value;
        VisualStateManager.GoToState((Control) this, this._isSelected ? "Selected" : "Unselected", true);
      }
    }
  }
}
