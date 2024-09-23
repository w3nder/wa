// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapConfigurationFromFile
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Xml;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class MapConfigurationFromFile : MapConfigurationProvider
  {
    private readonly XmlReader config;

    public MapConfigurationFromFile(XmlReader config) => this.config = config;

    public override event EventHandler<MapConfigurationLoadedEventArgs> Loaded;

    public override void LoadConfiguration()
    {
      try
      {
        this.Sections = MapConfigurationProvider.ParseConfiguration(this.config);
        if (this.Loaded == null)
          return;
        this.Loaded((object) this, new MapConfigurationLoadedEventArgs((Exception) null));
      }
      catch (Exception ex)
      {
        if (this.Loaded == null)
          return;
        this.Loaded((object) this, new MapConfigurationLoadedEventArgs(ex));
      }
    }

    public override void GetConfigurationSection(
      string version,
      string sectionName,
      string culture,
      MapConfigurationCallback callback,
      object userState)
    {
      if (callback == null)
        return;
      callback(this.GetSection(version, sectionName, culture), userState);
    }
  }
}
