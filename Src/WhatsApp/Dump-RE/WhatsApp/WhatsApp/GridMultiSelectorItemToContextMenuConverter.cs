// Decompiled with JetBrains decompiler
// Type: WhatsApp.GridMultiSelectorItemToContextMenuConverter
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace WhatsApp
{
  public class GridMultiSelectorItemToContextMenuConverter : IValueConverter
  {
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
      MediaMultiSelector.Item item = value as MediaMultiSelector.Item;
      if (item == null || item.Operations == null)
        return (object) null;
      LinkedList<MenuItem> linkedList = new LinkedList<MenuItem>();
      foreach (MediaMultiSelector.Item.Operation operation in item.Operations)
      {
        MediaMultiSelector.Item.Operation op = operation;
        MenuItem menuItem1 = new MenuItem();
        menuItem1.Header = (object) op.Name;
        MenuItem menuItem2 = menuItem1;
        menuItem2.Click += (RoutedEventHandler) ((sender, e) => op.Op(item));
        linkedList.AddLast(menuItem2);
      }
      if (linkedList.Count <= 0)
        return (object) null;
      ContextMenu contextMenu = new ContextMenu();
      contextMenu.ItemsSource = (IEnumerable) linkedList;
      return (object) contextMenu;
    }

    public object ConvertBack(
      object value,
      System.Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
