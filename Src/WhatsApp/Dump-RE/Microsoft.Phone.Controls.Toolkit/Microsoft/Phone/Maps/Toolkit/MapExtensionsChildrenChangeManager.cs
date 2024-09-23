// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Maps.Toolkit.MapExtensionsChildrenChangeManager
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Maps.Toolkit
{
  internal class MapExtensionsChildrenChangeManager : CollectionChangeListener<DependencyObject>
  {
    public MapExtensionsChildrenChangeManager(INotifyCollectionChanged sourceCollection)
    {
      if (sourceCollection == null)
        throw new ArgumentNullException(nameof (sourceCollection));
      this.ObjectToMapLayerMapping = new Dictionary<DependencyObject, MapLayer>();
      sourceCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(((CollectionChangeListener<DependencyObject>) this).CollectionChanged);
    }

    public Map Map { get; set; }

    private Dictionary<DependencyObject, MapLayer> ObjectToMapLayerMapping { get; set; }

    protected override void InsertItemInternal(int index, DependencyObject obj)
    {
      MapLayer mapLayer = !this.ObjectToMapLayerMapping.ContainsKey(obj) ? MapExtensionsChildrenChangeManager.GetMapLayerForObject((object) obj) : throw new InvalidOperationException("Attempted to insert the same object twice");
      this.ObjectToMapLayerMapping[obj] = mapLayer;
      this.Map.Layers.Insert(index, mapLayer);
    }

    protected override void RemoveItemInternal(DependencyObject obj)
    {
      if (!this.ObjectToMapLayerMapping.ContainsKey(obj))
        return;
      MapLayer mapLayer = this.ObjectToMapLayerMapping[obj];
      this.ObjectToMapLayerMapping.Remove(obj);
      this.Map.Layers.Remove(mapLayer);
      MapItemsControl mapItemsControl = obj as MapItemsControl;
      foreach (MapOverlay mapOverlay in (Collection<MapOverlay>) mapLayer)
        MapChild.ClearMapOverlayBindings(mapOverlay);
    }

    protected override void ResetInternal()
    {
      foreach (Collection<MapOverlay> layer in this.Map.Layers)
      {
        foreach (MapOverlay mapOverlay in layer)
          MapChild.ClearMapOverlayBindings(mapOverlay);
      }
      this.Map.Layers.Clear();
      this.ObjectToMapLayerMapping.Clear();
    }

    protected override void AddInternal(DependencyObject obj)
    {
      MapLayer mapLayer = !this.ObjectToMapLayerMapping.ContainsKey(obj) ? MapExtensionsChildrenChangeManager.GetMapLayerForObject((object) obj) : throw new InvalidOperationException("Attempted to insert the same object twice");
      this.ObjectToMapLayerMapping[obj] = mapLayer;
      this.Map.Layers.Add(mapLayer);
    }

    protected override void MoveInternal(DependencyObject obj, int newIndex)
    {
      if (!this.ObjectToMapLayerMapping.ContainsKey(obj))
        return;
      ObservableCollection<MapLayer> layers = (ObservableCollection<MapLayer>) this.Map.Layers;
      layers.Move(layers.IndexOf(this.ObjectToMapLayerMapping[obj]), newIndex);
    }

    private static MapLayer GetMapLayerForObject(object obj)
    {
      MapLayer mapLayerForObject;
      if (obj is MapItemsControl mapItemsControl)
      {
        mapLayerForObject = mapItemsControl.MapLayer;
      }
      else
      {
        mapLayerForObject = new MapLayer();
        MapOverlay mapOverlay = MapChild.CreateMapOverlay(obj, (DataTemplate) null);
        mapLayerForObject.Add(mapOverlay);
      }
      return mapLayerForObject;
    }
  }
}
