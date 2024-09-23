// Decompiled with JetBrains decompiler
// Type: WhatsApp.GifProviders
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public class GifProviders
  {
    private IGifProvider[] providers;
    public const int GifLoadingNumber = 12;
    private static GifProviders instance;

    private GifProviders()
    {
      this.providers = new IGifProvider[3]
      {
        (IGifProvider) new GifProviderTenor(),
        (IGifProvider) new GifProviderGiphy(),
        (IGifProvider) new GifProviderTenor()
      };
    }

    public static GifProviders Instance
    {
      get => GifProviders.instance ?? (GifProviders.instance = new GifProviders());
    }

    public IObservable<GifSearchResult> GifSearch(string query)
    {
      return this.GetCurrentProvider().GifSearch(query);
    }

    public IObservable<GifSearchResult> TrendingGifs() => this.GetCurrentProvider().TrendingGifs();

    public IObservable<GifSearchResult> LoadAdditionalGifs()
    {
      return this.GetCurrentProvider().LoadAdditionalGifs();
    }

    public IGifProvider GetWadminOverriddenProvider()
    {
      IGifProvider overriddenProvider = (IGifProvider) null;
      if (Settings.IsWaAdmin && (Settings.WaAdminForceGifProvider == 1 || Settings.WaAdminForceGifProvider == 2))
        overriddenProvider = ((IEnumerable<IGifProvider>) this.providers).ElementAtOrDefault<IGifProvider>(Settings.WaAdminForceGifProvider);
      return overriddenProvider;
    }

    public IGifProvider GetCurrentProvider(bool skipWadminOverride = false)
    {
      IGifProvider gifProvider = (IGifProvider) null;
      if (!skipWadminOverride)
        gifProvider = this.GetWadminOverriddenProvider();
      if (gifProvider == null)
        gifProvider = ((IEnumerable<IGifProvider>) this.providers).ElementAtOrDefault<IGifProvider>(Settings.GifSearchProvider);
      return gifProvider ?? ((IEnumerable<IGifProvider>) this.providers).First<IGifProvider>();
    }

    public wam_enum_gif_search_provider GetProviderForFieldStats()
    {
      return this.GetCurrentProvider() is GifProviderGiphy ? wam_enum_gif_search_provider.GIPHY : wam_enum_gif_search_provider.TENOR;
    }
  }
}
