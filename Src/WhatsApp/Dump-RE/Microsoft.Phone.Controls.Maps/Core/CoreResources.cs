// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.CoreResources
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
namespace Microsoft.Phone.Controls.Maps.Core
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [CompilerGenerated]
  internal class CoreResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal CoreResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) CoreResources.resourceMan, (object) null))
          CoreResources.resourceMan = new ResourceManager("Microsoft.Phone.Controls.Maps.Core.CoreResources", typeof (CoreResources).Assembly);
        return CoreResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => CoreResources.resourceCulture;
      set => CoreResources.resourceCulture = value;
    }

    internal static string DefaultCopyright
    {
      get
      {
        return CoreResources.ResourceManager.GetString(nameof (DefaultCopyright), CoreResources.resourceCulture);
      }
    }
  }
}
