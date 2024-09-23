// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.AutomationPeers.MapTileLayerAutomationPeer
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.AutomationPeers
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class MapTileLayerAutomationPeer : BaseAutomationPeer, IValueProvider
  {
    private readonly MapTileLayer tileLayer;

    public MapTileLayerAutomationPeer(FrameworkElement element)
      : base(element, "MapTileLayer")
    {
      this.tileLayer = element as MapTileLayer;
    }

    public override object GetPattern(PatternInterface patternInterface)
    {
      return patternInterface == PatternInterface.Value ? (object) this : base.GetPattern(patternInterface);
    }

    public bool IsReadOnly => true;

    public string Value
    {
      get
      {
        if (this.tileLayer == null)
          return "Invalid MapTileLayer object";
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IsIdle={0}", (object) this.tileLayer.IsIdle);
      }
    }

    public void SetValue(string value) => throw new NotImplementedException();
  }
}
