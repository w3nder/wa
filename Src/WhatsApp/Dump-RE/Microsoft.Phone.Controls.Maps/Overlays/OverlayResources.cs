// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Overlays.OverlayResources
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Overlays
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  internal class OverlayResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal OverlayResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) OverlayResources.resourceMan, (object) null))
          OverlayResources.resourceMan = new ResourceManager("Microsoft.Phone.Controls.Maps.Overlays.OverlayResources", typeof (OverlayResources).Assembly);
        return OverlayResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => OverlayResources.resourceCulture;
      set => OverlayResources.resourceCulture = value;
    }

    internal static string Feet
    {
      get
      {
        return OverlayResources.ResourceManager.GetString(nameof (Feet), OverlayResources.resourceCulture);
      }
    }

    internal static string InvalidCredentialsErrorMessage
    {
      get
      {
        return OverlayResources.ResourceManager.GetString(nameof (InvalidCredentialsErrorMessage), OverlayResources.resourceCulture);
      }
    }

    internal static string Kilometers
    {
      get
      {
        return OverlayResources.ResourceManager.GetString(nameof (Kilometers), OverlayResources.resourceCulture);
      }
    }

    internal static string LoadingConfigurationErrorMessage
    {
      get
      {
        return OverlayResources.ResourceManager.GetString(nameof (LoadingConfigurationErrorMessage), OverlayResources.resourceCulture);
      }
    }

    internal static string LoadingUriSchemeErrorMessage
    {
      get
      {
        return OverlayResources.ResourceManager.GetString(nameof (LoadingUriSchemeErrorMessage), OverlayResources.resourceCulture);
      }
    }

    internal static string Meters
    {
      get
      {
        return OverlayResources.ResourceManager.GetString(nameof (Meters), OverlayResources.resourceCulture);
      }
    }

    internal static string Miles
    {
      get
      {
        return OverlayResources.ResourceManager.GetString(nameof (Miles), OverlayResources.resourceCulture);
      }
    }

    internal static string Yards
    {
      get
      {
        return OverlayResources.ResourceManager.GetString(nameof (Yards), OverlayResources.resourceCulture);
      }
    }
  }
}
