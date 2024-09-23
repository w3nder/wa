// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.LocationCollection
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.Design;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [TypeConverter(typeof (LocationCollectionConverter))]
  public class LocationCollection : ObservableCollection<GeoCoordinate>
  {
  }
}
