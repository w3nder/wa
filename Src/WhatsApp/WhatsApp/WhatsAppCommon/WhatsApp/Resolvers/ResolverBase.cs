// Decompiled with JetBrains decompiler
// Type: WhatsApp.Resolvers.ResolverBase
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;


namespace WhatsApp.Resolvers
{
  public abstract class ResolverBase : IResolver
  {
    public virtual string DisplayName => this.GetType().Name;

    public abstract void ResolveImpl(
      string host,
      Action<IEnumerable<ResolveResult>> onResults,
      Action onError);
  }
}
