// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.AutomationPeers.MapBaseAutomationPeer
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.AutomationPeers
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class MapBaseAutomationPeer : BaseAutomationPeer, IValueProvider
  {
    private readonly MapBase map;

    public MapBaseAutomationPeer(MapBase element)
      : this(element, "MapBase")
    {
    }

    public MapBaseAutomationPeer(MapBase element, string className)
      : base((FrameworkElement) element, className)
    {
      this.map = element;
    }

    public override object GetPattern(PatternInterface patternInterface)
    {
      return patternInterface == PatternInterface.Value ? (object) this : base.GetPattern(patternInterface);
    }

    protected MapBase Map => this.map;

    public bool IsReadOnly => true;

    public string Value
    {
      get
      {
        if (this.map == null)
          return "Invalid map object";
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ZoomLevel={0}|Latitude={1}|Longitude={2}|Heading={3}|Pitch={4}|MapMode={5}|Width={6}|Height={7}", (object) this.map.ZoomLevel, (object) this.map.Center.Latitude, (object) this.map.Center.Longitude, (object) this.map.Heading, (object) this.map.Pitch, (object) this.map.Mode, (object) this.map.ActualWidth, (object) this.map.ActualHeight);
      }
    }

    public void SetValue(string value) => throw new NotImplementedException();
  }
}
