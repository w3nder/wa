// Decompiled with JetBrains decompiler
// Type: WhatsApp.Resolvers.IpResolver
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;


namespace WhatsApp.Resolvers
{
  public class IpResolver : ResolverBase
  {
    public string Ip;

    public override void ResolveImpl(
      string host,
      Action<IEnumerable<ResolveResult>> onRes,
      Action onError)
    {
      onRes((IEnumerable<ResolveResult>) new ResolveResult[1]
      {
        new ResolveResult() { Address = this.Ip }
      });
    }
  }
}
