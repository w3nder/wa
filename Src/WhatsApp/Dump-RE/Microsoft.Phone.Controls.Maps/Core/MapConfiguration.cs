// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapConfiguration
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.ComponentModel;
using System.IO;
using System.Xml;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public static class MapConfiguration
  {
    private static MapConfigurationProvider configuration;
    private static string defaultServiceUriFormat = "{UriScheme}://dev.virtualearth.net/webservices/v1/MapControlConfigurationService/MapControlConfigurationService.svc/binaryHttp";

    public static event EventHandler<MapConfigurationLoadedEventArgs> Loaded;

    public static void SetServiceUri(Uri address)
    {
      MapConfiguration.LoadConfiguration((MapConfigurationProvider) new MapConfigurationFromService(address));
    }

    public static void Load(XmlReader config)
    {
      MapConfiguration.LoadConfiguration((MapConfigurationProvider) new MapConfigurationFromFile(config));
    }

    public static void Load(Uri address)
    {
      MapConfiguration.LoadConfiguration((MapConfigurationProvider) new MapConfigurationFromWeb(address));
    }

    public static void GetSection(
      string version,
      string sectionName,
      string culture,
      MapConfigurationCallback callback)
    {
      MapConfiguration.GetSection(version, sectionName, culture, callback, (object) null);
    }

    public static void GetSection(
      string version,
      string sectionName,
      string culture,
      MapConfigurationCallback callback,
      object userState)
    {
      if (MapConfiguration.configuration == null)
      {
        if (DesignerProperties.IsInDesignTool)
        {
          try
          {
            Stream manifestResourceStream = typeof (MapConfiguration).Assembly.GetManifestResourceStream("Microsoft.Phone.Controls.Maps.Data.DesignConfig.xml");
            if (manifestResourceStream != null)
              MapConfiguration.LoadConfiguration((MapConfigurationProvider) new MapConfigurationFromFile(XmlReader.Create(manifestResourceStream)));
          }
          catch (XmlException ex)
          {
          }
        }
        else
          MapConfiguration.LoadConfiguration((MapConfigurationProvider) new MapConfigurationFromService(new Uri(MapConfiguration.defaultServiceUriFormat.Replace("{UriScheme}", "HTTP"), UriKind.Absolute)));
      }
      if (string.IsNullOrEmpty(version))
        throw new ArgumentException(ExceptionStrings.MapConfiguration_GetSection_NonNull, nameof (version));
      if (string.IsNullOrEmpty(sectionName))
        throw new ArgumentException(ExceptionStrings.MapConfiguration_GetSection_NonNull, nameof (sectionName));
      MapConfiguration.configuration.GetConfigurationSection(version, sectionName, culture, callback, userState);
    }

    internal static void Reset()
    {
      MapConfiguration.Loaded = (EventHandler<MapConfigurationLoadedEventArgs>) null;
      if (MapConfiguration.configuration == null)
        return;
      MapConfiguration.configuration.Loaded -= new EventHandler<MapConfigurationLoadedEventArgs>(MapConfiguration.provider_Loaded);
      MapConfiguration.configuration = (MapConfigurationProvider) null;
    }

    private static void LoadConfiguration(MapConfigurationProvider provider)
    {
      MapConfiguration.configuration = MapConfiguration.configuration == null ? provider : throw new ConfigurationNotLoadedException(ExceptionStrings.ConfigurationException_InvalidLoad);
      provider.Loaded += new EventHandler<MapConfigurationLoadedEventArgs>(MapConfiguration.provider_Loaded);
      provider.LoadConfiguration();
    }

    private static void provider_Loaded(object sender, MapConfigurationLoadedEventArgs e)
    {
      EventHandler<MapConfigurationLoadedEventArgs> loaded = MapConfiguration.Loaded;
      if (loaded == null)
        return;
      loaded((object) null, e);
    }
  }
}
