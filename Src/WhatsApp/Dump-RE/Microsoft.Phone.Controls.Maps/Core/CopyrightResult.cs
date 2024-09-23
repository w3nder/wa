// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.CopyrightResult
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class CopyrightResult
  {
    internal CopyrightResult(
      IList<string> copyrightStrings,
      CultureInfo culture,
      GeoCoordinate location,
      double zoomLevel)
    {
      this.CopyrightStrings = copyrightStrings;
      this.Culture = culture;
      this.Location = location;
      this.ZoomLevel = zoomLevel;
    }

    public IList<string> CopyrightStrings { get; private set; }

    public CultureInfo Culture { get; private set; }

    public GeoCoordinate Location { get; private set; }

    public double ZoomLevel { get; private set; }
  }
}
