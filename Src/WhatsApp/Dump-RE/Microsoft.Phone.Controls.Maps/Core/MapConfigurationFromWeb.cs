// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapConfigurationFromWeb
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class MapConfigurationFromWeb : MapConfigurationProvider
  {
    private readonly object configLock = new object();
    private readonly Uri configurationUri;
    private Collection<MapConfigurationGetSectionRequest> requestQueue;

    public MapConfigurationFromWeb(Uri configurationUri)
    {
      this.configurationUri = configurationUri;
    }

    public override event EventHandler<MapConfigurationLoadedEventArgs> Loaded;

    public override void LoadConfiguration()
    {
      try
      {
        this.requestQueue = new Collection<MapConfigurationGetSectionRequest>();
        WebClient webClient = new WebClient();
        webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(this.LoadFromServer_OpenReadCompleted);
        webClient.OpenReadAsync(this.configurationUri);
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
      bool flag = this.Sections != null;
      if (!flag)
      {
        lock (this.configLock)
        {
          flag = this.Sections != null;
          if (!flag)
            this.requestQueue.Add(new MapConfigurationGetSectionRequest(version, sectionName, culture, callback, userState));
        }
      }
      if (!flag || callback == null)
        return;
      callback(this.GetSection(version, sectionName, culture), userState);
    }

    private void LoadFromServer_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
    {
      try
      {
        if (e.Error != null || e.Result == null)
          throw e.Error;
        Dictionary<string, MapConfigurationSection> configuration = MapConfigurationProvider.ParseConfiguration(XmlReader.Create(e.Result));
        lock (this.configLock)
          this.Sections = configuration;
        if (this.Loaded != null)
          this.Loaded((object) this, new MapConfigurationLoadedEventArgs((Exception) null));
      }
      catch (Exception ex)
      {
        lock (this.configLock)
          this.Sections = new Dictionary<string, MapConfigurationSection>();
        if (this.Loaded != null)
          this.Loaded((object) this, new MapConfigurationLoadedEventArgs(ex));
      }
      foreach (MapConfigurationGetSectionRequest request in this.requestQueue)
      {
        if (request.Callback != null)
          request.Callback(this.GetSection(request.Version, request.SectionName, request.Culture), request.UserState);
      }
      this.requestQueue = (Collection<MapConfigurationGetSectionRequest>) null;
    }
  }
}
