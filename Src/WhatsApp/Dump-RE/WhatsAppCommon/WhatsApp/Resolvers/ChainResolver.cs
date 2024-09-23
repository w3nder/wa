// Decompiled with JetBrains decompiler
// Type: WhatsApp.Resolvers.ChainResolver
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Net;

#nullable disable
namespace WhatsApp.Resolvers
{
  public class ChainResolver : IResolver
  {
    private IResolver[] sources;

    public string DisplayName => (string) null;

    public ChainResolver(params IResolver[] sources) => this.sources = sources;

    public void ResolveImpl(
      string host,
      Action<IEnumerable<ResolveResult>> onResults,
      Action onError)
    {
      if (IPAddress.TryParse(host, out IPAddress _))
      {
        onResults((IEnumerable<ResolveResult>) new ResolveResult[1]
        {
          new ResolveResult() { Address = host, Ttl = new uint?(0U) }
        });
      }
      else
      {
        int i = 0;
        Action attempt = (Action) (() => { });
        attempt = (Action) (() =>
        {
          if (i < this.sources.Length)
            this.sources[i++].Resolve(host, onResults, attempt);
          else
            onError();
        });
        attempt();
      }
    }
  }
}
