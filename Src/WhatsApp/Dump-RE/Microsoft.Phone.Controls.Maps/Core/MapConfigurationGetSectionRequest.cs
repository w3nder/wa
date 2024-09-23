// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapConfigurationGetSectionRequest
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class MapConfigurationGetSectionRequest
  {
    public MapConfigurationGetSectionRequest(
      string version,
      string sectionName,
      string culture,
      MapConfigurationCallback callback,
      object userState)
    {
      this.Version = version;
      this.SectionName = sectionName;
      this.Culture = culture;
      this.Callback = callback;
      this.UserState = userState;
    }

    public string Version { get; private set; }

    public string SectionName { get; private set; }

    public string Culture { get; private set; }

    public MapConfigurationCallback Callback { get; private set; }

    public object UserState { get; private set; }
  }
}
