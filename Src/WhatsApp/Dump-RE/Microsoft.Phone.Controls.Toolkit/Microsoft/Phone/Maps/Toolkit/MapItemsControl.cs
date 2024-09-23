// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Maps.Toolkit.MapItemsControl
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

#nullable disable
namespace Microsoft.Phone.Maps.Toolkit
{
  [ContentProperty("Items")]
  public sealed class MapItemsControl : DependencyObject
  {
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (IEnumerable), typeof (MapItemsControl), new PropertyMetadata(new PropertyChangedCallback(MapItemsControl.OnItemsSourceChanged)));
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof (ItemTemplate), typeof (DataTemplate), typeof (MapItemsControl), new PropertyMetadata(new PropertyChangedCallback(MapItemsControl.OnItemTemplateChanged)));
    public static readonly DependencyProperty NameProperty = DependencyProperty.Register(nameof (ItemTemplate), typeof (string), typeof (MapItemsControl), (PropertyMetadata) null);

    public MapItemsControl()
    {
      this.MapLayer = new MapLayer();
      this.Items = new MapChildCollection();
      this.ItemsChangeManager = new MapItemsControlChangeManager((INotifyCollectionChanged) this.Items)
      {
        MapLayer = this.MapLayer
      };
      this.ItemsSource = (IEnumerable) null;
      this.ItemTemplate = (DataTemplate) null;
    }

    public MapChildCollection Items { get; private set; }

    public string Name
    {
      get => (string) this.GetValue(MapItemsControl.NameProperty);
      set => this.SetValue(MapItemsControl.NameProperty, (object) value);
    }

    public IEnumerable ItemsSource
    {
      get => (IEnumerable) this.GetValue(MapItemsControl.ItemsSourceProperty);
      set => this.SetValue(MapItemsControl.ItemsSourceProperty, (object) value);
    }

    public DataTemplate ItemTemplate
    {
      get => (DataTemplate) this.GetValue(MapItemsControl.ItemTemplateProperty);
      set => this.SetValue(MapItemsControl.ItemTemplateProperty, (object) value);
    }

    internal MapItemsControlChangeManager ItemsChangeManager { get; private set; }

    internal MapItemsSourceChangeManager ItemsSourceChangeManager { get; private set; }

    internal MapLayer MapLayer { get; set; }

    private static void OnItemTemplateChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      MapItemsControl mapItemsControl = (MapItemsControl) d;
      DataTemplate newValue = (DataTemplate) e.NewValue;
      mapItemsControl.ItemsChangeManager.ItemTemplate = newValue;
      foreach (MapOverlay mapOverlay in (Collection<MapOverlay>) mapItemsControl.MapLayer)
      {
        MapChild.ClearMapOverlayBindings(mapOverlay);
        ((ContentPresenter) mapOverlay.Content).ContentTemplate = newValue;
      }
    }

    private static void OnItemsSourceChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      MapItemsControl mapItemsControl = (MapItemsControl) d;
      IEnumerable newValue = (IEnumerable) e.NewValue;
      if (mapItemsControl.Items.Count > 0)
        throw new InvalidOperationException("Items must be empty before using Items Source");
      if (newValue != null)
      {
        if (mapItemsControl.ItemsSourceChangeManager != null)
        {
          mapItemsControl.ItemsSourceChangeManager.Disconnect();
          mapItemsControl.ItemsSourceChangeManager = (MapItemsSourceChangeManager) null;
        }
        mapItemsControl.Items.AddRange(newValue);
        if (newValue is INotifyCollectionChanged sourceCollection)
          mapItemsControl.ItemsSourceChangeManager = new MapItemsSourceChangeManager(sourceCollection)
          {
            Items = mapItemsControl.Items
          };
      }
      mapItemsControl.Items.IsReadOnly = newValue != null;
    }
  }
}
