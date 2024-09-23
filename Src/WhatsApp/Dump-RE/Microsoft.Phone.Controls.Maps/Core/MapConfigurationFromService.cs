// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapConfigurationFromService
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.ConfigService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Windows.Threading;
using System.Xml;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class MapConfigurationFromService : MapConfigurationProvider
  {
    private readonly object configLock = new object();
    private readonly Dictionary<string, Collection<MapConfigurationGetSectionRequest>> requestQueue;
    private readonly Collection<string> requestedSections;
    private readonly MapControlConfigurationServiceClient serviceClient;
    private MapControlConfigurationRequest configRequest;
    private string configRequestKey;
    private bool detectingNetworkChanged;
    private int retryLimit = 10;
    private bool isConfigLoaded;
    private DispatcherTimer networkChangePollTimer;

    public MapConfigurationFromService(Uri address)
    {
      this.serviceClient = new MapControlConfigurationServiceClient((Binding) new CustomBinding(new BindingElement[2]
      {
        (BindingElement) new BinaryMessageEncodingBindingElement(),
        (BindingElement) new HttpTransportBindingElement()
      }), new EndpointAddress(address, new AddressHeader[0]));
      this.serviceClient.GetConfigurationCompleted += new EventHandler<GetConfigurationCompletedEventArgs>(this.ServiceClient_GetConfigurationCompleted);
      this.Sections = new Dictionary<string, MapConfigurationSection>();
      this.requestQueue = new Dictionary<string, Collection<MapConfigurationGetSectionRequest>>();
      this.requestedSections = new Collection<string>();
    }

    public override event EventHandler<MapConfigurationLoadedEventArgs> Loaded;

    public override void LoadConfiguration()
    {
    }

    public override void GetConfigurationSection(
      string version,
      string sectionName,
      string culture,
      MapConfigurationCallback callback,
      object userState)
    {
      bool flag1 = this.ContainConfigurationSection(version, sectionName, culture);
      if (!flag1)
      {
        string configurationKey = MapConfigurationProvider.GetConfigurationKey(version, sectionName, culture);
        bool flag2;
        lock (this.configLock)
        {
          flag1 = this.ContainConfigurationSection(version, sectionName, culture) || this.requestedSections.Contains(configurationKey);
          flag2 = !flag1 && !this.requestQueue.ContainsKey(configurationKey);
          if (!this.requestQueue.ContainsKey(configurationKey))
            this.requestQueue.Add(configurationKey, new Collection<MapConfigurationGetSectionRequest>());
          if (!flag1)
            this.requestQueue[configurationKey].Add(new MapConfigurationGetSectionRequest(version, sectionName, culture, callback, userState));
        }
        if (flag2)
        {
          MapControlConfigurationRequest configurationRequest = new MapControlConfigurationRequest();
          configurationRequest.ExecutionOptions = new ExecutionOptions();
          configurationRequest.ExecutionOptions.SuppressFaults = true;
          if (string.IsNullOrEmpty(culture))
            configurationRequest.Culture = culture;
          configurationRequest.Version = version;
          configurationRequest.SectionName = sectionName;
          this.configRequest = configurationRequest;
          this.configRequestKey = configurationKey;
          string filename = this.GetFilename(configurationKey);
          try
          {
            using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
            {
              if (storeForApplication.FileExists(filename))
              {
                using (IsolatedStorageFileStream input = new IsolatedStorageFileStream(filename, FileMode.Open, storeForApplication))
                {
                  try
                  {
                    this.LoadConfig(configurationKey, XmlReader.Create((Stream) input));
                  }
                  catch (XmlException ex)
                  {
                  }
                }
              }
            }
          }
          catch (IsolatedStorageException ex)
          {
          }
          this.MakeServiceRequest();
        }
      }
      if (!flag1 || callback == null)
        return;
      callback(this.GetSection(version, sectionName, culture), userState);
    }

    private void MakeServiceRequest()
    {
      if (NetworkInterface.GetIsNetworkAvailable())
      {
        --this.retryLimit;
        if (this.configRequest == null || this.configRequestKey == null)
          return;
        this.serviceClient.GetConfigurationAsync(this.configRequest, (object) this.configRequestKey);
      }
      else
      {
        if (this.isConfigLoaded || this.detectingNetworkChanged)
          return;
        this.detectingNetworkChanged = true;
        if (this.networkChangePollTimer == null)
        {
          this.networkChangePollTimer = new DispatcherTimer()
          {
            Interval = TimeSpan.FromSeconds(5.0)
          };
          this.networkChangePollTimer.Tick += new EventHandler(this.NetworkChangePollTimer_Tick);
        }
        this.networkChangePollTimer.Start();
      }
    }

    private void NetworkChangePollTimer_Tick(object sender, EventArgs e)
    {
      if (!NetworkInterface.GetIsNetworkAvailable())
        return;
      this.detectingNetworkChanged = false;
      this.networkChangePollTimer.Stop();
      this.networkChangePollTimer.Tick -= new EventHandler(this.NetworkChangePollTimer_Tick);
      this.networkChangePollTimer = (DispatcherTimer) null;
      this.MakeServiceRequest();
    }

    private void ServiceClient_GetConfigurationCompleted(
      object sender,
      GetConfigurationCompletedEventArgs e)
    {
      string userState = e.UserState as string;
      if (e.Error != null)
      {
        if (this.Loaded != null)
          this.Loaded((object) this, new MapConfigurationLoadedEventArgs(e.Error));
        if (this.retryLimit > 0)
          this.MakeServiceRequest();
        else
          this.CallRequests(userState);
      }
      else if (e.Result == null || e.Result.Configuration == null)
      {
        if (this.Loaded != null)
          this.Loaded((object) this, new MapConfigurationLoadedEventArgs((Exception) new ConfigurationNotLoadedException(ExceptionStrings.MapConfiguration_WebService_InvalidResult)));
        this.CallRequests(userState);
      }
      else
      {
        if (!this.isConfigLoaded)
          this.LoadConfig(userState, XmlReader.Create((TextReader) new StringReader(e.Result.Configuration)));
        try
        {
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            using (IsolatedStorageFileStream storageFileStream = new IsolatedStorageFileStream(this.GetFilename(userState), FileMode.Create, storeForApplication))
            {
              string configuration = e.Result.Configuration;
              try
              {
                storageFileStream.Write(new UTF8Encoding().GetBytes(configuration), 0, configuration.Length);
              }
              catch (IOException ex)
              {
              }
            }
          }
        }
        catch (IsolatedStorageException ex)
        {
        }
      }
    }

    private void LoadConfig(string requestKey, XmlReader config)
    {
      this.isConfigLoaded = true;
      Dictionary<string, MapConfigurationSection> configuration = MapConfigurationProvider.ParseConfiguration(config);
      lock (this.configLock)
      {
        foreach (string key in configuration.Keys)
        {
          if (!this.Sections.ContainsKey(key))
            this.Sections[key] = configuration[key];
        }
        this.requestedSections.Add(requestKey);
      }
      if (this.Loaded != null)
        this.Loaded((object) this, new MapConfigurationLoadedEventArgs((Exception) null));
      this.CallRequests(requestKey);
    }

    private void CallRequests(string requestKey)
    {
      if (!this.requestQueue.ContainsKey(requestKey))
        return;
      Collection<MapConfigurationGetSectionRequest> request = this.requestQueue[requestKey];
      this.requestQueue.Remove(requestKey);
      foreach (MapConfigurationGetSectionRequest getSectionRequest in request)
      {
        if (getSectionRequest.Callback != null)
          getSectionRequest.Callback(this.GetSection(getSectionRequest.Version, getSectionRequest.SectionName, getSectionRequest.Culture), getSectionRequest.UserState);
      }
    }

    private string GetFilename(string requestKey)
    {
      return "MapControlConfiguration_" + requestKey + ".xml";
    }
  }
}
