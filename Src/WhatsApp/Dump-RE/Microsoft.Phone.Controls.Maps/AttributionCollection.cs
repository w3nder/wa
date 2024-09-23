// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.AttributionCollection
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class AttributionCollection : ReadOnlyObservableCollection<AttributionInfo>
  {
    public AttributionCollection(ObservableCollection<AttributionInfo> list)
      : base(list)
    {
    }

    public new event NotifyCollectionChangedEventHandler CollectionChanged;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      base.OnCollectionChanged(args);
      if (this.CollectionChanged == null)
        return;
      this.CollectionChanged((object) this, args);
    }
  }
}
