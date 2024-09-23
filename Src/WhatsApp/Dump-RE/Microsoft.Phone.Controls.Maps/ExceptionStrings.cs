// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ExceptionStrings
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [DebuggerNonUserCode]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [CompilerGenerated]
  internal class ExceptionStrings
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ExceptionStrings()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) ExceptionStrings.resourceMan, (object) null))
          ExceptionStrings.resourceMan = new ResourceManager("Microsoft.Phone.Controls.Maps.ExceptionStrings", typeof (ExceptionStrings).Assembly);
        return ExceptionStrings.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ExceptionStrings.resourceCulture;
      set => ExceptionStrings.resourceCulture = value;
    }

    internal static string ConfigurationException_InvalidLoad
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (ConfigurationException_InvalidLoad), ExceptionStrings.resourceCulture);
      }
    }

    internal static string ConfigurationException_NullXml
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (ConfigurationException_NullXml), ExceptionStrings.resourceCulture);
      }
    }

    internal static string InvalidMode
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (InvalidMode), ExceptionStrings.resourceCulture);
      }
    }

    internal static string LocationToViewportPoint_DefaultException
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (LocationToViewportPoint_DefaultException), ExceptionStrings.resourceCulture);
      }
    }

    internal static string MapConfiguration_GetSection_NonNull
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (MapConfiguration_GetSection_NonNull), ExceptionStrings.resourceCulture);
      }
    }

    internal static string MapConfiguration_ParseConfiguration_DuplicateNodeKey
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (MapConfiguration_ParseConfiguration_DuplicateNodeKey), ExceptionStrings.resourceCulture);
      }
    }

    internal static string MapConfiguration_ParseConfiguration_DuplicateSection
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (MapConfiguration_ParseConfiguration_DuplicateSection), ExceptionStrings.resourceCulture);
      }
    }

    internal static string MapConfiguration_ParseConfiguration_InvalidRoot
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (MapConfiguration_ParseConfiguration_InvalidRoot), ExceptionStrings.resourceCulture);
      }
    }

    internal static string MapConfiguration_ParseConfiguration_InvalidSection_NoVersion
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (MapConfiguration_ParseConfiguration_InvalidSection_NoVersion), ExceptionStrings.resourceCulture);
      }
    }

    internal static string MapConfiguration_ParseConfiguration_InvalidTag
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (MapConfiguration_ParseConfiguration_InvalidTag), ExceptionStrings.resourceCulture);
      }
    }

    internal static string MapConfiguration_WebService_InvalidResult
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (MapConfiguration_WebService_InvalidResult), ExceptionStrings.resourceCulture);
      }
    }

    internal static string TileSource_InvalidSubdomain_stringNull
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (TileSource_InvalidSubdomain_stringNull), ExceptionStrings.resourceCulture);
      }
    }

    internal static string TileSource_InvalidSubdomains_DifferentLength
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (TileSource_InvalidSubdomains_DifferentLength), ExceptionStrings.resourceCulture);
      }
    }

    internal static string TileSource_InvalidSubdomains_LengthMoreThan0
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (TileSource_InvalidSubdomains_LengthMoreThan0), ExceptionStrings.resourceCulture);
      }
    }

    internal static string TypeConverter_InvalidApplicationIdCredentialsProvider
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (TypeConverter_InvalidApplicationIdCredentialsProvider), ExceptionStrings.resourceCulture);
      }
    }

    internal static string TypeConverter_InvalidLocationCollection
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (TypeConverter_InvalidLocationCollection), ExceptionStrings.resourceCulture);
      }
    }

    internal static string TypeConverter_InvalidLocationFormat
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (TypeConverter_InvalidLocationFormat), ExceptionStrings.resourceCulture);
      }
    }

    internal static string TypeConverter_InvalidLocationRectFormat
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (TypeConverter_InvalidLocationRectFormat), ExceptionStrings.resourceCulture);
      }
    }

    internal static string TypeConverter_InvalidMapMode
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (TypeConverter_InvalidMapMode), ExceptionStrings.resourceCulture);
      }
    }

    internal static string TypeConverter_InvalidPositionOriginFormat
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (TypeConverter_InvalidPositionOriginFormat), ExceptionStrings.resourceCulture);
      }
    }

    internal static string TypeConverter_InvalidRangeFormat
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (TypeConverter_InvalidRangeFormat), ExceptionStrings.resourceCulture);
      }
    }

    internal static string ViewportPointToLocation_DefaultException
    {
      get
      {
        return ExceptionStrings.ResourceManager.GetString(nameof (ViewportPointToLocation_DefaultException), ExceptionStrings.resourceCulture);
      }
    }
  }
}
