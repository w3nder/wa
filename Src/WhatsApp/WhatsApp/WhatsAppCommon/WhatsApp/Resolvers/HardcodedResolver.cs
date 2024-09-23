// Decompiled with JetBrains decompiler
// Type: WhatsApp.Resolvers.HardcodedResolver
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp.Resolvers
{
  public class HardcodedResolver : ResolverBase
  {
    public override void ResolveImpl(
      string host,
      Action<IEnumerable<ResolveResult>> onResults,
      Action onError)
    {
      HostCollection.Entry hostByName = HostCollection.Instance.GetHostByName(host);
      if (hostByName != null)
        onResults(hostByName.Addresses.Select<string, ResolveResult>((Func<string, ResolveResult>) (addr => new ResolveResult()
        {
          Address = addr,
          Ttl = new uint?(0U)
        })));
      else
        onError();
    }
  }
}
