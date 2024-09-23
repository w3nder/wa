// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.ResourceUtility
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal static class ResourceUtility
  {
    private static readonly Dictionary<Type, Dictionary<CultureInfo, object>> cache = new Dictionary<Type, Dictionary<CultureInfo, object>>();

    private static bool TryToGetCultureInfo(string cultureName, out CultureInfo cultureInfo)
    {
      try
      {
        cultureInfo = new CultureInfo(cultureName);
      }
      catch (ArgumentException ex)
      {
        cultureInfo = (CultureInfo) null;
        return false;
      }
      return true;
    }

    internal static CultureInfo GetCultureInfo(string cultureName)
    {
      CultureInfo cultureInfo;
      if (ResourceUtility.TryToGetCultureInfo(cultureName, out cultureInfo))
        return cultureInfo;
      int length = cultureName.IndexOf('-');
      return length > 0 && ResourceUtility.TryToGetCultureInfo(cultureName.Substring(0, length), out cultureInfo) ? cultureInfo : CultureInfo.CurrentUICulture;
    }

    private static bool TryToGetRegionInfo(string regionName, out RegionInfo regionInfo)
    {
      try
      {
        regionInfo = new RegionInfo(regionName);
      }
      catch (ArgumentException ex)
      {
        regionInfo = (RegionInfo) null;
        return false;
      }
      return true;
    }

    internal static RegionInfo GetRegionInfo(string regionName)
    {
      RegionInfo regionInfo;
      return ResourceUtility.TryToGetRegionInfo(regionName, out regionInfo) ? regionInfo : RegionInfo.CurrentRegion;
    }

    internal static TResourceClass GetResource<TResourceClass, THelper>(CultureInfo culture)
      where TResourceClass : class
      where THelper : class, IResourceHelper<TResourceClass>, new()
    {
      Dictionary<CultureInfo, object> dictionary;
      if (!ResourceUtility.cache.TryGetValue(typeof (TResourceClass), out dictionary))
      {
        dictionary = new Dictionary<CultureInfo, object>();
        ResourceUtility.cache[typeof (TResourceClass)] = dictionary;
      }
      object resource1;
      if (!dictionary.TryGetValue(culture, out resource1))
      {
        THelper helper = new THelper();
        TResourceClass resource2 = helper.CreateResource();
        helper.SetResourceCulture(resource2, culture);
        dictionary[culture] = (object) resource2;
        resource1 = (object) resource2;
      }
      return resource1 as TResourceClass;
    }
  }
}
