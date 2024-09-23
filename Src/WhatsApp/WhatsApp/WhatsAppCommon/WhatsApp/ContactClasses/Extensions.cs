// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactClasses.Extensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp.ContactClasses
{
  public static class Extensions
  {
    public static IEnumerable<Q> SafeSelect<P, Q>(this IEnumerable<P> source, Func<P, Q> map)
    {
      return source == null ? (IEnumerable<Q>) null : source.Select<P, Q>(map);
    }

    public static IEnumerable<P> SafeWhere<P>(this IEnumerable<P> source, Func<P, bool> map)
    {
      return source == null ? (IEnumerable<P>) null : source.Where<P>(map);
    }

    public static Q WrapIfNonNull<P, Q>(this P p, Func<P, Q> map)
    {
      return (object) p == null ? default (Q) : map(p);
    }
  }
}
