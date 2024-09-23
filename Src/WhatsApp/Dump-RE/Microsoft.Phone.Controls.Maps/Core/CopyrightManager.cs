// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.CopyrightManager
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.PlatformServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.Globalization;
using System.Net.NetworkInformation;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class CopyrightManager
  {
    private static CopyrightManager instance;
    private static readonly Dictionary<CultureInfo, string> defaultCopyrightCache = new Dictionary<CultureInfo, string>();
    private readonly Dictionary<CopyrightKey, List<CopyrightRequestState>> fetching = new Dictionary<CopyrightKey, List<CopyrightRequestState>>((IEqualityComparer<CopyrightKey>) new CopyrightKeyComparer());
    private readonly Dictionary<CopyrightKey, Collection<ImageryProvider>> imageryProviders = new Dictionary<CopyrightKey, Collection<ImageryProvider>>((IEqualityComparer<CopyrightKey>) new CopyrightKeyComparer());
    private readonly TimeSpan minimumRetryInterval = new TimeSpan(0, 2, 0);
    private readonly Dictionary<CopyrightKey, BadFetchState> retryFailedFetchAt = new Dictionary<CopyrightKey, BadFetchState>((IEqualityComparer<CopyrightKey>) new CopyrightKeyComparer());
    private ImageryServiceClient client;

    private CopyrightManager()
    {
      MapConfiguration.GetSection("v1", "WP7SLMapControl", (string) null, new MapConfigurationCallback(this.AsynchronousConfigurationLoaded));
    }

    private ImageryServiceClient Client => this.client;

    private void AsynchronousConfigurationLoaded(MapConfigurationSection config, object userState)
    {
      if (config == null || !config.Contains("ImageryServiceUri"))
        return;
      this.client = WebServicesUtility.CreateImageryServiceClient(config["ImageryServiceUri"]);
      this.client.GetImageryMetadataCompleted += new EventHandler<GetImageryMetadataCompletedEventArgs>(this.clientGetImageryMetadataCompleted);
    }

    public static CopyrightManager GetInstance()
    {
      if (CopyrightManager.instance == null)
        CopyrightManager.instance = new CopyrightManager();
      return CopyrightManager.instance;
    }

    internal static CopyrightManager GetCleanInstance() => new CopyrightManager();

    private static string DefaultCopyright(CultureInfo culture)
    {
      if (!CopyrightManager.defaultCopyrightCache.ContainsKey(culture))
      {
        string str = string.Format((IFormatProvider) culture, CoreResources.DefaultCopyright, (object) DateTime.Now.Year);
        CopyrightManager.defaultCopyrightCache[culture] = str;
      }
      return CopyrightManager.defaultCopyrightCache[culture];
    }

    private IList<string> GetCopyrightStrings(
      CopyrightKey copyrightKey,
      GeoCoordinate location,
      double zoomLevel)
    {
      List<string> copyrightStrings = new List<string>();
      List<string> collection = new List<string>();
      Collection<ImageryProvider> imageryProvider1 = this.imageryProviders.ContainsKey(copyrightKey) ? this.imageryProviders[copyrightKey] : (Collection<ImageryProvider>) null;
      if (imageryProvider1 == null)
      {
        copyrightStrings.Add(CopyrightManager.DefaultCopyright(copyrightKey.Culture));
      }
      else
      {
        int num = (int) Math.Round(zoomLevel);
        foreach (ImageryProvider imageryProvider2 in imageryProvider1)
        {
          foreach (CoverageArea coverageArea in imageryProvider2.CoverageAreas)
          {
            RangeOfint zoomRange = coverageArea.ZoomRange;
            if (zoomRange.From <= num && num <= zoomRange.To)
            {
              Rectangle boundingRectangle = coverageArea.BoundingRectangle;
              if (boundingRectangle.Southwest.Latitude <= location.Latitude && location.Latitude <= boundingRectangle.Northeast.Latitude && boundingRectangle.Southwest.Longitude <= location.Longitude && location.Longitude <= boundingRectangle.Northeast.Longitude)
              {
                if (imageryProvider2.Attribution[0] == '©')
                {
                  copyrightStrings.Add(imageryProvider2.Attribution);
                  break;
                }
                collection.Add(imageryProvider2.Attribution);
                break;
              }
            }
          }
        }
      }
      copyrightStrings.AddRange((IEnumerable<string>) collection);
      return (IList<string>) copyrightStrings;
    }

    private void SetFetchFailed(
      CopyrightKey copyrightKey,
      CopyrightRequestState requestState,
      bool failed)
    {
      if (failed)
      {
        this.retryFailedFetchAt[copyrightKey] = new BadFetchState(DateTime.Now + this.minimumRetryInterval, requestState.Credentials, requestState.Location);
      }
      else
      {
        if (!this.retryFailedFetchAt.ContainsKey(copyrightKey))
          return;
        this.retryFailedFetchAt.Remove(copyrightKey);
      }
    }

    private bool ReadyToFetch(CopyrightKey copyrightKey, CopyrightRequestState requestState)
    {
      if (this.Client == null)
        return false;
      if (!this.retryFailedFetchAt.ContainsKey(copyrightKey))
        return true;
      BadFetchState badFetchState = this.retryFailedFetchAt[copyrightKey];
      return DateTime.Now >= badFetchState.TryAgainAt || requestState.Credentials != badFetchState.CredentialsLastUsed;
    }

    internal void RequestCopyrightString(
      Microsoft.Phone.Controls.Maps.MapStyle style,
      GeoCoordinate location,
      double zoomLevel,
      CredentialsProvider credentialsProvider,
      CultureInfo culture,
      Action<CopyrightResult> copyrightCallback)
    {
      if (credentialsProvider != null)
      {
        credentialsProvider.GetCredentials((Action<Microsoft.Phone.Controls.Maps.Credentials>) (credentials => this.RequestCopyrightString(style, location, zoomLevel, credentials, culture, copyrightCallback)));
      }
      else
      {
        Microsoft.Phone.Controls.Maps.Credentials credentials = (Microsoft.Phone.Controls.Maps.Credentials) null;
        this.RequestCopyrightString(style, location, zoomLevel, credentials, culture, copyrightCallback);
      }
    }

    private void RequestCopyrightString(
      Microsoft.Phone.Controls.Maps.MapStyle style,
      GeoCoordinate location,
      double zoomLevel,
      Microsoft.Phone.Controls.Maps.Credentials credentials,
      CultureInfo culture,
      Action<CopyrightResult> copyrightCallback)
    {
      bool flag = DesignerProperties.IsInDesignTool || !NetworkInterface.GetIsNetworkAvailable();
      CopyrightKey copyrightKey = new CopyrightKey(culture, style);
      CopyrightRequestState copyrightRequestState = new CopyrightRequestState(culture, style, location, zoomLevel, credentials, copyrightCallback);
      if (flag || !this.ReadyToFetch(copyrightKey, copyrightRequestState))
        copyrightCallback(new CopyrightResult((IList<string>) new List<string>()
        {
          CopyrightManager.DefaultCopyright(culture)
        }, culture, location, zoomLevel));
      else if (this.imageryProviders.ContainsKey(copyrightKey))
        copyrightCallback(new CopyrightResult(this.GetCopyrightStrings(copyrightKey, location, zoomLevel), culture, location, zoomLevel));
      else if (this.fetching.ContainsKey(copyrightKey))
      {
        this.fetching[copyrightKey].Add(copyrightRequestState);
      }
      else
      {
        this.fetching.Add(copyrightKey, new List<CopyrightRequestState>()
        {
          copyrightRequestState
        });
        ImageryMetadataRequest request = new ImageryMetadataRequest();
        request.Culture = culture.Name;
        switch (style)
        {
          case Microsoft.Phone.Controls.Maps.MapStyle.Aerial:
            request.Style = Microsoft.Phone.Controls.Maps.PlatformServices.MapStyle.Aerial;
            break;
          case Microsoft.Phone.Controls.Maps.MapStyle.AerialWithLabels:
            request.Style = Microsoft.Phone.Controls.Maps.PlatformServices.MapStyle.AerialWithLabels;
            break;
          default:
            request.Style = Microsoft.Phone.Controls.Maps.PlatformServices.MapStyle.Road;
            break;
        }
        request.Options = new ImageryMetadataOptions();
        request.Options.ReturnImageryProviders = true;
        request.ExecutionOptions = new ExecutionOptions();
        request.ExecutionOptions.SuppressFaults = true;
        if (credentials != null)
          request.Credentials = new Microsoft.Phone.Controls.Maps.PlatformServices.Credentials()
          {
            ApplicationId = credentials.ApplicationId
          };
        this.Client.GetImageryMetadataAsync(request, (object) copyrightRequestState);
      }
    }

    private void clientGetImageryMetadataCompleted(
      object sender,
      GetImageryMetadataCompletedEventArgs e)
    {
      bool flag1 = false;
      CopyrightRequestState userState = e.UserState as CopyrightRequestState;
      CopyrightKey copyrightKey = new CopyrightKey(userState.Culture, userState.Style);
      try
      {
        bool flag2 = e.Error == null && e.Result != null && e.Result.Results != null && e.Result.Results.Count > 0;
        this.SetFetchFailed(copyrightKey, userState, !flag2);
        if (flag2)
          this.imageryProviders[copyrightKey] = e.Result.Results[0].ImageryProviders;
        List<CopyrightRequestState> copyrightRequestStateList = new List<CopyrightRequestState>((IEnumerable<CopyrightRequestState>) this.fetching[copyrightKey]);
        flag1 = this.fetching.Remove(copyrightKey);
        foreach (CopyrightRequestState copyrightRequestState in copyrightRequestStateList)
          copyrightRequestState.CopyrightCallback(new CopyrightResult(this.GetCopyrightStrings(copyrightKey, copyrightRequestState.Location, copyrightRequestState.ZoomLevel), copyrightRequestState.Culture, copyrightRequestState.Location, copyrightRequestState.ZoomLevel));
      }
      finally
      {
        if (!flag1)
          this.fetching.Remove(copyrightKey);
      }
    }
  }
}
