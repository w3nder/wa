// Decompiled with JetBrains decompiler
// Type: WhatsApp.Resolver
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsApp.Resolvers;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class Resolver
  {
    private static IResolver instance;
    private static IHostResolver nativeInstance;
    private static IResolver uncachedInstance;
    private static IHostResolver uncachedNativeInstance;

    public static IResolver Instance
    {
      get
      {
        return Utils.LazyInit<IResolver>(ref Resolver.instance, (Func<IResolver>) (() => (IResolver) new CacheResolver(Resolver.UncachedInstance)));
      }
    }

    public static IHostResolver NativeInstance
    {
      get
      {
        return Utils.LazyInit<IHostResolver>(ref Resolver.nativeInstance, (Func<IHostResolver>) (() => Resolver.Instance.ToNativeResolver()));
      }
    }

    public static IResolver UncachedInstance
    {
      get
      {
        return Utils.LazyInit<IResolver>(ref Resolver.uncachedInstance, (Func<IResolver>) (() => (IResolver) new ChainResolver(new IResolver[4]
        {
          (IResolver) new SystemResolver(),
          (IResolver) new DnsResolver("8.8.8.8"),
          (IResolver) new DnsResolver("8.8.4.4"),
          (IResolver) new HardcodedResolver()
        })));
      }
    }

    public static IHostResolver UncachedNativeInstance
    {
      get
      {
        return Utils.LazyInit<IHostResolver>(ref Resolver.uncachedNativeInstance, (Func<IHostResolver>) (() => Resolver.UncachedInstance.ToNativeResolver()));
      }
    }
  }
}
