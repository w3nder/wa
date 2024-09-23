// Decompiled with JetBrains decompiler
// Type: WhatsApp.Resolvers.CacheResolver
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp.Resolvers
{
  public class CacheResolver : IResolver
  {
    private object @lock = new object();
    private Dictionary<string, IEnumerable<ResolveResult>> cache = new Dictionary<string, IEnumerable<ResolveResult>>();
    private IResolver source;

    public string DisplayName => (string) null;

    public CacheResolver(IResolver source)
    {
      this.source = source;
      NetworkStateMonitor.Instance.Observable.Subscribe<NetworkStateChange>((Action<NetworkStateChange>) (flags =>
      {
        lock (this.@lock)
          this.cache.Clear();
      }));
    }

    private bool LoadFromCache(string host, Action<IEnumerable<ResolveResult>> onResults)
    {
      IEnumerable<ResolveResult> source = (IEnumerable<ResolveResult>) null;
      lock (this.@lock)
      {
        this.cache.TryGetValue(host, out source);
        if (source != null)
        {
          ResolveResult[] resolveResultArray = source.ToArray<ResolveResult>();
          if (resolveResultArray.Length == 0)
          {
            this.cache.Remove(host);
            resolveResultArray = (ResolveResult[]) null;
          }
          source = (IEnumerable<ResolveResult>) resolveResultArray;
        }
      }
      if (source == null)
        return false;
      onResults(source);
      return true;
    }

    private void StoreToCache(string host, IEnumerable<ResolveResult> results)
    {
      ResolveResult[] array = results.Where<ResolveResult>((Func<ResolveResult, bool>) (r =>
      {
        uint? ttl = r.Ttl;
        uint num = 0;
        return (int) ttl.GetValueOrDefault() != (int) num || !ttl.HasValue;
      })).ToArray<ResolveResult>();
      if (array.Length == 0)
        return;
      DateTime now = DateTime.Now;
      results = ((IEnumerable<ResolveResult>) array).Where<ResolveResult>((Func<ResolveResult, bool>) (r => now + TimeSpan.FromSeconds((double) Math.Min(3600U, (uint) ((int) r.Ttl ?? 3600))) > DateTime.Now));
      lock (this.@lock)
      {
        IEnumerable<ResolveResult> resolveResults = (IEnumerable<ResolveResult>) null;
        this.cache.TryGetValue(host, out resolveResults);
        if (resolveResults != null && resolveResults.Any<ResolveResult>())
          results = results.Concat<ResolveResult>(resolveResults);
        this.cache[host] = results;
      }
    }

    public void ResolveImpl(
      string host,
      Action<IEnumerable<ResolveResult>> onResults,
      Action onError)
    {
      if (this.LoadFromCache(host, onResults))
        return;
      Action<IEnumerable<ResolveResult>> innerCallback = onResults;
      onResults = (Action<IEnumerable<ResolveResult>>) (res =>
      {
        this.StoreToCache(host, res);
        innerCallback(res);
      });
      this.source.Resolve(host, onResults, onError);
    }
  }
}
