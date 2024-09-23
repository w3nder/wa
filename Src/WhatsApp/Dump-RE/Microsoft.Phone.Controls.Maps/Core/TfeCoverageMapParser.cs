// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.TfeCoverageMapParser
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal static class TfeCoverageMapParser
  {
    public static bool Parse(
      XmlReader reader,
      List<TfeZoomLevelRange> maps,
      out string mapGeneration,
      out int minTfeZoomLevel,
      out int maxTfeZoomLevel)
    {
      bool flag = false;
      mapGeneration = (string) null;
      minTfeZoomLevel = -1;
      maxTfeZoomLevel = -1;
      try
      {
        flag = TfeCoverageMapParser.ParseXml(reader, maps, out mapGeneration, out minTfeZoomLevel, out maxTfeZoomLevel);
      }
      catch (XmlException ex)
      {
      }
      return flag;
    }

    private static bool ParseXml(
      XmlReader xml,
      List<TfeZoomLevelRange> maps,
      out string mapGeneration,
      out int minTfeZoomLevel,
      out int maxTfeZoomLevel)
    {
      bool xml1 = true;
      mapGeneration = "1";
      minTfeZoomLevel = int.MaxValue;
      maxTfeZoomLevel = int.MinValue;
      TfeZoomLevelRange currentMap = (TfeZoomLevelRange) null;
      while (xml.Read() && xml1)
      {
        switch (xml.NodeType)
        {
          case XmlNodeType.Element:
            switch (xml.Name)
            {
              case "MapGeneration":
                mapGeneration = xml.ReadElementContentAsString();
                continue;
              case "ZoomLevelRange":
                xml1 = TfeCoverageMapParser.ParseZoomLevelRange(xml, out currentMap);
                continue;
              case "R":
                xml1 = TfeCoverageMapParser.ParseRegion(xml, currentMap);
                continue;
              default:
                continue;
            }
          case XmlNodeType.EndElement:
            switch (xml.Name)
            {
              case "ZoomLevelRange":
                maps.Add(currentMap);
                if (currentMap.MinimumZoom < minTfeZoomLevel)
                  minTfeZoomLevel = currentMap.MinimumZoom;
                if (currentMap.MaximumZoom > maxTfeZoomLevel)
                  maxTfeZoomLevel = currentMap.MaximumZoom;
                currentMap = (TfeZoomLevelRange) null;
                continue;
              default:
                continue;
            }
          default:
            continue;
        }
      }
      return xml1;
    }

    private static bool ParseZoomLevelRange(XmlReader xml, out TfeZoomLevelRange currentMap)
    {
      bool zoomLevelRange = true;
      int minZoomLevel = 0;
      int maxZoomLevel = 0;
      currentMap = (TfeZoomLevelRange) null;
      while (xml.MoveToNextAttribute())
      {
        switch (xml.Name)
        {
          case "min":
            minZoomLevel = xml.ReadContentAsInt();
            continue;
          case "max":
            maxZoomLevel = xml.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      if (minZoomLevel != 0 && maxZoomLevel != 0)
        currentMap = new TfeZoomLevelRange((byte) minZoomLevel, (byte) maxZoomLevel);
      else
        zoomLevelRange = false;
      return zoomLevelRange;
    }

    private static bool ParseRegion(XmlReader xml, TfeZoomLevelRange currentMap)
    {
      bool region = false;
      string basequadKey = (string) null;
      int generation = 0;
      if (currentMap != null)
      {
        while (xml.MoveToNextAttribute())
        {
          switch (xml.Name)
          {
            case "c":
              basequadKey = xml.ReadContentAsString();
              continue;
            case "g":
              generation = xml.ReadContentAsInt();
              continue;
            default:
              continue;
          }
        }
        if (basequadKey != null && generation != 0)
        {
          region = true;
          currentMap.AddRegion(basequadKey, generation);
        }
      }
      return region;
    }
  }
}
