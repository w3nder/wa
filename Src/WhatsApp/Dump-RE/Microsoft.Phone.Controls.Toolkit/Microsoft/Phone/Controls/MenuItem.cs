// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.MenuItem
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.Primitives;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
  [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof (MenuItem))]
  [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
  public class MenuItem : HeaderedItemsControl
  {
    private bool _isFocused;
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof (Command), typeof (ICommand), typeof (MenuItem), new PropertyMetadata((object) null, new PropertyChangedCallback(MenuItem.OnCommandChanged)));
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof (CommandParameter), typeof (object), typeof (MenuItem), new PropertyMetadata((object) null, new PropertyChangedCallback(MenuItem.OnCommandParameterChanged)));

    public event RoutedEventHandler Click;

    internal MenuBase ParentMenuBase { get; set; }

    public ICommand Command
    {
      get => (ICommand) this.GetValue(MenuItem.CommandProperty);
      set => this.SetValue(MenuItem.CommandProperty, (object) value);
    }

    private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      ((MenuItem) o).OnCommandChanged((ICommand) e.OldValue, (ICommand) e.NewValue);
    }

    private void OnCommandChanged(ICommand oldValue, ICommand newValue)
    {
      if (oldValue != null)
        oldValue.CanExecuteChanged -= new EventHandler(this.OnCanExecuteChanged);
      if (newValue != null)
        newValue.CanExecuteChanged += new EventHandler(this.OnCanExecuteChanged);
      this.UpdateIsEnabled(true);
    }

    public object CommandParameter
    {
      get => this.GetValue(MenuItem.CommandParameterProperty);
      set => this.SetValue(MenuItem.CommandParameterProperty, value);
    }

    private static void OnCommandParameterChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((MenuItem) o).OnCommandParameterChanged();
    }

    private void OnCommandParameterChanged() => this.UpdateIsEnabled(true);

    public MenuItem()
    {
      this.DefaultStyleKey = (object) typeof (MenuItem);
      this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnIsEnabledChanged);
      this.SetValue(TiltEffect.IsTiltEnabledProperty, (object) true);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.UpdateIsEnabled(false);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.ChangeVisualState(false);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
      this._isFocused = true;
      this.ChangeVisualState(true);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
      base.OnLostFocus(e);
      this._isFocused = false;
      this.ChangeVisualState(true);
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);
      this.Focus();
      this.ChangeVisualState(true);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      if (this.ParentMenuBase != null)
        this.ParentMenuBase.Focus();
      this.ChangeVisualState(true);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      if (!e.Handled)
      {
        this.OnClick();
        e.Handled = true;
      }
      base.OnMouseLeftButtonUp(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      if (!e.Handled && Key.Enter == e.Key)
      {
        this.OnClick();
        e.Handled = true;
      }
      base.OnKeyDown(e);
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      throw new NotImplementedException();
    }

    protected virtual void OnClick()
    {
      if (this.ParentMenuBase is ContextMenu parentMenuBase)
        parentMenuBase.ChildMenuItemClicked();
      RoutedEventHandler click = this.Click;
      if (click != null)
        click((object) this, new RoutedEventArgs());
      if (this.Command == null || !this.Command.CanExecute(this.CommandParameter))
        return;
      this.Command.Execute(this.CommandParameter);
    }

    private void OnCanExecuteChanged(object sender, EventArgs e) => this.UpdateIsEnabled(true);

    private void UpdateIsEnabled(bool changeVisualState)
    {
      this.IsEnabled = this.Command == null || this.Command.CanExecute(this.CommandParameter);
      if (!changeVisualState)
        return;
      this.ChangeVisualState(true);
    }

    private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.ChangeVisualState(true);
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => this.ChangeVisualState(false);

    protected virtual void ChangeVisualState(bool useTransitions)
    {
      if (!this.IsEnabled)
        VisualStateManager.GoToState((Control) this, "Disabled", useTransitions);
      else
        VisualStateManager.GoToState((Control) this, "Normal", useTransitions);
      if (this._isFocused && this.IsEnabled)
        VisualStateManager.GoToState((Control) this, "Focused", useTransitions);
      else
        VisualStateManager.GoToState((Control) this, "Unfocused", useTransitions);
    }
  }
}
