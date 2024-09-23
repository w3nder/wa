// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapConfigurationProvider
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal abstract class MapConfigurationProvider
  {
    private const string RootNodeName = "configuration";
    private const string versionAttribute = "version";
    private const string cultureAttribute = "culture";
    private const string PropertyNodeName = "add";
    private const string KeyAttribute = "key";
    private const string ValueAttribute = "value";

    public abstract event EventHandler<MapConfigurationLoadedEventArgs> Loaded;

    public abstract void LoadConfiguration();

    public abstract void GetConfigurationSection(
      string version,
      string sectionName,
      string culture,
      MapConfigurationCallback callback,
      object userState);

    protected Dictionary<string, MapConfigurationSection> Sections { get; set; }

    protected static string GetConfigurationKey(string version, string sectionName, string culture)
    {
      if (string.IsNullOrEmpty(culture))
        culture = string.Empty;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_{2}", (object) version, (object) sectionName, (object) culture).ToUpper(CultureInfo.InvariantCulture);
    }

    protected MapConfigurationSection GetSection(
      string version,
      string sectionName,
      string culture)
    {
      MapConfigurationSection section = (MapConfigurationSection) null;
      if (this.Sections != null)
      {
        string configurationKey1 = MapConfigurationProvider.GetConfigurationKey(version, sectionName, culture);
        if (this.Sections.ContainsKey(configurationKey1))
          section = this.Sections[configurationKey1];
        else if (!string.IsNullOrEmpty(culture))
        {
          string configurationKey2 = MapConfigurationProvider.GetConfigurationKey(version, sectionName, string.Empty);
          if (this.Sections.ContainsKey(configurationKey2))
            section = this.Sections[configurationKey2];
        }
      }
      return section;
    }

    protected bool ContainConfigurationSection(string version, string sectionName, string culture)
    {
      return this.Sections != null && this.Sections.ContainsKey(MapConfigurationProvider.GetConfigurationKey(version, sectionName, culture));
    }

    protected static Dictionary<string, MapConfigurationSection> ParseConfiguration(
      XmlReader sectionReader)
    {
      if (sectionReader == null)
        throw new ConfigurationNotLoadedException(ExceptionStrings.ConfigurationException_NullXml);
      Dictionary<string, MapConfigurationSection> configuration = new Dictionary<string, MapConfigurationSection>();
      if (sectionReader.Read() && sectionReader.IsStartElement() && sectionReader.LocalName == "configuration")
      {
        while (sectionReader.Read())
        {
          if (sectionReader.IsStartElement() && sectionReader.LocalName != "configuration")
          {
            string version = sectionReader["version"];
            string localName = sectionReader.LocalName;
            string culture = sectionReader["culture"];
            if (string.IsNullOrEmpty(localName) || string.IsNullOrEmpty(version))
              throw new XmlException(ExceptionStrings.MapConfiguration_ParseConfiguration_InvalidSection_NoVersion);
            string configurationKey = MapConfigurationProvider.GetConfigurationKey(version, localName, culture);
            if (!configuration.ContainsKey(configurationKey))
              configuration.Add(configurationKey, MapConfigurationProvider.ParseConfigurationSection(sectionReader.ReadSubtree()));
            else
              throw new XmlException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStrings.MapConfiguration_ParseConfiguration_DuplicateSection, (object) localName, (object) version, (object) culture));
          }
        }
        return configuration;
      }
      throw new XmlException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStrings.MapConfiguration_ParseConfiguration_InvalidRoot, (object) sectionReader.LocalName));
    }

    private static MapConfigurationSection ParseConfigurationSection(XmlReader sectionReader)
    {
      Dictionary<string, string> values = new Dictionary<string, string>();
      if (sectionReader.Read() && !sectionReader.IsEmptyElement)
      {
        while (sectionReader.Read())
        {
          if (sectionReader.IsStartElement())
          {
            string key = sectionReader.LocalName == "add" ? sectionReader["key"].ToUpper(CultureInfo.InvariantCulture) : throw new XmlException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStrings.MapConfiguration_ParseConfiguration_InvalidTag, (object) sectionReader.LocalName));
            string str = sectionReader["value"];
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(str))
            {
              if (!values.ContainsKey(key))
                values.Add(key, str);
              else
                throw new XmlException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStrings.MapConfiguration_ParseConfiguration_DuplicateNodeKey, (object) key));
            }
          }
        }
      }
      return new MapConfigurationSection(values);
    }
  }
}
