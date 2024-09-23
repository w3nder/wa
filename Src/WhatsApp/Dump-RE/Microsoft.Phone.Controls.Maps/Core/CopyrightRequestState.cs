// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.CopyrightRequestState
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Device.Location;
using System.Globalization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class CopyrightRequestState
  {
    internal CopyrightRequestState(
      CultureInfo _culture,
      MapStyle _style,
      GeoCoordinate _location,
      double _zoomLevel,
      Credentials _credentials,
      Action<CopyrightResult> _copyrightCallback)
    {
      this.Culture = _culture;
      this.Style = _style;
      this.Location = _location;
      this.ZoomLevel = _zoomLevel;
      this.Credentials = _credentials;
      this.CopyrightCallback = _copyrightCallback;
    }

    internal CultureInfo Culture { get; private set; }

    internal MapStyle Style { get; private set; }

    internal GeoCoordinate Location { get; private set; }

    internal double ZoomLevel { get; private set; }

    internal Credentials Credentials { get; private set; }

    internal Action<CopyrightResult> CopyrightCallback { get; private set; }
  }
}
