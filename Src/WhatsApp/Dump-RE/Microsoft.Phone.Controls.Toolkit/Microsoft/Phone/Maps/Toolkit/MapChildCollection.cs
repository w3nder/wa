// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Maps.Toolkit.MapChildCollection
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Maps.Toolkit
{
  public sealed class MapChildCollection : 
    ObservableCollection<object>,
    IList,
    ICollection,
    ICollection<object>,
    IEnumerable<object>,
    IEnumerable
  {
    public MapChildCollection() => this.IsInternalCall = false;

    bool IList.IsReadOnly => this.IsReadOnly;

    bool ICollection<object>.IsReadOnly => this.IsReadOnly;

    internal bool IsReadOnly { get; set; }

    private bool IsInternalCall { get; set; }

    public void Add(DependencyObject dependencyObject, GeoCoordinate geoCoordinate)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException(nameof (dependencyObject));
      if (geoCoordinate == (GeoCoordinate) null)
        throw new ArgumentNullException(nameof (geoCoordinate));
      dependencyObject.SetValue(MapChild.GeoCoordinateProperty, (object) geoCoordinate);
      this.Add((object) dependencyObject);
    }

    public void Add(
      DependencyObject dependencyObject,
      GeoCoordinate geoCoordinate,
      Point positionOrigin)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException(nameof (dependencyObject));
      if (geoCoordinate == (GeoCoordinate) null)
        throw new ArgumentNullException(nameof (geoCoordinate));
      dependencyObject.SetValue(MapChild.GeoCoordinateProperty, (object) geoCoordinate);
      dependencyObject.SetValue(MapChild.PositionOriginProperty, (object) positionOrigin);
      this.Add((object) dependencyObject);
    }

    internal void AddRange(IEnumerable source)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      foreach (object obj in source)
        this.Add(obj);
    }

    internal void AddInternal(object item)
    {
      this.IsInternalCall = true;
      this.Add(item);
      this.IsInternalCall = false;
    }

    internal void InsertInternal(int index, object item)
    {
      this.IsInternalCall = true;
      base.InsertItem(index, item);
      this.IsInternalCall = false;
    }

    internal void MoveInternal(int oldIndex, int newIndex)
    {
      this.IsInternalCall = true;
      base.MoveItem(oldIndex, newIndex);
      this.IsInternalCall = false;
    }

    internal void RemoveInternal(int index)
    {
      this.IsInternalCall = true;
      base.RemoveItem(index);
      this.IsInternalCall = false;
    }

    internal void ClearInternal()
    {
      this.IsInternalCall = true;
      base.ClearItems();
      this.IsInternalCall = false;
    }

    protected override void InsertItem(int index, object item)
    {
      this.CheckCanWriteAndRaiseExceptionIfNecessary();
      base.InsertItem(index, item);
    }

    protected override void MoveItem(int oldIndex, int newIndex)
    {
      this.CheckCanWriteAndRaiseExceptionIfNecessary();
      base.MoveItem(oldIndex, newIndex);
    }

    protected override void RemoveItem(int index)
    {
      this.CheckCanWriteAndRaiseExceptionIfNecessary();
      base.RemoveItem(index);
    }

    protected override void SetItem(int index, object item)
    {
      this.CheckCanWriteAndRaiseExceptionIfNecessary();
      base.SetItem(index, item);
    }

    protected override void ClearItems()
    {
      this.CheckCanWriteAndRaiseExceptionIfNecessary();
      base.ClearItems();
    }

    private void CheckCanWriteAndRaiseExceptionIfNecessary()
    {
      if (this.IsReadOnly && !this.IsInternalCall)
        throw new InvalidOperationException("Collection is in non writeable mode");
    }
  }
}
