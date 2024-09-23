// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.TileSourceCollection
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class TileSourceCollection : ObservableCollection<TileSource>
  {
    public TileSourceCollection()
    {
      this.CollectionChanged += new NotifyCollectionChangedEventHandler(this.TileSourceCollection_CollectionChanged);
    }

    public event EventHandler<TileSourcePropertyChangedEventArgs> ItemPropertyChanged;

    private void TileSourceCollection_CollectionChanged(
      object sender,
      NotifyCollectionChangedEventArgs e)
    {
      this.AddTileSourceEventHandler(e.NewItems);
      this.RemoveTileSourceEventHandler(e.OldItems);
    }

    private void AddTileSourceEventHandler(IList tileSources)
    {
      if (tileSources == null)
        return;
      foreach (TileSource tileSource in (IEnumerable) tileSources)
        tileSource.PropertyChanged += new PropertyChangedEventHandler(this.TileSource_PropertyChanged);
    }

    private void RemoveTileSourceEventHandler(IList tileSources)
    {
      if (tileSources == null)
        return;
      foreach (TileSource tileSource in (IEnumerable) tileSources)
        tileSource.PropertyChanged -= new PropertyChangedEventHandler(this.TileSource_PropertyChanged);
    }

    private void TileSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      EventHandler<TileSourcePropertyChangedEventArgs> itemPropertyChanged = this.ItemPropertyChanged;
      if (itemPropertyChanged == null)
        return;
      TileSourcePropertyChangedEventArgs e1 = new TileSourcePropertyChangedEventArgs((TileSource) sender, e.PropertyName);
      itemPropertyChanged((object) this, e1);
    }
  }
}
