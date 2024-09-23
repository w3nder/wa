// Decompiled with JetBrains decompiler
// Type: WhatsApp.Resolvers.SystemResolver
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

#nullable disable
namespace WhatsApp.Resolvers
{
  public class SystemResolver : ResolverBase
  {
    public override void ResolveImpl(
      string host,
      Action<IEnumerable<ResolveResult>> onResults,
      Action onError)
    {
      DeviceNetworkInformation.ResolveHostNameAsync(new DnsEndPoint(host, 80), (NameResolutionCallback) (res =>
      {
        if (res.NetworkErrorCode != NetworkError.Success)
        {
          Log.WriteLineDebug("System resolver: got error: {0}", (object) res.NetworkErrorCode);
          onError();
        }
        else
          onResults(((IEnumerable<IPEndPoint>) res.IPEndPoints).Select<IPEndPoint, ResolveResult>((Func<IPEndPoint, ResolveResult>) (r => new ResolveResult()
          {
            Address = r.Address.ToString()
          })));
      }), (object) null);
    }
  }
}
