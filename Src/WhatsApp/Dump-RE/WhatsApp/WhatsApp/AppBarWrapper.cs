// Decompiled with JetBrains decompiler
// Type: WhatsApp.AppBarWrapper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public class AppBarWrapper
  {
    private IApplicationBar appBar;
    private bool? isEnabled;
    private Dictionary<int, Action<object, ApplicationBarMenuItem>> menuItemUpdates;
    private Dictionary<int, Action<object, ApplicationBarIconButton>> buttonUpdates;

    public IApplicationBar AppBar => this.appBar;

    public bool IsEnabled
    {
      get => this.isEnabled ?? true;
      set
      {
        if (this.appBar != null && (!this.isEnabled.HasValue || this.isEnabled.Value != value))
        {
          this.isEnabled = new bool?(value);
          foreach (object button in (IEnumerable) this.AppBar.Buttons)
          {
            if (button is ApplicationBarIconButton applicationBarIconButton)
              applicationBarIconButton.IsEnabled = this.isEnabled.Value;
          }
          this.AppBar.IsMenuEnabled = this.isEnabled.Value;
        }
        if (((int) this.isEnabled ?? 1) == 0)
          return;
        this.UpdateAppBar();
      }
    }

    public AppBarWrapper(IApplicationBar bar) => this.appBar = bar;

    public void AddMenuItemUpdateAction(
      int menuItemIndex,
      Action<object, ApplicationBarMenuItem> action)
    {
      if (this.menuItemUpdates == null)
        this.menuItemUpdates = new Dictionary<int, Action<object, ApplicationBarMenuItem>>();
      this.menuItemUpdates[menuItemIndex] = action;
    }

    public void AddButtonUpdateAction(
      int buttonIndex,
      Action<object, ApplicationBarIconButton> action)
    {
      if (this.buttonUpdates == null)
        this.buttonUpdates = new Dictionary<int, Action<object, ApplicationBarIconButton>>();
      this.buttonUpdates[buttonIndex] = action;
    }

    public void UpdateAppBar(object obj = null)
    {
      if (this.menuItemUpdates != null)
      {
        foreach (KeyValuePair<int, Action<object, ApplicationBarMenuItem>> menuItemUpdate in this.menuItemUpdates)
        {
          ApplicationBarMenuItem menuItem = this.AppBar.MenuItems[menuItemUpdate.Key] as ApplicationBarMenuItem;
          Action<object, ApplicationBarMenuItem> action = menuItemUpdate.Value;
          if (action != null && menuItem != null)
            action(obj, menuItem);
        }
      }
      if (this.buttonUpdates == null)
        return;
      foreach (KeyValuePair<int, Action<object, ApplicationBarIconButton>> buttonUpdate in this.buttonUpdates)
      {
        ApplicationBarIconButton button = this.AppBar.Buttons[buttonUpdate.Key] as ApplicationBarIconButton;
        Action<object, ApplicationBarIconButton> action = buttonUpdate.Value;
        if (action != null && button != null)
          action(obj, button);
      }
    }

    public void EnableMenuItems(bool enable)
    {
      if (this.appBar == null)
        return;
      foreach (object menuItem in (IEnumerable) this.appBar.MenuItems)
      {
        if (menuItem is ApplicationBarMenuItem applicationBarMenuItem)
          applicationBarMenuItem.IsEnabled = enable;
      }
    }
  }
}
