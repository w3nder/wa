// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ActionPopUp`2
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class ActionPopUp<T, TPopUpResult> : PopUp<T, TPopUpResult>
  {
    private const string ActionButtonAreaName = "actionButtonArea";
    protected Panel ActionButtonArea;
    public readonly DependencyProperty ActionPopUpButtonsProperty = DependencyProperty.Register(nameof (ActionPopUpButtons), typeof (List<Button>), typeof (ActionPopUp<T, TPopUpResult>), new PropertyMetadata((object) new List<Button>(), new PropertyChangedCallback(ActionPopUp<T, TPopUpResult>.OnActionPopUpButtonsChanged)));

    public override void OnApplyTemplate()
    {
      this.Focus();
      base.OnApplyTemplate();
      this.ActionButtonArea = this.GetTemplateChild("actionButtonArea") as Panel;
      this.SetButtons();
    }

    public List<Button> ActionPopUpButtons
    {
      get => (List<Button>) ((DependencyObject) this).GetValue(this.ActionPopUpButtonsProperty);
      set => ((DependencyObject) this).SetValue(this.ActionPopUpButtonsProperty, (object) value);
    }

    private static void OnActionPopUpButtonsChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ActionPopUp<T, TPopUpResult> actionPopUp = (ActionPopUp<T, TPopUpResult>) o;
      if (actionPopUp == null || e.NewValue == e.OldValue)
        return;
      actionPopUp.SetButtons();
    }

    private void SetButtons()
    {
      if (this.ActionButtonArea == null)
        return;
      ((PresentationFrameworkCollection<UIElement>) this.ActionButtonArea.Children).Clear();
      foreach (UIElement actionPopUpButton in this.ActionPopUpButtons)
        ((PresentationFrameworkCollection<UIElement>) this.ActionButtonArea.Children).Add(actionPopUpButton);
    }
  }
}
