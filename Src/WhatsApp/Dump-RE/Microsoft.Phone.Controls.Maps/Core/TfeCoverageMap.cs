// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.TfeCoverageMap
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
  internal sealed class TfeCoverageMap
  {
    private static readonly object requestLock = new object();
    private static readonly Dictionary<Uri, TfeCoverageMap> instances = new Dictionary<Uri, TfeCoverageMap>();
    private static readonly Dictionary<Uri, Collection<TfeCoverageMapState>> requests = new Dictionary<Uri, Collection<TfeCoverageMapState>>();
    private readonly List<TfeZoomLevelRange> levelMaps;
    private readonly string mapGeneration;
    private readonly int maxTfeZoomLevel;
    private readonly int minTfeZoomLevel;

    internal TfeCoverageMap(XmlReader reader)
    {
      this.levelMaps = new List<TfeZoomLevelRange>();
      if (reader != null)
      {
        this.Loaded = TfeCoverageMapParser.Parse(reader, this.levelMaps, out this.mapGeneration, out this.minTfeZoomLevel, out this.maxTfeZoomLevel);
        if (this.levelMaps.Count != 0)
          return;
        this.Loaded = false;
      }
      else
        this.Loaded = false;
    }

    public static void GetInstance(Uri coverageMapUri, TfeCoverageMapCallback callback)
    {
      TfeCoverageMap.GetInstance(coverageMapUri, callback, (object) null);
    }

    public static void GetInstance(
      Uri coverageMapUri,
      TfeCoverageMapCallback callback,
      object userState)
    {
      if (!(coverageMapUri != (Uri) null) || callback == null)
        return;
      bool flag = false;
      TfeCoverageMap coverageMap = (TfeCoverageMap) null;
      lock (TfeCoverageMap.requestLock)
      {
        if (TfeCoverageMap.instances.ContainsKey(coverageMapUri))
        {
          coverageMap = TfeCoverageMap.instances[coverageMapUri];
        }
        else
        {
          if (!TfeCoverageMap.requests.ContainsKey(coverageMapUri))
          {
            TfeCoverageMap.requests.Add(coverageMapUri, new Collection<TfeCoverageMapState>());
            flag = true;
          }
          TfeCoverageMap.requests[coverageMapUri].Add(new TfeCoverageMapState(callback, userState));
        }
      }
      if (coverageMap != null)
      {
        callback(coverageMap, userState);
      }
      else
      {
        if (!flag)
          return;
        WebClient webClient = new WebClient();
        webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(TfeCoverageMap.WebClient_OpenReadCompleted);
        webClient.OpenReadAsync(coverageMapUri, (object) coverageMapUri);
      }
    }

    public bool Loaded { get; private set; }

    public string MapGeneration => this.mapGeneration;

    public int MinZoomLevel => this.minTfeZoomLevel;

    public int MaxZoomLevel => this.maxTfeZoomLevel;

    public bool Covers(QuadKey quadKey) => !this.Loaded || this.GetGeneration(quadKey.Key) > 0;

    public int GetMaximumZoomLevel(QuadKey quadKey)
    {
      int maximumZoomLevel = this.minTfeZoomLevel;
      if (this.Loaded)
      {
        for (int index = this.levelMaps.Count - 1; index >= 0; --index)
        {
          if (this.levelMaps[index].MaximumZoom > maximumZoomLevel && this.levelMaps[index].GetGeneration(quadKey.ZoomLevel < this.levelMaps[index].MaximumZoom ? quadKey.Key : quadKey.Key.Substring(0, this.levelMaps[index].MaximumZoom)) > 0)
            maximumZoomLevel = this.levelMaps[index].MaximumZoom;
        }
      }
      return maximumZoomLevel;
    }

    private static void WebClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
    {
      Uri userState = e.UserState as Uri;
      if (!(userState != (Uri) null))
        return;
      TfeCoverageMap coverageMap;
      if (e.Error == null && e.Result != null)
      {
        XmlReader reader = (XmlReader) null;
        try
        {
          reader = XmlReader.Create(e.Result);
        }
        catch (XmlException ex)
        {
        }
        coverageMap = new TfeCoverageMap(reader);
        if (!coverageMap.Loaded)
          coverageMap = (TfeCoverageMap) null;
      }
      else
        coverageMap = (TfeCoverageMap) null;
      Collection<TfeCoverageMapState> collection;
      lock (TfeCoverageMap.requestLock)
      {
        if (coverageMap != null && !TfeCoverageMap.instances.ContainsKey(userState))
          TfeCoverageMap.instances.Add(userState, coverageMap);
        if (TfeCoverageMap.requests.ContainsKey(userState))
        {
          collection = TfeCoverageMap.requests[userState];
          TfeCoverageMap.requests.Remove(userState);
        }
        else
          collection = new Collection<TfeCoverageMapState>();
      }
      foreach (TfeCoverageMapState coverageMapState in collection)
        coverageMapState.Callback(coverageMap, coverageMapState.UserState);
    }

    private int GetGeneration(string quadcode)
    {
      int generation = -1;
      for (int index = 0; index < this.levelMaps.Count; ++index)
      {
        generation = this.levelMaps[index].GetGeneration(quadcode);
        if (generation > 0)
          break;
      }
      return generation;
    }
  }
}
